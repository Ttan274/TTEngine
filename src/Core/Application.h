#pragma once
#include "Platform/Scene.h"

namespace EngineCore 
{
	class IRenderer;
	class Application
	{
	public:
		Application();
		~Application();

		void Run();
	private:
		bool m_Running = true;
		IRenderer* m_Renderer = nullptr;
		EnginePlatform::Scene m_Scene;  

		void ProcessInput();
		void Update(float deltaTime);
		void Render();
	};
}