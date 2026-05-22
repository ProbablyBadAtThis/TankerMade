# TankerMade — Roadmap
Source: Distilled from original artifact.md phases A-I, updated to reflect current stack and progress.
Last reviewed: 2026-05-22

---

## Current Phase: B — Crafting Module V2

Phase A is behavior-complete as of the May 22, 2026 smoke test. The base app now acts as a module host, the first bundled Crafting module is active behind module gating, and the reference module has working pattern/project CRUD.

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
- [ ] Expand module-owned project workspace screens beyond the Phase A reference baseline

---

## Phase C — Module Project Workspace

- [ ] Module-owned step/checklist progress
- [ ] Module-owned timers with play/pause
- [ ] Module-specific completion percentage logic
- [ ] Module-specific piece/section selector
- [ ] Module-owned archive flow
- [ ] Non-destructive editing

---

## Phase D — Module Inventory & Kits

- [ ] Craft module inventory: yarn, tools, notions, lots, purchase history, and sale price handling
- [ ] 3D printing module inventory: materials, spools, printer/tooling needs, and module-specific purchase history
- [ ] Module-owned filtering and reference data
- [ ] Purchase history per source; sale price handling
- [ ] Module-owned project/inventory linking
- [ ] Module-owned kit/grouping behavior
- [ ] Module-owned kit/grouping to project flows

---

## Phase E — Reference Data Integration

- [ ] Wire core Settings / ReferenceItem categories into module extension points
- [ ] Keep module-specific reference data, such as fiber type, owned by the module that needs it
- [ ] Support module-provided add/new option flows where modules expose dropdowns/tagging

---

## Phase F — Module Platform V1

- [ ] Harden IModule contract and registration after the first module proves the shape
- [ ] Support external module discovery from a configurable module directory
- [ ] Expand UI extension points via DynamicComponent
- [ ] Package first craft module as an installable/loadable module artifact
- [ ] 3D printing module scaffold

---

## Phase G — Images & Assets

- [ ] File storage (local disk first)
- [ ] Thumbnail generation
- [ ] Core asset picker extension points for module-owned records

---

## Phase H — Performance & Search

- [ ] DB indexes on commonly filtered columns
- [ ] Server-side filters and pagination on all list endpoints
- [ ] Full-text search (SQLite FTS if needed)
- [ ] Caching where beneficial

---

## Phase I — Security, Ops & Cleanup

- [ ] JWT secret properly managed (user-secrets / env vars)
- [ ] HTTPS enforced in production
- [ ] Data Protection key persistence
- [ ] Export/import round-trip tested and documented
- [ ] Legacy code removal (any remaining stubs/scaffolds)
- [ ] Deployment guidance (self-hosted, single binary, optional Docker)

---

## Open Decisions

| Topic | Status | Options / Notes |
|---|---|---|
| Mobile | Deferred | PWA first; Capacitor wrapper or dedicated Flutter/React Native client later |
| Offline WASM SQLite | Planned | Microsoft.Data.Sqlite compiled to WASM, or sqlite-wasm package |
| Module data strategy | Partially decided | Phase A reference Crafting module uses relational module-owned tables; revisit packaging/external-module storage in Phase F |
| Production DB provider | Open | SQLite default; Postgres/SQL Server as optional future provider |
| Image storage | Open | Local disk to start; pluggable provider interface for cloud later |
| Repository pattern | Skipped | Direct DbContext injection for now; revisit if complexity warrants |
