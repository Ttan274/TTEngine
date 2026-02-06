#pragma once
#include "Core/AABB.h"
#include "Core/IRenderer.h"

namespace EngineGame
{
	class Player;
	class Camera2D;

	class Trap
	{
	public:
		virtual ~Trap() = default;

		virtual void Update(float dt, Player& player) = 0;
		virtual void Render(EngineCore::IRenderer* renderer, const Camera2D& camera) = 0;
		virtual const EngineCore::AABB& GetCollider() const = 0;

	protected:
		float m_Damage = 0.0f;
	};
}