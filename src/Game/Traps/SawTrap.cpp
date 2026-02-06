#include "Game/Traps/SawTrap.h"
#include "Game/Camera.h"
#include "Core/Math/Collision.h"
#include "Game/Player.h"

namespace EngineGame
{
	SawTrap::SawTrap(const EngineMath::Vector2& a, const EngineMath::Vector2& b, float speed, float damage, float damageCooldown)
		:m_PointA(a), m_PointB(b), m_Speed(speed), m_DamageCooldown(damageCooldown)
	{
		m_Position = a;
		m_Damage = damage;

		m_Collider.SetFromPositionSize(m_Position.x, m_Position.y, 16.0f, 16.0f);
	}

	void SawTrap::Update(float dt, Player& player) 
	{
		//Movement
		EngineMath::Vector2 target = (m_Direction > 0.0f) ? m_PointB : m_PointA;

		EngineMath::Vector2 dir = target - m_Position;
		float dist = dir.Length();

		if (dist < 1.0f)
		{
			m_Direction *= -1.0f;
		}
		else
		{
			dir.Normalize();
			m_Position += dir * m_Speed * dt;
		}

		m_Collider.SetFromPositionSize(m_Position.x, m_Position.y, 16.0f, 16.0f);
		
		//Damage
		if (m_DamageTimer > 0.0f)
			m_DamageTimer -= dt;

		if (EngineMath::AABBIntersectsAABB(m_Collider, player.GetCollider()))
		{
			if (m_DamageTimer <= 0.0f)
			{
				player.TakeDamage(m_Damage, m_Direction);
				m_DamageTimer = m_DamageCooldown;
			}
		}
	}

	void SawTrap::Render(EngineCore::IRenderer* renderer, const Camera2D& camera)
	{
		EngineCore::Rect r;
		r.x = m_Position.x - camera.GetX();
		r.y = m_Position.y - camera.GetY();
		r.w = m_Collider.Width();
		r.h = m_Collider.Height();

		renderer->DrawRectOutline(r, { 255, 0, 0, 255 });
	}

	const EngineCore::AABB& SawTrap::GetCollider() const
	{ 
		return m_Collider; 
	}
}