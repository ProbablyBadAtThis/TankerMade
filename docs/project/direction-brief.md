# TankerMade direction brief

Read `docs/project/charter.md`, `docs/project/roadmap.md`, `docs/project/handoff.md`, `docs/project/module-boundaries.md`, and `AGENTS.md` before changing code. Follow those module boundaries. This brief overrides the handoff’s “next work” list.

## Product

TankerMade is a local-first workshop for makers who take commissions. Craft modules are templates. Knitting is the first real vertical. Crochet, embroidery, quilting, sewing, and 3D printing stay as patterns to copy later.

The product that matters is a client progress page, in the spirit of a pizza tracker. A client opens a link and sees where a commissioned piece is, what the materials were, and what happens next. The client never installs the app and never gets an account.

The maker’s local app remains the workshop: projects, patterns, inventory, timers, photos. It also shows the maker, in private, whether the commission clears their own target hourly rate after materials.

Two surfaces, two audiences:

- **Workshop (local, signed-in maker).** Cozy notebook. Full data. Private economics.
- **Client page (plain language, studio tone).** Stages, photos, material cost, next step, last updated. No craft jargon, no hour log, no hourly rate, no addresses, no measurements, no private notes.

## Stop

Do not continue the MudBlazor parity pass across crochet, embroidery, quilting, sewing, or 3D printing. Do not add a module store, licensing, payments, gamification, or a second production craft. Do not expose the home server to clients. Do not put maker hours or the maker’s rate on any client-facing API or page.

Knitting UI may change only where this brief needs it.

## Lifecycle to model

One commission thread:

1. **Quote** — piece, options, material estimate, labor estimate, price, expected window.
2. **Accepted** — client agreed. Deposit can be marked received. Do not take or process payments.
3. **Materials** — materials in hand, with cost.
4. **In progress** — module-specific progress, published as a plain sentence.
5. **Revision** — dated client-driven change. Record what changed and whether price or due date moved.
6. **Ready** — finished photos and handoff notes the client is allowed to see.
7. **Delivered / closed.**

Revisions are required in the first slice. They are how unpaid scope gets recorded.

## Architecture rules

Core stays craft-agnostic. A commission’s public stages are neutral. Knitting maps rows, pieces, and timers onto those stages through a module capability, the same way recent-work summaries already work (`IModuleRecentWorkSummaryProvider` and the other handlers under `src/TankerMade.Contracts/Services/ModuleCapabilities/`).

Add something in that family, for example a module client-status provider:

- Input: the maker’s project and what they choose to publish.
- Output: stage, plain-language summary, material lines safe to show, photo asset ids safe to show, next step, last updated.
- Knitting owns the jargon translation. “Blocking the body” stays in the workshop. The client sees a sentence such as “Assembling the pieces.”

Persist a **public projection**, not a live query of workshop tables. Treat it as an outbox: the workshop queues updates locally, including while offline, and the projection stores only the last published snapshot plus a history of stage changes and revisions. The full database never leaves the machine.

The first implementation can render that projection on the local server at an unguessable link. Hosted sync comes after the local preview is right. Design the projection so a later uploader can send that snapshot to a hosted page without reshaping workshop tables.

Client link requirements for the first slice:

- Unguessable token, revocable by the maker.
- Shows last updated time, so a closed laptop does not look like a stalled order.
- Read-only. No client login.
- Maker can preview the page before sending it.

Private economics stay on workshop APIs only:

- Material cost from inventory already linked to the project.
- Time from existing project timers.
- Maker sets a target hourly rate in knitting or core settings (pick the place that does not force other modules to be fiber-shaped; a core “maker rate” is fine because it is not craft-specific).
- Show the maker: materials, time, implied net, and whether the price beats their rate.
- Never serialize those fields into the public projection.

## Work order

Do these in order. Finish each before starting the next.

**1. Record the direction.** Update `docs/project/handoff.md` and add a short section to `docs/project/charter.md` or a new `docs/project/client-progress.md`. State the two surfaces, the lifecycle, the private-versus-public data rule, and that fiber commissions are the only vertical in scope. Point the handoff’s “do next” at the steps below. Leave the old MudBlazor checklist in place as reference, and stop treating it as the active plan.

**2. Projection and knitting translation, backend first.** Contracts, knitting capability handler, storage for the published snapshot, revision events, and token. Local API: publish, revoke, and anonymous read-by-token. Tests for: private fields cannot appear on the public DTO; a revision records a price or date change; an inactive knitting module cannot publish; a revoked token does not resolve.

**3. Maker preview and private economics on one knitting project.** From the knitting project workspace, the maker can set a quote price, mark deposit received, publish a status, add a revision, and open the client preview. On the same screen, show private net versus their target rate. Client preview uses studio layout, separate from the cozy workshop theme.

**4. Offline queue behavior.** Publishing while the future hosted target is absent still saves the local projection. Last-updated is the publish time. Do not build the cloud uploader in this pass.

**5. Stop and ask for a real-browser check.** Maker path: open a knitting project, publish, copy preview link, revoke, confirm the link dies. Client path: open the link signed out and confirm hours, rate, notes, and measurements are absent. Then the user runs `dotnet build TankerMade.sln`.

## Out of this pass

Hosted dashboard, accounts for clients, payments, email or SMS notifications, Ravelry import, and any non-knitting module work.

## Done when

A maker can run the local app, publish a knitting commission, and hand someone a link that shows stage, materials, photos, next step, and last updated. The maker can see, only while signed in, whether that price beats their rate. A revision is visible on the link. Revoking the link cuts off access. No other craft module was changed to get there.
