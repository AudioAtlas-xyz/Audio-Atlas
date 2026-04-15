<script setup>
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import Globe from './../components/Globe.vue'

const scrollProgress = ref(0)
const introFinished = ref(false)
const viewportHeight = ref(800)
let rafId = 0

// The globe starts here and stays here throughout the scroll intro.
// Only after the intro locks do we transition to the settled altitude.
const landingPointOfView = {
  lat: 16,
  lng: 0,
  altitude: 1.55
}
const settledPointOfView = {
  lat: 16,
  lng: 0,
  altitude: 2.15
}

const easeOutCubic = (t) => 1 - Math.pow(1 - t, 3)

const activeProgress = computed(() => introFinished.value ? 1 : scrollProgress.value)
const introComplete = computed(() => introFinished.value)

// During scroll: only move the globe vertically via offset.
// The altitude stays fixed at landingPointOfView.altitude the entire time.
const globeOffset = computed(() => [
  0,
  Math.round((1 - easeOutCubic(activeProgress.value)) * viewportHeight.value * 0.14)
])

// During scroll: keep altitude locked so globe.gl doesn't internally
// recompute camera position (which causes the snap).
// After intro: we signal Globe.vue to animate to settled altitude.
const globePointOfView = computed(() => {
  if (!introFinished.value) {
    return { ...landingPointOfView }
  }

  return { ...settledPointOfView }
})

const pageStyle = computed(() => ({
  '--title-opacity': Math.max(0, 1 - activeProgress.value * 1.55).toFixed(3),
  '--title-lift': `${Math.round(activeProgress.value * -88)}px`
}))

const lockIntro = () => {
  if (introFinished.value) {
    return
  }

  introFinished.value = true
  scrollProgress.value = 1

  if (rafId) {
    window.cancelAnimationFrame(rafId)
    rafId = 0
  }

  document.documentElement.classList.add('globe-intro-complete')
  window.scrollTo({ top: 0, left: 0, behavior: 'instant' })
}

const updateScrollProgress = () => {
  if (introFinished.value) {
    return
  }

  viewportHeight.value = window.innerHeight
  const scrollRange = Math.max(1, window.innerHeight * 0.85)
  const progress = Math.min(1, Math.max(0, window.scrollY / scrollRange))

  scrollProgress.value = progress

  if (progress >= 1) {
    lockIntro()
  }
}

const requestScrollUpdate = () => {
  if (introFinished.value) {
    return
  }

  if (rafId) {
    return
  }

  rafId = window.requestAnimationFrame(() => {
    rafId = 0
    updateScrollProgress()
  })
}

onMounted(() => {
  document.documentElement.classList.remove('globe-intro-complete')
  updateScrollProgress()
  window.addEventListener('scroll', requestScrollUpdate, { passive: true })
  window.addEventListener('resize', requestScrollUpdate)
})

onBeforeUnmount(() => {
  document.documentElement.classList.remove('globe-intro-complete')
  window.removeEventListener('scroll', requestScrollUpdate)
  window.removeEventListener('resize', requestScrollUpdate)

  if (rafId) {
    window.cancelAnimationFrame(rafId)
    rafId = 0
  }
})
</script>

<template>
  <main
    class="landing-page"
    :style="pageStyle"
  >
    <div class="globe-layer">
      <ClientOnly>
        <div class="globe-motion">
          <Globe
            :intro-complete="introComplete"
            :initial-point-of-view="landingPointOfView"
            :point-of-view="globePointOfView"
            :globe-offset="globeOffset"
          />
        </div>
      </ClientOnly>
    </div>

    <div class="hero-title">
      <p class="eyebrow">
        Sounds across borders
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
  .hero-title {
    top: 16vh;
    width: min(24rem, calc(100% - 2rem));
  }

  h1 {
    font-size: 4rem;
  }
}
</style>