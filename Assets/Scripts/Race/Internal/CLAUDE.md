# Race Internal Components

## Purpose
Internal implementation classes for the Race system. Only `RaceManager` should reference these.

## Conventions
- Namespace: `R8EOX.Race.Internal`
- Access: `internal class` (enforced by pre-commit hook)

## Contents
- `RaceState.cs` — FSM with RacePhase enum (PreRace/Countdown/Racing/Finished): manages countdown timer and phase transitions
- `Standings.cs` — Tracks lap counts, last checkpoint, and position order per vehicle using Dictionaries
- `RaceTimer.cs` — Tracks elapsed race time, per-vehicle lap times, and best lap times
