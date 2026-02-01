#include "Platform/Scene.h"
#include "Platform/AnimationLibrary.h"
#include "Core/Log.h"
#include "Core/PathUtil.h"
#include "Core/Math/Collision.h"
#include "Platform/LevelManager.h"

namespace EnginePlatform
{
	//HP bar constants
	const int HP_BAR_X = 20;
	const int HP_BAR_Y = 20;
	const int HP_BAR_W = 200;
	const int HP_BAR_H = 20;
	const int HP_BAR_W_EN = 40;
	const int HP_BAR_H_EN = 6;

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

		//Animation Library loaded
		std::string animDir = EngineCore::GetRootDirectory() + "\\Assets\\Animation";
		if (!EnginePlatform::AnimationLibrary::LoadAllAnims(animDir))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Fatal,
				EngineCore::LogCategory::Scene,
				"Failed to load all animation files"
			);
			return;
		}

		//Entity Definitions loaded
		if (!EngineGame::MapLoader::LoadEntityDefs(EngineCore::GetFile("Data", "entity_def.json"),
												   m_EntityDefs))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Fatal,
				EngineCore::LogCategory::Scene,
				"Failed to load entity.json"
			);
			return;
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Scene basics have been loaded"
		);
	}

	void Scene::LoadSpawnEntities()
	{
		std::string rootDir = EngineCore::GetRootDirectory();
		m_AliveEnemyCount = 0;

		for (const auto& spawn : m_MapData.spawns)
		{
			auto it = m_EntityDefs.find(spawn.defId);
			if (it == m_EntityDefs.end())
			{
				EngineCore::Log::Write(
					EngineCore::LogLevel::Warning,
					EngineCore::LogCategory::Scene,
					"Unknown entity def : " + spawn.defId
				);
				continue;
			}

			const EngineGame::EntityDefs& def = it->second;

			//Selecting is it player or enemy
			if (spawn.defId == "Player")
			{
				if (m_PlayerSpawned)
				{
					EngineCore::Log::Write(
						EngineCore::LogLevel::Warning,
						EngineCore::LogCategory::Scene,
						"Multiple player spawns found iggnoring the extra one"
					);
					continue;
				}

				LoadPlayer(spawn, def);
				m_PlayerSpawned = true;
			}
			else
			{
				LoadEnemy(spawn, def);
				m_AliveEnemyCount++;
			}
		}

		if (!m_PlayerSpawned)
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Fatal,
				EngineCore::LogCategory::Scene,
				"No Player spawn found in map!"
			);
		}
	}

	void Scene::LoadPlayer(const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def)
	{
		//Set World Reference
		m_Player.SetWorld(m_TileMap.get());

		//Set Definitions
		m_Player.ApplyDefinition(def);

		//Set Position
		EngineMath::Vector2 newPos;
		newPos.x = spawn.x * m_TileMap->GetTileSize();
		newPos.y = spawn.y * m_TileMap->GetTileSize();

		EngineCore::AABB spawnBox;
		spawnBox.SetFromPositionSize(
			newPos.x,
			newPos.y,
			m_Player.GetColliderW(),
			m_Player.GetColliderH()
		);

		//Wall Overlap Check
		if (m_TileMap->IsSolidX(spawnBox) || m_TileMap->IsSolidY(spawnBox, -1.0f))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Warning,
				EngineCore::LogCategory::Scene,
				"Player spawn is on a wall tile!"
			);
		}

		//Set position and spawn point
		m_Player.SetPosition(newPos);
		m_Player.SetSpawnPoint(newPos);
		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Player pos : " + std::to_string(newPos.x) + "," + std::to_string(newPos.y)
		);
	}

	void Scene::LoadEnemy(const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def)
	{
		//Enemy Loading
		auto enemy = std::make_unique<EngineGame::Enemy>();

		//Set World Reference
		enemy->SetWorld(m_TileMap.get());

		//Set Definitions
		enemy->ApplyDefinition(def);

		//Set Position
		EngineMath::Vector2 newPos;
		newPos.x = spawn.x * m_TileMap->GetTileSize();
		newPos.y = spawn.y * m_TileMap->GetTileSize();
		enemy->SetPosition(newPos);

		//Set Death Callback
		enemy->OnKilled = [this]()
			{
				OnEnemyKilled();
			};

		//Add new enemy to the array
		m_Enemies.push_back(std::move(enemy));
	}

	void Scene::LoadCamera()
	{
		//Camera loading
		m_Camera.SetWorldBounds(m_TileMap->GetWorldWidth(), m_TileMap->GetWorldHeight());
		m_Camera.Follow(m_Player.GetPosition().x, m_Player.GetPosition().y);
		m_Camera.SetSmoothness(20.0f);
	}

	void Scene::OnEnemyKilled()
	{
		m_AliveEnemyCount--;

		if (m_AliveEnemyCount <= 0)
			m_LevelCompleted = true;
	}

	#pragma region Update Region

	void Scene::Update(float dt)
	{
		switch (m_GameState)
		{
		case GameState::MainMenu:
			UpdateMainMenu(dt);
			break;
		case GameState::Playing:
			UpdatePlaying(dt);
			break;
		case GameState::DeathScreen:
			UpdateDeathScreen(dt);
			break;
		}
	}

	void Scene::ChangeGameState(GameState newState)
	{
		m_GameState = newState;

		switch (m_GameState)
		{
		case GameState::MainMenu:
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"State changed to MAIN MENU"
			);
			break;

		case GameState::Playing:
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"State changed to PLAYING"
			);
			break;

		case GameState::DeathScreen:
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"State changed to DEATH SCREEN"
			);
			break;
		}
	}

	void Scene::UpdateMainMenu(float dt)
	{
		if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::F5))
			ChangeGameState(GameState::Playing);
	}

	void Scene::UpdatePlaying(float dt)
	{
		if (m_Player.IsDead())
		{
			ChangeGameState(GameState::DeathScreen);
			return;
		}

		m_Player.Update(dt);
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

		if (m_LevelCompleted)
		{
			m_LevelCompleted = false;
			OnLevelCompleted();
			return;
		}

		m_Camera.FollowSmooth(
			m_Player.GetPosition().x,
			m_Player.GetPosition().y,
			dt
		);
		m_Camera.UpdateShake(dt);
	}

	void Scene::UpdateDeathScreen(float dt)
	{
		if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::F5))
		{
			//Respawn Player
			m_Player.Respawn();
			ChangeGameState(GameState::Playing);
		}

		if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::F9))
		{
			ChangeGameState(GameState::MainMenu);
		}
	}

	#pragma endregion
	
	#pragma region Render Region

	void Scene::Render(EngineCore::IRenderer* renderer)
	{
		switch (m_GameState)
		{
		case GameState::MainMenu:
			renderer->DrawUIText("PRESS F5 TO START", 300, 310, { 255, 255, 255, 255 });
			break;
		case GameState::Playing:
			m_TileMap->Draw(renderer, m_Camera);
			m_TileMap->DrawCollisionDebug(renderer, m_Camera);
			m_Player.Render(renderer, m_Camera);
			RenderPlayerHP(renderer);
			for (auto& e : m_Enemies)
			{
				e->Render(renderer, m_Camera);
				RenderEnemyHP(renderer, e.get());
			}
			break;
		case GameState::DeathScreen:
			renderer->DrawUIText("PRESS F5 TO RESTART", 300, 400, { 255, 255, 255, 255 });
			renderer->DrawUIText("PRESS F9 TO MAIN MENU", 300, 300, { 255, 255, 255, 255 });
			break;
		}
	}

	void Scene::RenderPlayerHP(EngineCore::IRenderer* renderer)
	{
		float ratio = m_Player.GetRatio();
		ratio = std::clamp(ratio, 0.0f, 1.0f);

		EngineCore::Color hpColor = { 200, 40, 40, 255 };
		if (m_Player.IsDamageFlashing())
		{
			float t = fmod(m_Player.GetDamageFlashTimer() * 10.0f, 1.0f);
			if (t < 0.5f)
				hpColor = { 255, 255, 255, 255 };
		}

		renderer->DrawRect({ HP_BAR_X, HP_BAR_Y, HP_BAR_W, HP_BAR_H }, { 60, 60, 60, 255 });	//Background
		renderer->DrawRect({ (float)HP_BAR_X, (float)HP_BAR_Y, (HP_BAR_W * ratio), (float)HP_BAR_H }, hpColor);	//HP area
		renderer->DrawRectOutline({ HP_BAR_X, HP_BAR_Y, HP_BAR_W, HP_BAR_H }, { 255, 255, 255, 255 });
	}

	void Scene::RenderEnemyHP(EngineCore::IRenderer* renderer, EngineGame::Enemy* enemy)
	{
		float ratio = enemy->GetRatio();
		if (ratio <= 0.0f)
			return;

		const EngineCore::AABB& col = enemy->GetCollider();

		float centerX = col.Left() + col.Width() * 0.5f;
		float x = centerX - HP_BAR_W_EN * 0.5f - m_Camera.GetX();
		float y = col.Top() - 25.0f - m_Camera.GetY();

		EngineCore::Color hpColor = { 200, 40, 40, 255 };
		if (enemy->IsDamageFlashing())
			hpColor = { 255, 255, 255, 255 };

		renderer->DrawRect({ x, y, HP_BAR_W_EN, HP_BAR_H_EN }, { 40, 40, 40, 255 });	//Background
		renderer->DrawRect({ x, y, (HP_BAR_W_EN * ratio), HP_BAR_H_EN }, hpColor);	//HP area
		renderer->DrawRectOutline({ x, y, HP_BAR_W_EN, HP_BAR_H_EN }, { 255, 255, 255, 255 });

		float textY = y - 14.0f;
		renderer->DrawUIText(enemy->GetStateName(), x + HP_BAR_W_EN * 0.5f, textY, { 255, 255, 0, 255 });
	}

	#pragma endregion

	#pragma region Level Region

	void Scene::LoadCurrentLevel()
	{
		const LevelData* level = LevelManager::Get().GetCurrentLevel();

		if (!level)
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"No active level to load"
			);
			return;
		}

		LoadMap(level->mapId);
	}

	void Scene::ReloadLevel()
	{
		LoadCurrentLevel();
	}

	void Scene::OnLevelCompleted()
	{
		LevelManager::Get().LoadNextLevel();

		if (LevelManager::Get().GetCurrentLevel() != nullptr)
		{
			LoadCurrentLevel();
		}
		else
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"All levels have been completed"
			);

			ChangeGameState(GameState::MainMenu);
		}
	}

	void Scene::LoadMap(const std::string& mapId)
	{
		std::string targetPath = EngineCore::GetFile("Maps", mapId);

		//Map Loading 
		if (!EngineGame::MapLoader::LoadFromFile(targetPath, m_MapData))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Fatal,
				EngineCore::LogCategory::Scene,
				"Failed to load map.json"
			);
			return;
		}
		//Tilemap Creation
		m_TileMap = std::make_unique<EngineGame::TileMap>(m_MapData.w, m_MapData.h, m_MapData.tSize);

		std::vector<EngineGame::TileType> tiles;
		tiles.reserve(m_MapData.tiles.size());

		for (int v : m_MapData.tiles)
			tiles.push_back(static_cast<EngineGame::TileType>(v));

		m_TileMap->SetTiles(tiles);
		m_TileMap->LoadAssets();

		LoadSpawnEntities();
		LoadCamera();

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Map Loaded + TileMap initialized + Player-Enemies have been spawned + Camera loaded."
		);
	}

	#pragma endregion
}