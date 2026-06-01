# TankerMade — Roadmap
Source: Distilled from original artifact.md phases A-I, updated to reflect current stack and progress.
Last reviewed: 2026-06-01

---

## Current Phase: I — Security, Ops & Cleanup

Phase A is behavior-complete as of the May 22, 2026 smoke test. Phase B is complete after expanding the reference Crafting module with pattern pieces, steps, readiness validation, and project workspace screens. Phase C is complete after adding module-owned project workspace behavior: step progress, per-step timers, completion rules, piece selection, archive flow, and non-destructive editing. Phase D is complete after proving module-owned inventory, reference data, project/inventory links, kit/grouping behavior, and kit-to-project backend flows.
Phase E is complete after wiring neutral Core reference categories through module extension points, enforcing module-owned category boundaries, and supporting module-provided add/new option flows in inventory surfaces.

Phase D should make modules architecturally real, not distribution-real. The goal is to prove that domain modules own their inventory, kit, filtering, reference data, and project-linking behavior while Core remains an independent host. Packaging, external module directories, installable artifacts, module-store concepts, and licensing remain later concerns for Phase F or beyond.

Phase I cleanup now includes neutralizing module API surfaces so module-specific legacy controllers are replaced by Core-neutral module capability endpoints. Current direction remains: Core provides neutral templates/contracts, modules provide behavior through registration/handlers.

## Phase A — Hardening, Module Host & Reference Module

### Done
- [x] Stack decided: Blazor WASM + ASP.NET Core + SQLite + EF Core 10
- [x] Solution structure: Core / Contracts / Application / Server / Client
- [x] SQL Server → SQLite swap
- [x] All projects upgraded to net10.0
- [x] Misplaced packages removed from Core, Application, Contracts
- [x] Swashbuckle replaced with Scalar (Swashbuckle broken on .NET 10)
- [x] IDesignTimeDbContextFactory added
- [x] Auto-migration on startup
- [x] InitialCreate migration: Users, Projects, Patterns, reference data seeded
- [x] JWT auth + BCrypt password hashing wired
- [x] AuthController present (register/login)
- [x] Server running, DB created, Scalar docs accessible
- [x] Modularity guardrail documented: base app is a module host, not a built-in craft app
- [x] GitHub Actions CI workflow (restore/build/test)
- [x] JWT secret removed from committed config; use user-secrets or environment variables

### Done in Phase A Completion
- [x] Add Core module-host entities, for example module manifest/registration and active module settings
- [x] Register core module-host entities in DbContext + migration
- [x] Implement module discovery/activation service contracts and service implementations
- [x] Add module host API endpoints
- [x] Scaffold first crafting module as a separate module project
- [x] Move/extract existing project and pattern foundation into the crafting module, not the base host
- [x] Add module-owned project/pattern entities, services, and API endpoints for the reference crafting module
- [x] Add minimal module-provided navigation/UI surfaces so loading the module visibly changes the app
- [x] Add tests for module host services and reference module services (xUnit)
- [x] Manual smoke test: register/login, activate/deactivate Crafting, confirm module APIs are available through Scalar
- [x] Confirm crafting module endpoint gating: inactive module returns forbidden for module-owned endpoints
- [x] Confirm crafting pattern CRUD
- [x] Confirm crafting project CRUD
- [x] Confirm cross-user ownership scoping
- [x] Confirm client module activation flow: Crafting appears in nav and page is accessible after activation
- [x] Patch module-owned update behavior so omitted/blank fields do not erase existing pattern/project values

---

## Phase B — Crafting Module V2

Phase B continues to treat `TankerMade.Modules.Crafting` as a reference/template module for development and platform proving. It should demonstrate module patterns that future golive niche modules can copy, but it should not become the production catch-all for knitting, crochet, sewing, or other specific crafts. Niche-specific rules belong in future dedicated modules unless represented here as neutral sample behavior.

- [x] Full CRUD + reorder for module-owned pattern pieces and steps
- [x] Pattern detail page in module UI
- [x] Step range display where relevant to the module
- [x] Progress aggregation and validation where relevant to the module
- [x] Expand module-owned project workspace screens beyond the Phase A reference baseline

---

## Phase C — Module Project Workspace

- [x] Module-owned step/checklist progress
- [x] Module-owned timers with play/pause
- [x] Module-specific completion percentage logic
- [x] Module-specific piece/section selector
- [x] Module-owned archive flow
- [x] Non-destructive editing

---

## Phase D — Module Inventory & Kits

- [x] Define and document the module-owned inventory pattern before adding niche behavior: module entities, services, endpoints, DTOs, UI routes, filtering, reference data, project links, and activation gating
- [x] Craft module inventory: yarn, tools, notions, lots, purchase history, and sale price handling
- [x] Add a thin 3D printing inventory proof so the module pattern is not accidentally craft-shaped: materials, spools, printer/tooling needs, and module-specific purchase history
- [x] Module-owned filtering and reference data
- [x] Purchase history per source; sale price handling
- [x] Module-owned project/inventory linking
- [x] Module-owned kit/grouping behavior
- [x] Module-owned kit/grouping to project flows

Phase D ordering preference:

1. Establish the reusable module inventory shape.
2. Implement Crafting inventory as the reference implementation.
3. Add a thin 3D printing inventory slice to pressure-test the boundary against a second domain.
4. Build richer project/inventory linking and kit flows after the boundary is proven by more than one module shape.

Phase D intentionally stops at backend/API proof for kits. Polished kit UI should wait until Phase F, when module UI extension points and module-provided surfaces are being hardened.

---

## Phase E — Reference Data Integration

- [x] Wire core Settings / ReferenceItem categories into module extension points
- [x] Keep module-specific reference data, such as fiber type, owned by the module that needs it
- [x] Support module-provided add/new option flows where modules expose dropdowns/tagging

---

## Phase F — Module Platform V1

- [x] Harden IModule contract and registration after the first module proves the shape
- [x] Support external module discovery from a configurable module directory
- [x] Expand UI extension points via DynamicComponent
- [x] Build/refine module-provided kit UI after the module UI extension model is clearer
- [x] Package first craft module as an installable/loadable module artifact
- [x] 3D printing module scaffold

---

## Phase G — Images & Assets

- [x] File storage (local disk first)
- [x] Thumbnail generation
- [x] Core asset picker extension points for module-owned records

---

## Phase H — Performance & Search

- [x] DB indexes on commonly filtered columns
- [x] Server-side filters and pagination on all list endpoints
- [x] Full-text search (SQLite FTS if needed)
- [x] Caching where beneficial

---

## Phase I — Security, Ops & Cleanup

- [x] JWT secret properly managed (user-secrets / env vars)
- [x] HTTPS enforced in production
- [x] Data Protection key persistence
- [x] Export/import round-trip tested and documented (`docs/project/export-import-roundtrip.md`)
- [x] Legacy code removal (any remaining stubs/scaffolds)
- [x] Deployment guidance (self-hosted, single binary, optional Docker) (`docs/project/deployment-guidance.md`)

---

## Open Decisions

| Topic | Status | Options / Notes |
|---|---|---|
| Mobile | Deferred | PWA first; Capacitor wrapper or dedicated Flutter/React Native client later |
| Offline WASM SQLite | Planned | Microsoft.Data.Sqlite compiled to WASM, or sqlite-wasm package |
| Module data strategy | Partially decided | Phase A reference Crafting module uses relational module-owned tables. Phase D should prove module-owned domain behavior while Core remains independently useful. Revisit packaging, external module storage, and install/uninstall mechanics in Phase F |
| Production DB provider | Open | SQLite default; Postgres/SQL Server as optional future provider |
| Image storage | Open | Local disk to start; pluggable provider interface for cloud later |
| Repository pattern | Skipped | Direct DbContext injection for now; revisit if complexity warrants |
| Module store/licensing | Future | Out of scope for Phase D. Keep the architecture compatible with future module distribution and licensing, but do not design those systems yet |
