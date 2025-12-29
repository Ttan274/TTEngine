#pragma once
#include "Core/AABB.h" 

namespace EngineMath
{
	inline bool RectIntersectsAABB(const EngineCore::Rect& a, const EngineCore::AABB& b)
	{
		return !(
			a.x + a.w < b.Left() ||
			a.x > b.Right() ||
			a.y + a.h < b.Top() ||
			a.y > b.Bottom()
			);
	}

	inline bool AABBIntersectsAABB(const EngineCore::AABB& a, const EngineCore::AABB& b)
	{
		return !(
			a.Right() < b.Left() ||
			a.Left() > b.Right() ||
			a.Bottom() < b.Top() ||
			a.Top() > b.Bottom()
			);
	}
}