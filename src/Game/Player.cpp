#include "Game/Player.h"
#include "Game/Animator.h"

namespace EngineGame
{
	Player::Player()
	{
		m_Position = { 0.0f, 0.0f};
		m_Speed = 150.0f;
		m_AttackDamage = 20.0f;	
		
		m_CurrentAnim = nullptr;
		UpdateCollider();
	}

	void Player::ApplyDefinition(const EntityDefs& def)
	{
		//Base settings
		m_Speed = def.speed;
		m_AttackDamage = def.attackDamage;
		m_AttackInterval = def.attackInterval;
		m_MaxHP = def.maxHp;
		m_HP = m_MaxHP;

		//Animations
		m_IdleAnim = Animator::Create(def.idleAnim);
		m_WalkAnim = Animator::Create(def.walkAnim);
		m_HurtAnim = Animator::Create(def.hurtAnim);
		m_DeathAnim = Animator::Create(def.deathAnim);
		
		m_AttackAnims.clear();
		for (const auto& animId : def.attackAnims)
			m_AttackAnims.push_back(Animator::Create(animId));

		m_CurrentAnim = &m_IdleAnim;
	}

	void Player::Update(float dt)
	{
		if (m_DamageFlashTimer > 0.0f)
			m_DamageFlashTimer -= dt;

		switch (m_State)
		{
		case PlayerState::Normal:
			UpdateMovement(dt);
			UpdateAttack(dt);
			break;
		case PlayerState::Hurt:
			UpdateHurt(dt);
			break;
		case PlayerState::Dead:
			UpdateDeath(dt);
			break;
		}

		UpdatePhysics(dt);
	}

	void Player::UpdateMovement(float dt)
	{
		if (m_IsAttacking)
		{
			m_CurrentAnim->Update(dt);
			return;
		}

		//Horizontal Movement
		float moveX = 0.0f;

		if (EngineCore::Input::IsKeyDown(EngineCore::KeyCode::A)) moveX -= 1.0f;
		if (EngineCore::Input::IsKeyDown(EngineCore::KeyCode::D)) moveX += 1.0f;
	
		float targetSpeed = moveX * m_Speed;
		float accel = m_IsGrounded ? m_Acceleration : m_Acceleration * m_AirControl;

		if (moveX != 0.0f)
			m_Velocity.x = Approach(m_Velocity.x, targetSpeed, accel * dt);
		else
			m_Velocity.x = Approach(m_Velocity.x, 0.0f, m_Friction * dt);

		//Facing
		if (moveX > 0.0f)
			m_FacingRight = true;
		else if (moveX < 0.0f)
			m_FacingRight = false;

		//Animation
		m_IsMoving = (moveX != 0.0f);
		m_CurrentAnim = m_IsMoving ? &m_WalkAnim : &m_IdleAnim;
		m_CurrentAnim->Update(dt);

		//Jump
		if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::W))
		{
			m_JumpBufferTimer = m_JumpBufferTime;
		}
		else
		{
			m_JumpBufferTimer -= dt;
		}
	}

	void Player::StartAttack(AttackStage stage)
	{
		m_AttackStage = stage;
		m_IsAttacking = true;
		m_ComboQueued = false;
		m_HasHitThisAttack = false;
		m_Velocity.x *= 0.2f;

		int index = 0;
		if (stage == AttackStage::Attack2) index = 1;
		if (stage == AttackStage::Attack3) index = 2;

		m_CurrentAnim = &m_AttackAnims[index];
		m_CurrentAnim->Reset();
	}

	void Player::ResetCombo()
	{
		m_AttackStage = AttackStage::None;
		m_IsAttacking = false;
		m_ComboQueued = false;
		m_CurrentAnim = m_IsMoving ? &m_WalkAnim : &m_IdleAnim;
	}

	void Player::Respawn()
	{
		m_IsDead = false;
		m_Position = m_SpawnPoint;
		m_HP = m_MaxHP;

		m_State = PlayerState::Normal;

		m_CurrentAnim = &m_IdleAnim;
		m_CurrentAnim->Reset();
	}

	void Player::Jump()
	{
		if (m_CoyoteTimer > 0.0f && m_JumpBufferTimer > 0.0f)
		{
			m_Velocity.y = -m_JumpForce;
			m_IsGrounded = false;

			m_CoyoteTimer = 0.0f;
			m_JumpBufferTimer = 0.0f;
		}
	}

	#pragma region Inherited Methods

	void Player::Render(EngineCore::IRenderer* renderer,
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
		renderer->DrawRectOutline(
			{
				m_Collider.Left() - camera.GetX(),
				m_Collider.Top() - camera.GetY(),
				m_Collider.Width(),
				m_Collider.Height()
			},
			{ 255, 0, 0, 255 });


		EngineCore::Color c = IsDamageFrame() ? EngineCore::Color{ 255, 0, 0, 255 } : EngineCore::Color{ 255, 255, 255, 255 };
		auto box = GetAttackBox();
		renderer->DrawRectOutline({
				box.x - camera.GetX(),
				box.y - camera.GetY(),
				box.w,
				box.h
			},
			c
		);
			
	}

	void Player::UpdateHurt(float dt)
	{
		m_HurtTimer -= dt;
		MoveAndCollide({ m_KnockbackVel.x * dt, m_KnockbackVel.y * dt });
		m_CurrentAnim->Update(dt);

		if (m_HurtTimer <= 0.0f && m_CurrentAnim->IsFinished())
		{
			m_State = PlayerState::Normal;
			m_KnockbackVel = { 0, 0 };
			m_CurrentAnim = m_IsMoving ? &m_WalkAnim : &m_IdleAnim;
		}
	}

	void Player::UpdateAttack(float dt)
	{
		if (m_AttackCooldown > 0.0f)
			m_AttackCooldown -= dt;

		if (m_IsAttacking)
		{
			m_CurrentAnim->Update(dt);

			if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::E))
				m_ComboQueued = true;

			if (m_CurrentAnim->IsFinished())
			{
				if (m_ComboQueued)
				{
					if (m_AttackStage == AttackStage::Attack1)
						StartAttack(AttackStage::Attack2);
					else if (m_AttackStage == AttackStage::Attack2)
						StartAttack(AttackStage::Attack3);
					else
						ResetCombo();
				}
				else
				{
					ResetCombo();
				}
			}
			return;
		}

		if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::E) && m_AttackCooldown <= 0.0f)
		{
			m_AttackCooldown = m_AttackInterval;
			StartAttack(AttackStage::Attack1);
		}
	}

	void Player::UpdateDeath(float dt)
	{
		m_DeathTimer -= dt;
		m_CurrentAnim->Update(dt);

		if (m_CurrentAnim->IsFinished() && m_DeathTimer <= 0.0f)
			m_IsDead = true;
	}

	void Player::TakeDamage(float amount, float objectDir)
	{
		if (m_State == PlayerState::Dead)
			return;

		m_IsAttacking = false;
		m_HasHitThisAttack = false;

		m_HP -= amount;
		m_DamageFlashTimer = m_DamageFlashDuration;

		if (m_HP <= 0)
		{
			OnDeath();
			return;
		}

		//Hurt State
		m_State = PlayerState::Hurt;
		m_HurtTimer = m_HurtDuration;
		m_CurrentAnim = &m_HurtAnim;
		m_CurrentAnim->Reset();

		//Knockback
		if (objectDir == 0.0f)
			return;

		float dir = (objectDir == 1.0f) ? -1.0f : 1.0f;
		m_KnockbackVel.x = dir * 150.0f;
		m_KnockbackVel.y = -200.0f;
	}

	void Player::OnDeath()
	{
		m_State = PlayerState::Dead;

		m_DeathTimer = m_DeathDuration;

		m_CurrentAnim = &m_DeathAnim;
		m_CurrentAnim->Reset();

		m_KnockbackVel = { 0, 0 };
	}

	void Player::UpdatePhysics(float dt)
	{
		if (m_IsDead)
			return;

		//Gravity
		if (!m_IsGrounded)
		{
			m_CoyoteTimer -= dt;
			m_Velocity.y += m_Gravity * dt;

			if (m_Velocity.y > m_MaxFallSpeed)
				m_Velocity.y = m_MaxFallSpeed;
		}

		MoveAndCollide(m_Velocity * dt);

		//Grounded Behaviour
		if (m_IsGrounded)
		{
			m_Velocity.y = 0.0f;
			m_CoyoteTimer = m_CoyoteTime;
		}

		Jump();
	}

	#pragma endregion

	void Player::Reset()
	{
		m_Position = m_SpawnPoint;
		m_Velocity = { 0.0f, 0.0f };
		
		m_HP = m_MaxHP;
		m_IsDead = false;

		m_IsAttacking = false;
		m_HasHitThisAttack = false;

		m_State = PlayerState::Normal;
		m_CurrentAnim = &m_IdleAnim;
		m_CurrentAnim->Reset();
	}
}