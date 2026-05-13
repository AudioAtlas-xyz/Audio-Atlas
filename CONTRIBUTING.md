# Contributing to Audio Atlas

Thank you for your interest in contributing to Audio Atlas, the living map of the world's music. Whether you are fixing a bug, adding a feature, improving documentation, or contributing genre knowledge, your help makes the project better for everyone.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How to Contribute](#how-to-contribute)
- [Project Structure](#project-structure)
- [Development Setup](#development-setup)
- [Pull Request Process](#pull-request-process)
- [Licensing and Copyright](#licensing-and-copyright)
- [Cultural Sensitivity Guidelines](#cultural-sensitivity-guidelines)
- [Getting Help](#getting-help)

---

## Code of Conduct

Audio Atlas is dedicated to making global music culture more discoverable and interconnected. We expect all contributors to treat each other and the musical cultures represented in this project with respect. Be kind, be constructive, and assume good intentions.

We do not tolerate harassment, discrimination, or disrespectful behavior of any kind. Contributors who violate these expectations may be removed from the project.

---

## How to Contribute

### Contributing Code

1. Check existing issues in the [Issues](../../issues) tab to see whether someone is already working on the change.
2. Fork the repository and create a branch from `main`.
3. Use a descriptive branch name, such as `feature/genre-search-filters`, `fix/map-zoom-on-mobile`, or `docs/update-setup`.
4. Follow the conventions already used in the affected backend or frontend area.
5. Run the relevant checks before opening a pull request.
6. Submit a focused pull request and link the related issue when one exists.

### Contributing Genre Data

Audio Atlas is community-powered. If you have knowledge about a music genre that is missing or incomplete, you can contribute through either the website or the repository.

**Via the website:** Use the contribution form on the Audio Atlas site to propose a new genre or suggest edits to an existing one. Submissions go through validation, AI-assisted checks, and human moderation before publication.

**Via the repository:** Submit changes to the seed data in `Backend/src/AudioAtlasInfrastructure/resources/` by pull request. Keep additions aligned with the existing JSON schema and include the fields already used by nearby records, such as:

- Genre name and known aliases
- Country or region of origin
- Short description
- Related genres
- Instruments, where applicable
- Sources or context for verification

When contributing genre data, include your sources or explain your personal connection to the genre. First-hand cultural knowledge is highly valued, but it should still be presented clearly enough for maintainers to review.

### Reporting Bugs

Open an issue with the label `bug` and include:

- A clear description of the problem
- Steps to reproduce it
- What you expected to happen
- What actually happened
- Browser, device, and screen size when relevant
- Screenshots or screen recordings when helpful

### Suggesting Features

Open an issue with the label `enhancement` and describe:

- The problem or opportunity
- Your proposed solution
- How it would benefit Audio Atlas users
- Any technical considerations you are aware of

---

## Project Structure

```text
Audio-Atlas/
|-- Backend/
|   |-- AudioAtlasBackend.slnx
|   |-- docker-compose.yml
|   |-- README.md
|   |-- src/
|   |   |-- AudioAtlasApplication/
|   |   |-- AudioAtlasDomain/
|   |   |-- AudioAtlasInfrastructure/
|   |   `-- AudioAtlasView/
|   `-- test/
|       `-- AudioAtlasInfrastructureTests/
|-- frontend/
|   |-- app/
|   |-- public/
|   |-- server/
|   |-- tests/
|   |-- package.json
|   `-- README.md
|-- docs/
|-- README.md
|-- CONTRIBUTING.md
|-- LICENSE
`-- LICENSE-CONTENT
```

The backend is a .NET 10 ASP.NET Core API with Entity Framework Core and SQL Server. The frontend is a Nuxt 4 application using npm.

---

## Development Setup

Use the component READMEs as the source of truth for detailed setup:

- Backend setup and run instructions: `Backend/README.md`
- Frontend environment and API proxy instructions: `frontend/README.md`

Current prerequisites:

- .NET 10 SDK
- Node.js 22 for frontend builds
- npm, using the frontend lockfile in `frontend/package-lock.json`
- Docker, if you want to run the local SQL Server from `Backend/docker-compose.yml`

Typical local workflow:

```bash
cd Backend
docker compose up -d
dotnet restore
dotnet user-secrets set --project src/AudioAtlasView/AudioAtlas.API.csproj "Jwt:Key" "replace-with-a-long-local-development-secret"
dotnet run --project src/AudioAtlasView/AudioAtlas.API.csproj
```

In a second terminal:

```bash
cd frontend
npm ci
npm run dev
```

The backend development profile listens on `http://localhost:5085`. The frontend development server proxies `/api/...` requests to that backend by default. If you need explicit frontend configuration, create `frontend/.env` as described in `frontend/README.md`.

OAuth login requires local Google and GitHub credentials. Most API and frontend work can run without using those login flows, but authentication changes need the matching `Authentication:Google:*` and `Authentication:GitHub:*` secrets configured locally.

### Checks

Run the checks that match the area you changed:

```bash
cd Backend
dotnet build
dotnet test
```

```bash
cd frontend
npm run lint
npm run typecheck
npm run test
npm run build
```

GitHub Actions run backend build/test/deploy checks for changes under `Backend/**` and frontend build/test/deploy checks for changes under `frontend/**`.

---

## Pull Request Process

1. Keep PRs focused on one feature, fix, or documentation update.
2. Write a clear description explaining what changed, why it changed, and how it was tested.
3. Ensure the relevant backend and frontend checks pass.
4. Maintain accessibility. Audio Atlas targets WCAG AA compliance.
5. Be responsive to review feedback.

### Branch Naming Conventions

- `feature/` for new features, for example `feature/vibe-filter-sliders`
- `fix/` for bug fixes, for example `fix/search-results-overlap`
- `docs/` for documentation changes, for example `docs/update-readme-setup`
- `data/` for genre data additions or corrections, for example `data/add-west-african-genres`

### Commit Messages

Write clear, descriptive commit messages. Use the present tense and keep the first line under 72 characters:

```text
Add fuzzy search for genre names
Fix map rendering on Safari mobile
Add Caribbean genre seed data
Update contribution form validation
```

---

## Licensing and Copyright

Audio Atlas uses two licences:

| What | Licence | File |
| --- | --- | --- |
| Source code | MIT License | `LICENSE` |
| Genre data, descriptions, and content | Creative Commons BY-NC-SA 4.0 | `LICENSE-CONTENT` |

By submitting a contribution to this project, you agree that:

- Your code contributions are licensed under the MIT License.
- Your content contributions are licensed under Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0).
- You grant Audio Atlas, and any future legal entity operating Audio Atlas, the right to offer your contributions under a separate commercial licence.
- You have the right to make the contribution because it is your own work or you have permission to share it under these terms.
- You understand your contribution will be publicly visible and may be used by others in accordance with these licences.

Audio Atlas genre data is free for non-commercial use under CC BY-NC-SA 4.0. Commercial product or service usage requires a separate commercial licence from the maintainers.

All contributors are credited in the project. Code contributors appear in Git history and may be listed in a contributors file. Content contributors are credited on the genre pages they helped create or enrich.

---

## Cultural Sensitivity Guidelines

Audio Atlas documents music from cultures around the world. This comes with responsibility. Please follow these guidelines when contributing genre data.

**Respect cultural origins.** When describing a genre's origins, credit the communities and cultures that created it. Do not attribute a genre to a country or group that popularized it commercially if it originated elsewhere.

**Use appropriate terminology.** Prefer the terms that communities use to describe their own music. If a genre has multiple names, list the original or most culturally significant name first, with aliases noted.

**Flag sensitive content.** Some music is tied to sacred, ceremonial, or private cultural practices. If you are unsure whether specific musical knowledge should be publicly documented, flag it in your contribution and the moderation process can handle it carefully.

**Do not speculate.** If you do not have reliable knowledge about a genre's history or cultural context, say so or leave those fields for someone who does. Incomplete but accurate data is better than comprehensive but wrong data.

**Cite your sources.** Whether your knowledge comes from personal experience, academic research, or community elders, mention your source. This helps maintainers verify contributions and helps future readers assess reliability.

---

## Getting Help

- Questions about contributing: open an issue with the label `question`.
- Development setup problems: check `Backend/README.md` or `frontend/README.md`, then open an issue if you are still stuck.
- Ideas before implementation: open an issue and describe the user need, proposed approach, and expected impact.

### Maintainers

- **Jed Anang**: Product owner, genre data, project strategy
- **Christophe Berbec**: Design, interaction, visual experience

---

Thank you for helping build the living map of the world's music.
