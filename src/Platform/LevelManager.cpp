#include "Platform/LevelManager.h"
#include "Core/JsonLoader.h"
#include "Core/Data/Level/LevelParser.h"
#include "Core/Log.h"

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

		nlohmann::json j;

		if (!EngineCore::JsonLoader::LoadFromFile(filePath, j))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Core,
				"Levels loaded succesfully : " + std::to_string(m_Levels.size())
			);
			return false;
		}

		if (!EngineData::LevelParser::Parse(j, m_Levels))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Core,
				"Failed to parse level file"
			);
			return false;
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Core,
			"Levels loaded succesfully : " + std::to_string(m_Levels.size())
		);

		return true;
	}

	const EngineData::LevelData* LevelManager::GetCurrentLevel() const
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
		else
			CompleteAll();
	}

	void LevelManager::CompleteAll()
	{
		m_CurrentLevelIndex = -1;
	}
}