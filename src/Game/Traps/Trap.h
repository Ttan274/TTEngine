#pragma once
#include "Core/AABB.h"
#include "Core/IRenderer.h"
#include "Core/PathUtil.h"
#include "Platform/AssetManager.h"
#include "Game/Camera.h"
#include "Core/Data/Interactable/TrapData.h"

namespace EngineGame
{
	struct TrapInstance
	{
		const EngineData::TrapData* def = nullptr;
		EngineMath::Vector2 position;
		EngineCore::AABB collider;
	};

	class Player;

	class Trap
	{
	public:
		explicit Trap(const TrapInstance& instance) : m_Instance(instance) {}

		virtual ~Trap() = default;

		virtual void Update(float dt, Player& player) = 0;


		void Render(EngineCore::IRenderer* renderer, const Camera2D& camera)
		{
			if (!m_Instance.def)
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

		void DebugDraw(EngineCore::IRenderer* renderer, const Camera2D& camera)
		{
			if (!m_Instance.def)
				return;

			EngineCore::Rect r;
			r.x = m_Instance.collider.Left() - camera.GetX();
			r.y = m_Instance.collider.Top() - camera.GetY();
			r.w = m_Instance.collider.Width();
			r.h = m_Instance.collider.Height();

			renderer->DrawRectOutline(r, { 255, 0, 0, 255 });
		}

		const EngineCore::AABB& GetCollider() const { return m_Instance.collider; }
		const EngineMath::Vector2 GetPosition() const { return m_Instance.position; }

	protected:
		TrapInstance m_Instance;
	};
}