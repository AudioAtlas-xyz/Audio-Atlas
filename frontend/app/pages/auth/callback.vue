<script setup lang="ts">
import { useHead } from '#imports'
import { onMounted } from 'vue'

// Read immediately at module evaluation time, before hydration
const _search = typeof window !== 'undefined' ? window.location.search : ''
const _params = new URLSearchParams(_search)
const _token = _params.get('token')
const _newUser = _params.get('newUser')
const _pendingRegistrationId = _params.get('pendingRegistrationId')
const _suggestedUsername = _params.get('suggestedUsername')

useHead({
  title: 'Signing in...'
})

onMounted(() => {
  // Save JWT so fetchUser() in the layout can pick it up
  if (_token) {
    localStorage.setItem('token', _token)
  }

  // Persist post-login intent to sessionStorage before navigating.
  if (_newUser === 'true' && _pendingRegistrationId) {
    sessionStorage.setItem('pendingRegistrationId', _pendingRegistrationId)
    if (_suggestedUsername) {
      sessionStorage.setItem('suggestedUsername', _suggestedUsername)
    }
  } else if (_newUser === 'false') {
    sessionStorage.setItem('showWelcomeBanner', 'true')
  }

  // Full page reload to / so the app boots fresh with the token already in
  // localStorage. router.replace('/') does a SPA navigation which applies
  // the prerendered / payload and resets all Nuxt useState (auth, banner, etc).
  window.location.replace('/')
})
</script>

<template>
  <div class="auth-callback">
    <p>Signing you in...</p>
  </div>
</template>
