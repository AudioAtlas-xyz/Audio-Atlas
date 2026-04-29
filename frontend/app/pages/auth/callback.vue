<script setup lang="ts">
import { useRouter, useRoute } from 'vue-router'
import { useHead } from '#imports'
import { onMounted } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { useUIState } from '@/composables/useUIState'

const router = useRouter()
const route = useRoute()

useHead({
  title: 'Signing in...'
})

const { fetchUser } = useAuth()
const { triggerLoginBanner, openOnboarding } = useUIState()

onMounted(async () => {
  const token = route.query.token as string | undefined
  const newUser = route.query.newUser as string | undefined
  const pendingId = route.query.pendingRegistrationId as string | undefined
  const suggested = route.query.suggestedUsername as string | undefined

  // Save JWT
  if (token) {
    localStorage.setItem('token', token)
  }

  // Load user
  await fetchUser()

  // Existing user
  if (newUser === 'false') {
    triggerLoginBanner()
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