<!-- SPECTRA:START v1.0.2 -->

# Spectra Instructions

This project uses Spectra for Spec-Driven Development(SDD). Specs live in `openspec/specs/`, change proposals in `openspec/changes/`.

## Use `$spectra-*` skills when:

- A discussion needs structure before coding → `$spectra-discuss`
- User wants to plan, propose, or design a change → `$spectra-propose`
- Tasks are ready to implement → `$spectra-apply`
- There's an in-progress change to continue → `$spectra-ingest`
- User asks about specs or how something works → `$spectra-ask`
- Implementation is done → `$spectra-archive`
- Commit only files related to a specific change → `$spectra-commit`

## Workflow

discuss? → propose → apply ⇄ ingest → archive

- `discuss` is optional — skip if requirements are clear
- Requirements change mid-work? `ingest` → resume `apply`

## Parked Changes

Changes can be parked（暫存）— temporarily moved out of `openspec/changes/`. Parked changes won't appear in `spectra list` but can be found with `spectra list --parked`. To restore: `spectra unpark <name>`. The `$spectra-apply` and `$spectra-ingest` skills handle parked changes automatically.

<!-- SPECTRA:END -->

# Project AI Instructions

These instructions are the Codex-facing project rules migrated from the previous GitHub Copilot and Claude prompt setup.

## Frontend

- Prefer Vue Composition API and `<script setup>` for Vue components.
- Use typed `defineProps` and `defineEmits` when defining component contracts.
- Prefer Pinia over Vuex for Vue 3 state management.
- Keep stores organized by domain instead of creating one large global store.
- Use `storeToRefs` when extracting reactive Pinia state.
- Use Vue Router named routes and route meta fields instead of hardcoded paths when practical.
- Lazy-load route components where it improves bundle size without making flows harder to follow.
- Handle async UI states explicitly with loading, empty, and error states.

## Backend

- Use dependency injection consistently. Prefer scoped services for request-specific work and singleton services only for stateless behavior.
- Keep database reads efficient. Use eager loading where it avoids N+1 queries, and `AsNoTracking()` for read-only Entity Framework queries.
- Use migrations for schema changes and keep migration names descriptive.
- Apply centralized exception handling so API errors are consistent.
- Validate request models at API boundaries.

## Database

- Use parameterized SQL for all database access.
- Add indexes based on actual query patterns.
- Keep complex database logic explicit and documented when stored procedures are used.

## Git

- Use conventional commits when committing changes.
- Keep commits focused on a single logical change.
- Commit messages should explain why the change was made, not only what changed.
- When asked to add and commit, use this commit body format:

```text
Issue:

Description:
```
