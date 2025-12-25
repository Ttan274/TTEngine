#pragma once
#include <vector>
#include <SDL3/SDL.h>

namespace EngineCore
{
	class Animation
	{
	public:
		void Update(float dt);
		const SDL_FRect& GetCurrentFrame() const;

		void AddFrame(const SDL_FRect& rect);
		void SetFrameTime(float time);
	private:
		std::vector<SDL_FRect> m_Frames;
		float m_Timer = 0.0f;
		float m_FrameTime = 0.15f;
		int m_CurrentFrame = 0;
	};
}