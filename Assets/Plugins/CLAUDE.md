# Plugins

## Purpose
Third-party plugins and code analysis tools. Do not modify contents directly.

## Conventions
- Do not add files here manually — use Unity Package Manager for new plugins
- Roslyn DLLs are binary — do not attempt to read or modify them
- The pre-commit hook skips linting for files in `Plugins/Roslyn/`

## Contents
- `Roslyn/` — Roslyn analyzer DLLs (Microsoft.CodeAnalysis, System.Collections.Immutable, etc.)
