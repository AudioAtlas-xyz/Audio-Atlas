# Audio Atlas Backend

The backend is a .NET 10 ASP.NET Core API using Entity Framework Core, ASP.NET Identity, JWT authentication, Google/GitHub OAuth, and SQL Server.

## Prerequisites

Install these before running the backend locally:

- .NET 10 SDK
- Docker Desktop or another Docker engine, used for the local SQL Server container
- A terminal with the `dotnet` and `docker` commands available
- Optional: SQL Server tooling such as Azure Data Studio or SQL Server Management Studio for inspecting the local database

Verify the required tools:

```bash
dotnet --version
docker --version
```

## Project Layout

```text
Backend/
|-- AudioAtlasBackend.slnx
|-- docker-compose.yml
|-- src/
|   |-- AudioAtlasApplication/
|   |-- AudioAtlasDomain/
|   |-- AudioAtlasInfrastructure/
|   `-- AudioAtlasView/
`-- test/
    `-- AudioAtlasInfrastructureTests/
```

`AudioAtlasView` is the API startup project. `AudioAtlasInfrastructure` contains the EF Core database context, migrations, repositories, and seed resources.

## Local Configuration

Development settings are loaded from `src/AudioAtlasView/appsettings.json`, `src/AudioAtlasView/appsettings.Development.json`, and local user secrets.

The development connection string expects SQL Server on `localhost:1433`:

```text
Server=localhost,1433;Database=AudioAtlasDb;User Id=sa;Password=password123!;Encrypt=False;TrustServerCertificate=True;
```

Do not commit real secrets to `appsettings*.json`. Use .NET user secrets for local secret values.

### Required Local Secrets

The API requires `Jwt:Key` at startup. Set it once from the `Backend/` directory:

```bash
dotnet user-secrets set --project src/AudioAtlasView/AudioAtlas.API.csproj "Jwt:Key" "replace-with-a-long-local-development-secret"
```

Use a long random value for real local work. The placeholder above is only an example.

### Optional OAuth Secrets

Google and GitHub login flows need provider credentials. Most API work can run without using those login flows, but authentication changes should configure the relevant secrets:

```bash
dotnet user-secrets set --project src/AudioAtlasView/AudioAtlas.API.csproj "Authentication:Google:ClientId" "your-google-client-id"
dotnet user-secrets set --project src/AudioAtlasView/AudioAtlas.API.csproj "Authentication:Google:ClientSecret" "your-google-client-secret"
dotnet user-secrets set --project src/AudioAtlasView/AudioAtlas.API.csproj "Authentication:GitHub:ClientId" "your-github-client-id"
dotnet user-secrets set --project src/AudioAtlasView/AudioAtlas.API.csproj "Authentication:GitHub:ClientSecret" "your-github-client-secret"
```

To inspect local secrets:

```bash
dotnet user-secrets list --project src/AudioAtlasView/AudioAtlas.API.csproj
```

## Restore and Build

From the `Backend/` directory:

```bash
dotnet restore
dotnet build
```

Build the release configuration when matching CI more closely:

```bash
dotnet build --configuration Release --no-restore
```

## Run Locally

Start SQL Server:

```bash
docker compose up -d
```

Run the API:

```bash
dotnet run --project src/AudioAtlasView/AudioAtlas.API.csproj
```

The development launch profile listens on:

- `http://localhost:5085`
- `https://localhost:7035` when using the HTTPS profile

On startup, the API runs EF Core migrations and seeds the database. If SQL Server is not running, the connection string is wrong, or `Jwt:Key` is missing, startup will fail.

The frontend development server expects the backend at `http://localhost:5085` unless overridden in `frontend/.env`.

## Test

Run all backend tests from the `Backend/` directory:

```bash
dotnet test
```

To avoid rebuilding after a successful build:

```bash
dotnet test --no-build
```

The current test suite uses SQLite and EF Core InMemory providers, so Docker SQL Server is not required for normal local test runs. CI still starts SQL Server for backend test jobs to mirror the deployed database environment.

## Publish

Create a release build artifact:

```bash
dotnet publish --configuration Release --output publish
```

GitHub Actions builds, tests, publishes, and deploys backend changes under `Backend/**`.

## Common Issues

If startup fails with `JWT Key not configured`, set the required `Jwt:Key` user secret.

If startup fails while connecting to SQL Server, confirm Docker is running and the `audioatlas-sql` container is healthy:

```bash
docker compose ps
```

If port `1433` is already in use, stop the conflicting SQL Server instance or update both `docker-compose.yml` and the development connection string.
