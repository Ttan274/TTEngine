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
		DeathScreen
	};

	class Scene
	{
	public:
		Scene();

		void Load();
		void LoadSpawnEntities();
		void LoadPlayer(const std::string& exeDir, const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def);
		void LoadEnemy(const std::string& exeDir, const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def);
		void LoadCamera();
		void Update(float dt);
		void Render(EngineCore::IRenderer* renderer);
		void ChangeGameState(GameState newState);
		GameState GetGameState() { return m_GameState; }
		void RenderPlayerHP(EngineCore::IRenderer* renderer);
		void RenderEnemyHP(EngineCore::IRenderer* renderer, EngineGame::Enemy* enemy);

		EngineGame::Player& GetPlayer() { return m_Player; }
		EngineGame::Camera2D& GetCamera() { return m_Camera; }
	private:
		void UpdateMainMenu(float dt);
		void UpdatePlaying(float dt);
		void UpdateDeathScreen(float dt);
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
	};
}