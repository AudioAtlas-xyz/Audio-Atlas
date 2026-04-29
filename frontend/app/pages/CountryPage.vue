<script setup lang="ts">
import type { Country } from '~/types/country'

const route = useRoute()
const { api } = useApi()

/**
 * Extract countryId safely from query
 */
const countryId = computed(() => {
  const raw = route.query.countryId
  return typeof raw === 'string' && raw.length > 0 ? raw : undefined
})

/**
 * Fetch country data
 */
const { data, pending, error } = await useAsyncData<Country | null>(
  'country-page',
  async () => {
    if (!countryId.value) return null
    return api<Country>(`/countries/${countryId.value}`)
  },
  {
    watch: [countryId],
    default: () => null
  }
)

/**
 * Derived state
 */
const country = computed(() => data.value)
const genres = computed(() => country.value?.genres ?? [])
const contributors = computed(() => country.value?.contributors ?? [])
const genreCount = computed(() => genres.value.length)

/**
 * Breadcrumbs
 */
const breadcrumbItems = computed(() =>
  [
    { label: 'Explore', to: '/' },
    country.value?.region
      ? { label: country.value.region, to: '/' }
      : null,
    country.value?.name
      ? { label: country.value.name, to: route.fullPath, active: true }
      : null
  ].filter(Boolean) as Array<{ label: string; to: string; active?: boolean }>
)

/**
 * Location badges
 */
const locationBadges = computed(() =>
  [country.value?.region, country.value?.continent].filter(
    (v): v is string => Boolean(v)
  )
)

/**
 * Description fallback
 */
const pageDescription = computed(() => {
  return country.value?.description?.trim()
    ? country.value.description
    : 'Country context from the Audio Atlas API will appear here once the backend payload is wired up.'
})

/**
 * Dynamic page title
 */
useHead(() => ({
  title: country.value?.name
    ? `${country.value.name} | Audio Atlas`
    : 'Country | Audio Atlas'
}))
</script>

<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">
      <!-- HERO -->
      <section class="border-b border-border bg-bg">
        <div class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:px-[120px] lg:py-10">

          <!-- LOADING -->
          <div v-if="pending" class="grid gap-6 lg:grid-cols-[minmax(0,1fr)_320px] lg:items-start">
            <div class="space-y-4">
              <USkeleton class="h-14 w-64 rounded-md bg-surface-2" />
              <div class="flex gap-2">
                <USkeleton class="h-6 w-24 rounded-full bg-surface-2" />
                <USkeleton class="h-6 w-20 rounded-full bg-surface-2" />
              </div>
              <USkeleton class="h-20 w-full rounded-md bg-surface-2" />
            </div>

            <div class="space-y-3">
              <USkeleton class="h-10 w-full rounded-md bg-surface-2" />
              <USkeleton class="h-4 w-40 rounded-md bg-surface-2" />
            </div>
          </div>

          <!-- SUCCESS -->
          <CountryHeroSection
            v-else-if="country"
            :location-badges="locationBadges"
            :country-name="country.name"
            :description="pageDescription"
            :bread-crumb-items="breadcrumbItems"
          />

          <!-- ERROR -->
          <UAlert
            v-else-if="error"
            color="error"
            variant="soft"
            title="Could not load country data"
            :description="error?.message || 'Unknown error'"
          />

          <!-- MISSING ID -->
          <UAlert
            v-else
            color="warning"
            variant="soft"
            title="Missing countryId"
            description="Open this page with a ?countryId=... query so the page can request country data from the backend."
          />
        </div>
      </section>

      <!-- STATS -->
      <section class="border-y border-border bg-surface">
        <div class="mx-auto max-w-[1200px] px-6 py-3 sm:px-10 lg:px-[120px]">
          <div class="space-y-1">
            <p class="font-display text-xl text-aurora">
              {{ genreCount }}
            </p>
            <p class="text-[11px] text-[#373d5a]">
              Genres documented
            </p>
          </div>
        </div>
      </section>

      <!-- CONTENT -->
      <section class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:px-[120px] lg:py-10">
        <div class="border-b border-border pb-4">
          <h2 class="font-display text-[28px] tracking-[-0.02em] text-space-50">
            Genres
          </h2>
          <p class="mt-2 text-xs text-[#373d5a]">
            {{ genreCount }} {{ genreCount === 1 ? 'genre' : 'genres' }} from
            {{ country?.name || 'this country' }} documented in Audio Atlas
          </p>
        </div>

        <div class="grid gap-8 lg:grid-cols-[minmax(0,1fr)_300px] lg:items-start">
          <!-- GENRES -->
          <div>
            <div v-if="pending" class="grid gap-5 md:grid-cols-2">
              <USkeleton
                v-for="i in 4"
                :key="i"
                class="h-48 rounded-md bg-surface-2"
              />
            </div>

            <div v-else-if="genres.length" class="grid gap-5 md:grid-cols-2">
              <CountryGenreCard
                v-for="genre in genres"
                :key="genre.id"
                :genre="genre"
              />
            </div>

            <UAlert
              v-else
              color="neutral"
              variant="soft"
              title="No genres documented yet"
              :description="country
                ? `No genres have been returned for ${country.name} yet.`
                : 'No genre data has been returned yet.'"
            />
          </div>

          <!-- CONTRIBUTORS -->
          <CountryContributorsCard :contributors="contributors" />
        </div>
      </section>
    </UContainer>
  </div>
</template>