#pragma once
#include <cmath>

namespace EngineMath
{
	struct Vector2
	{
		float x = 0.0f;
		float y = 0.0f;

		Vector2() = default;
		Vector2(float _x, float _y) : x(_x), y(_y) {}

		// Operators
		Vector2 operator+(const Vector2& other) const
		{
			return { x + other.x, y + other.y };
		}

		Vector2 operator-(const Vector2& other) const
		{
			return { x - other.x, y - other.y };
		}

		Vector2& operator+=(const Vector2& other)
		{
			x += other.x;
			y += other.y;
			return *this;
		}

		Vector2& operator-=(const Vector2& other)
		{
			x -= other.x;
			y -= other.y;
			return *this;
		}

		Vector2 operator*(float scalar) const
		{
			return { x * scalar, y * scalar };
		}

		Vector2& operator*=(float scalar)
		{
			x *= scalar;
			y *= scalar;
			return *this;
		}

		//Math

		float Length() const
		{
			float length = std::sqrt(LengthSquared());
			return length;
		}

		float LengthSquared() const
		{
			return x * x + y * y;
		}

		void Normalize()
		{
			float len = Length();
			if (len > 0.0001f)
			{
				x /= len;
				y /= len;
			}
		}

		Vector2 Normalized() const
		{
			float len = Length();
			if (len > 0.0001f)
				return { x / len, y / len };

			return { 0.0f, 0.0f };
		}
	};
}