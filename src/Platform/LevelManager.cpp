#include "Platform/LevelManager.h"
#include <fstream>
#include <json.hpp>
#include "Core/Log.h"

using json = nlohmann::json;


namespace EnginePlatform
{
	LevelManager& LevelManager::Get()
	{
		static LevelManager instance;
		return instance;
	}

	bool LevelManager::LoadAllLevels(const std::string& filePath)
	{
		m_Levels.clear();
		m_CurrentLevelIndex = -1;

		std::ifstream file(filePath);
		if (!file.is_open())
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Core,
				"Failed to open levels.json"
			);
			return false;
		}

		json j;
		file >> j;

		if (!j.contains("Levels"))
			return false;

		for (auto& l : j["Levels"])
		{
			LevelData data;
			data.id = l["Id"].get<std::string>();
			data.mapId = l["MapId"].get<std::string>();
			data.IsActive = l["IsActive"].get<bool>();

			if (data.IsActive)
			{
				m_Levels.push_back(data);
				EngineCore::Log::Write(
					EngineCore::LogLevel::Info,
					EngineCore::LogCategory::Core,
					"level added :" + data.id
				);
			}
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Core,
			"Levels loaded succesfully : " + std::to_string(m_Levels.size())
		);

		return true;
	}

	const LevelData* LevelManager::GetCurrentLevel() const
	{
		if (m_CurrentLevelIndex < 0 || m_CurrentLevelIndex >= (int)m_Levels.size())
			return nullptr;

		return &m_Levels[m_CurrentLevelIndex];
	}

	void LevelManager::StartLevelByIndex(int index)
	{
		if(index < 0 || index >= (int)m_Levels.size())
			return;

		m_CurrentLevelIndex = index;

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Core,
			"Level started : " + GetCurrentLevel()->id
		);
	}

	void LevelManager::LoadNextLevel()
	{
		int next = m_CurrentLevelIndex + 1;
		if (next < (int)m_Levels.size())
			StartLevelByIndex(next);
	}
}