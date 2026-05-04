<script setup lang="ts">
import { ref } from 'vue'
import Globe from '@/components/Globe.vue'
import CountryPanel from '@/components/CountryPanel.vue'
import { useHead } from '#imports'

/**
 * Page meta
 */
useHead({
  title: 'Explore - Audio Atlas',
  meta: [
    { name: 'description', content: 'Explore music genres around the world' }
  ]
})

/**
 * Globe is mounted in its "settled" state directly — no hero, no scroll
 * intro. `/explore` is the dedicated globe view, distinct from the landing
 * page at `/` which runs the scroll-driven intro animation.
 */
const settledPov = { lat: 54, lng: 12, altitude: 2.2 }

/**
 * Country panel state — clicking a country in the globe opens the side
 * panel with that country's genres, mirroring the landing page behaviour.
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
  <main class="explore-page">
    <!-- Globe -->
    <div class="globe-layer">
      <ClientOnly>
        <Globe
          :intro-complete="true"
          :initial-point-of-view="settledPov"
          :point-of-view="settledPov"
          :globe-offset="[0, 0]"
          @country-click="handleCountryClick"
        />
      </ClientOnly>
    </div>

    <!-- Country panel -->
    <CountryPanel
      v-if="selectedCountryId"
      :country-id="selectedCountryId"
      :open="Boolean(selectedCountryId)"
      @close="closeCountryPanel"
    />
  </main>
</template>

<style scoped>
.explore-page {
  position: relative;
  width: 100%;
  height: 100vh;
  color: #eefcf8;
  background: #02070a;
  overflow: hidden;
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
</style>
