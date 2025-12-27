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

		//Combat
		virtual void TakeDamage(float amount);
		bool IsAlive() const { return m_HP > 0; }
		float GetHp() const { return m_HP; }
	protected:
		void MoveAndCollide(const EngineMath::Vector2& velocity);
		bool IsCollidingWithWorld(const EngineCore::Rect& rect) const;
		virtual void OnDeath();
	protected:
		//Movement
		EngineMath::Vector2 m_Position{};
		float m_Speed = 0.0f;
		bool m_FacingRight = true;

		//Hp
		float m_MaxHP = 100;
		float m_HP = 100;

		//Collider
		EngineCore::Rect m_Collider{};

		//World Reference
		TileMap* m_World = nullptr;
	};
}