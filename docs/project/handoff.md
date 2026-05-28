# TankerMade Handoff

Last updated: 2026-05-28

## Current State

- Current roadmap phase: Phase I — Security, Ops & Cleanup.
- Phase A is complete.
- Phase B is complete.
- Phase C is complete.
- Phase D is complete: module-owned inventory, reference data, project/inventory linking, kit/grouping backend, and kit-to-project backend flows are verified.
- Latest verified slice: Phase H slice 4 targeted caching (module list/discovery + core reference lookups) with compatibility fixes.
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
- Core module contracts are hardened with `IModule`, `IModuleNavigation`, and `IModulePackaging` seams.
- Bundled modules now register through a shared registration pipeline with startup sync.
- Module discovery now supports both bundled providers and configurable external manifest discovery.
- Active module navigation is now module-owned metadata rendered through host extension points.
- Crafting module kit UI now includes dedicated module-owned kits surface and flows.
- Registration now validates packaging compatibility against host runtime and manifest rules.

## Next Work

- Start Phase I security/ops slices: JWT secret handling hardening, production HTTPS enforcement, and Data Protection key persistence.

## Phase G Direction

- Phase G should establish module-friendly image and asset primitives after Phase F hardened module seams.
- Core must remain an independent host. If a domain module is removed, Core authentication, settings, module management, shell behavior, and other active modules should keep functioning.
- Domain modules should continue owning their own language, workflows, persistence, services, endpoints, validation, UI surfaces, filtering, and reference data.
- Asset primitives should be neutral and extensible so modules can attach media without Core becoming craft-specific.

## Product Input Backlog

- `Scratch/TankerInput.md` contains ignored scratch notes for future non-technical product discussion, including pattern-version migration and possible UI theme feedback.
- `Scratch/TankerMade Documentation/Pattern Examples/` contains real-world downloaded pattern examples kept only as guidance/reference material for feature design and workflow validation.
- Reference pattern files under `Scratch/TankerMade Documentation/Pattern Examples/` are explicitly out of product scope and must not be included, referenced, seeded, packaged, published, or redistributed as project content.

## Working Rules

- Treat `docs/project/roadmap.md` as the roadmap source of truth.
- Keep Core craft-agnostic; module-specific workflows belong in modules.
- Do not run full solution/server builds in the Codex sandbox. They can silently hang and fail after several minutes with no useful diagnostics.
- The user will manually run builds and tests as needed. When verification is required, provide the exact local command(s), usually `dotnet build TankerMade.sln`, and ask for the results.
- Do not commit local databases, Finder metadata, build outputs, or `Scratch/` content.
- Pattern example files in `Scratch/TankerMade Documentation/Pattern Examples/` are reference-only and must never be wired into app code, tests, docs, demos, or shipped artifacts.

## Verification From Last Slice

The user reported:

- Build passed.
- Test passed.
- Phase F module platform hardening slices were verified locally by the user rather than through Codex sandbox builds.

## Verification Needed Next

No Phase H verification is currently pending.
