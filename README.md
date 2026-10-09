  <br>
<div align="center">
  <a href="https://ko-fi.com/cursedentertainment">
    <img src="https://ko-fi.com/img/githubbutton_sm.svg" alt="ko-fi" style="width: 20%;"/>
  </a>
</div>
  <br>

<div align="center"> 
  <img alt="Unity" src="https://img.shields.io/badge/unity%206-%23323330.svg?&style=for-the-badge&logo=unity&logoColor=white"/>  
</div>

<div align="center">
  <img alt="C#" src="https://img.shields.io/badge/C%23-%23323330.svg?&style=for-the-badge&logo=csharp&logoColor=white"/> 
</div>

# oU-RTS

Open Source Unity RTS Editor  
Real-Time Strategy (RTS) with a Command & Conquer style UI.

<div align="center">
  <img src="DemoImages/Demo.png" alt="oU-RTS gameplay" width="80%"/>
</div>

## Requirements

- **Unity 6** (6000.0.20f1 or newer 6000.0.x LTS)
- Built-in Render Pipeline (no URP/HDRP setup needed)
- Visual Studio 2022 or JetBrains Rider

## Getting Started

1. Clone the repository.
2. Open the project folder in Unity Hub with Unity 6.
3. Open `Assets/Example Scene.unity` and press **Play**.

The first import takes a few minutes while Unity builds the `Library` folder.

## Controls

| Input | Action |
| --- | --- |
| Left click / drag | Select units and buildings |
| Right click | Move / attack / deploy |
| `Shift` + click | Add to selection |
| `G` + `0`–`9` | Assign control group |
| `0`–`9` | Recall control group |
| `S` (aircraft selected) | Stop |
| Screen edges | Scroll the camera |

## Features

- **A\* Pathfinding** on a tile grid
- **Base building** with power, refineries and ore harvesting
- **Repairing and selling** structures
- **Engineers**, **infantry**, **vehicles** and **aircraft**
- **MiniMap** showing the whole battlefield and unit positions
- **RTS camera** with edge scrolling
- **Skirmish AI** (see below)

## Skirmish AI

The AI opponent (`Assets/Scripts/RTS_AI.cs`, `Assets/Scripts/Unit AI/MediumLevel.cs`) works like this:

- **Base building is driven by need.** The AI builds power when it is running low, gets its economy going first (refinery, barracks, war factory, radar), adds refineries when money is short, and adds a second war factory when it has money to spare. It rebuilds anything that gets destroyed.
- **Harvesters are kept topped up**, about two per refinery.
- **Mixed army.** Light tanks early; heavy tanks once a Radar is up.
- **Attack waves grow over time.** Each new attack group is a little bigger than the last, up to `maxAttackLimit`.
- **Attack groups target the nearest enemy** rather than an arbitrary one. They only fall back to regroup after losing half their strength.
- **Defense groups** guard the base and respond to player units that come within `longRange` of the AI's `rangeCenter`.

You can tune all of this in the Inspector on the `RTS_AI` component: `decisionInterval`, `powerMargin`, `maxRefineries`, `harvestersPerRefinery`, `maxWarFactories`, `midAttackLimit` and `maxAttackLimit`.

## Project Layout

```
Assets/
  Scripts/            Core game code (player, AI, units, buildings, GUI, camera)
    Unit AI/          Unit logic, movement, A* pathfinding, squad AI
    Building Controllers/
    Database/         Unit and building definitions (DB_Database.cs)
  Modern/             Work-in-progress uGUI interface, minimap and NavMesh test scene
  Resources/          Runtime-loaded prefabs (units, buildings, projectiles)
  Particles/          Particle effects
  Standard Assets/    Unity Standard Assets (legacy)
```

## Upgrading from the Unity 2017 Version

The project was originally built in Unity 2017.4 and has been migrated to Unity 6:

- Legacy particles (`ParticleEmitter` / `ParticleAnimator` / `ParticleRenderer`) were converted to `ParticleSystem` (aircraft engines, SAM trails, laser, tesla coil, lightning and building dust).
- Removed components `GUIText`, `GUITexture` and `GUILayer` were stripped or replaced.
- Assets now use **Force Text** serialization, so prefabs and scenes diff cleanly in git.
- Packages are now managed through `Packages/manifest.json`.

The converted particle effects are close to the originals. Some may need small tweaks in the Particle System inspector to look exactly the same.

## Roadmap

- Switch to the new Input System
- Move the IMGUI interface to the uGUI interface in `Assets/Modern`
- Use NavMesh for unit movement
- Teach the AI to build defenses and infantry

## How To Improve Your Graphics

- Add post-processing (Built-in pipeline, or switch to URP and use a Volume)
- Model your own units and buildings
- Experiment with the GUI drawing

### Bonus Features

- Featuring the finest textures from BingAI
- Specially crafted speed trees exclusively designed for this project

## License

MIT, see [LICENSE](LICENSE).

<br>
<div align="center">
<a href="https://cursed-entertainment.itch.io/" target="_blank">
    <img src="https://github.com/CursedPrograms/cursedentertainment/raw/main/images/logos/logo-wide-grey.png"
        alt="CursedEntertainment Logo" style="width:250px;">
</a>
</div>
