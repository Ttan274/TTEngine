#pragma once
#include "Core/Data/Level/LevelData.h"
#include "json.hpp"

namespace EngineData
{
	class LevelParser
	{
	public:
		static bool Parse(const nlohmann::json& j, std::vector<LevelData>& outLevel)
		{
			if (!j.contains("Levels") && !j["Levels"].is_array())
				return false;

			for (auto& l : j["Levels"])
			{
				LevelData data;
				data.id = l.value("Id", "");
				data.mapId = l.value("MapId", "");
				data.IsActive = l.value("IsActive", false);

				if (data.IsActive)
				{
					outLevel.push_back(data);
				}
			}

			return true;
		}
	};
}