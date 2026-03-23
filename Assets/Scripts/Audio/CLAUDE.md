# Audio System

## Purpose
Sound effects and music — engine sounds, tire skids, collision impacts, and music management.

## Conventions
- Top-level: `AudioManager.cs` (namespace `R8EOX.Audio`)
- Internal components in `Internal/` (namespace `R8EOX.Audio.Internal`, `internal` access)
- AudioManager receives a vehicle reference via `SetTarget(GameObject)`, caches VehicleManager, polls telemetry each frame
- Internal components never call other systems directly — AudioManager routes all data
- Manual override methods (PlayEngineSound, PlayTireSound, etc.) allow direct calls from other systems

## Contents
- `AudioManager.cs` — Top-level API: auto-routes vehicle telemetry to internal components, exposes manual override methods for engine/tire/collision/music
- `Internal/EngineSound.cs` — RPM-driven engine audio: pitch from normalized RPM, volume from throttle with idle baseline, smoothed transitions
- `Internal/TireSound.cs` — Slip-based tire audio: threshold before playing, volume and pitch scale with slip, smoothed transitions
- `Internal/MusicPlayer.cs` — Background music playback: play/stop tracks, volume control
