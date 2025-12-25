#pragma once
#include "Game/TileMap.h"
#include "Core/IRenderer.h"
#include "Game/Camera.h"
#include "Game/Player.h"
#include <memory>

namespace EnginePlatform
{
	class Scene
	{
	public:
		Scene();

		void Load();
		void Update(float dt);
		void Render(EngineCore::IRenderer* renderer);
		
		EngineGame::Player& GetPlayer() { return m_Player; }
		EngineGame::Camera2D& GetCamera() { return m_Camera; }
	private:
		EngineGame::Player m_Player;
		EngineGame::Camera2D m_Camera;
		std::unique_ptr < EngineGame::TileMap> m_TileMap;
	};
}