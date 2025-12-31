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
		
		//Loading Map Data
		outMap.w = j["Width"];
		outMap.h = j["Height"];
		outMap.tSize = j["TileSize"];
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
			"Map loaded : " + path
		);

		return true;
	}

	bool MapLoader::LoadEntityDefs(const std::string& path, std::unordered_map<std::string, EntityDefs>& outDefs)
	{
		std::ifstream file(path);
		if (!file.is_open())
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Failed to open definitions file: " + path
			);
			return false;
		}

		json j;
		file >> j;

		outDefs.clear();

		for (auto& e : j)
		{
			EntityDefs def;
			def.defId = e["Id"].get<std::string>();
			
			def.speed = e["Speed"].get<float>();
			def.attackDamage = e["AttackDamage"].get<float>();
			def.attackInterval = e["AttackInterval"].get<float>();
			def.maxHp = e["MaxHP"].get<float>();

			def.idleTexture = e["IdleTexture"];
			def.walkTexture = e["WalkTexture"];
			def.hurtTexture = e["HurtTexture"];
			def.deathTexture = e["DeathTexture"];

			if (e.contains("AttackTextures"))
				def.attackTextures = e["AttackTextures"].get<std::vector<std::string>>();

			outDefs[def.defId] = def;
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Definitions loaded : " + std::to_string(outDefs.size())
		);

		return true;
	}
}