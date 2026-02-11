#pragma once
#include "Game/Traps/Trap.h"
#include "Core/Math/Collision.h"
#include "Game/Player.h"

namespace EngineGame
{
	class FireTrap : public Trap
	{
	public:
		explicit FireTrap(const TrapInstance& instance) : Trap(instance) 
		{
			m_Instance.collider.SetFromPositionSize(GetPosition().x, GetPosition().y, 16.0f, 16.0f);
		}
		
		void Update(float dt, Player& player) override
		{
			m_Timer += dt;

			if (m_Active && m_Timer >= m_Instance.def->activeDuration)
			{
				m_Active = false;
				m_Timer = 0.0f;
			}
			else if (!m_Active && m_Timer >= m_Instance.def->inactiveDuration)
			{
				m_Active = true;
				m_Timer = 0.0f;
			}
		
			if (!m_Active)
				return;

			if (EngineMath::AABBIntersectsAABB(GetCollider(), player.GetCollider()))
				player.TakeDamage(m_Instance.def->damagePerSecond * dt, 0.0f);
		}
		
	private:
		float m_Timer = 0.0f;
		bool m_Active = false;
	};
}