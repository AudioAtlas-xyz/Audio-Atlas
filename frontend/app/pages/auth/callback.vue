<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useHead } from '#imports'
import { onMounted } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { useUIState } from '@/composables/useUIState'

const router = useRouter()

if (import.meta.client) console.log('callback script setup running', window.location.href)

useHead({
  title: 'Signing in...'
})

const { user, fetchUser } = useAuth()
const { triggerAppBanner, openOnboarding } = useUIState()

onMounted(async () => {
  console.log('callback onMounted firing', window.location.search)
  console.log('[auth/callback] window.location.search:', window.location.search)
  const params = new URLSearchParams(window.location.search)
  const token = params.get('token')
  const newUser = params.get('newUser')
  const pendingId = params.get('pendingRegistrationId')
  const suggested = params.get('suggestedUsername')

  // Save JWT
  if (token) {
    localStorage.setItem('token', token)
    console.log('token stored', token.substring(0, 20))
  }

  // Load user
  await fetchUser()

  // Existing user
  if (newUser === 'false') {
    const name = user.value?.username || ''
    triggerAppBanner(`Welcome back, ${name} 👋`)
}

  // New user
  if (newUser === 'true') {
    openOnboarding(pendingId || null, suggested || null)
  }

  // Clean redirect
  router.replace('/')
})
</script>

<template>
  <div class="auth-callback">
    <p>Signing you in...</p>
  </div>
</template>