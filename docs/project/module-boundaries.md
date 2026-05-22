# TankerMade Module Boundaries

Last reviewed: 2026-05-22

TankerMade is a modular maker workbench. The base application provides the shared workbench shell and stable extension points; individual maker domains are supplied by modules.

The base program should not expose craft workflows by itself. On a fresh install, a user can sign in, manage core settings, and choose which modules to load, but they cannot create a knitting project, crochet pattern, 3D print job, or any other craft-specific workflow until a module supplies that functionality.

## Startup Model

The intended startup flow is:

1. User opens TankerMade.
2. Core shell loads authentication, settings, module discovery, and module selection.
3. If no modules are active, the app prompts the user to choose one or more modules.
4. User selects modules, for example `Knitting`.
5. TankerMade loads module-provided navigation, entities, API endpoints, services, validation, reference data, and UI.
6. The user can now work with the selected module's domain-specific content.

## Base Application

The base app owns platform concepts that apply before any maker module is selected:

- Users, roles, authentication, and authorization
- Module registration, discovery, activation, dependency validation, and UI extension points
- Application shell, navigation slots, settings, persistence, migrations, export/import, and backup
- Neutral attachment/asset storage primitives that modules can attach to their own records
- Optional generic labels/tags/reference lists only when they are useful across modules

Base entities should avoid hard-coded craft taxonomies. When a value varies by maker domain, store it as module-defined data or configurable reference data rather than a Core enum.

## Modules

Modules own domain-specific language, validation, workflow, and UI:

- Project, pattern, inventory, kit, timer, and progress concepts when their behavior is craft-specific
- Crochet and knitting pattern types, forms, tools, supplies, gauges, stitch vocabulary, and yarn-specific inventory behavior
- 3D printing material types, slicer settings, printer profiles, filament/spool behavior, print jobs, and calibration flows
- Any future craft or maker domain that needs specialized fields or screens

Modules may extend the shell, but should not require base Core to reference module assemblies or craft-specific enum values.

## Phase A Checkpoint

Phase A proved the module-host boundary with a bundled `Crafting` reference module:

- The base shell exposes module discovery and activation without built-in craft workflows.
- Crafting-specific project/pattern functionality lives in `TankerMade.Modules.Crafting`.
- Crafting APIs are gated by per-user module activation.
- Crafting navigation appears in the client only after activation.
- Pattern/project CRUD and cross-user ownership scoping were manually smoke-tested.

## Module Boundary Guardrails

- Keep `TankerMade.Core` dependency-light and craft-agnostic.
- Build the core program as a module host first, not as a built-in crafting app.
- Include the first crafting module in Phase A as a reference implementation so the module host is proven against a real module.
- Extract existing project/pattern foundation into that crafting module rather than continuing it in the base host.
- Prefer strings or configurable/reference records for early values that modules will later own.
- Seed only neutral base reference data in `TankerMadeDbContext`.
- Put craft-specific seed data in the future crafting module, not the base migration.
- Do not add a repository abstraction unless the roadmap calls for it later; services can use `TankerMadeDbContext` directly for now.
