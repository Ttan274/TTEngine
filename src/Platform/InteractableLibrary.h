#pragma once
#include <string>
#include <unordered_map>

namespace EnginePlatform
{
	struct InteractableDef
	{
		std::string id;
		std::string type;
		std::string imagePath;
	};

	struct TrapDef
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

	class InteractableLibrary
	{
	public:
		static InteractableLibrary& Get();

		bool LoadInteractableDefs(const std::string& path);
		bool LoadTrapDefs(const std::string& path);
		const InteractableDef* GetInteractableDef(const std::string& id) const;
		const TrapDef* GetTrapDef(const std::string& id) const;
	private:
		InteractableLibrary() = default;

		std::unordered_map<std::string, InteractableDef> m_Defs;
		std::unordered_map<std::string, TrapDef> m_TrapDefs;
	};
}