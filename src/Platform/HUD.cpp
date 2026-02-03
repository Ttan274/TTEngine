#include "Platform/HUD.h"
#include "Platform/Scene.h"
#include <algorithm>

namespace EnginePlatform
{
	// HP bar constants
	const int HP_BAR_X = 20;
	const int HP_BAR_Y = 20;
	const int HP_BAR_W = 200;
	const int HP_BAR_H = 20;
	const int HP_BAR_W_EN = 40;
	const int HP_BAR_H_EN = 6;

	// UI button constants
	const EngineCore::Rect topBtn{ 300, 260, 200, 40 };
	const EngineCore::Rect bottomBtn{ 300, 320, 200, 40 };
	const EngineCore::Color normalColor{ 50, 50, 50, 255 };
	const EngineCore::Color hoverColor{ 80, 80, 80, 255 };

	void HUD::Render(EngineCore::IRenderer* renderer, const EngineGame::Player& player,
		const std::vector<std::unique_ptr<EngineGame::Enemy>>& enemies,
		const EngineGame::Camera2D& camera,
		Scene& scene,
		GameState state,
		float fadeAlpha)
	{
		switch (state)
		{
		case GameState::MainMenu:
			RenderMainMenu(renderer, scene);
			m_CanRenderCursor = true;
			break;

		case GameState::Playing:
			RenderPlayerHP(renderer, player);
			for (auto& e : enemies)
				RenderEnemyHP(renderer, *e, camera);
			m_CanRenderCursor = false;
			break;

		case GameState::LevelComplete:
			RenderLevelComplete(renderer, fadeAlpha);
			m_CanRenderCursor = false;
			break;

		case GameState::DeathScreen:
			RenderDeathScreen(renderer, scene);
			m_CanRenderCursor = true;
			break;
		}

		RenderCursor(renderer);
		RenderInteractPopup(renderer);
	}

	void HUD::RenderMainMenu(EngineCore::IRenderer* renderer, Scene& scene)
	{
		if (renderer->DrawUIButton("Start Game", topBtn, normalColor, hoverColor).clicked)
		{
			scene.StartGame();
		}

		if (renderer->DrawUIButton("Quit Game", bottomBtn, normalColor, hoverColor).clicked)
		{
			//Quit Game
		}
	}

	//Pause Menu eklenmeli

	void HUD::RenderLevelComplete(EngineCore::IRenderer* renderer, float fadeAlpha)
	{
		EngineCore::Color fadeColor = { 0, 0, 0, static_cast<uint8_t>(fadeAlpha * 255) };
		renderer->DrawRect({ 0, 0, 800, 600 }, fadeColor);
		renderer->DrawUIText("Level Completed", 300.0f, 310.0f, { 255, 215, 0, 255 });
	}

	void HUD::RenderDeathScreen(EngineCore::IRenderer* renderer, Scene& scene)
	{
		if (renderer->DrawUIButton("Restart Game", topBtn, normalColor, hoverColor).clicked)
		{
			scene.StartGame();
		}

		if (renderer->DrawUIButton("Main Menu", bottomBtn, normalColor, hoverColor).clicked)
		{
			scene.MainMenu();
		}
	}

	void HUD::RenderPlayerHP(EngineCore::IRenderer* renderer, const EngineGame::Player& player)
	{
		float ratio = player.GetRatio();
		ratio = std::clamp(ratio, 0.0f, 1.0f);

		EngineCore::Color hpColor = { 200, 40, 40, 255 };
		if (player.IsDamageFlashing())
		{
			float t = fmod(player.GetDamageFlashTimer() * 10.0f, 1.0f);
			if (t < 0.5f)
				hpColor = { 255, 255, 255, 255 };
		}

		renderer->DrawRect({ HP_BAR_X, HP_BAR_Y, HP_BAR_W, HP_BAR_H }, { 60, 60, 60, 255 });	//Background
		renderer->DrawRect({ (float)HP_BAR_X, (float)HP_BAR_Y, (HP_BAR_W * ratio), (float)HP_BAR_H }, hpColor);	//HP area
		renderer->DrawRectOutline({ HP_BAR_X, HP_BAR_Y, HP_BAR_W, HP_BAR_H }, { 255, 255, 255, 255 });
	}

	void HUD::RenderEnemyHP(EngineCore::IRenderer* renderer, const EngineGame::Enemy& enemy, const EngineGame::Camera2D& camera)
	{
		float ratio = enemy.GetRatio();
		if (ratio <= 0.0f)
			return;

		const EngineCore::AABB& col = enemy.GetCollider();

		float centerX = col.Left() + col.Width() * 0.5f;
		float x = centerX - HP_BAR_W_EN * 0.5f - camera.GetX();
		float y = col.Top() - 25.0f - camera.GetY();

		EngineCore::Color hpColor = { 200, 40, 40, 255 };
		if (enemy.IsDamageFlashing())
			hpColor = { 255, 255, 255, 255 };

		renderer->DrawRect({ x, y, HP_BAR_W_EN, HP_BAR_H_EN }, { 40, 40, 40, 255 });	//Background
		renderer->DrawRect({ x, y, (HP_BAR_W_EN * ratio), HP_BAR_H_EN }, hpColor);	//HP area
		renderer->DrawRectOutline({ x, y, HP_BAR_W_EN, HP_BAR_H_EN }, { 255, 255, 255, 255 });

		float textY = y - 14.0f;
		renderer->DrawUIText(enemy.GetStateName(), x + HP_BAR_W_EN * 0.5f, textY, { 255, 255, 0, 255 });
	}

	void HUD::RenderCursor(EngineCore::IRenderer* renderer)
	{
		if (!m_CanRenderCursor)
			return;

		auto mousePos = EngineCore::Input::GetMousePosition();

		EngineCore::Color cursorColor{ 220, 220, 220, 150 };

		renderer->DrawCircle(mousePos.x, mousePos.y, 6.0f, cursorColor);
	}

	void HUD::SetInteractPopup(bool canShow, float x, float y)
	{
		m_CanRenderPopup = canShow;
		m_InteractPopupPos = { x, y };
	}

	void HUD::RenderInteractPopup(EngineCore::IRenderer* renderer)
	{
		if (m_CanRenderPopup)
			renderer->DrawUIText("Press F5 Button", m_InteractPopupPos.x, m_InteractPopupPos.y, { 255, 255, 255, 255 });
	}
}