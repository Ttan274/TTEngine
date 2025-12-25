#pragma once

namespace EngineMath
{
	struct Vector2
	{
		float x = 0.0f;
		float y = 0.0f;

		Vector2() = default;
		Vector2(float _x, float _y) : x(_x), y(_y) {}

		Vector2 operator+(const Vector2& other) const
		{
			return { x + other.x, y + other.y };
		}

		Vector2 operator+=(const Vector2& other)
		{
			x += other.x;
			y += other.y;
			return *this;
		}

		Vector2 operator*(float scalar) const
		{
			return { x * scalar, y * scalar };
		}
	};
}