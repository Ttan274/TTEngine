#pragma once
#include "Game/Traps/Trap.h"

namespace EngineGame
{
	class SawTrap : public Trap
	{
	public:
		SawTrap(const EngineMath::Vector2& a, const EngineMath::Vector2& b, float speed, float damage, float damageCooldown);
		void Update(float dt, Player& player) override;
		void Render(EngineCore::IRenderer* renderer, const Camera2D& camera) override;
		const EngineCore::AABB& GetCollider() const override;

	private:
		EngineMath::Vector2 m_PointA;
		EngineMath::Vector2 m_PointB;
		EngineMath::Vector2 m_Position;

		float m_Speed = 0.0f;
		float m_Direction = 1.0f;

		float m_DamageCooldown = 0.0f;
		float m_DamageTimer = 0.0f;

		EngineCore::AABB m_Collider;
	};
}