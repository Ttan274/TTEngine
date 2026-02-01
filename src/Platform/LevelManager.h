#pragma once
#include <vector>
#include <string>

namespace EnginePlatform
{
	struct LevelData
	{
		std::string id;
		std::string mapId;
		bool IsActive;
	};

	class LevelManager
	{
	public:
		static LevelManager& Get();

		bool LoadAllLevels(const std::string& filePath);

		void StartLevelByIndex(int index);
		void LoadNextLevel();

		const LevelData* GetCurrentLevel() const;
		const std::vector<LevelData>& GetLevels() const { return m_Levels; }
		int GetCurrentIndex() const { return m_CurrentLevelIndex; }

	private:
		LevelManager() = default;

		std::vector<LevelData> m_Levels;
		int m_CurrentLevelIndex = -1;
	};
}