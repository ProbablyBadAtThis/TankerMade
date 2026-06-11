# Knitting Module — UI Parity Checklist

Source deck: `Scratch/TankerMade Documentation/UI_Feature_Ideas.pptx` (also mirrored at `docs/product/ui-feature-ideas.pptx`)

Companion UX spec: `docs/product/ux-reference.md` (YarnProject.pdf wireframes; largely aligned with the PPT)

**Scope:** Knitting module only (`/modules/knitting/*`). Crochet/Knit toggle filters from the deck are marked **N/A** where the knitting module is already craft-scoped.

**Last reviewed:** 2026-06-11 (after Phase K6 operational UI slice)

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

## Summary (2026-06-11)

| Area | ✅ | 🟡 | ❌ | ➖ |
|------|---:|---:|---:|---:|
| Home / Dashboard | 0 | 1 | 2 | 1 |
| Settings | 0 | 1 | 5 | 0 |
| Projects (list + detail) | 6 | 8 | 9 | 1 |
| Patterns (list + detail) | 10 | 5 | 6 | 1 |
| Inventory — Yarn | 4 | 4 | 10 | 0 |
| Inventory — Tools | 3 | 3 | 8 | 0 |
| Inventory — Notions | 3 | 3 | 9 | 0 |
| Kits (list + detail) | 5 | 4 | 11 | 1 |
| **Rough totals** | **31** | **29** | **60** | **5** |

Strongest today: **project workspace** (steps, timers, archive), **pattern piece/step editor**, **typed inventory create/merge**, **settings-driven defaults**.

Weakest today: **wizards**, **list filters/sorts**, **card images**, **detail pages** (view yarn/tool/notion), **stitch-driven completion**, **reference settings** (themes/sources/brands/terms).

---

## Recommended implementation order

Work top-down so each phase unlocks meaningful UI testing.

### Phase A — Navigation & list polish (high visibility, moderate effort)
1. Home hero: most recent project + image + one-click open
2. Card thumbnails on project/pattern/kit/inventory lists (from Phase G assets)
3. Project list: sort (recent default) + filters (theme, difficulty, complete/incomplete)
4. Pattern list: sort (date added) + filters (theme, source, difficulty)
5. Kit list: same filter/sort pattern as projects

### Phase B — Project workspace fidelity (core maker loop)
1. Stitch totals: completed stitches / total stitches + % driven by stitch counts (not just step count)
2. Per-piece completion % and per-piece tracked time in piece sidebar
3. Step range display in compact `5–7` format with per-row checkboxes where ranges expand
4. Scrollable / expandable pattern step panel
5. Difficulty colour-coding (6 levels) on project header and cards
6. Theme / main color on project header (needs pattern/project metadata wiring)

### Phase C — Creation wizards (large UX gap)
1. **New Project** 4-step wizard (pattern pick → steps → supplies → metadata → finalize)
2. **New Pattern** multi-step flow (steps entry → supplies → metadata) — can reuse pattern detail after create
3. **New Kit** wizard (pattern/kit piece flow per slides 30–34)
4. Core reference pickers in wizards: Theme, Source (with search + add-new)

### Phase D — Inventory detail pages (backend + UI)
1. `KnittingYarnDetail.razor` — lots, remaining length/weight, purchase history, active/history projects
2. `KnittingToolDetail.razor` — sizes available, patterns referencing tool, project dropdowns
3. `KnittingNotionDetail.razor` — options available, bulk split UX, patterns referencing notion
4. ApiClient: `GetModuleYarnByIdAsync` (server route exists), tool/notion by-id if missing
5. Inventory list filters (fiber tag Synthetic/Natural/Blended; sort by brand, theme, etc.)

### Phase E — Settings & reference data (PPT slide 2)
1. Module settings UI for Themes, Colors, Sources, Brands, Fiber Type, Terms (or link to core reference admin)
2. Wire Theme/Source into pattern and project forms
3. Terms/glossary surface (even a read-only panel counts for parity)

### Phase F — Kit workspace parity
1. Kit detail layout mirrors project workspace (timers, stitch stats, piece progress) — today kits are planning-only
2. Kit archive / complete flow
3. Kit wizard supplies step: text-only option per deck (optional alongside inventory picker)

---

## Page-by-page checklist

### Home — `KnittingDashboard.razor` (PPT slide 1)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| H1 | Show most recently opened project name | ❌ | Dashboard is static nav cards only |
| H2 | Show image of most recent project | ❌ | No project fetch; no hero image |
| H3 | “Open Most Recent Project” primary action | ❌ | Generic “Open Projects” link only |
| H4 | Quick nav: Patterns, Yarns, Tools, Notions, Kits, Settings | 🟡 | Patterns, Inventory (combined), Kits, Settings — no separate Yarns/Tools/Notions links |
| H5 | Archive entry point | ❌ | No archive shortcut on home |
| H6 | Terms link | ❌ | Not on dashboard |
| H7 | Crochet/Knit filter | ➖ | Module-scoped |

**Target files:** `KnittingDashboard.razor`, possibly shared “recent project” service

---

### Settings — `KnittingSettings.razor` (PPT slide 2)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| S1 | Themes management | ❌ | Only behavior defaults today |
| S2 | Colors (reference list) | ❌ | |
| S3 | Sources (purchase sources) | ❌ | Source is free text on inventory forms |
| S4 | Brands (reference list) | ❌ | Brand is free text |
| S5 | Fiber Type list | 🟡 | `fiber-tag` reference via inventory add-new only |
| S6 | Terms (glossary) | ❌ | |
| S7 | Module behavior defaults | ✅ | Projects, kits, workspace, timers — beyond PPT but valuable |

**Blocking:** Core/module reference category UI or knitting settings expansion for PPT categories.

---

### Project list — `KnittingProjects.razor` (PPT slide 3)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| PL1 | Card grid with image + title | 🟡 | Cards without images |
| PL2 | Default sort: most recent | ❌ | API order only; no “last opened” |
| PL3 | Sort/filter: theme | ❌ | Theme not on project cards |
| PL4 | Sort/filter: main color | ❌ | |
| PL5 | Sort/filter: difficulty | ❌ | Difficulty shown but not filterable |
| PL6 | Sort/filter: completion % | ❌ | Progress shown but not filterable |
| PL7 | Sort/filter: complete vs incomplete | 🟡 | Archived toggle only |
| PL8 | Crochet/Knit type filter | ➖ | Module-scoped |
| PL9 | “New Project” entry | 🟡 | Inline form, not wizard |
| PL10 | Show archived | ✅ | Setting-backed default |

---

### Project detail — `KnittingProjectDetail.razor` (PPT slide 4)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| PD1 | Step checkboxes update progress | ✅ | Per-step done toggles |
| PD2 | Stitch count rolls into completed total | ❌ | Stitch counts on steps exist; totals not aggregated |
| PD3 | Completion % from stitches | ❌ | Progress is step-based (`Progress%`) |
| PD4 | Row ranges in `5–7` format | 🟡 | Range stored/displayed; not compact multi-checkbox UX |
| PD5 | Expandable / scrollable pattern box | 🟡 | List layout; no expand-to-full-pattern |
| PD6 | Timer play/pause | ✅ | Per-step timers |
| PD7 | Total time on piece | 🟡 | Project total + per-step; not per-piece total in sidebar |
| PD8 | Project image | ❌ | Assets panel exists but no header thumbnail |
| PD9 | Title, theme, main color in header | 🟡 | Title/description; no theme/color |
| PD10 | Multi-piece selector | ✅ | Piece sidebar |
| PD11 | Per-piece completion % | ❌ | Step count only in sidebar meta |
| PD12 | Per-piece time display | ❌ | |
| PD13 | Complete → archive (stay on page) | ✅ | Archive without navigation away |
| PD14 | Linked supplies | ✅ | Typed inventory links |
| PD15 | Project assets | ✅ | Upload/attach |
| PD16 | Difficulty colour-coded (6 levels) | ❌ | Enum select in form; no colour chips |

---

### New project wizard (PPT slides 5–9)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| NP1 | Step 1: pattern type filter (Crochet/Knit) | ➖ | Module-scoped |
| NP2 | Step 1: form filter (2D/3D) | ❌ | |
| NP3 | Step 1: theme filter + search + add-new | ❌ | `ThemeId` on pattern entity; no UI |
| NP4 | Step 1: pattern dropdown + search + add-new | 🟡 | Simple pattern select on list page |
| NP5 | Step 2: row/rnd + stitch count entry | ❌ | No inline pattern authoring in project flow |
| NP6 | Step 2: Add Row, New Piece, Next Step | ❌ | |
| NP7 | Step 3: yarn/tool/notion dropdowns + add-new | 🟡 | Supply links on detail only after create |
| NP8 | Step 4: image, title, difficulty colour picker | 🟡 | Difficulty on inline form; no image step |
| NP9 | Finalize: summary card, supply checkboxes, Start Project | ❌ | |
| NP10 | Date started (auto or manual) | ❌ | |

**Target:** New `KnittingProjectWizard.razor` or routed multi-step component; reuse pattern/project APIs.

---

### Pattern list — `KnittingPatterns.razor` (PPT slide 10)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| PT1 | Card grid with image + name | 🟡 | No card images |
| PT2 | Default sort: date added | ❌ | Name order from API |
| PT3 | Sort/filter: theme, color, source, difficulty | ❌ | |
| PT4 | Crochet/Knit filter | ➖ | Module-scoped |
| PT5 | Inline create | ✅ | Not wizard |
| PT6 | Supply metadata on create | ✅ | Yarn weight, needles, notions fields |

---

### Pattern detail — `KnittingPatternDetail.razor` (PPT slides 11–13)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| PV1 | Pattern name, source, theme, difficulty | 🟡 | Name + type/form/difficulty text; source/theme not in UI |
| PV2 | Suggested yarn weight, needle sizes, notions | ✅ | Metadata form |
| PV3 | Pattern image | 🟡 | Assets panel; no hero image |
| PV4 | Scrollable pattern steps | 🟡 | Step list; not scroll-box styled per deck |
| PV5 | Multi-piece dropdown / X of X pieces | 🟡 | Sidebar piece list; no X/X counter |
| PV6 | Piece/step CRUD + reorder | ✅ | |
| PV7 | Stitch count on steps | ✅ | |
| PV8 | Pattern supplies list | ✅ | With inventory picker |
| PV9 | Pattern yarn supply as text-only option | 🟡 | Inventory picker OR manual name (deck allows text for yarn on patterns) |
| PV10 | Add-new pattern flow (slides 12–13) | ❌ | No dedicated wizard; use detail after create |

---

### Yarn inventory (PPT slides 14–17)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| YL1 | Card grid with brand/color/fiber image | 🟡 | Text cards; asset attach on select only |
| YL2 | Filter: Synthetic / Natural / Blended | ❌ | Fiber tag on create only |
| YL3 | Sort/filter: theme, color, source, brand, fiber | ❌ | Search text only |
| YL4 | Add yarn form (brand, color, weight, lot, etc.) | ✅ | Tab form |
| YL5 | Fiber tag dropdown | ✅ | + add-new |
| YL6 | Sale price flag | ✅ | |
| YL7 | Auto-merge brand + color | ✅ | Backend merge on create |
| YL8 | Add / Finish buttons (multi-add flow) | 🟡 | Single “Add / Merge”; no Finish return navigation |
| YV1 | **View Yarn** detail page | ❌ | No `KnittingYarnDetail.razor` |
| YV2 | Skeins, estimated remaining length | 🟡 | Totals on list card; no weight→length UI |
| YV3 | Enter new weight (remaining skein) | ❌ | Backend fields exist; no UI |
| YV4 | Lots dropdown → lot detail | ❌ | Lots in DTO; no lot UI |
| YV5 | Projects Active / History dropdowns | ❌ | Needs reverse links from inventory |
| YV6 | Purchase history list | 🟡 | “Latest purchase” summary on card only |
| YV7 | Lot detail page (slide 16) | ❌ | |

**Blocking:** `GetModuleYarnByIdAsync` in ApiClient; project↔yarn reverse lookup APIs for dropdowns.

---

### Tool inventory (PPT slides 18–20)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| TL1 | Card grid with image | 🟡 | Same as yarn |
| TL2 | Filter/sort by type, brand, size | ❌ | Search only |
| TL3 | Add tool form | ✅ | |
| TL4 | Auto-merge brand + type | ✅ | Backend |
| TV1 | **View Tool** detail page | ❌ | |
| TV2 | Sizes available (variants) | ❌ | |
| TV3 | Patterns suggesting this tool | ❌ | Reverse lookup |
| TV4 | Projects Active / History | ❌ | |
| TV5 | Purchase history | 🟡 | Summary on card |

---

### Notion inventory (PPT slides 21–27)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| NL1 | Card grid with image | 🟡 | |
| NL2 | Filter/sort by type, brand, size, color | ❌ | |
| NL3 | Add notion form | ✅ | |
| NL4 | Multi size/color → split listings dialog | ❌ | Deck slides 23, 27 |
| NV1 | **View Notion** detail page | ❌ | |
| NV2 | Options available (bulk variants) | ❌ | |
| NV3 | Patterns suggesting this notion | ❌ | |
| NV4 | Projects Active / History | ❌ | |

---

### Kit list — `KnittingKits.razor` (PPT slide 28)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| KL1 | Card grid with image + title | ❌ | List buttons, not image cards |
| KL2 | Sort/filter like project list | ❌ | Archived toggle only |
| KL3 | Kit type filter (Crochet/Knit) | ➖ | Module-scoped |
| KL4 | Create kit | ✅ | Inline form |

---

### Kit detail / workspace (PPT slide 29)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| KD1 | Same layout as project detail | ❌ | Planning panel: pieces + supplies only |
| KD2 | Per kit-piece timers & stitch stats | ❌ | Pieces link to projects; no unified kit workspace |
| KD3 | Kit complete → archive | ❌ | Kit archive may exist backend-side; no complete UX |
| KD4 | Kit piece → create/open project | ✅ | |
| KD5 | Supply inventory picker | ✅ | Recent addition |
| KD6 | Kit supplies text-only mode (deck preference) | 🟡 | Inventory optional; manual name still allowed |

---

### New kit wizard (PPT slides 30–34)

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| NK1 | Step 1: pattern/theme/type filters | ❌ | |
| NK2 | Step 2: pattern steps + New Kit Piece | ❌ | Piece name/notes only on detail |
| NK3 | Step 3: text-only supplies | 🟡 | Optional inventory link added |
| NK4 | Step 4: image, difficulty colours | ❌ | |
| NK5 | Finalize + Start Project | ❌ | |

---

## Cross-cutting gaps

| # | Requirement | Status | Notes |
|---|-------------|--------|-------|
| X1 | Card thumbnails everywhere | ❌ | Assets exist; list cards don’t render them |
| X2 | Theme / Source on patterns & projects | 🟡 | Entity + API; knitting UI doesn’t expose |
| X3 | Main color field | ❌ | Not on knitting pattern/project forms |
| X4 | Difficulty colour coding (6 levels) | ❌ | Values exist; no visual system |
| X5 | Search on pattern picker | ❌ | `SearchModulePatternsAsync` may exist — not used in forms |
| X6 | Pagination on long lists | ❌ | All lists load first page/default batch |
| X7 | Last-opened / recent tracking | ❌ | Needed for home hero and project sort |

---

## Backend/API gaps blocking UI parity

These are not visible in Blazor but will block checklist items until addressed:

| API / behavior | Unblocks |
|----------------|----------|
| `GetModuleYarnByIdAsync` (+ tool/notion by id) in ApiClient | View Yarn/Tool/Notion pages |
| Inventory item → active/archived projects | Projects Active/History dropdowns |
| Inventory item → referencing patterns | Patterns panel on tool/notion detail |
| Project stitch rollup (completed/total stitches, %) | PD2–PD3, kit workspace |
| Per-piece timer/stitch aggregates | PD11–PD12, KD2 |
| `lastOpenedAt` or client recent-project tracking | Home hero, PL2 |
| Theme/Source CRUD or core reference picker endpoints in knitting UI | Wizards, filters, PV1 |
| Kit workspace timers/progress (or aggregate from child projects) | KD1–KD2 |

---

## Verification harness (for parity sign-off)

When implementing phases, use this smoke path:

1. **Home** — recent project appears with image after opening a project
2. **Project wizard** — create from existing pattern with supplies and image; lands in workspace
3. **Workspace** — checking steps updates stitch-based %; per-piece stats visible; timer works
4. **Pattern** — full metadata, supplies, image; pieces/steps match deck
5. **Inventory** — add yarn with lot; open yarn detail; see purchase history and project links
6. **Kit** — wizard create; kit workspace or child projects show progress; archive kit

---

## Related files

| Area | Primary UI | Primary API |
|------|------------|-------------|
| Dashboard | `KnittingDashboard.razor` | `GetModuleProjectsAsync` |
| Projects | `KnittingProjects.razor`, `KnittingProjectDetail.razor` | `ModuleProjectsController` |
| Patterns | `KnittingPatterns.razor`, `KnittingPatternDetail.razor` | `ModulePatternsController` |
| Inventory | `KnittingInventory.razor` | `ModuleInventoryController` |
| Kits | `KnittingKits.razor` | `ModuleKitsController` |
| Settings | `KnittingSettings.razor` | `ModuleSettingsController` |
| Assets | pattern/project/inventory pages | `api/assets/*` |

---

*Update this checklist as phases complete. Prefer checking items only when behavior is verifiable in the running app, not merely when a control exists.*
