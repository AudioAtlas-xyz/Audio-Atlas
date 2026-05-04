/**
 * Roles known to the backend Identity layer.
 *
 * Mirrors the seeded role names in `DbInitializer.cs`. Kept as a string
 * union so role checks are type-safe — e.g. `hasRole('Admin')` is checked
 * at compile time and refactor-safe.
 */
export type UserRole = 'Admin' | 'Curator' | 'Banned'

export interface AuthUser {
  userId: string
  email: string
  username: string
  provider?: string

  /**
   * Roles assigned to the user via Identity (AspNetUserRoles).
   *
   * Returned by `/auth/me` and also embedded as JWT claims so the backend
   * can authorize requests via `[Authorize(Roles = "Admin")]`. Optional
   * because legacy tokens issued before the roles feature won't include it
   * — treat `undefined` and `[]` the same (no roles).
   */
  roles?: UserRole[]
}
