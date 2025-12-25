#pragma once
#include "Game/Texture.h"
#include "Core/IRenderer.h"
#include "Core/Math/Vector2.h"
#include "Core/Input.h"
#include "Game/Camera.h"
#include "Game/TileMap.h"
#include "Core/Animation.h"

namespace EngineGame
{
	class Player
	{
	public:
		Player();

		void Update(float dt);
		void Render(EngineCore::IRenderer* renderer,
					const Camera2D& camera);

		void SetTexture(Texture2D* idleT, Texture2D* walkT) {
			m_IdleTexture = idleT;
			m_WalkTexture = walkT;
		}

		void SetWorld(TileMap* world);
		bool IsCollidingWithWorld(const EngineCore::Rect& rect) const;

		const EngineCore::Rect& GetCollider() const { return m_Collider; }
		EngineMath::Vector2 GetPosition() const { return m_Position; }
		void SetPosition(float px, float py) { m_Position.x = px; m_Position.y = py; }
	private:

		void UpdateMovement(float dt);
		void UpdateCollider();
		void CheckCollision(EngineMath::Vector2 v);
	private:
		//Movement
		EngineMath::Vector2 m_Position;
		float m_Speed;
		bool m_IsMoving = false;

		//Animation
		EngineCore::Animation m_IdleAnim;
		EngineCore::Animation m_WalkAnim;
		EngineCore::Animation* m_CurrentAnim = nullptr;

		//Sprite
		EngineGame::Texture2D* m_IdleTexture = nullptr;
		EngineGame::Texture2D* m_WalkTexture = nullptr;
		float m_SpriteW = 128.0f;
		float m_SpriteH = 128.0f;
		bool m_FacingRight = true;

		//Collider
		EngineCore::Rect m_Collider;

		//World Reference
		TileMap* m_World;
	};
}