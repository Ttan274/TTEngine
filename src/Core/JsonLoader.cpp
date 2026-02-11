#include "Core/JsonLoader.h"
#include <Core/Log.h>
#include <fstream>

namespace EngineCore
{
	bool JsonLoader::LoadFromFile(const std::string& path, nlohmann::json& outJson)
	{
		std::ifstream file(path);
		if (!file.is_open())
		{
			Log::Write(
				LogLevel::Error,
				LogCategory::Core,
				"Failed to open json file :" + path
			);
			return false;
		}

		try
		{
			file >> outJson;
		}
		catch (const std::exception& e)
		{
			Log::Write(
				LogLevel::Error,
				LogCategory::Core,
				"Json parse error :" + path + "\n" + e.what()
			);
			return false;
		}

		return true;
	}
}