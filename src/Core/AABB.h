#pragma once
#include "Math/Vector2.h"

namespace EngineCore
{
	struct Rect
	{
		float x, y, w, h;
	};

	struct AABB
	{
		float minX;
		float minY;
		float maxX;
		float maxY;

		float Left() const { return minX; }
		float Right() const { return maxX; }
		float Top() const { return minY; }
		float Bottom() const { return maxY; }

		float Width() const { return maxX - minX; }
		float Height() const { return maxY - minY; }

		void Translate(const EngineMath::Vector2& delta)
		{
			minX += delta.x;
			maxX += delta.x;
			minY += delta.y;
			maxY += delta.y;
		}

		void SetFromPositionSize(float x, float y, float w, float h)
		{
			minX = x;
			minY = y;
			maxX = x + w;
			maxY = y + h;
		}
	};
}