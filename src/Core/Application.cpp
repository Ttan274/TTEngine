#include "Core/Application.h"
#include "Core/Time.h"
#include "Core/Input.h"
#include "Core/Log.h"
#include "Core/Debug.h"
#include "Core/DebugOverlay.h"
#include "Platform/Window.h"
#include "Platform/AssetManager.h"
#include "Platform/RendererSdl.h"

namespace EngineCore
{
	const std::string SCENE_PATH = "scene_test.json";

	Application::Application()
	{
		Log::Init();
		EnginePlatform::Window::Init("TTEngine", 800, 600);
		EnginePlatform::RendererSdl::Init();

		m_Renderer = EnginePlatform::RendererSdl::Get();
		DebugOverlay::Init(m_Renderer);
		EnginePlatform::AssetManager::Init(EnginePlatform::RendererSdl::GetSdl());
	
		m_Scene.Load();
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

			if (event.type == SDL_EVENT_KEY_DOWN)
				Input::OnKey(event.key.key, true);

			if (event.type == SDL_EVENT_KEY_UP)
				Input::OnKey(event.key.key, false);
		}

		if(Input::IsKeyDown(KeyCode::Escape))
			m_Running = false;

		if (Input::IsKeyPressed(KeyCode::F1))
			DebugOverlay::Toggle();

		//Save atýyoruz
		if (Input::IsKeyPressed(KeyCode::F5))
		{
			
		}

		//Load yapýyoruz
		if (Input::IsKeyPressed(KeyCode::F9))
		{
			
		}
	}

	void Application::Update(float deltaTime)
	{
		Debug::Update(deltaTime);
		if (DebugOverlay::IsEnabled())
			DebugOverlay::AddLine("FPS: " + std::to_string(Debug::GetFPS()));

		Input::Update();
		m_Scene.Update(deltaTime);
	}

	void Application::Render()
	{
		m_Renderer->BeginFrame();

		m_Scene.Render(m_Renderer);
		DebugOverlay::Render();
	
		m_Renderer->EndFrame();
	}
}