# TankerMade Handoff

Last updated: 2026-09-24

## Pickup summary (new conversation)

**Active plan:** `docs/project/direction-brief.md`. Fiber commissions only. Knitting is the live vertical.

| Doc | Purpose |
|-----|---------|
| `docs/project/roadmap.md` | Active tracker (Phase J). Overrides this file’s older “next work” list |
| `page/docs` | Dev tracker published to Cloudflare Pages. Keep it in step with the roadmap |
| `docs/project/direction-brief.md` | Product direction and build order |
| `docs/project/client-progress.md` | Two surfaces, lifecycle, private-versus-public rule |
| `docs/project/knitting-ui-parity-checklist.md` | Reference only. Not the active plan |
| `docs/product/ux-reference.md` | Crafter decisions + wireframe reference |

**Do next:** Phase J, Step 2 in `docs/project/roadmap.md`. Do not continue the MudBlazor pass onto other modules.

---

## Current State

- Current roadmap phase: Phase J — Client progress. Tracker: `docs/project/roadmap.md`. Phases A–I are complete. Crafting reference module retired (`PhaseN_RetireCraftingModule`).
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

The checklist is Phase J in `docs/project/roadmap.md`. Finish each step before the next.

1. **Done.** Direction recorded.
2. **Now.** Projection backend: contracts, knitting client-status provider, snapshot storage, revision rows, token, publish / revoke / anonymous read.
3. Maker preview and private economics on one knitting project.
4. Local outbox. No cloud uploader.
5. Stop for a browser check. Then the user runs `dotnet build TankerMade.sln`.

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
- When the roadmap, current phase, or handoff “do next” changes, update `page/docs` in the same change. That folder is the dev tracker and publishes to Cloudflare Pages on push. Dashboard, phase list, Phase J task list, architecture summary, and footer should stay aligned with the roadmap. Do not leave the page on an older phase.
- Do not run full solution builds in Codex sandbox; user runs `dotnet build TankerMade.sln` locally.
- Do not commit databases, build outputs, or `Scratch/` content.

## Verification Needed Next

Not yet. Step 2 is backend only. The real-browser check waits until step 5 of `docs/project/direction-brief.md`. The user runs `dotnet build TankerMade.sln` at that stop.
