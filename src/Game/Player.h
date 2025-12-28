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
		void Render(EngineCore::IRenderer* renderer,
					const Camera2D& camera) override;
		void TakeDamage(float amount, bool objectDir) override;

		//Player Spesific Methods
		void Update(float dt);
		void SetAttackTexture(Texture2D* aT1, Texture2D* aT2, Texture2D* aT3);
		void SetSpawnPoint(const EngineMath::Vector2& pos) { m_SpawnPoint = pos; }
		void Respawn();

	protected:
		//Base Class Methods
		void OnDeath() override;
		void UpdateAttack(float dt) override;
		void UpdateHurt(float dt) override;
		void UpdateDeath(float dt) override;

	private:
		//Player Spesific Methods
		void UpdateMovement(float dt);
		void StartAttack(AttackStage stage);
		void ResetCombo();
		
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
	};
}