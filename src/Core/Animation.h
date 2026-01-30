#pragma once
#include <vector>
#include <SDL3/SDL.h>
#include "Game/Texture.h"

namespace EngineCore
{
	class Animation
	{
	public:
		void Update(float dt);
		const SDL_FRect& GetCurrentFrame() const;

		void AddFrame(const SDL_FRect& rect);
		void SetFrameTime(float time);

		int GetCurrentFrameIndex() const;
		int GetFrameCount() const;

		void SetLoop(bool loop) { m_Loop = loop; }
		void Reset();
		bool IsFinished() const;

		void SetEventFrames(const std::vector<int>& frames) { m_EventFrames = frames; }
		bool IsEventTriggered() const;
		bool IsInEventWindow() const;

		void SetTexture(EngineGame::Texture2D* tex) { m_Texture = tex; }
		EngineGame::Texture2D* GetTexture() const { return m_Texture; }
	private:
		std::vector<SDL_FRect> m_Frames;
		float m_Timer = 0.0f;
		float m_FrameTime = 0.15f;
		int m_CurrentFrame = 0;
		int m_PreviousFrame = -1;
		bool m_Loop = true;
		EngineGame::Texture2D* m_Texture = nullptr;
		std::vector<int> m_EventFrames;
	};
}