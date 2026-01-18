# TTEngine Custom 2D Game Engine & Editor

TTEngine is a custom-built 2D game engine written in **C++**, accompanied by a **WPF-based level and entity editor** written in **C#**.  
The project focuses on building a **data-driven game pipeline**, where maps, entities, and behaviors are authored in an editor and consumed by the engine at runtime.

---

## 🎯 Project Goals

- Build a **custom 2D game engine** using C++
- Design a **standalone editor** for authoring game data
- Understand **engine architecture**, rendering flow, and asset pipelines
- Explore **tooling-first development** instead of engine-only gameplay
- Create a clean separation between runtime and editor responsibilities

---

## 🧱 High-Level Architecture

```tex
TTEngine
├── Engine Runtime (C++)
│ ├── Core Systems
│ ├── Platform Layer (SDL3)
│ ├── Rendering / Input / Time
│ └── Game Logic
│
├── Editor (C# / WPF)
│ ├── Map Editor
│ ├── Animation Editor
│ ├── Asset Definitions
│ └── Data Serialization
│
├── Assets
│ ├── Textures
│ ├── Animations
│ ├── Maps
│ └── Data Files
│
└── External Libraries
```


---

## ⚙️ Engine Runtime (C++)

The engine runtime is responsible for:

- Application lifecycle and game loop
- Platform abstraction using **SDL3**
- Rendering, input handling, and timing
- Loading and interpreting externally-authored asset data
- Executing game logic independent of editor tools

The engine is designed to **consume data**, not author it.

---

## 🛠 Editor (C# / WPF)

The editor is a separate application built to create and manage engine data:

- Tile-based map editing
- Sprite animation definitions and timelines
- Asset inspection and validation
- Data serialization for engine consumption

The editor exports structured data (e.g. JSON) that the engine loads at runtime.

This separation allows rapid tooling iteration without affecting engine stability.

---

## 📦 Asset-Driven Workflow

TTEngine follows a data-driven approach:

```tex
Editor → Asset Files → Engine Runtime
```


- The editor produces all gameplay and visual data
- The engine loads and interprets assets at runtime
- No gameplay logic is hard-coded into editor tools

---

## 🧠 Design Philosophy

- Clear separation of concerns
- Engine-first, not game-first
- Tooling is as important as runtime systems
- Prefer explicit systems over hidden abstractions
- Learn-by-building, not by wrapping existing engines

---

## 🚀 Current Features

- SDL3-based windowing and input
- Custom rendering pipeline (2D)
- Tilemap support
- Sprite animation system
- Standalone editor for map and animation authoring
- Asset serialization and loading

---


## 📌 Intended Use

This project is primarily intended as:

- A **learning-oriented engine architecture project**
- A **portfolio showcase** for engine and tooling development
- A foundation for experimenting with custom engine systems

It is not intended to compete with production-ready engines.

---


