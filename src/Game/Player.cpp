#include "Game/Player.h"
#include "Platform/AssetManager.h"
#include "Core/Log.h"

namespace EngineGame
{
	Player::Player()
	{
		m_Position = { 0.0f, 0.0f};
		m_Speed = 150.0f;
		m_AttackDamage = 55.0f;	//silinecek

		//Idle Anim
		CreateAnim(&m_IdleAnim, 0.25f, 6, true);
		
		//Walk Anim
		CreateAnim(&m_WalkAnim, 0.12f, 8, true);

		//Attack Animations
		EngineCore::Animation attack1;
		CreateAnim(&attack1, 0.18f, 5, false);
		m_AttackAnims.push_back(attack1);

		EngineCore::Animation attack2;
		CreateAnim(&attack2, 0.14f, 3, false);
		m_AttackAnims.push_back(attack2);

		EngineCore::Animation attack3;
		CreateAnim(&attack3, 0.16f, 4, false);
		m_AttackAnims.push_back(attack3);

		m_CurrentAnim = &m_IdleAnim;
		UpdateCollider();
	}

	void Player::SetTexture(Texture2D* idleT, Texture2D* walkT, Texture2D* aT1, Texture2D* aT2, Texture2D* aT3)
	{
		m_IdleTexture = idleT;
		m_WalkTexture = walkT;
		m_AttackTextures.push_back(aT1);
		m_AttackTextures.push_back(aT2);
		m_AttackTextures.push_back(aT3);
	}

	void Player::CreateAnim(EngineCore::Animation* anim, float frameTime, int frameSize, bool loop)
	{
		anim->SetLoop(loop);
		anim->SetFrameTime(frameTime);
		for (int i = 0; i < frameSize; i++)
		{
			anim->AddFrame({ i * m_SpriteW, 0.0f, m_SpriteW, m_SpriteH });
		}
	}

	void Player::Update(float dt)
	{
		UpdateMovement(dt);
		UpdateAttack(dt);
		UpdateCollider();
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
		if (m_IsAttacking)
			currentTexture = m_AttackTextures[m_CurrentAttackAnimIndex];
		else
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

	void Player::UpdateAttack(float dt)
	{
		if (m_AttackCooldown > 0.0f)
			m_AttackCooldown -= dt;

		if (m_IsAttacking)
		{
			m_CurrentAnim->Update(dt);

			if (m_CurrentAnim->IsFinished())
			{
				m_IsAttacking = false;
				m_CurrentAnim = m_IsMoving ? &m_WalkAnim : &m_IdleAnim;
			}
			return;
		}

		if (EngineCore::Input::IsKeyPressed(EngineCore::KeyCode::E) && m_AttackCooldown <= 0.0f)
		{
			m_IsAttacking = true;
			m_HasHitThisAttack = false;
			m_AttackCooldown = m_AttackInterval;

			m_CurrentAttackAnimIndex = rand() % m_AttackAnims.size();

			m_AttackAnims[m_CurrentAttackAnimIndex].Reset();
			m_CurrentAnim = &m_AttackAnims[m_CurrentAttackAnimIndex];
		}
	}

	EngineCore::Rect Player::GetAttackBox() const
	{
		EngineCore::Rect attackBox;

		attackBox.w = 40;
		attackBox.h = m_Collider.h - 20;
		attackBox.y = m_Collider.y + 10;
		attackBox.x = m_FacingRight ? m_Collider.x + m_Collider.w : m_Collider.x - attackBox.w;

		return attackBox;
	}

	bool Player::IsDamageFrame() const
	{
		if (!m_IsAttacking || m_CurrentAnim == nullptr)
			return false;

		int frame = m_CurrentAnim->GetCurrentFrameIndex();
		int count = m_CurrentAnim->GetFrameCount();

		int center = count / 2;
		int halfwidth = (count <= 3) ? 0 : 1;

		int start = std::max(0, center - halfwidth);
		int end = std::max(count - 1, center + halfwidth);

		return frame >= start && frame <= end;
	}
}