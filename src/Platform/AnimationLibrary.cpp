#include "AnimationLibrary.h"
#include "Core/Log.h"
#include <json.hpp>
#include <fstream>
#include "Platform/AssetManager.h"

using json = nlohmann::json;

namespace EnginePlatform
{
	std::unordered_map<std::string, AnimationData> AnimationLibrary::s_Animations;

	bool AnimationLibrary::LoadAllAnims(const std::string& folder)
	{
		s_Animations.clear();

		for (const auto& entry : std::filesystem::directory_iterator(folder))
		{
			if (entry.path().extension() != ".json")
				continue;

			std::ifstream file(entry.path());
			if (!file.is_open())
				continue;

			json j;
			file >> j;

			AnimationData anim;
			anim.id = j["Id"];
			anim.spritePath = j["SpriteSheetPath"].get<std::string>();
			anim.frameW = j["FrameWidth"].get<float>();
			anim.frameH = j["FrameHeight"].get<float>();
			anim.frameCount = j["FrameCount"];
			anim.frameTime = j["FrameTime"].get<float>();
			anim.loop = j["Loop"];

			if (j.contains("EventFrames"))
				anim.eventFrames = j["EventFrames"].get<std::vector<int>>();

			s_Animations[anim.id] = anim;
		}

		EngineCore::Log::Write(
			EngineCore::LogLevel::Info,
			EngineCore::LogCategory::Scene,
			"Animations loaded: " + std::to_string(s_Animations.size())
		);

		return true;
	}

	const AnimationData* AnimationLibrary::Get(const std::string& id)
	{
		auto it = s_Animations.find(id);
		return it != s_Animations.end() ? &it->second : nullptr;
	}

	EngineCore::Animation AnimationLibrary::CreateAnimation(const std::string& id)
	{
		EngineCore::Animation anim;

		auto it = s_Animations.find(id);
		if (it == s_Animations.end())
			return anim;

		const AnimationData& data = it->second;

		anim.SetLoop(data.loop);
		anim.SetFrameTime(data.frameTime);

		EngineGame::Texture2D* tex = AssetManager::GetTexture(data.spritePath);
		anim.SetTexture(tex);

		for (int i = 0; i < data.frameCount; i++)
		{
			anim.AddFrame({ i * data.frameW, 0.0f, data.frameW, data.frameH });
		}

		anim.SetEventFrames(data.eventFrames);

		return anim;
	}
}