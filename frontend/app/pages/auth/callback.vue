<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useHead } from '#imports'
import { onMounted } from 'vue'

// Read immediately at module evaluation time, before hydration
const _search = typeof window !== 'undefined' ? window.location.search : ''
const _params = new URLSearchParams(_search)
const _token = _params.get('token')
const _newUser = _params.get('newUser')
const _pendingRegistrationId = _params.get('pendingRegistrationId')
const _suggestedUsername = _params.get('suggestedUsername')

const router = useRouter()

useHead({
  title: 'Signing in...'
})

onMounted(() => {
  // Save JWT so fetchUser() in the layout can pick it up
  if (_token) {
    localStorage.setItem('token', _token)
  }

  // Persist post-login intent to sessionStorage so it survives the
  // client-side navigation to the prerendered home page (which resets useState).
  if (_newUser === 'true' && _pendingRegistrationId) {
    sessionStorage.setItem('pendingRegistrationId', _pendingRegistrationId)
    if (_suggestedUsername) {
      sessionStorage.setItem('suggestedUsername', _suggestedUsername)
    }
  } else if (_newUser === 'false') {
    sessionStorage.setItem('showWelcomeBanner', 'true')
  }

  // Navigate home — default.vue layout onMounted handles the banner/modal.
  router.replace('/')
})
</script>

<template>
  <div class="auth-callback">
    <p>Signing you in...</p>
  </div>
</template>
