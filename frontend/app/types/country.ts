import type { NestedGenre } from './nestedgenre'

export interface ContributorSummary {
  id: string
  username: string
  displayName?: string
  avatarUrl?: string
  genresCount?: number
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