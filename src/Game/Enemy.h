#pragma once
#include "Game/Entity.h"
#include <functional>

namespace EngineGame
{
	enum class EnemyState
	{
		Idle,
		Patrol,
		Chase,
		Attack,
		Hurt,
		Dead
	};

	class Enemy : public Entity
	{
	public:
		Enemy();

		//Base class methods
		void Render(EngineCore::IRenderer* renderer,
			const Camera2D& camera) override;
		void TakeDamage(float amount, bool objectDir) override;
		void ApplyDefinition(const EntityDefs& def) override;

		//Enemy Spesific Methods
		void Update(float dt, const EngineMath::Vector2& playerPos, const EngineCore::AABB& playerCollider);
		bool CanAttack(const EngineMath::Vector2& playerPos, 
					   const EngineCore::AABB& playerCollider) const;
		bool CanDealDamage() const { return m_CanDealDamage; }
	
		//Debug Helper
		std::string GetStateName() const;
		std::function<void()> OnKilled;
	protected:
		//Base Class Methods
		void OnDeath() override;
		void UpdateHurt(float dt) override;
		void UpdateDeath(float dt) override;
		void UpdateAttack(float dt) override;
		void UpdatePhysics(float dt) override;

	private:
		//Enemy Spesific Methods
		void UpdateIdle(float dt);
		void UpdatePatrol(float dt);
		void UpdateChase(float dt, const EngineMath::Vector2& playerPos,
			const EngineCore::AABB& playerCollider);
		void ChangeState(EnemyState newState);
		bool IsPlayerInDetectionRange(const EngineMath::Vector2& playerPos) const;
		bool CanMoveForward() const;

	private:
		//State
		EnemyState m_State = EnemyState::Idle;
		float m_StateTimer = 0.0f;
		float m_IdleDuration = 1.5f;
		float m_PatrolDuration = 3.0f;
		float m_Direction = 1.0f;

		//Animation
		EngineCore::Animation m_AttackAnim;

		//Attack Detection
		EngineMath::Vector2 m_AttackRange = { 60.0f, 20.0f };
		EngineMath::Vector2 m_DetectRange = { 200.0f, 20.0f };
		float m_DetectRangeX = 200.0f;

		//Attack Wind Up
		float m_AttackWindUpTimer = 0.0f;
		float m_AttackWindUpTime = 0.2f;
		bool m_CanDealDamage = false;

		//Knockback
		EngineMath::Vector2 m_KnockbackVel{ 0, 0 };
		float m_KnockbackTimer = 0.0f;
		float m_KnockbackDuration = 0.15f;
	};
}