#pragma once
#include <string>
#include "Platform/LoadContext.h"

namespace EngineGame
{
	struct SpawnData;
	struct EntityDefs;
}

namespace EnginePlatform
{
	class Loader
	{
	public:
		void LoadBasics(std::unordered_map<std::string, EngineGame::EntityDefs>& entityDefs);
		void LoadCurrentLevel(LoadContext& ctx);
	private:
		void LoadMap(LoadContext& ctx, const std::string& mapId);
		void LoadSpawnEntities(LoadContext& ctx);
		void LoadPlayer(LoadContext& ctx, const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def);
		void LoadEnemy(LoadContext& ctx, const EngineGame::SpawnData& spawn, const EngineGame::EntityDefs& def);
		void LoadCamera(LoadContext& ctx);
		void LoadInteractables(LoadContext& ctx);
		void LoadTraps(LoadContext& ctx);
	};
}