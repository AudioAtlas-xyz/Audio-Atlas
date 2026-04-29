export interface AuthUser {
  userId: string
  email: string
  username?: string
  provider?: 'google' | 'github'
}