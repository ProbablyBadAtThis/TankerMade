# TankerMade Handoff

Last updated: 2026-05-27

## Current State

- Current roadmap phase: Phase F — Module Platform V1.
- Phase A is complete.
- Phase B is complete.
- Phase C is complete.
- Phase D is complete: module-owned inventory, reference data, project/inventory linking, kit/grouping backend, and kit-to-project backend flows are verified.
- Latest verified slice: Phase E module-owned add/new option reference flows.
- Latest pushed commit: `b5041e7 Implement Phase E reference data extension points and module option flows`.
- Latest unverified slice: none.

## Completed Recently

- Crafting project detail can display linked pattern pieces and steps as a checklist.
- Step completion is persisted per project through `CraftingProjectStepProgress`.
- Per-step timers support play, pause, manual adjustment, and reset; project elapsed time is the sum of step timers.
- Project completion is derived from checked linked pattern steps when linked steps exist.
- Project detail has a piece selector, per-piece completion, and per-piece time.
- Crafting projects can be archived and reopened without deleting project data.
- Linked pattern edits are non-destructive: rename/edit/reorder remain available, while destructive deletes and project pattern changes are blocked once project work exists.
- Client error delivery was cleaned up for blocked project updates and linked pattern delete attempts.
- Crafting project/inventory linking backend is in place for yarn, tools, and notions.
- Crafting project detail can link and remove yarn, tool, and notion supplies from module inventory.
- Crafting kit/grouping backend is in place with user-scoped kits, ordered kit pieces, and text-based kit supplies.
- Crafting kit pieces can create one linked project each through Crafting-owned backend/API flows.
- Kit-created projects carry optional `KitId` and `KitPieceId` backlinks; service code clears those links if a kit or kit piece is deleted.
- Crafting remains the reference/template module, not the final production catch-all for knitting, crochet, sewing, or other niches.
- Core neutral reference categories are now available through module extension-point lookup flows (theme/color/source/brand).
- Module-owned reference category boundaries are now explicitly enforced in module services.
- Crafting inventory now supports module-provided add/new option flows for yarn weight, fiber tag, tool type, and notion type.
- 3D printing inventory now supports module-provided add/new option flows for material type and diameter.

## Next Work

- Start Phase F module platform hardening.
- Defer polished kit UI to Phase F, when module UI extension points and module-provided surfaces are hardened.

## Phase F Direction

- Phase F should harden the module platform after Phases A-E proved the baseline shape.
- Core must remain an independent host. If a domain module is removed, Core authentication, settings, module management, shell behavior, and other active modules should keep functioning.
- Domain modules should own their own language, workflows, persistence, services, endpoints, validation, UI surfaces, filtering, and reference data.
- Crafting inventory remains the reference implementation, not the production catch-all for knitting/crochet/sewing/etc.
- 3D printing remains the secondary boundary pressure-test for non-craft-shaped assumptions.
- Packaging, external module directories, installable artifacts, module-store concepts, and licensing should be addressed as Phase F+ concerns while avoiding premature coupling.

## Product Input Backlog

- `Scratch/TankerInput.md` contains ignored scratch notes for future non-technical product discussion, including pattern-version migration and possible UI theme feedback.

## Working Rules

- Treat `docs/project/roadmap.md` as the roadmap source of truth.
- Keep Core craft-agnostic; module-specific workflows belong in modules.
- Do not run full solution/server builds in the Codex sandbox. They can silently hang and fail after several minutes with no useful diagnostics.
- The user will manually run builds and tests as needed. When verification is required, provide the exact local command(s), usually `dotnet build TankerMade.sln`, and ask for the results.
- Do not commit local databases, Finder metadata, build outputs, or `Scratch/` content.

## Verification From Last Slice

The user reported:

- Build passed.
- Test passed.
- Phase E reference data integration slices were verified locally by the user rather than through Codex sandbox builds.

## Verification Needed Next

No Phase E verification is currently pending.
