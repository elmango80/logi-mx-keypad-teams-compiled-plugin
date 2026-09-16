---
name: todo-rules
description: Rules for creating clear and actionable TODO comments using the repository's Git author
---

# TODO Rules

---

## 1. Author Resolution

Before writing a TODO, obtain the author's name from the Git configuration in the following order:

1. Read the repository-local `user.name`:
   ```bash
   git config --local user.name
   ```
2. If no local value is configured, read the global `user.name`:
   ```bash
   git config --global user.name
   ```
3. If neither command returns a non-empty value, ask the user which author identifier should be used. Do not guess the author's name or use a default author.

The value configured in the local repository always takes precedence over the global value.

---

## 2. Author Name Normalization

Normalize the obtained author name before including it in the TODO:

- The name must be written in kebab-case.
- Remove any special characters, such as accents, diaereses, and similar marks.
- Preserve the complete name unless the user explicitly requests a different identifier.

Examples:

| Git `user.name`   | TODO author identifier |
| ----------------- | ---------------------- |
| `María@García`    | `maria-garcia`         |
| `Ana María López` | `ana-maria-lopez`      |
| `John #Smith`     | `john-smith`           |

---

## 3. Required Format

Write all TODOs using exactly this format:

```typescript
// TODO: @author<author-name> short description
```

For example, if `user.name` is `María García`:

```typescript
// TODO: @author<john-doe> Review the analytics event
```

Formatting rules:

- Use `//` for a single-line TODO unless the language requires another comment syntax.
- Write `TODO:` immediately after the comment marker, separated by one space.
- Use the literal marker `@author` followed immediately by `<author-name>`.
- Do not use spaces, accents, punctuation marks, or special characters inside `<author-name>`.
- Separate the author marker and the description with exactly one space.
- Do not add additional tags, dates, ticket identifiers, or metadata unless explicitly requested.

---

## 4. Description Rules

The description must be brief, specific, and actionable:

- Start, whenever possible, with an infinitive verb: `Review`, `Add`, `Fix`, `Remove`, `Migrate`, `Validate`.
- Describe the pending work, not the reason why the TODO is being created.
- Identify the affected behavior, component, or data when relevant.
- Prioritize one concrete action per TODO.
- Always write the description in English, regardless of the language of the surrounding code or the user's request.
- Keep the description concise, ideally under 100 characters.
- Do not end the description with a period unless the project convention requires it.
- Do not use vague descriptions such as `Fix this`, `Pending`, or `Make changes`.
- Do not include speculative work or implementation details that are not yet known.
- Do not expose credentials, tokens, personal data, or any other sensitive information.

Examples:

```typescript
// TODO: @author<john-doe> Review the benefits analytics event
// TODO: @author<john-doe> Add validation for empty responses
// TODO: @author<john-doe> Migrate this mapper to the new domain model
```

---

## 5. When to Create a TODO

Create a TODO only when there is a specific and intentionally deferred task that cannot be completed as part of the current change.

Before adding one:

- Check whether the task can be safely completed now; if so, implement it instead of leaving a TODO.
- Search for an existing TODO covering the same work and avoid duplicating it.
- Place the TODO as close as possible to the related code.
- Include the TODO in the same change that makes the deferred work relevant.
- Do not add TODOs merely to describe existing behavior or provide general commentary.

If the required author cannot be resolved or a specific description cannot be written, ask for clarification before writing the TODO.

---

## 6. Quick Reference

### Required

- Check the local Git `user.name` before the global Git `user.name`.
- Normalize the author to kebab-case, lowercase, and without accents.
- Use `// TODO: @author<author-name> short description`.
- Write the description in English, keeping it concise, specific, and actionable.
- Avoid duplicates and place the TODO next to the relevant code.

### Prohibited

- Do not use the global author when a local author is configured.
- Do not guess the author or use a default author.
- Do not write vague, speculative, duplicated TODOs or TODOs containing sensitive information.
- Do not add a TODO when the work can be safely completed in the current change.
