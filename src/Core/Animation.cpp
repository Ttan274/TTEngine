#include "Core/Animation.h"

namespace EngineCore
{
	void Animation::AddFrame(const SDL_FRect& rect)
	{
		m_Frames.push_back(rect);
	}

	void Animation::SetFrameTime(float time)
	{
		m_FrameTime = time;
	}

	void Animation::Update(float dt)
	{
		if (m_Frames.empty())
			return;

		m_Timer += dt;
		if (m_Timer >= m_FrameTime)
		{
			m_Timer = 0.0f;
			m_CurrentFrame = (m_CurrentFrame + 1) % m_Frames.size();
		}
	}

	const SDL_FRect& Animation::GetCurrentFrame() const
	{
		return m_Frames[m_CurrentFrame];
	}
}