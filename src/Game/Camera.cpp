#include "Game/Camera.h"
#include <algorithm>

namespace EngineGame
{
	Camera2D::Camera2D(float width, float height)
		: m_Width(width), m_Height(height)
	{
	}

	void Camera2D::SetWorldBounds(float width, float height)
	{
		m_WorldWidth = width;
		m_WorldHeight = height;
	}

	void Camera2D::SetPosition(float x, float y)
	{
		m_X = x;
		m_Y = y;
		ApplyWorldBounds();
	}

	void Camera2D::SetSmoothness(float value)
	{
		m_Smoothness = value;
	}	

	void Camera2D::Follow(float targetX, float targetY)
	{
		m_X = targetX - m_Width * 0.5f;
		m_Y = targetY - m_Height * 0.5f;
		ApplyWorldBounds();
	}

	void Camera2D::FollowSmooth(float targetX, float targetY, float dt)
	{
		float desiredX = targetX - m_Width * 0.5f;
		float desiredY = targetY - m_Height * 0.5f;

		m_X += (desiredX - m_X) * m_Smoothness * dt;
		m_Y += (desiredY - m_Y) * m_Smoothness * dt;

		ApplyWorldBounds();
	}

	void Camera2D::ApplyWorldBounds()
	{
		if (m_WorldWidth <= 0 || m_WorldHeight <= 0)
			return;

		float maxX = m_WorldWidth - m_Width;
		float maxY = m_WorldHeight - m_Height;

		m_X = std::clamp(m_X, 0.0f, maxX);
		m_Y = std::clamp(m_Y, 0.0f, maxY);
	}

	float Camera2D::GetX() const { return m_X; }
	float Camera2D::GetY() const { return m_Y; }
}