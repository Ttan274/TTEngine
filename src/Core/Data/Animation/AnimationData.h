#pragma once
#include <string>
#include <vector>

namespace EngineData
{
	struct AnimationData
	{
		std::string id;
		std::string spritePath;

		float frameW;
		float frameH;
		int frameCount;
		float frameTime;
		bool loop;

		std::vector<int> eventFrames;
	};
}