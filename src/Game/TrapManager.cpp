#include "Game/TrapManager.h"
#include "Game/Traps/SawTrap.h"
#include "Game/Traps/FireTrap.h"

namespace EngineGame
{
	void TrapManager::Update(float dt, Player& player)
	{
		for (auto& it : m_Traps)
			it->Update(dt, player);
	}

	void TrapManager::Render(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera)
	{
		for (auto& it : m_Traps)
			it->Render(renderer, camera);
	}

	void TrapManager::DebugDraw(EngineCore::IRenderer* renderer, const EngineGame::Camera2D& camera)
	{
		for (auto& it : m_Traps)
			it->DebugDraw(renderer, camera);
	}

	void TrapManager::Add(const TrapInstance& instance)
	{
		auto trap = CreateTrap(instance);
		if (trap)
			m_Traps.push_back(std::move(trap));
	}

	void TrapManager::Clear()
	{
		m_Traps.clear();
	}

	std::unique_ptr<Trap> TrapManager::CreateTrap(const TrapInstance& instance)
	{
		if(instance.def->id == "Fire")
			return std::make_unique<FireTrap>(instance);
		if(instance.def->id == "Saw")
			return std::make_unique<SawTrap>(instance);

		return nullptr;
	}
}