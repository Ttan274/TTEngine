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

	class InteractableLibrary
	{
	public:
		static InteractableLibrary& Get();

		bool LoadDefs(const std::string& path);
		const InteractableDef* GetDef(const std::string& id) const;
	private:
		InteractableLibrary() = default;

		std::unordered_map<std::string, InteractableDef> m_Defs;
	};
}