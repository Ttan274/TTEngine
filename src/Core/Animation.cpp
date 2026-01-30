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

		float safeDt = std::min(dt, 0.033f);
		m_Timer += safeDt;

		if (m_Timer >= m_FrameTime)
		{
			m_Timer -= m_FrameTime;
			m_PreviousFrame = m_CurrentFrame;

			if (m_CurrentFrame < (int)m_Frames.size() - 1)
			{
				m_CurrentFrame++;
			}
			else if (m_Loop)
			{
				m_CurrentFrame = 0;
			}
		}
	}

	const SDL_FRect& Animation::GetCurrentFrame() const
	{
		return m_Frames[m_CurrentFrame];
	}

	void Animation::Reset()
	{
		m_CurrentFrame = 0;
		m_Timer = 0.0f;
		m_PreviousFrame = -1;
	}

	bool Animation::IsFinished() const
	{
		return	!m_Loop && 
				m_CurrentFrame == (int)m_Frames.size() - 1;
	}

	int Animation::GetCurrentFrameIndex() const
	{
		return m_CurrentFrame;
	}

	int Animation::GetFrameCount() const
	{
		return static_cast<int>(m_Frames.size());
	}

	bool Animation::IsEventTriggered() const
	{
		if (m_CurrentFrame == m_PreviousFrame)
			return false;

		return std::find(m_EventFrames.begin(),
			m_EventFrames.end(),
			m_CurrentFrame) != m_EventFrames.end();
	}

	bool Animation::IsInEventWindow() const
	{
		return std::find(m_EventFrames.begin(),
			m_EventFrames.end(),
			m_CurrentFrame) != m_EventFrames.end();
	}
}