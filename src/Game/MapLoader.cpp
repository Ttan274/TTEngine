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
		
		if (j.contains("PlayerSpawnX") && j.contains("PlayerSpawnY"))
		{
			outMap.playerSpawn.x = j["PlayerSpawnX"];
			outMap.playerSpawn.y = j["PlayerSpawnY"];
		}

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

		if (j.contains("EnemySpawns"))
		{
			for (auto& e : j["EnemySpawns"])
			{
				outMap.enemySpawns.push_back(
					{
						e["X"].get<float>(),
						e["Y"].get<float>()
					});
			}
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Map loaded : " + path
		);

		return true;
	}
}