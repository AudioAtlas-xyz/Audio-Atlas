# Audio Atlas Frontend

## API routing

The frontend now always calls the Nuxt/Nitro server at `/api/...`.
Nitro proxies those requests to the real backend using the server-side runtime variable `NUXT_API_PROXY_TARGET`.

This means:

- In development, the browser calls `http://localhost:3000/api/...`
- Nitro forwards that to your local backend, for example `http://localhost:5085/api/...`
- In production, the browser still calls `/api/...`
- Nitro forwards that to your production backend URL

## Development config

Create a local `.env` file:

```bash
NUXT_API_PROXY_TARGET=http://localhost:5085
```

Then run:

```bash
npm install
npm run dev
```

If you do not set `NUXT_API_PROXY_TARGET`, development falls back to `http://localhost:5085`.

## Production config

Set this environment variable in your production host before starting Nuxt:

```bash
NUXT_API_PROXY_TARGET=https://your-prod-backend.example.com
```

Then build and run Nuxt normally:

```bash
npm run build
npm run preview
```

On a real deployment target, start the built Nitro server with your platform's normal Nuxt/Nitro startup command and make sure `NUXT_API_PROXY_TARGET` is present in the environment.

## How to use Nitro in code

Frontend code should use:

```ts
const config = useRuntimeConfig()
await $fetch(`${config.public.apiBase}/countries/123`)
```

`config.public.apiBase` is `/api`, so the browser always stays on the Nuxt origin and Nitro handles the backend switch for DEV vs PROD.

## Test

```bash
npm test
```
