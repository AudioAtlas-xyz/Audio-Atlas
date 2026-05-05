/**
 * Roles surfaced in admin UI.
 *
 * `Member` is a UI-only category for users with no role assignment in
 * Identity (i.e. the join row in `AspNetUserRoles` doesn't exist). The
 * backend has no `Member` role; the admin frontend invents the label
 * so "no role" is filterable and badge-able like any other category.
 */
export type AdminUserRole = 'Admin' | 'Curator' | 'Banned' | 'Member'

/**
 * One row of the admin user table.
 *
 * Shape is intentionally flat — a single API call returning
 * `AdminUserRow[]` is what the backend will eventually serve. Until
 * then, `pages/admin/users.vue` populates this from a mock array.
 */
export interface AdminUserRow {
  id: string
  username: string
  email: string
  role: AdminUserRole
  /** ISO 8601 timestamp — used for both display and sort. */
  memberSince: string
  submissionCount: number
  approvedSubmissionCount: number
}
