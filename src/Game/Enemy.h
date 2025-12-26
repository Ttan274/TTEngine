#pragma once
#include "Game/Texture.h"
#include "Core/Animation.h"
#include "Game/Entity.h"

namespace EngineGame
{
	enum class EnemyState
	{
		Idle,
		Patrol
	};

	class Enemy : public Entity
	{
	public:
		Enemy();

		void Update(float dt) override;
		void Render(EngineCore::IRenderer* renderer,
			const Camera2D& camera) override;

		void SetTexture(Texture2D* idleT, Texture2D* walkT) {
			m_IdleTexture = idleT;
			m_WalkTexture = walkT;
		}

		void SetPosition(float px, float py) { m_Position.x = px; m_Position.y = py; }
		EngineMath::Vector2 GetPosition() const { return m_Position; }

	private:
		void UpdateIdle(float dt);
		void UpdatePatrol(float dt);
		void ChangeState(EnemyState newState);
	private:
		//State
		EnemyState m_State = EnemyState::Idle;
		float m_StateTimer = 0.0f;
		float m_IdleDuration = 1.5f;
		float m_PatrolDuration = 3.0f;

		//Animation
		EngineCore::Animation m_IdleAnim;
		EngineCore::Animation m_WalkAnim;
		EngineCore::Animation* m_CurrentAnim = nullptr;
		bool m_IsMoving = false;

		//Sprite
		EngineGame::Texture2D* m_IdleTexture = nullptr;
		EngineGame::Texture2D* m_WalkTexture = nullptr;
		float m_SpriteW = 128.0f;
		float m_SpriteH = 128.0f;

		//Patrol direction
		float m_Direction = 1.0f;
	};
}