# Audio System

## Purpose
Sound effects and music — engine sounds, tire skids, ambient audio, and music management.

## Conventions
- Top-level: `AudioManager.cs` (namespace `R8EOX.Audio`)
- Internal components in `Internal/` (namespace `R8EOX.Audio.Internal`, `internal` access)
- Audio receives events from other systems via AudioManager — never polls
- Use AudioSource pooling for frequently played sounds

## Contents
- `AudioManager.cs` — Top-level API: play sounds, manage music
- `Internal/EngineSound.cs` — RPM-driven engine audio synthesis
- `Internal/TireSound.cs` — Skid sounds, surface-dependent audio
- `Internal/MusicPlayer.cs` — Background music, crossfades, track selection
