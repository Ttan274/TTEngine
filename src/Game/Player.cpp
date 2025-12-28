#include "Game/Player.h"
#include "Platform/AssetManager.h"
#include "Core/Log.h"

namespace EngineGame
{
	Player::Player()
	{
		m_Position = { 0.0f, 0.0f};
		m_Speed = 150.0f;
		m_AttackDamage = 20.0f;	

		//Idle Anim
		CreateAnim(&m_IdleAnim, 0.25f, 6, true);
		
		//Walk Anim
		CreateAnim(&m_WalkAnim, 0.12f, 8, true);

		//Attack Animations
		EngineCore::Animation attack1;
		CreateAnim(&attack1, 0.12f, 5, false);
		m_AttackAnims.push_back(attack1);

		EngineCore::Animation attack2;
		CreateAnim(&attack2, 0.18f, 3, false);
		m_AttackAnims.push_back(attack2);

		EngineCore::Animation attack3;
		CreateAnim(&attack3, 0.14f, 4, false);
		m_AttackAnims.push_back(attack3);

		//Hurt Animation
		CreateAnim(&m_HurtAnim, 0.12f, 2, false);

		//Death Animation
		CreateAnim(&m_DeathAnim, 0.3f, 4, false);

		m_CurrentAnim = &m_IdleAnim;
		UpdateCollider();
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

		UpdateCollider();
	}

	void Player::SetAttackTexture(Texture2D* aT1, Texture2D* aT2, Texture2D* aT3)
	{
		m_AttackTextures.push_back(aT1);
		m_AttackTextures.push_back(aT2);
		m_AttackTextures.push_back(aT3);
	}

	void Player::UpdateMovement(float dt)
	{
		if (m_IsAttacking)
		{
			m_CurrentAnim->Update(dt);
			return;
		}

		EngineMath::Vector2 movement{ 0, 0 };

		if (EngineCore::Input::IsKeyDown(EngineCore::KeyCode::W)) movement.y -= 1.0f;
		if (EngineCore::Input::IsKeyDown(EngineCore::KeyCode::S)) movement.y += 1.0f;
		if (EngineCore::Input::IsKeyDown(EngineCore::KeyCode::A)) movement.x -= 1.0f;
		if (EngineCore::Input::IsKeyDown(EngineCore::KeyCode::D)) movement.x += 1.0f;
	
		m_IsMoving = (movement.x != 0.0f || movement.y != 0.0f);
		m_CurrentAnim = m_IsMoving ? &m_WalkAnim : &m_IdleAnim;
		m_CurrentAnim->Update(dt);

		if (movement.x > 0.0f)
			m_FacingRight = true;
		else if (movement.x < 0.0f)
			m_FacingRight = false;

		EngineMath::Vector2 velocity =
		{
			movement.x * m_Speed * dt,
			movement.y * m_Speed * dt
		};

		MoveAndCollide(velocity);
	}

	void Player::StartAttack(AttackStage stage)
	{
		m_AttackStage = stage;
		m_IsAttacking = true;
		m_ComboQueued = false;
		m_HasHitThisAttack = false;

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

	#pragma region Inherited Methods

	void Player::Render(EngineCore::IRenderer* renderer,
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
		if (m_CurrentAnim == &m_IdleAnim)
			currentTexture = m_IdleTexture;
		else if (m_CurrentAnim == &m_WalkAnim)
			currentTexture = m_WalkTexture;
		else if (m_CurrentAnim == &m_AttackAnims[0])
			currentTexture = m_AttackTextures[0];
		else if (m_CurrentAnim == &m_AttackAnims[1])
			currentTexture = m_AttackTextures[1];
		else if (m_CurrentAnim == &m_AttackAnims[2])
			currentTexture = m_AttackTextures[2];
		else if (m_CurrentAnim == &m_HurtAnim)
			currentTexture = m_HurtTexture;
		else if (m_CurrentAnim == &m_DeathAnim)
			currentTexture = m_DeathTexture;

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


		if (IsAttacking())
		{
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

	void Player::TakeDamage(float amount, bool objectDir)
	{
		if (m_State == PlayerState::Dead)
			return;

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
		float dir = objectDir ? 1.0f : -1.0f;
		m_KnockbackVel.x = dir * 100.0f;
		m_KnockbackVel.y = dir * -50.0f;
	}

	void Player::OnDeath()
	{
		m_State = PlayerState::Dead;

		m_DeathTimer = m_DeathDuration;

		m_CurrentAnim = &m_DeathAnim;
		m_CurrentAnim->Reset();

		m_KnockbackVel = { 0, 0 };
	}

	#pragma endregion

}