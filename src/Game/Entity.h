#pragma once
#include "Core/Math/Vector2.h"
#include "Core/IRenderer.h"
#include "Game/Camera.h"
#include "Game/TileMap.h"
#include "Core/Animation.h"
#include "Core/Data/Entity/EntityData.h"

namespace EngineGame
{
	class Entity
	{
	public:
		Entity();
		virtual ~Entity() = default;

		//Virtual methods
		virtual void Render(EngineCore::IRenderer* renderer,
			const Camera2D& camera) = 0;
		virtual void ApplyDefinition(const EngineData::EntityData& def) = 0;

		//Basic Methods
		void SetWorld(TileMap* world);
		
		//Position
		void SetPosition(EngineMath::Vector2 pos) { m_Position.x = pos.x; m_Position.y = pos.y; };
		EngineMath::Vector2 GetPosition() const { return m_Position; }
		
		//Colliders
		const EngineCore::AABB& GetCollider() const { return m_Collider; }
		EngineCore::Rect GetAttackBox() const;
		float GetColliderW() const { return m_ColliderWidth; }
		float GetColliderH() const { return m_ColliderHeight; }

		//Direction
		bool IsFacingRight() const { return m_FacingRight; }
		
		//Hp
		float GetHp() const { return m_HP; }
		float GetRatio() const { return m_HP / m_MaxHP; }
		bool IsDead() const { return m_IsDead; }

		//Damage Flashing
		float GetDamageFlashTimer() const { return m_DamageFlashTimer; }
		bool IsDamageFlashing() const { return m_DamageFlashTimer > 0.0f; }
		
		//Attacking
		bool IsDamageFrame() const;
		bool IsAttacking() const { return m_IsAttacking; }
		bool HasHitThisAttack() const { return m_HasHitThisAttack; }
		float GetAttackDamage() const { return m_AttackDamage; }
		void MarkHitDone() { m_HasHitThisAttack = true; }

	protected:
		void MoveAndCollide(const EngineMath::Vector2& velocity);
		void UpdateCollider();

		virtual void OnDeath() = 0;
		virtual void TakeDamage(float amount, float objectDir) = 0;
		virtual void UpdateAttack(float dt) = 0;
		virtual void UpdateHurt(float dt) = 0;
		virtual void UpdateDeath(float dt) = 0;
		virtual void UpdatePhysics(float dt) = 0;

	protected:
		//Movement
		EngineMath::Vector2 m_Position{};
		EngineMath::Vector2 m_Velocity{};
		float m_Speed = 0.0f;
		bool m_FacingRight = true;

		//Hp
		float m_MaxHP = 100;
		float m_HP = 100;
		bool m_IsDead = false;

		//Hurt Cooldown
		float m_HurtTimer = 0.0f;
		float m_HurtDuration = 0.3f;

		//Animation
		EngineCore::Animation m_IdleAnim;
		EngineCore::Animation m_WalkAnim;
		EngineCore::Animation m_HurtAnim;
		EngineCore::Animation m_DeathAnim;
		EngineCore::Animation* m_CurrentAnim = nullptr;
		bool m_IsMoving = false;

		//Sprite
		float m_SpriteW = 128.0f;
		float m_SpriteH = 128.0f;

		//Attack
		bool m_IsAttacking = false;
		float m_AttackCooldown = 0.0f;
		float m_AttackInterval = 1.0f;
		float m_AttackDamage = 10.0f;
		bool m_HasHitThisAttack = false;

		//Damage Flash
		float m_DamageFlashTimer = 0.0f;
		float m_DamageFlashDuration = 0.2f;

		//Collider
		EngineCore::AABB m_Collider{};
		float m_ColliderWidth = 32.0f;
		float m_ColliderHeight = 64.0f;
		bool m_IsGrounded = false;
		bool m_ColliderEnabled = true;

		//Physics
		float m_Gravity = 200.0f;
		float m_MaxFallSpeed = 800.0f;

		//World Reference
		TileMap* m_World = nullptr;
	};
}