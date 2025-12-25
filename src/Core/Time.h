#pragma once

namespace EngineCore
{
	class Time
	{
	public:
		static void Update();
		static float GetDeltaTime();
	private:
		static float s_DeltaTime;
	};
}