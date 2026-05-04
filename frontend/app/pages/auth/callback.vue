<script setup lang="ts">
import { useRouter, useRoute } from 'vue-router'
import { useHead } from '#imports'
import { onMounted } from 'vue'
import { useAuth } from '@/composables/useAuth'

/**
 * Router utilities
 */
const router = useRouter()
const route = useRoute()

/**
 * Page metadata
 */
useHead({
  title: 'Signing in...'
})

/**
 * Access auth composable
 */
const {
  fetchUser,
  triggerLoginBanner,
  openUsernameModal
} = useAuth()

/**
 * Handle OAuth redirect
 */
onMounted(async () => {
  const token = route.query.token as string | undefined
  const newUser = route.query.newUser as string | undefined
  const pendingId = route.query.pendingRegistrationId as string | undefined
  const suggested = route.query.suggestedUsername as string | undefined

  /**
   * Save JWT
   */
  if (token) {
    localStorage.setItem('token', token)
  }

  /**
   * Load user immediately
   */
  await fetchUser()

  /**
   * Existing user
   */
  if (newUser === 'false') {
    triggerLoginBanner()
  }

  /**
   * New user (onboarding)
   */
  if (newUser === 'true') {
    openUsernameModal(pendingId || null, suggested || null)}

  /**
   * Redirect
   */
  router.replace({
  path: '/',
  query: route.query
})

})
</script>

<template>
  <div class="auth-callback">
    <p>Signing you in...</p>
  </div>
</template>