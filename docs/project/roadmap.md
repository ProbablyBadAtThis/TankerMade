# TankerMade — Roadmap
Source: Distilled from original artifact.md phases A-I, updated to reflect current stack and progress.
Last reviewed: 2026-05-20

---

## Current Phase: A — Hardening & Foundation

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

### Remaining in Phase A
- [ ] Add missing Core entities (see TankerMade-Artifact.md §9 Step 1)
- [ ] Register new entities in DbContext + migration
- [ ] Implement ProjectService, PatternService (currently all NotImplementedException stubs)
- [ ] Add ProjectsController, PatternsController
- [ ] Add tests for services (xUnit)
- [ ] GitHub Actions CI workflow (restore/build/test)
- [ ] Move JWT secret to user-secrets

---

## Phase B — Patterns V2

- [ ] Full CRUD + reorder for PatternPieces and PatternSteps
- [ ] Pattern detail page in client
- [ ] Step range display (rows 5–7 repeat format)
- [ ] Stitch count aggregation (per piece, per pattern)
- [ ] Validation and improved UX

---

## Phase C — Project Workspace

- [ ] Step checklist with completion tracking (ProjectStepProgress)
- [ ] Per-piece timers (TimerSession) with play/pause
- [ ] Completion percentage calculated from checked steps + stitch counts
- [ ] Piece selector on project detail
- [ ] Archive flag + archive flow
- [ ] Non-destructive editing

---

## Phase D — Inventory & Kits

- [ ] Yarn inventory: brand, color, weight, fiber type, lot tracking
- [ ] Tool inventory: type, brand, size, purchase history
- [ ] Notions inventory: type, brand, size, color, multi-listing support
- [ ] Fiber tags (Synthetic / Natural / Blended) for filtering
- [ ] Lot number tracking with estimated remaining length calculation
- [ ] Purchase history per source; sale price handling
- [ ] Project ↔ Inventory linking
- [ ] Kit entity: multi-piece bundle with its own progress tracking
- [ ] Kit → Project flows

---

## Phase E — Reference Data Integration

- [ ] Wire Settings / ReferenceItem categories into forms (dropdowns/tagging)
- [ ] Theme, Color, Source, Brand, FiberType all selectable in UI
- [ ] Add/new option inline in dropdowns

---

## Phase F — Module Platform V1

- [ ] Define IModule contract and registration
- [ ] Module discovery (load from directory)
- [ ] UI extension points via DynamicComponent
- [ ] Crafting module extracted as first example
- [ ] 3D printing module scaffold

---

## Phase G — Images & Assets

- [ ] File storage (local disk first)
- [ ] Thumbnail generation
- [ ] Image pickers on Projects, Patterns, Inventory items

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
| Module data strategy | Open | JSON fields (ModuleDataJson) vs relational tables — JSON for V1, relational later |
| Production DB provider | Open | SQLite default; Postgres/SQL Server as optional future provider |
| Image storage | Open | Local disk to start; pluggable provider interface for cloud later |
| Repository pattern | Skipped | Direct DbContext injection for now; revisit if complexity warrants |
