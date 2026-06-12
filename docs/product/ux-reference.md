# TankerMade — UX Wireframes Reference
Source: YarnProject.pdf (original hand-sketched wireframe spec)
Status: REFERENCE — initial module UI scaffolding and first-pass styling now exist in app; this file remains the source for deeper UX behavior and visual refinement targets.
Last reviewed: 2026-06-11

## Crafter decisions (#ui-discussion, 2026-06-11)

Source: TankerMunk answers in Slack `#ui-discussion`; full capture in `Scratch/ui-discussion/2026-06-11-crafter-answers.md`.

**Feel & theme:** Cozy notebook; calm with some color; **warm tones**; **dark mode default**, light mode optional; keep **6-level difficulty color coding**.

**Home:** Hero = **most recently worked-on** project (progress/timer/check activity, not view-only opens). Optional smaller **recently viewed** list. **Single Inventory** entry on home (yarn/tools/notions chosen inside inventory, not three home buttons).

**Project workspace:** Pattern steps may scroll; **timer + progress % stay pinned/visible**. **Checkbox per row** even for compact ranges like `5–7`. Timer **always visible**. End session = **quiet save** only (no summary modal).

**Cards & layout:** **Large photo cards** for projects (Pinterest-style, not compact rows). Settings = **folding/accordion sections**. Avoid horizontal scroll at half-width.

**Editing patterns:** **Inline** step edits on pattern detail (not modal-first).

**Adding yarn:** **One form** on inventory page (not a multi-step wizard).

**Reference add-new:** **Autocomplete while typing** for themes/brands/types to surface existing names and reduce duplicates.

**Must-keep from deck:** Project photos, timer, per-row step checkboxes. Nothing willingly dropped.

**Dealbreakers:** Lost progress, unreliable save, slow project page loads.

**Revised vs pre-answer engineering backlog:** Deprioritize modal-first pattern/inventory edits and aggressive card compaction; prioritize sticky workspace chrome, per-row checkboxes, dark+warm theme, home “worked on” semantics, settings accordion, reference autocomplete.

The PDF is attached to the Claude Project. Do not remove it — it is the primary UX specification.

---

## Screens Documented in the PDF

### Home Screen
- Shows image + name of most recently opened project
- Quick navigation: Patterns, Yarns, Tools, Notions, Kits, Settings
- Open Most Recent Project button

### Project List
- Card grid (image + title), sorted by most recent by default
- Filter toggles: Crochet / Knit pattern type
- Sort/filter by: pattern theme, main color, difficulty, completion %, complete/incomplete

### Project Detail (View Project)
- Pattern display: rows/rounds listed with checkboxes; ticking adds stitch count to completed total
- Repeated rows shown in "5–7" format — individual checkbox per row preferred
- Timer: play/pause, shows total time spent on piece
- If pattern has multiple pieces: piece selector showing piece names; pattern window shows selected piece
- Per-piece completion % and time display
- Complete button: moves to archive even if not 100% complete

### Adding New Project (4-step wizard)
1. Pattern selection: type (Crochet/Knit), form (2D/3D), theme, pattern dropdown with search + Add New
2. Pattern steps entry: row/round + stitch count, Add Row, New Piece (saves current, prompts for piece name), Next Step
3. Supplies: yarn, tools, notions dropdowns with Add New
4. Metadata: image, project title, difficulty (colour-coded: Beginner → Advanced+)
5. Finalisation: summary card with all details, Start Project button, date started (auto or manual)

### Pattern Inventory
- Card grid (image + name), sorted by date added
- Filter toggles: Crochet / Knit
- Sort/filter by: theme, color, source, difficulty

### View Pattern
- Name, source, theme, difficulty, suggested yarn weight, suggested needle/hook sizes, required notions
- Pattern steps in scrollable box
- If multiple pieces: dropdown to switch which piece's steps are shown; X/X piece count

### Adding New Pattern
- Same step entry flow as project wizard step 2
- Piece naming, multi-piece support
- Supply list: yarn (no dropdown — text only for patterns), tools, notions

### Yarn Inventory
- Card grid: brand/color/fiber image
- Filter toggles: Synthetic / Natural / Blended (based on fiber tag)
- Sort/filter by: theme, color, source, brand, fiber

### View Yarn
- Brand, color name, main color, weight, fiber, image
- Number of skeins, estimated remaining length, enter new weight input
- Lots dropdown (opens lot detail)
- Projects Active dropdown → opens project page
- Project History dropdown → opens archived project page
- Purchase history: source + price per source

### View Yarn Lots
- Per-lot tracking: lot number, skeins, remaining length, weight entry
- Same estimated-length calculation as main yarn view but scoped to lot

### Adding New Yarn
- Brand, type, color name, weight, length, lot number, source, fiber, main color, price
- Fiber tag: Synthetic / Natural / Blended (for filtering only)
- Sale price flag: if marked as sale, does NOT update existing source price
- Add button: saves and clears for another entry
- Finish: returns to list or supply page
- Auto-merge: if new yarn matches existing brand + color name, adds to existing inventory and appends purchase history

### Tool Inventory
- Card grid: type/brand/size
- Filter/sort by: type, brand, size

### View Tool
- Brand, type, size, description, image
- Sizes available (if multiple sizes of same tool exist)
- Projects Active / History dropdowns
- Purchase history
- Patterns that suggest/require this tool
- Auto-merge: same brand + type = add to existing, update purchase history

### Adding New Tool
- Brand, type, size, description, source, price
- Sale price flag
- Add / Finish buttons

### Notion Inventory
- Card grid: type/brand
- Filter/sort by: type, brand, size, color

### View Notion
- Brand, type, size, description, image
- Options available (if added as bulk with multiple sizes/colors)
- Projects Active / History dropdowns
- Purchase history
- Patterns that suggest/require this notion

### Adding New Notion
- Brand, type, size(s), color(s), description, source, price
- Multiple sizes/colors: option to split into individual listings
- If splitting: associate each size to a specific color
- Sale price flag

### Kit List
- Card grid, same layout as Project List
- Filter: Crochet / Knit kit type
- Sort/filter: theme, color, difficulty, completion %, complete/incomplete

### View Kit
- Same layout as Project Detail
- Kit pieces shown (each piece = a sub-project within the kit)
- Per-piece timer, stitch count, completion %
- Complete button: archives kit

### Adding New Kit (4-step wizard)
- Same flow as Adding New Project but:
  - Step 2 has "New Kit Piece" button in addition to "New Pattern Piece"
  - Supplies step: text boxes only (no dropdowns that open other screens)
  - Kit pieces each get their own pattern entry

### Settings
- Themes (colour themes for UI)
- Colors (reference list)
- Sources (purchase source list)
- Brands (brand list)
- Fiber Type (fiber type list)
- Terms (glossary/custom terminology)

---

## Key UX Notes (from PDF annotations)

- Difficulty is colour-coded with 6 levels: Beginner, Beginner+, Intermediate, Intermediate+, Advanced, Advanced+
- Yarn lot numbers track which batch yarn was processed in — different lots can have colour variation even within the same colourway
- When adding yarn that matches an existing brand + color name, it should auto-merge into the existing inventory entry rather than creating a duplicate
- Sale prices should NOT overwrite existing source prices when merging into an existing inventory item
- Pattern steps that repeat (e.g. rows 5–7 are identical) should be shown in "5–7" compact format with individual checkboxes still preferred
- Piece order in patterns/projects is determined by the order pieces were entered during creation
- Kit supplies entry intentionally has no dropdowns opening other screens — keep it simple since most kit items won't carry over
