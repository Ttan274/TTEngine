#include "Platform/Scene.h"
#include "Platform/AssetManager.h"
#include "Game/MapLoader.h"
#include "Core/Log.h"
#include "Core/PathUtil.h"

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

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Map Loaded map.json"
		);

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
			"TileMap initialized"
		);

		//Player Loading
		const char* basePath = SDL_GetBasePath();
		std::string idlePath = std::string(basePath) + "Assets/Textures/idle.png";
		std::string walkPath = std::string(basePath) + "Assets/Textures/walk.png";
		m_Player.SetTexture(AssetManager::GetTexture(idlePath),
							AssetManager::GetTexture(walkPath));
		m_Player.SetWorld(m_TileMap.get());
		//Player Spawning
		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Player pos : " + std::to_string(mapData.spawnX) + "," + std::to_string(mapData.spawnY)
		);
		if (mapData.spawnX >= 0 && mapData.spawnY >= 0)
		{
			float xPos = mapData.spawnX * m_TileMap->GetTileSize();
			float yPos = mapData.spawnY * m_TileMap->GetTileSize();

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


		//Camera loading
		m_Camera.SetWorldBounds(m_TileMap->GetWorldWidth(), m_TileMap->GetWorldHeight());
		m_Camera.Follow(m_Player.GetPosition().x, m_Player.GetPosition().y);
		m_Camera.SetSmoothness(20.0f);

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Scene loaded successfully"
		);
	}

	void Scene::Update(float dt)
	{
		m_Player.Update(dt);

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
	}
}