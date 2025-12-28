#pragma once
#include "Game/TileMap.h"
#include "Core/IRenderer.h"
#include "Game/Camera.h"
#include "Game/Player.h"
#include "Game/Enemy.h"
#include "Game/Texture.h"
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
		void LoadPlayer(std::string exeDir, EngineMath::Vector2 pos);
		void LoadEnemy(std::string exeDir, std::vector<EngineMath::Vector2> spawns);
		void LoadCamera();
		void Update(float dt);
		void Render(EngineCore::IRenderer* renderer);
		bool Intersects(const EngineCore::Rect& a, const EngineCore::Rect& b);	//?
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
	};
}