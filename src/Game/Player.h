#pragma once
#include "Game/Texture.h"
#include "Core/Input.h"
#include "Core/Animation.h"
#include "Game/Entity.h"

namespace EngineGame
{
	enum class AttackStage
	{
		None,
		Attack1,
		Attack2,
		Attack3
	};

	enum class PlayerState
	{
		Normal,
		Hurt,
		Dead
	};

	class Player : public Entity
	{
	public:
		Player();

		//Base class methods
		void Update(float dt) override;
		void Render(EngineCore::IRenderer* renderer,
					const Camera2D& camera) override;
		void OnDeath() override;
		void TakeDamage(float amount, bool objectDir) override;

		void SetAttackTexture(Texture2D* aT1, Texture2D* aT2, Texture2D* aT3);
		
		const EngineCore::Rect& GetCollider() const { return m_Collider; }
		
		
		EngineCore::Rect GetAttackBox() const;
		bool IsAttacking() const { return m_IsAttacking;}
		float GetAttackDamage() const { return m_AttackDamage; }
		bool IsDamageFrame() const;
		void SetSpawnPoint(const EngineMath::Vector2& pos) { m_SpawnPoint = pos; }

		bool HasHitThisAttack() const { return m_HasHitThisAttack; }
		void MarkHitDone() { m_HasHitThisAttack = true; }
	
	private:
		void UpdateMovement(float dt);
		void UpdateAttack(float dt);
		void UpdateHurt(float dt);
		void UpdateDeath(float dt);
		void StartAttack(AttackStage stage);
		void ResetCombo();
		void Respawn();
	private:
		//State
		PlayerState m_State = PlayerState::Normal;

		//Animation
		std::vector<EngineCore::Animation> m_AttackAnims;

		//Sprite
		std::vector<Texture2D*> m_AttackTextures;

		//Attack
		bool m_IsAttacking = false;
		float m_AttackCooldown = 0.0f;
		float m_AttackInterval = 0.5f;
		float m_AttackDamage = 10.0f;
		bool m_HasHitThisAttack = false;

		//Attack Combo
		AttackStage m_AttackStage = AttackStage::None;
		bool m_ComboQueued = false;
	
		//Knockback
		EngineMath::Vector2 m_KnockbackVel{ 0, 0 };

		//Dead
		float m_DeathTimer = 0.0f;
		float m_DeathDuration = 1.0f;

		//SpawnPoint
		EngineMath::Vector2 m_SpawnPoint;
	};
}