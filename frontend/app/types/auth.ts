// Roles seeded in DbInitializer.cs.
export type UserRole = 'Admin' | 'Curator' | 'Banned'

export interface AuthUser {
  userId: string
  email: string
  username: string
  provider?: string
  // Roles assigned via Identity. Returned by /auth/me and embedded in
  // the JWT. Optional so legacy tokens without roles still parse.
  roles?: UserRole[]
}
