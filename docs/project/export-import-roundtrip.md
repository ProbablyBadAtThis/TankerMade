# Export/Import Round-Trip Verification (Phase I Slice 4)

Last reviewed: 2026-05-28

## Purpose

Verify that TankerMade supports a full export/import round-trip without data loss using the current SQLite-first operations model:

- **Export:** file-level backup copy of `tankermade.db`
- **Import:** full replace-restore of `tankermade.db` from backup

This aligns with the charter requirement for admin backup/import operations while dedicated JSON export/import APIs are still future work.

## Database Location

- Active DB: `src/TankerMade.Server/App_Data/tankermade.db`

## Preconditions

- Server can start cleanly.
- At least one user account exists.
- You can log in through the client or API.

## Round-Trip Procedure

### 1) Prepare baseline

1. Stop server and client.
2. Confirm DB exists:
   - `ls -la src/TankerMade.Server/App_Data/tankermade.db`

### 2) Export (backup copy)

1. Create a timestamped backup directory:
   - `mkdir -p backups`
2. Copy DB:
   - `cp src/TankerMade.Server/App_Data/tankermade.db backups/tankermade-$(date +%Y%m%d-%H%M%S).db`
3. Record the backup filename for restore.

### 3) Mutate live data after export

1. Start server/client.
2. Create a clearly identifiable canary record after backup (for example):
   - New project named `ROUNDTRIP-CANARY`
3. Confirm canary appears in UI/API responses.
4. Stop server/client.

### 4) Import (restore backup)

1. Replace active DB with the backup file captured in step 2:
   - `cp backups/<backup-file>.db src/TankerMade.Server/App_Data/tankermade.db`
2. Start server/client again.

### 5) Verify round-trip result

1. Log in with a pre-existing account from before step 3.
2. Confirm application loads normally.
3. Confirm `ROUNDTRIP-CANARY` does **not** exist (proves restore replaced post-export changes).
4. Confirm pre-export records still exist (proves baseline data survived round-trip).

## Pass Criteria

- Server starts successfully after restore.
- Authentication still works.
- Pre-export data is present.
- Post-export canary data is absent.

## Notes

- This is a **full replace-restore** flow, not row-level merge.
- Keep backup files outside source control.
- Future work may add dedicated JSON export/import endpoints, but this procedure is the current production-aligned backup/import path.
