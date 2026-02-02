#pragma once
#include <vector>
#include <memory>
#include "Core/IRenderer.h"
#include "Platform/GameState.h"
#include "Game/Player.h"
#include "Game/Enemy.h"
#include "Game/Camera.h"

namespace EnginePlatform
{
	class Scene;

	class HUD
	{
	public:
		void Render(EngineCore::IRenderer* renderer, const EngineGame::Player& player, 
			const std::vector<std::unique_ptr<EngineGame::Enemy>>& enemies,
			const EngineGame::Camera2D& camera,
			Scene& scene,
			GameState state,
			float fadeAlpha);
	private:
		void RenderMainMenu(EngineCore::IRenderer* renderer, Scene& scene);
		void RenderLevelComplete(EngineCore::IRenderer* renderer, float fadeAlpha);
		void RenderDeathScreen(EngineCore::IRenderer* renderer, Scene& scene);
		
		void RenderPlayerHP(EngineCore::IRenderer* renderer, const EngineGame::Player& player);
		void RenderEnemyHP(EngineCore::IRenderer* renderer, const EngineGame::Enemy& enemy, const EngineGame::Camera2D& camera);

		void RenderCursor(EngineCore::IRenderer* renderer);

		bool m_CanRenderCursor = false;
	};
}