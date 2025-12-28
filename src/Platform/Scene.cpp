#include "Platform/Scene.h"
#include "Platform/AssetManager.h"
#include "Game/MapLoader.h"
#include "Core/Log.h"
#include "Core/PathUtil.h"

namespace EnginePlatform
{
	//HP bar constants
	const int HP_BAR_X = 20;
	const int HP_BAR_Y = 20;
	const int HP_BAR_W = 200;
	const int HP_BAR_H = 20;
	const int HP_BAR_W_EN = 40;
	const int HP_BAR_H_EN = 6;

	Scene::Scene()
		: m_Camera(800.0f, 600.0f)
	{
	}

	void Scene::Load()
	{
		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Scene loading started"
		);

		ChangeGameState(GameState::MainMenu);

		//Map Loading
		std::string mapPath = EngineCore::GetRootDirectory("Maps", "active_map.json");
		EngineGame::MapData mapData;
		if (!EngineGame::MapLoader::LoadFromFile(mapPath, mapData))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Fatal,
				EngineCore::LogCategory::Scene,
				"Failed to load map.json"
			);
			return;
		}

		//Tilemap Creation
		m_TileMap = std::make_unique<EngineGame::TileMap>(mapData.w, mapData.h, mapData.tSize);
		
		std::vector<EngineGame::TileType> tiles;
		tiles.reserve(mapData.tiles.size());

		for (int v : mapData.tiles)
			tiles.push_back(static_cast<EngineGame::TileType>(v));

		m_TileMap->SetTiles(tiles);
		m_TileMap->LoadAssets();

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Map Loaded + TileMap initialized"
		);
		
		std::string exeDir = EngineCore::GetExecutableDirectory();
		LoadPlayer(exeDir, mapData.playerSpawn);
		LoadEnemy(exeDir, mapData.enemySpawns);
		LoadCamera();

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Camera + Player + Enemy Loaded. Scene Completed"
		);
	}

	void Scene::LoadPlayer(std::string exeDir, EngineMath::Vector2 pos)
	{
		//Player Loading
		std::string path = exeDir + "\\Assets/Textures";
		m_Player.SetTexture(
			AssetManager::GetTexture(path + "\\idle.png"),
			AssetManager::GetTexture(path + "\\walk.png"),
			AssetManager::GetTexture(path + "\\hurt.png"),
			AssetManager::GetTexture(path + "\\dead.png")
			);
		m_Player.SetAttackTexture(
			AssetManager::GetTexture(path + "\\attack1.png"),
			AssetManager::GetTexture(path + "\\attack2.png"),
			AssetManager::GetTexture(path + "\\attack3.png")
			);
		m_Player.SetWorld(m_TileMap.get());
		
		EngineMath::Vector2 newPos;
		//Player Spawning
		if (pos.x >= 0 && pos.y >= 0)
		{
			newPos.x = pos.x * m_TileMap->GetTileSize();
			newPos.y = pos.y * m_TileMap->GetTileSize();

			if (m_TileMap->IsSolidAt(newPos))
			{
				EngineCore::Log::Write(
					EngineCore::LogLevel::Warning,
					EngineCore::LogCategory::Scene,
					"Player spawn is on a wall tile!"
				);
			}

			m_Player.SetPosition(newPos);
			m_Player.SetSpawnPoint(newPos);
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"Player pos : " + std::to_string(newPos.x) + "," + std::to_string(newPos.y)
			);
		}
		else
		{
			// Fallback (spawn yoksa)
			newPos.x = m_TileMap->GetTileSize();
			newPos.y = m_TileMap->GetTileSize();
			m_Player.SetPosition(newPos);
		}
	}

	void Scene::LoadEnemy(std::string exeDir, std::vector<EngineMath::Vector2> spawns)
	{
		//Enemy Loading
		std::string path = exeDir + "\\Assets/Textures";
		EngineGame::Texture2D* enemyIdle = AssetManager::GetTexture(path + "\\idle1.png");
		EngineGame::Texture2D* enemyWalk = AssetManager::GetTexture(path + "\\walk1.png");
		EngineGame::Texture2D* enemyHurt = AssetManager::GetTexture(path + "\\hurt1.png");
		EngineGame::Texture2D* enemyDeath = AssetManager::GetTexture(path + "\\dead1.png");

		//Enemy Spawning
		EngineMath::Vector2 newPos;
		for (auto& sp : spawns)
		{
			auto enemy = std::make_unique<EngineGame::Enemy>();

			newPos.x = sp.x * m_TileMap->GetTileSize();
			newPos.y = sp.y * m_TileMap->GetTileSize();

			enemy->SetTexture(enemyIdle, enemyWalk, enemyHurt, enemyDeath);
			enemy->SetPosition(newPos);
			enemy->SetWorld(m_TileMap.get());

			m_Enemies.push_back(std::move(enemy));
		}
	}

	void Scene::LoadCamera()
	{
		//Camera loading
		m_Camera.SetWorldBounds(m_TileMap->GetWorldWidth(), m_TileMap->GetWorldHeight());
		m_Camera.Follow(m_Player.GetPosition().x, m_Player.GetPosition().y);
		m_Camera.SetSmoothness(20.0f);
	}

	void Scene::Update(float dt)
	{
		switch (m_GameState)
		{
		case GameState::MainMenu:
			UpdateMainMenu(dt);
			break;
		case GameState::Playing:
			UpdatePlaying(dt);
			break;
		case GameState::DeathScreen:
			UpdateDeathScreen(dt);
			break;
		}
	}

	void Scene::Render(EngineCore::IRenderer* renderer)
	{
		switch (m_GameState)
		{
		case GameState::MainMenu:
			renderer->DrawUIText("PRESS F5 TO START", 300, 310, { 255, 255, 255, 255 });
			break;
		case GameState::Playing:
			m_TileMap->Draw(renderer, m_Camera);
			m_TileMap->DrawSolidDebug(renderer, m_Camera);
			m_Player.Render(renderer, m_Camera);
			RenderPlayerHP(renderer);
			for (auto& e : m_Enemies)
			{
				e->Render(renderer, m_Camera);
				RenderEnemyHP(renderer, e.get());
			}
			break;
		case GameState::DeathScreen:
			renderer->DrawUIText("PRESS F5 TO RESTART", 300, 400, { 255, 255, 255, 255 });
			renderer->DrawUIText("PRESS F9 TO MAIN MENU", 300, 300, { 255, 255, 255, 255 });
			break;
		}
	}

	bool Scene::Intersects(const EngineCore::Rect& a, const EngineCore::Rect& b)
	{
		return !(
			a.x + a.w < b.x ||
			a.x > b.x + b.w ||
			a.y + a.h < b.y ||
			a.y > b.y + b.h
			);
	}

	void Scene::ChangeGameState(GameState newState)
	{
		m_GameState = newState;

		switch (m_GameState)
		{
		case GameState::MainMenu:
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"State changed to MAIN MENU"
			);
			break;

		case GameState::Playing:
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"State changed to PLAYING"
			);
			break;

		case GameState::DeathScreen:
			EngineCore::Log::Write(
				EngineCore::LogLevel::Info,
				EngineCore::LogCategory::Scene,
				"State changed to DEATH SCREEN"
			);
			break;
		}
	}

	void Scene::UpdateMainMenu(float dt)
	{
		if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::F5))
			ChangeGameState(GameState::Playing);
	}

	void Scene::UpdatePlaying(float dt)
	{
		if (m_Player.IsDead())
		{
			ChangeGameState(GameState::DeathScreen);
			return;
		}

		m_Player.Update(dt);
		for (auto it = m_Enemies.begin(); it != m_Enemies.end();)
		{
			auto& e = *it;
			EngineCore::Rect playerBox = m_Player.GetAttackBox();
			e->Update(dt);

			if (e->IsDead())
			{
				it = m_Enemies.erase(it);
				continue;
			}

			//Enemy attack behaviour
			if (Intersects(m_Player.GetCollider(), e->GetCollider()) && !m_Player.IsDead())
			{
				if (e->CanAttack())
				{
					m_Player.TakeDamage(20, e->IsFacingRight());
					e->ResetCooldown();
					m_Camera.StartShake(0.15f, 6.0f);
				}
			}

			//Player attack behaviour
			if (Intersects(playerBox, e->GetCollider()))
			{
				if (m_Player.IsAttacking() && m_Player.IsDamageFrame() && !m_Player.HasHitThisAttack())
				{
					e->TakeDamage(m_Player.GetAttackDamage(), m_Player.IsFacingRight());
					m_Player.MarkHitDone();
				}
			}
			++it;
		}

		m_Camera.FollowSmooth(
			m_Player.GetPosition().x,
			m_Player.GetPosition().y,
			dt
		);
		m_Camera.UpdateShake(dt);
	}

	void Scene::UpdateDeathScreen(float dt)
	{
		if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::F5))
		{
			//Respawn Player
			m_Player.Respawn();
			ChangeGameState(GameState::Playing);
		}

		if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::F9))
		{
			ChangeGameState(GameState::MainMenu);
		}
	}

	void Scene::RenderPlayerHP(EngineCore::IRenderer* renderer)
	{
		float ratio = m_Player.GetRatio();
		ratio = std::clamp(ratio, 0.0f, 1.0f);

		EngineCore::Color hpColor = { 200, 40, 40, 255 };
		if (m_Player.IsDamageFlashing())
		{
			float t = fmod(m_Player.GetDamageFlashTimer() * 10.0f, 1.0f);
			if(t < 0.5f)
				hpColor = { 255, 255, 255, 255 };
		}

		renderer->DrawRect({ HP_BAR_X, HP_BAR_Y, HP_BAR_W, HP_BAR_H }, { 60, 60, 60, 255 });	//Background
		renderer->DrawRect({ (float)HP_BAR_X, (float)HP_BAR_Y, (HP_BAR_W * ratio), (float)HP_BAR_H }, hpColor);	//HP area
		renderer->DrawRectOutline({ HP_BAR_X, HP_BAR_Y, HP_BAR_W, HP_BAR_H }, { 255, 255, 255, 255 });
	}

	void Scene::RenderEnemyHP(EngineCore::IRenderer* renderer, EngineGame::Enemy* enemy)
	{
		float ratio = enemy->GetRatio();

		if (ratio <= 0.0f)
			return;

		EngineCore::Rect col = enemy->GetCollider();

		float x = col.x + col.w * 0.5f - HP_BAR_W_EN * 0.5f - m_Camera.GetX();
		float y = col.y - 10 - m_Camera.GetY();

		EngineCore::Color hpColor = { 200, 40, 40, 255 };
		if (enemy->IsDamageFlashing())
			hpColor = { 255, 255, 255, 255 };

		renderer->DrawRect({ x, y, HP_BAR_W_EN, HP_BAR_H_EN }, { 40, 40, 40, 255 });	//Background
		renderer->DrawRect({ x, y, (HP_BAR_W_EN * ratio), (float)HP_BAR_H_EN }, hpColor);	//HP area
		renderer->DrawRectOutline({ x, y, HP_BAR_W_EN, HP_BAR_H_EN }, { 255, 255, 255, 255 });
	}
}