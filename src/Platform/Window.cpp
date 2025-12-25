#include "Window.h"

namespace EnginePlatform
{
	SDL_Window* Window::s_Window = nullptr;

	void Window::Init(const char* title, int width, int height)
	{
		SDL_Init(SDL_INIT_VIDEO);
		s_Window = SDL_CreateWindow(
			title,
			width,
			height,
			SDL_WINDOW_RESIZABLE
		);
	}

	void Window::Shutdown()
	{
		SDL_DestroyWindow(s_Window);
		SDL_Quit();
	}

	SDL_Window* Window::Get()
	{
		return s_Window;
	}
}