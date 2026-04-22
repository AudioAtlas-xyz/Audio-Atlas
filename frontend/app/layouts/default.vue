<script setup lang="ts">
import { ref, onMounted } from 'vue'
import AppHeader from '@/components/AppHeader.vue'
import LoginModal from '@/components/LoginModal.vue'
import UsernameModal from '@/components/UsernameModal.vue'
import SuccessModal from '@/components/SuccessModal.vue'
import LoginBanner from '@/components/LoginBanner.vue'
import { useRoute } from 'vue-router'
import { useAuth } from '@/composables/useAuth'

/**
 * Access current route (used for query params after OAuth)
 */
const route = useRoute()

/**
 * Auth composable providing user state and fetch method
 */
const { fetchUser, user } = useAuth()

/**
 * UI state for modals and banners
 */
const showLoginModal = ref(false)
const showSuccessModal = ref(false)
const showLoginBanner = ref(false)
const showUsernameModal = ref(false)

/**
 * Open login modal when user clicks "Sign in"
 */
const handleLogin = () => {
  showLoginModal.value = true
}

/**
 * Handle completion of username onboarding
 */
const handleUsernameFinished = () => {
  showUsernameModal.value = false
  showSuccessModal.value = true
}

/**
 * Initialize auth state and UI flags on mount
 */
onMounted(async () => {
  // Fetch authenticated user from backend
  await fetchUser()

  /**
   * Show login success banner (for returning users)
   */
  if (localStorage.getItem('showLoginBanner') === 'true') {
    showLoginBanner.value = true
    localStorage.removeItem('showLoginBanner')

    setTimeout(() => {
      showLoginBanner.value = false
    }, 3000)
  }

  /**
   * Show username onboarding modal (for new users)
   */
  if (localStorage.getItem('showUsernameModal') === 'true') {
    showUsernameModal.value = true
    localStorage.removeItem('showUsernameModal')
  }
})
</script>

<template>
  <div>
    <!-- Header with login trigger -->
    <AppHeader @login="handleLogin" />

    <!-- Login success banner -->
    <LoginBanner
      v-if="showLoginBanner && user"
      :username="user ? (user.username || user.email) : 'there'"
    />

    <!-- Login modal -->
    <LoginModal
      v-if="showLoginModal"
      @close="showLoginModal = false"
    />

    <!-- Username onboarding modal for new users -->
    <UsernameModal
      v-if="showUsernameModal"
      :pending-registration-id="String(route.query.pendingRegistrationId || '')"
      @close="showUsernameModal = false"
      @finished="handleUsernameFinished"
    />

    <!-- Success modal after onboarding -->
    <SuccessModal
      v-if="showSuccessModal"
      @close="showSuccessModal = false"
    />

    <!-- Main page content -->
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