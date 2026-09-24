# Client progress

Last reviewed: 2026-09-24. Active build order: `docs/project/direction-brief.md`.

Fiber commissions are the only vertical in scope. Knitting is the live module. Other craft modules stay as templates.

## Two surfaces

| Surface | Who | What they see |
|---|---|---|
| Workshop | Signed-in maker, on the local app | Projects, patterns, inventory, timers, photos, and whether the commission clears the maker’s target hourly rate |
| Client page | Someone with the link, no account | Stage, photos the maker published, material lines safe to show, next step, last updated |

The client page uses plain language and a studio tone. It does not use the cozy workshop theme. The client never installs the app.

## Lifecycle

1. **Quote** — piece, options, material estimate, labor estimate, price, expected window.
2. **Accepted** — client agreed. Deposit can be marked received. The app does not take or process payments.
3. **Materials** — materials in hand, with cost.
4. **In progress** — module-specific progress, published as a plain sentence.
5. **Revision** — dated client-driven change. Record what changed and whether price or due date moved.
6. **Ready** — finished photos and handoff notes the client is allowed to see.
7. **Delivered / closed.**

Revisions are part of the first slice. They are how unpaid scope gets recorded.

## Private versus public

Core stages are craft-agnostic. Knitting maps rows, pieces, and timers onto those stages through a module capability. “Blocking the body” stays in the workshop. The client sees a sentence such as “Assembling the pieces.”

Persist a public projection, not a live query of workshop tables. The projection stores the last published snapshot plus a history of stage changes and revisions. The full database never leaves the machine.

Private economics stay on workshop APIs only:

- Material cost from inventory linked to the project.
- Time from project timers.
- A core maker rate, because a target hourly rate is not fiber-specific.
- The workshop shows materials, time, implied net, and whether the price beats that rate.

Those fields are never serialized into the public projection.

The first link is an unguessable, revocable token rendered by the local server. The maker can preview it before sending it. The link shows last updated time. It is read-only. The home server is not exposed to clients. A later uploader can send the same snapshot to a hosted page without reshaping workshop tables.

## Not in this pass

Hosted dashboard, client accounts, payments, email or SMS, Ravelry import, gamification, a module store, and any non-knitting module work.
