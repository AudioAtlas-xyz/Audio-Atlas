export const useAuth = () => {
  const user = useState('auth_user', () => null)

  const parseJwt = (token) => {
    try {
      return JSON.parse(atob(token.split('.')[1]))
    } catch {
      return null
    }
  }

  const isExpired = (token) => {
    if (!token) return true

    const payload = parseJwt(token)
    if (!payload?.exp) return true

    return payload.exp < Date.now() / 1000
  }

  const fetchUser = async () => {
    if (process.server) return

    const token = localStorage.getItem('token')
    const config = useRuntimeConfig()

    if (!token || isExpired(token)) {
      logout()
      return
    }

    try {
      user.value = await $fetch(`${config.public.apiBase}/api/auth/me`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      })
    } catch {
      logout()
    }
  }

  const logout = () => {
    if (process.client) {
      localStorage.removeItem('token')
    }
    user.value = null
  }

  return {
    user,
    fetchUser,
    logout
  }
}