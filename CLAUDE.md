# Audio Atlas

A community-built, curated map of the world's music genres. An interactive 3D globe is the primary
interface: users click a country to see its genres, then drill into genre pages with descriptions,
relationships, instruments, sources and an embedded example song. Contributors submit new genres or
suggest edits to existing ones; curators review them in an admin queue.

Content is licensed **CC BY-NC-SA 4.0** (see `LICENSE-CONTENT`). Embedded third-party video is *not*
covered by that licence.

## Layout

```
Backend/src/AudioAtlasDomain          entities (Genre, Country, Submission, ApplicationUser…)
Backend/src/AudioAtlasApplication     DTOs, service/repository interfaces, helpers (Media/YouTubeVideo)
Backend/src/AudioAtlasInfrastructure  EF Core, repositories, services, seeding, migrations
Backend/src/AudioAtlasView            ASP.NET Core Web API (AudioAtlas.API.csproj) — controllers, Program.cs
Backend/test/AudioAtlasInfrastructureTests   the entire test suite
frontend/app/{pages,components,layouts,composables,assets}   Nuxt 4 app
frontend/scripts/check-contrast.mjs   dependency-free WCAG AA checker, runs in CI
docs/                                 API.md, ERDs, domain notes
```

## Stack

.NET 10 · EF Core · Azure SQL · Azure App Service (Linux)
Nuxt 4 · Vue 3 `<script setup>` · Nuxt UI 4 · Tailwind 4 · globe.gl + three.js · Vitest · Zod
Azure Static Web Apps (frontend), config in `frontend/public/staticwebapp.config.json`

## Commands

Run frontend commands from `frontend/`.

```bash
npm run dev              # needs the backend running, or set NUXT_API_PROXY_TARGET (see below)
npm run build            # ALSO prerenders — see "the build crawls links" below
npm run lint             # eslint (@nuxt/eslint) — the authoritative linter for this repo
npm run typecheck        # nuxt typecheck
npm run check:contrast   # WCAG AA gate, runs in CI
npm run test             # vitest

cd Backend && dotnet test    # whole suite
```

**Known-good baselines** — compare against these rather than expecting zero:

| Check | Baseline |
|---|---|
| `dotnet test` | **135 passing, 0 failing** |
| `npm run typecheck` | **2 errors**, both pre-existing in `app/pages/admin/users.vue` (readonly array → `SelectMenuItem[]`) |
| `npm run check:contrast` | **0 sub-AA colours** — must stay at zero |
| `npm run lint` | a few pre-existing errors (e.g. `Footer` multi-word name, two `Globe.vue` brace-style). Establish the baseline with `git stash` + re-run before blaming your change. |

## Things that have actually caused outages

**Migrations run at application startup.** `Program.cs:243` calls `ctx.Database.Migrate()`. A failing
migration means the API does not boot. Treat every migration as a deployment gate.

**There are two seeders, and they behave completely differently.**

- `DbInitializer.SeedDatabase` — initial seed. **Bails if any Instruments/Genres/Countries exist**
  (`DbInitializer.cs:36`), so it never runs against populated production. Editing `genreSeeding.json`
  will not change prod.
- `DbInitializer.SeedAdditionalDataAsync` — supplemental, runs on **every startup** via
  `SupplementalSeedBackgroundService` (15s delay). Additive and idempotent. This is the one that
  reaches production. It reads `genreSeeding2.json`.

**The two seed JSON files are not edited the same way.**

| File | Size | Round-trips through `json.dumps(…, ensure_ascii=False, indent=2)`? |
|---|---|---|
| `genreSeeding.json` | ~743 KB | **No** — PowerShell `ConvertTo-Json` formatting. Edit **textually** (regex), never load-and-dump, or you reformat the whole file. |
| `genreSeeding2.json` | ~410 KB | **Yes**, with **no trailing newline**. Safe to load, mutate, dump. |

Assert the round-trip before writing:
```python
raw = open(p, encoding='utf-8').read()
assert json.dumps(json.loads(raw), ensure_ascii=False, indent=2) == raw
```

**Seed data is matched by name, and a mismatch fails silently.** Genre names must match the catalogue
*exactly* — `Coupe-Decale` not `Coupé-Décalé`, `Forro` not `Forró`. A mismatch logs a warning and
skips. Always verify names against the live catalogue before committing seed data.

**The seeder swallows its own exceptions on purpose** so a seeding fault cannot take the API down.
That means the log line is the only signal. Stable markers to alert on: `SeederFailed`, `DataQuality`.
A duplicate `isoCode` once silently killed every genre batch's relations for days.

**Error paths must not return benign-looking values.** This has bitten twice — the seeder above, and a
research script whose `except: continue` turned a total DNS outage into a plausible "found 0 results".
If something *couldn't* run, say so distinctly from "ran and found nothing".

**`npm run build` crawls every internal link.** `nitro.preset: 'static'`, so prerender follows links
and **any dead internal link fails the build**. A link to a route that doesn't exist (`/contributors/:id`
was one) breaks CI for everyone. Before adding a dynamic `:to`, confirm the route exists.

**CI is path-filtered.** `Backend CICD.yml` runs on `Backend/**`, `Frontend CICD.yml` on `frontend/**`.
A backend-only run of PRs can leave a frontend break undetected for weeks.

## Frontend conventions

- **Vue 3 `<script setup>`** with Nuxt auto-imports. `useRoute`, `useRouter`, `useHead`,
  `definePageMeta` are not imported.
- **One layout** (`layouts/default.vue`). It owns the OAuth callback handling, login/username/success
  modals and the app banner — do not duplicate it. Full-bleed pages opt in with
  `definePageMeta({ fullBleed: true })`, which drops page padding and locks body scroll via `useHead`
  (bound to route meta so it is removed on navigation).
- **Colour tokens, not raw hex.** `--color-label` (7.0:1), `--color-meta` (4.6:1), `--color-aurora`,
  `--color-space-50` in `app/assets/css/main.css`. The contrast checker **only scans token colours** —
  raw hex in scoped CSS slips past it, which is how `#4a6070` (3.0:1) shipped in ~11 places.
- **API access** goes through `composables/useApi.ts`. In dev, `/api` is proxied; to work without a
  local backend: `NUXT_API_PROXY_TARGET=https://api-audioatlas.azurewebsites.net npm run dev`.
  (Pointing `NUXT_PUBLIC_API_BASE` at production instead fails CORS.)

## Backend conventions

- Clean-ish layering: Domain → Application (interfaces/DTOs) → Infrastructure (EF, services) → View
  (controllers). Services are constructor-injected and registered in `Program.cs`.
- Genre relationships are three self-referencing join tables: `GenreHierarchy(ParentGenreId, SubGenreId)`,
  `GenreSimilarity` (directional — the seeder writes both directions), `GenreCountry`.
- Genres are **soft-deleted** (`IsArchived`) with a global query filter. Admin endpoints that need
  archived rows must call `.IgnoreQueryFilters()`.
- `Genre.RowVersion` is a SQL `rowversion` concurrency token; the admin PUT sends it and gets a 409 on
  conflict.
- **Never store a URL where an ID belongs.** `ExampleSongYoutubeId` holds the bare 11-char video ID,
  parsed by `AudioAtlasApplication.Media.YouTubeVideo.TryParseId`, because the value ends up in an
  iframe `src`. Same parser is used by both the submission path and the admin PUT.

## Codacy

Codacy runs rules that do not fit this stack and produces recurring false positives. **The repo's own
ESLint is authoritative.** Verify before "fixing":

- Nuxt auto-imports and macros reported as `'useRoute' is not defined`, `'definePageMeta' is not defined`
- **Qwik** rules on a Vue app: "Non-serializable expression must be wrapped with `$(...)`"
- React `rules-of-hooks`: "hook called at module level" — `<script setup>` *is* the component body
- `<script setup>` variables used only in `<template>` reported as unused
- `no-unnecessary-condition` on Vue props: it reads `type: Object` and ignores `default: null`

**But not all of them are wrong.** A `no-unnecessary-condition` finding on `route.query` was a real
latent bug once (vue-router types query values as `string | null | (string|null)[]`). The dangerous
direction is obeying one blindly: a guard spanning a closure or async boundary flagged "always truthy"
is usually load-bearing, and deleting it introduces a regression.

Resolution for genuine false positives is the Codacy dashboard, not code.

## Working style that fits this repo

- **Verify, don't assume.** Establish the baseline (`git stash`, re-run) before attributing a failure
  to your change.
- **Beware pipes masking exit codes** — `npm run build | tail` reports `tail`'s status. Capture `$?`.
- Seed-data PRs should be **additions-only**; assert that non-target sections are byte-identical and
  that existing entries are unchanged and in place.
- Prefer one PR against `main` over stacked PRs. Several PRs appending to the same JSON array will
  conflict pairwise; consolidate instead.
