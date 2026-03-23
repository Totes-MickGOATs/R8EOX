# Shaders

## Purpose
Custom shaders for the game's UI and visual effects.

## Contents
| File | Shader Path | Purpose |
|------|-------------|---------|
| AtmosphereBackground.shader | R8EOX/AtmosphereBackground | Animated FBM noise gradient for Options overlay background. Deep purple-blue tones, 3-octave noise, radial vignette. Canvas-compatible. |
| Vignette.shader | R8EOX/Vignette | Radial edge darkening overlay. Canvas-compatible. |

## Conventions
- All shaders use HLSL (not CG) for URP compatibility
- UI shaders include standard stencil properties for Canvas masking
- Shader paths prefixed with `R8EOX/`
