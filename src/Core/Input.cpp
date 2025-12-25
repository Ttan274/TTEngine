#include "Input.h"

namespace EngineCore
{
	bool Input::s_Quit = false;
	std::unordered_map<KeyCode, KeyState> Input::s_KeyState;

	/*Lifecycle*/
	void Input::Init()
	{
		s_Quit = false;
		s_KeyState.clear();

		const KeyCode allKeys[] = {
			KeyCode::W,
			KeyCode::A,
			KeyCode::S,
			KeyCode::D,
			KeyCode::Escape,
			KeyCode::F1,
			KeyCode::F5,
			KeyCode::F9
		};

		for(KeyCode key : allKeys)
			s_KeyState[key] = KeyState::None;
	}

	void Input::Update()
	{
		for (auto& [key, state] : s_KeyState)
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
	KeyCode Input::TranslateSdlKey(int sdlKey)
	{
		switch (sdlKey)
		{
		case SDLK_W:
			return KeyCode::W;
		case SDLK_A:
			return KeyCode::A;
		case SDLK_S:
			return KeyCode::S;
		case SDLK_D:
			return KeyCode::D;
		case SDLK_ESCAPE:
			return KeyCode::Escape;
		case SDLK_F1:
			return KeyCode::F1;
		case SDLK_F5:
			return KeyCode::F5;
		case SDLK_F9:
			return KeyCode::F9;
		default:
			return KeyCode::Unknown;
		}
	}
}