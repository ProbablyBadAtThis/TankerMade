# TankerMade Handoff

Last updated: 2026-06-11

## Current State

- Current roadmap phase: Phase I — Security, Ops & Cleanup (complete).
- Phase A is complete.
- Phase B is complete.
- Phase C is complete.
- Phase D is complete: module-owned inventory, reference data, project/inventory linking, kit/grouping backend, and kit-to-project backend flows are verified.
- Latest verified slice: Neutral module capability/API cleanup verified locally by user.
- Latest pushed commit: `962797d` on `working/knitting-settings-ui-pass` (Knitting parity follow-up: glossary, live search, thumbnails, wizards, color/yarn APIs, Phase K7 migration).
- Working tree (uncommitted): Phase K8 parity pass — project `StartedAt`, color pickers/filters, wizard finalize/date started, projects/inventory pagination + list filters, yarn lot detail route, tool size variants, notion bulk-split, inventory form layout fix.
- Knitting PPT parity (see `docs/project/knitting-ui-parity-checklist.md`): **functional pass complete**; crafter answers landed in `#ui-discussion` (2026-06-11) — see `docs/product/ux-reference.md` § Crafter decisions and `Scratch/ui-discussion/2026-06-11-crafter-answers.md`.

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
- Added live bundled modules for: 3D Printing, Crochet, Embroidery, Knitting, Quilting, and Sewing.
- Added EF migration `20260530172000_PhaseJ_LiveModulesSeedSwap` to align bundled module seed data.
- 3D Printing now has a module home (`/modules/printing-3d`) and section pages (inventory, projects, patterns, queue, settings).
- Initial UI pass completed for all live module dashboards and 3D Printing section pages.
- Legacy Crafting module-specific controllers were removed in favor of neutral module capability endpoints.
- Legacy Printing inventory controller was removed; Printing inventory now flows through neutral module inventory capabilities.
- Printing inventory client page was updated to use neutral module inventory + neutral asset assignment/picker APIs.
- Scalar now reflects neutral/core module capability endpoints for active module workflows.
- Knitting project workspace capability slice is now substantially implemented in current branch work:
  - Added module project DTO coverage for step progress, timers, and project inventory links in `TankerMade.Contracts`.
  - Added Knitting project DTO/entity support for step progress, timers, and inventory links in `TankerMade.Modules.Knitting`.
  - Added Knitting project service behavior for step progress toggles, timer start/pause/set/reset, and supply link add/remove.
  - Added capability-handler/controller flow for module project workspace operations to support Knitting through neutral module capability endpoints.
  - Added EF migration `20260601215423_PhaseK5_KnittingProjectWorkspaceCapabilities`.
  - Knitting project detail UI now has first-pass workspace behavior for piece/step checklist, timer controls, and linked supplies.
- Knitting settings now provide a real module behavior surface instead of only raw key/value CRUD:
  - Added typed settings groups for projects, kits, workspace defaults, and timer behavior.
  - Added per-setting save/reset, per-category reset, and advanced custom-key management for non-modeled keys.
  - Wired known settings into Knitting pages so values influence behavior at runtime:
    - Projects page can default `Show archived` and default new-project difficulty from settings.
    - Kits page can default `Show archived` from settings.
    - Project workspace can default planned supply quantity, timer edit starting minutes, and optional auto-pause-on-complete behavior.
- Knitting UX updates from user review are now reflected in the current slice:
  - Project timers now use compact tracked-time display and a D/H/M/S timer edit input model.
  - Kit detail editing now runs in a dedicated modal to avoid create/edit form overlap.
  - Pattern detail half-width layout was tightened for piece controls and workspace readability.
- Knitting UI parity phases A–F plus follow-up passes are on `working/knitting-settings-ui-pass` (see `docs/project/knitting-ui-parity-checklist.md`):
  - **A:** Home hero, stash deep links, glossary section, archived shortcut; list thumbnails/filters/live search on projects, patterns, kits, inventory.
  - **B:** Stitch-driven progress, per-piece stats, timers, difficulty badges, project/pattern header thumbnails.
  - **C:** Project, pattern, and kit wizards (first-pass; deck fidelity still partial).
  - **D:** Yarn/tool/notion detail pages, fiber filters, yarn remaining + lot edit APIs, sectioned inventory add forms.
  - **E:** Reference settings; standalone `KnittingGlossary.razor`; theme/source/color on pattern flows.
  - **F:** Kit thumbnail card grid, archive/reopen, child-project workspace aggregation.
  - Shared client: `KnittingUi`, `KnittingRecentActivity`, `KnittingCardAssetCache`, `AssetThumbnailImage`, `DifficultyPicker`, `ReferenceDataController`.
- User verification (real browser): home, projects, patterns, project/pattern detail pass; inventory add-form layout was unusable until `crafting-panel` → `project-panel` fix (in working tree).
- Crafter UI answers in `#ui-discussion` (TankerMunk, 2026-06-11): cozy notebook + warm dark default; big photo project cards; single Inventory on home; sticky timer/progress; per-row checkboxes; inline pattern edits; one-form yarn add; settings accordion; autocomplete on reference add-new; reliability/speed are dealbreakers.

## Next Work

1. **Visual lock-in pass** (crafter-directed): dark+warm theme default, sticky workspace chrome (timer + %), settings accordion, home → single Inventory + “last worked on” hero.
2. **Deck fidelity gap:** per-row checkboxes for row ranges (`5–7` → one checkbox per row) — crafter must-keep.
3. **Reference UX:** autocomplete on theme/brand/type add-new to prevent duplicates.
4. **Commit/push** parity pass + doc updates after smoke test (`dotnet build TankerMade.sln`).
5. **Deprioritize** modal-first pattern edits and compact card density (crafter prefers large photo cards and inline pattern editing).
6. After Knitting sign-off, propagate shared patterns to other live modules.

Parity field-check and harness: `docs/project/knitting-ui-parity-checklist.md`.

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

- `dotnet build TankerMade.sln` passed.
- EF migration checks passed.
- Smoke checks passed.
- Neutral endpoint checks passed, including expected negative tests.

## Verification Needed Next

Functional parity sign-off (real browser, not IDE embedded browser):

1. `dotnet build TankerMade.sln`
2. Run server + client; execute harness in `docs/project/knitting-ui-parity-checklist.md`
3. Re-test inventory add forms after sectioned layout fix (yarn/tools/notions tabs)
4. Commit/push any remaining working-tree fixes after smoke pass
