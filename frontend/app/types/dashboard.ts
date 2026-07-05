export interface LabeledCount {
  label: string
  count: number
}

export interface LabeledShare {
  label: string
  count: number
  sharePercent: number
}

export interface SearchTerm {
  term: string
  frequency: number
}

export interface CuratorWorkload {
  reviewerId: string
  reviewerUsername?: string | null
  decisions: number
}

export interface ContributorSummary {
  accountId: string
  username?: string | null
  submissionCount: number
}

export interface ContentGate {
  ready: number
  notReady: number
}

export interface CountryCoverage {
  withGenres: number
  total: number
  gapList: string[]
}

export interface DataCompleteness {
  orphanGenres: number
  missingOriginsNote: number
  missingMedia: number
}

export interface ContributorRetention {
  repeat: number
  oneTime: number
}

export interface CataloguePanel {
  totalGenres: number
  genresByContinent: LabeledCount[]
  genresByRegion: LabeledCount[]
  contentGate: ContentGate
  countryCoverage: CountryCoverage
  geographicBalance: LabeledShare[]
  dataCompleteness: DataCompleteness
  genreCountryLinkCount: number
}

export interface PipelinePanel {
  queueDepth: number
  oldestPendingAgeDays?: number | null
  approvedThisMonth: number
  approvedLastMonth: number
  rejectedThisMonth: number
  rejectedLastMonth: number
  approvalRate?: number | null
  medianTimeToReviewHours?: number | null
  curatorWorkload: CuratorWorkload[]
  rejectionBreakdown: LabeledCount[]
  sensitivityHolds: number
}

export interface CommunityPanel {
  usersByRole: LabeledCount[]
  newSignupsThisMonth: number
  activeContributors: number
  contributorRetention: ContributorRetention
  topContributors: ContributorSummary[]
}

export interface DiscoveryPanel {
  zeroResultSearches: SearchTerm[]
  topSearches: SearchTerm[]
}

export interface CostPanel {
  azureMonthlySpend?: number | null
  source: string
}

export interface DashboardResponse {
  earliestReviewAt?: string | null
  catalogue: CataloguePanel
  pipeline: PipelinePanel
  community: CommunityPanel
  discovery: DiscoveryPanel
  cost: CostPanel
}
