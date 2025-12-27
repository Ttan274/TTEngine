#pragma once
#include "Game/Entity.h"

namespace EngineGame
{
	enum class EnemyState
	{
		Idle,
		Patrol,
		Hurt,
		Dead
	};

	class Enemy : public Entity
	{
	public:
		Enemy();

		//Base class methods
		void Update(float dt) override;
		void Render(EngineCore::IRenderer* renderer,
			const Camera2D& camera) override;
		void OnDeath() override;
		void TakeDamage(float amount, bool objectDir) override;

		const EngineCore::Rect& GetCollider() const { return m_Collider; }
		bool CanAttack() const { return m_AttackCooldown <= 0.0f; }
		void ResetCooldown() { m_AttackCooldown = m_AttackInterval; }

	private:
		void UpdateIdle(float dt);
		void UpdatePatrol(float dt);
		void UpdateHurt(float dt);
		void UpdateDeath(float dt);
		void ChangeState(EnemyState newState);
	private:
		//State
		EnemyState m_State = EnemyState::Idle;
		float m_StateTimer = 0.0f;
		float m_IdleDuration = 1.5f;
		float m_PatrolDuration = 3.0f;
		float m_Direction = 1.0f;

		//Attack Cooldown
		float m_AttackCooldown = 0.0f;
		float m_AttackInterval = 1.0f;

		//Knockback
		EngineMath::Vector2 m_KnockbackVel{ 0, 0 };
		float m_KnockbackTimer = 0.0f;
		float m_KnockbackDuration = 0.15f;
	};
}