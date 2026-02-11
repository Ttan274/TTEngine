#pragma once
#include <vector>
#include <memory>
#include <unordered_map>

namespace EngineGame
{
	class Player;
	class Enemy;
	class TileMap;
	class InteractableManager;
	class TrapManager;
	class Camera2D;

	struct MapData;
}

namespace EnginePlatform
{
	struct LoadContext
	{
		//Entity
		EngineGame::Player& player;
		std::vector<std::unique_ptr<EngineGame::Enemy>>& enemies;

		//Map
		std::unique_ptr<EngineGame::TileMap>& tileMap;
		EngineGame::MapData& mapData;

		//Interactables
		EngineGame::InteractableManager& interactableList;
		EngineGame::TrapManager& trapList;

		//Camera
		EngineGame::Camera2D& camera;

		//Other Variables
		bool& playerSpawned;
		bool& levelCompleted;

		LoadContext(
			EngineGame::Player& p,
			std::vector<std::unique_ptr<EngineGame::Enemy>>& e,
			std::unique_ptr<EngineGame::TileMap>& t,
			EngineGame::MapData& m,
			EngineGame::InteractableManager& i,
			EngineGame::TrapManager& trap,
			EngineGame::Camera2D& c,
			bool& pSpawn,
			bool& lC
		) 
			:
			player(p),
			enemies(e),
			tileMap(t),
			mapData(m),
			interactableList(i),
			trapList(trap),
			camera(c),
			playerSpawned(pSpawn),
			levelCompleted(lC)
		{}
	};
}