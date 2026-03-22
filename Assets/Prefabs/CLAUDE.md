# Prefabs

## Purpose
Reusable prefab assets created from GameObjects.

## Conventions
- PascalCase names (e.g., `EnemySpawner.prefab`, `PlayerCharacter.prefab`)
- Create prefabs with `mcp__UnityMCP__manage_asset` or via Unity Editor
- Organize in subfolders by system/feature when the count grows
- Prefab variants should be named `{Base}_{Variant}` (e.g., `Enemy_Ranged.prefab`)

## Contents
- `RCBuggy.prefab` — RC buggy vehicle with VehicleManager, Rigidbody, RCInput on root; 4 RaycastWheel pivots (WheelFL/FR/RL/RR); Drivetrain and RCAirPhysics sub-objects; URP Lit materials from Assets/Materials/Vehicle/
