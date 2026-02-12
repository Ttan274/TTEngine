#pragma once
#include <string>
#include <vector>

namespace EngineData
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
}