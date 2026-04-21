<script setup lang="ts">
import { computed, ref } from 'vue'
import Globe from './../components/Globe.vue'
import AppHeader from './../components/Appheader.vue'
import { useScrollIntro } from './../composables/useScrollIntro'

const { progress, finished } = useScrollIntro()
const demoCountryId = '06d584ed-80a4-40e4-b7e7-fd1a499e9e05'

const landingPov = { lat: 16, lng: 0, altitude: 1.55 }
const settledPov = { lat: 16, lng: 0, altitude: 2.15 }

const easeOut = (t: number) => 1 - Math.pow(1 - t, 3)

const eased = computed(() => easeOut(finished.value ? 1 : progress.value))

const globeOffset = computed(() => [
  0,
  Math.round((1 - eased.value) * window.innerHeight * 0.14)
])

const globePov = computed(() =>
  finished.value ? { ...settledPov } : { ...landingPov }
)

const pageStyle = computed(() => {
  const p = finished.value ? 1 : progress.value
  return {
    '--title-opacity': Math.max(0, 1 - p * 1.55).toFixed(3),
    '--title-lift': `${Math.round(p * -88)}px`
  }
})

const handleLogin = () => {
  console.log('Login clicked')
}

const selectedCountryId = ref<string | null>(null)

const closeCountryPanel = () => {
  selectedCountryId.value = null
}

const openDemoCountryPanel = () => {
  selectedCountryId.value = demoCountryId
}
</script>

<template>
  <main
    class="landing-page"
    :style="pageStyle"
  >
    <Appheader
      :visible="finished"
      @login="handleLogin"
    />

    <UButton
      class="demo-panel-button"
      color="neutral"
      variant="solid"
      @click="openDemoCountryPanel"
    >
      open dk
    </UButton>

    <div class="globe-layer">
      <ClientOnly>
        <div class="globe-motion">
          <Globe
            :intro-complete="finished"
            :initial-point-of-view="landingPov"
            :point-of-view="globePov"
            :globe-offset="globeOffset"
          />
        </div>
      </ClientOnly>
    </div>

    <CountryPanel
      v-if="selectedCountryId"
      :country-id="selectedCountryId"
      :open="Boolean(selectedCountryId)"
      @close="closeCountryPanel"
    />

    <div class="hero-title">
      <p class="eyebrow">
        explorable by map <br> growable by community
      </p>
      <h1>Audio Atlas</h1>
    </div>

    <div
      class="scroll-spacer"
      aria-hidden="true"
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
  will-change: transform, opacity;
}

.demo-panel-button {
  position: fixed;
  top: 5.5rem;
  right: 1.5rem;
  z-index: 11;
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
  letter-spacing: 0;
  text-shadow: 0 1.5rem 4rem rgba(5, 23, 22, 0.7);
}

.scroll-spacer {
  height: 185vh;
  pointer-events: none;
}

@media (max-width: 720px) {
  .demo-panel-button {
    top: 4.75rem;
    right: 1rem;
  }

  .hero-title {
    top: 16vh;
    width: min(24rem, calc(100% - 2rem));
  }

  h1 {
    font-size: 4rem;
  }
}
</style>
