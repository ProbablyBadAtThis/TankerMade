# Knitting Module — UI Parity Checklist

Source deck: `Scratch/TankerMade Documentation/UI_Feature_Ideas.pptx` (also mirrored at `docs/product/ui-feature-ideas.pptx`)

Companion UX spec: `docs/product/ux-reference.md` (YarnProject.pdf wireframes; largely aligned with the PPT)

**Scope:** Knitting module only (`/modules/knitting/*`). Crochet/Knit toggle filters from the deck are marked **N/A** where the knitting module is already craft-scoped.

**Last reviewed:** 2026-06-11 (after Phase A–F parity implementation pass)

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

## Summary (2026-06-11, post A–F)

| Area | ✅ | 🟡 | ❌ | ➖ |
|------|---:|---:|---:|---:|
| Home / Dashboard | 3 | 2 | 1 | 1 |
| Settings | 7 | 0 | 0 | 0 |
| Projects (list + detail) | 18 | 4 | 3 | 1 |
| Patterns (list + detail) | 14 | 7 | 2 | 1 |
| Inventory — Yarn | 9 | 5 | 4 | 0 |
| Inventory — Tools | 5 | 4 | 5 | 0 |
| Inventory — Notions | 5 | 4 | 6 | 0 |
| Kits (list + detail) | 6 | 4 | 3 | 1 |
| Wizards (project/pattern/kit) | 3 | 9 | 8 | 1 |
| **Rough totals** | **70** | **39** | **32** | **6** |

**Phase A–F status:** Implemented in working tree on `working/knitting-settings-ui-pass` (uncommitted at last review). Client + server project builds pass locally.

**Strongest today:** project workspace (stitch-driven progress, timers, per-piece stats), pattern piece/step editor, typed inventory + detail pages, reference settings, creation wizards, list polish (thumbnails, filters, difficulty badges).

**Remaining gaps vs deck:** main-color field, full wizard fidelity (form filters, inline step authoring, finalize cards), inventory deep UX (lot detail, remaining-weight entry, notion bulk split), kit image-card list layout, pagination, dashboard archive/terms shortcuts, project header hero image.

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
1. Home hero: most recent project + image + one-click open — ✅
2. Card thumbnails on project/pattern lists — ✅ (kits/inventory lists still text-first)
3. Project list: sort + filters (difficulty, completion, archived) — ✅ (theme/color filters still missing)
4. Pattern list: sort + filters (theme, difficulty, search) — ✅ (color/source filter gaps)
5. Kit list: search/sort in sidebar — 🟡 (not full card-grid parity)

### Phase B — Project workspace fidelity
1. Stitch totals + stitch-driven % — ✅
2. Per-piece completion % and tracked time in sidebar — ✅
3. Step range `5–7` format with expanded row chips — 🟡 (chips, not per-row checkboxes)
4. Scrollable pattern step panel — ✅
5. Difficulty colour-coding (6 levels) — ✅
6. Theme on project header — 🟡 (theme yes; main color still missing)

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
3. Terms glossary — ✅ (settings panel; not linked from dashboard)

### Phase F — Kit workspace parity
1. Kit workspace with child-project progress — ✅ (aggregated panel; not full project-layout clone)
2. Kit archive / reopen — ✅
3. Kit supplies text-only option — ✅ (manual name alongside inventory picker)

---

## Page-by-page checklist

### Home — `KnittingDashboard.razor` (PPT slide 1)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| H1 | Show most recently opened project name | ✅ | `KnittingRecentActivity` + fallback to most recently updated project |
| H2 | Show image of most recent project | ✅ | `AssetThumbnailImage` on hero card |
| H3 | “Open Most Recent Project” primary action | ✅ | Direct link to project workspace |
| H4 | Quick nav: Patterns, Yarns, Tools, Notions, Kits, Settings | ✅ | Stash card links to `inventory?tab=yarn|tools|notions` |
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

---

### Project list — `KnittingProjects.razor` (PPT slide 3)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| PL1 | Card grid with image + title | ✅ | `AssetThumbnailImage` on cards |
| PL2 | Default sort: most recent | ✅ | “Last opened” sort via `KnittingRecentActivity`; default remains `UpdatedAt` |
| PL3 | Sort/filter: theme | ✅ | Theme dropdown filter |
| PL4 | Sort/filter: main color | ❌ | Main color not modeled in knitting UI |
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
| PD4 | Row ranges in `5–7` format | 🟡 | Compact range + row chips; not per-row checkboxes |
| PD5 | Expandable / scrollable pattern box | ✅ | `step-panel-scroll` |
| PD6 | Timer play/pause | ✅ | Per-step timers |
| PD7 | Total time on piece | ✅ | Per-piece tracked time in sidebar meta |
| PD8 | Project image | ✅ | Header hero thumbnail (project asset, else linked pattern asset) |
| PD9 | Title, theme, main color in header | 🟡 | Title, theme, pattern; main color missing |
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
| NP2 | Step 1: form filter (2D/3D) | ❌ | |
| NP3 | Step 1: theme filter + search + add-new | 🟡 | Theme filter + add-new; not full deck search UX |
| NP4 | Step 1: pattern dropdown + search + add-new | 🟡 | Pattern pick + filter; limited search |
| NP5 | Step 2: row/rnd + stitch count entry | 🟡 | Supply/metadata steps exist; not full inline pattern authoring |
| NP6 | Step 2: Add Row, New Piece, Next Step | ❌ | Authoring remains on pattern detail |
| NP7 | Step 3: yarn/tool/notion dropdowns + add-new | 🟡 | Supply linking in wizard |
| NP8 | Step 4: image, title, difficulty colour picker | 🟡 | Metadata + difficulty; image step thin |
| NP9 | Finalize: summary card, supply checkboxes, Start Project | 🟡 | Review step exists; not full deck summary card |
| NP10 | Date started (auto or manual) | ❌ | |

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
| YL1 | Card grid with brand/color/fiber image | 🟡 | Clickable cards to detail; list thumbnails still thin |
| YL2 | Filter: Synthetic / Natural / Blended | ✅ | Fiber-tag filter chips |
| YL3 | Sort/filter: theme, color, source, brand, fiber | 🟡 | Search + fiber; not full multi-axis sort |
| YL4 | Add yarn form (brand, color, weight, lot, etc.) | ✅ | Tab form |
| YL5 | Fiber tag dropdown | ✅ | + add-new |
| YL6 | Sale price flag | ✅ | |
| YL7 | Auto-merge brand + color | ✅ | Backend merge on create |
| YL8 | Add / Finish buttons (multi-add flow) | 🟡 | Single “Add / Merge”; no Finish return navigation |
| YV1 | **View Yarn** detail page | ✅ | `KnittingYarnDetail.razor` |
| YV2 | Skeins, estimated remaining length | ✅ | Detail header + summary |
| YV3 | Enter new weight (remaining skein) | ✅ | Yarn detail “Update Remaining” form |
| YV4 | Lots dropdown → lot detail | 🟡 | Inline lot edit for remaining length/weight; no dedicated route |
| YV5 | Projects Active / History dropdowns | ✅ | Reverse lookup on detail page |
| YV6 | Purchase history list | ✅ | Full list on detail |
| YV7 | Lot detail page (slide 16) | ❌ | |

---

### Tool inventory (PPT slides 18–20)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| TL1 | Card grid with image | 🟡 | Clickable cards; list thumbnails thin |
| TL2 | Filter/sort by type, brand, size | 🟡 | Search; not full filter matrix |
| TL3 | Add tool form | ✅ | |
| TL4 | Auto-merge brand + type | ✅ | Backend |
| TV1 | **View Tool** detail page | ✅ | `KnittingToolDetail.razor` |
| TV2 | Sizes available (variants) | ❌ | |
| TV3 | Patterns suggesting this tool | 🟡 | Detail shows linked patterns when data exists |
| TV4 | Projects Active / History | 🟡 | Partial on detail page |
| TV5 | Purchase history | 🟡 | Summary + detail list |

---

### Notion inventory (PPT slides 21–27)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| NL1 | Card grid with image | 🟡 | Clickable cards; list thumbnails thin |
| NL2 | Filter/sort by type, brand, size, color | 🟡 | Search; not full filter matrix |
| NL3 | Add notion form | ✅ | |
| NL4 | Multi size/color → split listings dialog | ❌ | Deck slides 23, 27 |
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

## Cross-cutting gaps

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| X1 | Card thumbnails everywhere | ✅ | Projects, patterns, kits, inventory list cards |
| X2 | Theme / Source on patterns & projects | ✅ | Settings, forms, wizards, headers |
| X3 | Main color field | 🟡 | Core color picker on patterns; project header shows color; project create/edit picker still thin |
| X4 | Difficulty colour coding (6 levels) | ✅ | `KnittingUi` CSS badges |
| X5 | Search on pattern picker | ✅ | Pattern list + project wizard |
| X6 | Pagination on long lists | 🟡 | Client pagination on patterns + kits; projects/inventory still unpaged |
| X7 | Last-opened / recent tracking | ✅ | `KnittingRecentActivity` (localStorage) |

---

## Backend/API gaps still blocking full deck parity

Most Phase A–F blockers are cleared. Remaining gaps are primarily **deep inventory UX** and **wizard fidelity**, not core CRUD:

| API / behavior | Unblocks |
|----------------|----------|
| Main color on pattern/project entities + forms | PL4, PD9, PV1, wizard metadata steps |
| Lot detail route + remaining-weight edit APIs | YV3–YV7 |
| Notion variant / bulk-split backend | NL4, NV2 |
| Tool size variants | TV2 |
| Server-side `lastOpenedAt` (optional) | PL2 true “last opened” sort |
| List pagination | X6 |
| Inventory → pattern reverse lookup completeness | TV3, NV3 |

**Cleared in A–F pass:** `GetModuleYarn/Tool/NotionByIdAsync`, stitch rollup, per-piece client aggregates, recent-project tracking, core reference CRUD (`api/reference/*`), kit archive/reopen, kit child-project workspace aggregation.

---

## Verification harness (for parity sign-off)

Run after the full UI pass:

1. **Home** — open a project, return home; recent project hero shows name, thumbnail, and one-click open
2. **Project wizard** — create from existing pattern with supplies; lands in workspace with stitch-based %
3. **Workspace** — step checks update progress; per-piece stats and timers work; archive stays on page
4. **Pattern** — theme/source/metadata/supplies; wizard + detail authoring
5. **Inventory** — fiber filter; open yarn/tool/notion detail; confirm purchases, lots, project dropdowns
6. **Kit** — wizard or quick-create; workspace shows child progress; archive/reopen kit
7. **Settings** — add theme/source/brand/fiber tag/term; confirm pickers elsewhere

Local build:

```bash
dotnet build TankerMade.sln
```

---

## Recommended next work (post A–F)

Priority order for closing remaining deck gaps:

1. **Full UI verification pass** — run harness above; fix regressions
2. **Visual lock-in** — card grids, header hero images, kit list layout
3. **Wizard fidelity** — form filters, inline step authoring, finalize summary cards
4. **Inventory depth** — lot detail, remaining-weight entry, notion bulk split
5. **Main color** — entity + picker wiring across patterns/projects
6. **Pagination** — long lists

---

## Related files

| Area | Primary UI | Primary API |
|------|------------|-------------|
| Dashboard | `KnittingDashboard.razor` | `GetModuleProjectsAsync`, `KnittingRecentActivity` |
| Projects | `KnittingProjects.razor`, `KnittingProjectDetail.razor`, `KnittingProjectWizard.razor` | `ModuleProjectsController` |
| Patterns | `KnittingPatterns.razor`, `KnittingPatternDetail.razor`, `KnittingPatternWizard.razor` | `ModulePatternsController` |
| Inventory | `KnittingInventory.razor`, `KnittingYarnDetail.razor`, `KnittingToolDetail.razor`, `KnittingNotionDetail.razor` | `ModuleInventoryController` |
| Kits | `KnittingKits.razor`, `KnittingKitWizard.razor` | `ModuleKitsController` |
| Settings | `KnittingSettings.razor` | `ModuleSettingsController`, `ReferenceDataController` |
| Shared client | `KnittingUi.cs`, `KnittingCardAssetCache.cs`, `AssetThumbnailImage.razor` | `api/assets/*` |

---

*Update this checklist when behavior is verifiable in the running app, not merely when a control exists.*
