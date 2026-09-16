---
name: select-commits
description: Reusable skill for selecting commits from a source branch. Shows recent commits in a numbered list and lets the user pick which ones to use. Used by cherry-pick skills.
---

# Select Commits

Fetches a source branch and shows recent commits for the user to select. Does **not** checkout the branch — reads from `origin/{source_branch}` to avoid switching branches.

> **UX**: Follow the conventions in the `interactive-prompts` skill for all user prompts.

## Parameters

- **source_branch** (optional): The branch to read commits from. Defaults to the greatest `release/X.Y.0` branch.
- **commits** (optional): Pre-provided comma-separated commit hashes. If provided, skip discovery and prompting.

## Workflow

### 1. Resolve Source Branch

If `source_branch` is not provided, discover the greatest `release/X.Y.0` branch:
```bash
git fetch --all
git branch -r | grep -E 'origin/release/[0-9]+\.[0-9]+\.0$' | sed 's|origin/release/||' | sort -V | tail -1
```
Use `release/{result}` as `source_branch`. If no release branch found, fall back to `main`.

Fetch the source branch:
```bash
git fetch origin {source_branch}
```

> **No `git checkout`** — read from `origin/{source_branch}` directly to avoid switching branches.

### 2. Show Recent Commits

If commits not provided, show last 20 commits:
```bash
git log --oneline --format="%h %s (%ad)" --date=short -20 origin/{source_branch}
```

Display as a **numbered markdown table**:

| # | Hash | Description | Date |
|---|------|-------------|------|
| 1 | abc1234 | Fix login validation | 2024-01-15 |
| 2 | def5678 | Update dependencies | 2024-01-14 |
| ... | ... | ... | ... |

### 3. Prompt User (Hybrid Format)

Use the **hybrid format** from `interactive-prompts`:

1. The full list is already shown as a table (step 2).
2. Use `ask_user_question` with multi-select enabled to offer the **4 most recent PATCH commits** as quick-pick options.
3. Each option label: `#N — <short_hash>`. Each option description: the full commit message and date.
4. The question text must tell the user they can also type a custom selection (e.g., `1,3-5,7`).

### 4. Parse Selection

| Input | Interpretation |
|-------|----------------|
| `1,3,5` | Commits 1, 3, and 5 |
| `1-5` | Commits 1 through 5 |
| `1,3-5,7` | Commits 1, 3, 4, 5, and 7 |
| Interactive click | The commit(s) from the clicked option(s) |

Convert selected numbers to commit hashes.

### 5. Return

Return the list of selected commit hashes.
