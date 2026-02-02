#pragma once
#include <vector>
#include <memory>
#include <unordered_map>

namespace EngineGame
{
	class Player;
	class Enemy;
	class TileMap;
	class Camera2D;

	struct MapData;
	struct EntityDefs;
}

namespace EnginePlatform
{
	struct LoadContext
	{
		//Entity
		EngineGame::Player& player;
		std::vector<std::unique_ptr<EngineGame::Enemy>>& enemies;
		std::unordered_map<std::string, EngineGame::EntityDefs>& entityDefs;

		//Map
		std::unique_ptr<EngineGame::TileMap>& tileMap;
		EngineGame::MapData& mapData;

		//Camera
		EngineGame::Camera2D& camera;

		//Other Variables
		bool& playerSpawned;
		bool& levelCompleted;

		LoadContext(
			EngineGame::Player& p,
			std::vector<std::unique_ptr<EngineGame::Enemy>>& e,
			std::unordered_map<std::string, EngineGame::EntityDefs>& eDef,
			std::unique_ptr<EngineGame::TileMap>& t,
			EngineGame::MapData& m,
			EngineGame::Camera2D& c,
			bool& pSpawn,
			bool& lC
		) 
			:
			player(p),
			enemies(e),
			entityDefs(eDef),
			tileMap(t),
			mapData(m),
			camera(c),
			playerSpawned(pSpawn),
			levelCompleted(lC)
		{}
	};
}