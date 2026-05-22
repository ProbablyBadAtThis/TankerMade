# TankerMade — Project Charter
Source: project-artifact.md (original project brief, preserved content)
Last reviewed: 2026-05-22 — Phase A module-host checkpoint completed; vision/goals/principles retained verbatim.

---

## Subject

The core host for a local-first modular maker workbench. The primary program is a shell for authentication, settings, module discovery/loading, storage, and extension points. Maker workflows such as projects, patterns, pieces/steps, timers, inventory, kits, print jobs, and gamification are supplied by loaded modules.

Built on .NET 10 Blazor WASM (client) + ASP.NET Core (local server) + EF Core (SQLite). Designed around a module system that adds domain-specific fields, flows, data, API behavior, and UI. Knitting/crochet-style crafting is expected to be delivered as a module, and 3D printing is planned as another module.

---

## Vision

- Help makers manage small-to-medium projects efficiently once the appropriate maker module is loaded.
- Provide a platform that grows into a modular ecosystem supporting multiple maker domains.
- Balance approachability and power: a simple, locally-hosted app with offline capability and optional local network sharing.
- Similar deployment model to Calibre — runs as a local server, client points to localhost, optionally shared on LAN.

---

## Core Goals (MVP)

- **Module host:** discovery, activation, persisted module selection, and extension points.
- **Reference module:** a first crafting module extracted from the earlier project/pattern foundation so future modules have a concrete implementation to follow.
- **Empty-state startup:** when no modules are active, prompt the user to choose modules before domain workflows appear.
- **Identity:** Cookie/JWT-based auth, role-gated features (viewer, editor, admin).
- **Admin:** JSON export/import, DB backup, migrations management.
- **APIs:** REST endpoints for core host capabilities and extension points modules can attach to.

---

## Extended Goals (Post-MVP)

- Module platform (V1): discovery, contracts, UI extension points; knitting/crochet as an installable module, 3D printing as another module.
- Images and assets: uploads, thumbnails, image pickers across entities.
- Performance/search: indexes, server-side filtering/paging, optional full-text search.
- Tests and CI/CD: service-level, integration, and component tests; automated pipelines.
- Optional DB providers: Postgres/SQL Server; portable packaging and deployment guidance.
- Mobile: PWA first; native wrapper (Capacitor) or dedicated client later if needed.

---

## Non-Goals (for now)

- Full client-side-only WASM runtime without a server — focus is server + offline toggle.
- Complex multi-tenant SaaS features — single-tenant or small-team deployments only.
- Public OAuth/OIDC integrations — JWT with roles and local accounts for now.
- App Store distribution — PWA covers the mobile story initially.

---

## Primary Users (Personas)

- **Maker:** Tracks projects and inventory daily; benefits from timers and gamification.
- **Editor:** Creates and updates content; manages patterns and inventory.
- **Admin:** Performs backups, imports, migrations; manages sensitive operations.

---

## Operating Principles

- Favor simple, well-understood patterns: EF services via direct DbContext injection; REST APIs for parity with service logic.
- Harden essentials: auth gating, error boundaries, robust state persistence.
- Keep namespaces consistent: TankerMade.* across all projects.
- Write clean, minimal code; avoid over-engineering.
- Core is craft-agnostic; all craft-specific logic and workflows live in modules.
- The base program is not a usable craft app by itself; it becomes useful for a maker domain only after one or more modules are loaded.
- Module boundary details are tracked in `docs/project/module-boundaries.md`.

---

## Success Criteria

- **Phase A checkpoint:** users can register/login, activate/deactivate the bundled Crafting module, use module-owned pattern/project CRUD, and see Crafting navigation appear only after activation.
- **Reliability:** CI build/test pass rate ≥ 95%; critical bug rate low and trending downward.
- **Usability:** Users can discover, activate, and enter module-provided workflows without confusion.
- **Data integrity:** Export/import round-trips without loss; migrations apply cleanly on new and existing databases.
- **Extensibility:** Module platform supports one example module with documented extension points.
- **Performance:** Operations on medium datasets (5–50k inventory items) remain responsive with server-side filtering.

---

## Risks and Mitigations

- **Namespace/path drift:** Standardise on TankerMade.*; enforce via CI.
- **Data growth:** Add indexes and filters early; monitor performance; plan for optional DB providers.
- **Security pitfalls:** Keep demo/dev endpoints out of production; document JWT settings and data protection.
- **Scope creep:** Maintain a clear MVP; defer nice-to-haves to later phases with explicit non-goals.

---

## Testing Strategy

- Unit tests: core host service logic and module registration/activation behavior.
- Integration tests: core API endpoints, module activation, admin operations, export/import round-trip.
- Component tests: critical UI flows (auth gating, module selection, busy/error states).
- CI: GitHub Actions workflow for restore/build/test on push/PR.

---

## Glossary

- **Module:** A loadable package, likely a Razor Class Library, that adds domain-specific entities, API behavior, UI, validation, reference data, and workflows to the core app.
- **Piece:** A module-defined section of a pattern or project, when that module has such a concept.
- **Step:** A module-defined instruction/progress item, when that module has such a concept.
- **Kit:** A module-defined grouping, when that module has such a concept.
- **Export/Import:** Admin operations for JSON backup and full replace-restore.
- **Lot:** A yarn batch number; different lots of the same yarn may have slight color variation.
