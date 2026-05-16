# Audio Atlas API Documentation

**Base URL:** `https://audioatlasbackend.azurewebsites.net/api`

**OpenAPI / Swagger UI:** Available at `/swagger` in development and staging environments.

---

## Overview

The Audio Atlas API is an ASP.NET Web API serving the Audio Atlas frontend. It exposes endpoints for discovering genres by geography, reading genre detail, submitting new genres for curation, and managing users.

All responses are JSON. Authentication uses **JWT bearer tokens** issued after OAuth login via `Authorization: Bearer <token>`.

---

## Authentication

Audio Atlas uses OAuth (GitHub / Google) via ASP.NET Identity. Most read endpoints are **public**. Write endpoints require a valid token.

| Role | Capabilities |
| :---- | :---- |
| Public | Read countries, read genres, read instruments |
| Contributor | All public \+ submit genres |
| Curator | *(reserved — not yet assigned to any endpoints)* |
| Banned | Can browse but cannot submit |
| Admin | All contributor capabilities \+ approve/reject submissions \+ user management |

Contributors have an empty `roles` array in their JWT — Contributor is the default state and is not stored as an Identity role.

---

## Error Handling

| Status | Meaning |
| :---- | :---- |
| 400 | Bad request — invalid input or missing required field |
| 401 | Unauthenticated — valid bearer token required |
| 403 | Forbidden — insufficient role (includes banned users) |
| 404 | Resource not found |
| 409 | Conflict — e.g. username already taken |
| 500 | Internal server error |

---

## Auth

### `GET /api/auth/login/github`

Initiates the GitHub OAuth flow. Redirects the user to GitHub for authentication.

**Auth:** Public

---

### `GET /api/auth/login/google`

Initiates the Google OAuth flow. Redirects the user to Google for authentication.

**Auth:** Public

---

### `GET /api/auth/external-callback`

OAuth redirect handler. Called by the provider after the user authenticates. Not intended to be called directly.

On success, redirects to the frontend with either:

- `?token=<jwt>&newUser=false` — existing user, ready to use  
- `?newUser=true&pendingRegistrationId=<uuid>&suggestedUsername=<string>` — new user, onboarding required

Pending registrations expire after **15 minutes**.

---

### `GET /api/auth/check-username`

Checks whether a username is available. Used during onboarding.

**Auth:** Public

**Query parameters:**

| Parameter | Type | Description |
| :---- | :---- | :---- |
| `username` | `string` | Username to check |

Usernames must be 3–20 characters and contain only letters, numbers, or underscores (`^[a-zA-Z0-9_]{3,20}$`).

**Response `200 OK`:**

{

  "available": true,

  "message": "Username is available."

}

---

### `POST /api/auth/complete-onboarding`

Finalises registration for a new OAuth user. Creates the account, records policy consent timestamps, and returns a JWT.

**Auth:** Public (uses `pendingRegistrationId` from the callback redirect)

**Request body:**

{

  "pendingRegistrationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

  "username": "cooluser",

  "acceptedPrivacyPolicy": true,

  "acceptedContributionGuidelines": true

}

**Response `200 OK`:**

{

  "requiresOnboarding": false,

  "token": "\<jwt\>"

}

**Errors:**

- `400` — policies not accepted, invalid username format, or registration expired  
- `404` — pending registration not found  
- `409` — username already in use

---

### `GET /api/auth/me`

Returns the authenticated user's profile and roles.

**Auth:** Any authenticated user

**Response `200 OK`:**

{

  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

  "email": "user@example.com",

  "username": "cooluser",

  "provider": "github",

  "roles": \["Curator"\]

}

---

### `PUT /api/auth/username`

Updates the authenticated user's username.

**Auth:** Any authenticated user

**Request body:**

{

  "username": "newusername"

}

**Response `200 OK`:**

{

  "username": "newusername"

}

**Errors:**

- `400` — invalid format  
- `409` — username already in use

---

## Countries

### `GET /api/countries`

Returns a map of ISO country code to genre count. Used to drive globe marker sizing.

**Auth:** Public

**Response `200 OK`:**

{

  "BR": 24,

  "NG": 18,

  "IN": 31

}

---

### `GET /api/countries/all`

Returns the full list of countries with metadata.

**Auth:** Public

**Response `200 OK`:**

\[

  {

    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

    "name": "Brazil",

    "region": "South America",

    "continent": "Americas",

    "isoCode": "BR",

    "description": "Brazil is the largest country in South America..."

  }

\]

---

### `GET /api/countries/{key}`

Returns a single country by its ISO code, including its genres and contributor summary.

**Auth:** Public

**Path parameters:**

| Parameter | Type | Description |
| :---- | :---- | :---- |
| `key` | `string` | ISO country code (e.g. `"BR"`) |

**Response `200 OK`:**

{

  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

  "name": "Brazil",

  "description": "Brazil is the largest country in South America...",

  "region": "South America",

  "continent": "Americas",

  "isoCode": "BR",

  "contributors": \[

    {

      "id": "user-uuid",

      "username": "cooluser",

      "genreCount": 3

    }

  \],

  "genres": \[

    {

      "id": "7cb8a123-...",

      "name": "Bossa Nova",

      "summary": "A sophisticated style of samba...",

      "isSensitive": false

      // ... full GenreDTO shape — see GET /api/genres/{id}

    }

  \]

}

---

### `GET /api/countries/{id}/genres`

Returns all genres associated with a country as domain entities.

**Auth:** Public

**Path parameters:**

| Parameter | Type | Description |
| :---- | :---- | :---- |
| `id` | `uuid` | Country UUID |

**Response `200 OK`:**

\[

  {

    "id": "7cb8a123-...",

    "name": "Bossa Nova",

    "description": "A sophisticated style...",

    "summary": "Jazz-influenced samba from Rio.",

    "startYear": 1958,

    "isSensitive": false,

    "playlistLink": "https://open.spotify.com/playlist/..."

  }

\]

---

## Genres

### `GET /api/genres`

Returns all genres as an array of `GenreDTO`.

**Auth:** Public

**Response `200 OK`:** Array of `GenreDTO` — see shape below under `GET /api/genres/{id}`.

---

### `GET /api/genres/search/{keyword}`

Searches genres by keyword against name and aliases.

**Auth:** Public

**Path parameters:**

| Parameter | Type | Description |
| :---- | :---- | :---- |
| `keyword` | `string` | Search term |

**Response `200 OK`:** Array of matching `GenreDTO` objects — see shape below.

---

### `GET /api/genres/{id}`

Returns full detail for a single genre.

**Auth:** Public

**Path parameters:**

| Parameter | Type | Description |
| :---- | :---- | :---- |
| `id` | `uuid` | Genre UUID |

**Response `200 OK`:**

{

  "id": "7cb8a123-4562-b3fc-2c963f66afa6",

  "authorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

  "name": "Bossa Nova",

  "description": "A sophisticated style of samba that emerged in the late 1950s in Rio de Janeiro...",

  "summary": "Jazz-influenced samba from Rio de Janeiro.",

  "startYear": 1958,

  "isSensitive": false,

  "sensitiveDescription": null,

  "playlistLink": "https://open.spotify.com/playlist/...",

  "countries": \[

    {

      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

      "name": "Brazil",

      "isoCode": "BR",

      "region": "South America",

      "continent": "Americas",

      "description": null,

      "contributors": \[\],

      "genres": \[\]

    }

  \],

  "instruments": \[

    {

      "id": "9d12c456-...",

      "type": "Classical Guitar",

      "description": "A nylon-string acoustic guitar central to bossa nova."

    }

  \],

  "aliases": \[

    { "alias": "bossa-nova" },

    { "alias": "new wave samba" }

  \],

  "sources": \[

    { "sourceLink": "https://example.com/bossa-nova-history" }

  \],

  "similarGenres": \[

    { "id": "...", "name": "MPB", "summary": "...", ... }

  \],

  "subGenres": \[

    { "id": "...", "name": "Samba-Jazz", "summary": "...", ... }

  \],

  "parentGenres": \[

    { "id": "...", "name": "Samba", "summary": "...", ... }

  \]

}

`similarGenres`, `subGenres`, and `parentGenres` are each arrays of `GenreDTO` with the same shape as the parent object.

Returns `null` if the genre does not exist.

---

## Instruments

### `GET /api/instruments`

Returns all instruments. Used to populate the instrument selector on the contribution form.

**Auth:** Public

**Response `200 OK`:**

\[

  {

    "id": "9d12c456-5717-4562-b3fc-2c963f66afa6",

    "type": "Berimbau",

    "description": "A single-string percussion instrument of African origin, central to capoeira."

  }

\]

Note: the instrument name is stored in the `type` field, not `name`.

---

## Submissions

All submission endpoints require authentication (`[Authorize]`). The pending queue and approve/reject actions are restricted to **Admin** only.

### `POST /api/submissions`

Creates a new genre submission with status `pending`.

**Auth:** Any authenticated user (Banned users receive `403`)

**Request body:**

{

  "newGenreName": "Cumbia",

  "description": "A musical genre and folk dance tradition from the Caribbean coast of Colombia...",

  "startDate": "1940-01-01",

  "endDate": null,

  "isSensitive": false,

  "sensitiveDescription": null,

  "playlistLink": "https://open.spotify.com/playlist/...",

  "aliases": \["cumbia colombiana"\],

  "sourceLinks": \["https://example.com/cumbia-history"\],

  "countryIds": \["3fa85f64-5717-4562-b3fc-2c963f66afa6"\],

  "instrumentIds": \["9d12c456-5717-4562-b3fc-2c963f66afa6"\],

  "similarGenreIds": \["7cb8a123-5717-4562-b3fc-2c963f66afa6"\],

  "subGenreIds": \[\],

  "predecessorGenreIds": \[\]

}

**Response `201 Created`:**

{

  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"

}

**Errors:**

- `400` — invalid submission data  
- `403` — account is banned

---

### `GET /api/submissions/pending`

Returns all pending submissions for the curation queue.

**Auth:** Admin only

**Response `200 OK`:**

\[

  {

    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

    "accountId": "user-uuid",

    "accountUsername": "cooluser",

    "newGenreName": "Cumbia",

    "description": "A musical genre...",

    "startDate": "1940-01-01",

    "endDate": null,

    "isSensitive": false,

    "sensitiveDescription": null,

    "playlistLink": null,

    "aliases": \["cumbia colombiana"\],

    "sourceLinks": \["https://example.com/cumbia-history"\],

    "countryIds": \["3fa85f64-5717-4562-b3fc-2c963f66afa6"\],

    "instrumentIds": \["9d12c456-5717-4562-b3fc-2c963f66afa6"\],

    "similarGenreIds": \["7cb8a123-5717-4562-b3fc-2c963f66afa6"\],

    "subGenreIds": \[\],

    "predecessorGenreIds": \[\]

  }

\]

---

### `POST /api/submissions/{id}/approve`

Approves a submission and promotes it to a live genre.

**Auth:** Admin only

**Path parameters:**

| Parameter | Type | Description |
| :---- | :---- | :---- |
| `id` | `uuid` | Submission UUID |

**Response `204 No Content`**

**Errors:**

- `400` — submission cannot be approved (e.g. already reviewed)

---

### `POST /api/submissions/{id}/reject`

Rejects a submission. The submission is retained as an audit record alongside a `RejectedSubmission` record containing the reason.

**Auth:** Admin only

**Path parameters:**

| Parameter | Type | Description |
| :---- | :---- | :---- |
| `id` | `uuid` | Submission UUID |

**Request body:**

{

  "reason": "Duplicate of existing Cumbia entry."

}

**Response `204 No Content`**

**Errors:**

- `400` — submission cannot be rejected (e.g. already reviewed)

---

## User

### `DELETE /api/user/delete`

Permanently deletes the authenticated user's account. Approved genre contributions are anonymised rather than deleted, per GDPR policy.

**Auth:** Any authenticated user

**Response `200 OK`:**

{

  "message": "Account successfully deleted."

}

**Errors:**

- `400` — invalid user ID  
- `500` — deletion failed

---

## Admin

All admin endpoints require `[Authorize(Roles = "Admin")]`.

### `GET /api/admin/users`

Returns all non-system, non-deleted users with their resolved display role.

**Auth:** Admin only

**Response `200 OK`:**

\[

  {

    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",

    "username": "cooluser",

    "email": "user@example.com",

    "role": "Curator",

    "memberSince": "2025-01-15T08:00:00.000Z"

  }

\]

`role` is resolved as the highest applicable Identity role: `Admin` \> `Banned` \> `Curator` \> `Contributor` (default when no role is assigned).

`memberSince` is the UTC timestamp of privacy policy acceptance. May be `null` for users who completed onboarding before this field was introduced.

---

### `PUT /api/admin/users/{id}/role`

Changes a user's role. Assignable values: `Admin`, `Curator`, `Banned`, `Contributor`.

**Auth:** Admin only

**Path parameters:**

| Parameter | Type | Description |
| :---- | :---- | :---- |
| `id` | `uuid` | User UUID |

**Request body:**

{

  "role": "Curator"

}

**Response `200 OK`:**

{

  "role": "Curator"

}

**Errors:**

- `400` — unknown role or failed to apply change  
- `404` — user not found  
- `409` — self-demotion attempt, or last Admin cannot be demoted

---

## Known Gaps

The following behaviours exist by design decision but are not yet implemented in code:

| Gap | Notes |
| :---- | :---- |
| `DELETE /api/user/delete` has no typed confirmation step | Agreed design requires the user to type `"delete my account"` before the request is sent. Currently the endpoint deletes on any authenticated `DELETE` with no body. |
| `GET /api/submissions/{id}` does not exist | No endpoint to fetch a single submission by ID. Needed for the contributor submission history view. |
| Curator role has no assigned endpoints | The role exists in the identity system but `SubmissionsController` gates curation to Admin only. |

---

## Code Notes

Minor inconsistencies found in the domain layer during documentation. Flagged here for the team:

| Location | Issue |
| :---- | :---- |
| `Country.cs` | `isoCode` is lowercase — inconsistent with all other entity properties which use PascalCase. `CountryDTO.cs` correctly uses `IsoCode`. |
| `ContributorSummaryDTO.cs` | Properties use camelCase (`id`, `username`, `genreCount`) — inconsistent with all other DTOs which use PascalCase. |
| `Instrument.cs` | The instrument name is stored in a field called `Type`, which is misleading. `Type` implies a category (e.g. "String", "Percussion") not a name (e.g. "Berimbau"). Consider renaming to `Name`. |
| `GET /api/countries/{id}/genres` | Returns the `Genre` domain entity directly, not `GenreDTO`. All other genre endpoints return `GenreDTO`. This is likely unintentional. |

---

## Changelog

| Version | Sprint | Notes |
| :---- | :---- | :---- |
| 0.1 | Sprint 1 | Countries, genres, instruments |
| 0.2 | Sprint 2 | Genre search, auth flow, onboarding, user deletion |
| 0.3 | Sprint 5 | Submissions, admin user management |

