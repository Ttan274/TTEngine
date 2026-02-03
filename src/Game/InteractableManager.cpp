#include "Game/InteractableManager.h"
#include "Game/Player.h"
#include "Core/Math/Collision.h"
#include "Core/Log.h"

namespace EngineGame
{
	void InteractableManager::Update(Player& player)
	{
		m_Interacted = nullptr;

		const EngineCore::AABB& playerCol = player.GetCollider();

		for (const auto& it : m_Interactables)
		{
			if (!it.def)
				continue;

			if (EngineMath::AABBIntersectsAABB(playerCol, it.collider))
			{
				m_Interacted = &it;
				return;
			}
		}
	}

	bool InteractableManager::HasInteractableInRange() const
	{
		return m_Interacted != nullptr;
	}

	void InteractableManager::HandleInteraction(const InteractableInstance& instance)
	{
		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Interacted with me"
		);
	}

	void InteractableManager::Clear()
	{
		m_Interactables.clear();
	}

	void InteractableManager::Add(const InteractableInstance& instance)
	{
		m_Interactables.push_back(instance);
	}

	void InteractableManager::DebugDraw(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera)
	{
		for (const auto& it : m_Interactables)
		{
			EngineCore::Rect r;
			r.x = it.collider.Left() - camera.GetX();
			r.y = it.collider.Top() - camera.GetY();
			r.w = it.collider.Width();
			r.h = it.collider.Height();

			renderer->DrawRectOutline(r, { 0, 255, 255, 255 });
		}
	}
}