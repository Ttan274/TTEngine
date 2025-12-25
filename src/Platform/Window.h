#pragma once
#include <SDL3/SDL.h>

namespace EnginePlatform
{
	class Window
	{
	public:
		static void Init(const char* title, int width, int height);
		static void Shutdown();
		
		static SDL_Window* Get();
	private:
		static SDL_Window* s_Window;
	};
}