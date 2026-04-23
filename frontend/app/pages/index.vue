<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import Globe from '@/components/Globe.vue'
import AppHeader from '@/components/AppHeader.vue'
import CountryPanel from '@/components/CountryPanel.vue'
import LoginModal from '@/components/LoginModal.vue'
import UsernameModal from '@/components/UsernameModal.vue'
import SuccessModal from '@/components/SuccessModal.vue'
import LoginBanner from '@/components/LoginBanner.vue'
import { useRoute } from 'vue-router'
import { useScrollIntro } from '@/composables/useScrollIntro'
import { useAuth } from '@/composables/useAuth'
import { useHead } from '#imports'

/**
 * Page metadata
 */
useHead({
  title: 'Audio Atlas',
  meta: [
    { name: 'description', content: 'Explore music genres around the world' }
  ]
})
 const route = useRoute()


/**
 * Auth state (GLOBAL)
 */
const {
  user,
  fetchUser,
  showLoginBanner,
  showUsernameModal,
  pendingRegistrationId
} = useAuth()

/**
 * Fetch user on load (VERY important)
 */

onMounted(async () => {

  const newUser = route.query.newUser
  const pendingId = route.query.pendingRegistrationId
  const suggested = route.query.suggestedUsername

  if (newUser === 'true' && pendingId) {
    showUsernameModal.value = true
    pendingRegistrationId.value = String(pendingId)
  }

  if (newUser === 'false') {
    showLoginBanner.value = true
  }

  await fetchUser()

  window.history.replaceState({}, '', '/')
})

/**
 * Scroll intro animation
 */
const { progress, finished } = useScrollIntro()

/**
 * Local UI state
 */
const showLoginModal = ref(false)
const showSuccessModal = ref(false)

/**
 * Globe positions
 */
const landingPov = { lat: 16, lng: 0, altitude: 1.55 }
const settledPov = { lat: 54, lng: 12, altitude: 2.2 }

/**
 * Easing
 */
const easeOut = (t: number) => 1 - Math.pow(1 - t, 3)

const eased = computed(() => {
  const t = finished.value ? 1 : progress.value
  return easeOut(easeOut(t))
})

/**
 * Globe offset
 */
const globeOffset = computed<[number, number]>(() => {
  if (process.server) return [0, 0]

  return [
    0,
    Math.round((1 - eased.value) * window.innerHeight * 0.14)
  ]
})

/**
 * Camera POV
 */
const globePov = computed(() =>
  finished.value ? settledPov : landingPov
)

/**
 * Hero animation
 */
const pageStyle = computed(() => {
  const p = finished.value ? 1 : progress.value

  return {
    '--title-opacity': Math.max(0, 1 - p * 1.55),
    '--title-lift': `${Math.round(p * -88)}px`
  }
})

/**
 * Login modal trigger
 */
const handleLogin = () => {
  showLoginModal.value = true
}

/**
 * Username flow finished
 */
const handleUsernameFinished = () => {
  showUsernameModal.value = false
  showSuccessModal.value = true
}

/**
 * Country state
 */
const selectedCountryId = ref<string | null>(null)

const handleCountryClick = (country: { isoA3: string } | string) => {
  selectedCountryId.value =
    typeof country === 'string' ? country : country.isoA3
}

const closeCountryPanel = () => {
  selectedCountryId.value = null
}
</script>

<template>
  <main class="landing-page" :style="pageStyle">

    <!-- Header -->
    <AppHeader
      :visible="finished"
      @login="handleLogin"
    />

    <!-- Login banner -->
    <LoginBanner
      v-if="showLoginBanner && user"
      :username="user.username || user.email"
    />

    <!-- Hero -->
    <div class="hero-title">
      <p class="eyebrow">
        explorable by map <br>
        growable by community
      </p>
      <h1>Audio Atlas</h1>
    </div>

    <!-- Globe -->
    <div class="globe-layer">
      <ClientOnly>
        <div class="globe-motion">
          <Globe
            :intro-complete="finished"
            :initial-point-of-view="landingPov"
            :point-of-view="globePov"
            :globe-offset="globeOffset"
            @country-click="handleCountryClick"
          />
        </div>
      </ClientOnly>
    </div>

    <!-- Country panel -->
    <CountryPanel
      v-if="selectedCountryId"
      :country-id="selectedCountryId"
      :open="Boolean(selectedCountryId)"
      @close="closeCountryPanel"
    />

    <div class="scroll-spacer" />

    <!-- Login modal -->
    <LoginModal
      v-if="showLoginModal"
      @close="showLoginModal = false"
    />

    <!-- Username onboarding (CRITICAL FIX) -->
    <UsernameModal
      v-if="showUsernameModal && pendingRegistrationId"
      @close="showUsernameModal = false"

      @finished="handleUsernameFinished"
    />

    <!-- Success modal -->
    <SuccessModal
      v-if="showSuccessModal"
      @close="showSuccessModal = false"
    />


  </main>
</template>

<style>
html,
body,
#__nuxt {
  width: 100%;
  min-height: 100%;
  margin: 0;
  padding: 0;
  overflow-x: hidden;
  background: #02070a;
}

html.globe-intro-complete,
html.globe-intro-complete body {
  height: 100%;
  overflow: hidden;
}
</style>

<style scoped>
.landing-page {
  min-height: 185vh;
  color: #eefcf8;
  background: #02070a;
}

.globe-layer {
  position: fixed;
  z-index: 1;
  inset: 0;
  overflow: hidden;
  pointer-events: auto;

  background:
    radial-gradient(circle at 50% 12%, rgba(98, 194, 210, 0.2), transparent 34rem),
    linear-gradient(180deg, #02070a 0%, #071512 100%);
}

.globe-motion {
  width: 100%;
  height: 100%;
}

.hero-title {
  position: fixed;
  z-index: 3;
  top: 17vh;
  left: 50%;
  width: min(42rem, calc(100% - 3rem));
  transform: translate3d(-50%, var(--title-lift), 0);
  opacity: var(--title-opacity);
  pointer-events: none;
  text-align: center;
}

.eyebrow {
  margin: 0 0 0.9rem;
  color: #8ddbe6;
  font-size: 0.78rem;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
}

h1 {
  margin: 0;
  color: #f6fffb;
  font-size: 6.5rem;
  font-weight: 850;
  line-height: 0.88;
}

.scroll-spacer {
  height: 185vh;
}
</style>