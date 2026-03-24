# Subagent Workflow Checklist

Every subagent must follow this process. The orchestrator injects this into your prompt.

## ⛔ RULE #1: COMMIT EVERY FILE IMMEDIATELY — YOU ARE BLOCKED UNTIL YOU DO

> **This is the most important rule in the entire project. It overrides everything else.**

**You MUST `git commit` IMMEDIATELY after writing or modifying EACH file.** You are **BLOCKED from starting work on the next file** until the commit for the current file succeeds. No batching. No deferring. No exceptions.

### The Workflow (repeat for EVERY file you touch)

```bash
# 1. Write or modify ONE file
# 2. Check for companion .meta files (Unity auto-generates these!)
git status
# 3. Stage the file AND its .meta (if new file/folder, .meta WILL exist)
git add path/to/ExactFile.cs
git add path/to/ExactFile.cs.meta   # ← REQUIRED for new files
# For new folders, also: git add path/to/NewFolder.meta

# 4. Commit them together NOW
git commit -m "feat: description of this specific file change"

# 5. STOP — verify commit succeeded
# 6. Only NOW may you proceed to the next file
```

### ⚠️ UNITY .meta FILES — THE #1 MISSED ITEM

Unity generates a `.meta` file for **every** new file and folder. If you create `Foo.cs`, Unity creates `Foo.cs.meta`. If you create a new folder `Bar/`, Unity creates `Bar.meta`. **These .meta files are required** — without them, Unity loses track of asset GUIDs and references break (magenta materials, missing scripts, etc.).

**After writing any new file, ALWAYS run `git status` and stage the companion `.meta` file in the same commit.** A `.cs` commit without its `.meta` is an incomplete commit.

### BANNED — Violations Will Cause Lost Work

- `git add -A` / `git add .` / `git add --all` — **BANNED** (stages other agents' work)
- Batching multiple files into one commit — **BANNED** (exception: a file + its companion `.meta` = one commit)
- Deferring commits to "after all work is done" — **BANNED**
- Silently skipping a failed commit — **BANNED** (report the error)
- `--no-verify` — **BANNED** (pre-commit hook must validate)
- Proceeding to the next file before the current commit succeeds — **BANNED**
- Committing a new file without its `.meta` — **BANNED** (causes broken references)

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
- [ ] **SAVE after every MCP modification** — see below

## ⛔ RULE #2: SAVE AFTER EVERY MCP MODIFICATION

Every MCP call that modifies a scene, GameObject, component, or asset MUST be immediately followed by a save. Unsaved changes are silently lost when another agent loads a scene or enters play mode.

**Preferred pattern** — use `batch_execute` to make modification + save atomic:
```json
{
  "commands": [
    { "tool": "manage_gameobject", "params": { "action": "create", "name": "MyObject" } },
    { "tool": "execute_menu_item", "params": { "item_path": "R8EOX/Save All" } }
  ]
}
```

- Always include `execute_menu_item("R8EOX/Save All")` as the **last command** in any `batch_execute` that modifies editor state
- Before `manage_scene(action="load")` or `manage_editor(action="play")`, save first
- This prevents other agents from interleaving and wiping your unsaved changes

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
