#include "TileMap.h"
#include "Platform/AssetManager.h"

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
		const char* basePath = SDL_GetBasePath();
		std::string gPath = std::string(basePath) + "Assets/Textures/ground.png";
		std::string wPath = std::string(basePath) + "Assets/Textures/wall.png";

		m_GroundTex = EnginePlatform::AssetManager::GetTexture(gPath);
		m_WallTex = EnginePlatform::AssetManager::GetTexture(wPath);
	}

	void TileMap::GenerateTestMap()
	{
		for (int y = 0; y < m_Height; ++y)
		{
			for (int x = 0; x < m_Width; ++x)
			{
				if (y == m_Height - 1 || y == 0 || x == 0 || x == m_Width - 1)
				{
					m_Tiles[y * m_Width + x] = TileType::Wall;
				}
				else
				{
					m_Tiles[y * m_Width + x] = TileType::Ground;
				}
			}
		}
	}

	TileType TileMap::GetTile(int x, int y) const
	{
		if (x < 0 || x >= m_Width || y < 0 || y >= m_Height)
			return TileType::None;

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

	void TileMap::DrawSolidDebug(EngineCore::IRenderer* renderer, const Camera2D& camera) const
	{
		float camX = camera.GetX();
		float camY = camera.GetY();

		int startX = (int)(camX / m_TileSize) - 1;
		int startY = (int)(camY / m_TileSize) - 1;
		
		int tilesX = (int)(800 / m_TileSize) + 2;
		int tilesY = (int)(600 / m_TileSize) + 2;

		int endX = startX + tilesX;
		int endY = startY + tilesY;
		
		for (int y = startY; y <= endY; y++)
		{
			for (int x = startX; x <= endX; x++)
			{
				if (GetTile(x, y) != TileType::Wall)
					continue;
				
				float worldX = x * m_TileSize;
				float worldY = y * m_TileSize;
				
				float screenX = worldX - camX;
				float screenY = worldY - camY;
				
				renderer->DrawRectOutline(
					{ screenX, screenY, (float)m_TileSize, (float)m_TileSize },
					{ 255, 0, 0, 255 });
			}
		}
	}

	bool TileMap::IsSolid(const EngineCore::AABB& box) const
	{
		int startX = (int)(box.Left() / m_TileSize);
		int endX =	(int)(box.Right() / m_TileSize);
		int startY = (int)(box.Top() / m_TileSize);
		int endY =	(int)(box.Bottom() / m_TileSize);

		for (int y = startY; y <= endY; ++y)
		{
			for (int x = startX; x <= endX; ++x)
			{
				TileType tile = GetTile(x, y);
				if (tile == TileType::Wall)
					return true;
			}
		}
		return false;
	}

	bool TileMap::IsSolidAt(float worldX, float worldY)
	{
		int tileX = (int)(worldX / m_TileSize);
		int tileY = (int)(worldY / m_TileSize);

		TileType tile = GetTile(tileX, tileY);
		return tile == TileType::Wall;
	}

	bool TileMap::IsSolidAt(EngineMath::Vector2 pos)
	{
		int tileX = (int)(pos.x / m_TileSize);
		int tileY = (int)(pos.y / m_TileSize);

		TileType tile = GetTile(tileX, tileY);
		return tile == TileType::Wall;
	}

	float TileMap::GetWorldWidth() const
	{
		return m_Width * m_TileSize;
	}

	float TileMap::GetWorldHeight() const
	{
		return m_Height * m_TileSize;
	}
}