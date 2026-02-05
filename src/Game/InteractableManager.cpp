#include "Game/InteractableManager.h"
#include "Core/Math/Collision.h"
#include "Game/Interactables/KeyInteractable.h"
#include "Game/Interactables/DoorInteractable.h"
#include "Game/Interactables/ChestInteractable.h"
#include "Core/Log.h"

namespace EngineGame
{
	void InteractableManager::Update(Player& player)
	{
		m_Interacted = nullptr;
		const EngineCore::AABB& playerCol = player.GetCollider();

		for (auto& it : m_Interactables)
		{
			if (it->IsUsed())
				continue;

			if (EngineMath::AABBIntersectsAABB(playerCol, it->GetCollider()))
			{
				m_Interacted = it.get();
				return;
			}
		}
	}

	void InteractableManager::Render(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera)
	{
		for (auto& it : m_Interactables)
			it->Render(renderer, camera);
	}

	void InteractableManager::DebugDraw(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera)
	{
		for (auto& it : m_Interactables)
			it->DebugDraw(renderer, camera);
	}

	void InteractableManager::Clear()
	{
		m_Interactables.clear();
	}

	void InteractableManager::Add(const InteractableInstance& instance)
	{
		auto interactable = CreateInteractable(instance);
		if (interactable)
			m_Interactables.push_back(std::move(interactable));
	}

	std::unique_ptr<Interactable> InteractableManager::CreateInteractable(const InteractableInstance& instance)
	{
		const std::string& id = instance.def->id;

		if (id == "Key")
			return std::make_unique<KeyInteractable>(instance);
		if(id == "Door")
			return std::make_unique<DoorInteractable>(instance);
		if(id == "Chest")
			return std::make_unique<ChestInteractable>(instance);

		return nullptr;
	}

	bool InteractableManager::HasInteractableInRange() const
	{
		return m_Interacted != nullptr;
	}

	void InteractableManager::HandleInteraction(Player& player)
	{
		if (!m_Interacted)
			return;
		InteractResult result =	m_Interacted->OnInteract(player);

		switch (result)
		{
		case InteractResult::SpawnKey:
			SpawnKey(m_Interacted->GetPosition());
			break;
		case InteractResult::FinishGame:
			if (m_OnLevelComplete)
				m_OnLevelComplete();
			break;
		}
	}

	void InteractableManager::SpawnKey(const EngineMath::Vector2& pos)
	{
		const EnginePlatform::InteractableDef* keyDef =
			EnginePlatform::InteractableLibrary::Get().GetDef("Key");

		if (!keyDef)
			return;

		InteractableInstance instance;
		instance.def = keyDef;
		instance.position = pos + EngineMath::Vector2(16.0f, 0.0f);
		instance.collider.SetFromPositionSize(instance.position.x, instance.position.y, 50, 50);
		instance.used = false;

		Add(instance);
	}
}