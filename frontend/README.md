# Audio Atlas Frontend

The frontend is a Nuxt 4 application using Vue, Nuxt UI, Tailwind CSS, Three.js/globe.gl, TypeScript, ESLint, and Vitest.

## Prerequisites

Install these before running the frontend locally:

- Node.js 22, matching the GitHub Actions build job
- npm 10.8.2, matching `package.json`
- A running backend API for full local development

Verify the required tools:

```bash
node --version
npm --version
```

The frontend can start without the backend, but API-backed pages and login redirects need the backend running at the configured URL.

## Project Layout

```text
frontend/
|-- app/
|   |-- assets/
|   |-- components/
|   |-- composables/
|   |-- layouts/
|   |-- middleware/
|   |-- pages/
|   `-- types/
|-- public/
|-- server/
|   `-- routes/
|-- tests/
|-- nuxt.config.ts
|-- package.json
`-- vitest.config.ts
```

Nuxt app code lives under `app/`. Static assets live under `public/`. The local API proxy route lives in `server/routes/api/[...path].ts`.

## Install Dependencies

From the `frontend/` directory:

```bash
npm ci
```

Use `npm ci` for normal setup because the repository has a committed `package-lock.json` and CI installs dependencies this way. Use `npm install` only when intentionally updating dependencies or the lockfile.

`npm ci` runs the `postinstall` script, which calls `nuxt prepare` and generates the local `.nuxt/` files used by TypeScript and ESLint.

## Local Configuration

The frontend uses Nuxt runtime configuration from environment variables. Create `frontend/.env` when you need to override the defaults:

```bash
NUXT_API_PROXY_TARGET=http://localhost:5085
NUXT_PUBLIC_BACKEND_BASE_URL=http://localhost:5085
NUXT_PUBLIC_API_BASE=/api
```

Default local behavior without a `.env` file:

- `NUXT_API_PROXY_TARGET` falls back to `http://localhost:5085`
- `NUXT_PUBLIC_BACKEND_BASE_URL` falls back to `http://localhost:5085`
- `NUXT_PUBLIC_API_BASE` falls back to `/api`

There are no frontend-only local secrets required. OAuth provider secrets belong to the backend. The frontend only needs `NUXT_PUBLIC_BACKEND_BASE_URL` so login buttons can redirect to the backend login endpoints.

## API Routing

In development, browser requests go to the Nuxt/Nitro server at `/api/...`.

```text
Browser -> http://localhost:3000/api/...
Nitro   -> NUXT_API_PROXY_TARGET/api/...
Backend -> http://localhost:5085/api/... by default
```

In production/static hosting, browser requests should go directly to the deployed backend through `NUXT_PUBLIC_API_BASE`.

Login buttons redirect to:

```text
${NUXT_PUBLIC_BACKEND_BASE_URL}/api/auth/login/...
```

Frontend code should call the API through `useApi()`:

```ts
const { api } = useApi()
await api('/countries/123')
```

`useApi()` reads `config.public.apiBase`, so local development can use the Nuxt proxy while production can use an absolute backend URL.

## Run Locally

Start the backend first for full functionality. From the repository root, follow `Backend/README.md`.

Then start the frontend from `frontend/`:

```bash
npm run dev
```

Nuxt prints the local URL when it starts. By default this is usually `http://localhost:3000`.

## Checks

Run the checks that match the area you changed:

```bash
npm run lint
npm run typecheck
npm run test
npm run build
```

Individual commands:

- `npm run lint` runs ESLint through the Nuxt ESLint configuration.
- `npm run typecheck` runs Nuxt type checking.
- `npm run test` runs Vitest tests in `tests/**/*.test.ts` using `happy-dom`.
- `npm run build` builds the Nuxt app for production.

## Preview Production Build

Build and preview locally:

```bash
npm run build
npm run preview
```

The preview server uses the same runtime config rules as the app. Set production-like environment variables before previewing if you need to test against a non-local backend.

## Production Configuration

For deployed environments, set:

```bash
NUXT_API_PROXY_TARGET=https://your-prod-backend.example.com
NUXT_PUBLIC_BACKEND_BASE_URL=https://your-prod-backend.example.com
NUXT_PUBLIC_API_BASE=https://your-prod-backend.example.com/api
```

For static hosting, `NUXT_PUBLIC_API_BASE` must be available when the frontend is built. Static frontend assets cannot read changed public environment variables after deployment unless the host provides a separate runtime config injection step.

GitHub Actions builds frontend changes under `frontend/**`, uploads `frontend/.output/public`, and deploys it to Azure Static Web Apps.

## Common Issues

If `npm run lint` or `npm run typecheck` fails because `.nuxt` files are missing, run:

```bash
npm ci
```

or:

```bash
npx nuxt prepare
```

If API calls return connection errors locally, confirm the backend is running at `http://localhost:5085` or update `NUXT_API_PROXY_TARGET`.

If login redirects go to the wrong place, check `NUXT_PUBLIC_BACKEND_BASE_URL`.
