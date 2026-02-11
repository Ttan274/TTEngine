#pragma once
#include "Core/Data/Animation/AnimationData.h"
#include "json.hpp"

namespace EngineData
{
	class AnimationParser
	{
	public:
		static AnimationData Parse(const nlohmann::json& j)
		{
			AnimationData anim;

			anim.id = j.value("Id", "");
			anim.spritePath = j.value("SpriteSheetPath", "");
			anim.frameW = j.value("FrameWidth", 0.f);
			anim.frameH = j.value("FrameHeight", 0.f);
			anim.frameCount = j.value("FrameCount", 0);
			anim.frameTime = j.value("FrameTime", 0.f);;
			anim.loop = j.value("Loop", true);

			if (j.contains("EventFrames"))
				anim.eventFrames = j["EventFrames"].get<std::vector<int>>();

			return anim;
		}
	};
}