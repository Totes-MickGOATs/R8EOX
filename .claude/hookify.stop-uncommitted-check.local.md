---
name: warn-stop-uncommitted-check
enabled: true
event: stop
action: warn
pattern: .*
---

**Before finishing, check for uncommitted work.**

Run `git status` and verify there are no uncommitted `.cs` file changes. If there are:

1. `git add <specific files>` (never `git add -A` or `git add .`)
2. `git commit -m "feat/fix/test: {description}"`
3. Then you may stop.
