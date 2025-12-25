#pragma once
#include <string>
#include <unordered_map>
#include "Game/Texture.h"

namespace EnginePlatform
{
	class AssetManager
	{
	public:
		static void Init(SDL_Renderer* renderer);
		static EngineGame::Texture2D* GetTexture(const std::string& path);
		static void Shutdown();
	private:
		static SDL_Renderer* s_Renderer;
		static std::unordered_map<std::string, EngineGame::Texture2D*> s_Textures;
	};
}