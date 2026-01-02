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
		
		//Tile Queries
		TileType GetTile(int x, int y) const;
		bool IsWall(int x, int y) const;
		bool IsGround(int x, int y) const;

		//Collision Queries
		bool IsSolidX(const EngineCore::AABB& box) const; // Wall Only
		bool IsSolidY(const EngineCore::AABB& box, float velocityY) const; //Wall + Ground
		bool IsGrounded(const EngineCore::AABB& box) const;

		//Rendering
		void Draw(EngineCore::IRenderer* renderer, const Camera2D& camera) const;
		void DrawCollisionDebug(EngineCore::IRenderer* renderer, const Camera2D& camera) const;

		float GetWorldWidth() const;
		float GetWorldHeight() const;

		//Setups
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
	};
}