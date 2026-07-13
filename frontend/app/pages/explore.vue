<script setup lang="ts">
import { ref } from 'vue'
import Globe from '@/components/Globe.vue'
import CountryPanel from '@/components/CountryPanel.vue'

useSeoMeta({
  title: 'Explore Genres | Audio Atlas',
  description: 'Browse every genre on Audio Atlas — filter by continent, region or country and dive into the cultural stories behind each one.',
  ogTitle: 'Explore Genres | Audio Atlas',
  ogDescription: 'Browse every genre on Audio Atlas — filter by continent, region or country and dive into the cultural stories behind each one.',
  ogType: 'website',
  twitterCard: 'summary_large_image'
})

// Globe in its settled state, no scroll intro. The landing page at /
// runs the intro animation; this page skips it.
const settledPov = { lat: 54, lng: 12, altitude: 2.2 }

const selectedCountryId = ref<string | null>(null)

const handleCountryClick = (country: { isoA3: string } | string) => {
  selectedCountryId.value
    = typeof country === 'string' ? country : country.isoA3
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
