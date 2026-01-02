#include "TileMap.h"
#include "Platform/AssetManager.h"
#include "Core/PathUtil.h"

namespace EngineGame
{
	TileMap::TileMap(int width, int height, int tileSize)
		: m_Width(width), m_Height(height), m_TileSize(tileSize)
	{
		m_Tiles.resize(width * height, TileType::None);

		m_GroundTex = nullptr;
		m_WallTex = nullptr;
	}

	void TileMap::LoadAssets()
	{
		std::string exeDir = EngineCore::GetRootDirectory() + "\\Assets/Textures";
		m_GroundTex = EnginePlatform::AssetManager::GetTexture(exeDir + "\\ground.png");
		m_WallTex = EnginePlatform::AssetManager::GetTexture(exeDir + "\\wall.png");
	}

	TileType TileMap::GetTile(int x, int y) const
	{
		if (x < 0 || x >= m_Width || y < 0 || y >= m_Height)
			return TileType::Wall;

		return m_Tiles[y * m_Width + x];
	}

	void TileMap::Draw(EngineCore::IRenderer* renderer, const Camera2D& camera) const
	{
		float camX = camera.GetX();
		float camY = camera.GetY();

		int startX = (int)(camX / m_TileSize) - 2;
		int startY = (int)(camY / m_TileSize) - 2;

		int endX = startX + (int)(800 / m_TileSize) + 4;
		int endY = startY + (int)(600 / m_TileSize) + 4;

		Texture2D* t = nullptr;
		for (int y = startY; y <= endY; y++)
		{
			for (int x = startX; x <= endX; x++) 
			{
				TileType tile = GetTile(x, y);
				if (tile == TileType::None)
					continue;

				float worldX = x * m_TileSize;
				float worldY = y * m_TileSize;

				float screenX = worldX - camX;
				float screenY = worldY - camY;

				t = tile == TileType::Wall ? m_WallTex : m_GroundTex;

				if (!t) continue;

				renderer->DrawTexture(
					t,
					{screenX, screenY, (float)m_TileSize, (float)m_TileSize});
			}
		}
	}

	void TileMap::DrawCollisionDebug(EngineCore::IRenderer* renderer, const Camera2D& camera) const
	{
		float camX = camera.GetX();
		float camY = camera.GetY();

		for (int y = 0; y < m_Height; y++)
		{
			for (int x = 0; x < m_Width; x++)
			{
				TileType tile = GetTile(x, y);
				if (tile == TileType::None)
					continue;

				EngineCore::Color c = tile == TileType::Wall 
									? EngineCore::Color{255, 0 ,0, 255}
									: EngineCore::Color{0, 255, 0, 255 };

				renderer->DrawRectOutline(
				{
					x * m_TileSize - camX,
					y * m_TileSize - camY,
					(float)m_TileSize,
					(float)m_TileSize
				},
				c);
			}
		}
	}

	float TileMap::GetWorldWidth() const
	{
		return (float)(m_Width * m_TileSize);
	}

	float TileMap::GetWorldHeight() const
	{
		return (float)(m_Height * m_TileSize);
	}

	//Needs update
	bool TileMap::IsWall(int x, int y) const
	{
		return GetTile(x, y) == TileType::Wall;
	}

	bool TileMap::IsGround(int x, int y) const
	{
		return GetTile(x, y) == TileType::Ground;
	}

	bool TileMap::IsSolidX(const EngineCore::AABB& box) const
	{
		int startX = (int)(box.Left() / m_TileSize);
		int endX = (int)(box.Right() / m_TileSize);
		int startY = (int)(box.Top() / m_TileSize);
		int endY = (int)(box.Bottom() / m_TileSize);

		for (int y = startY; y <= endY; ++y)
		{
			for (int x = startX; x <= endX; ++x)
			{
				if (IsWall(x, y))
					return true;
			}
		}
		return false;
	}

	bool TileMap::IsSolidY(const EngineCore::AABB& box, float velocityY) const
	{
		int startX = (int)(box.Left() / m_TileSize);
		int endX = (int)(box.Right() / m_TileSize);

		if (velocityY < 0)
		{
			int y = (int)(box.Bottom()/ m_TileSize);

			for (int x = startX; x <= endX; x++)
			{
				if (IsGround(x, y) || IsWall(x, y))
					return true;
			}
		}
		else if (velocityY > 0)
		{
			int y = (int)(box.Top() / m_TileSize);

			for (int x = startX; x <= endX; x++)
			{
				if (IsWall(x, y))
					return true;
			}
		}

		return false;
	}

	bool TileMap::IsGrounded(const EngineCore::AABB& box) const
	{
		int startX = (int)(box.Left() / m_TileSize);
		int endX = (int)(box.Right() / m_TileSize);
		int y = (int)((box.Bottom() + 1) / m_TileSize);

		for (int x = startX; x <= endX; x++)
		{
			if (IsGround(x, y) || IsWall(x, y))
				return true;
		}
		return false;
	}
}