# Knitting Module — UI Parity Checklist

Source deck: `Scratch/TankerMade Documentation/UI_Feature_Ideas.pptx` (also mirrored at `docs/product/ui-feature-ideas.pptx`)

Companion UX spec: `docs/product/ux-reference.md` (YarnProject.pdf wireframes; largely aligned with the PPT)

**Scope:** Knitting module only (`/modules/knitting/*`). Crochet/Knit toggle filters from the deck are marked **N/A** where the knitting module is already craft-scoped.

**Last reviewed:** 2026-06-15 (`working/knitting-settings-ui-pass`; pushed `f8dbfd7`)

---

## Status legend

| Status | Meaning |
|--------|---------|
| ✅ | Implemented and usable for knitting testing |
| 🟡 | Partial — some behavior exists but not to deck spec |
| ❌ | Missing — no UI and/or blocking backend |
| ➖ | N/A for knitting module scope |

**Backend note:** 🟡/❌ items often need API or DTO work before UI can ship. Blocking backend gaps are called out inline.

---

## Summary (2026-06-15, current)

| Area | ✅ | 🟡 | ❌ | ➖ |
|------|---:|---:|---:|---:|
| Home / Dashboard | 6 | 0 | 0 | 1 |
| Settings + Glossary | 8 | 1 | 0 | 0 |
| Projects (list + detail) | 23 | 2 | 0 | 1 |
| Patterns (list + detail) | 13 | 2 | 0 | 1 |
| Inventory — Yarn | 12 | 2 | 0 | 0 |
| Inventory — Tools | 4 | 3 | 0 | 0 |
| Inventory — Notions | 3 | 3 | 0 | 0 |
| Kits (list + detail + wizard) | 8 | 6 | 0 | 1 |
| Wizards (project/pattern/kit) | 2 | 10 | 1 | 1 |
| App shell (host) | 6 | 2 | 0 | 0 |
| Core dashboard | 4 | 0 | 0 | 0 |
| Cross-cutting | 9 | 0 | 0 | 0 |
| **Rough totals** | **~98** | **~28** | **~1** | **6** |

**Branch / build:**

- **Pushed:** `f8dbfd7` on `working/knitting-settings-ui-pass` (MudBlazor shell, bottom nav, core recent-work platform + dashboard UI).
- Prior functional parity: `5df49e0` (Phase K7/K8 migrations, inventory depth, filters, pagination).

**Strongest today:** MudBlazor host shell with bottom contextual nav; core dashboard cross-module recent projects; knitting workspace with per-row checkboxes + sticky progress/timer; inventory depth; list pagination/filters.

**Crafter answers:** `docs/product/ux-reference.md` § Crafter decisions; `Scratch/ui-discussion/2026-06-11-crafter-answers.md`.

**Remaining vs deck:** knitting pages still Bootstrap (Mud migration in progress); wizard slide layout fidelity; pattern/inventory reference autocomplete outside settings; projects list worked-on sort; real-browser shell sign-off.

---

## Implementation phases (A–F)

Phases A–F from the original plan are **implemented at first-pass level**. Use the page-by-page checklist below for deck-fidelity gaps and the verification harness for sign-off.

| Phase | Scope | Status |
|-------|-------|--------|
| **A** | Home hero, card thumbnails, list filters/sorts | ✅ First pass |
| **B** | Stitch progress, per-piece stats, difficulty UI, workspace polish | ✅ First pass |
| **C** | Project / Pattern / Kit creation wizards | ✅ First pass |
| **D** | Inventory detail pages + list filters + ApiClient | ✅ First pass |
| **E** | Reference settings + theme/source on forms | ✅ First pass |
| **F** | Kit workspace parity + archive flow | ✅ First pass |

### Phase A — Navigation & list polish
1. Home hero: last worked-on project + image + one-click open — ✅ (`KnittingRecentActivity`; module home)
2. Card thumbnails on project/pattern/kit/inventory lists — ✅
3. Project list: sort + filters (theme, color, difficulty, completion, archived, live search, pagination) — ✅ (worked-on sort on list still thin; dashboard hero uses worked-on)
4. Pattern list: sort + filters (theme, color, source, difficulty, live search, pagination) — ✅
5. Kit list: thumbnail card grid + search/sort + pagination — ✅ (filter depth vs project list still thin)

### Phase B — Project workspace fidelity
1. Stitch totals + stitch-driven % — ✅
2. Per-piece completion % and tracked time in sidebar — ✅
3. Step range `5–7` format with per-row checkboxes — ✅ (`KnittingRowProgress`)
4. Scrollable pattern step panel — ✅
5. Difficulty colour-coding (6 levels) — ✅
6. Sticky workspace chrome (progress %, timer summary) — ✅ (`KnittingProjectDetail`)
7. Theme + color on project header — ✅

### Phase C — Creation wizards
1. New Project wizard — ✅ (`KnittingProjectWizard.razor`)
2. New Pattern wizard — ✅ (`KnittingPatternWizard.razor`)
3. New Kit wizard — ✅ (`KnittingKitWizard.razor`)
4. Theme/Source pickers with add-new — 🟡 (theme/source wired; search/add-new partial vs deck)

### Phase D — Inventory detail pages
1. `KnittingYarnDetail.razor` — ✅
2. `KnittingToolDetail.razor` — ✅
3. `KnittingNotionDetail.razor` — ✅
4. ApiClient by-id methods — ✅
5. Inventory list fiber-tag filters — ✅ (broader sort/filter still thin)

### Phase E — Settings & reference data
1. Themes, Colors, Sources, Brands, Fiber Tags in settings — ✅
2. Theme/Source on pattern forms — ✅
3. Terms glossary — ✅ (`KnittingGlossary.razor`; removed from settings clutter)

### Phase F — Kit workspace parity
1. Kit workspace with child-project progress — ✅ (aggregated panel; not full project-layout clone)
2. Kit archive / reopen — ✅
3. Kit supplies text-only option — ✅ (manual name alongside inventory picker)

---

## Page-by-page checklist

### Home — `KnittingDashboard.razor` (PPT slide 1)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| H1 | Show last worked-on project name | ✅ | `RecordProjectWorkedOnAsync`; recently viewed list for view-only opens |
| H2 | Show image of most recent project | ✅ | `AssetThumbnailImage` on hero card |
| H3 | “Open Most Recent Project” primary action | ✅ | Direct link to project workspace |
| H4 | Quick nav: Patterns, Yarns, Tools, Notions, Kits, Settings | ✅ | Single **Inventory** entry on dashboard; tabs inside inventory |
| H5 | Archive entry point | ✅ | `projects?archived=true` shortcut on dashboard |
| H6 | Terms link | ✅ | Dedicated `/modules/knitting/glossary` section on dashboard |
| H7 | Crochet/Knit filter | ➖ | Module-scoped |

**Target files:** `KnittingDashboard.razor`, `KnittingRecentActivity.cs`, `KnittingCardAssetCache.cs`

---

### Settings — `KnittingSettings.razor` (PPT slide 2)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| S1 | Themes management | ✅ | Core reference API + add-new in settings |
| S2 | Colors (reference list) | ✅ | |
| S3 | Sources (purchase sources) | ✅ | |
| S4 | Brands (reference list) | ✅ | |
| S5 | Fiber Type list | ✅ | Settings panel + inventory add-new |
| S6 | Terms (glossary) | ✅ | `KnittingGlossary.razor` (backed by module settings category `terms`) |
| S7 | Module behavior defaults | ✅ | Projects, kits, workspace, timers |
| S8 | Folding / accordion layout | ✅ | `<details>` sections — uncommitted UI pass |
| S9 | Reference autocomplete on add-new | 🟡 | `ReferenceAutocompleteInput` on theme/brand/etc.; pattern/inventory forms still thin |

**Target files:** `KnittingSettings.razor`, `ReferenceAutocompleteInput.razor`

---

### Project list — `KnittingProjects.razor` (PPT slide 3)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| PL1 | Card grid with image + title | ✅ | `AssetThumbnailImage` on cards |
| PL2 | Default sort: most recent progress | 🟡 | “Last opened” sort option; dashboard hero uses worked-on; list default still `UpdatedAt` |
| PL3 | Sort/filter: theme | ✅ | Theme dropdown filter |
| PL4 | Sort/filter: main color | ✅ | Color dropdown filter + card badge |
| PL5 | Sort/filter: difficulty | ✅ | Difficulty filter + badge |
| PL6 | Sort/filter: completion % | 🟡 | Progress sort + complete/incomplete filter; not fine-grained % bands |
| PL7 | Sort/filter: complete vs incomplete | ✅ | Completion filter |
| PL8 | Crochet/Knit type filter | ➖ | Module-scoped |
| PL9 | “New Project” entry | ✅ | Wizard link + quick-create form |
| PL10 | Show archived | ✅ | Setting-backed default |

---

### Project detail — `KnittingProjectDetail.razor` (PPT slide 4)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| PD1 | Step checkboxes update progress | ✅ | Per-step done toggles |
| PD2 | Stitch count rolls into completed total | ✅ | Server rollup on `ModuleProjectDto` |
| PD3 | Completion % from stitches | ✅ | Progress prefers stitch totals when present |
| PD4 | Row ranges in `5–7` format | ✅ | Per-row checkboxes; step completes when all rows in range checked |
| PD5 | Expandable / scrollable pattern box | ✅ | `step-panel-scroll` |
| PD6 | Timer play/pause | ✅ | Per-step timers |
| PD7 | Total time on piece | ✅ | Per-piece tracked time in sidebar meta |
| PD8 | Project image | ✅ | Header hero thumbnail (project asset, else linked pattern asset) |
| PD9 | Title, theme, main color in header | ✅ | Header badges + theme/color pickers in workspace setup |
| PD10 | Multi-piece selector | ✅ | Piece sidebar |
| PD11 | Per-piece completion % | ✅ | Sidebar meta |
| PD12 | Per-piece time display | ✅ | Sidebar meta |
| PD13 | Complete → archive (stay on page) | ✅ | Archive without navigation away |
| PD14 | Linked supplies | ✅ | Typed inventory links |
| PD15 | Project assets | ✅ | Upload/attach |
| PD16 | Difficulty colour-coded (6 levels) | ✅ | `KnittingUi` badges |

---

### New project wizard (PPT slides 5–9)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| NP1 | Step 1: pattern type filter (Crochet/Knit) | ➖ | Module-scoped |
| NP2 | Step 1: form filter (2D/3D) | 🟡 | Type + form filters in wizard; not full deck layout |
| NP3 | Step 1: theme filter + search + add-new | 🟡 | Theme filter + live pattern search; add-new partial |
| NP4 | Step 1: pattern dropdown + search + add-new | 🟡 | Pattern pick + live filter; create-new-pattern path exists |
| NP5 | Step 2: row/rnd + stitch count entry | ✅ | Inline draft steps when creating new pattern in wizard |
| NP6 | Step 2: Add Row, New Piece, Next Step | ✅ | `Add Row` + `New Piece` on wizard Steps step |
| NP7 | Step 3: yarn/tool/notion dropdowns + add-new | 🟡 | Supply linking in wizard |
| NP8 | Step 4: image, title, difficulty colour picker | 🟡 | Metadata + difficulty; image step thin |
| NP9 | Finalize: summary card, supply checkboxes, Start Project | ✅ | Summary card + supply checkboxes on finalize step |
| NP10 | Date started (auto or manual) | ✅ | Auto today or manual date; `StartedAt` on project (Phase K8) |

**Target:** `KnittingProjectWizard.razor`

---

### Pattern list — `KnittingPatterns.razor` (PPT slide 10)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| PT1 | Card grid with image + name | ✅ | Thumbnails via `KnittingCardAssetCache` |
| PT2 | Default sort: date added | ✅ | Recent (`CreatedAt`) default |
| PT3 | Sort/filter: theme, color, source, difficulty | ✅ | Theme, color, source, difficulty filters + live search |
| PT4 | Crochet/Knit filter | ➖ | Module-scoped |
| PT5 | Inline create | ✅ | Quick-create + wizard |
| PT6 | Supply metadata on create | ✅ | Yarn weight, needles, notions fields |

---

### Pattern detail — `KnittingPatternDetail.razor` (PPT slides 11–13)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| PV1 | Pattern name, source, theme, difficulty | ✅ | Header badges + metadata form |
| PV2 | Suggested yarn weight, needle sizes, notions | ✅ | Metadata form |
| PV3 | Pattern image | ✅ | Header hero thumbnail from first pattern asset |
| PV4 | Scrollable pattern steps | 🟡 | Step list; scroll styling lighter than deck mock |
| PV5 | Multi-piece dropdown / X of X pieces | ✅ | “Piece X of Y” counter above piece list |
| PV6 | Piece/step CRUD + reorder | ✅ | |
| PV7 | Stitch count on steps | ✅ | |
| PV8 | Pattern supplies list | ✅ | With inventory picker |
| PV9 | Pattern yarn supply as text-only option | ✅ | Inventory picker OR manual name |
| PV10 | Add-new pattern flow (slides 12–13) | 🟡 | Wizard exists; deck multi-step authoring fidelity incomplete |

---

### Yarn inventory (PPT slides 14–17)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| YL1 | Card grid with brand/color/fiber image | ✅ | Thumbnail cards + detail links |
| YL2 | Filter: Synthetic / Natural / Blended | ✅ | Fiber-tag filter chips |
| YL3 | Sort/filter: theme, color, source, brand, fiber | 🟡 | Search + fiber + brand/weight sort-filter toolbar |
| YL4 | Add yarn form (brand, color, weight, lot, etc.) | ✅ | Sectioned vertical form (`project-panel`; not `crafting-panel` flex) |
| YL5 | Fiber tag dropdown | ✅ | + add-new |
| YL6 | Sale price flag | ✅ | |
| YL7 | Auto-merge brand + color | ✅ | Backend merge on create |
| YL8 | Add / Finish buttons (multi-add flow) | 🟡 | Add / Merge + Finish link; not full deck multi-add loop |
| YV1 | **View Yarn** detail page | ✅ | `KnittingYarnDetail.razor` |
| YV2 | Skeins, estimated remaining length | ✅ | Detail header + summary |
| YV3 | Enter new weight (remaining skein) | ✅ | Yarn detail “Update Remaining” form |
| YV4 | Lots dropdown → lot detail | ✅ | View Lot link → `KnittingYarnLotDetail.razor` |
| YV5 | Projects Active / History dropdowns | ✅ | Reverse lookup on detail page |
| YV6 | Purchase history list | ✅ | Full list on detail |
| YV7 | Lot detail page (slide 16) | ✅ | `/inventory/yarn/{id}/lots/{lotId}` |

---

### Tool inventory (PPT slides 18–20)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| TL1 | Card grid with image | ✅ | Thumbnail cards + detail links |
| TL2 | Filter/sort by type, brand, size | 🟡 | Search + brand/type sort-filter toolbar |
| TL3 | Add tool form | ✅ | Sectioned vertical form (matches yarn/notion) |
| TL4 | Auto-merge brand + type | ✅ | Backend |
| TV1 | **View Tool** detail page | ✅ | `KnittingToolDetail.razor` |
| TV2 | Sizes available (variants) | ✅ | Sibling tools (same brand + type) on detail page |
| TV3 | Patterns suggesting this tool | 🟡 | Detail shows linked patterns when data exists |
| TV4 | Projects Active / History | 🟡 | Partial on detail page |
| TV5 | Purchase history | 🟡 | Summary + detail list |

---

### Notion inventory (PPT slides 21–27)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| NL1 | Card grid with image | ✅ | Thumbnail cards + detail links |
| NL2 | Filter/sort by type, brand, size, color | 🟡 | Search + brand/type/size/color sort-filter toolbar |
| NL3 | Add notion form | ✅ | Sectioned vertical form (matches yarn/tool) |
| NL4 | Multi size/color → split listings dialog | ✅ | Comma-separated size/color → split preview + confirm |
| NV1 | **View Notion** detail page | ✅ | `KnittingNotionDetail.razor` |
| NV2 | Options available (bulk variants) | ❌ | |
| NV3 | Patterns suggesting this notion | 🟡 | Partial on detail |
| NV4 | Projects Active / History | 🟡 | Partial on detail |

---

### Kit list — `KnittingKits.razor` (PPT slide 28)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| KL1 | Card grid with image + title | ✅ | Thumbnail card grid in kits sidebar with pagination |
| KL2 | Sort/filter like project list | 🟡 | Search + sort + archived toggle |
| KL3 | Kit type filter (Crochet/Knit) | ➖ | Module-scoped |
| KL4 | Create kit | ✅ | Quick-create + wizard |

---

### Kit detail / workspace (PPT slide 29)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| KD1 | Same layout as project detail | 🟡 | Dedicated workspace panel; planning forms still adjacent |
| KD2 | Per kit-piece timers & stitch stats | ✅ | Aggregated from linked child projects |
| KD3 | Kit complete → archive | ✅ | Archive / reopen actions |
| KD4 | Kit piece → create/open project | ✅ | |
| KD5 | Supply inventory picker | ✅ | |
| KD6 | Kit supplies text-only mode (deck preference) | ✅ | Manual name alongside inventory picker |

---

### New kit wizard (PPT slides 30–34)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| NK1 | Step 1: pattern/theme/type filters | 🟡 | Theme wired; type/form filters thin |
| NK2 | Step 2: pattern steps + New Kit Piece | 🟡 | Piece naming; not full step authoring |
| NK3 | Step 3: text-only supplies | ✅ | |
| NK4 | Step 4: image, difficulty colours | 🟡 | Metadata step partial |
| NK5 | Finalize + Start Project | 🟡 | Review/create; not full deck finalize |

**Target:** `KnittingKitWizard.razor`

---

## App shell (host-wide)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| S1 | MudBlazor theme + dark/light toggle | ✅ | `ThemeService`, `MudProviders`, `TankerMadeMudTheme`; toggle in avatar menu on bottom bar |
| S2 | Minimal top bar | ✅ | Brand only in `MainLayout`; no module links in header |
| S3 | Bottom contextual navigation | ✅ | `AppBottomNav.razor` — Core + Knitting scopes; hover-expand labels; dashboard return (left); avatar menu (right) |
| S4 | Auth on empty layout | ✅ | `Login.razor` / `Register` on `EmptyLayout`; `/` = login |
| S5 | Core dashboard at `/home` | ✅ | Module picker + module activation link |
| S6 | Nav usable at half-width | 🟡 | Bottom bar + hover labels; verify in real browser |
| S7 | Legacy top nav removed from active shell | 🟡 | `NavMenu.razor` retained but unused by `MainLayout`; candidate for deletion |

---

## Core dashboard (`/home`)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| C1 | Cross-module recent projects (top 5) | ✅ | `RecentWorkService` + `GET api/dashboard/recent-work`; featured + sidebar layout |
| C2 | Thumbnail + title + last active only | ✅ | `RecentWorkSummaryDto`; no craft-specific fields on core UI |
| C3 | Module default thumbnail when no photo | ✅ | Knitting: `modules/knitting/default-project.svg` via provider |
| C4 | Auto-record project open | ✅ | On module project GET; ledger `UserRecentWorkAccess` |

**Target files:** `Pages/Home.razor`, `RecentWorkController.cs`, `RecentWorkService.cs`, `KnittingRecentWorkSummaryProvider.cs`

---

## Cross-cutting gaps

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| X1 | Card thumbnails everywhere | ✅ | Projects, patterns, kits, inventory list cards |
| X2 | Theme / Source on patterns & projects | ✅ | Settings, forms, wizards, headers |
| X3 | Main color field | ✅ | Pattern + project pickers/filters (Phase K7 + `5df49e0`) |
| X4 | Difficulty colour coding (6 levels) | ✅ | `KnittingUi` CSS badges |
| X5 | Live search on lists / pickers | ✅ | Projects, patterns, inventory, kit list; wizard pattern filter |
| X6 | Pagination on long lists | ✅ | Client pagination on projects, patterns, kits, and inventory tabs |
| X7 | Last-opened / worked-on tracking | ✅ | Knitting: `KnittingRecentActivity` (client). Core: `UserRecentWorkAccess` ledger + recent-work API (cross-module) |
| X8 | Reference autocomplete on add-new | 🟡 | Settings + project wizard theme; pattern/inventory forms still thin |

---

## Backend/API gaps still blocking full deck parity

Core CRUD and workspace APIs are in place. Remaining blockers are mostly **wizard layout fidelity** and **optional scale**:

| API / behavior | Status | Notes |
|----------------|--------|-------|
| `ColorId` on pattern/project entities | ✅ Phase K7 | UI wired in `5df49e0` |
| `StartedAt` on projects | ✅ Phase K8 | Wizard + detail pickers |
| Yarn remaining + lot remaining edit | ✅ Phase K7 | |
| Lot detail route | ✅ | `KnittingYarnLotDetail.razor` (`5df49e0`) |
| Notion bulk-split | ✅ | Client-side split on add (`5df49e0`) |
| Tool size variants (sibling tools) | ✅ | Tool detail page (`5df49e0`) |
| Per-row step progress (server) | 🟡 | Row state in client localStorage (`KnittingRowProgress`); step complete when all rows checked |
| Core cross-module recent work | ✅ | Phase L — `UserRecentWorkAccess`, `IRecentWorkService`, knitting summary provider |
| Server-side list pagination | ❌ | Optional at scale |
| Inventory → pattern reverse lookup | 🟡 | TV3, NV3 |

**Cleared:** typed inventory by-id, stitch rollup, per-piece aggregates, core reference CRUD, kit archive/reopen, yarn remaining PUT endpoints.

---

## Verification harness — functional parity

Run in a real browser (not IDE embedded browser) as `member@test.com`:

1. **Home** — last worked-on hero; recently viewed; single Inventory entry; glossary; archived shortcut
2. **Projects** — live search, theme + color filters, pagination; header thumbnail + started date
3. **Project wizard** — finalize summary, supply checkboxes, date started; lands in workspace
4. **Workspace** — per-row checkboxes on ranged steps; timers; per-piece stats; theme/color edit; archive in place
5. **Patterns** — filters, pagination; detail metadata + piece X of Y
6. **Inventory** — sectioned add forms; filters/pagination; yarn lot detail; notion comma-split; tool size variants
7. **Kits** — thumbnail grid; child progress; archive/reopen
8. **Settings + Glossary** — accordion sections; reference autocomplete; glossary page

## Verification harness — host shell + core dashboard

Run in a real browser (not IDE embedded browser) as `member@test.com`:

1. **Login** — `/` centered card; sign in → `/home`
2. **Core dashboard** — recent projects featured + sidebar after opening knitting projects; empty state when none; no reload loop
3. **Bottom nav** — icon-only default; active label pinned; hover expand; dashboard return (knitting routes); avatar menu above bar
4. **Avatar menu** — Dark Mode toggle; username; sign out
5. **Theme** — dark/light persists; Mud + legacy CSS variables stay in sync
6. **Half-width** — bottom bar usable; no horizontal page scroll on `/home`

## Verification harness — knitting UI pass

1. **Theme** — warm dark surfaces on knitting dashboard and project cards; difficulty colors readable
2. **Module home** — “Last Worked On” label; single Inventory entry; no separate Yarn/Tools/Notions buttons
3. **Workspace** — sticky bar stays visible while scrolling steps (progress %, tracked time, piece name)
4. **Row checks** — `5–7` range shows one checkbox per row; all checked marks step complete
5. **Settings** — folded accordion sections; datalist suggestions when typing new theme/brand
6. **Half-width** — no horizontal page scroll on inventory/projects at ~50% browser width

Local build:

```bash
dotnet build TankerMade.sln
dotnet run --project src/TankerMade.Server
dotnet run --project src/TankerMade.Client
```

Optional re-seed: `dotnet run --project Scratch/knitting-seed/SeedKnittingData.csproj`

---

## Recommended next work

1. Run **all three** verification harness sections above in a real browser.
2. **Continue UI pass:** migrate knitting module pages from Bootstrap to MudBlazor; align with shell aesthetic.
3. **Polish if needed:** pattern/inventory reference autocomplete; projects list worked-on sort.
4. Propagate shell + Mud patterns to other live modules after knitting sign-off; remove legacy `NavMenu` when confirmed unused.

---

## Related files

| Area | Primary UI | Primary API |
|------|------------|-------------|
| Core dashboard | `Pages/Home.razor` | `RecentWorkController`, `RecentWorkService` |
| Dashboard | `KnittingDashboard.razor` | `GetModuleProjectsAsync`, `KnittingRecentActivity` |
| Projects | `KnittingProjects.razor`, `KnittingProjectDetail.razor`, `KnittingProjectWizard.razor` | `ModuleProjectsController` |
| Patterns | `KnittingPatterns.razor`, `KnittingPatternDetail.razor`, `KnittingPatternWizard.razor` | `ModulePatternsController` |
| Inventory | `KnittingInventory.razor`, `KnittingYarnDetail.razor`, `KnittingYarnLotDetail.razor`, `KnittingToolDetail.razor`, `KnittingNotionDetail.razor` | `ModuleInventoryController` |
| Kits | `KnittingKits.razor`, `KnittingKitWizard.razor` | `ModuleKitsController` |
| Settings | `KnittingSettings.razor` | `ModuleSettingsController`, `ReferenceDataController` |
| Glossary | `KnittingGlossary.razor` | `ModuleSettingsController` (category `terms`) |
| Shared client | `KnittingUi.cs`, `KnittingRecentActivity.cs`, `KnittingRowProgress.cs`, `KnittingCardAssetCache.cs`, `AssetThumbnailImage.razor`, `ReferenceAutocompleteInput.razor` | `api/assets/*` |
| App shell | `Layout/MainLayout.razor`, `Components/AppBottomNav.razor`, `Components/MudProviders.razor`, `Services/AppNavigation.cs`, `Services/ThemeService.cs`, `Layout/EmptyLayout.razor`, `wwwroot/css/app.css` | `RecentWorkController` |
| Legacy (unused) | `Layout/NavMenu.razor` | — |

---

*Update this checklist when behavior is verifiable in the running app, not merely when a control exists.*
