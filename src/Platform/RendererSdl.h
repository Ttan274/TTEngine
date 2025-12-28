#pragma once
#include <SDL3/SDL.h>
#include <SDL3_ttf/SDL_ttf.h>
#include "Core/IRenderer.h"

namespace EnginePlatform
{
    class RendererSdl : public EngineCore::IRenderer
    {
    public:
        static void Init();
        static void Shutdown();
        static EngineCore::IRenderer* Get();
        static SDL_Renderer* GetSdl();

        void BeginFrame() override;
        void EndFrame() override;
        void Clear(const EngineCore::Color& color) override;
        void DrawRect(const EngineCore::Rect& rect,
            const EngineCore::Color& color) override;
        void DrawRectOutline(const EngineCore::Rect& rect,
            const EngineCore::Color& color) override;

        void DrawTexture(EngineGame::Texture2D* texture, const EngineCore::Rect& rect) override;
        void DrawTexture(EngineGame::Texture2D* texture, 
            const EngineCore::Rect& src,
            const EngineCore::Rect& dest,
            EngineCore::SpriteFlip flip) override;
        void DrawUIText(const std::string& text,
            float x,
            float y,
            const EngineCore::Color& color) override;

    private:
        static SDL_Renderer* s_Renderer;
        static RendererSdl* s_Instance;

        TTF_Font* m_Font = nullptr;
    };
}