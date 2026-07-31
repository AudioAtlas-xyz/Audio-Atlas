<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import Globe from '@/components/Globe.vue'
import CountryPanel from '@/components/CountryPanel.vue'

useSeoMeta({
  title: 'Audio Atlas — A living map of the world\'s music',
  description: 'Spin an interactive globe to explore music genres by country. Discover the origins, connections and cultural context of hundreds of genres worldwide.',
  ogTitle: 'Audio Atlas — A living map of the world\'s music',
  ogDescription: 'Spin an interactive globe to explore music genres by country. Discover the origins, connections and cultural context of hundreds of genres worldwide.',
  ogType: 'website',
  ogImage: 'https://audioatlas.xyz/og-image.png',
  twitterCard: 'summary_large_image',
  twitterImage: 'https://audioatlas.xyz/og-image.png'
})

// Renders edge to edge with no in-flow footer, so the globe is usable without
// scrolling. `default.vue` reads this to drop its page padding and lock scroll.
definePageMeta({ fullBleed: true })

/**
 * Globe — settled and interactive from the first frame. `introEase` only
 * animates the arrival; it never gates input.
 */
const settledPov = { lat: 54, lng: 12, altitude: 2.2 }
const arrivalPov = { lat: 16, lng: 0, altitude: 2.9 }

/**
 * Country panel, driven by the URL so a selection is shareable and the browser
 * Back button closes it.
 */
const route = useRoute()
const router = useRouter()

// The value arrives from the address bar, so treat it as untrusted: only a
// three-letter ISO A3 code is allowed to reach the panel or the API.
const ISO_A3 = /^[A-Za-z]{3}$/

const selectedCountryId = computed<string | null>(() => {
  const raw = route.query.country
  const value = Array.isArray(raw) ? raw[0] : raw

  return typeof value === 'string' && ISO_A3.test(value)
    ? value.toUpperCase()
    : null
})

const handleCountryClick = (country: { isoA3: string } | string) => {
  const iso = typeof country === 'string' ? country : country?.isoA3
  if (!iso) return

  router.push({ query: { ...route.query, country: iso } })
}

const closeCountryPanel = () => {
  const query = { ...route.query }
  delete query.country

  router.push({ query })
}

/**
 * Onboarding — shown once per browser, and re-openable from the globe chrome.
 * With the hero gone this modal is the only place the site explains itself, so
 * it needs a way back in.
 */
const showOnboarding = ref(false)

onMounted(() => {
  if (!localStorage.getItem('audio_atlas_onboarded')) {
    showOnboarding.value = true
  }
})

function startExploring() {
  localStorage.setItem('audio_atlas_onboarded', '1')
  showOnboarding.value = false
}
</script>

<template>
  <div class="globe-page">
    <!-- Crawlable heading and copy. Visually hidden because the globe is the
         interface, but the page still needs an h1 and a description. -->
    <h1 class="sr-only">
      Audio Atlas
    </h1>
    <p class="sr-only">
      A living map of the world's music. Explore music genres by country on an
      interactive globe, or browse by continent and region.
    </p>

    <!-- Globe -->
    <div class="globe-layer">
      <ClientOnly>
        <Globe
          :intro-complete="true"
          :initial-point-of-view="settledPov"
          :point-of-view="settledPov"
          :intro-ease="arrivalPov"
          :globe-offset="[0, 0]"
          @country-click="handleCountryClick"
        />

        <!-- Without JS there is no globe at all, so this must never be blank. -->
        <template #fallback>
          <div class="globe-fallback">
            <p>Loading the globe…</p>
            <p class="globe-fallback__links">
              <NuxtLink to="/browse/Africa">Browse by continent</NuxtLink>
            </p>
          </div>
        </template>
      </ClientOnly>
    </div>

    <!-- Re-open the explainer; the modal is otherwise first-visit only. -->
    <button
      type="button"
      class="how-it-works"
      @click="showOnboarding = true"
    >
      <UIcon
        name="i-heroicons-question-mark-circle"
        class="how-it-works__icon"
      />
      How it works
    </button>

    <!-- Country panel -->
    <CountryPanel
      v-if="selectedCountryId"
      :country-id="selectedCountryId"
      :open="Boolean(selectedCountryId)"
      @close="closeCountryPanel"
    />

    <!-- Onboarding modal — client-side only, shown once per browser -->
    <ClientOnly>
      <OnboardingModal
        v-if="showOnboarding"
        @start="startExploring"
      />
    </ClientOnly>
  </div>
</template>

<style scoped>
.globe-page {
  position: relative;
  width: 100%;
  height: 100vh;
  /* dvh tracks mobile browser chrome collapsing; vh above is the fallback. */
  height: 100dvh;
  overflow: hidden;
  color: #eefcf8;
  background: #02070a;
}

.globe-layer {
  position: fixed;
  inset: 0;
  overflow: hidden;
  pointer-events: auto;

  background:
    radial-gradient(circle at 50% 12%, rgba(98, 194, 210, 0.2), transparent 34rem),
    linear-gradient(180deg, #02070a 0%, #071512 100%);
}

.globe-fallback {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: var(--color-label);
  font-size: 0.9rem;
}

.globe-fallback__links a {
  color: var(--color-aurora);
  text-decoration: underline;
}

.how-it-works {
  position: fixed;
  z-index: 4;
  bottom: 3.25rem;
  left: 1.5rem;
  display: inline-flex;
  gap: 0.4rem;
  align-items: center;
  padding: 0.4rem 0.75rem;
  border: 1px solid rgba(141, 219, 230, 0.28);
  border-radius: 999px;
  background: rgba(2, 7, 10, 0.62);
  color: var(--color-label);
  font-size: 0.75rem;
  cursor: pointer;
  backdrop-filter: blur(6px);
  transition: color 0.15s ease, border-color 0.15s ease;
}

.how-it-works:hover {
  border-color: rgba(141, 219, 230, 0.6);
  color: #eefcf8;
}

.how-it-works__icon {
  width: 0.95rem;
  height: 0.95rem;
}

@media (max-width: 640px) {
  .how-it-works {
    bottom: 3.75rem;
    left: 1rem;
  }
}
</style>
