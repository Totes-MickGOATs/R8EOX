# Editor

## Purpose
Editor-only scripts excluded from runtime builds by Unity's convention.

## Conventions
- Namespace: `R8EOX.Editor` (NOT `R8EOX`)
- Must use `using UnityEditor;`
- Never create circular dependencies between editor and runtime scripts
- Place custom inspectors, editor windows, property drawers, and build tools here
- See `Builders/` subfolder for programmatic scene/prefab construction

## Contents
- `Builders/` — Editor builder scripts for constructing vehicles, terrain, and scenes programmatically
