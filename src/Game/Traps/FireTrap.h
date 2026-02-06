#pragma once
#include "Game/Traps/Trap.h"

namespace EngineGame
{
	class FireTrap : public Trap
	{
	public:
		FireTrap(const EngineMath::Vector2& position, float activeDuration, float inactiveDuration, float damagePerSecond);
		void Update(float dt, Player& player) override;
		void Render(EngineCore::IRenderer* renderer, const Camera2D& camera) override;
		const EngineCore::AABB& GetCollider() const override;
	private:
		EngineMath::Vector2 m_Position;
		EngineCore::AABB m_Collider;

		float m_InActiveDuration = 0.0f;
		float m_ActiveDuration = 0.0f;

		float m_Timer = 0.0f;
		bool m_Active = false;
	};
}