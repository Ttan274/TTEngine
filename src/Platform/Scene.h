#pragma once
#include "Game/TileMap.h"
#include "Game/MapLoader.h"
#include "Platform/HUD.h"
#include "Platform/Loader.h"
#include "Game/InteractableManager.h"
#include "Game/TrapManager.h"
#include "Core/Data/Entity/EntityData.h"

namespace EnginePlatform
{
	class Scene
	{
	public:
		Scene();
		
		//Loading Objects
		void Load();
		
		void Update(float dt);
		void Render(EngineCore::IRenderer* renderer);
		void ChangeGameState(GameState newState);
		GameState GetGameState() { return m_GameState; }

		//Level
		void LoadCurrentLevel();
		void OnLevelCompleted();

		//UI Methods
		void StartGame();
		void MainMenu();

		//Get Methods
		LoadContext GetLoadContext();
		EngineGame::Player& GetPlayer() { return m_Player; }

	private:
		void UpdatePlaying(float dt);
		void UpdateLevelComplete(float dt);
	private:
		EngineGame::Player m_Player;
		std::vector<std::unique_ptr<EngineGame::Enemy>> m_Enemies;
		EngineGame::Camera2D m_Camera;
		std::unique_ptr<EngineGame::TileMap> m_TileMap;
		GameState m_GameState = GameState::MainMenu;

		//Load data
		EngineGame::MapData m_MapData;
		bool m_PlayerSpawned = false;
		bool m_LevelCompleted = false;

		//Level Complete Transition
		float m_LevelCompleteTimer = 0.0f;
		float m_FadeAlpha = 0.0f;
		bool m_FadeOut = true;
		float m_LevelTextScale = 0.0f;
		float m_LevelTextTimer = 0.0f;

		//HUD - Loader - Managers
		HUD m_HUD;
		Loader m_Loader;
		EngineGame::InteractableManager m_InteractableManager;
		EngineGame::TrapManager m_TrapManager;
	};
}