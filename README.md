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

The frontend expects the backend at `http://localhost:5085` by default. Override
the Nuxt runtime configuration in `frontend/.env` when using a different API URL.

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
npm run test
npm run build
```

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

## Maintainers

- **Jed Anang** - Product owner, strategy
- **Christophe Berbec** - Design, interaction

## Team 14

- **Alexander Olsen** - Scrum Master
- **Camilla Froekjaer Joergensen** - Developer
- **Camille Holmskov Larsen** - Developer
- **Anna Rasmussen** - Developer
- **Ditte Astof Hansen** - Developer
- **Alfred Oersted Damgaard** - Developer
- **Noah Leerbeck Van Wagenen** - Developer
- **Andreas John-Holaus** - Developer
- **Philip Bay Quorning** - Developer
- **Freja Skakke Joergensen** - Developer

## Links

- [Production site](https://audioatlas.xyz)
- [Backend API](https://api-audioatlas.azurewebsites.net)
