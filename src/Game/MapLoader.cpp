#include "Game/MapLoader.h"
#include "Core/Log.h"
#include <json.hpp>
#include <fstream>

using json = nlohmann::json;

namespace EngineGame
{
	bool MapLoader::LoadFromFile(const std::string& path, MapData& outMap)
	{
		std::ifstream file(path);
		if (!file.is_open())
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Failed to open map file: " + path
			);
			return false;
		}

		json j;
		file >> j;

		outMap.w = j["Width"];
		outMap.h = j["Height"];
		outMap.tSize = j["TileSize"];
		
		if (j.contains("PlayerSpawnX"))
			outMap.spawnX = j["PlayerSpawnX"];
		if (j.contains("PlayerSpawnY"))
			outMap.spawnY = j["PlayerSpawnY"];

		outMap.tiles = j["Tiles"].get<std::vector<int>>();
		if ((int)outMap.tiles.size() != outMap.w * outMap.h)
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Tile count mismatch in map file"
			);
			return false;
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Map loaded : " + path
		);

		return true;
	}
}