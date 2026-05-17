<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useHead } from '#imports'
import { onMounted } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { useUIState } from '@/composables/useUIState'

// Read immediately at module evaluation time, before hydration
const _search = typeof window !== 'undefined' ? window.location.search : ''
const _params = new URLSearchParams(_search)
const _token = _params.get('token')
const _newUser = _params.get('newUser')
const _pendingRegistrationId = _params.get('pendingRegistrationId')
const _suggestedUsername = _params.get('suggestedUsername')
console.log('captured at setup:', _search, _token)

if (import.meta.client) console.log('callback script setup running', window.location.href)

const router = useRouter()

useHead({
  title: 'Signing in...'
})

const { user, fetchUser } = useAuth()
const { triggerAppBanner, openOnboarding } = useUIState()

onMounted(async () => {
  console.log('callback onMounted firing', window.location.search)
  console.log('[auth/callback] window.location.search:', window.location.search)

  // Save JWT
  if (_token) {
    localStorage.setItem('token', _token)
    console.log('token stored', _token.substring(0, 20))
  }

  // Load user
  await fetchUser()

  // Existing user
  if (_newUser === 'false') {
    const name = user.value?.username || ''
    triggerAppBanner(`Welcome back, ${name} 👋`)
  }

  // New user
  if (_newUser === 'true') {
    openOnboarding(_pendingRegistrationId || null, _suggestedUsername || null)
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
