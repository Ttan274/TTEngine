#include "Game/Enemy.h"
#include "Core/Math/Collision.h"

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

		//Dead Anim
		CreateAnim(&m_DeathAnim, 0.3f, 3, false);

		//Attack Anim
		CreateAnim(&m_AttackAnim, 0.05f, 6, false);

		m_CurrentAnim = &m_IdleAnim;
		UpdateCollider();
	}

	void Enemy::Update(float dt, const EngineMath::Vector2& playerPos, const EngineCore::AABB& playerColldier)
	{
		if (m_AttackCooldown > 0.0f)
			m_AttackCooldown -= dt;

		if (m_DamageFlashTimer > 0.0f)
			m_DamageFlashTimer -= dt;

		if (CanAttack(playerPos, playerColldier))
			ChangeState(EnemyState::Attack);

		switch (m_State)
		{
		case EnemyState::Idle:
			UpdateIdle(dt);
			break;
		case EnemyState::Patrol:
			UpdatePatrol(dt);
			break;
		case EnemyState::Attack:
			UpdateAttack(dt);
			break;
		case EnemyState::Hurt:
			UpdateHurt(dt);
			break;
		case  EnemyState::Dead:
			UpdateDeath(dt);
			break;
		}

		UpdatePhysics(dt);
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

		switch (newState)
		{
		case EnemyState::Attack:
			m_IsAttacking = true;
			m_HasHitThisAttack = false;
			m_AttackAnim.Reset();
			m_CurrentAnim = &m_AttackAnim;
			break;

		case EnemyState::Idle:
			m_IsAttacking = false;
			m_CurrentAnim = &m_IdleAnim;
			break;

		case EnemyState::Hurt:
			m_IsAttacking = false;
			break;

		default:
			break;
		}
	}
	
	bool Enemy::CanAttack(const EngineMath::Vector2& playerPos,
						  const EngineCore::AABB& playerCollider) const
	{
		if (m_State == EnemyState::Dead || m_State == EnemyState::Hurt || m_State == EnemyState::Attack || m_AttackCooldown > 0.0f)
			return false;

		bool isFacingToPlayer =
			(m_FacingRight && playerPos.x > m_Position.x) ||
			(!m_FacingRight && playerPos.x < m_Position.x);

		if (!isFacingToPlayer)
			return false;

		//X
		float distX = std::abs(playerPos.x - m_Position.x);
		if (distX > m_AttackRangeX)
			return false;

		//Y
		if (std::abs(m_Collider.Bottom() - playerCollider.Bottom()) > m_AttackRangeY)
			return false;

		return true;
	}

	#pragma region Inherited Methods

	void Enemy::Render(EngineCore::IRenderer* renderer,
		const Camera2D& camera)
	{
		float spriteOffsetX = (m_ColliderWidth - m_SpriteW) * 0.5f;
		float spriteOffsetY = m_ColliderHeight - m_SpriteH;

		float pX = m_Position.x + spriteOffsetX - camera.GetX();
		float pY = m_Position.y + spriteOffsetY - camera.GetY();

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
		switch (m_State)
		{
		case EnemyState::Dead:
			currentTexture = m_DeathTexture;
			break;
		case EnemyState::Hurt:
			currentTexture = m_HurtTexture;
			break;
		case EnemyState::Patrol:
			currentTexture = m_WalkTexture;
			break;
		case EnemyState::Attack:
			currentTexture = m_AttackTexture;
			break;
		default:
			currentTexture = m_IdleTexture;
			break;
		}

		renderer->DrawTexture(
			currentTexture,
			src,
			{ pX, pY, m_SpriteW, m_SpriteH },
			flip);

		//Collider Debug
		EngineCore::Color c = (m_State == EnemyState::Hurt) ? EngineCore::Color{ 0, 255, 0, 255 } : EngineCore::Color{ 255, 255, 255, 255 };
		renderer->DrawRectOutline(
			{
				m_Collider.Left() - camera.GetX(),
				m_Collider.Top() - camera.GetY(),
				m_Collider.Width(),
				m_Collider.Height()
			},
			c
		);

		EngineCore::Color c1 = IsDamageFrame() ? EngineCore::Color{ 255, 0, 0, 255 } : EngineCore::Color{ 255, 255, 255, 255 };
		auto box = GetAttackBox();
		renderer->DrawRectOutline({
				box.x - camera.GetX(),
				box.y - camera.GetY(),
				box.w,
				box.h
			},
			c1
		);
	}

	void Enemy::UpdateAttack(float dt)
	{
		if (m_IsAttacking)
		{
			m_CurrentAnim->Update(dt);

			if (m_CurrentAnim->IsFinished())
			{
				m_AttackCooldown = m_AttackInterval;
				m_IsAttacking = false;
				m_HasHitThisAttack = false;
				ChangeState(EnemyState::Idle);
			}
			return;
		}
	}

	void Enemy::UpdateHurt(float dt)
	{
		if (m_KnockbackTimer > 0.0f)
		{
			m_KnockbackTimer -= dt;
			MoveAndCollide({ m_KnockbackVel.x * dt, m_KnockbackVel.y * dt });
		}

		m_HurtTimer -= dt;
		m_CurrentAnim->Update(dt);

		if (m_HurtTimer <= 0.0f && m_CurrentAnim->IsFinished())
		{
			m_State = EnemyState::Idle;
			m_CurrentAnim = &m_IdleAnim;
			m_KnockbackVel = { 0, 0 };
		}
	}

	void Enemy::UpdateDeath(float dt)
	{
		m_CurrentAnim->Update(dt);
		if (m_CurrentAnim->IsFinished())
			m_IsDead = true;
	}

	void Enemy::TakeDamage(float amount, bool objectDir)
	{
		if (m_State == EnemyState::Dead)
			return;

		m_HP -= amount;
		m_DamageFlashTimer = m_DamageFlashDuration;

		if (m_HP <= 0)
		{
			OnDeath();
			return;
		}

		//Hurt State
		m_State = EnemyState::Hurt;
		m_HurtTimer = m_HurtDuration;
		m_CurrentAnim = &m_HurtAnim;
		m_CurrentAnim->Reset();

		//Knockback
		float dir = objectDir ? 1.0f : -1.0f;
		m_KnockbackVel.x = dir * 100.0f;
		m_KnockbackVel.y = -50.0f;
		m_KnockbackTimer = m_KnockbackDuration;
	}

	void Enemy::OnDeath()
	{
		m_State = EnemyState::Dead;

		m_DeathAnim.Reset();
		m_CurrentAnim = &m_DeathAnim;

		m_ColliderEnabled = false;
	}

	void Enemy::UpdatePhysics(float dt)
	{
		if (m_IsDead)
			return;

		if (!m_IsGrounded)
		{
			m_Velocity.y += m_Gravity * dt;

			if (m_Velocity.y > m_MaxFallSpeed)
				m_Velocity.y = m_MaxFallSpeed;
		}

		MoveAndCollide(m_Velocity * dt);

		//Grounded Behaviour
		if (m_IsGrounded)
			m_Velocity.y = 0.0f;
	}

	#pragma endregion
}