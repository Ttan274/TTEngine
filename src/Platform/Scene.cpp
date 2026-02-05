#include "Platform/Scene.h"
#include "Core/Log.h"
#include "Core/Math/Collision.h"
#include "Platform/LevelManager.h"

namespace EnginePlatform
{
	//Level Transition constants
	constexpr float LEVEL_COMPLETE_DELAY = 0.8f;
	constexpr float FADE_SPEED = 1.5f;
	constexpr float TEXT_POP_SPEED = 6.0f;
	constexpr float TEXT_MAX_SCALE = 1.2f;

	Scene::Scene()
		: m_Camera(800.0f, 600.0f)
	{
	}

	void Scene::Load()
	{
		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Scene loading started"
		);

		ChangeGameState(GameState::MainMenu);
		m_Loader.LoadBasics(m_EntityDefs);
		m_InteractableManager.SetOnLevelComplete([this]()
		{
			OnLevelCompleted();;
		});
	}

	void Scene::Update(float dt)
	{
		switch (m_GameState)
		{
		case GameState::Playing:
			UpdatePlaying(dt);
			break;
		case GameState::LevelComplete:
			UpdateLevelComplete(dt);
			break;
		}
	}

	void Scene::ChangeGameState(GameState newState)
	{
		m_GameState = newState;
	}

	void Scene::UpdateLevelComplete(float dt)
	{
		m_LevelCompleteTimer += dt;
		m_LevelTextTimer += dt;

		//Text Scale
		m_LevelTextScale += dt * TEXT_POP_SPEED;
		if (m_LevelTextScale > TEXT_MAX_SCALE)
			m_LevelTextScale = TEXT_MAX_SCALE;

		//Delay
		if (m_LevelCompleteTimer < LEVEL_COMPLETE_DELAY)
			return;

		if (m_FadeOut)	//Fade Out
		{
			m_FadeAlpha += dt * FADE_SPEED;
			if (m_FadeAlpha >= 1.0f)
			{
				m_FadeAlpha = 1.0f;
				m_FadeOut = false;
			}
		}
		else			//Fade IN
		{
			m_FadeAlpha -= dt * FADE_SPEED;
			if (m_FadeAlpha <= 0.0f)
			{
				m_FadeAlpha = 0.0f;
				LoadCurrentLevel();
				ChangeGameState(GameState::Playing);
			}
		}
	}

	void Scene::UpdatePlaying(float dt)
	{
		if (m_Player.IsDead())
		{
			ChangeGameState(GameState::DeathScreen);
			return;
		}

		m_Player.Update(dt);
		m_InteractableManager.Update(m_Player);

		if (m_InteractableManager.HasInteractableInRange())
		{
			const auto it = m_InteractableManager.GetPosition();
			
			float screenX = it.x - m_Camera.GetX();
			float screenY = it.y - m_Camera.GetY() - 20.0f;

			m_HUD.SetInteractPopup(true, screenX, screenY);

			if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::F5))
				m_InteractableManager.HandleInteraction(m_Player);
		}
		else
		{
			m_HUD.SetInteractPopup(false, 0, 0);
		}

		for (auto it = m_Enemies.begin(); it != m_Enemies.end();)
		{
			auto& e = *it;

			//Enemy AI update
			e->Update(dt, m_Player.GetPosition(), m_Player.GetCollider());

			//Enemy died
			if (e->IsDead())
			{
				it = m_Enemies.erase(it);
				continue;
			}

			//Enemy attack behaviour
			if (!m_Player.IsDead())
			{
				const EngineCore::Rect eAttackBox = e->GetAttackBox();
				if (EngineMath::RectIntersectsAABB(eAttackBox, m_Player.GetCollider()))
				{
					if (e->CanDealDamage() && !e->HasHitThisAttack() && e->IsDamageFrame())
					{
						m_Player.TakeDamage(e->GetAttackDamage(), e->IsFacingRight());
						m_Camera.StartShake(0.15f, 6.0f);
						e->MarkHitDone();
					}
				}
			}

			//Player attack behaviour
			const EngineCore::Rect pAttackBox = m_Player.GetAttackBox();
			if (EngineMath::RectIntersectsAABB(pAttackBox, e->GetCollider()))
			{
				if (m_Player.IsAttacking() && m_Player.IsDamageFrame() && !m_Player.HasHitThisAttack())
				{
					e->TakeDamage(m_Player.GetAttackDamage(), m_Player.IsFacingRight());
					m_Player.MarkHitDone();
				}
			}
			++it;
		}

		m_Camera.FollowSmooth(
			m_Player.GetPosition().x,
			m_Player.GetPosition().y,
			dt
		);
		m_Camera.UpdateShake(dt);
	}

	//UI
	void Scene::StartGame()
	{
		ChangeGameState(GameState::LevelComplete);
		//LoadCurrentLevel();
	}

	void Scene::MainMenu()
	{
		ChangeGameState(GameState::MainMenu);
	}

	//Render
	void Scene::Render(EngineCore::IRenderer* renderer)
	{
		switch (m_GameState)
		{
		case GameState::Playing:
			m_TileMap->Draw(renderer, m_Camera);
			m_TileMap->DrawCollisionDebug(renderer, m_Camera);
			m_Player.Render(renderer, m_Camera);
			m_InteractableManager.Render(renderer, m_Camera);
			m_InteractableManager.DebugDraw(renderer, m_Camera);
			for (auto& e : m_Enemies)
				e->Render(renderer, m_Camera);
			break;
		default:
			break;
		}

		m_HUD.Render(renderer, m_Player, m_Enemies, m_Camera, *this, m_GameState, m_FadeAlpha);
	}

	//Level Area
	void Scene::LoadCurrentLevel()
	{
		LoadContext ctx = GetLoadContext();
		m_Loader.LoadCurrentLevel(ctx);
	}

	void Scene::OnLevelCompleted()
	{
		m_HUD.SetInteractPopup(false, 0, 0);
		LevelManager::Get().LoadNextLevel();

		if (LevelManager::Get().GetCurrentLevel() != nullptr)
		{
			ChangeGameState(GameState::LevelComplete);
			m_LevelCompleteTimer = 0.0f;
			m_FadeOut = true;
		}
		else
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"All levels have been completed"
			);

			//Game Completed State eklenicek
			ChangeGameState(GameState::MainMenu);
		}
	}

	LoadContext Scene::GetLoadContext()
	{
		return LoadContext(
			m_Player,
			m_Enemies,
			m_EntityDefs,
			m_TileMap,
			m_MapData,
			m_InteractableManager,
			m_Camera,
			m_PlayerSpawned,
			m_LevelCompleted
		);
	}
}