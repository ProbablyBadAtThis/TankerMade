# TankerMade Handoff

Last updated: 2026-06-11

## Pickup summary (new conversation)

**Branch:** `working/knitting-settings-ui-pass`  
**Latest pushed commit:** `5df49e0` — functional parity (Phase K8 `StartedAt`, color filters, inventory depth, pagination, crafter-answer docs).  
**Uncommitted working tree:** crafter-directed **UI pass** (horizontal top nav, dark warm theme, sticky workspace, per-row checkboxes, home “worked on”, settings accordion, reference autocomplete). Client build passes; **not verified in real browser yet**; **not committed**.

| Doc | Purpose |
|-----|---------|
| `docs/project/knitting-ui-parity-checklist.md` | Field-check + verification harness |
| `docs/product/ux-reference.md` | Crafter decisions + wireframe reference |
| `Scratch/ui-discussion/2026-06-11-crafter-answers.md` | Full `#ui-discussion` Q&A |
| `Scratch/ui-discussion/2026-06-11-knitting-verification-ui-backlog.md` | Pre-answer engineering backlog (partially superseded) |

**Do next:** Real-browser UI verification → commit UI pass → knitting sign-off → propagate shell/theme to other modules.

---

## Current State

- Current roadmap phase: Phase I — Security, Ops & Cleanup (complete).
- Knitting module on `working/knitting-settings-ui-pass`: **functional parity complete** (pushed `5df49e0`); **UI pass in working tree** per `#ui-discussion` answers.
- Crafter direction captured: `docs/product/ux-reference.md` § Crafter decisions; `Scratch/ui-discussion/2026-06-11-crafter-answers.md`.
- PPT parity rough score: **~86 ✅ · ~25 🟡 · ~1 ❌ · 6 ➖** — see `docs/project/knitting-ui-parity-checklist.md`.

### Pushed (`5df49e0`) — functional parity

- Phase K7: `ColorId` on patterns/projects; yarn/lot remaining edit APIs.
- Phase K8: project `StartedAt`; project color pickers/filters; wizard finalize + date started.
- Inventory: lot detail route, tool size variants, notion bulk-split, sectioned add forms, list filters + pagination.
- Wizards: inline step authoring (new pattern path), pattern/project/kit first-pass flows.
- Docs synced with crafter answers from Slack `#ui-discussion`.

### Uncommitted — UI pass (crafter-directed)

**App shell (host-wide, not knitting-only):**

- Vertical sidebar → **horizontal top nav** (`MainLayout.razor`, `NavMenu.razor` + CSS).
- **App footer**; **dark warm default theme** + light toggle (`ThemeService`, `wwwroot/js/theme.js`, CSS variables in `app.css`).

**Knitting module:**

- **Home:** last *worked on* hero (`RecordProjectWorkedOnAsync`); **recently viewed** list; **single Inventory** entry (no separate yarn/tools/notions home buttons).
- **Workspace:** sticky chrome (progress %, tracked time, current piece); **per-row checkboxes** (`KnittingRowProgress` + localStorage); timer always on step rows.
- **Settings:** accordion sections; `ReferenceAutocompleteInput` on reference add-new (project wizard theme too).
- **Cards:** larger photo project cards retained (crafter preference).

**New / touched client files:**

- `Layout/MainLayout.razor`, `Layout/NavMenu.razor`, `*.razor.css`
- `Services/ThemeService.cs`, `Services/KnittingRowProgress.cs`, `Services/KnittingRecentActivity.cs` (worked-on + viewed)
- `Components/ReferenceAutocompleteInput.razor`
- `Pages/KnittingDashboard.razor`, `KnittingProjectDetail.razor`, `KnittingSettings.razor`, `KnittingProjectWizard.razor`
- `wwwroot/css/app.css`, `wwwroot/index.html`

### Still thin / not done

- Pattern wizard + inventory reference autocomplete (settings + project wizard theme only today).
- Projects list “last worked on” sort (dashboard hero uses worked-on; list sort still last-opened/updated).
- Wizard slide **layout** fidelity (cosmetic).
- Server-side pagination / `lastOpenedAt` (optional scale).
- Propagate horizontal nav + theme to non-knitting modules.

---

## Next Work

1. **Verify UI pass** in a real browser using harness in `docs/project/knitting-ui-parity-checklist.md` (§ UI pass verification).
2. **Commit + push** uncommitted UI pass when green: `dotnet build TankerMade.sln`.
3. **Polish** (if verification finds gaps): inventory/pattern wizard autocomplete, projects list worked-on sort.
4. After knitting sign-off: propagate shell + theme tokens to other live modules.

---

## Completed Recently (historical context)

- Phases A–I roadmap slices complete per `docs/project/roadmap.md`.
- Crafting remains reference/template module; live modules: Knitting, Crochet, Embroidery, Quilting, Sewing, 3D Printing.
- Knitting K5–K8: project workspace (steps, timers, supplies), settings behavior, full operational inventory/patterns/kits, color/`StartedAt`, UI parity passes.
- Neutral module capability APIs replace legacy craft-specific controllers.
- User verification (earlier passes): home, projects, patterns, workspace, wizards; inventory form layout fix; difficulty picker closure bug fix.

---

## Phase G Direction

- Phase G established module-friendly image/asset primitives; thumbnails and asset picker extension points in place.
- Core remains craft-agnostic host; modules own workflows, UI, and reference data.

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

Run **both** harness sections in `docs/project/knitting-ui-parity-checklist.md`:

1. Functional parity (inventory forms, wizards, filters, etc.)
2. UI pass (nav, theme, sticky workspace, per-row checks, home hero, settings accordion)

Sign in as `member@test.com` → `/modules/knitting`. Use a real browser, not the IDE embedded browser.

Optional re-seed: `dotnet run --project Scratch/knitting-seed/SeedKnittingData.csproj`
