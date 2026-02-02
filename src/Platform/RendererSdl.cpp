#include "RendererSdl.h"
#include "Window.h"
#include "SDL3/SDL.h"
#include "Game/Texture.h"
#include "Core/PathUtil.h"
#include "Core/Input.h"

namespace EnginePlatform
{
    SDL_Renderer* RendererSdl::s_Renderer = nullptr;
    RendererSdl* RendererSdl::s_Instance = nullptr;
    //Circle
    const int segments = 24;
    const float step = 2.0f * 3.1415926f / segments;

    void RendererSdl::Init()
    {
        s_Renderer = SDL_CreateRenderer(Window::Get(), nullptr);
        s_Instance = new RendererSdl();

        SDL_HideCursor();
        
        std::string fontPath = EngineCore::GetRootDirectory() + "\\Assets/Fonts/FontTest.ttf";
        TTF_Init();
        s_Instance->m_Font = TTF_OpenFont(fontPath.c_str(), 14);
    }

    void RendererSdl::Shutdown()
    {
        if (s_Instance->m_Font)
            TTF_CloseFont(s_Instance->m_Font);

        TTF_Quit();
        SDL_DestroyRenderer(s_Renderer);
        delete s_Instance;
    }

    EngineCore::IRenderer* RendererSdl::Get()
    {
        return s_Instance;
    }

    SDL_Renderer* RendererSdl::GetSdl()
    {
        if (!s_Instance)
            return nullptr;

        return s_Instance->s_Renderer;
    }

    void RendererSdl::BeginFrame()
    {
        Clear({ 20,20,20,255 });
    }

    void RendererSdl::Clear(const EngineCore::Color& c)
    {
        SDL_SetRenderDrawColor(s_Renderer, c.r, c.g, c.b, c.a);
        SDL_RenderClear(s_Renderer);
    }

    void RendererSdl::DrawRect(const EngineCore::Rect& r,
        const EngineCore::Color& c)
    {
        SDL_FRect rect{ r.x, r.y, r.w, r.h };
        SDL_SetRenderDrawColor(s_Renderer, c.r, c.g, c.b, c.a);
        SDL_RenderFillRect(s_Renderer, &rect);
    }

    void RendererSdl::DrawRectOutline(const EngineCore::Rect& r,
        const EngineCore::Color& c)
    {
        SDL_FRect rect{ r.x, r.y, r.w, r.h };
        SDL_SetRenderDrawColor(s_Renderer, c.r, c.g, c.b, c.a);
        SDL_RenderRect(s_Renderer, &rect);
    }

    void RendererSdl::DrawCircle(float cx, float cy, float radius, const EngineCore::Color& color)
    {
        SDL_SetRenderDrawColor(s_Renderer, color.r, color.g, color.b, color.a);

        for (int i = 0; i < segments; i++)
        {
            float a1 = i * step;
            float a2 = (i + 1) * step;

            float x1 = cx + cosf(a1) * radius;
            float y1 = cy + sinf(a1) * radius;

            float x2 = cx + cosf(a2) * radius;
            float y2 = cy + sinf(a2) * radius;

            SDL_RenderLine(s_Renderer, x1, y1, x2, y2);
        }
    }

    void RendererSdl::DrawTexture(EngineGame::Texture2D* texture, const EngineCore::Rect& rect)
    {
        SDL_FRect r{ rect.x, rect.y, rect.w, rect.h };
        SDL_RenderTexture(s_Renderer, texture->Get(), nullptr, &r);
    }

    void RendererSdl::DrawTexture(EngineGame::Texture2D* texture, const EngineCore::Rect& src, const EngineCore::Rect& dest, EngineCore::SpriteFlip flip)
    {
        if (!texture || !texture->Get())
            return;

        SDL_FRect s{ src.x, src.y, src.w, src.h };
        SDL_FRect d{ dest.x, dest.y, dest.w, dest.h };
        SDL_FlipMode sdlFlip = (flip == EngineCore::SpriteFlip::Horizontal) ?
            SDL_FLIP_HORIZONTAL :
            SDL_FLIP_NONE;

        SDL_RenderTextureRotated(s_Renderer, texture->Get(), &s, &d, 0.0, nullptr, sdlFlip);
    }

    void RendererSdl::DrawUIText(const std::string& text,
        float x,
        float y,
        const EngineCore::Color& c)
    {
        if (!m_Font || text.empty())
            return;

        SDL_Color col{ c.r, c.g, c.b, c.a };

        SDL_Surface* surface = TTF_RenderText_Blended(
            m_Font,
            text.c_str(),
            text.length(),
            col
        );

        SDL_Texture* texture =
            SDL_CreateTextureFromSurface(s_Renderer, surface);

        SDL_FRect rect{ x, y,
                        (float)surface->w,
                        (float)surface->h };

        SDL_RenderTexture(s_Renderer, texture, nullptr, &rect);

        SDL_DestroyTexture(texture);
        SDL_DestroySurface(surface);
    }

    EngineCore::ButtonResult RendererSdl::DrawUIButton(const std::string& text,
        const EngineCore::Rect& rect,
        const EngineCore::Color& normal,
        const EngineCore::Color& hover)
    {
        EngineCore::ButtonResult result;

        auto mouse = EngineCore::Input::GetMousePosition();
        bool isHover =
            mouse.x >= rect.x &&
            mouse.x <= rect.x + rect.w &&
            mouse.y >= rect.y &&
            mouse.y <= rect.y + rect.h;

        result.hovered = isHover;
        const EngineCore::Color& bg = isHover ? hover : normal;

        DrawRect(rect, bg);
        DrawRectOutline(rect, { 255, 255, 255, 255 });

        DrawUIText(text,
                rect.x + rect.w * 0.35f,
                rect.y + rect.h * 0.35f,
                { 255, 255, 255, 255 });

        if (isHover && EngineCore::Input::IsMouseButtonPressed(EngineCore::MouseButton::Left))
            result.clicked = true;

        return result;
    }

    void RendererSdl::EndFrame()
    {
        SDL_RenderPresent(s_Renderer);
    }
}