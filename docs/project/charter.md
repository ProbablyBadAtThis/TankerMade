# TankerMade — Project Charter
Source: project-artifact.md (original project brief, preserved content)
Last reviewed: 2025-05-17 — architecture section updated; vision/goals/principles retained verbatim.

---

## Subject

A craft project management workbench focused on repeatable workflows: projects, patterns (pieces/steps), timers, inventory, and gamification (XP, streaks, achievements).

Built on .NET 10 Blazor WASM (client) + ASP.NET Core (local server) + EF Core (SQLite). Designed to be extensible via a module system that adds craft-specific fields, flows, and UI — crafting is the first module, 3D printing is planned as the second.

---

## Vision

- Help makers manage small-to-medium craft projects efficiently, track progress, and maintain inventory with minimal friction.
- Provide a platform that grows into a modular ecosystem supporting multiple maker domains.
- Balance approachability and power: a simple, locally-hosted app with offline capability and optional local network sharing.
- Similar deployment model to Calibre — runs as a local server, client points to localhost, optionally shared on LAN.

---

## Core Goals (MVP)

- **Projects:** CRUD, detail view, per-piece timers, archive.
- **Patterns:** CRUD with pieces and steps, reordering, stitch count tracking, validation.
- **Inventory:** CRUD for yarn, tools, and notions; lot tracking; linkage to projects.
- **Identity:** Cookie/JWT-based auth, role-gated features (viewer, editor, admin).
- **Admin:** JSON export/import, DB backup, migrations management.
- **APIs:** REST endpoints mirroring service capabilities.

---

## Extended Goals (Post-MVP)

- Module platform (V1): discovery, contracts, UI extension points; example module (crafting built-in, 3D printing as first external).
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
- Core is craft-agnostic; all craft-specific logic lives in modules.

---

## Success Criteria

- **Reliability:** CI build/test pass rate ≥ 95%; critical bug rate low and trending downward.
- **Usability:** Users complete common flows (project create/edit, pattern editing, inventory updates) without errors.
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

- Unit tests: service logic (Projects, Patterns, Inventory, ReferenceData).
- Integration tests: API endpoints and admin operations; export/import round-trip.
- Component tests: critical UI flows (auth gating, forms, busy/error states).
- CI: GitHub Actions workflow for restore/build/test on push/PR.

---

## Glossary

- **Piece:** A named section of a pattern (e.g. "Body", "Left Sleeve"). Has many Steps.
- **Step:** A single row/round/instruction within a piece, with a stitch count.
- **Module:** A Razor Class Library that adds craft-specific entities, UI, and logic to the core app.
- **Kit:** A bundle of multiple pattern pieces treated as a single project grouping.
- **Export/Import:** Admin operations for JSON backup and full replace-restore.
- **Lot:** A yarn batch number; different lots of the same yarn may have slight color variation.
