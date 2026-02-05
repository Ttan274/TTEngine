#pragma once
#include "Core/IRenderer.h"
#include "Game/Camera.h"
#include "Platform/InteractableLibrary.h"
#include "Platform/AssetManager.h"
#include "Core/PathUtil.h"
#include "Game/Player.h"

namespace EngineGame
{
	struct InteractableInstance
	{
		const EnginePlatform::InteractableDef* def = nullptr;
		EngineMath::Vector2 position;
		EngineCore::AABB collider;
		bool used = false;
	};

	enum class InteractResult
	{
		None,
		SpawnKey,
		FinishGame
	};

	class Interactable
	{
	public:
		explicit Interactable(const InteractableInstance& instance) : m_Instance(instance) {}

		virtual ~Interactable() = default;

		virtual InteractResult OnInteract(Player& player) = 0;
		
		void Render(EngineCore::IRenderer* renderer, const EngineGame::Camera2D camera)
		{
			if (m_Instance.used || !m_Instance.def)
				return;

			std::string path = EngineCore::GetFile("Textures", m_Instance.def->imagePath);
			EngineGame::Texture2D* tex = EnginePlatform::AssetManager::GetTexture(path);
			
			if (!tex)
				return;

			EngineCore::Rect dst;
			dst.x = m_Instance.position.x - camera.GetX();
			dst.y = m_Instance.position.y - camera.GetY();
			dst.w = m_Instance.collider.Width();
			dst.h = m_Instance.collider.Height();

			renderer->DrawTexture(tex, dst);
		}

		void DebugDraw(EngineCore::IRenderer* renderer, const EngineGame::Camera2D camera)
		{
			if (m_Instance.used || !m_Instance.def)
				return;

			EngineCore::Rect r;
			r.x = m_Instance.collider.Left() - camera.GetX();
			r.y = m_Instance.collider.Top() - camera.GetY();
			r.w = m_Instance.collider.Width();
			r.h = m_Instance.collider.Height();

			renderer->DrawRectOutline(r, { 0, 255, 255, 255 });
		}

		const EngineCore::AABB& GetCollider() const { return m_Instance.collider; }
		bool IsUsed() const { return m_Instance.used; }
		const EngineMath::Vector2 GetPosition() const { return m_Instance.position; }

	protected:

		InteractableInstance m_Instance;
	};
}