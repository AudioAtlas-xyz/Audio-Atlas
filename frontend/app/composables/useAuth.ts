import { computed } from 'vue'
import type { AuthUser, UserRole } from '~/types/auth'

export const useAuth = () => {
  const user = useState<AuthUser | null>('auth_user', () => null)

  const parseJwt = (token: string): Record<string, any> | null => {
    try {
      const base64 = token.split('.')[1]
      if (!base64) return null

      const normalized = base64.replace(/-/g, '+').replace(/_/g, '/')
      return JSON.parse(atob(normalized))
    } catch {
      return null
    }
  }

  const isExpired = (token: string | null): boolean => {
    if (!token) return true
    const payload = parseJwt(token)
    if (!payload || typeof payload.exp !== 'number') return true
    return payload.exp < Date.now() / 1000
  }

  const fetchUser = async () => {
    if (process.server) return

    const token = localStorage.getItem('token')

    if (!token || isExpired(token)) {
      void logout({ redirect: false })
      return
    }

    const { api } = useApi()

    try {
      user.value = await api<AuthUser>('/auth/me')
    } catch (err) {
      console.error('fetchUser failed:', err)
      logout({ redirect: false })
    }
  }

  // Clears the session and sends the user home. Pass { redirect: false }
  // if the caller wants to handle navigation itself.
  const logout = async (options: { redirect?: boolean } = {}) => {
    if (process.client) {
      localStorage.removeItem('token')
    }

    user.value = null

    if (options.redirect !== false && process.client) {
      await navigateTo('/')
    }
  }

  // Role helpers — both reactive to user.value.
  const hasRole = (role: UserRole) =>
    computed(() => Boolean(user.value?.roles?.includes(role)))

  const isAdmin = computed(() =>
    Boolean(user.value?.roles?.includes('Admin'))
  )

  const isCurator = computed(() =>
    Boolean(user.value?.roles?.includes('Curator'))
  )

  return {
    user,
    fetchUser,
    logout,
    hasRole,
    isAdmin,
    isCurator
  }
}
