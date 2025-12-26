#include "Platform/Scene.h"
#include "Platform/AssetManager.h"
#include "Game/MapLoader.h"
#include "Core/Log.h"
#include "Core/PathUtil.h"
#include "Game/Texture.h"

namespace EnginePlatform
{
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

		//Map Loading
		std::string mapPath =EngineCore::GetRootDirectory("Maps", "active_map.json");
		EngineGame::MapData mapData;
		if (!EngineGame::MapLoader::LoadFromFile(mapPath, mapData))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Fatal,
				EngineCore::LogCategory::Scene,
				"Failed to load map.json"
			);
			return;
		}

		//Tilemap Creation
		m_TileMap = std::make_unique<EngineGame::TileMap>(mapData.w, mapData.h, mapData.tSize);
		
		std::vector<EngineGame::TileType> tiles;
		tiles.reserve(mapData.tiles.size());

		for (int v : mapData.tiles)
			tiles.push_back(static_cast<EngineGame::TileType>(v));

		m_TileMap->SetTiles(tiles);
		m_TileMap->LoadAssets();

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Map Loaded + TileMap initialized"
		);
		
		std::string exeDir = EngineCore::GetExecutableDirectory();
		LoadPlayer(exeDir, mapData.playerSpawn);
		LoadEnemy(exeDir, mapData.enemySpawns);
		LoadCamera();

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Camera + Player + Enemy Loaded. Scene Completed"
		);
	}

	void Scene::LoadPlayer(std::string exeDir, EngineMath::Vector2 pos)
	{
		//Player Loading
		std::string idlePath = exeDir + "\\Assets/Textures/idle.png";
		std::string walkPath = exeDir + "\\Assets/Textures/walk.png";
		m_Player.SetTexture(AssetManager::GetTexture(idlePath),
			AssetManager::GetTexture(walkPath));
		m_Player.SetWorld(m_TileMap.get());
		
		//Player Spawning
		if (pos.x >= 0 && pos.y >= 0)
		{
			float xPos = pos.x * m_TileMap->GetTileSize();
			float yPos = pos.y * m_TileMap->GetTileSize();

			if (m_TileMap->IsSolidAt(xPos, yPos))
			{
				EngineCore::Log::Write(
					EngineCore::LogLevel::Warning,
					EngineCore::LogCategory::Scene,
					"Player spawn is on a wall tile!"
				);
			}

			m_Player.SetPosition(xPos, yPos);
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"Player pos : " + std::to_string(xPos) + "," + std::to_string(yPos)
			);
		}
		else
		{
			// Fallback (spawn yoksa)
			m_Player.SetPosition(
				m_TileMap->GetTileSize(),
				m_TileMap->GetTileSize()
			);
		}
	}

	void Scene::LoadEnemy(std::string exeDir, std::vector<EngineMath::Vector2> spawns)
	{
		//Enemy Loading
		std::string idlePath = exeDir + "\\Assets/Textures/idle1.png";
		std::string walkPath = exeDir + "\\Assets/Textures/walk1.png";
		EngineGame::Texture2D* enemyIdle = AssetManager::GetTexture(idlePath);
		EngineGame::Texture2D* enemyWalk = AssetManager::GetTexture(walkPath);

		//Enemy Spawning
		for (auto& sp : spawns)
		{
			auto enemy = std::make_unique<EngineGame::Enemy>();

			float xPos = sp.x * m_TileMap->GetTileSize();
			float yPos = sp.y * m_TileMap->GetTileSize();

			enemy->SetTexture(enemyIdle, enemyWalk);
			enemy->SetPosition(xPos, yPos);
			enemy->SetWorld(m_TileMap.get());

			m_Enemies.push_back(std::move(enemy));
		}
	}

	void Scene::LoadCamera()
	{
		//Camera loading
		m_Camera.SetWorldBounds(m_TileMap->GetWorldWidth(), m_TileMap->GetWorldHeight());
		m_Camera.Follow(m_Player.GetPosition().x, m_Player.GetPosition().y);
		m_Camera.SetSmoothness(20.0f);
	}

	void Scene::Update(float dt)
	{
		m_Player.Update(dt);
		for (auto& e : m_Enemies)
			e->Update(dt);
		m_Camera.FollowSmooth(
			m_Player.GetPosition().x,
			m_Player.GetPosition().y,
			dt
		);
	}

	void Scene::Render(EngineCore::IRenderer* renderer)
	{
		m_TileMap->Draw(renderer, m_Camera);
		m_TileMap->DrawSolidDebug(renderer, m_Camera);
		m_Player.Render(renderer, m_Camera);
		for (auto& e : m_Enemies)
			e->Render(renderer, m_Camera);
	}
}