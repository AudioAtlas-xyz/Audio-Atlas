import type { NestedGenre } from './nestedgenre'

export interface ContributorSummary {
  id: string
  username: string
  genreCount?: number
}

export interface Country {
  id: string
  name: string
  description: string
  genres: NestedGenre[]
  continent?: string
  region?: string
  contributors?: ContributorSummary[]
}