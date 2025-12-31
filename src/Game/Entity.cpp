#include "Game/Entity.h"
#include "Core/Log.h"

namespace EngineGame
{
	Entity::Entity()
	{
		m_Position = { 0.0f, 0.0f };
		m_Speed = 0.0f;
	}

	void Entity::SetWorld(TileMap* world)
	{
		m_World = world;
	}

	void Entity::MoveAndCollide(const EngineMath::Vector2& velocity)
	{
		if (!m_World || !m_ColliderEnabled)
			return;		

		//X axis collision
		EngineCore::AABB nextX = m_Collider;
		nextX.Translate({ velocity.x, 0.0f });

		if (!m_World->IsSolidX(nextX))
		{
			m_Position.x += velocity.x;
			m_Collider = nextX;
		}
		
		//Y axis collision
		EngineCore::AABB nextY = m_Collider;
		nextY.Translate({0.0f, velocity.y});

		if (!m_World->IsSolidY(nextY, velocity.y))
		{
			m_Position.y += velocity.y;
			m_Collider = nextY;
		}
		else
		{
			m_Velocity.y = 0.0f;
		}

		m_IsGrounded = m_World->IsGrounded(m_Collider);
		UpdateCollider();
	}

	void Entity::UpdateCollider()
	{
		m_Collider.SetFromPositionSize(
			m_Position.x,
			m_Position.y,
			m_ColliderWidth,
			m_ColliderHeight
		);
	}
	
	void Entity::SetTexture(Texture2D* idleT, Texture2D* walkT, Texture2D* hurtT, Texture2D* deadT)
	{
		m_IdleTexture = idleT;
		m_WalkTexture = walkT;
		m_HurtTexture = hurtT;
		m_DeathTexture = deadT;
	}

	void Entity::ApplyDefinition(const EngineGame::EntityDefs& def)
	{
		m_Speed = def.speed;
		m_AttackDamage = def.attackDamage;
		m_AttackInterval = def.attackInterval;
		m_MaxHP = def.maxHp;
		m_HP = m_MaxHP;
	}

	void Entity::CreateAnim(EngineCore::Animation* anim, float frameTime, int frameSize, bool loop)
	{
		anim->SetLoop(loop);
		anim->SetFrameTime(frameTime);
		for (int i = 0; i < frameSize; i++)
		{
			anim->AddFrame({ i * m_SpriteW, 0.0f, m_SpriteW, m_SpriteH });
		}
	}

	EngineCore::Rect Entity::GetAttackBox() const
	{
		EngineCore::Rect attackBox;

		attackBox.w = 40.0f;
		attackBox.h = m_Collider.Height() - 20.0f;
		attackBox.y = m_Collider.Top() + 10.0f;
		attackBox.x = m_FacingRight ? m_Collider.Right() : m_Collider.Left() - attackBox.w;

		return attackBox;
	}

	bool Entity::IsDamageFrame() const
	{
		if (!m_IsAttacking || m_CurrentAnim == nullptr)
			return false;

		int frame = m_CurrentAnim->GetCurrentFrameIndex();
		int count = m_CurrentAnim->GetFrameCount();

		int center = count / 2;
		int halfwidth = (count <= 3) ? 0 : 1;

		int start = std::max(0, center - halfwidth);
		int end = std::min(count - 1, center + halfwidth);

		return frame >= start && frame <= end;
	}
}