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


