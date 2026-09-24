# TankerMade — Roadmap
Source: Phases A–I are the completed host. Phase J is the active tracker.
Last reviewed: 2026-09-24
Direction: `docs/project/direction-brief.md`. Product rules: `docs/project/client-progress.md`.

---

## Current phase: J — Client progress

Fiber commissions only. Knitting is the only module that changes. Finish each step before the next.

| | |
|---|---|
| Now | Step 2 — projection backend |
| Done | Step 1 — direction recorded |
| Stop | Step 5 — browser check, then `dotnet build TankerMade.sln` |

**Done when:** a signed-in maker publishes a knitting commission and hands someone a local link. The link shows stage, materials safe to show, photos, next step, last updated, and revisions. Revoking the link cuts it off. The maker, only while signed in, sees whether the price beats their rate. No other craft module was changed.

### Already in place

Use these. Do not rebuild them for this phase.

- Knitting project workspace: pieces, steps, server row checks, per-step timers, inventory links, photos via core assets.
- Capability handlers under `src/TankerMade.Contracts/Services/ModuleCapabilities/`. Copy `IModuleRecentWorkSummaryProvider`: Core stores an opaque ref, the module fills in the words.
- Signed-in asset API. The client page does not reuse it.

### Step 1 — Record the direction

- [x] `docs/project/client-progress.md` states the two surfaces, lifecycle, and private-versus-public rule
- [x] Charter and handoff point at the brief; MudBlazor checklist is reference only
- [x] `.gitignore` keeps scratch patterns and the local workshop photo off the remote

### Step 2 — Projection backend

Core owns the commission record and the public snapshot. Knitting owns the sentence the client reads. The snapshot is the last publish, not a live query of workshop tables.

**Contracts**

- [ ] Core stage enum: Quote, Accepted, Materials, In progress, Revision, Ready, Delivered
- [ ] Public snapshot DTO: stage, plain-language summary, safe material lines, photo asset ids, next step, last updated, visible revision history
- [ ] Workshop-only DTOs: quote price, deposit received, target rate, material dollars, elapsed time, implied net, beats-rate
- [ ] `IModuleClientStatusProvider` beside the other capability interfaces. Input is the project plus what the maker chose to publish. Output is the public fields. Knitting translates jargon (“Blocking the body” → “Assembling the pieces.”)

**Storage** (Core tables, opaque project id, no FK into knitting)

- [ ] Publication: owner, module key, project id, token hash, revoked-at, last published at
- [ ] Snapshot: one JSON blob of the public DTO, replace on each publish
- [ ] Revision rows: when, what changed, whether price moved, whether the due date moved
- [ ] Migration. Historical crafting migrations stay

**API** (local server only)

- [ ] Signed-in: publish, add revision, revoke, preview (same payload the link returns)
- [ ] Anonymous: read by token. Read-only. No client account
- [ ] Anonymous photo route: token + asset id, and only if that id is in the current snapshot
- [ ] Inactive knitting module cannot publish
- [ ] Revoked or unknown token does not resolve (same response either way)
- [ ] Token is unguessable. Store the hash, not the raw token

**Tests**

- [ ] Public DTO has no hours, rate, implied net, private notes, or measurements
- [ ] A revision records a price change or a due-date change
- [ ] Inactive knitting module cannot publish
- [ ] Revoked token does not resolve
- [ ] Photo id absent from the snapshot does not resolve on the anonymous route

### Step 3 — Maker preview and private economics

One knitting project screen. Workshop theme stays. The preview is a separate studio layout with no workshop nav.

- [ ] Core maker rate on the user, not in knitting settings
- [ ] On the project: quote price, expected window, deposit marked received (no payment processing)
- [ ] Actions: publish current stage, add a revision, copy preview link, revoke
- [ ] Preview opens the anonymous payload in the studio layout, signed-in maker included
- [ ] Private panel on the workshop screen only: linked material cost, timer total, implied net, whether price beats the rate
- [ ] Knitting supplies the public sentence from the active piece and step. The maker can edit that sentence before publish

### Step 4 — Local outbox, no uploader

- [ ] Publish with no hosted target still writes the local snapshot
- [ ] Last updated is the publish time, so a closed laptop does not look stalled
- [ ] Snapshot shape is something a later uploader can send unchanged
- [ ] No cloud client, retry loop, or hosted dashboard in this step

### Step 5 — Stop for a real check

Do not start the next product slice in the same pass.

- [ ] Maker, signed in: open a knitting project, publish, copy the link, revoke, confirm the link dies
- [ ] Client, signed out: open the link and confirm hours, rate, notes, and measurements are absent; a revision is visible
- [ ] User runs `dotnet build TankerMade.sln`

### Decide before Step 2 code

| Topic | Call |
|---|---|
| Quote price and due date | Public. Revisions exist to show when they moved |
| Inventory material dollars, hours, rate, implied net | Private. Workshop APIs only. Never on the snapshot |
| Material lines on the link | Names the maker publishes. Dollar cost stays in the private panel unless a later decision says the quote’s material estimate is public |
| Where the rate lives | Core user setting. A rate is not fiber-specific |
| Client photos | Token-scoped asset ids. Not `AssetsController` |

### Not in Phase J

Hosted page, client accounts, payments, email or SMS, Ravelry import, gamification, module store, licensing, and any work in crochet, embroidery, quilting, sewing, or 3D printing. MudBlazor parity on those modules stays stopped.

---

## Phases A–I — complete

Host, module platform, assets, search, and security ops. Knitting replaced the retired Crafting reference module. Detail below is the record, not the active plan.

Phase A is behavior-complete as of the May 22, 2026 smoke test. Phase B is complete after expanding the reference Crafting module with pattern pieces, steps, readiness validation, and project workspace screens. Phase C is complete after adding module-owned project workspace behavior: step progress, per-step timers, completion rules, piece selection, archive flow, and non-destructive editing. Phase D is complete after proving module-owned inventory, reference data, project/inventory links, kit/grouping behavior, and kit-to-project backend flows.
Phase E is complete after wiring neutral Core reference categories through module extension points, enforcing module-owned category boundaries, and supporting module-provided add/new option flows in inventory surfaces.

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

Phase B treated `TankerMade.Modules.Crafting` as a reference module. That module is retired. Knitting is the live fiber implementation to copy. Niche-specific rules belong in dedicated modules.

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
