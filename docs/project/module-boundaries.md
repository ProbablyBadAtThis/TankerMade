# TankerMade Module Boundaries

Last reviewed: 2026-05-26

TankerMade is a modular maker workbench. The base application provides the shared workbench shell and stable extension points; individual maker domains are supplied by modules.

The base program should not expose craft workflows by itself. On a fresh install, a user can sign in, manage core settings, and choose which modules to load, but they cannot create a knitting project, crochet pattern, 3D print job, or any other craft-specific workflow until a module supplies that functionality.

Core must remain useful and internally coherent without any particular domain module installed or active. Removing a module should not break authentication, settings, module management, the shell, neutral platform services, or unrelated active modules. Module records may become unavailable or orphaned according to future uninstall/export rules, but Core should not require a domain assembly, domain enum, or domain service to boot and operate.

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

Core APIs should be foundational rather than domain-shaped. They may expose authenticated user context, module activation checks, shared persistence hooks, shell/navigation extension points, neutral reference-data extension points, and future asset/export primitives. They should not expose yarn, tools, notions, filament, spools, printers, slicer settings, or any other domain-specific concept as a Core contract.

## Modules

Modules own domain-specific language, validation, workflow, and UI:

- Project, pattern, inventory, kit, timer, and progress concepts when their behavior is craft-specific
- Crochet and knitting pattern types, forms, tools, supplies, gauges, stitch vocabulary, and yarn-specific inventory behavior
- 3D printing material types, slicer settings, printer profiles, filament/spool behavior, print jobs, and calibration flows
- Any future craft or maker domain that needs specialized fields or screens

Modules may extend the shell, but should not require base Core to reference module assemblies or craft-specific enum values.

Modules should be able to shape their own sections by using Core extension points rather than changing Core for each niche. A future woodworking, electronics, painting, leatherwork, model-building, or other maker module should be able to define its own inventory, project, kit/grouping, reference data, and workflows without inheriting assumptions from knitting/crochet or 3D printing.

## Phase D Boundary Target

Phase D should prove module-owned domain behavior while keeping physical packaging and external distribution for later. In practical terms, Phase D should make modules architecturally real:

- Module-owned inventory entities, services, endpoints, DTOs, UI routes, filters, and reference data.
- Module-owned project/inventory links and kit/grouping workflows.
- Activation gating around module-owned APIs and UI.
- At least one second-domain pressure test, such as a thin 3D printing inventory slice, so the shared pattern does not accidentally become craft-shaped.

Phase D should not attempt to solve the future module store, licensing, installable package distribution, or full external module loading model. Those concerns belong to Phase F or later, after the module-owned domain shape has been proven.

## Phase A Checkpoint

Phase A proved the module-host boundary with a bundled `Crafting` reference module:

- The base shell exposes module discovery and activation without built-in craft workflows.
- Crafting-specific project/pattern functionality lives in `TankerMade.Modules.Crafting`.
- Crafting APIs are gated by per-user module activation.
- Crafting navigation appears in the client only after activation.
- Pattern/project CRUD and cross-user ownership scoping were manually smoke-tested.

## Reference Module vs. Golive Modules

`TankerMade.Modules.Crafting` is the reference/template module used to prove platform behavior and provide an implementation pattern for future modules. It may include neutral sample workflows such as patterns, pieces, steps, ordering, and progress aggregation, but it should stay niche-neutral.

Production golive modules should be specific maker domains, for example knitting, crochet, sewing, quilting, or 3D printing. Domain-specific language, validation, supplies, progress rules, and UI belong in those dedicated modules rather than being folded into the reference Crafting module.

## Module Boundary Guardrails

- Keep `TankerMade.Core` dependency-light and craft-agnostic.
- Build the core program as a module host first, not as a built-in crafting app.
- Core should boot and remain usable if any individual domain module is deleted, disabled, or absent.
- Do not add Core references to module assemblies, module-specific services, or module-specific enums.
- Include the first crafting module in Phase A as a reference implementation so the module host is proven against a real module.
- Extract existing project/pattern foundation into that crafting module rather than continuing it in the base host.
- Keep the Crafting module copyable as a module template; avoid knitting, crochet, sewing, or other niche-specific behavior unless it is intentionally neutral sample behavior.
- Prefer strings or configurable/reference records for early values that modules will later own.
- Seed only neutral base reference data in `TankerMadeDbContext`.
- Put craft-specific seed data in the future crafting module, not the base migration.
- Do not add a repository abstraction unless the roadmap calls for it later; services can use `TankerMadeDbContext` directly for now.
