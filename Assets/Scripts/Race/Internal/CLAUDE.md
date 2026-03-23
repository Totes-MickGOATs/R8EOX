# Race Internal Components

## Purpose
Internal implementation classes for the Race system. Only `RaceManager` should reference these.

## Conventions
- Namespace: `R8EOX.Race.Internal`
- Access: `internal class` (enforced by pre-commit hook)

## Contents
- `RaceState.cs` — FSM managing countdown timer and phase transitions; exposes CurrentPhase and CountdownRemaining; ticked by RaceManager
- `Standings.cs` — Tracks lap counts, checkpoint progress, and position order per vehicle; lap completion requires crossing checkpoint 0 after the final checkpoint (prevents false increment on race start); RecalculatePositions sorts by laps then checkpoint index then distance-along-track
- `RaceTimer.cs` — Tracks elapsed race time, per-vehicle current lap time, last completed lap time, and best lap time; lap start times reset on each lap completion
