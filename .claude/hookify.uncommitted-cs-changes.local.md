---
name: warn-uncommitted-cs-changes
enabled: false
event: file
conditions:
  - field: file_path
    operator: regex_match
    pattern: \.cs$
---

**Uncommitted .cs file change detected.**

You just modified a C# file. You MUST `git add` your changed files by name and `git commit` before finishing or moving to the next task. Do NOT accumulate uncommitted .cs changes across multiple edits.
