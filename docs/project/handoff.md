# TankerMade Handoff

Last updated: 2026-09-24

## Pickup summary (new conversation)

**Active plan:** `docs/project/direction-brief.md`. Fiber commissions only. Knitting is the live vertical.

| Doc | Purpose |
|-----|---------|
| `docs/project/direction-brief.md` | Build order. Overrides this file’s older “next work” list |
| `docs/project/client-progress.md` | Two surfaces, lifecycle, private-versus-public rule |
| `docs/project/knitting-ui-parity-checklist.md` | Reference only. Not the active plan |
| `docs/product/ux-reference.md` | Crafter decisions + wireframe reference |

**Do next:** Projection and knitting translation, backend first. See Next Work below. Do not continue the MudBlazor pass onto other modules.

---

## Current State

- Current roadmap phase: Phase I — Security, Ops & Cleanup (complete). Crafting reference module retired (`PhaseN_RetireCraftingModule`).
- Knitting is the live fiber module. Per-row checks are stored on the server. Module pages use MudBlazor; inventory detail layouts use `MudGrid`.
- Crafter direction captured: `docs/product/ux-reference.md` § Crafter decisions; `Scratch/ui-discussion/2026-06-11-crafter-answers.md`.
- PPT parity rough score: see `docs/project/knitting-ui-parity-checklist.md` (knitting pages unchanged functionally; shell scoring updated).

### Pushed (`f8dbfd7`) — host shell + core dashboard

**MudBlazor integration (client):**

- Package: MudBlazor 9.5.0; `MudProviders.razor`, `Theme/TankerMadeMudTheme.cs`.
- `ThemeService` bridges MudBlazor `IsDarkMode` with legacy `data-theme` CSS variables (`wwwroot/js/theme.js`).

**Auth & routing:**

- `/` and `/login` → `Login.razor` on `EmptyLayout` (centered Mud card).
- `/home` → core `Home.razor` dashboard (module picker + recent projects).
- `/home/modules` → `ModuleActivation.razor`.
- Post-auth navigation → `/home`.

**Notebook shell (`MainLayout.razor`):**

- Minimal top bar: TankerMade brand only (sign-in link when logged out).
- No left binder strip (removed per feedback).
- **Bottom app bar** (`AppBottomNav.razor`) when signed in on Core or Knitting routes:
  - **Center:** contextual nav icons with hover-expand labels (Core: Dashboard, Modules; Knitting: Home, Projects, Patterns, Inventory, Settings).
  - **Left (module scope):** Dashboard return → `/home`.
  - **Right:** `MudAvatar` menu — Dark Mode toggle, username, sign out (menu opens upward above bar).

**Core dashboard recent projects (cross-module, boundary-safe):**

- Core ledger: `UserRecentWorkAccess` + migration `PhaseL_CoreRecentWorkAccess`.
- Contracts: `IRecentWorkService`, `IModuleRecentWorkSummaryProvider`, `RecentWorkSummaryDto` (title, thumbnail asset id, fallback path, last active, navigation path).
- API: `GET/POST api/dashboard/recent-work`.
- Knitting provider: `KnittingRecentWorkSummaryProvider`; default thumbnail `wwwroot/modules/knitting/default-project.svg` via `KnittingModule.DefaultProjectThumbnailPath`.
- Auto-record on `GET api/modules/{moduleKey}/capabilities/projects/{id}`.
- `Home.razor`: featured card (#1) + sidebar (#2–5); module-neutral display only.

**Knitting module UI (prior passes, still Bootstrap-heavy):**

- Home hero: last *worked on* + recently viewed (`KnittingRecentActivity`).
- Workspace: sticky chrome, per-row checkboxes (`KnittingRowProgress`), timers.
- Settings accordion + `ReferenceAutocompleteInput` (settings + project wizard theme).
- Inventory depth, wizards, filters, pagination (functional parity from `5df49e0` and earlier).

**Tests:** `RecentWorkServiceTests` (ledger upsert + inactive-module filter).

The MudBlazor checklist in `docs/project/knitting-ui-parity-checklist.md` stays as reference. It is not the active plan. Knitting UI changes only where the client-progress brief needs them.

---

## Next Work

Finish each step before the next. Source: `docs/project/direction-brief.md`.

1. **Done:** direction recorded in this handoff, `docs/project/charter.md`, and `docs/project/client-progress.md`.
2. **Projection and knitting translation, backend first.** Contracts, knitting capability handler, published snapshot, revision events, and token. Local API: publish, revoke, and anonymous read-by-token. Tests: private fields cannot appear on the public DTO; a revision records a price or date change; an inactive knitting module cannot publish; a revoked token does not resolve.
3. **Maker preview and private economics** on one knitting project. Quote price, deposit received, publish, revision, and client preview. Private net versus target rate on the same screen. Client preview uses a studio layout.
4. **Offline queue behavior.** Publishing with no hosted target still saves the local projection. Last-updated is the publish time. Do not build the cloud uploader.
5. **Stop and ask for a real-browser check.** Then the user runs `dotnet build TankerMade.sln`.

---

## Completed Recently (historical context)

- Phases A–I roadmap slices complete per `docs/project/roadmap.md`.
- Crafting reference module retired. Knitting is the live fiber module. Other craft modules remain templates.
- Knitting K5–K8: project workspace, settings, full operational inventory/patterns/kits, color/`StartedAt`.
- Phase L (client/host): cross-module recent-work ledger + knitting summary provider + core dashboard UI.
- MudBlazor shell: bottom nav, avatar account menu, EmptyLayout auth, centered login, `/home` core dashboard.

---

## Phase G Direction

- Phase G established module-friendly image/asset primitives; thumbnails and asset picker extension points in place.
- Core remains craft-agnostic host; modules own workflows, UI, and reference data. Recent-work summaries follow this pattern (opaque ledger + module enrichment).

## Product Input Backlog

- `Scratch/TankerInput.md` — ignored scratch product notes.
- `Scratch/docs/patterns/` — downloaded pattern PDFs and images. Gitignored. Never ship or seed.

## Working Rules

- `docs/project/roadmap.md` is roadmap source of truth.
- Do not run full solution builds in Codex sandbox; user runs `dotnet build TankerMade.sln` locally.
- Do not commit databases, build outputs, or `Scratch/` content.

## Verification Needed Next

Not yet. Step 2 is backend only. The real-browser check waits until step 5 of `docs/project/direction-brief.md`. The user runs `dotnet build TankerMade.sln` at that stop.
