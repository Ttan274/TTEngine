#include "DebugOverlay.h"
#include "IRenderer.h"

namespace EngineCore
{
	std::vector<EngineCore::DebugLine> EngineCore::DebugOverlay::s_Lines;
	IRenderer* DebugOverlay::s_Renderer = nullptr;
	bool DebugOverlay::s_Enabled = true;

	void DebugOverlay::Init(IRenderer* renderer)
	{
		s_Renderer = renderer;
		s_Enabled = true;
	}

	void DebugOverlay::Shutdown()
	{
		s_Lines.clear();
	}

	void DebugOverlay::BeginFrame()
	{
		if (!s_Enabled) return;
		s_Lines.clear();
	}

	void DebugOverlay::AddLine(const std::string& text)
	{
		if (!s_Enabled) return;
		s_Lines.push_back({ text });
	}

	void DebugOverlay::Render()
	{
		if (!s_Renderer || !s_Enabled)
			return;

		int y = 5;

		for (auto& line : s_Lines)
		{
			if (line.text.empty())
				continue;

			s_Renderer->DrawUIText(line.text, 5.0f, y);

			y += 18.0f;
		}
	}

	void DebugOverlay::Toggle()
	{
		s_Enabled = !s_Enabled;
	}	

	bool DebugOverlay::IsEnabled()
	{
		return s_Enabled;
	}
}

