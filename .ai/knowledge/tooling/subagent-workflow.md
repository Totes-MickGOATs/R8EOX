# Subagent Workflow Checklist

Every subagent must follow this process. The orchestrator injects this into your prompt.

## ⛔ RULE #1: COMMIT EVERY FILE IMMEDIATELY — YOU ARE BLOCKED UNTIL YOU DO

> **This is the most important rule in the entire project. It overrides everything else.**

**You MUST `git commit` IMMEDIATELY after writing or modifying EACH file.** You are **BLOCKED from starting work on the next file** until the commit for the current file succeeds. No batching. No deferring. No exceptions.

### The Workflow (repeat for EVERY file you touch)

```bash
# 1. Write or modify ONE file
# 2. Stage ONLY that file by exact path
git add path/to/ExactFile.cs

# 3. Commit it NOW
git commit -m "feat: description of this specific file change"

# 4. STOP — verify commit succeeded
# 5. Only NOW may you proceed to the next file
```

### BANNED — Violations Will Cause Lost Work

- `git add -A` / `git add .` / `git add --all` — **BANNED** (stages other agents' work)
- Batching multiple files into one commit — **BANNED**
- Deferring commits to "after all work is done" — **BANNED**
- Silently skipping a failed commit — **BANNED** (report the error)
- `--no-verify` — **BANNED** (pre-commit hook must validate)
- Proceeding to the next file before the current commit succeeds — **BANNED**

---

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

## After All Files Are Written and Committed

By this point, you should have already committed each file individually as you created/modified it. If somehow you have uncommitted files, commit them NOW one at a time before proceeding.

### 1. Update Documentation
- [ ] If you created files in a folder with a CLAUDE.md, update its Contents section
- [ ] **COMMIT the CLAUDE.md update immediately**: `git add path/to/CLAUDE.md && git commit -m "docs: update CLAUDE.md"`
- [ ] If you created files in a folder WITHOUT a CLAUDE.md, create one and **commit it immediately**:
  ```markdown
  # {Folder Name}

  ## Purpose
  {What this folder contains}

  ## Conventions
  - {Folder-specific rules}

  ## Contents
  - `File.cs` — {What it does}
  ```
- [ ] If you created a NEW directory, it needs a CLAUDE.md — **commit it immediately**

### 2. Verify All Commits Happened
- [ ] Run `git status` — there should be NO uncommitted changes from your work
- [ ] If anything is uncommitted, commit it NOW (one file at a time)
- [ ] Run `git log --oneline -5` to verify your commit history

### 3. Report Results
- [ ] List every file you created or modified (full paths)
- [ ] List every commit hash (run `git log --oneline -N` where N = number of files you touched)
- [ ] Note any compilation errors from read_console
- [ ] Flag anything that needs follow-up by another agent or the orchestrator
