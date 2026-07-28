export interface RegionGrouping {
  region: string
  genreCount: number
}

export interface Grouping {
  continent: string
  genreCount: number
  regions: RegionGrouping[]
}
