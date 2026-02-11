#pragma once
#include "Core/Animation.h"
#include "Platform/LibraryManager.h"
#include "Platform/AssetManager.h"

namespace EngineGame
{
	class Animator
	{
	public:
		static EngineCore::Animation Create(const std::string& id)
		{
			EngineCore::Animation anim;

			const EngineData::AnimationData* data = EnginePlatform::AnimationLibrary::Get(id);
			if (!data)
				return anim;

			anim.SetLoop(data->loop);
			anim.SetFrameTime(data->frameTime);

			Texture2D* tex = EnginePlatform::AssetManager::GetTexture(data->spritePath);
			anim.SetTexture(tex);

			for (int i = 0; i < data->frameCount; i++)
			{
				anim.AddFrame({ i * data->frameW, 0.0f, data->frameW, data->frameH });
			}

			anim.SetEventFrames(data->eventFrames);

			return anim;
		}
	};
}