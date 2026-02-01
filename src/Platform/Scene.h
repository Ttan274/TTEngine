#pragma once
#include "Game/TileMap.h"
#include "Core/IRenderer.h"
#include "Game/Camera.h"
#include "Game/Player.h"
#include "Game/Enemy.h"
#include "Game/Texture.h"
#include "Game/MapLoader.h"
#include <memory>

namespace EnginePlatform
{
	enum class GameState
	{
		MainMenu,
		Playing,
		LevelComplete,
		DeathScreen
	};

	class Scene
	{
	public:
		Scene();
		
		//Loading Objects
		void Load();
		void LoadSpawnEntities();
		void LoadPlayer(const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def);
		void LoadEnemy(const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def);
		void LoadCamera();
		
		void Update(float dt);
		void Render(EngineCore::IRenderer* renderer);
		void ChangeGameState(GameState newState);
		GameState GetGameState() { return m_GameState; }
		void RenderPlayerHP(EngineCore::IRenderer* renderer);
		void RenderEnemyHP(EngineCore::IRenderer* renderer, EngineGame::Enemy* enemy);

		void LoadCurrentLevel();
		void ReloadLevel();
		void OnLevelCompleted();

		EngineGame::Player& GetPlayer() { return m_Player; }
		EngineGame::Camera2D& GetCamera() { return m_Camera; }
		int GetAliveEnemyCount() { return m_AliveEnemyCount; }
		void OnEnemyKilled();
	private:
		void UpdateMainMenu(float dt);
		void UpdatePlaying(float dt);
		void UpdateDeathScreen(float dt);
		void UpdateLevelComplete(float dt);
		void LoadMap(const std::string& mapId);
	private:
		EngineGame::Player m_Player;
		std::vector<std::unique_ptr<EngineGame::Enemy>> m_Enemies;
		EngineGame::Camera2D m_Camera;
		std::unique_ptr < EngineGame::TileMap> m_TileMap;
		EngineGame::Texture2D* m_MainMenuBg = nullptr;
		GameState m_GameState = GameState::MainMenu;

		//Load data
		std::unordered_map<std::string, EngineGame::EntityDefs> m_EntityDefs;
		EngineGame::MapData m_MapData;
		bool m_PlayerSpawned = false;
		int m_AliveEnemyCount = 0;
		bool m_LevelCompleted = false;

		//Level Complete Transition
		float m_LevelCompleteTimer = 0.0f;
		float m_FadeAlpha = 0.0f;
		bool m_FadeOut = true;
		float m_LevelTextScale = 0.0f;
		float m_LevelTextTimer = 0.0f;
	};
}