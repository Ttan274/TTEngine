#include "Platform/HUD.h"
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

	void HUD::Render(EngineCore::IRenderer* renderer, const EngineGame::Player& player,
		const std::vector<std::unique_ptr<EngineGame::Enemy>>& enemies,
		const EngineGame::Camera2D& camera,
		GameState state,
		float fadeAlpha)
	{
		switch (state)
		{
		case GameState::MainMenu:
			RenderMainMenu(renderer);
			break;

		case GameState::Playing:
			RenderPlayerHP(renderer, player);
			for (auto& e : enemies)
				RenderEnemyHP(renderer, *e, camera);
			break;

		case GameState::LevelComplete:
			RenderLevelComplete(renderer, fadeAlpha);
			break;

		case GameState::DeathScreen:
			RenderDeathScreen(renderer);
			break;
		}
	}

	void HUD::RenderMainMenu(EngineCore::IRenderer* renderer)
	{
		renderer->DrawUIText("PRESS F5 TO START", 300.0f, 310.0f, { 255, 255, 255, 255 });
	}

	void HUD::RenderLevelComplete(EngineCore::IRenderer* renderer, float fadeAlpha)
	{
		EngineCore::Color fadeColor = { 0, 0, 0, static_cast<uint8_t>(fadeAlpha * 255) };
		renderer->DrawRect({ 0, 0, 800, 600 }, fadeColor);
		renderer->DrawUIText("Level Completed", 300.0f, 310.0f, { 255, 215, 0, 255 });
	}

	void HUD::RenderDeathScreen(EngineCore::IRenderer* renderer)
	{
		renderer->DrawUIText("PRESS F5 TO RESTART", 300, 400, { 255, 255, 255, 255 });
		renderer->DrawUIText("PRESS F9 TO MAIN MENU", 300, 300, { 255, 255, 255, 255 });
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
}