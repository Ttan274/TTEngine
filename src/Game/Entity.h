#pragma once
#include "Core/Math/Vector2.h"
#include "Core/IRenderer.h"
#include "Game/Camera.h"
#include "Game/TileMap.h"

namespace EngineGame
{
	class Entity
	{
	public:
		Entity();
		virtual ~Entity() = default;

		virtual void Update(float dt) = 0;
		virtual void Render(EngineCore::IRenderer* renderer,
			const Camera2D& camera) = 0;
		virtual void UpdateCollider();

		void SetWorld(TileMap* world);
	protected:
		void MoveAndCollide(const EngineMath::Vector2& velocity);
		bool IsCollidingWithWorld(const EngineCore::Rect& rect) const;
	protected:
		//Movement
		EngineMath::Vector2 m_Position{};
		float m_Speed = 0.0f;
		bool m_FacingRight = true;

		//Collider
		EngineCore::Rect m_Collider{};

		//World Reference
		TileMap* m_World = nullptr;
	};
}