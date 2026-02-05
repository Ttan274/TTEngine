#pragma once
#include <functional>
#include <vector>
#include <memory>
#include "Game/Interactables/Interactable.h"

namespace EngineGame
{
	class InteractableManager
	{
	public:
		void Update(Player& player);
		void Render(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera);
		void DebugDraw(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera);
		
		void Add(const InteractableInstance& instance);
		void Clear();
		
		bool HasInteractableInRange() const;
		void HandleInteraction(Player& player);
		std::unique_ptr<Interactable> CreateInteractable(const InteractableInstance& instance);

		const EngineMath::Vector2 GetPosition() const { return m_Interacted->GetPosition(); }
		void SpawnKey(const EngineMath::Vector2& pos);

		//Callback
		void SetOnLevelComplete(std::function<void()> callback)
		{
			m_OnLevelComplete = std::move(callback);
		}

	private:

		std::vector<std::unique_ptr<Interactable>> m_Interactables;
		Interactable* m_Interacted = nullptr;
		std::function<void()> m_OnLevelComplete;
	};
}