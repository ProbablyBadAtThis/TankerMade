# Module Inventory Pattern

Last reviewed: 2026-05-26

Phase D proves module-owned inventory behavior while Core remains an independent host. Inventory records, validation, merge rules, purchase history, filters, and project links belong to the module that understands the domain.

## Ownership Rules

- Core should not define domain inventory concepts such as yarn, notions, tools, filament, spools, printers, or slicer settings.
- Module inventory endpoints must be gated by module activation and scoped to the authenticated user.
- Module inventory services should use `TankerMadeDbContext` directly for now, matching the existing Phase A-C service pattern.
- Shared shell/reference concepts may be used only when they are genuinely neutral. Module-specific reference values belong to the module.

## Proven Shapes

The first Phase D inventory slice uses `TankerMade.Modules.Crafting` to prove yarn inventory:

- `CraftingYarnInventoryItem` owns the user-scoped yarn aggregate.
- `CraftingYarnLot` tracks lot-specific quantities and remaining length.
- `CraftingInventoryPurchase` appends purchase history without overwriting sale-sensitive pricing rules.
- `ICraftingInventoryService` exposes module-level operations.
- `CraftingInventoryController` exposes module-gated API routes under `api/modules/crafting/inventory`.

The Crafting inventory foundation now also includes tools and notions:

- `CraftingToolInventoryItem` merges tools per user by brand + type.
- `CraftingNotionInventoryItem` merges notions per user by brand + type.
- Tool and notion purchase history preserve the same sale-price rule as yarn.
- Tool routes live under `api/modules/crafting/inventory/tools`.
- Notion routes live under `api/modules/crafting/inventory/notions`.

The second Phase D inventory slice uses `TankerMade.Modules.Printing3D` to pressure-test the same host pattern against a non-craft domain:

- `PrintingMaterialInventoryItem` owns the user-scoped 3D printing material aggregate.
- `PrintingSpool` tracks spool-specific quantity and printer/tooling compatibility notes.
- `PrintingInventoryPurchase` appends module-specific purchase history.
- `IPrintingInventoryService` exposes module-level operations.
- `PrintingInventoryController` exposes module-gated API routes under `api/modules/printing-3d/inventory`.

## Merge Rules

The initial yarn rule follows the UX reference:

- Yarn merges per user by brand name + color name after normalization.
- Adding the same yarn increases quantity and appends purchase history.
- Non-sale prices update the yarn's regular price.
- Sale prices are recorded in purchase history but do not replace the regular price.
- Lot entries merge by lot number within the yarn item.

## Filtering Rules

Module inventory list endpoints accept module-owned query DTOs rather than Core-owned filter contracts:

- Crafting yarn filters: search, brand, color, main color, weight, fiber tag, source.
- Crafting tool filters: search, brand, type, size, source.
- Crafting notion filters: search, brand, type, size, color, source.
- 3D Printing material filters: search, material type, brand, color, diameter, storage location, source.

Filtering is intentionally implemented inside module services so each module can define the terms that make sense for its domain.

## Reference Data Rules

Module inventory reference data is also module-owned:

- Crafting exposes reference categories under `api/modules/crafting/inventory/reference/{category}`.
- 3D Printing exposes reference categories under `api/modules/printing-3d/inventory/reference/{category}`.
- Crafting seed categories currently include `yarn-weight`, `fiber-tag`, `tool-type`, and `notion-type`.
- 3D Printing seed categories currently include `material-type`, `diameter`, and `printer-tooling`.
- These are deliberately not Core enums or Core reference tables. Phase E may later wire module-owned reference data into shared Settings extension points.

## UI Routes

The first inventory UI pass is intentionally pragmatic and module-scoped:

- Crafting inventory lives at `modules/crafting/inventory`.
- 3D Printing inventory lives at `modules/printing-3d/inventory`.
- Crafting project supplies are linked from `modules/crafting/projects/{projectId}` through Crafting-owned project/inventory endpoints.
- The pages call module-owned APIs directly through the client API wrapper.
- The pages do not introduce Core inventory screens or Core inventory navigation.

## Project Links

Crafting projects can link module-owned inventory as supplies:

- Links live in `CraftingProjectInventoryLinks`.
- A link references a module-owned inventory type (`yarn`, `tool`, or `notion`) and item id.
- The project service validates that both the project and inventory item belong to the same user.
- Re-adding the same item to the same project updates the planned quantity and notes instead of creating a duplicate.
- The project DTO includes `InventoryLinks` so UI/workspace flows can show selected supplies without Core knowing inventory semantics.

## Next Extensions

The same pattern should be reused for:

- Kit/grouping flows.
- Deeper 3D printing workflows only after the thin inventory proof remains stable.

## Kit/Grouping Rules

The first kit slice is module-owned and Crafting-scoped:

- Kits live in `CraftingKits` and are user-scoped.
- Ordered kit pieces live in `CraftingKitPieces`.
- Text-based kit supplies live in `CraftingKitSupplies`.
- Kit supplies intentionally do not reference Core or inventory records yet; this mirrors the UX note that kit supplies are lightweight text entries rather than dropdown-driven inventory flows.
- Kit endpoints live under `api/modules/crafting/kits` and are gated by Crafting module activation.
- Kit pieces may optionally reference a Crafting pattern that belongs to the same user, keeping pattern linkage inside the Crafting module boundary.
- Projects can be created from a kit piece through `api/modules/crafting/kits/{kitId}/pieces/{pieceId}/project`.
- Kit-created projects keep optional `KitId` and `KitPieceId` links back to the Crafting kit records, and each kit piece may have at most one linked project.
- Polished kit UI is deferred to Phase F so it can be built against the hardened module UI extension model rather than locking in an interim pattern.
