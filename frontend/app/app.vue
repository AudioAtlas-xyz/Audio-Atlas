<script setup>
import { onMounted } from 'vue'
import { useAuth } from '@/composables/useAuth'

const { fetchUser } = useAuth()

onMounted(async () => {
  const rawHash = window.location.hash

  if (rawHash) {
    const params = new URLSearchParams(rawHash.slice(1))

    const token = params.get('token')
    const newUser = params.get('newUser')

    if (token) {
      localStorage.setItem('token', token)

      if (newUser === 'false') {
        localStorage.setItem('showLoginBanner', 'true')
      }

      if (newUser === 'true') {
        localStorage.setItem('showUsernameModal', 'true')
      }

      window.history.replaceState(null, '', '/')

      return
    }
  }

  await fetchUser()
})
</script>

<template>
  <UApp>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </UApp>
</template>