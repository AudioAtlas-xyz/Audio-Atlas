// Roles known to the backend, plus a UI-only "Member" bucket for users
// with no role assignment.
export type AdminUserRole = 'Admin' | 'Curator' | 'Banned' | 'Member'

// One row of the admin user table.
export interface AdminUserRow {
  id: string
  username: string
  email: string
  role: AdminUserRole
  // Joined-at timestamp (ISO 8601). Backed by AcceptedPrivacyPolicyAtUtc.
  memberSince: string | null
}
