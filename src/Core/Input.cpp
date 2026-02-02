#include "Input.h"

namespace EngineCore
{
	bool Input::s_Quit = false;
	std::unordered_map<KeyCode, KeyState> Input::s_KeyState;
	std::unordered_map<MouseButton, KeyState> Input::s_MouseState;
	int Input::s_MouseX = 0;
	int Input::s_MouseY = 0;

	/*Lifecycle*/
	void Input::Init()
	{
		s_Quit = false;
		s_KeyState.clear();
		s_MouseState.clear();
		s_MouseState[MouseButton::Left] = KeyState::None;
		s_MouseState[MouseButton::Right] = KeyState::None;
		s_MouseState[MouseButton::Middle] = KeyState::None;

		const KeyCode allKeys[] = {
			KeyCode::W,
			KeyCode::A,
			KeyCode::S,
			KeyCode::D,
			KeyCode::E,
			KeyCode::Escape,
			KeyCode::F1,
			KeyCode::F5,
			KeyCode::F9
		};

		for(KeyCode key : allKeys)
			s_KeyState[key] = KeyState::None;
	}

	void Input::BeginFrame()
	{

	}

	void Input::EndFrame()
	{
		for (auto& [key, state] : s_KeyState)
		{
			if (state == KeyState::Pressed)
				state = KeyState::Held;
			else if (state == KeyState::Released)
				state = KeyState::None;
		}

		for (auto& [btn, state] : s_MouseState)
		{
			if (state == KeyState::Pressed)
				state = KeyState::Held;
			else if (state == KeyState::Released)
				state = KeyState::None;
		}
	}

	/*SDL bridge*/
	bool Input::IsQuit()
	{
		return s_Quit;
	}

	void Input::OnQuit()
	{
		s_Quit = true;
	}

	void Input::OnKey(int sdlKey, bool pressed)
	{
		KeyCode key = TranslateSdlKey(sdlKey);
		if (key == KeyCode::Unknown)
			return;

		if (pressed)
		{
			if (s_KeyState[key] == KeyState::None ||
				s_KeyState[key] == KeyState::Released)
			{
				s_KeyState[key] = KeyState::Pressed;
			}
		}
		else
		{
			s_KeyState[key] = KeyState::Released;
		}
	}

	void Input::OnMouseButton(int sdlBtn, bool pressed)
	{
		MouseButton btn;

		switch (sdlBtn)
		{
		case SDL_BUTTON_LEFT:
			btn = MouseButton::Left;
			break;
		case SDL_BUTTON_RIGHT:
			btn = MouseButton::Right;
			break;
		case SDL_BUTTON_MIDDLE:
			btn = MouseButton::Middle;
			break;
		default:
			return;
		}

		if (pressed)
		{
			if (s_MouseState[btn] == KeyState::None ||
				s_MouseState[btn] == KeyState::Released)
			{
				s_MouseState[btn] = KeyState::Pressed;
			}
		}
		else
		{
			s_MouseState[btn] = KeyState::Released;
		}
	}

	void Input::OnMouseMove(int x, int y)
	{
		s_MouseX = x;
		s_MouseY = y;
	}

	/*Queries*/
	bool Input::IsKeyDown(KeyCode key)
	{
		auto k = s_KeyState.find(key);
		if (k == s_KeyState.end())
			return false;

		return k->second == KeyState::Pressed || k->second == KeyState::Held;
	}

	bool Input::IsKeyPressed(KeyCode key)
	{
		auto k = s_KeyState.find(key);
		if (k == s_KeyState.end())
			return false;

		return k->second == KeyState::Pressed;
	}

	bool Input::IsKeyReleased(KeyCode key)
	{
		auto k = s_KeyState.find(key);
		if (k == s_KeyState.end())
			return false;

		return k->second == KeyState::Released;
	}

	bool Input::IsMouseButtonDown(MouseButton btn)
	{
		return s_MouseState[btn] == KeyState::Pressed || s_MouseState[btn] == KeyState::Held;
	}

	bool Input::IsMouseButtonPressed(MouseButton btn)
	{
		return s_MouseState[btn] == KeyState::Pressed;
	}

	bool Input::IsMouseButtonReleased(MouseButton btn)
	{
		return s_MouseState[btn] == KeyState::Released;
	}

	EngineMath::Vector2 Input::GetMousePosition()
	{
		return {
			static_cast<float>(s_MouseX),
			static_cast<float>(s_MouseY)
		};
	}

	/*Axis*/
	float Input::GetAxisHorizontal()
	{
		float axis = 0.0f;
		if (IsKeyDown(KeyCode::A))
			axis -= 1.0f;
		if (IsKeyDown(KeyCode::D))
			axis += 1.0f;
		return axis;
	}

	float Input::GetAxisVertical()
	{
		float axis = 0.0f;
		if (IsKeyDown(KeyCode::W))
			axis -= 1.0f;
		if (IsKeyDown(KeyCode::S))
			axis += 1.0f;
		return axis;
	}

	//Helper method
	KeyCode Input::TranslateSdlKey(int scancode)
	{
		switch (scancode)
		{
		case SDL_SCANCODE_W:
			return KeyCode::W;
		case SDL_SCANCODE_A:
			return KeyCode::A;
		case SDL_SCANCODE_S:
			return KeyCode::S;
		case SDL_SCANCODE_D:
			return KeyCode::D;
		case SDL_SCANCODE_E:
			return KeyCode::E;
		case SDL_SCANCODE_ESCAPE:
			return KeyCode::Escape;
		case SDL_SCANCODE_F1:
			return KeyCode::F1;
		case SDL_SCANCODE_F5:
			return KeyCode::F5;
		case SDL_SCANCODE_F9:
			return KeyCode::F9;
		default:
			return KeyCode::Unknown;
		}
	}
}