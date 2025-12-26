#include "Game/Enemy.h"

namespace EngineGame
{
	Enemy::Enemy()
	{
		m_Position = { 0.0f, 0.0f };
		m_Speed = 100.0f;

		//Idle Anim
		m_IdleAnim.SetFrameTime(0.25f);
		for (int i = 0; i < 6; i++)
		{
			m_IdleAnim.AddFrame({ i * m_SpriteW, 0.0f, m_SpriteW, m_SpriteH });
		}

		//Walk Anim
		m_WalkAnim.SetFrameTime(0.12f);
		for (int i = 0; i < 8; i++)
		{
			m_WalkAnim.AddFrame({ i * m_SpriteW, 0.0f, m_SpriteW, m_SpriteH });
		}

		m_CurrentAnim = &m_IdleAnim;
		UpdateCollider();
	}

	void Enemy::Update(float dt)
	{
		switch (m_State)
		{
		case EnemyState::Idle:
			UpdateIdle(dt);
			break;
		case EnemyState::Patrol:
			UpdatePatrol(dt);
			break;
		}

		UpdateCollider();
	}

	void Enemy::UpdateIdle(float dt)
	{
		m_StateTimer += dt;
		m_IsMoving = false;

		m_CurrentAnim = &m_IdleAnim;
		m_CurrentAnim->Update(dt);

		if (m_StateTimer >= m_IdleDuration)
			ChangeState(EnemyState::Patrol);
	}

	void Enemy::UpdatePatrol(float dt)
	{
		m_StateTimer += dt;
		m_IsMoving = true;

		m_CurrentAnim = &m_WalkAnim;
		m_CurrentAnim->Update(dt);

		EngineMath::Vector2 move =
		{
			m_Direction * m_Speed * dt,
			0.0f
		};

		MoveAndCollide(move);

		//Flipping
		if (m_StateTimer >= m_PatrolDuration)
		{
			m_FacingRight = !m_FacingRight;
			m_Direction *= -1.0f;
			ChangeState(EnemyState::Idle);
		}
	}

	void Enemy::ChangeState(EnemyState newState)
	{
		m_State = newState;
		m_StateTimer = 0.0f;
	}

	void Enemy::Render(EngineCore::IRenderer* renderer,
		const Camera2D& camera)
	{
		float pX = m_Position.x - camera.GetX();
		float pY = m_Position.y - camera.GetY();

		SDL_FRect sldRect = m_CurrentAnim->GetCurrentFrame();
		EngineCore::Rect src =
		{
			sldRect.x,
			sldRect.y,
			sldRect.w,
			sldRect.h
		};
		EngineCore::SpriteFlip flip = m_FacingRight ? EngineCore::SpriteFlip::None : EngineCore::SpriteFlip::Horizontal;

		Texture2D* currentTexture = nullptr;
		currentTexture = (m_CurrentAnim == &m_WalkAnim) ? m_WalkTexture : m_IdleTexture;

		renderer->DrawTexture(
			currentTexture,
			src,
			{ pX, pY, m_SpriteW, m_SpriteH },
			flip);
		
		//Collider Debug
		renderer->DrawRectOutline(
			{
				m_Collider.x - camera.GetX(),
				m_Collider.y - camera.GetY(),
				m_Collider.w,
				m_Collider.h
			},
			{ 255, 0, 0, 255 });
	}
}