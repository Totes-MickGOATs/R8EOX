#!/usr/bin/env bash
# Install git hooks for R8EOX project
# Run once after cloning: bash tools/install-hooks.sh

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

HOOK_SRC="${PROJECT_ROOT}/tools/hooks/pre-commit"
HOOK_DST="${PROJECT_ROOT}/.git/hooks/pre-commit"

if [ -f "$HOOK_DST" ] && [ ! -L "$HOOK_DST" ]; then
  echo "Backing up existing pre-commit hook to pre-commit.bak"
  mv "$HOOK_DST" "${HOOK_DST}.bak"
fi

ln -sf "$HOOK_SRC" "$HOOK_DST"
chmod +x "$HOOK_SRC"
echo "Git hooks installed successfully."
