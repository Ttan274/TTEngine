#include "Platform/Loader.h"
#include "Platform/AnimationLibrary.h"
#include "Platform/InteractableLibrary.h"
#include "Platform/LevelManager.h"
#include "Core/PathUtil.h"
#include "Core/Log.h"
#include "Platform/Scene.h"

namespace EnginePlatform
{
	void Loader::LoadBasics(std::unordered_map<std::string, EngineGame::EntityDefs>& entityDefs)
	{
		//Animation Library loaded
		std::string animDir = EngineCore::GetExecutableDirectory() + "\\Assets\\Animation";
		if (!EnginePlatform::AnimationLibrary::LoadAllAnims(animDir))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Fatal,
				EngineCore::LogCategory::Scene,
				"Failed to load all animation files"
			);
			return;
		}

		//Entity Definitions loaded
		if (!EngineGame::MapLoader::LoadEntityDefs(EngineCore::GetFile("Data", "entity_def.json"), entityDefs))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Fatal,
				EngineCore::LogCategory::Scene,
				"Failed to load entity.json"
			);
			return;
		}

		InteractableLibrary::Get().LoadInteractableDefs(EngineCore::GetFile("Data", "Interactables.json"));
		InteractableLibrary::Get().LoadTrapDefs(EngineCore::GetFile("Data", "TrapDef.json"));

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Scene basics have been loaded"
		);
	}

	void Loader::LoadCurrentLevel(LoadContext& ctx)
	{
		const LevelData* level = LevelManager::Get().GetCurrentLevel();
	
		if (!level)
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Error,
				EngineCore::LogCategory::Scene,
				"No active level to load"
			);
			return;
		}

		LoadMap(ctx, level->mapId);
	}

	//Map
	void Loader::LoadMap(LoadContext& ctx, const std::string& mapId)
	{
		ctx.enemies.clear();
		ctx.levelCompleted = false;
		ctx.playerSpawned = false;
		ctx.player.Reset();

		std::string targetPath = EngineCore::GetFile("Maps", mapId);

		//Loading Map
		if (!EngineGame::MapLoader::LoadFromFile(targetPath, ctx.mapData))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Fatal,
				EngineCore::LogCategory::Scene,
				"Failed to load map.json"
			);
			return;
		}

		//TileMap Creation
		ctx.tileMap = std::make_unique<EngineGame::TileMap>(ctx.mapData.w, ctx.mapData.h, ctx.mapData.tSize);

		std::vector<EngineGame::TileType> tiles;
		tiles.reserve(ctx.mapData.tiles.size());

		for (int v : ctx.mapData.tiles)
			tiles.push_back(static_cast<EngineGame::TileType>(v));

		ctx.tileMap->SetTiles(tiles);
		ctx.tileMap->LoadAssets();

		//Other Load Operations
		LoadSpawnEntities(ctx);
		LoadInteractables(ctx);
		LoadTraps(ctx);
		LoadCamera(ctx);

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Map Loaded + TileMap initialized + Player-Enemies have been spawned + Interactables-Traps spawned + Camera loaded."
		);
	}

	//Entities
	void Loader::LoadSpawnEntities(LoadContext& ctx)
	{
		for (const auto& spawn : ctx.mapData.spawns)
		{
			auto it = ctx.entityDefs.find(spawn.defId);
			if (it == ctx.entityDefs.end())
			{
				EngineCore::Log::Write(
					EngineCore::LogLevel::Warning,
					EngineCore::LogCategory::Scene,
					"Unknown entity def : " + spawn.defId
				);
				continue;
			}

			const EngineGame::EntityDefs& def = it->second;

			//Selecting is it player or enemy
			if (spawn.defId == "Player")
			{
				if (ctx.playerSpawned)
				{
					EngineCore::Log::Write(
						EngineCore::LogLevel::Warning,
						EngineCore::LogCategory::Scene,
						"Multiple player spawns found iggnoring the extra one"
					);
					continue;
				}

				LoadPlayer(ctx, spawn, def);
				ctx.playerSpawned = true;
			}
			else
			{
				LoadEnemy(ctx, spawn, def);
			}

			if (!ctx.playerSpawned)
			{
				EngineCore::Log::Write(
					EngineCore::LogLevel::Fatal,
					EngineCore::LogCategory::Scene,
					"No Player spawn found in map!"
				);
			}
		}
	}

	void Loader::LoadPlayer(LoadContext& ctx, const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def)
	{
		//Set World Reference
		ctx.player.SetWorld(ctx.tileMap.get());

		//Set Definitions
		ctx.player.ApplyDefinition(def);

		//Set Position
		EngineMath::Vector2 newPos;
		newPos.x = spawn.x * ctx.tileMap->GetTileSize();
		newPos.y = spawn.y * ctx.tileMap->GetTileSize();

		EngineCore::AABB spawnBox;
		spawnBox.SetFromPositionSize(
			newPos.x,
			newPos.y,
			ctx.player.GetColliderW(),
			ctx.player.GetColliderH()
		);

		//Wall Overlap Check
		if (ctx.tileMap->IsSolidX(spawnBox) || ctx.tileMap->IsSolidY(spawnBox, -1.0f))
		{
			EngineCore::Log::Write(
				EngineCore::LogLevel::Warning,
				EngineCore::LogCategory::Scene,
				"Player spawn is on a wall tile!"
			);
		}

		//Set position and spawn point
		ctx.player.SetPosition(newPos);
		ctx.player.SetSpawnPoint(newPos);
		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Player pos : " + std::to_string(newPos.x) + "," + std::to_string(newPos.y)
		);
	}

	void Loader::LoadEnemy(LoadContext& ctx, const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def)
	{
		//Enemy Loading
		auto enemy = std::make_unique<EngineGame::Enemy>();

		//Set World Reference
		enemy->SetWorld(ctx.tileMap.get());

		//Set Definitions
		enemy->ApplyDefinition(def);

		//Set Position
		EngineMath::Vector2 newPos;
		newPos.x = spawn.x * ctx.tileMap->GetTileSize();
		newPos.y = spawn.y * ctx.tileMap->GetTileSize();
		enemy->SetPosition(newPos);

		//Add new enemy to the array
		ctx.enemies.push_back(std::move(enemy));
	}

	//Camera
	void Loader::LoadCamera(LoadContext& ctx)
	{
		//Camera loading
		ctx.camera.SetWorldBounds(ctx.tileMap->GetWorldWidth(), ctx.tileMap->GetWorldHeight());
		ctx.camera.Follow(ctx.player.GetPosition().x, ctx.player.GetPosition().y);
		ctx.camera.SetSmoothness(20.0f);
	}

	void Loader::LoadInteractables(LoadContext& ctx)
	{
		ctx.interactableList.Clear();

		const int tileSize = ctx.tileMap->GetTileSize();

		for (const auto& s : ctx.mapData.interactables)
		{
			const InteractableDef* def = InteractableLibrary::Get().GetInteractableDef(s.defId);

			if (!def)
			{
				EngineCore::Log::Write(
					EngineCore::LogLevel::Warning,
					EngineCore::LogCategory::Scene,
					"Unknown interactable def:" + s.defId
				);
				continue;
			}

			EngineMath::Vector2 worldPos;
			worldPos.x = static_cast<float>(s.x * tileSize);
			worldPos.y = static_cast<float>(s.y * tileSize);

			EngineGame::InteractableInstance instance;
			instance.position = worldPos;
			instance.def = def;
			instance.collider.SetFromPositionSize(worldPos.x, worldPos.y, static_cast<float>(tileSize), static_cast<float>(tileSize));

			ctx.interactableList.Add(instance);
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Interactables loaded :" + std::to_string(ctx.mapData.interactables.size())
		);
	}

	void Loader::LoadTraps(LoadContext& ctx)
	{
		ctx.trapList.Clear();

		const int tileSize = ctx.tileMap->GetTileSize();

		for (const auto& s : ctx.mapData.traps)
		{
			const TrapDef* def = InteractableLibrary::Get().GetTrapDef(s.defId);

			if (!def)
			{
				EngineCore::Log::Write(
					EngineCore::LogLevel::Warning,
					EngineCore::LogCategory::Scene,
					"Unknown trap def:" + s.defId
				);
				continue;
			}

			EngineMath::Vector2 worldPos;
			worldPos.x = static_cast<float>(s.x * tileSize);
			worldPos.y = static_cast<float>(s.y * tileSize);

			EngineGame::TrapInstance trap;
			trap.def = def;
			trap.position = worldPos;

			ctx.trapList.Add(trap);
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"traps loaded :" + std::to_string(ctx.mapData.traps.size())
		);
	}
}