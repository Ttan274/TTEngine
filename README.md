# TTEngine – Custom 2D Game Engine & Editor

TTEngine is a custom-built 2D game engine written in **C++**, accompanied by a **WPF-based level and entity editor** written in **C#**.  
The project focuses on building a **data-driven game pipeline**, where maps, entities, and behaviors are authored in an editor and consumed by the engine at runtime.

---

## 🎯 Project Goals

- Build a lightweight 2D engine from scratch
- Separate **engine**, **game logic**, and **editor**
- Use **data-driven design** instead of hardcoded values
- Support external **map**, **entity**, and **definition** files
- Allow fast iteration through a custom editor

---

## 🧱 Engine Architecture (C++)

### Core Systems
- Custom game loop (update / render)
- Camera system (2D camera with world → screen transform)
- AssetManager with texture caching
- Logging system (Info / Warning / Error / Fatal)
- Math utilities (Vector2, Rect, AABB)

### Rendering
- SDL3 + SDL3_image
- Texture rendering with source/destination rectangles
- Sprite flipping (left / right)
- Debug rendering (colliders, tiles, attack boxes)

---

## 🗺 TileMap System

- Grid-based tile map
- Tile types:
  - `None`
  - `Ground`
  - `Wall`
- TileMap supports:
  - Rendering only visible tiles (camera-based culling)
  - World size calculation
  - Solid checks using AABB

### Collision Model
- AABB-based collision
- X and Y axis collision handled separately
- Ground detection
- Solid tiles block movement from all directions

---

## 🧍 Entity System

### Base Entity
- Position
- Velocity
- AABB collider
- World reference (TileMap)
- Health system
- Damage flashing
- Attack state handling

### Player
- Horizontal movement (A / D)
- Jump + gravity-based physics
- Grounded checks
- Attack system
- Hurt / Death states
- Camera follow

### Enemy
- AI states:
  - Idle
  - Patrol
  - Attack
  - Hurt
  - Dead
- Attack range & cooldown
- Knockback handling
- Shared physics system with player (gravity + collision)

---

## ⚙ Physics System

- Custom physics (no Rigidbody)
- Gravity applied manually
- Separate:
  - Movement input
  - Physics update
  - Collision resolution
- Prevents:
  - Wall penetration
  - Ground clipping
  - Mid-air floating

---

## 🧠 Combat System

- Attack boxes (Rect-based)
- Facing-direction based attacks
- Hit detection with Intersects()
- Per-attack hit locking (cannot hit multiple times per frame)
- Damage, knockback, death handling

---

## 🧩 Data-Driven Design

### Entity Definitions (JSON)

Entities are no longer hardcoded.

Each entity definition includes:
- `Id`
- `Speed`
- `AttackDamage`
- `AttackInterval`
- `MaxHP`
- Textures:
  - Idle
  - Walk
  - Hurt
  - Death
  - AttackTextures (list)

Example:
```json
{
  "Id": "Samurai",
  "Speed": 80,
  "AttackDamage": 10,
  "AttackInterval": 0.5,
  "MaxHP": 70,
  "IdleTexture": "idle1.png",
  "WalkTexture": "walk1.png",
  "HurtTexture": "hurt1.png",
  "DeathTexture": "dead1.png",
  "AttackTextures": ["attack4.png"]
}

---

## 🗺 Map System (Editor → JSON → Engine)

The map system is fully **editor-driven** and serialized to JSON.  
Maps are never hardcoded in the engine.

### Map Authoring (Editor Side)

Maps are created using a custom **WPF Tile Map Editor**.

Editor features:
- Grid-based tile painting
- Tile types:
  - `None`
  - `Ground`
  - `Wall`
- Brush tool
- Fill tool
- Adjustable brush size
- Undo / redo system
- Visual grid rendering
- Mouse-based painting

---

### Spawn Placement

The editor supports explicit spawn placement:

- **Player Spawn**
  - Single spawn point
  - Assigned a `DefinitionId` (usually `"Player"`)

- **Enemy Spawns**
  - Multiple spawn points
  - Each spawn references an `EntityDefinitionId`
  - Allows different enemy types in the same map

Spawns are placed visually on the map canvas.

---

### Map JSON Format

Maps are exported as JSON files and loaded directly by the engine.

Example structure:

```json
{
  "Width": 50,
  "Height": 30,
  "TileSize": 50,
  "Tiles": [0, 0, 2, 2, 1, ...],
  "PlayerSpawn": {
    "X": 30,
    "Y": 4,
    "DefinitionId": "Player"
  },
  "EnemySpawns": [
    { "X": 5,  "Y": 19, "DefinitionId": "Samurai" },
    { "X": 11, "Y": 11, "DefinitionId": "Samurai" }
  ]
}


