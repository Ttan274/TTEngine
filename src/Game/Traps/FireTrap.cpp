#include "Game/Traps/FireTrap.h"
#include "Game/Camera.h"
#include "Game/Player.h"
#include "Core/Math/Collision.h"

namespace EngineGame
{
	FireTrap::FireTrap(const EngineMath::Vector2& position, float activeDuration, float inactiveDuration, float damagePerSecond)
		: m_Position(position), m_ActiveDuration(activeDuration), m_InActiveDuration(inactiveDuration)
	{
		m_Damage = damagePerSecond;
		m_Collider.SetFromPositionSize(m_Position.x, m_Position.y, 16.0f, 16.0f);
	}

	void FireTrap::Update(float dt, Player& player) 
	{
		m_Timer += dt;

		if (m_Active)
		{
			if (EngineMath::AABBIntersectsAABB(m_Collider, player.GetCollider()))
				player.TakeDamage(m_Damage * dt, 0.0f);
			
			if (m_Timer >= m_ActiveDuration)
			{
				m_Active = false;
				m_Timer = 0.0f;
			}
		}
		else
		{
			if (m_Timer >= m_InActiveDuration)
			{
				m_Active = true;
				m_Timer = 0.0f;
			}
		}
	}

	void FireTrap::Render(EngineCore::IRenderer* renderer, const Camera2D& camera)
	{
		EngineCore::Rect r;
		r.x = m_Position.x - camera.GetX();
		r.y = m_Position.y - camera.GetY();
		r.w = m_Collider.Width();
		r.h = m_Collider.Height();

		if(m_Active)
			renderer->DrawRectOutline(r, { 255, 80, 0, 255 });
		else
			renderer->DrawRectOutline(r, { 255, 200, 0, 255 });
	}

	const EngineCore::AABB& FireTrap::GetCollider() const
	{
		return m_Collider;
	}
}