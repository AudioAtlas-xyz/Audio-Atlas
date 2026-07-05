<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, computed } from 'vue'
import type { Genre } from '~/types/genre'
import type { Country } from '~/types/country'
import { useApi } from '@/composables/useApi'

const { api } = useApi()

const searchTerm = ref('')
const loading = ref(false)
const genres = ref<Genre[]>([])
const hasSearched = ref(false)

// used by the outside-click handler
const wrapperRef = ref<HTMLElement | null>(null)

// reset everything — called on result click and on outside click
function closeDropdown() {
  searchTerm.value = ''
  genres.value = []
  hasSearched.value = false
  loading.value = false
}

function handleOutsideClick(e: MouseEvent) {
  if (!wrapperRef.value) return
  if (wrapperRef.value.contains(e.target as Node)) return
  closeDropdown()
}

onMounted(() => {
  if (typeof window !== 'undefined') {
    window.addEventListener('mousedown', handleOutsideClick)
  }
})

onUnmounted(() => {
  if (typeof window !== 'undefined') {
    window.removeEventListener('mousedown', handleOutsideClick)
  }
})

// Fetch all countries once and filter locally (~200 rows, tiny payload).
const allCountries = ref<Country[]>([])

onMounted(async () => {
  try {
    allCountries.value = await api<Country[]>('/countries/all')
  } catch (error) {
    console.error('Failed to load countries for search:', error)
    allCountries.value = []
  }
})

// Local country filter, capped so the dropdown stays short.
const countryMatches = computed<Country[]>(() => {
  const q = searchTerm.value.trim().toLowerCase()
  if (q.length < 2) return []

  return allCountries.value
    .filter(c => c.name?.toLowerCase().includes(q))
    .slice(0, 5)
})

type GroupingResult = { label: string; type: 'continent' | 'region' }

// Deduplicate continents and regions from the already-loaded countries list,
// filter against the search term, and cap results so the dropdown stays short.
const groupingMatches = computed<GroupingResult[]>(() => {
  const q = searchTerm.value.trim().toLowerCase()
  if (q.length < 2) return []

  const seen = new Set<string>()
  const results: GroupingResult[] = []

  for (const c of allCountries.value) {
    for (const [value, type] of [
      [c.continent, 'continent'],
      [c.region, 'region'],
    ] as [string, 'continent' | 'region'][]) {
      if (!value || seen.has(value)) continue
      seen.add(value)
      if (value.toLowerCase().includes(q)) results.push({ label: value, type })
    }
  }

  return results.slice(0, 5)
})

const hasResults = computed(
  () => genres.value.length > 0 || countryMatches.value.length > 0 || groupingMatches.value.length > 0
)

let timeout: ReturnType<typeof setTimeout> | null = null

watch(searchTerm, (value) => {
  if (timeout) clearTimeout(timeout)

  hasSearched.value = false

  if (!value || value.trim().length < 2) {
    genres.value = []
    loading.value = false
    return
  }

  timeout = setTimeout(async () => {
    loading.value = true

    try {
      genres.value = await api<Genre[]>(
        `/genres/search/${encodeURIComponent(value)}`
      )
    } catch (error) {
      console.error('Search failed:', error)
      genres.value = []
    } finally {
      loading.value = false
      hasSearched.value = true
    }
  }, 200)
})
</script>

<template>
  <div ref="wrapperRef" class="search-wrapper">
    <UInput
      v-model="searchTerm"
      placeholder="Search for genres/countries"
      icon="i-lucide-search"
      class="search-input"
    />

    <!-- RESULTS -->
    <div
      v-if="hasResults"
      class="search-dropdown"
    >
      <!-- COUNTRIES -->
      <template v-if="countryMatches.length">
        <p class="search-section-label">Countries</p>
        <NuxtLink
          v-for="country in countryMatches"
          :key="`country-${country.id}`"
          :to="`/CountryPage?countryId=${country.id}`"
          class="search-result"
          @click="closeDropdown"
        >
          <strong>{{ country.name }}</strong>
          <span v-if="country.region || country.continent">
            {{ [country.region, country.continent].filter(Boolean).join(' · ') }}
          </span>
        </NuxtLink>
      </template>

      <!-- REGIONS & CONTINENTS -->
      <template v-if="groupingMatches.length">
        <p class="search-section-label">Regions &amp; Continents</p>
        <NuxtLink
          v-for="g in groupingMatches"
          :key="`grouping-${g.label}`"
          :to="`/browse/${encodeURIComponent(g.label)}`"
          class="search-result"
          @click="closeDropdown"
        >
          <strong>{{ g.label }}</strong>
          <span>{{ g.type }}</span>
        </NuxtLink>
      </template>

      <!-- GENRES -->
      <template v-if="genres.length">
        <p class="search-section-label">Genres</p>
        <NuxtLink
          v-for="genre in genres"
          :key="`genre-${genre.id}`"
          :to="`/genres?genreId=${genre.id}`"
          class="search-result"
          @click="closeDropdown"
        >
          <strong>{{ genre.name }} | {{ genre.countries?.[0]?.name || 'Unknown' }}</strong>
          <span>{{ genre.summary }}</span>
        </NuxtLink>
      </template>
    </div>

    <!-- EMPTY -->
    <div
      v-else-if="hasSearched && !loading && searchTerm.trim().length >= 2"
      class="search-dropdown"
    >
      <div class="search-result">
        <strong>No genres, countries or regions found</strong>
      </div>
    </div>
  </div>
</template>

<style>
.search-wrapper {
  position: relative;
  width: 18rem;
  flex-shrink: 0;
}

.search-input {
  width: 100%;
}

.search-dropdown {
  position: absolute;
  top: calc(100% + 0.5rem);
  left: 0;
  right: 0;
  z-index: 100;

  max-height: 24rem;
  overflow-y: auto;

  padding: 0.5rem;
  border-radius: 14px;
  border: 1px solid rgba(141, 219, 230, 0.18);
  background: rgba(4, 18, 22, 0.96);
  backdrop-filter: blur(16px);
}

.search-section-label {
  margin: 0.25rem 0.5rem;
  font-family: 'Space Mono', monospace;
  font-size: 0.65rem;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: #8a93b8;
}

.search-result {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;

  padding: 0.75rem;
  border-radius: 10px;

  color: #8ddbe6;
  text-decoration: none;
}

.search-result:hover {
  background: rgba(255, 255, 255, 0.06);
}

.search-result strong {
  color: #3DE8C8;
  font-size: 0.9rem;
}

.search-result span {
  color: rgba(221, 245, 248, 0.75);
  font-size: 0.8rem;
  line-height: 1.3;
}
</style>
