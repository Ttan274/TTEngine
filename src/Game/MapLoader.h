#pragma once
#include <string>
#include <vector>
#include <unordered_map>
#include "Entity.h"

namespace EngineGame
{
	struct SpawnData
	{
		float x;
		float y;
		std::string defId;
	};

	struct MapData
	{
		int w;
		int h;
		int tSize;
		std::vector<int> tiles;

		std::vector<SpawnData> spawns;
		std::vector<SpawnData> interactables;
		std::vector<SpawnData> traps;
	};

	class MapLoader
	{
	public:
		static bool LoadFromFile(const std::string& path, MapData& outMap);
	};
}