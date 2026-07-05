<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import Globe from '@/components/Globe.vue'
import CountryPanel from '@/components/CountryPanel.vue'

import { useScrollIntro } from '@/composables/useScrollIntro'
import { useHead } from '#imports'

/**
 * Page meta
 */
useHead({
  title: 'Audio Atlas',
  meta: [
    { name: 'description', content: 'Explore music genres around the world' }
  ]
})

const viewportHeight = ref(0)

onMounted(() => {
  viewportHeight.value = window.innerHeight

  window.addEventListener('resize', () => {
    viewportHeight.value = window.innerHeight
  })
})

/**
 * Scroll intro
 */
const { progress, finished } = useScrollIntro()

/**
 * Globe setup
 */
const landingPov = { lat: 16, lng: 0, altitude: 1.55 }
const settledPov = { lat: 54, lng: 12, altitude: 2.2 }
const introSpinSpeed = 0.4

const easeOut = (t: number) => 1 - Math.pow(1 - t, 3)

const eased = computed(() => {
  const t = finished.value ? 1 : progress.value
  return easeOut(easeOut(t))
})

const globeOffset = computed<[number, number]>(() => {
  const p = finished.value ? 1 : progress.value

  // positive Y moves globe down, negative moves it up
  const y = Math.round((1 - eased.value) * 260)

  return [0, y]
})

const globePov = computed(() =>
  finished.value ? settledPov : landingPov
)

const pageStyle = computed(() => {
  const p = finished.value ? 1 : progress.value
  const globeY = Math.round((1 - eased.value) * viewportHeight.value * 0.3)

  return {
    '--title-opacity': Math.max(0, 1 - p * 1.55),
    '--title-lift': `${Math.round(p * -88)}px`,
    '--globe-y': `${globeY}px`
  }
})

/**
 * Country panel
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

    <!-- Hero -->
    <div class="hero-title">
      <p class="eyebrow">
        A living map of the world's music
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
            :intro-spin-speed="introSpinSpeed"
            @country-click="handleCountryClick"
          />
        </div>
      </ClientOnly>
    </div>

    <!-- Scroll cue -->
    <div
      v-if="!finished"
      class="scroll-cue"
      :style="{ opacity: Math.max(0, 1 - progress * 6) }"
    >
      <span class="scroll-cue__label">scroll to explore</span>
      <UIcon name="i-heroicons-chevron-down-20-solid" class="scroll-cue__chevron" />
    </div>

    <!-- Country panel -->
    <CountryPanel
      v-if="selectedCountryId"
      :country-id="selectedCountryId"
      :open="Boolean(selectedCountryId)"
      @close="closeCountryPanel"
    />

    <div class="scroll-spacer" />

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
  font-size: clamp(2.8rem, 15vw, 6.5rem);
  font-weight: 850;
  line-height: 0.88;
}

.scroll-spacer {
  height: 185vh;
}

.scroll-cue {
  position: fixed;
  z-index: 3;
  bottom: 2.5rem;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.35rem;
  pointer-events: none;
  transition: opacity 0.15s ease;
}

.scroll-cue__label {
  color: #8ddbe6;
  font-size: 0.65rem;
  font-weight: 700;
  letter-spacing: 0.18em;
  text-transform: uppercase;
}

.scroll-cue__chevron {
  width: 1.25rem;
  height: 1.25rem;
  color: #8ddbe6;
  animation: cue-bounce 1.4s ease-in-out infinite;
}

@keyframes cue-bounce {
  0%, 100% { transform: translateY(0); }
  50%       { transform: translateY(6px); }
}
</style>
