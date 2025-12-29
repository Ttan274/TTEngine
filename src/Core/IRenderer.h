#pragma once
#include <string>
#include "Game/Texture.h"
#include "Core/AABB.h"

namespace EngineCore
{
	struct Color
	{
		unsigned char r, g, b, a;
	};

	enum class SpriteFlip
	{
		None,
		Horizontal
	};

	class IRenderer
	{
	public:
		virtual ~IRenderer() = default;

		virtual void BeginFrame() = 0;
		virtual void EndFrame() = 0;

		virtual void Clear(const Color& color) = 0;
		virtual void DrawRect(const Rect& rect, const Color& color) = 0;
		virtual void DrawRectOutline(const Rect& rect, const Color& color) = 0;

		virtual void DrawTexture(EngineGame::Texture2D* texture, const Rect& rect) = 0;
		virtual void DrawTexture(EngineGame::Texture2D* texture, const Rect& src, const Rect& dest, SpriteFlip flip) = 0;
		virtual void DrawUIText(const std::string& text,
							 float x, float y,
							 const Color& color = { 255, 255, 255, 255 }) = 0;
	};
}