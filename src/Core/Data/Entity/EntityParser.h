#pragma once
#include "Core/Data/Entity/EntityData.h"
#include "json.hpp"

namespace EngineData
{
	class EntityParser
	{
	public:
		static EntityData Parse(const nlohmann::json& j)
		{
			EntityData entity;

			entity.id = j.value("Id", "");

			entity.speed = j.value("Speed", 0.f);
			entity.attackDamage = j.value("AttackDamage", 0.f);
			entity.attackInterval = j.value("AttackInterval", 0.f);
			entity.maxHp = j.value("MaxHP", 0.f);

			entity.idleAnim = j.value("IdleAnimation", "");
			entity.walkAnim = j.value("WalkAnimation", "");
			entity.hurtAnim = j.value("HurtAnimation", "");
			entity.deathAnim = j.value("DeathAnimation", "");

			if (j.contains("AttackAnimations") && j["AttackAnimations"].is_array())
				entity.attackAnims = j["AttackAnimations"].get<std::vector<std::string>>();

			return entity;
		}
	};
}