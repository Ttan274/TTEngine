#include "Game/MapLoader.h"
#include "Core/Log.h"
#include <json.hpp>
#include <fstream>

using json = nlohmann::json;

namespace EngineGame
{
	bool MapLoader::LoadFromFile(const std::string& path, MapData& outMap)
	{
		std::ifstream file(path + ".json");
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

		//Loading Map Data
		outMap.w = j["Width"];
		outMap.h = j["Height"];
		outMap.tSize = j["TileSize"];

		if (!j.contains("Layers") || !j["Layers"].contains("Collision"))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Map has no collision layer"
			);
			return false;
		}

		outMap.tiles = j["Layers"]["Collision"].get<std::vector<int>>();
		if ((int)outMap.tiles.size() != outMap.w * outMap.h)
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Tile count mismatch in map file"
			);
			return false;
		}

		

		//Loading Interactables Data
		if (!j.contains("Interactables"))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Map has no interactables"
			);
			return false;
		}

		//Loading Interactbles SpawnData in Map
		outMap.interactables.clear();
		if (j.contains("Interactables"))
		{
			for (auto& i : j["Interactables"])
			{
				SpawnData d;
				d.x = i["X"].get<float>();
				d.y = i["Y"].get<float>();
				d.defId = i["DefinitionId"].get<std::string>();

				outMap.interactables.push_back(d);
			}
		}

		//Loading Traps SpawnData in Map
		outMap.traps.clear();
		if (j.contains("Traps"))
		{
			for (auto& i : j["Traps"])
			{
				SpawnData d;
				d.x = i["X"].get<float>();
				d.y = i["Y"].get<float>();
				d.defId = i["DefinitionId"].get<std::string>();

				outMap.traps.push_back(d);
			}
		}

		//Loading Spawn Data
		outMap.spawns.clear();

		//Player Spawn
		if (j.contains("PlayerSpawn"))
		{
			auto& p = j["PlayerSpawn"];

			SpawnData d;
			d.x = p["X"].get<float>();
			d.y = p["Y"].get<float>();
			d.defId = p["DefinitionId"].get<std::string>();

			outMap.spawns.push_back(d);
		}
		else
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Warning,
				EngineCore::LogCategory::Scene,
				"Map has no player spawn"
			);
		}

		//Enemy Spawns
		if (j.contains("EnemySpawns"))
		{
			for (auto& e : j["EnemySpawns"])
			{
				SpawnData d;
				d.x = e["X"].get<float>();
				d.y = e["Y"].get<float>();
				d.defId = e["DefinitionId"].get<std::string>();

				outMap.spawns.push_back(d);
			}
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Map loaded (collision layer only) : " + path
		);

		return true;
	}
}