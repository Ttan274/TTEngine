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

	bool InteractableLibrary::LoadInteractableDefs(const std::string& path)
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

	bool InteractableLibrary::LoadTrapDefs(const std::string& path)
	{
		m_TrapDefs.clear();

		std::ifstream file(path);
		if (!file.is_open())
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"Failed to open traps.json=" + path
			);
			return false;
		}

		json j;
		file >> j;

		for (auto& i : j)
		{
			TrapDef def;

			def.id = i["Id"].get<std::string>();
			def.imagePath = i["ImagePath"].get<std::string>();

			//Saw Data
			def.speed = i["Speed"].get<float>();
			def.damage = i["Damage"].get<float>();
			def.damageCooldown = i["DamageCooldown"].get<float>();

			//Fire Data
			def.activeDuration = i["ActiveDuration"].get<float>();
			def.inactiveDuration = i["InactiveDuration"].get<float>();
			def.damagePerSecond = i["DamagePerSecond"].get<float>();

			m_TrapDefs[def.id] = def;
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Traps Defs loaded : " + std::to_string(m_TrapDefs.size())
		);

		return true;
	}

	const InteractableDef* InteractableLibrary::GetInteractableDef(const std::string& id) const
	{
		auto it = m_Defs.find(id);
		if (it == m_Defs.end())
			return nullptr;

		return &it->second;
	}

	const TrapDef* InteractableLibrary::GetTrapDef(const std::string& id) const
	{
		auto it = m_TrapDefs.find(id);
		if (it == m_TrapDefs.end())
			return nullptr;

		return &it->second;
	}
}