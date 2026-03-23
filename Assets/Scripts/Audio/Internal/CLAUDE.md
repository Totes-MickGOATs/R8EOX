# Audio Internal Components

## Purpose
Internal implementation for the Audio system. Only `AudioManager` should reference these.

## Conventions
- Namespace: `R8EOX.Audio.Internal`
- Access: `internal class` (enforced by pre-commit hook)
- All are MonoBehaviours with AudioSource references
- Components receive data via explicit method calls from AudioManager, never poll external systems

## Contents
- `EngineSound.cs` — RPM-driven engine audio: pitch mapped from normalized RPM (minPitch to maxPitch), volume interpolated between idleVolume and fullVolume based on throttle, smoothed transitions via MoveTowards, auto-starts loop on first update
- `TireSound.cs` — Slip-based tire audio: silent below slipThreshold (0.15 default), volume and pitch scale with effective slip above threshold, auto-starts/stops loop, SetSurface placeholder for future clip swapping
- `MusicPlayer.cs` — Background music playback: play/stop tracks, volume control (DO NOT MODIFY — already functional)
