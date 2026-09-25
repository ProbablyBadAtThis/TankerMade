interface Env {
  SNAPSHOTS: KVNamespace;
  HOST_KEY: string;
}

interface MaterialLine {
  name: string;
}

interface Revision {
  occurredAt: string;
  summary: string;
  priceChanged: boolean;
  dueDateChanged: boolean;
  previousPrice: number | null;
  newPrice: number | null;
  previousDueDate: string | null;
  newDueDate: string | null;
}

interface Snapshot {
  stage: string;
  title: string;
  summary: string;
  materialLines: MaterialLine[];
  photoAssetIds: string[];
  nextStep: string;
  price: number | null;
  dueDate: string | null;
  lastUpdated: string;
  revisions: Revision[];
}

const MAX_SNAPSHOT_BYTES = 100_000;
const MAX_PHOTO_BYTES = 8_000_000;

export default {
  async fetch(request, env): Promise<Response> {
    const url = new URL(request.url);
    const parts = url.pathname.split("/").filter(Boolean);

    if (request.method === "PUT" && parts[0] === "snapshots" && parts.length === 2) {
      return saveSnapshot(request, env, parts[1]);
    }

    if (request.method === "PUT" && parts[0] === "snapshots" && parts[2] === "photos" && parts.length === 4) {
      return savePhoto(request, env, parts[1], parts[3]);
    }

    if (request.method === "DELETE" && parts[0] === "snapshots" && parts.length === 2) {
      return revoke(request, env, parts[1]);
    }

    if (request.method === "GET" && parts[0] === "client-progress" && parts.length === 2) {
      return page(env, decodeURIComponent(parts[1]));
    }

    if (request.method === "GET" && parts[0] === "client-progress" && parts[2] === "photos" && parts.length === 4) {
      return photo(env, decodeURIComponent(parts[1]), parts[3]);
    }

    return new Response("Not found", { status: 404 });
  },
} satisfies ExportedHandler<Env>;

async function saveSnapshot(request: Request, env: Env, publicationId: string): Promise<Response> {
  if (!authorized(request, env.HOST_KEY)) {
    return new Response(null, { status: 401 });
  }

  if (!isGuid(publicationId)) {
    return new Response(null, { status: 400 });
  }

  const body = await request.arrayBuffer();
  if (body.byteLength > MAX_SNAPSHOT_BYTES) {
    return new Response(null, { status: 400 });
  }

  let parsed: unknown;
  try {
    parsed = JSON.parse(new TextDecoder().decode(body));
  } catch {
    return new Response(null, { status: 400 });
  }

  const snapshot = toPublicSnapshot(parsed);
  if (!snapshot) {
    return new Response(null, { status: 400 });
  }

  const previous = await readSnapshot(env, publicationId);
  const token = request.headers.get("X-Client-Token")?.trim();
  await env.SNAPSHOTS.put(publicationKey(publicationId), JSON.stringify(snapshot));

  if (token) {
    const previousToken = await env.SNAPSHOTS.get(publicationTokenKey(publicationId));
    if (previousToken && previousToken !== token) {
      await env.SNAPSHOTS.delete(tokenKey(previousToken));
    }
    await env.SNAPSHOTS.put(tokenKey(token), publicationId);
    await env.SNAPSHOTS.put(publicationTokenKey(publicationId), token);
  }

  if (previous) {
    const allowed = new Set(snapshot.photoAssetIds);
    for (const assetId of previous.photoAssetIds) {
      if (!allowed.has(assetId)) {
        await env.SNAPSHOTS.delete(photoKey(publicationId, assetId));
      }
    }
  }

  return new Response(null, { status: 204 });
}

async function savePhoto(request: Request, env: Env, publicationId: string, assetId: string): Promise<Response> {
  if (!authorized(request, env.HOST_KEY)) {
    return new Response(null, { status: 401 });
  }

  if (!isGuid(publicationId) || !isGuid(assetId)) {
    return new Response(null, { status: 400 });
  }

  const snapshot = await readSnapshot(env, publicationId);
  if (!snapshot || !snapshot.photoAssetIds.includes(assetId)) {
    return new Response(null, { status: 404 });
  }

  const body = await request.arrayBuffer();
  if (body.byteLength === 0 || body.byteLength > MAX_PHOTO_BYTES) {
    return new Response(null, { status: 400 });
  }

  const contentType = request.headers.get("Content-Type") || "application/octet-stream";
  await env.SNAPSHOTS.put(photoKey(publicationId, assetId), body, { metadata: { contentType } });
  return new Response(null, { status: 204 });
}

async function revoke(request: Request, env: Env, publicationId: string): Promise<Response> {
  if (!authorized(request, env.HOST_KEY)) {
    return new Response(null, { status: 401 });
  }

  if (!isGuid(publicationId)) {
    return new Response(null, { status: 400 });
  }

  const snapshot = await readSnapshot(env, publicationId);
  const token = await env.SNAPSHOTS.get(publicationTokenKey(publicationId));
  await env.SNAPSHOTS.delete(publicationKey(publicationId));
  await env.SNAPSHOTS.delete(publicationTokenKey(publicationId));
  if (token) {
    await env.SNAPSHOTS.delete(tokenKey(token));
  }

  if (snapshot) {
    for (const assetId of snapshot.photoAssetIds) {
      await env.SNAPSHOTS.delete(photoKey(publicationId, assetId));
    }
  }

  return new Response(null, { status: 204 });
}

async function page(env: Env, token: string): Promise<Response> {
  const snapshot = await findByToken(env, token);
  const html = renderPage(snapshot, token);
  return new Response(html, {
    status: snapshot ? 200 : 404,
    headers: { "content-type": "text/html; charset=utf-8" },
  });
}

async function photo(env: Env, token: string, assetId: string): Promise<Response> {
  const publicationId = await env.SNAPSHOTS.get(tokenKey(token));
  const snapshot = publicationId ? await readSnapshot(env, publicationId) : null;
  if (!snapshot || !isGuid(assetId) || !snapshot.photoAssetIds.includes(assetId)) {
    return new Response(null, { status: 404 });
  }

  const stored = await env.SNAPSHOTS.getWithMetadata<PhotoMetadata>(photoKey(publicationId!, assetId), "arrayBuffer");
  if (!stored.value) {
    return new Response(null, { status: 404 });
  }

  return new Response(stored.value, {
    headers: { "content-type": stored.metadata?.contentType || "application/octet-stream" },
  });
}

interface PhotoMetadata {
  contentType?: string;
}

async function findByToken(env: Env, token: string): Promise<Snapshot | null> {
  const publicationId = await env.SNAPSHOTS.get(tokenKey(token));
  return publicationId ? readSnapshot(env, publicationId) : null;
}

async function readSnapshot(env: Env, publicationId: string): Promise<Snapshot | null> {
  const json = await env.SNAPSHOTS.get(publicationKey(publicationId));
  if (!json) {
    return null;
  }

  try {
    return toPublicSnapshot(JSON.parse(json));
  } catch {
    return null;
  }
}

function toPublicSnapshot(value: unknown): Snapshot | null {
  if (!value || typeof value !== "object") {
    return null;
  }

  const source = value as Record<string, unknown>;
  const photoAssetIds = Array.isArray(source.photoAssetIds)
    ? source.photoAssetIds.filter((id): id is string => typeof id === "string" && isGuid(id))
    : [];
  const materialLines = Array.isArray(source.materialLines)
    ? source.materialLines
        .map((line) => {
          if (!line || typeof line !== "object") {
            return null;
          }
          const name = (line as Record<string, unknown>).name;
          return typeof name === "string" ? { name } : null;
        })
        .filter((line): line is MaterialLine => line !== null)
    : [];
  const revisions = Array.isArray(source.revisions)
    ? source.revisions
        .map((item) => {
          if (!item || typeof item !== "object") {
            return null;
          }
          const revision = item as Record<string, unknown>;
          return {
            occurredAt: text(revision.occurredAt),
            summary: text(revision.summary),
            priceChanged: revision.priceChanged === true,
            dueDateChanged: revision.dueDateChanged === true,
            previousPrice: numberOrNull(revision.previousPrice),
            newPrice: numberOrNull(revision.newPrice),
            previousDueDate: textOrNull(revision.previousDueDate),
            newDueDate: textOrNull(revision.newDueDate),
          };
        })
        .filter((item): item is Revision => item !== null)
    : [];

  return {
    stage: text(source.stage) || "Quote",
    title: text(source.title),
    summary: text(source.summary),
    materialLines,
    photoAssetIds,
    nextStep: text(source.nextStep),
    price: numberOrNull(source.price),
    dueDate: textOrNull(source.dueDate),
    lastUpdated: text(source.lastUpdated),
    revisions,
  };
}

function renderPage(snapshot: Snapshot | null, token: string): string {
  if (!snapshot) {
    return layout("<h1>This link is not available</h1><p>It may have been revoked, or it was never published.</p>");
  }

  const parts: string[] = ['<p class="studio-kicker">Commission</p>'];
  if (snapshot.title.trim()) {
    parts.push(`<h1>${escapeHtml(snapshot.title)}</h1>`);
  }

  parts.push(`<p class="studio-stage">${escapeHtml(stageLabel(snapshot.stage))}</p>`);
  parts.push(`<p class="studio-summary">${escapeHtml(snapshot.summary)}</p>`);
  parts.push(`<p class="studio-meta">Updated ${escapeHtml(formatUtc(snapshot.lastUpdated))}</p>`);

  if (snapshot.price !== null || snapshot.dueDate) {
    parts.push('<dl class="studio-facts">');
    if (snapshot.price !== null) {
      parts.push(`<div><dt>Price</dt><dd>${escapeHtml(money(snapshot.price))}</dd></div>`);
    }
    if (snapshot.dueDate) {
      parts.push(`<div><dt>Expected</dt><dd>${escapeHtml(formatDate(snapshot.dueDate, true))}</dd></div>`);
    }
    parts.push("</dl>");
  }

  if (snapshot.materialLines.length > 0) {
    parts.push("<h2>Materials</h2><ul>");
    for (const line of snapshot.materialLines) {
      parts.push(`<li>${escapeHtml(line.name)}</li>`);
    }
    parts.push("</ul>");
  }

  if (snapshot.photoAssetIds.length > 0) {
    parts.push('<h2>Photos</h2><div class="studio-photos">');
    for (const assetId of snapshot.photoAssetIds) {
      parts.push(`<img src="/client-progress/${encodeURIComponent(token)}/photos/${assetId}" alt="Published photo" />`);
    }
    parts.push("</div>");
  }

  parts.push(`<h2>Next</h2><p>${escapeHtml(snapshot.nextStep)}</p>`);
  if (snapshot.revisions.length > 0) {
    parts.push("<h2>Revisions</h2><ol>");
    for (const revision of snapshot.revisions) {
      parts.push(`<li><span>${escapeHtml(formatDate(revision.occurredAt, true))}</span>`);
      if (revision.summary.trim()) {
        parts.push(`<span>${escapeHtml(revision.summary)}</span>`);
      }
      if (revision.priceChanged) {
        parts.push(`<span>Price ${escapeHtml(money(revision.previousPrice))} to ${escapeHtml(money(revision.newPrice))}</span>`);
      }
      if (revision.dueDateChanged) {
        parts.push(`<span>Window ${escapeHtml(formatDate(revision.previousDueDate, false))} to ${escapeHtml(formatDate(revision.newDueDate, true))}</span>`);
      }
      parts.push("</li>");
    }
    parts.push("</ol>");
  }

  return layout(parts.join(""));
}

function layout(body: string): string {
  return `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Commission</title>
  <style>
    body { margin: 0; background: #f4f1ec; }
    .studio-page { max-width: 40rem; margin: 3rem auto; padding: 2rem 1.5rem 4rem; color: #1c2430; font-family: "Segoe UI", sans-serif; }
    h1, h2 { font-weight: 500; }
    .studio-kicker { margin: 0; text-transform: uppercase; letter-spacing: 0.08em; font-size: 0.75rem; color: #5c6778; }
    .studio-stage { margin: 0.25rem 0 1rem; font-size: 1.25rem; }
    .studio-summary { font-size: 1.125rem; line-height: 1.5; }
    .studio-meta, dt { color: #5c6778; }
    .studio-facts { display: flex; gap: 2rem; }
    .studio-facts dd { margin: 0.25rem 0 0; font-size: 1.125rem; }
    .studio-photos img { max-width: 100%; margin: 0 0 1rem; }
    ol li span { display: block; }
  </style>
</head>
<body>
  <article class="studio-page">${body}</article>
</body>
</html>`;
}

function stageLabel(stage: string): string {
  return stage === "InProgress" ? "In progress" : stage;
}

function formatUtc(value: string): string {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return "";
  }

  const formatted = new Intl.DateTimeFormat("en-US", {
    timeZone: "UTC",
    month: "short",
    day: "numeric",
    year: "numeric",
    hour: "numeric",
    minute: "2-digit",
  }).format(date);
  return `${formatted} UTC`;
}

function formatDate(value: string | null, withYear: boolean): string {
  if (!value) {
    return "";
  }

  const dateOnly = /^(\d{4})-(\d{2})-(\d{2})/.exec(value);
  const date = dateOnly
    ? new Date(Date.UTC(Number(dateOnly[1]), Number(dateOnly[2]) - 1, Number(dateOnly[3])))
    : new Date(value);
  if (Number.isNaN(date.getTime())) {
    return "";
  }

  return new Intl.DateTimeFormat("en-US", {
    timeZone: "UTC",
    month: "short",
    day: "numeric",
    year: withYear ? "numeric" : undefined,
  }).format(date);
}

function money(value: number | null): string {
  if (value === null) {
    return "";
  }

  return new Intl.NumberFormat("en-US", { style: "currency", currency: "USD" }).format(value);
}

function escapeHtml(value: string): string {
  return value
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;");
}

function authorized(request: Request, secret: string): boolean {
  const provided = request.headers.get("X-Host-Key") ?? "";
  if (!secret || provided.length !== secret.length) {
    return false;
  }

  let diff = 0;
  for (let i = 0; i < secret.length; i++) {
    diff |= secret.charCodeAt(i) ^ provided.charCodeAt(i);
  }

  return diff === 0;
}

function isGuid(value: string): boolean {
  return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value);
}

function text(value: unknown): string {
  return typeof value === "string" ? value : "";
}

function textOrNull(value: unknown): string | null {
  return typeof value === "string" && value.length > 0 ? value : null;
}

function numberOrNull(value: unknown): number | null {
  return typeof value === "number" && Number.isFinite(value) ? value : null;
}

function publicationKey(publicationId: string): string {
  return `pub:${publicationId}`;
}

function publicationTokenKey(publicationId: string): string {
  return `pub-token:${publicationId}`;
}

function tokenKey(token: string): string {
  return `token:${token}`;
}

function photoKey(publicationId: string, assetId: string): string {
  return `photo:${publicationId}:${assetId}`;
}
