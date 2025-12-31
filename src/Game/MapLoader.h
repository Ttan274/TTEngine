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
	};

	class MapLoader
	{
	public:
		static bool LoadFromFile(const std::string& path, MapData& outMap);
		static bool LoadEntityDefs(const std::string& path, std::unordered_map<std::string, EntityDefs>& outDefs);
	};
}