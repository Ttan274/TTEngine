#pragma once

namespace EngineGame
{
	class Camera2D
	{
	public:
		Camera2D(float width, float height);

		void SetPosition(float x, float y);
		void SetSmoothness(float value);
		float GetSmoothness() { return m_Smoothness; }

		void Follow(float targetX, float targetY);
		void FollowSmooth(float targetX, float targetY, float dt);
		void SetWorldBounds(float width, float height);

		float GetX() const;
		float GetY() const;
	private:
		void ApplyWorldBounds();

		float m_Width;
		float m_Height;

		float m_X = 0.0f;
		float m_Y = 0.0f;

		float m_WorldWidth = 0.0f;
		float m_WorldHeight = 0.0f;

		float m_Smoothness = 5.0f;
	};
}