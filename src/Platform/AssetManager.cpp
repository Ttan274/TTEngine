#include "AssetManager.h"

namespace EnginePlatform
{
	SDL_Renderer* AssetManager::s_Renderer = nullptr;
	std::unordered_map<std::string, EngineGame::Texture2D*> AssetManager::s_Textures;

	void AssetManager::Init(SDL_Renderer* renderer)
	{
		s_Renderer = renderer;
	}

	EngineGame::Texture2D* AssetManager::GetTexture(const std::string& path)
	{
		if (s_Textures.contains(path))
			return s_Textures[path];

		EngineGame::Texture2D* tex = new EngineGame::Texture2D(s_Renderer, path);
		s_Textures[path] = tex;
		return tex;
	}

	void AssetManager::Shutdown()
	{
		for (auto& [_, tex] : s_Textures)
			delete tex;

		s_Textures.clear();
	}
}