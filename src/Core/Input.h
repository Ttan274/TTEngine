#pragma once
#include <SDL3/SDL.h>
#include <unordered_map>
#include "Core/Math/Vector2.h"


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

	enum class MouseButton
	{
		Left,
		Right,
		Middle
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
		static bool IsMouseButtonDown(MouseButton btn);
		static bool IsMouseButtonPressed(MouseButton btn);
		static bool IsMouseButtonReleased(MouseButton btn);
		static EngineMath::Vector2 GetMousePosition();

		//Axis
		static float GetAxisHorizontal();		// A/D or Left/Right
		static float GetAxisVertical();			// W/S or Up/Down

		//SDL bridge
		static void OnKey(int sdlKey, bool pressed);
		static bool IsQuit();
		static void OnQuit();
		static void OnMouseButton(int sdlBtn, bool pressed);
		static void OnMouseMove(int x, int y);
	private:
		static KeyCode TranslateSdlKey(int sdlKey);
		static bool s_Quit;
		static std::unordered_map<KeyCode, KeyState> s_KeyState;
		static std::unordered_map<MouseButton, KeyState> s_MouseState;
		static int s_MouseX;
		static int s_MouseY;
	};
}