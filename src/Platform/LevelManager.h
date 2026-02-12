#pragma once
#include <vector>
#include <string>
#include "Core/Data/Level/LevelData.h"

namespace EnginePlatform
{
	class LevelManager
	{
	public:
		static LevelManager& Get();

		bool LoadAllLevels(const std::string& filePath);

		void StartLevelByIndex(int index);
		void LoadNextLevel();
		void CompleteAll();

		const EngineData::LevelData* GetCurrentLevel() const;
		const std::vector<EngineData::LevelData>& GetLevels() const { return m_Levels; }
		int GetCurrentIndex() const { return m_CurrentLevelIndex; }

	private:
		LevelManager() = default;

		std::vector<EngineData::LevelData> m_Levels;
		int m_CurrentLevelIndex = -1;
	};
}