#include "Game/MapLoader.h"
#include "Core/Data/Map/MapParser.h"
#include "Core/JsonLoader.h"
#include "Core/Log.h"

namespace EngineGame
{
	bool MapLoader::LoadFromFile(const std::string& path, EngineData::MapData& outMap)
	{
		nlohmann::json j;

		if (!EngineCore::JsonLoader::LoadFromFile(path + ".json", j))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Failed to open map file: " + path
			);
		}
		
		outMap = {};

		if (!EngineData::MapParser::Parse(j, outMap))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Failed to parse map: " + path
			);
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Map loaded : " + path
		);

		return true;
	}
}