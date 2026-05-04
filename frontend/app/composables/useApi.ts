export const useApi = () => {
  const config = useRuntimeConfig()

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

    return await $fetch<T>(`${config.public.apiBase}${url}`, {
      ...options,
      headers: {
        ...(options.headers || {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {})
      }
    })
  }

  return { api }
}