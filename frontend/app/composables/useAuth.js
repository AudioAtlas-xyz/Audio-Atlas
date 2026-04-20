import { ref } from 'vue'

const user = ref(null)

const fetchUser = async () => {
  const token = localStorage.getItem('token')
  if (!token) return

  try {
    user.value = await $fetch('http://localhost:5000/api/auth/me', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
  } catch {
    user.value = null
  }
}

const logout = () => {
  localStorage.removeItem('token')
  user.value = null
}

export const useAuth = () => ({
  user,
  fetchUser,
  logout
})