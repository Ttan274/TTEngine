#pragma once
#include <string>
#include <vector>

namespace EngineData
{
	struct EntityData
	{
		std::string id;
		float speed;
		float attackDamage;
		float attackInterval;
		float maxHp;
		std::string idleAnim;
		std::string walkAnim;
		std::string hurtAnim;
		std::string deathAnim;
		std::vector<std::string> attackAnims;
	};
}