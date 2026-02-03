#pragma once
#include <vector>
#include "Core/Math/Vector2.h"
#include "Core/AABB.h"
#include "Platform/InteractableLibrary.h"
#include "Core/IRenderer.h"
#include "Game/Camera.h"

namespace EngineGame
{
	struct InteractableInstance
	{
		const EnginePlatform::InteractableDef* def = nullptr;
		EngineMath::Vector2 position;
		EngineCore::AABB collider;
	};

	class Player;
	class InteractableManager
	{
	public:
		void Update(Player& player);
		void Clear();
		void Add(const InteractableInstance& instance);
		void DebugDraw(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera);
		bool HasInteractableInRange() const;
		const InteractableInstance* GetInteracted() const { return m_Interacted; }

	private:
		void HandleInteraction(const InteractableInstance& instance);

		std::vector<InteractableInstance> m_Interactables;
		const InteractableInstance* m_Interacted = nullptr;
	};
}