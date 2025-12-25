#pragma once
#include <string>
#include <vector>

namespace EngineCore
{
	class IRenderer;
	struct DebugLine 
	{
		std::string text;
	};

	class DebugOverlay
	{
	public:
		static void Init(IRenderer* renderer);
		static void Shutdown();

		static void BeginFrame();
		static void AddLine(const std::string& text);
		static void Render();

		static void Toggle();
		static bool IsEnabled();
	private:
		static std::vector<DebugLine> s_Lines;
		static IRenderer* s_Renderer;
		static bool s_Enabled;
	};
}