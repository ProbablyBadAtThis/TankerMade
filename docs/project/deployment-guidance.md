# Deployment Guidance (Phase I Slice 6)

Last reviewed: 2026-05-28

## Scope

This guide covers the current supported deployment patterns for TankerMade:

- Self-hosted server process
- Single-binary publish (server)
- Optional Docker container

## Runtime Requirements

- .NET 10 runtime (unless using self-contained publish)
- Writable app data directory for:
  - SQLite DB: `App_Data/tankermade.db`
  - Data Protection keys: `App_Data/DataProtectionKeys`
  - Asset files: `App_Data/assets`

## Required Configuration

Set these as environment variables in non-development environments:

- `JwtSettings__SecretKey` (minimum 32 characters)
- `JwtSettings__Issuer` (example: `TankerMade`)
- `JwtSettings__Audience` (example: `TankerMadeClient`)
- `JwtSettings__ExpirationMinutes` (example: `60`)

Recommended:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS` for explicit HTTP/HTTPS binding

Example:

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:5236;https://0.0.0.0:7065"
export JwtSettings__SecretKey="replace-with-32-plus-char-secret"
export JwtSettings__Issuer="TankerMade"
export JwtSettings__Audience="TankerMadeClient"
export JwtSettings__ExpirationMinutes="60"
```

## Option A: Self-Hosted (Framework-Dependent)

Run with local files in place:

```bash
dotnet run --project src/TankerMade.Server --no-launch-profile
```

For deployment from published output:

```bash
dotnet publish src/TankerMade.Server -c Release -o out/server
dotnet out/server/TankerMade.Server.dll
```

## Option B: Single-Binary Publish

Example for macOS Apple Silicon:

```bash
dotnet publish src/TankerMade.Server -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -o out/server-osx-arm64
```

Linux x64 example:

```bash
dotnet publish src/TankerMade.Server -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o out/server-linux-x64
```

Run:

```bash
./out/server-linux-x64/TankerMade.Server
```

Notes:

- Keep `App_Data` writable alongside the executable (or set working directory so relative paths resolve correctly).
- First startup applies EF migrations automatically.

## Option C: Optional Docker

If you choose Docker, keep data in mounted volumes so DB, assets, and key ring persist across container restarts.

Typical volume targets:

- `/app/App_Data/tankermade.db`
- `/app/App_Data/DataProtectionKeys/`
- `/app/App_Data/assets/`

Typical env vars (same as above):

- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://0.0.0.0:8080;https://0.0.0.0:8443`
- `JwtSettings__SecretKey=...`
- `JwtSettings__Issuer=...`
- `JwtSettings__Audience=...`
- `JwtSettings__ExpirationMinutes=...`

## Reverse Proxy / HTTPS Notes

- Production pipeline enforces HTTPS + HSTS in non-development environments.
- If TLS terminates at a reverse proxy, ensure forwarded headers and upstream scheme handling are configured appropriately in host infrastructure.

## Post-Deploy Smoke Checklist

1. Server starts without configuration exceptions.
2. `/api/Auth/register` and `/api/Auth/login` succeed.
3. HTTP endpoint redirects to HTTPS in production mode.
4. HTTPS responses include `Strict-Transport-Security`.
5. `App_Data/DataProtectionKeys` contains persisted key files.
6. Asset upload/download works and files persist under `App_Data/assets`.
