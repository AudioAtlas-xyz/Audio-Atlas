// Roles assignable from the admin UI. "Contributor" is a UI label for
// users with no elevated Identity role (matches the backend's
// ResolveDisplayRole return value).
export type AdminUserRole = 'Admin' | 'Curator' | 'Banned' | 'Contributor'

// One row of the admin user table.
export interface AdminUserRow {
  id: string
  username: string
  email: string
  role: AdminUserRole
  // Joined-at timestamp (ISO 8601). Backed by AcceptedPrivacyPolicyAtUtc.
  memberSince: string | null
}
