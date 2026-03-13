# Contributing to Audio Atlas

Thank you for your interest in contributing to Audio Atlas — the living map of the world's music. Whether you are fixing a bug, adding a feature, improving documentation, or contributing genre knowledge, your help makes the project better for everyone.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How to Contribute](#how-to-contribute)
  - [Contributing Code](#contributing-code)
  - [Contributing Genre Data](#contributing-genre-data)
  - [Reporting Bugs](#reporting-bugs)
  - [Suggesting Features](#suggesting-features)
- [Development Setup](#development-setup)
- [Pull Request Process](#pull-request-process)
- [Licensing & Copyright](#licensing--copyright)
- [Cultural Sensitivity Guidelines](#cultural-sensitivity-guidelines)
- [Getting Help](#getting-help)

---

## Code of Conduct

Audio Atlas is a project dedicated to making global music culture more discoverable and interconnected. We expect all contributors to treat each other and the musical cultures represented in this project with respect. Be kind, be constructive, and assume good intentions.

We do not tolerate harassment, discrimination, or disrespectful behavior of any kind. Contributors who violate these expectations may be removed from the project.

---

## How to Contribute

### Contributing Code

1. **Check existing issues** — Look at the [Issues](../../issues) tab to see if someone is already working on what you have in mind. If not, open an issue to discuss your idea before writing code.
2. **Fork the repository** — Create your own fork of the project.
3. **Create a feature branch** — Branch off `main` with a descriptive name: `feature/genre-search-filters` or `fix/map-zoom-on-mobile`.
4. **Write clear, documented code** — Follow the coding conventions already in the project. Add comments where the logic is non-obvious.
5. **Test your changes** — Make sure the app builds and runs correctly. Test on both desktop and mobile if your changes affect the UI.
6. **Submit a pull request** — See [Pull Request Process](#pull-request-process) below.

### Contributing Genre Data

Audio Atlas is community-powered. If you have knowledge about a music genre that is missing or incomplete, you can contribute through two channels:

**Via the website:** Use the contribution form on the Audio Atlas site to propose a new genre or suggest edits to an existing one. Your submission will go through our review process (validation, AI-assisted checks, and human moderation) before being published.

**Via the repository:** If you prefer to work directly with the data, you can submit changes to the Audio Atlas JSON dataset via a pull request. Make sure your additions follow the existing data schema and include all required fields:

- Genre name and known aliases
- Country or region of origin
- Short description
- Related genres (predecessors, sub-genres, similar genres)
- Instruments (where applicable)

When contributing genre data, please include your sources or explain your personal connection to the genre. First-hand cultural knowledge is highly valued.

### Reporting Bugs

Open an issue with the label `bug` and include:

- A clear description of the problem
- Steps to reproduce it
- What you expected to happen
- What actually happened
- Your browser, device, and screen size
- Screenshots if applicable

### Suggesting Features

Open an issue with the label `enhancement` and describe:

- The problem or opportunity
- Your proposed solution
- How it would benefit Audio Atlas users
- Any technical considerations you are aware of

---

### Project Structure
```
audio-atlas/
├── Backend/
│   ├── src/
│       ├── AudioAtlasApplication/
│       ├── AudioAtlasDomain/
│       ├── AudioAtlasInfrastructure/
│       ├── AudioAtlasView/
│   ├── AudioAtlasBackend.slnx/             # Solution file
│   ├── README.md/                          # README for backend developing
├── docs/                                   # ERD diagram
├── Frontend/
│   ├── src/
│       ├── app/
│       ├── public/
│       ├── README.md/                      # README for frontend developing
├── README.md                               # top level README
├── CONTRIBUTING.md                         # This file
├── LICENSE                                 # MIT License (code)
└── LICENSE-CONTENT                         # CC BY-SA 4.0 (genre data and content)
```


---

## Setup
Fork and clone your own version see README.MD for building and running


## Pull Request Process

1. **Keep PRs focused** — One feature or fix per pull request. Large PRs are harder to review and more likely to have merge conflicts.
2. **Write a clear description** — Explain what your PR does, why, and how. Link to the relevant issue if there is one.
3. **Ensure the build passes** — Your PR should not break the existing application.
4. **Maintain accessibility** — Audio Atlas targets WCAG AA compliance. Do not introduce changes that reduce accessibility.
5. **Be responsive to feedback** — Reviewers may request changes. This is normal and collaborative, not adversarial.

### Branch Naming Conventions

- `feature/` — New features (e.g., `feature/vibe-filter-sliders`)
- `fix/` — Bug fixes (e.g., `fix/search-results-overlap`)
- `docs/` — Documentation changes (e.g., `docs/update-readme-setup`)
- `data/` — Genre data additions or corrections (e.g., `data/add-west-african-genres`)

### Commit Messages

Write clear, descriptive commit messages. Use the present tense and keep the first line under 72 characters:

```
Add fuzzy search for genre names with Fuse.js
Fix map not rendering on Safari mobile
Add 12 Caribbean genres to the dataset
Update contribution form validation rules
```

---

## Licensing & Copyright

Audio Atlas uses two licences:

| What | Licence | File |
|------|---------|------|
| Source code (JavaScript, HTML, CSS, config) | MIT License | `LICENSE` |
| Genre data, descriptions, and content | Creative Commons BY-NC-SA 4.0 | `LICENSE-CONTENT` |

**By submitting a contribution to this project, you agree that:**

- Your **code contributions** are licensed under the MIT License.
- Your **content contributions** (genre descriptions, cultural context, artist information) are licensed under Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0).
- You grant Audio Atlas (and any future legal entity such as a foundation operating Audio Atlas) the right to offer your contributions under a separate commercial licence. This enables Audio Atlas to sustain itself through commercial data licensing while keeping all data freely available for non-commercial use.
- You have the right to make the contribution (it is your own work, or you have permission to share it under these terms).
- You understand your contribution will be publicly visible and may be used by others in accordance with these licences.

This approach is modelled on how MusicBrainz / the MetaBrainz Foundation operates: all data is free for non-commercial use, and commercial users pay for a licence. The revenue funds the project's continued development.

### Commercial Use

Audio Atlas genre data is free for non-commercial use under CC BY-NC-SA 4.0. If you want to use the data in a commercial product or service, contact the maintainers to discuss a commercial licence.

### Attribution

All contributors are credited in the project. Code contributors appear in the Git history and may be listed in a CONTRIBUTORS file. Content contributors are credited on the genre pages they helped create or enrich.

---

## Cultural Sensitivity Guidelines

Audio Atlas documents music from cultures around the world. This comes with responsibility. Please follow these guidelines when contributing genre data:

**Respect cultural origins.** When describing a genre's origins, credit the communities and cultures that created it. Do not attribute a genre to a country or group that popularized it commercially if it originated elsewhere.

**Use appropriate terminology.** Prefer the terms that communities use to describe their own music. If a genre has multiple names, list the original or most culturally significant name first, with aliases noted.

**Flag sensitive content.** Some music is tied to sacred, ceremonial, or private cultural practices. If you are unsure whether specific musical knowledge should be publicly documented, flag it in your contribution and our moderation team will consult appropriate sources before publishing.

**Do not speculate.** If you do not have reliable knowledge about a genre's history or cultural context, say so or leave those fields for someone who does. Incomplete but accurate data is better than comprehensive but wrong data.

**Cite your sources.** Whether your knowledge comes from personal experience, academic research, or community elders, mention your source. This helps our moderation team verify contributions and helps future readers assess reliability.

---

## Getting Help

- **Questions about contributing?** Open an issue with the label `question`.
- **Need help with the development setup?** Check the README first, then open an issue if you are stuck.
- **Want to discuss ideas before contributing?** Open an issue or reach out to the maintainers.

### Maintainers

- **Jed Anang** — Product owner, genre data, project strategy
- **Christophe Berbec** — Design, interaction, visual experience

---

Thank you for helping build the living map of the world's music.
