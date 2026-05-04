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

const { user, fetchUser } = useAuth()
const { triggerAppBanner, openOnboarding } = useUIState()

onMounted(async () => {
  const token = route.query.token as string | undefined
  const newUser = route.query.newUser as string | undefined
  const pendingId = route.query.pendingRegistrationId as string | undefined
  const suggested = route.query.suggestedUsername as string | undefined

  // Save JWT (might be a temporary onboarding token if newUser === 'true').
  if (token) {
    localStorage.setItem('token', token)
  }

  if (newUser === 'true') {
    // New user: do NOT call fetchUser here. The token we just saved is a
    // temporary onboarding token; /auth/me would reject it and useAuth.fetchUser
    // would call logout(), wiping the token before UsernameModal can post to
    // /auth/complete-onboarding. The "Welcome, ${user} 👋" banner is fired
    // later by the SuccessModal close handler in layouts/default.vue.
    openOnboarding(pendingId || null, suggested || null)
  } else {
    // Returning user: load profile, then greet by username (skip the banner
    // entirely if the name is empty so we never render "Welcome back,  👋").
    await fetchUser()

    const name = user.value?.username
    if (name) {
      triggerAppBanner(`Welcome back, ${name} 👋`)
    }
  }

  // Strip query params so a refresh doesn't re-fire the banner / re-open onboarding.
  router.replace('/')
})
</script>

<template>
  <div class="auth-callback">
    <p>Signing you in...</p>
  </div>
</template>