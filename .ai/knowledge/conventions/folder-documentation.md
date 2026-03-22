# Folder Documentation (CLAUDE.md) Conventions

## Every Non-Dot Folder Gets a CLAUDE.md

When you create files in a folder that doesn't have a `CLAUDE.md`, create one. When you add or significantly change files in a folder that has one, update it.

## Required Structure

```markdown
# {Folder Name}

## Purpose
{One-line description of what this folder contains}

## Conventions
- {Folder-specific rules — namespace, access modifiers, naming, etc.}

## Contents
- `FileName.cs` — {Brief description of what it does}
- `SubFolder/` — {Brief description of subfolder purpose}
```

## Rules

1. **Must start with a `#` heading** matching the folder name
2. **Must have a `## Purpose` section** (except root CLAUDE.md)
3. **Must have at least 3 lines** of content
4. **Contents section must list actual files** — not placeholders or "TBD"
5. **Update when files change** — add new files, remove deleted ones, update descriptions

## Pre-Commit Enforcement

The pre-commit hook validates:
- Heading exists (first 5 lines contain `#`)
- Minimum 3 lines of content
- `## Purpose` section exists (for non-root CLAUDE.md files)

## When to Create vs Update

- **Create**: First file added to a folder that has no CLAUDE.md
- **Update**: Any time you add, remove, or significantly modify files in the folder
- **Skip**: Don't update for minor edits (typo fixes, small refactors within existing files)
