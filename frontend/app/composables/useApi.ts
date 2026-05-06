export const useApi = () => {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase.replace(/\/+$/, '')

  const getToken = () => {
    if (process.client) {
      return localStorage.getItem('token')
    }
    return null
  }

  const api = async <T>(
    url: string,
    options: Partial<Parameters<typeof $fetch>[1]> = {}
  ) => {
    const token = getToken()

    return await $fetch<T>(`${apiBase}${url.startsWith('/') ? url : `/${url}`}`, {
      ...options,
      headers: {
        ...(options.headers || {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {})
      }
    })
  }

  return { api }
}
