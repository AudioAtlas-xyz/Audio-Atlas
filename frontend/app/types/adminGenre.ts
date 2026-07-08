export interface AdminGenreLookup {
  id: string
  name: string
}

export interface AdminGenreRow {
  id: string
  name: string
  hasDescription: boolean
  countryNames: string[]
  isArchived: boolean
  archivedAt: string | null
  lastEditedAt: string | null
  rowVersion: string
}

export interface AdminGenrePageResponse {
  items: AdminGenreRow[]
  totalCount: number
  page: number
  pageSize: number
}

export interface AdminGenreDetail {
  id: string
  name: string
  description: string | null
  startYear: number | null
  isSensitive: boolean
  sensitiveDescription: string | null
  playlistLink: string | null
  countries: AdminGenreLookup[]
  instruments: AdminGenreLookup[]
  similarGenres: AdminGenreLookup[]
  subGenres: AdminGenreLookup[]
  parentGenres: AdminGenreLookup[]
  aliases: string[]
  sources: string[]
  isArchived: boolean
  archivedAt: string | null
  archivedByUsername: string | null
  lastEditedAt: string | null
  lastEditedByUsername: string | null
  rowVersion: string
}
