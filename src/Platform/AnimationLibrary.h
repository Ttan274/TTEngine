#pragma once
#include <string>
#include <unordered_map>
#include "Core/Animation.h"

namespace EnginePlatform
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

	class AnimationLibrary
	{
	public:
		static bool LoadAllAnims(const std::string& folder);
		static const AnimationData* Get(const std::string& id);
		static EngineCore::Animation CreateAnimation(const std::string& id);
	private:
		static std::unordered_map<std::string, AnimationData> s_Animations;
	};
}