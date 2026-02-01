#pragma once
#include "Game/Texture.h"
#include "Core/Animation.h"
#include "Game/Entity.h"
#include "Core/Input.h"

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
		void Render(EngineCore::IRenderer* renderer,
					const Camera2D& camera) override;
		void TakeDamage(float amount, bool objectDir) override;
		void ApplyDefinition(const EntityDefs& def) override;

		//Player Spesific Methods
		void Update(float dt);
		void SetSpawnPoint(const EngineMath::Vector2& pos) { m_SpawnPoint = pos; }
		void Respawn();
		void Reset();

	protected:
		//Base Class Methods
		void OnDeath() override;
		void UpdateAttack(float dt) override;
		void UpdateHurt(float dt) override;
		void UpdateDeath(float dt) override;
		void UpdatePhysics(float dt) override;

	private:
		//Player Spesific Methods
		void UpdateMovement(float dt);
		void StartAttack(AttackStage stage);
		void ResetCombo();
		void Jump();

		float Approach(float current, float target, float delta)
		{
			if (current < target)
				return std::min(current + delta, target);
			else
				return std::max(current - delta, target);
		}

	private:
		//State
		PlayerState m_State = PlayerState::Normal;

		//Animation
		std::vector<EngineCore::Animation> m_AttackAnims;

		//Sprite
		std::vector<Texture2D*> m_AttackTextures;

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

		//Physics
		float m_JumpForce = 200.0f;
		float m_CoyoteTime = 0.1f;
		float m_CoyoteTimer = 0.0f;
		float m_JumpBufferTimer = 0.0f;
		float m_JumpBufferTime = 0.1f;
		float m_Acceleration = 150.0f;
		float m_Friction = 2000.0f;
		float m_AirControl = 0.5f;
	};
}