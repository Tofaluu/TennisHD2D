# TennisHD2D

HD-2D tennis game (Octopath Traveler look: pixel-art sprites in a lit 3D diorama with
tilt-shift, bloom, color grading). Workflow is AI-first for the prototype; AI-generated
placeholder art gets replaced by hand-made assets later.

## Stack
- Unity 6.6 (6000.6.5f1), URP 17.6, **3D Universal Renderer** (not the 2D Renderer)
- New Input System (`Assets/InputSystem_Actions.inputactions`)
- Cinemachine 6.6 for the broadcast-style camera
- Unity AI Assistant (beta, 14-day trial started 2026-10-08) — provides the official MCP bridge
- Git + LFS, remote: https://github.com/Tofaluu/TennisHD2D (GitHub LFS free tier is 1 GB — keep raw art sources out)

## Unity MCP bridge
- Official Unity relay, registered for this folder only (local scope):
  `claude mcp add --scope local unity-mcp -- "%USERPROFILE%\.unity\relay\relay_win.exe" --mcp`
- Unity must be open; first connection needs **Accept** in Edit > Project Settings > AI > Unity MCP.
- Third-party Unity-MCP (IvanMurzak) was tried and removed — don't run two bridges.
- Don't make the project depend on the bridge: everything should also work via plain code and editor menu commands.

## Conventions
- Gameplay logic lives in C# scripts; keep scenes and prefabs thin.
- Build scenes, prefabs and materials through editor scripts (`Tennis > ...` menu items)
  rather than hand-editing Unity YAML.
- Shaders as hand-written HLSL (text, diffable) rather than Shader Graph.
- Ball physics is custom (gravity, drag, Magnus/spin, bounce model, fixed timestep) — no stock rigidbodies.
- Court at regulation size: 23.77 m x 10.97 m (doubles), net 0.914 m at center.
- Namespaces: `TennisHD2D.<Area>` (e.g. `TennisHD2D.Rendering`, `TennisHD2D.Editor`).

## HD-2D rendering notes
- Sprites: alpha-clipped quads / sprite renderers with a lit shader that responds to 3D lights
  and casts shadows. `Sprite-Lit-Default` does NOT work here (2D Renderer / Light2D only).
- Billboarding rotates around Y with an optional partial tilt (`Assets/Scripts/Rendering/Billboard.cs`);
  sprite pivots go at the feet.
- Pixel art: point filtering, no mipmaps, consistent pixels-per-unit, avoid sub-pixel shimmer.
- Tilt-shift needs a custom full-screen pass (URP DoF is distance-based, not screen-band).

## Machines / quality
- Quality levels: `PC` (gaming PC) and `Laptop`, each with its own URP asset in `Assets/Settings/`.
- Per-machine choice via `Tennis > Machine Quality Profile` (EditorPrefs). `QualitySettings.asset`
  always commits `PC` as the active level; the switcher swaps it back on save.

## Roadmap
1. Setup — packages, folders, quality tiers, this file (done)
2. Graybox — court, capsule players, ball physics + hitting that feels good (next)
3. HD-2D look — sprite shader, lighting, tilt-shift, bloom, grading
4. Game loop — serve, scoring, AI opponent, basic UI
5. Art pass — AI sprites (PixelLab / Retro Diffusion / Unity Sprite Generator / Mixamo renders), later hand-made

## Working with the user
- The user tests in Unity (Play, menu commands) and reports back with screenshots / console output.
- Commit in small steps and push to GitHub.
