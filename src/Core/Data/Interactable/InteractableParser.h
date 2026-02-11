#pragma once
#include "Core/Data/Interactable/InteractableData.h"
#include "json.hpp"


namespace EngineData
{
	class InteractableParser
	{
	public:
		static InteractableData Parse(const nlohmann::json& j)
		{
			InteractableData interactable;

			interactable.id = j.value("Id", "");
			interactable.type = j.value("Type", "");
			interactable.imagePath = j.value("ImagePath", "");

			return interactable;
		}
	};
}