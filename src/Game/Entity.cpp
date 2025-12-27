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
		if (!m_World)
			return;		

		EngineMath::Vector2 oldPos = m_Position;

		EngineCore::Rect nextX = m_Collider;
		nextX.x += velocity.x;

		if (!IsCollidingWithWorld(nextX))
		{
			m_Position.x += velocity.x;
		}
			
		EngineCore::Rect nextY = m_Collider;
		nextY.y += velocity.y;

		if (!IsCollidingWithWorld(nextY))
		{
			m_Position.y += velocity.y;
		}
	}

	bool Entity::IsCollidingWithWorld(const EngineCore::Rect& rect) const
	{
		if (!m_World) return false;

		return
			m_World->IsSolidAt(rect.x, rect.y) ||
			m_World->IsSolidAt(rect.x + rect.w, rect.y) ||
			m_World->IsSolidAt(rect.x, rect.y + rect.h) ||
			m_World->IsSolidAt(rect.x + rect.w, rect.y + rect.h);
	}

	void Entity::UpdateCollider()
	{
		m_Collider.x = m_Position.x + 50;
		m_Collider.y = m_Position.y + 50;
		m_Collider.w = 32;
		m_Collider.h = 80;
	}
	
	void Entity::SetTexture(Texture2D* idleT, Texture2D* walkT, Texture2D* hurtT, Texture2D* deadT)
	{
		m_IdleTexture = idleT;
		m_WalkTexture = walkT;
		m_HurtTexture = hurtT;
		m_DeathTexture = deadT;
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
}