#pragma once
#include <vector>
#include <memory>
#include "Game/Traps/Trap.h"

namespace EngineGame
{
	class TrapManager
	{
	public:
		void Update(float dt, Player& player);
		void Render(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera);
		void DebugDraw(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera);

		void Add(const TrapInstance& instance);
		void Clear();
		std::unique_ptr<Trap> CreateTrap(const TrapInstance& instance);

	private:
		std::vector<std::unique_ptr<Trap>> m_Traps;
	};
}