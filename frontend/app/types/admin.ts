// Contributor = no elevated Identity role.
export type AdminUserRole = 'Admin' | 'Curator' | 'Banned' | 'Contributor'

export interface AdminUserRow {
  id: string
  username: string
  email: string
  role: AdminUserRole
  // ISO 8601 joined-at, backed by AcceptedPrivacyPolicyAtUtc
  memberSince: string | null
}
