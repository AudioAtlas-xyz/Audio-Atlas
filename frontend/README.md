# Audio Atlas Frontend

## API routing

In development, the frontend calls the Nuxt/Nitro server at `/api/...`.
Nitro proxies those requests to the real backend using the server-side runtime variable `NUXT_API_PROXY_TARGET`.
In production/static hosting, the frontend calls the backend directly through the public runtime variable `NUXT_PUBLIC_API_BASE`.

This means:

- In development, the browser calls `http://localhost:3000/api/...`
- Nitro forwards that to your local backend, for example `http://localhost:5085/api/...`
- In production, the browser calls `https://api-audioatlas.azurewebsites.net/api/...` by default
- Login buttons redirect to `${NUXT_PUBLIC_BACKEND_BASE_URL}/api/auth/login/...`

## Development config

Create a local `.env` file:

```bash
NUXT_API_PROXY_TARGET=http://localhost:5085
NUXT_PUBLIC_BACKEND_BASE_URL=http://localhost:5085
NUXT_PUBLIC_API_BASE=/api
```

Then run:

```bash
npm install
npm run dev
```

If you do not set these variables, development falls back to `http://localhost:5085`.

## Production config

Set this environment variable in your production host before starting Nuxt:

```bash
NUXT_API_PROXY_TARGET=https://your-prod-backend.example.com
NUXT_PUBLIC_BACKEND_BASE_URL=https://your-prod-backend.example.com
NUXT_PUBLIC_API_BASE=https://your-prod-backend.example.com/api
```

Then build and run Nuxt normally:

```bash
npm run build
npm run preview
```

For static hosting, make sure `NUXT_PUBLIC_API_BASE` is available while the frontend is built. Static frontend assets cannot read new public environment variables after deployment unless the host has a separate runtime config injection step.

## How to call the API in code

Frontend code should use:

```ts
const { api } = useApi()
await api('/countries/123')
```

`useApi()` reads `config.public.apiBase`, so development can keep using the local Nuxt proxy and production/static hosting can use the absolute backend URL.

If you need to override the browser API base for a deployment, set `NUXT_PUBLIC_API_BASE`.

## Test

```bash
npm test
```
