#pragma once
#include "Game/Interactables/Interactable.h"

namespace EngineGame
{
	class DoorInteractable : public Interactable
	{
	public:
		explicit DoorInteractable(const InteractableInstance& instance)
			: Interactable(instance)
		{
		}

		InteractResult OnInteract(Player& player) override
		{
			if (m_Instance.used)
				return InteractResult::None;

			if (!player.HasKey())
				return InteractResult::None;

			m_Instance.used = true;
			return InteractResult::FinishGame;
		}
	};
}