#pragma once

namespace EngineCore
{
	class Debug
	{
	public:
		static void Update(float deltaTime);

		static void BeginFrame();
		static void EndFrame();

		static void SetFPS(int fps);
		static int GetFPS();

		static bool ShowFPS;
		static bool ShowPlayerInfo;
		static bool ShowInput;
	private:
		static int s_FPS;
		static float fpsTimer;
		static int frames;
	};
}