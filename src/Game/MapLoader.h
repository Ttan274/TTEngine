#pragma once
#include "Game/TileMap.h"
#include <string>
#include <vector>
#include "Core/Math/Vector2.h"

namespace EngineGame
{
	struct MapData
	{
		int w;
		int h;
		int tSize;
		std::vector<int> tiles;

		EngineMath::Vector2 playerSpawn;
		std::vector<EngineMath::Vector2> enemySpawns;
	};

	class MapLoader
	{
	public:
		static bool LoadFromFile(const std::string& path, MapData& outMap);
	};
}