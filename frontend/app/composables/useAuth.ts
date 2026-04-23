export interface AuthUser {
  userId: string
  email: string
  username?: string
}

export const useAuth = () => {
  /**
   * Global user state
   */
  const user = useState<AuthUser | null>('auth_user', () => null)

  /**
   * UI state
   */
  const showLoginBanner = useState<boolean>('auth_login_banner', () => false)
  const showUsernameModal = useState<boolean>('auth_username_modal', () => false)
  const pendingRegistrationId = useState<string | null>('auth_pending_id', () => null)
  const suggestedUsername = useState<string | null>('auth_suggested_username', () => null)

  /**
   * Decode JWT safely
   */
  const parseJwt = (token: string): Record<string, any> | null => {
    try {
      const base64 = token.split('.')[1]
      if (!base64) return null
      return JSON.parse(atob(base64))
    } catch {
      return null
    }
  }

  /**
   * Check token expiry
   */
  const isExpired = (token: string | null): boolean => {
    if (!token) return true
    const payload = parseJwt(token)
    if (!payload || typeof payload.exp !== 'number') return true
    return payload.exp < Date.now() / 1000
  }

  /**
   * Fetch authenticated user
   */
  const fetchUser = async () => {
    if (process.server) return

    const token = localStorage.getItem('token')
    const config = useRuntimeConfig()

    if (!token || isExpired(token)) {
      logout()
      return
    }

    if (!config.public.backendBaseUrl) {
      console.error('Missing backendBaseUrl')
      return
    }

    try {
      user.value = await $fetch<AuthUser>(
        `${config.public.backendBaseUrl}/api/auth/me`,
        {
          headers: {
            Authorization: `Bearer ${token}`
          }
        }
      )
    } catch {
      logout()
    }
  }

  /**
   * Logout user and clear all state
   */
  const logout = () => {
    if (process.client) {
      localStorage.removeItem('token')
    }

    user.value = null
    showLoginBanner.value = false
    showUsernameModal.value = false
    pendingRegistrationId.value = null
    suggestedUsername.value = null
  }

  /**
   * UI HELPERS
   */

  // Show login banner temporarily
  const triggerLoginBanner = () => {
    showLoginBanner.value = true

    setTimeout(() => {
      showLoginBanner.value = false
    }, 3000)
  }

  // Start onboarding flow
  const openUsernameModal = (id: string | null, username: string | null) => {
    pendingRegistrationId.value = id
    suggestedUsername.value = username
    showUsernameModal.value = true
  }

  // Close onboarding and clean state
  const closeUsernameModal = () => {
    showUsernameModal.value = false
    pendingRegistrationId.value = null
    suggestedUsername.value = null
  }

  return {
    user,
    fetchUser,
    logout,

    // UI state
    showLoginBanner,
    showUsernameModal,
    pendingRegistrationId,
    suggestedUsername,

    // UI actions
    triggerLoginBanner,
    openUsernameModal,
    closeUsernameModal
  }
}