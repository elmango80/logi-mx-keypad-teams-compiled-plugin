---
name: create-pull-request
description: 'Generate comprehensive PR descriptions with JIRA integration and GitHub CLI support. Use when user asks to create a pull request, generate a PR, open a PR, or mentions "/pr", "/pull-request", "genera la PR", "crea la PR", "abre la PR". Supports: (1) Extracting JIRA ticket from branch name (RD-XXXX), (2) Analyzing git diff to generate detailed descriptions, (3) Creating PR via GitHub CLI with proper labels and assignees, (4) Conversational Spanish descriptions with meme GIFs'
allowed-tools: Bash
---

# PR GENERATION INSTRUCTIONS

## ROLE & OBJECTIVE

Expert PR Documentation Assistant - Generate comprehensive, professional PR descriptions for developers.

**WRITING STYLE**: Conversational, natural tone. Write as if explaining to a colleague.

## VALIDATION RULES

- ✅ Branch format: `(feat|feature|bug|fix)/[RD-XXXX-]description`
- `feat/feature` → **[MINOR]** | `bug/fix` → **[PATCH]**
- **Extract RD-XXXX** from branch name for title and JIRA link (ONLY if present)

## BEHAVIOR INSTRUCTIONS

1. **GET branch name**: Use `git branch --show-current` if not provided
2. **GET changes efficiently**:
   - `git diff --name-only main <branch>` for file list
   - `git diff --stat main <branch>` for overview
   - `git diff main <branch> -- <file>` only if needed
3. **Analyze modified files**: Use real file list, not assumptions
4. **READ strategically**:
   - read_file for key files only
   - grep_search for specific patterns
   - Focus on core changes
5. **CHECK for JIRA**: Extract RD-XXXX from branch name if present
6. **Generate with specifics**: Use actual changes, not generic content
7. **Use real code**: Never invent examples
8. **Follow template exactly**: No additional sections
9. **Conversational tone**: Natural, friendly explanations

## CONTENT STRUCTURE

**PR Title:** `[MINOR|PATCH] [RD-XXXX: ][Title]`

**PR Description:**

````markdown
## 📋 Descripción

[JIRA link if RD-XXXX in branch name:]
🔗 <a href="https://retail-commercial.atlassian.net/browse/RD-XXXX" target="_blank" rel="noopener noreferrer">RD-XXXX</a>

[Context + solution summary in conversational tone]

## 🔄 Cambios

- [Change - file/component]

## 🛠️ Detalles de Implementación

<details>
  <summary>Ver más información</summary>

[Natural, flowing technical explanation with conversational tone]

```[language]
// REAL CODE from git diff with inline explanation
[REAL CODE - NEVER invent]
```

[Continue explanation naturally, conclude with how it works]

</details>

## 🧪 Cómo Realizar las Pruebas

1. [Step] → Expected: [result]

[Include if user explicitly requests:]

## 💭 Consideraciones Adicionales

[Future developments, technical considerations, or additional notes - ONLY when requested]

## 😄 Descripción Memetizada

[Humorous description, brief and witty]

![GIF](https://media.giphy.com/media/[ID]/giphy.gif)
````

## WORKFLOW CLARIFICATION

**Workflow:**

1. Generate PR description
2. User review and adjustments
3. PR creation only when explicitly requested

**Actions:**

- Generate descriptions
- Wait for user instructions
- Create PR only when asked

## PR CREATION WITH GITHUB CLI

1. **Create temp file**: use the `write_to_file` tool to create a `pr_description.md` in project root
2. **Get available labels** (MANDATORY):
   ```bash
   gh label list --limit 50
   ```
3. **Determine labels to apply**:
   - **ALWAYS include**: `patch` (for fix branches) or `minor` (for feat branches)
   - **Add 1-2 relevant labels** from the list obtained in step 2 (e.g., `refactor`, `tests`, `documentation`, `bug`, `enhancement`)
4. **Create PR with labels**:
   ```bash
   gh pr create \
     --title "[TITLE]" \
     --body-file pr_description.md \
     --assignee @me \
     --label "patch,refactor"
   ```
5. **Clean up temporary file**:
   ```bash
   rm pr_description.md
   ```

**Guidelines:**

- **Title**: `[MINOR|PATCH] RD-XXXX: Title` (include JIRA ticket if present in branch)
- **Assignee**: Always `@me`
- **Labels**: MANDATORY - Always include `patch` or `minor` + 1-2 relevant labels from repo. NEVER skip labels.
