# Audio Atlas

**The living map of the world's music.**

Audio Atlas is an open-source, community-powered platform for discovering and
documenting music genres across cultures. The production site is live at
[audioatlas.xyz](https://audioatlas.xyz).

Explore the interactive globe, browse country and genre pages, and contribute
music knowledge through the submission flow.

> This project was developed as part of the Industrial Software Engineering
> course at IT University of Copenhagen (ITU), Spring 2025.

## Status

**Production** - Audio Atlas is live at [https://audioatlas.xyz](https://audioatlas.xyz).

The application is actively maintained. Backend and frontend changes are built,
tested, and deployed through GitHub Actions.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend | Nuxt 4, Vue, Nuxt UI, Tailwind CSS, Three.js/globe.gl |
| Backend | .NET 10 ASP.NET Core API, Entity Framework Core |
| Auth | ASP.NET Identity, JWT, Google/GitHub OAuth |
| Database | SQL Server / Azure SQL |
| Deployment | Azure Static Web Apps frontend, Azure App Service backend |

## Repository Layout

```text
Audio-Atlas/
|-- Backend/     # ASP.NET Core API, domain/application/infrastructure projects, tests
|-- frontend/    # Nuxt 4 app, public assets, frontend tests
|-- docs/        # Architecture, ERD, and setup notes
|-- .github/     # CI/CD workflows (path-filtered: Backend/** and frontend/**)
|-- CLAUDE.md    # Project context and conventions for AI coding assistants
`-- README.md
```

## Running Locally

Detailed setup instructions live in the component READMEs:

- [Backend/README.md](Backend/README.md)
- [frontend/README.md](frontend/README.md)

At a high level:

1. Start the backend dependencies and API from `Backend/`.
2. Start the Nuxt development server from `frontend/`.
3. Use the frontend local URL printed by Nuxt, usually `http://localhost:3000`.

The frontend expects the backend at `http://localhost:5000` by default (set in
`frontend/nuxt.config.ts`). In development the frontend calls `/api`, which Nuxt
proxies to that address, so a backend that is not running shows up as `502` on
every request.

Override with environment variables in `frontend/.env`:

| Variable | Purpose |
|----------|---------|
| `NUXT_API_PROXY_TARGET` | Where the dev server proxies `/api`. Point it at the deployed API to run the frontend without a local backend. |
| `NUXT_PUBLIC_BACKEND_BASE_URL` | Backend origin used to build absolute URLs. |
| `NUXT_PUBLIC_API_BASE` | Overrides the API base directly. Note that pointing the browser at the deployed API from `localhost` fails CORS - prefer `NUXT_API_PROXY_TARGET`. |

## Checks

Backend checks:

```bash
cd Backend
dotnet restore
dotnet build
dotnet test
```

Frontend checks:

```bash
cd frontend
npm ci
npm run lint
npm run typecheck
npm run check:contrast   # WCAG AA gate - also enforced in CI
npm run test
npm run build            # also prerenders, so it fails on any dead internal link
```

`npm run typecheck` and `npm run lint` both carry a small number of pre-existing
findings. Compare against the current state of `main` before treating one as a
regression - see [CLAUDE.md](CLAUDE.md) for the known-good baselines.

## Licensing

This project uses a dual-licence model:

| What | Licence |
|------|---------|
| Source code | [MIT License](LICENSE) |
| Genre data and content | [CC BY-NC-SA 4.0](LICENSE-CONTENT) |

Code is free to use for any purpose. Genre data is free for non-commercial use.
For commercial licensing enquiries, contact the maintainers.

## Contributing

We welcome contributions. Please read [CONTRIBUTING.md](CONTRIBUTING.md) before
getting started.

If you are working with an AI coding assistant, [CLAUDE.md](CLAUDE.md) documents
the architecture, conventions and the non-obvious behaviours that have caused
problems before - notably the two separate database seeders and the fact that
migrations run at application startup.

## Maintainers

- **Jed Anang** - Product owner, strategy
- **Christophe Berbec** - Design, interaction

## Links

- [Production site](https://audioatlas.xyz)
- [Backend API](https://api-audioatlas.azurewebsites.net)
