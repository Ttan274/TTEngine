#include "Platform/Scene.h"
#include "Platform/AnimationLibrary.h"
#include "Core/Log.h"
#include "Core/PathUtil.h"
#include "Core/Math/Collision.h"
#include "Platform/LevelManager.h"

namespace EnginePlatform
{
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
		case GameState::LevelComplete:
			UpdateLevelComplete(dt);
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
		case GameState::LevelComplete:
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"State changed to Level Complete"
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
				OnLevelCompleted();
				m_FadeOut = false;
			}
		}
		else			//Fade IN
		{
			m_FadeAlpha -= dt * FADE_SPEED;
			if (m_FadeAlpha <= 0.0f)
			{
				m_FadeAlpha = 0.0f;
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
			ChangeGameState(GameState::LevelComplete);
			m_LevelCompleteTimer = 0.0f;
			m_FadeOut = true;
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

	//Render
	void Scene::Render(EngineCore::IRenderer* renderer)
	{
		switch (m_GameState)
		{
		case GameState::Playing:
			m_TileMap->Draw(renderer, m_Camera);
			m_TileMap->DrawCollisionDebug(renderer, m_Camera);
			m_Player.Render(renderer, m_Camera);
			for (auto& e : m_Enemies)
				e->Render(renderer, m_Camera);
			break;
		default:
			break;
		}

		m_HUD.Render(renderer, m_Player, m_Enemies, m_Camera, m_GameState, m_FadeAlpha);
	}

	//Level Area
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
		m_Enemies.clear();
		m_LevelCompleted = false;
		m_PlayerSpawned = false;
		m_Player.Reset();

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
}