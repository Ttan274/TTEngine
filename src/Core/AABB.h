#pragma once

namespace EngineCore
{
	struct AABB
	{
		float x;
		float y;
		float w;
		float h;

		float Left() const { return x; }
		float Right() const { return x + w; }
		float Top() const { return y; }
		float Bottom() const { return y + h; }
	};
}