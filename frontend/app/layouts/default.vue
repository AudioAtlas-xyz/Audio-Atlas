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
  await fetchUser()

  // Check for post-login intents saved by /auth/callback before it navigated here.
  // We do this after fetchUser() so the user object is ready when the banner fires.
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
  </div>
</template>

<style scoped>
.page-content {
  padding-top: 5rem;
}
</style>
