#pragma once
#include <string>
#include "Platform/LoadContext.h"

namespace EngineData
{
	struct EntityData;
	struct SpawnData;
}

namespace EnginePlatform
{
	class Loader
	{
	public:
		void LoadBasics();
		void LoadCurrentLevel(LoadContext& ctx);
	private:
		void LoadMap(LoadContext& ctx, const std::string& mapId);
		void LoadSpawnEntities(LoadContext& ctx);
		void LoadPlayer(LoadContext& ctx, const EngineData::SpawnData& spawn, const EngineData::EntityData& def);
		void LoadEnemy(LoadContext& ctx, const EngineData::SpawnData& spawn, const EngineData::EntityData& def);
		void LoadCamera(LoadContext& ctx);
		void LoadInteractables(LoadContext& ctx);
		void LoadTraps(LoadContext& ctx);
	};
}