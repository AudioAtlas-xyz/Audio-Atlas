import type { GenreAlias } from "./genreAlias";
import type { Country, ContributorSummary } from "./country"

export interface Genre {
  id: string
  name: string
  description?: string
  startYear?: number
  isSensitive:boolean
  playlistLink?: string
  sensitiveDescription?: string
  countries: Country[]
  aliases?: GenreAlias[]
  similarGenres: Genre[]
  subGenres: Genre[]
  parentGenres: Genre[]
  contributors: ContributorSummary[]
}
