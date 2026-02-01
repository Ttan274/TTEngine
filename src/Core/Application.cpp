#include "Core/Application.h"
#include "Core/Time.h"
#include "Core/Input.h"
#include "Core/Log.h"
#include "Core/Debug.h"
#include "Core/DebugOverlay.h"
#include "Platform/Window.h"
#include "Platform/AssetManager.h"
#include "Platform/RendererSdl.h"
#include "Platform/LevelManager.h"
#include "Core/PathUtil.h"

namespace EngineCore
{
	Application::Application()
	{
		Log::Init();
		EnginePlatform::Window::Init("TTEngine", 800, 600);
		EnginePlatform::RendererSdl::Init();

		m_Renderer = EnginePlatform::RendererSdl::Get();
		DebugOverlay::Init(m_Renderer);
		EnginePlatform::AssetManager::Init(EnginePlatform::RendererSdl::GetSdl());
	
		EnginePlatform::LevelManager::Get().LoadAllLevels(EngineCore::GetFile("Data", "Levels.json"));
		EnginePlatform::LevelManager::Get().StartLevelByIndex(0);

		m_Scene.Load();
		m_Scene.LoadCurrentLevel();
	}

	Application::~Application()
	{
		EnginePlatform::Window::Shutdown();
		EnginePlatform::RendererSdl::Shutdown();
	}

	void Application::Run()
	{
		while (m_Running)
		{
			Debug::BeginFrame();
			DebugOverlay::BeginFrame();
			Time::Update();

			Input::BeginFrame();
			ProcessInput();
			Update(Time::GetDeltaTime());
			Render();

			Debug::EndFrame();
		}
	}

	void Application::ProcessInput()
	{
		SDL_Event event;
		while (SDL_PollEvent(&event))
		{
			if (event.type == SDL_EVENT_QUIT)
				Input::OnQuit();

			if (event.type == SDL_EVENT_KEY_DOWN && !event.key.repeat)
				Input::OnKey(event.key.scancode, true);

			if (event.type == SDL_EVENT_KEY_UP)
				Input::OnKey(event.key.scancode, false);
		}

		if(Input::IsKeyDown(KeyCode::Escape))
			m_Running = false;

		if (Input::IsKeyPressed(KeyCode::F1))
			DebugOverlay::Toggle();
	}

	void Application::Update(float deltaTime)
	{
		Debug::Update(deltaTime);
		if (DebugOverlay::IsEnabled() && m_Scene.GetGameState() == EnginePlatform::GameState::Playing)
		{
			DebugOverlay::AddLine("FPS: " + std::to_string(Debug::GetFPS()));
			DebugOverlay::AddLine("Player hp: " + std::to_string(m_Scene.GetPlayer().GetHp()));
			DebugOverlay::AddLine("Alive enemy count: " + std::to_string(m_Scene.GetAliveEnemyCount()));
		}

		//Input::Update();
		m_Scene.Update(deltaTime);
	}

	void Application::Render()
	{
		m_Renderer->BeginFrame();

		m_Scene.Render(m_Renderer);
		DebugOverlay::Render();
		Input::EndFrame();
	
		m_Renderer->EndFrame();
	}
}