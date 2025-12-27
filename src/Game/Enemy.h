#pragma once
#include "Game/Texture.h"
#include "Core/Animation.h"
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

		void Update(float dt) override;
		void Render(EngineCore::IRenderer* renderer,
			const Camera2D& camera) override;

		void SetTexture(Texture2D* idleT, Texture2D* walkT, Texture2D* hurtT, Texture2D* deadT) {
			m_IdleTexture = idleT;
			m_WalkTexture = walkT;
			m_HurtTexture = hurtT;
			m_DeathTexture = deadT;
		}

		void SetPosition(float px, float py) { m_Position.x = px; m_Position.y = py; }
		EngineMath::Vector2 GetPosition() const { return m_Position; }
		const EngineCore::Rect& GetCollider() const { return m_Collider; }
		bool CanAttack() const { return m_AttackCooldown <= 0.0f; }
		void ResetCooldown() { m_AttackCooldown = m_AttackInterval; }
		void TakeDamage(float amount, bool playerRight);
		void OnDeath() override;
		bool IsDead() const { return m_IsDead; }

	private:
		void UpdateIdle(float dt);
		void UpdatePatrol(float dt);
		void UpdateHurt(float dt);
		void UpdateDeath(float dt);
		void ChangeState(EnemyState newState);
		void CreateAnim(EngineCore::Animation* anim, float frameTime, int frameSize, bool loop);
	private:
		//State
		EnemyState m_State = EnemyState::Idle;
		float m_StateTimer = 0.0f;
		float m_IdleDuration = 1.5f;
		float m_PatrolDuration = 3.0f;

		//Animation
		EngineCore::Animation m_IdleAnim;
		EngineCore::Animation m_WalkAnim;
		EngineCore::Animation m_HurtAnim;
		EngineCore::Animation m_DeathAnim;
		EngineCore::Animation* m_CurrentAnim = nullptr;
		bool m_IsMoving = false;

		//Sprite
		EngineGame::Texture2D* m_IdleTexture = nullptr;
		EngineGame::Texture2D* m_WalkTexture = nullptr;
		EngineGame::Texture2D* m_HurtTexture = nullptr;
		EngineGame::Texture2D* m_DeathTexture = nullptr;
		float m_SpriteW = 128.0f;
		float m_SpriteH = 128.0f;

		//Patrol direction
		float m_Direction = 1.0f;

		//Attack Cooldown
		float m_AttackCooldown = 0.0f;
		float m_AttackInterval = 1.0f;

		//Hurt Cooldown
		float m_HurtTimer = 0.0f;
		float m_HurtDuration = 0.3f;

		//Knockback
		EngineMath::Vector2 m_KnockbackVel{ 0, 0 };
		float m_KnockbackTimer = 0.0f;
		float m_KnockbackDuration = 0.15f;

		//Dead
		bool m_IsDead = false;
	};
}