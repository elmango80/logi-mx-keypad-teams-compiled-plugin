---
name: create-releases
description: Create multiple dc-product releases at once from release branch PRs. Use when user requests to create dc-product releases, publish releases, or mentions "create dc-product releases" (e.g., "create dc-product releases for 8.4,8.5,9.1", "publish dc-product releases"). Discovers latest release PRs, confirms versions with user, and creates GitHub releases.
---

# Create DC Product Releases

Creates dc-product releases for multiple versions by discovering the latest `release/X.Y.Z` PR for each version and creating a GitHub release.

> **On failure**: If release creation fails and leaves an orphaned `dc-product-release/` branch, invoke `@release-failure-recovery` for cleanup.

## Tag Pattern

`{X.Y.Z}-dc-product` (e.g., `8.4.5-dc-product`)

## Workflow

### 0. Select and Dispatch Versions

| Input             | Action                                                                                                                                                                                  |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Single version    | Subagent invocation — skip to Step 1                                                                                                                                                    |
| Multiple versions | Dispatch below                                                                                                                                                                          |
| No versions       | Discover via `@select-versions` (branch_type: `maintenance`, default_strategy: `latest-3-majors`, prompt: `true`). Version list includes branch mappings (`maintenance/{VN}` or `main`) |

**Dispatch**: For each version, `run_subagent` → `@create-dc-product-releases {VN}`. Subagents run Step 1 independently. Collect results for the summary (Step 4).

---

### 1. Determine and Display Target Branch

Determine the target branch for this version:

```bash
git fetch origin
git branch -r | grep -E "^\\s*origin/maintenance/${VN}$"
```

- If `maintenance/{VN}` exists → `TARGET_BRANCH = maintenance/{VN}`
- Otherwise → `TARGET_BRANCH = main`

Display the version-to-branch mapping:

```
Target branch:
  - {VN} → {TARGET_BRANCH}
```

### 2. Discover Release PR and Confirm

Find the latest PR from a `release/*` branch into the target branch for this version (VN):

```bash
gh pr list --base "{TARGET_BRANCH}" --state merged --json number,url,headRefName,title,mergedAt --jq '[.[] | select(.headRefName | startswith("release/"))] | sort_by(.mergedAt) | reverse | .[0]'
```

Extract `X.Y.Z` from the head branch name `release/X.Y.Z`.

- If no release PR found → warn and return: `⚠️ Version {VN}: no merged release PR found into {TARGET_BRANCH}`

**Confirm with user**:

> **UX**: Follow the conventions in the `interactive-prompts` skill

```
Found release PR:
  - {VN}: release/{X.Y.Z} → {TARGET_BRANCH} (PR #{number})

New release tag to create:
  - {X.Y.Z}-dc-product
```

Use `ask_user_question`:

- **Confirm** — "Proceed with creating the release"
- **Abort** — "Cancel, do not create this release"

If user aborts → stop.

### 3. Create Release

For version (VN) with a confirmed release version `X.Y.Z`:

#### 3.1 Find Previous Release Tag

```bash
gh release list --limit 200 --json tagName,isPrerelease
```

Filter tags by these criteria (all must match):

- Tag matches `{X}.{Y}.*-dc-product` (same major.minor as the release being created)
- Not a pre-release
- No extra suffix after `-dc-product` (e.g. reject `-rc`, `-hotfix`)

Sort the matches by version descending. Pick the latest as `PREVIOUS_TAG`.

**Fallback** — trigger this when the filtered list above is empty (i.e. `Z == 0`, first release in this minor):

1. Compute `FALLBACK_TAG = {X}.{Y-1}.0-dc-product`
2. Check whether `FALLBACK_TAG` exists anywhere in the full release list
3. If yes → use it as `PREVIOUS_TAG`
4. If no (or if `Y == 0`) → `PREVIOUS_TAG` is empty

#### 3.2 Prepare Release Target

- If `TARGET_BRANCH` is `main`: CI forbids creating releases directly from `main`, so create a temporary branch first:
  ```bash
  git fetch origin main
  git push origin origin/main:refs/heads/dc-product-release/{X.Y.Z}
  ```
  Use `dc-product-release/{X.Y.Z}` as the effective `RELEASE_TARGET` for this version.
- Otherwise: use `TARGET_BRANCH` as `RELEASE_TARGET` directly.

#### 3.3 Build Changelog

Since release PRs are squash-merged into the target branch, GitHub's `--generate-notes` only sees the single squash commit and produces a meaningless changelog. Build the release notes manually from PR metadata instead.

##### 3.3.1 Collect PRs merged into the release branch

List all PRs that were merged into `release/{X.Y.Z}`:

```bash
gh pr list --base "release/{X.Y.Z}" --state merged --json title,number,headRefName,author --jq '[.[] | {title, number, head: .headRefName, author: .author.login}]'
```

##### 3.3.2 Expand integration PRs

For each PR whose `headRefName` starts with `integration/`, the squash commit hides the individual feature PRs. Replace the integration PR entry with the feature PRs that were merged into it:

```bash
gh pr list --base "{headRefName}" --state merged --json title,number,author --jq '[.[] | {title, number, author: .author.login}]'
```

##### 3.3.3 Filter noise

Exclude PRs whose titles match any of these patterns (branch-initialization and automated bookkeeping commits):

- `chore: initialize *`
- `chore: update deps *` (from `@update-release-dependencies`)

Keep dependency-update PRs if they contain meaningful version bumps — use judgement.

##### 3.3.4 Format notes

Build a markdown body following GitHub's auto-generated style:

```markdown
## What's Changed

- {PR_TITLE} by @{author} in #{number}
- {PR_TITLE} by @{author} in #{number}
  …

**Full Changelog**: https://github.com/{OWNER}/{REPO}/compare/{PREVIOUS_TAG}...{X.Y.Z}-dc-product
```

- If `PREVIOUS_TAG` is empty → omit the **Full Changelog** link.
- Sort entries by PR number ascending (merge order).

#### 3.4 Create Release

```bash
gh release create "{X.Y.Z}-dc-product" \
  --target "{RELEASE_TARGET}" \
  --notes "{FORMATTED_NOTES}"
```

- If release creation fails → log error, still proceed to cleanup below

#### 3.5 Cleanup Temporary Branch

If a temporary `dc-product-release/{X.Y.Z}` branch was created in step 3.2, **always** delete it — whether the release succeeded or failed:

```bash
git push origin --delete dc-product-release/{X.Y.Z}
```

### 4. Summary

> **Note**: This step is assembled by the main session after all subagents complete.

Determine the project type label from the repository name (e.g., `BASE` if repo ends with `-base`, `MC` if `-mc`). Use `@project-detection` if available.

Output the summary as a ready-to-copy markup text block (no header), one entry per version:

```
*Onboarding {PROJECT} (PATCH {X.Y.Z})*
• Target branch: {TARGET_BRANCH}
• Release tag: {X.Y.Z}-dc-product
• Release: {RELEASE_URL}
```

## Error Handling

- No release PR found → warn, skip version
- Target branch missing → warn, skip version
- Release creation fails → log error, continue with remaining versions
- No versions selected → abort early
