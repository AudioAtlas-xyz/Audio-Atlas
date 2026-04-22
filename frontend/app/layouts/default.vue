<script setup>
import { ref, onMounted } from 'vue'
import AppHeader from '@/components/AppHeader.vue'
import LoginModal from '@/components/LoginModal.vue'
import UsernameModal from '@/components/UsernameModal.vue'
import SuccessModal from '@/components/SuccessModal.vue'
import LoginBanner from '@/components/LoginBanner.vue'
import { useRoute } from 'vue-router'
import { useAuth } from '@/composables/useAuth'

const route = useRoute()
const { fetchUser, user } = useAuth()

const showLoginModal = ref(false)
const showSuccessModal = ref(false)
const showLoginBanner = ref(false)
const showUsernameModal = ref(false)

const handleLogin = () => {
  showLoginModal.value = true
}

const handleUsernameFinished = () => {
  showUsernameModal.value = false
  showSuccessModal.value = true
}

onMounted(async () => {
  await fetchUser()

  if (localStorage.getItem('showLoginBanner') === 'true') {
    showLoginBanner.value = true
    localStorage.removeItem('showLoginBanner')

    setTimeout(() => {
      showLoginBanner.value = false
    }, 3000)
  }

  if (localStorage.getItem('showUsernameModal') === 'true') {
    showUsernameModal.value = true
    localStorage.removeItem('showUsernameModal')
  }
})
</script>

<template>
  <div>
    <AppHeader @login="handleLogin" />

    <LoginBanner
        v-if="showLoginBanner"
        :username="user?.username || user?.email || 'there'"
    />

    <LoginModal
      v-if="showLoginModal"
      @close="showLoginModal = false"
    />

    <UsernameModal
      v-if="showUsernameModal"
      :pending-registration-id="String(route.query.pendingRegistrationId || '')"
      @close="showUsernameModal = false"
      @finished="handleUsernameFinished"
    />

    <SuccessModal
      v-if="showSuccessModal"
      @close="showSuccessModal = false"
    />

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