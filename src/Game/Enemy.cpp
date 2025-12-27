#include "Game/Enemy.h"

namespace EngineGame
{
	Enemy::Enemy()
	{
		m_Position = { 0.0f, 0.0f };
		m_Speed = 100.0f;

		//Idle Anim
		CreateAnim(&m_IdleAnim, 0.25f, 6, true);

		//Walk Anim
		CreateAnim(&m_WalkAnim, 0.12f, 8, true);

		//Hurt Anim
		CreateAnim(&m_HurtAnim, 0.12f, 2, false);

		m_CurrentAnim = &m_IdleAnim;
		UpdateCollider();
	}

	void Enemy::CreateAnim(EngineCore::Animation* anim, float frameTime, int frameSize, bool loop)
	{
		anim->SetLoop(loop);
		anim->SetFrameTime(frameTime);
		for (int i = 0; i < frameSize; i++)
		{
			anim->AddFrame({ i * m_SpriteW, 0.0f, m_SpriteW, m_SpriteH });
		}
	}

	void Enemy::Update(float dt)
	{
		if (m_AttackCooldown > 0.0f)
			m_AttackCooldown -= dt;

		switch (m_State)
		{
		case EnemyState::Idle:
			UpdateIdle(dt);
			break;
		case EnemyState::Patrol:
			UpdatePatrol(dt);
			break;
		case EnemyState::Hurt:
			UpdateHurt(dt);
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

	void Enemy::UpdateHurt(float dt)
	{
		m_HurtTimer -= dt;
		m_CurrentAnim->Update(dt);

		if (m_HurtTimer <= 0.0f && m_CurrentAnim->IsFinished())
		{
			m_State = EnemyState::Idle;
			m_CurrentAnim = &m_IdleAnim;
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
		currentTexture = (m_CurrentAnim == &m_WalkAnim) ? m_WalkTexture : 
						 (m_CurrentAnim == &m_IdleAnim) ? m_IdleTexture : 
						 m_HurtTexture;

		renderer->DrawTexture(
			currentTexture,
			src,
			{ pX, pY, m_SpriteW, m_SpriteH },
			flip);
		
		//Collider Debug
		EngineCore::Color c = (m_State == EnemyState::Hurt) ? EngineCore::Color{ 0, 255, 0, 255 } : EngineCore::Color{ 255, 255, 255, 255 };
		renderer->DrawRectOutline(
			{
				m_Collider.x - camera.GetX(),
				m_Collider.y - camera.GetY(),
				m_Collider.w,
				m_Collider.h
			},
			c
			);
	}

	void Enemy::TakeDamage(float amount)
	{
		if (m_State == EnemyState::Hurt)
			return;

		m_HP -= amount;
		m_State = EnemyState::Hurt;
		m_HurtTimer = m_HurtDuration;
		m_HurtAnim.Reset();
		m_CurrentAnim = &m_HurtAnim;
	}
}