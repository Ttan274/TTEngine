#pragma once
#include <string>
#include <json.hpp>

namespace EngineCore
{
	class JsonLoader
	{
	public:
		static bool LoadFromFile(const std::string& path, nlohmann::json& outJson);
	};
}