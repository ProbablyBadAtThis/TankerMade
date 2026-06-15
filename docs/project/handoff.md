# TankerMade Handoff

Last updated: 2026-06-15

## Pickup summary (new conversation)

**Branch:** `working/knitting-settings-ui-pass`  
**Latest pushed commit:** `f8dbfd7` — MudBlazor shell, bottom navigation, core dashboard recent-work platform.  
**Working tree:** clean (pushed).

| Doc | Purpose |
|-----|---------|
| `docs/project/knitting-ui-parity-checklist.md` | Field-check + verification harness |
| `docs/product/ux-reference.md` | Crafter decisions + wireframe reference |
| `Scratch/ui-discussion/2026-06-11-crafter-answers.md` | Full `#ui-discussion` Q&A |

**Do next:** Continue **UI pass** — migrate knitting pages from Bootstrap to MudBlazor; real-browser verification of shell + core dashboard; knitting sign-off; propagate patterns to other live modules.

---

## Current State

- Current roadmap phase: Phase I — Security, Ops & Cleanup (complete).
- Knitting module on `working/knitting-settings-ui-pass`: **functional parity complete**; **host shell + core dashboard** committed; **knitting module pages still largely Bootstrap** (UI pass in progress).
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

### Still thin / not done

- **Knitting pages MudBlazor migration** — most module routes still Bootstrap; shell/auth/home/login use Mud.
- Pattern wizard + inventory reference autocomplete (settings + project wizard theme only today).
- Projects list “last worked on” sort (core dashboard + knitting hero use worked-on; list sort still last-opened/updated).
- Wizard slide **layout** fidelity (cosmetic).
- Propagate Mud shell patterns to Crochet, Embroidery, Quilting, Sewing, 3D Printing dashboards.
- `NavMenu.razor` — legacy horizontal Bootstrap nav; superseded by bottom bar (candidate for removal after migration).
- Real-browser sign-off on new shell + core dashboard recent projects.

---

## Next Work

1. **Continue UI pass** on knitting module pages (MudBlazor components, align with shell aesthetic).
2. **Verify in real browser:** bottom nav expand/hover, avatar menu, core dashboard recent projects, login/home flows — harness in `docs/project/knitting-ui-parity-checklist.md`.
3. **Polish** (if verification finds gaps): pattern/inventory wizard autocomplete; projects list worked-on sort.
4. After knitting sign-off: migrate other live module dashboards + remove legacy `NavMenu` if unused.

---

## Completed Recently (historical context)

- Phases A–I roadmap slices complete per `docs/project/roadmap.md`.
- Crafting remains reference/template module; live modules: Knitting, Crochet, Embroidery, Quilting, Sewing, 3D Printing.
- Knitting K5–K8: project workspace, settings, full operational inventory/patterns/kits, color/`StartedAt`.
- Phase L (client/host): cross-module recent-work ledger + knitting summary provider + core dashboard UI.
- MudBlazor shell: bottom nav, avatar account menu, EmptyLayout auth, centered login, `/home` core dashboard.

---

## Phase G Direction

- Phase G established module-friendly image/asset primitives; thumbnails and asset picker extension points in place.
- Core remains craft-agnostic host; modules own workflows, UI, and reference data. Recent-work summaries follow this pattern (opaque ledger + module enrichment).

## Product Input Backlog

- `Scratch/TankerInput.md` — ignored scratch product notes.
- `Scratch/TankerMade Documentation/Pattern Examples/` — reference-only; never ship or seed.

## Working Rules

- `docs/project/roadmap.md` is roadmap source of truth.
- Do not run full solution builds in Codex sandbox; user runs `dotnet build TankerMade.sln` locally.
- Do not commit databases, build outputs, or `Scratch/` content.

## Verification Needed Next

```bash
dotnet build TankerMade.sln
dotnet run --project src/TankerMade.Server
dotnet run --project src/TankerMade.Client
```

Run harness sections in `docs/project/knitting-ui-parity-checklist.md`:

1. Functional parity (knitting module — inventory, wizards, filters, workspace)
2. Host shell (bottom nav, avatar menu, theme toggle, login, core dashboard recent projects)

Sign in as `member@test.com` → `/home` and `/modules/knitting`. Use a real browser, not the IDE embedded browser.

Optional re-seed: `dotnet run --project Scratch/knitting-seed/SeedKnittingData.csproj`
