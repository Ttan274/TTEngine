#include "Time.h"
#include <SDL3/SDL.h>

namespace EngineCore
{
	float Time::s_DeltaTime = 0.0f;
	static Uint64 lastTime = 0;

	void Time::Update()
	{
		Uint64 current = SDL_GetPerformanceCounter();
		Uint64 freq = SDL_GetPerformanceFrequency();

		s_DeltaTime = (float)(current - lastTime) / (float)freq;
		lastTime = current;
	}

	float Time::GetDeltaTime()
	{
		return s_DeltaTime;
	}
} 