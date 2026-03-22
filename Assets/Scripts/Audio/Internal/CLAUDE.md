# Audio Internal Components

## Purpose
Internal implementation for the Audio system. Only `AudioManager` should reference these.

## Conventions
- Namespace: `R8EOX.Audio.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- All are MonoBehaviours with AudioSource references

## Contents
- `EngineSound.cs` — RPM-driven engine audio: adjusts AudioSource pitch based on normalized RPM
- `TireSound.cs` — Slip-based tire audio: adjusts volume by slip amount, switches clips by surface type
- `MusicPlayer.cs` — Background music playback: play/stop tracks, volume control
