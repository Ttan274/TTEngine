#include "Game/Player.h"
#include "Platform/AssetManager.h"
#include "Core/Log.h"

namespace EngineGame
{
	Player::Player()
	{
		m_Position = { 100.0f, 100.0f };
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

	void Player::Update(float dt)
	{
		UpdateMovement(dt);
		UpdateCollider();
	}

	void Player::UpdateMovement(float dt)
	{
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

		movement.x *= m_Speed * dt;
		movement.y *= m_Speed * dt;

		CheckCollision(movement);
	}

	void Player::UpdateCollider()
	{
		m_Collider.x = m_Position.x + 50;
		m_Collider.y = m_Position.y + 50;
		m_Collider.w = 32;
		m_Collider.h = 80;
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

	void Player::CheckCollision(EngineMath::Vector2 v)
	{
		//X axis collision
		float nextX = m_Position.x + v.x;
		EngineCore::Rect nextColliderX = m_Collider;
		nextColliderX.x += v.x;

		if (!IsCollidingWithWorld(nextColliderX))
		{
			m_Position.x = nextX;
		}

		//Y axis collision
		float nextY = m_Position.y + v.y;
		EngineCore::Rect nextColliderY = m_Collider;
		nextColliderY.y += v.y;

		if (!IsCollidingWithWorld(nextColliderY))
		{
			m_Position.y = nextY;
		}
	}

	bool Player::IsCollidingWithWorld(const EngineCore::Rect& rect) const
	{
		return
			m_World->IsSolidAt(rect.x, rect.y) ||
			m_World->IsSolidAt(rect.x + rect.w, rect.y) ||
			m_World->IsSolidAt(rect.x, rect.y + rect.h) ||
			m_World->IsSolidAt(rect.x + rect.w, rect.y + rect.h);
	}

	void Player::SetWorld(TileMap* world)
	{
		m_World = world;
	}
}