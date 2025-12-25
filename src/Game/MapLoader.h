#pragma once
#include "Game/TileMap.h"
#include <string>
#include <vector>

namespace EngineGame
{
	struct MapData
	{
		int w;
		int h;
		int tSize;
		std::vector<int> tiles;

		int spawnX = -1;
		int spawnY = -1;
	};

	class MapLoader
	{
	public:
		static bool LoadFromFile(const std::string& path, MapData& outMap);
	};
}