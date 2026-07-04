<script setup lang="ts">
import type { Genre } from '~/types/genre'
import SourceList from '~/components/SourceList.vue'

const route = useRoute()
const { api } = useApi()

/**
 * Extract genreId safely
 */
const genreId = computed(() => {
  const raw = route.query.genreId
  return typeof raw === 'string' && raw.length > 0 ? raw : undefined
})

/**
 * Fetch data
 */
const { data, pending, error } = await useAsyncData<Genre | null>(
  () => `genre-${genreId.value ?? 'none'}`,
  async () => {
    if (!genreId.value) return null
    return api<Genre>(`/genres/${genreId.value}`)
  },
  {
    default: () => null
  }
)

/**
 * Derived state
 */
const genre = computed(() => data.value)

const pageDescription = computed(() =>
  genre.value?.description?.trim()
    ? genre.value.description
    : 'Genre context from the Audio Atlas API will appear here once the backend payload is wired up.'
)

const name = computed(() => genre.value?.name)
const isSensitive = computed(() => genre.value?.isSensitive ?? false)
const sensitivityDescription = computed(() => genre.value?.sensitiveDescription)

const countries = computed(() => genre.value?.countries ?? [])
const contributors = computed(() => genre.value?.contributors ?? [])
const sources = computed(() => genre.value?.sources ?? [])
const instruments = computed(() => genre.value?.instruments ?? [])

const parentGenres = computed(() => genre.value?.parentGenres ?? [])
const similarGenres = computed(() => genre.value?.similarGenres ?? [])
const subGenres = computed(() => genre.value?.subGenres ?? [])

/**
 * Counts
 */
const relatedCount = computed(() => {
  if (!genre.value) return '0'

  const total =
    (genre.value.similarGenres?.length || 0) +
    (genre.value.subGenres?.length || 0) +
    (genre.value.parentGenres?.length || 0)

  return total.toString()
})

const contributorsCount = computed(() =>
  contributors.value.length.toString()
)

const instrumentCount = computed(() =>
  instruments.value.length.toString()
)

/**
 * Country badges
 */
const countryBadges = computed(() =>
  countries.value.map(c => c.name)
)

/**
 * Breadcrumbs
 */
const breadcrumbItems = computed(() =>
  [
    { label: 'Explore', to: '/' },
    genre.value?.name
      ? { label: genre.value.name, to: route.fullPath, active: true }
      : null
  ].filter(Boolean) as Array<{ label: string; to: string; active?: boolean }>
)

/**
 * Dynamic title
 */
useHead(() => ({
  title: genre.value?.name
    ? `${genre.value.name} | Audio Atlas`
    : 'Genre | Audio Atlas'
}))
</script>

<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">

      <!-- HERO -->
      <section class="border-b border-border bg-bg">
        <div class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:py-10">

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
          <!-- translate-x lines HeroSection up with GenreInfo component -->
          <HeroSection
            v-else-if="genre"
            class="translate-x-[2.25rem]"
            :bread-crumb-items="breadcrumbItems"
            :badges="countryBadges"
            :name="name || ''"
          />

          <!-- ERROR -->
          <UAlert
            v-else-if="error"
            color="error"
            variant="soft"
            title="Could not load genre data"
            :description="error?.message || 'Unknown error'"
          />

          <!-- MISSING -->
          <UAlert
            v-else
            color="warning"
            variant="soft"
            title="Missing genreId"
            description="Open this page with a ?genreId=... query so the page can request genre data from the backend."
          />

          <!-- PANEL -->
          <GenrePanel
            class="translate-x-[2.25rem]"
            :related-count="relatedCount"
            :contributor-count="contributorsCount"
            :instrument-count="instrumentCount"
          />
        </div>
      </section>

      <!-- CONTENT -->
      <section class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:py-10">
        <div class="grid gap-8 lg:grid-cols-[minmax(0,1fr)_300px] lg:items-start">

          <GenreInfo
            :is-sensitive="isSensitive"
            :sensitivity-description="sensitivityDescription"
            :pageDescription="pageDescription"
            :similar-genres="similarGenres"
            :sub-genres="subGenres"
            :parent-genres="parentGenres"
          />

          <div class="flex flex-col gap-8">
            <CountryContributorsCard v-if="contributors.length" :contributors="contributors" />
            <SourceList :sources="sources" />
            <InstrumentList v-if="instruments.length" :instruments="instruments" />
          </div>

        </div>
      </section>

    </UContainer>
  </div>
</template>
