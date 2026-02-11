#pragma once
#include "Core/Data/Interactable/TrapData.h"
#include "json.hpp"

namespace EngineData
{
	class TrapParser
	{
	public:
		static TrapData Parse(const nlohmann::json& j)
		{
			TrapData trap;

			trap.id = j.value("Id", "");
			trap.imagePath = j.value("ImagePath", "");

			//Saw Data
			trap.speed = j.value("Speed", 0.f);
			trap.damage = j.value("Damage", 0.f);
			trap.damageCooldown = j.value("DamageCooldown", 0.f);

			//Fire Data
			trap.activeDuration = j.value("ActiveDuration", 0.f);
			trap.inactiveDuration = j.value("InactiveDuration", 0.f);
			trap.damagePerSecond = j.value("DamagePerSecond", 0.f);

			return trap;
		}
	};
}