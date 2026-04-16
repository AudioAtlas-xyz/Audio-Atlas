<script setup lang="ts">
import type { Genre } from '~/types/genre'

type GenrePageData = Genre

const route = useRoute()
const genreId = computed(() => {
  const rawGenreId = route.query.genreId
  return typeof rawGenreId === 'string' && rawGenreId.length > 0 ? rawGenreId : undefined
})

  const { data, pending, error } = await useAsyncData<GenrePageData | null>(
    'genre-page',
    async () => {
      if (!genreId.value) {
        return null
      }

      return $fetch<GenrePageData>(`http://localhost:5085/api/genres/${genreId.value}`)
    },
    {
      watch: [genreId],
      default: () => null
    })

const genre = computed(() => data.value)
const description = computed(() => genre.value?.description)
const name = computed(() => genre.value?.name)
const startYear = computed(()=> genre.value?.startYear)
const isSensitive = computed(()=> genre.value?.isSensitive)
const countries = computed(()=> genre.value?.countries?? [])

// it takes the array of countries from genre and returns an array of strings consisting of only the country names.
const countryBadges = computed(() => {
  return countries.value.map(country => country.name)
})


</script>


<template>
  <UContainer>

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

    <CountryHeroSection
      v-else-if="genre"
      :location-badges="countryBadges"
      :country-name="name"
      :description="description"
    />

    <UAlert
      v-else-if="error"
      color="error"
      variant="soft"
      title="Could not load genre data"
      :description="error.message"
    />

    <UAlert
      v-else
      color="warning"
      variant="soft"
      title="Missing genreId"
      description="Open this page with a ?genreId=... query so the page can request genre data from the backend."
    />
    <GenreInfo />
  </UContainer>
</template>


<style scoped>

</style>
