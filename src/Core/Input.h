#pragma once
#include <SDL3/SDL.h>
#include <unordered_map>

namespace EngineCore
{
	enum class KeyCode
	{
		Unknown = 0,
		W,
		A,
		S,
		D,
		E,
		Escape,
		F1,
		F5,
		F9
	};

	enum class KeyState
	{
		None = 0,
		Pressed,
		Held,
		Released
	};

	class Input
	{
	public:
		//Lifecycle
		static void Init();
		static void Update();
		static void BeginFrame();
		static void EndFrame();

		//Queries
		static bool IsKeyDown(KeyCode key);		//Held | Pressed
		static bool IsKeyPressed(KeyCode key);	//First frame pressed
		static bool IsKeyReleased(KeyCode key); //First frame released

		//Axis
		static float GetAxisHorizontal();		// A/D or Left/Right
		static float GetAxisVertical();			// W/S or Up/Down

		//SDL bridge
		static void OnKey(int sdlKey, bool pressed);
		static bool IsQuit();
		static void OnQuit();
	private:
		static KeyCode TranslateSdlKey(int sdlKey);
		static bool s_Quit;
		static std::unordered_map<KeyCode, KeyState> s_KeyState;
	};
}