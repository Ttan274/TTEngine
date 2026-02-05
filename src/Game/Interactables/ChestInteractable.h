#pragma once
#include "Game/Interactables/Interactable.h"

namespace EngineGame
{
	class ChestInteractable : public Interactable
	{
	public:
		explicit ChestInteractable(const InteractableInstance& instance)
			: Interactable(instance)
		{
		}

		InteractResult OnInteract(Player& player) override
		{
			if (m_Instance.used)
				return InteractResult::None;

			m_Instance.used = true;
			return InteractResult::SpawnKey;
		}
	};
}