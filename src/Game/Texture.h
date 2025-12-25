#pragma once
#include <SDL3/SDL.h>
#include <string>

namespace EngineGame
{
	class Texture2D
	{
	public:
		Texture2D(SDL_Renderer* renderer, const std::string& path);
		~Texture2D();

		SDL_Texture* Get() const { return m_Texture; }
		int GetWidth() const { return m_Width; }
		int GetHeight() const { return m_Height; }
	private:
		SDL_Texture* m_Texture = nullptr;
		int m_Width = 0;
		int m_Height = 0;
	};
}