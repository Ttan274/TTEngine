#pragma once
#include "Core/Data/Map/MapData.h"
#include "json.hpp"

namespace EngineData
{
	class MapParser
	{
	public:
		static bool Parse(const nlohmann::json& j, MapData& outMap)
		{
			//Parsing map basics
			outMap.w = j.value("Width", 0);
			outMap.h = j.value("Height", 0);
			outMap.tSize = j.value("TileSize", 0);

			if (!j.contains("Layers") || !j["Layers"].contains("Collision"))
				return false;

			outMap.tiles = j["Layers"]["Collision"].get<std::vector<int>>();

			if ((int)outMap.tiles.size() != outMap.w * outMap.h)
				return false;

			//Parsing Interactables-Traps
			outMap.interactables.clear();
			if (j.contains("Interactables"))
			{
				for (auto& i : j["Interactables"])
				{
					SpawnData d;
					d.x = i.value("X", 0.f);
					d.y = i.value("Y", 0.f);
					d.defId = i.value("DefinitionId", "");

					outMap.interactables.push_back(d);
				}
			}

			outMap.traps.clear();
			if (j.contains("Traps"))
			{
				for (auto& i : j["Traps"])
				{
					SpawnData d;
					d.x = i.value("X", 0.f);
					d.y = i.value("Y", 0.f);
					d.defId = i.value("DefinitionId", "");

					outMap.traps.push_back(d);
				}
			}

			//Parsing Entities
			outMap.spawns.clear();
			if (j.contains("PlayerSpawn"))
			{
				auto& p = j["PlayerSpawn"];

				SpawnData d;
				d.x = p.value("X", 0.f);
				d.y = p.value("Y", 0.f);
				d.defId = p.value("DefinitionId", "");

				outMap.spawns.push_back(d);
			}

			if (j.contains("EnemySpawns"))
			{
				for (auto& i : j["EnemySpawns"])
				{
					SpawnData d;
					d.x = i.value("X", 0.f);
					d.y = i.value("Y", 0.f);
					d.defId = i.value("DefinitionId", "");

					outMap.spawns.push_back(d);
				}
			}

			return true;
		}
	};
}