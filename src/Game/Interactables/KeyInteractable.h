#pragma once
#include "Game/Interactables/Interactable.h"

namespace EngineGame
{
	class KeyInteractable : public Interactable
	{
	public:
		explicit KeyInteractable(const InteractableInstance& instance)
			: Interactable(instance)
		{
		}

		InteractResult OnInteract(Player& player) override
		{
			if (m_Instance.used)
				return InteractResult::None;

			player.TakeKey();
			m_Instance.used = true;
			return InteractResult::None;
		}
	};
}