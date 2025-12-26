#pragma once
#include "Game/TileMap.h"
#include "Core/IRenderer.h"
#include "Game/Camera.h"
#include "Game/Player.h"
#include "Game/Enemy.h"
#include <memory>

namespace EnginePlatform
{
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
		
		EngineGame::Player& GetPlayer() { return m_Player; }
		EngineGame::Camera2D& GetCamera() { return m_Camera; }
	private:
		EngineGame::Player m_Player;
		std::vector<std::unique_ptr<EngineGame::Enemy>> m_Enemies;
		EngineGame::Camera2D m_Camera;
		std::unique_ptr < EngineGame::TileMap> m_TileMap;
	};
}