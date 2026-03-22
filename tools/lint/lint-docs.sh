#!/usr/bin/env bash
# AI documentation lint rules for R8EOX project
# Sourced by tools/hooks/pre-commit — do not run directly.

is_ai_doc() {
  local file="$1"
  case "$file" in
    CLAUDE.md|*/CLAUDE.md) return 0 ;;
    .claude/agents/*.md) return 0 ;;
    .claude/skills/*/SKILL.md) return 0 ;;
    *) return 1 ;;
  esac
}

lint_ai_doc() {
  local file="$1"
  local errors=0
  local content
  content=$(git show ":${file}" 2>/dev/null) || content=$(cat "$file" 2>/dev/null) || return 0

  local line_count
  line_count=$(echo "$content" | wc -l | tr -d ' ')

  # CLAUDE.md files
  case "$file" in
    CLAUDE.md|*/CLAUDE.md)
      if [ "$line_count" -lt 3 ]; then
        echo "FAIL [doc-min-content] ${file}: must have at least 3 lines"
        errors=$((errors + 1))
      fi
      if ! echo "$content" | head -5 | grep -q '^#'; then
        echo "FAIL [doc-heading] ${file}: must start with a heading"
        errors=$((errors + 1))
      fi
      # Folder-level CLAUDE.md needs Purpose section (not root)
      if [ "$file" != "CLAUDE.md" ]; then
        if ! echo "$content" | grep -qiE '^## (Purpose|Overview|Description)'; then
          echo "FAIL [doc-purpose] ${file}: folder CLAUDE.md must have a ## Purpose section"
          errors=$((errors + 1))
        fi
      fi
      ;;
    .claude/agents/*.md|.claude/skills/*/SKILL.md)
      # Agent/skill files need YAML frontmatter
      if ! echo "$content" | head -1 | grep -q '^---$'; then
        echo "FAIL [frontmatter] ${file}: must have YAML frontmatter (---)"
        errors=$((errors + 1))
      fi
      local frontmatter
      frontmatter=$(echo "$content" | sed -n '2,/^---$/p' | sed '$d')
      if ! echo "$frontmatter" | grep -q '^name:'; then
        echo "FAIL [frontmatter-name] ${file}: frontmatter must include name:"
        errors=$((errors + 1))
      fi
      if ! echo "$frontmatter" | grep -q '^description:'; then
        echo "FAIL [frontmatter-desc] ${file}: frontmatter must include description:"
        errors=$((errors + 1))
      fi
      ;;
  esac

  return $errors
}

lint_knowledge_doc() {
  local file="$1"
  local errors=0
  local content
  content=$(git show ":${file}" 2>/dev/null) || content=$(cat "$file" 2>/dev/null) || return 0

  local line_count
  line_count=$(echo "$content" | wc -l | tr -d ' ')

  if [ "$line_count" -lt 5 ]; then
    echo "FAIL [knowledge-min] ${file}: must have at least 5 lines"
    errors=$((errors + 1))
  fi

  if ! echo "$content" | head -3 | grep -q '^#'; then
    echo "FAIL [knowledge-heading] ${file}: must start with a heading"
    errors=$((errors + 1))
  fi

  return $errors
}
