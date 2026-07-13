<script setup lang="ts">
import { ref, onMounted } from 'vue'
import AppHeader from '@/components/AppHeader.vue'
import LoginModal from '~/components/UserFlow/LoginModal.vue'
import UsernameModal from '~/components/UserFlow/UsernameModal.vue'
import SuccessModal from '~/components/UserFlow/SuccessModal.vue'
import AppBanner from '@/components/Banners/AppBanner.vue'

import { useAuth } from '@/composables/useAuth'
import { useUIState } from '@/composables/useUIState'

const { fetchUser, user } = useAuth()

const {
  bannerMessage,
  pendingRegistrationId,
  openOnboarding,
  triggerAppBanner
} = useUIState()

// Login modal is layout-local: only opened by clicking "Sign in" in AppHeader.
const showLoginModal = ref(false)

// Account-created success popup — shown after UsernameModal finishes onboarding.
const showCreatedModal = ref(false)

// Account-deleted success popup — shown after the delete-account confirm flow.
const showDeletedModal = ref(false)

const handleLogin = () => {
  showLoginModal.value = true
}

/**
 * UsernameModal emits `finished` once onboarding is complete. By then it has
 * already cleared `pendingRegistrationId`, so its v-if has unmounted it.
 * We just open the success popup here.
 */
const handleUsernameFinished = () => {
  showCreatedModal.value = true
}

/**
 * Welcome banner is fired here (NOT inside UsernameModal) so it appears AFTER
 * SuccessModal closes — otherwise the banner would be hidden behind
 * SuccessModal's full-screen blurred backdrop.
 */
const handleCreatedClose = () => {
  showCreatedModal.value = false

  const username = user.value?.username
  if (username) {
    triggerAppBanner(`Welcome, ${username} 👋`)
  }
}

/**
 * AccountDetails (rendered inside AppHeader) emits `account-deleted` once the
 * user confirms deletion and the API call succeeds. AppHeader forwards it up.
 */
const handleAccountDeleted = () => {
  showDeletedModal.value = true
}

const handleDeletedClose = () => {
  showDeletedModal.value = false
}

onMounted(async () => {
  // In production, Azure SWA serves /index.html for /auth/callback and Nuxt's
  // hybrid renderer redirects the route to / while preserving the query string.
  // The callback page's onMounted never runs, so we read OAuth params directly
  // from the URL here instead.
  // In local dev the callback page works normally and uses sessionStorage instead.
  const urlParams = new URLSearchParams(window.location.search)
  const urlToken = urlParams.get('token')
  const urlNewUser = urlParams.get('newUser')
  const urlPendingId = urlParams.get('pendingRegistrationId')
  const urlSuggestedUsername = urlParams.get('suggestedUsername')
  const hasOAuthParams = !!(urlToken || urlPendingId)

  if (hasOAuthParams) {
    // Strip OAuth params from the URL immediately so they don't sit in history.
    window.history.replaceState({}, '', window.location.pathname)
    if (urlToken) localStorage.setItem('token', urlToken)
  }

  await fetchUser()

  if (hasOAuthParams) {
    // Production path — params came from the URL.
    if (urlNewUser === 'false') {
      const name = user.value?.username || ''
      triggerAppBanner(`Welcome back${name ? `, ${name}` : ''}! 👋`)
    } else if (urlNewUser === 'true' && urlPendingId) {
      openOnboarding(urlPendingId, urlSuggestedUsername || null)
    }
  } else {
    // Local dev path — callback page saved intents to sessionStorage.
    if (sessionStorage.getItem('showWelcomeBanner')) {
      sessionStorage.removeItem('showWelcomeBanner')
      const name = user.value?.username || ''
      triggerAppBanner(`Welcome back${name ? `, ${name}` : ''}! 👋`)
    }

    const pendingId = sessionStorage.getItem('pendingRegistrationId')
    if (pendingId) {
      const suggestedUser = sessionStorage.getItem('suggestedUsername') || null
      sessionStorage.removeItem('pendingRegistrationId')
      sessionStorage.removeItem('suggestedUsername')
      openOnboarding(pendingId, suggestedUser)
    }
  }
})
</script>

<template>
  <div>
    <!-- Header (single instance for the whole app) -->
    <AppHeader
      @login="handleLogin"
      @account-deleted="handleAccountDeleted"
    />

    <!-- Global banner — always mounted; visibility & fade are driven by
         `bannerMessage` going non-null / null inside AppBanner. -->
    <AppBanner :message="bannerMessage" />

    <!-- Login -->
    <LoginModal
      v-if="showLoginModal"
      @close="showLoginModal = false"
    />

    <!-- Username onboarding — bound DIRECTLY to the global pendingRegistrationId
         so the modal closes the instant UsernameModal.finish() clears it. -->
    <UsernameModal
      v-if="pendingRegistrationId"
      @close="pendingRegistrationId = null"
      @finished="handleUsernameFinished"
    />

    <!-- Account created -->
    <SuccessModal
      v-if="showCreatedModal"
      title="Account created"
      message="You're ready to explore Audio Atlas."
      @close="handleCreatedClose"
    />

    <!-- Account deleted -->
    <SuccessModal
      v-if="showDeletedModal"
      title="Account deleted"
      message="Your account has been permanently removed."
      @close="handleDeletedClose"
    />

    <!-- Page -->
    <main class="page-content">
      <slot />
    </main>

    <Footer />
  </div>
</template>

<style scoped>
.page-content {
  padding-top: 5rem;
  padding-bottom: 2.5rem;
}
</style>
