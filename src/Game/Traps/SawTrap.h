#pragma once
#include "Game/Traps/Trap.h"
#include "Core/Math/Collision.h"
#include "Game/Player.h"

namespace EngineGame
{
	class SawTrap : public Trap
	{
	public:
		explicit SawTrap(const TrapInstance& instance) : Trap(instance) 
		{
			m_PointA = GetPosition();
			m_PointB = GetPosition() + EngineMath::Vector2(10.0f, 0.0f);

			m_Instance.collider.SetFromPositionSize(GetPosition().x, GetPosition().y, 16.0f, 16.0f);
		}
		
		void Update(float dt, Player& player) override
		{
			//Movement
			EngineMath::Vector2 target = (m_Direction > 0.0f) ? m_PointB : m_PointA;

			EngineMath::Vector2 dir = target - GetPosition();
			float dist = dir.Length();

			if (dist < 1.0f)
			{
				m_Direction *= -1.0f;
			}
			else
			{
				dir.Normalize();
				m_Instance.position += dir * m_Instance.def->speed * dt;
			}

			m_Instance.collider.SetFromPositionSize(GetPosition().x , GetPosition().y , 16.0f, 16.0f);

			//Damage-Collision
			if (m_DamageTimer > 0.0f)
				m_DamageTimer -= dt;

			if (EngineMath::AABBIntersectsAABB(GetCollider(), player.GetCollider()))
			{
				if (m_DamageTimer <= 0.0f)
				{
					player.TakeDamage(m_Instance.def->damage, m_Direction);
					m_DamageTimer = m_Instance.def->damageCooldown;
				}
			}
		}
		

	private:
		EngineMath::Vector2 m_PointA;
		EngineMath::Vector2 m_PointB;

		float m_Direction = 1.0f;
		float m_DamageTimer = 0.0f;
	};
}