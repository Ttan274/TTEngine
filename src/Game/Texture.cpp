#include "Game/Texture.h"
#include "SDL3_image/SDL_image.h"
#include "Core/Log.h"

namespace EngineGame
{
	Texture2D::Texture2D(SDL_Renderer* renderer, const std::string& path)
	{
		SDL_Surface* surface = IMG_Load(path.c_str());
		if (!surface)
			return;

		m_Texture = SDL_CreateTextureFromSurface(renderer, surface);

		m_Width = surface->w;
		m_Height = surface->h;

		SDL_DestroySurface(surface);
	}

	Texture2D::~Texture2D()
	{
		if (m_Texture)
		{
			SDL_DestroyTexture(m_Texture);
			m_Texture = nullptr;
		}
	}
}