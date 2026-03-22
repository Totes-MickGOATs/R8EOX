# Subagent Workflow Checklist

Every subagent must follow this process. The orchestrator injects this into your prompt.

## Before You Start

- [ ] Read the pre-loaded context in your prompt — do NOT re-read CLAUDE.md or .ai/knowledge/ files if already provided
- [ ] Confirm you have exact file paths for everything you need to create or modify
- [ ] If a file you need to modify wasn't provided, read it first

## While Working

- [ ] One class per file, class name matches file name
- [ ] 300 line maximum per .cs file
- [ ] Use `[SerializeField] private` — never public fields
- [ ] Top-level classes: `namespace R8EOX.{System}`, `public class`
- [ ] Internal classes: `namespace R8EOX.{System}.Internal`, `internal class`
- [ ] No banned patterns: `SendMessage`, `FindObjectOfType`, `.Instance`, legacy `Input.GetKey/GetAxis`
- [ ] After creating/modifying C# files, check `read_console` for compilation errors

## After Each File

- [ ] Stage the file: the auto-stage hook handles this, but verify if needed

## After All Work Is Done

### 1. Update Documentation
- [ ] If you created files in a folder with a CLAUDE.md, update its Contents section
- [ ] If you created files in a folder WITHOUT a CLAUDE.md, create one:
  ```markdown
  # {Folder Name}

  ## Purpose
  {What this folder contains}

  ## Conventions
  - {Folder-specific rules}

  ## Contents
  - `File.cs` — {What it does}
  ```
- [ ] If you created a NEW directory, it needs a CLAUDE.md

### 2. Commit Your Work
- [ ] Stage all your changes: `git add {files}`
- [ ] Commit with a descriptive message: `git commit --no-verify -m "feat: {what you did}"`
- [ ] If the commit fails, report the error — do not silently skip

### 3. Report Results
- [ ] List every file you created or modified (full paths)
- [ ] Note any compilation errors from read_console
- [ ] Flag anything that needs follow-up by another agent or the orchestrator
