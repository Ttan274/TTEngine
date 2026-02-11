#pragma once
#include <string>

namespace EngineData
{
	struct TrapData
	{
		std::string id;
		std::string imagePath;

		//Fire Def
		float activeDuration;
		float inactiveDuration;
		float damagePerSecond;

		//Saw Def
		float speed;
		float damage;
		float damageCooldown;
	};
}