#pragma once
#include "Core/IRenderer.h"
#include "Game/Player.h"
#include "Game/Enemy.h"
#include "Game/Camera.h"
#include "Platform/GameState.h"
#include <vector>
#include <memory>

namespace EnginePlatform
{
	class HUD
	{
	public:
		void Render(EngineCore::IRenderer* renderer, const EngineGame::Player& player, 
			const std::vector<std::unique_ptr<EngineGame::Enemy>>& enemies,
			const EngineGame::Camera2D& camera,
			GameState state,
			float fadeAlpha);
	private:
		void RenderMainMenu(EngineCore::IRenderer* renderer);
		void RenderLevelComplete(EngineCore::IRenderer* renderer, float fadeAlpha);
		void RenderDeathScreen(EngineCore::IRenderer* renderer);
		
		void RenderPlayerHP(EngineCore::IRenderer* renderer, const EngineGame::Player& player);
		void RenderEnemyHP(EngineCore::IRenderer* renderer, const EngineGame::Enemy& enemy, const EngineGame::Camera2D& camera);
	};
}