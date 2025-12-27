#pragma once
#include "Game/Texture.h"
#include "Core/Input.h"
#include "Core/Animation.h"
#include "Game/Entity.h"

namespace EngineGame
{
	class Player : public Entity
	{
	public:
		Player();

		void Update(float dt) override;
		void Render(EngineCore::IRenderer* renderer,
					const Camera2D& camera) override;

		void SetTexture(Texture2D* idleT, Texture2D* walkT, Texture2D* aT1, Texture2D* aT2, Texture2D* aT3);
		
		const EngineCore::Rect& GetCollider() const { return m_Collider; }
		EngineMath::Vector2 GetPosition() const { return m_Position; }
		void SetPosition(float px, float py) { m_Position.x = px; m_Position.y = py; }
		
		EngineCore::Rect GetAttackBox() const;
		bool IsAttacking() const { return m_IsAttacking;}
		float GetAttackDamage() const { return m_AttackDamage; }
		bool IsDamageFrame() const;

		bool HasHitThisAttack() const { return m_HasHitThisAttack; }
		void MarkHitDone() { m_HasHitThisAttack = true; }
	
	private:
		void UpdateMovement(float dt);
		void UpdateAttack(float dt);
		void CreateAnim(EngineCore::Animation* anim, float frameTime, int frameSize, bool loop);
	private:
		//Animation
		EngineCore::Animation m_IdleAnim;
		EngineCore::Animation m_WalkAnim;
		std::vector<EngineCore::Animation> m_AttackAnims;
		int m_CurrentAttackAnimIndex = 0;
		EngineCore::Animation* m_CurrentAnim = nullptr;
		bool m_IsMoving = false;

		//Sprite
		Texture2D* m_IdleTexture = nullptr;
		Texture2D* m_WalkTexture = nullptr;
		std::vector<Texture2D*> m_AttackTextures;
		float m_SpriteW = 128.0f;
		float m_SpriteH = 128.0f;

		//Attack
		bool m_IsAttacking = false;
		float m_AttackCooldown = 0.0f;
		float m_AttackInterval = 0.5f;
		float m_AttackDamage = 10.0f;
		bool m_HasHitThisAttack = false;
	};
}