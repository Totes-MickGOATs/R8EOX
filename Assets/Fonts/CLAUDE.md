# Fonts

## Purpose
Font files for the game UI. All text uses TextMeshPro with SDF font assets.

## Contents
| File | Font | Usage |
|------|------|-------|
| Rajdhani-Bold.ttf | Rajdhani Bold | Titles, headers (H1, H2) |
| Rajdhani-SemiBold.ttf | Rajdhani SemiBold | Body text, labels, buttons |
| Rajdhani-Regular.ttf | Rajdhani Regular | Section headers, secondary text |
| SourceCodePro-Regular.ttf | Source Code Pro | Numeric readouts, monospace values |
| *-SDF.asset | TMP SDF assets | Auto-generated from TTF files by `R8EOX > Build Font Assets` |

## Conventions
- All fonts are OFL licensed (Google Fonts)
- SDF assets created via FontAssetBuilder (sampling 48px, padding 5, SDFAA)
- Fonts wired into MenuThemeConfig: titleFont=Bold, bodyFont=SemiBold, monoFont=SourceCode
