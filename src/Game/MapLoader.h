#pragma once
#include "Core/Data/Map/MapData.h"

namespace EngineGame
{
	class MapLoader
	{
	public:
		static bool LoadFromFile(const std::string& path, EngineData::MapData& outMap);
	};
}