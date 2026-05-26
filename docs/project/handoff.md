# TankerMade Handoff

Last updated: 2026-05-26

## Current State

- Current roadmap phase: Phase D — Module Inventory & Kits.
- Phase A is complete.
- Phase B is complete.
- Phase C is complete.
- Phase D is ready to start.
- Latest verified slice: Phase C completion, including non-destructive editing cleanup.
- Latest pushed commit before current uncommitted Phase C work: `52f3020 Add project step checklist progress`.

## Completed Recently

- Crafting project detail can display linked pattern pieces and steps as a checklist.
- Step completion is persisted per project through `CraftingProjectStepProgress`.
- Per-step timers support play, pause, manual adjustment, and reset; project elapsed time is the sum of step timers.
- Project completion is derived from checked linked pattern steps when linked steps exist.
- Project detail has a piece selector, per-piece completion, and per-piece time.
- Crafting projects can be archived and reopened without deleting project data.
- Linked pattern edits are non-destructive: rename/edit/reorder remain available, while destructive deletes and project pattern changes are blocked once project work exists.
- Client error delivery was cleaned up for blocked project updates and linked pattern delete attempts.
- Crafting remains the reference/template module, not the final production catch-all for knitting, crochet, sewing, or other niches.

## Next Phase D Work

- Craft module inventory: yarn, tools, notions, lots, purchase history, and sale price handling.
- 3D printing module inventory: materials, spools, printer/tooling needs, and module-specific purchase history.
- Module-owned filtering and reference data.
- Purchase history per source; sale price handling.
- Module-owned project/inventory linking.
- Module-owned kit/grouping behavior.
- Module-owned kit/grouping to project flows.

## Product Input Backlog

- `Scratch/TankerInput.md` contains ignored scratch notes for future non-technical product discussion, including pattern-version migration and possible UI theme feedback.

## Working Rules

- Treat `docs/project/roadmap.md` as the roadmap source of truth.
- Keep Core craft-agnostic; module-specific workflows belong in modules.
- Do not repeatedly try full solution/server builds in the Codex sandbox. Ask the user to run local build/tests and provide results.
- Do not commit local databases, Finder metadata, build outputs, or `Scratch/` content.

## Verification From Last Slice

The user reported:

- Build passed.
- Smoke test passed.
- Phase C slices were verified locally by the user rather than through Codex sandbox builds.
