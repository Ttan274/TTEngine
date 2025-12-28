#pragma once
#include "Core/Math/Vector2.h"
#include "Core/IRenderer.h"
#include "Game/Camera.h"
#include "Game/TileMap.h"
#include "Core/Animation.h"


namespace EngineGame
{
	class Entity
	{
	public:
		Entity();
		virtual ~Entity() = default;

		virtual void Update(float dt) = 0;
		virtual void Render(EngineCore::IRenderer* renderer,
			const Camera2D& camera) = 0;
		
		void SetTexture(Texture2D* idleT, Texture2D* walkT, Texture2D* hurtT, Texture2D* deadT);
		void SetWorld(TileMap* world);
		EngineMath::Vector2 GetPosition() const { return m_Position; }
		void SetPosition(EngineMath::Vector2 pos) { m_Position.x = pos.x; m_Position.y = pos.y; };
		bool IsFacingRight() const { return m_FacingRight; }
		bool IsDead() const { return m_IsDead; }

		float GetHp() const { return m_HP; }
		float GetRatio() const { return m_HP / m_MaxHP; }
		bool IsDamageFlashing() const { return m_DamageFlashTimer > 0.0f; }
		float GetDamageFlashTimer() { return m_DamageFlashTimer; }

	protected:
		bool IsCollidingWithWorld(const EngineCore::Rect& rect) const;
		void MoveAndCollide(const EngineMath::Vector2& velocity);
		void UpdateCollider();
		void CreateAnim(EngineCore::Animation* anim, float frameTime, int frameSize, bool loop);

		virtual void OnDeath() = 0;
		virtual void TakeDamage(float amount, bool objectDir) = 0;
	protected:
		//Movement
		EngineMath::Vector2 m_Position{};
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
		Texture2D* m_IdleTexture = nullptr;
		Texture2D* m_WalkTexture = nullptr;
		Texture2D* m_HurtTexture = nullptr;
		Texture2D* m_DeathTexture = nullptr;
		float m_SpriteW = 128.0f;
		float m_SpriteH = 128.0f;

		//Damage Flash
		float m_DamageFlashTimer = 0.0f;
		float m_DamageFlashDuration = 0.2f;

		//Collider
		EngineCore::Rect m_Collider{};

		//World Reference
		TileMap* m_World = nullptr;
	};
}