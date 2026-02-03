#include "Platform/InteractableLibrary.h"
#include <fstream>
#include <json.hpp>
#include "Core/Log.h"

using json = nlohmann::json;

namespace EnginePlatform
{
	InteractableLibrary& InteractableLibrary::Get()
	{
		static InteractableLibrary instance;
		return instance;
	}

	bool InteractableLibrary::LoadDefs(const std::string& path)
	{
		m_Defs.clear();

		std::ifstream file(path);
		if (!file.is_open())
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Failed to open interactables.json=" + path
			);
			return false;
		}

		json j;
		file >> j;

		
		for (auto& i : j)
		{
			InteractableDef def;

			def.id = i["Id"].get<std::string>();
			def.type = i["Type"].get<std::string>();
			def.imagePath = i["ImagePath"].get<std::string>();

			m_Defs[def.id] = def;
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Interactable Defs loaded : " + std::to_string(m_Defs.size())
		);

		return true;
	}

	const InteractableDef* InteractableLibrary::GetDef(const std::string& id) const
	{
		auto it = m_Defs.find(id);
		if (it == m_Defs.end())
			return nullptr;

		return &it->second;
	}
}