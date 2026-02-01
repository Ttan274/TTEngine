#include "Game/Enemy.h"
#include "Platform/AnimationLibrary.h"

namespace EngineGame
{
	Enemy::Enemy()
	{
		m_Position = { 0.0f, 0.0f };
		m_Speed = 100.0f;
		
		m_CurrentAnim = nullptr;
		UpdateCollider();
	}

	void Enemy::ApplyDefinition(const EntityDefs& def)
	{
		//Base settings
		m_Speed = def.speed;
		m_AttackDamage = def.attackDamage;
		m_AttackInterval = def.attackInterval;
		m_MaxHP = def.maxHp;
		m_HP = m_MaxHP;

		//Animations
		m_IdleAnim = EnginePlatform::AnimationLibrary::CreateAnimation(def.idleAnim);
		m_WalkAnim = EnginePlatform::AnimationLibrary::CreateAnimation(def.walkAnim);
		m_HurtAnim = EnginePlatform::AnimationLibrary::CreateAnimation(def.hurtAnim);
		m_DeathAnim = EnginePlatform::AnimationLibrary::CreateAnimation(def.deathAnim);
		m_AttackAnim = EnginePlatform::AnimationLibrary::CreateAnimation(def.attackAnims[0]);
	
		m_CurrentAnim = &m_IdleAnim;
	}

	void Enemy::Update(float dt, const EngineMath::Vector2& playerPos, const EngineCore::AABB& playerColldier)
	{
		if (m_AttackCooldown > 0.0f)
			m_AttackCooldown -= dt;

		if (m_DamageFlashTimer > 0.0f)
			m_DamageFlashTimer -= dt;

		if (IsPlayerInDetectionRange(playerPos))
			ChangeState(EnemyState::Chase);

		switch (m_State)
		{
		case EnemyState::Idle:
			UpdateIdle(dt);
			break;
		case EnemyState::Patrol:
			UpdatePatrol(dt);
			break;
		case EnemyState::Chase:
			UpdateChase(dt, playerPos, playerColldier);
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
		
		if (!CanMoveForward())
		{
			m_FacingRight = !m_FacingRight;
			m_Direction *= -1.0f;
			ChangeState(EnemyState::Idle);
			return;
		}

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

	void Enemy::UpdateChase(float dt, const EngineMath::Vector2& playerPos, const EngineCore::AABB& playerCollider)
	{
		//Facing
		m_Direction = (playerPos.x > m_Position.x) ? 1.0f : -1.0f;
		m_FacingRight = m_Direction > 0;

		if (!CanMoveForward())
		{
			ChangeState(EnemyState::Idle);
			return;
		}

		//Animation
		m_CurrentAnim = &m_WalkAnim;
		m_CurrentAnim->Update(dt);

		EngineMath::Vector2 move = { m_Direction * m_Speed * dt, 0.0f };
		MoveAndCollide(move);

		//Checking for attacking
		if (CanAttack(playerPos, playerCollider))
		{
			ChangeState(EnemyState::Attack);
			return;
		}

		//If Player out of range
		if (!IsPlayerInDetectionRange(playerPos))
			ChangeState(EnemyState::Idle);
	}

	bool Enemy::IsPlayerInDetectionRange(const EngineMath::Vector2& playerPos) const
	{
		if (m_State == EnemyState::Attack || m_State == EnemyState::Hurt || m_State == EnemyState::Dead)
			return false;

		return std::abs(playerPos.x - m_Position.x) <= m_DetectRange.x && std::abs(playerPos.y - m_Position.y) <= m_DetectRange.y;
	}

	void Enemy::ChangeState(EnemyState newState)
	{
		m_State = newState;
		m_StateTimer = 0.0f;

		switch (newState)
		{
		case EnemyState::Chase:
			m_IsAttacking = false;
			m_CurrentAnim = &m_WalkAnim;
			break;

		case EnemyState::Attack:
			m_IsAttacking = true;
			m_HasHitThisAttack = false;
			
			m_CanDealDamage = false;
			m_AttackWindUpTimer = m_AttackWindUpTime;

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
		if (std::abs(playerPos.x - m_Position.x) > m_AttackRange.x)
			return false;

		//Y
		if (std::abs(m_Collider.Bottom() - playerCollider.Bottom()) > m_AttackRange.y)
			return false;

		return true;
	}

	bool Enemy::CanMoveForward() const
	{
		if (!m_World || !m_IsGrounded)
			return true;

		EngineMath::Vector2 checkLedgePoints;
		checkLedgePoints.x = m_FacingRight ? m_Collider.Right() + 2.0f : m_Collider.Left() - 2.0f;
		checkLedgePoints.y = m_Collider.Bottom() + 4.0f;

		EngineCore::AABB fBox;
		fBox.SetFromPositionSize(
			checkLedgePoints.x,
			checkLedgePoints.y,
			2.0f,
			2.0f
		);

		if (!m_World->IsSolidY(fBox, -1.0f))
			return false;

		return true;
	}

	std::string Enemy::GetStateName() const
	{
		switch (m_State)
		{
		case EnemyState::Idle: return "IDLE";
		case EnemyState::Patrol: return "WALK";
		case EnemyState::Chase: return "CHASE";
		case EnemyState::Attack: return "ATTACK";
		case EnemyState::Hurt: return "HURT";
		case EnemyState::Dead: return "DEAD";
		default: return "UNKNOWN";
		}
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

		Texture2D* currentTexture = m_CurrentAnim->GetTexture();

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
		if (m_AttackWindUpTimer > 0.0f)
		{
			m_AttackWindUpTimer -= dt;
			m_CurrentAnim->Update(dt);
			return;
		}

		m_CanDealDamage = true;
		m_CurrentAnim->Update(dt);


		if (m_CurrentAnim->IsFinished())
		{
			m_AttackCooldown = m_AttackInterval;

			m_IsAttacking = false;
			m_HasHitThisAttack = false;
			m_CanDealDamage = false;
			
			ChangeState(EnemyState::Chase);
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

		if (OnKilled)
			OnKilled();
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