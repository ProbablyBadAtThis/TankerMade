# TankerMade Agent Guide

This file is the short operational guide for AI assistants working in this repo. Keep it focused on how to work safely and where to find the deeper product context.

## Read First

- Project charter: `docs/project/charter.md`
- Roadmap and current phase: `docs/project/roadmap.md`
- Current handoff: `docs/project/handoff.md`
- UX reference: `docs/product/ux-reference.md`
- Original UI idea deck: `docs/product/ui-feature-ideas.pptx`

Local, ignored scratch material may exist in `Scratch/`. Treat it as temporary or session-specific unless the user says otherwise.

## Project Shape

TankerMade is a local-first modular maker workbench. The primary program is a core host/shell; craft and maker workflows are supplied by separately loaded modules. Crafting should be an installable/loadable module, and 3D printing should be another module.

Current stack:

- Client: Blazor WebAssembly on .NET 10
- Server: ASP.NET Core API on .NET 10
- Database: SQLite via EF Core 10
- API docs: Scalar
- Auth: JWT bearer tokens with BCrypt password hashing

Solution layout:

- `src/TankerMade.Core`: domain entities and enums; avoid external package dependencies.
- `src/TankerMade.Contracts`: DTOs and service interfaces; references Core only.
- `src/TankerMade.Application`: service implementations; references Core and Contracts.
- `src/TankerMade.Server`: ASP.NET Core API, EF Core, auth, migrations.
- `src/TankerMade.Client`: Blazor WASM UI; references Contracts only.
- `page/`: Cloudflare Pages/dev tracker/docs site area.

## Working Principles

- Keep namespaces under `TankerMade.*`.
- Prefer direct `DbContext` injection in services for now; do not add a repository abstraction unless there is a clear need.
- Keep Core craft-agnostic. Craft-specific fields, workflows, navigation, validation, seed data, and UI live in modules.
- The base program should not expose project/pattern/inventory craft workflows by itself. On first run, it should load the shell and ask which module(s) to activate.
- Keep changes small, focused, and aligned with the roadmap.
- Read files before editing them.
- Do not commit local databases, Finder metadata, build outputs, or `Scratch/` content.
- Before deployment-oriented work, move secrets out of committed config and into user-secrets or environment variables.

## Common Commands

Start the API server:

```bash
dotnet run --project src/TankerMade.Server
```

Start the API server with HTTPS:

```bash
dotnet run --project src/TankerMade.Server --launch-profile https
```

Start the Blazor client:

```bash
dotnet run --project src/TankerMade.Client
```

Build everything:

```bash
dotnet build TankerMade.sln
```

List migrations:

```bash
dotnet ef migrations list --project src/TankerMade.Server --startup-project src/TankerMade.Server
```

Add a migration after entity or DbContext changes:

```bash
dotnet ef migrations add <MigrationName> --project src/TankerMade.Server --startup-project src/TankerMade.Server
```

Apply migrations manually:

```bash
dotnet ef database update --project src/TankerMade.Server --startup-project src/TankerMade.Server
```

API docs, with the server running:

```text
http://localhost:5236/scalar/v1
```

## Database Notes

- Local database path: `src/TankerMade.Server/App_Data/tankermade.db`
- The server creates `App_Data/` and applies migrations on startup.
- SQLite database files are ignored by git.
- After adding or changing EF entities, update `TankerMadeDbContext`, add a migration, and build.

## Current Roadmap Focus

Use `docs/project/roadmap.md` as the source of truth. As of the current docs, Phase D focuses on module-owned inventory and kit behavior:

- Keep `TankerMade.Modules.Crafting` as a reference/template module, not the eventual production catch-all for knitting, crochet, sewing, or another niche.
- Phase C module-owned project workspace behavior is complete: step progress, per-step timers, completion rules, piece selection, archive flow, and non-destructive editing.
- Add craft module inventory for yarn, tools, notions, lots, purchase history, and sale price handling.
- Add 3D printing module inventory for materials, spools, printer/tooling needs, and module-specific purchase history.
- Add module-owned project/inventory linking, kit/grouping behavior, and kit/grouping to project flows.

## UX Expectations

Use `docs/product/ux-reference.md` for product behavior. Important themes:

- Project, pattern, inventory, kit, settings, and gamification flows matter.
- Patterns are made of pieces and steps.
- Project progress is tied to checked steps, stitch counts, timers, and per-piece completion.
- Inventory merging rules matter, especially yarn lots and sale prices.
- Difficulty has six levels: Beginner, Beginner+, Intermediate, Intermediate+, Advanced, Advanced+.

## Verification

For code changes, prefer at least:

```bash
dotnet build TankerMade.sln
```

Add or run tests when changing service behavior, API behavior, migrations, or shared contracts.

In Codex sandbox sessions, solution/server builds can silently hang and fail after several minutes with no useful diagnostics. Do not spend time repeatedly trying sandbox builds. When verification is needed, ask the user to run the build/tests locally and provide the results. Small project-level builds may be used only when they are quick and clearly useful.
