#include "Debug.h"

namespace EngineCore
{
	bool Debug::ShowFPS = true;
	bool Debug::ShowPlayerInfo = true;
	bool Debug::ShowInput = false;

	int Debug::s_FPS = 0;
	float Debug::fpsTimer = 0.0f;
	int Debug::frames = 0;

	void Debug::Update(float deltaTime)
	{
		fpsTimer += deltaTime;
		frames++;

		if (fpsTimer >= 1.0f)
		{
			SetFPS(frames);
			frames = 0;
			fpsTimer = 0.0f;
		}
	}

	void Debug::BeginFrame()
	{

	}

	void Debug::EndFrame()
	{

	}

	void Debug::SetFPS(int fps)
	{
		s_FPS = fps;
	}

	int Debug::GetFPS()
	{
		return s_FPS;
	}
}