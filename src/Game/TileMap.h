#pragma once
#include "Game/Camera.h"
#include "Core/IRenderer.h"
#include "Core/AABB.h"
#include "Game/Texture.h"
#include <vector>
#include "Core/Math/Vector2.h"

namespace EngineGame
{
	enum class TileType
	{
		None = 0,
		Ground,
		Wall
	};

	class TileMap
	{
	public:
		TileMap(int width, int height, int tileSize);
		void LoadAssets();

		void GenerateTestMap();
		void Draw(EngineCore::IRenderer* renderer, const Camera2D& camera) const;
		void DrawSolidDebug(EngineCore::IRenderer* renderer, const Camera2D& camera) const;
		
		bool IsSolid(const EngineCore::AABB& box) const;
		bool IsSolidAt(float worldX, float worldY);
		bool IsSolidAt(EngineMath::Vector2 pos);

		float GetWorldWidth() const;
		float GetWorldHeight() const;

		int GetTileSize() const { return m_TileSize; }
		int GetHeight() const { return m_Height; }
		int GetWidth() const { return m_Width; }
		const std::vector<TileType>& GetTiles() const { return m_Tiles; }
		void SetTiles(const std::vector<TileType>& tiles) { m_Tiles = tiles; }
	private:
		int m_Width;
		int m_Height;
		int m_TileSize;

		Texture2D* m_GroundTex;
		Texture2D* m_WallTex;

		std::vector<TileType> m_Tiles;
		TileType GetTile(int x, int y) const;
	};
}