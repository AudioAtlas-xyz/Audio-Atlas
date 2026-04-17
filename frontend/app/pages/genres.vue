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
const name = computed(() => genre.value.name)
const startYear = computed(()=> genre.value?.startYear)
const isSensitive = computed(()=> genre.value?.isSensitive)
const countries = computed(()=> genre.value?.countries?? [])
const contributors = computed(() => genre.value?.contributors ?? [])

//for genrepanel (try ID: 01bce686-c704-4fd3-bb5b-0ea301a8b0fc)
const relatedCount = computed(()=> {
  if(!genre.value) return "0"

  //related genres are all similar, parent and sub genres.
  const similarGenresCount = genre.value.similarGenres?.length || 0
  const subGenresCount = genre.value.subGenres?.length || 0
  const parentGenresCount = genre.value.parentGenres?.length || 0

  //added together and converted into a string
  const total = similarGenresCount + subGenresCount + parentGenresCount
  return total.toString()
})
//takes the list length of the contributors list
const contributorsCount = computed(() => {
  const total = contributors.value.length || 0
  return total.toString()
})


// it takes the array of countries from genre and returns an array of strings consisting of only the country names.
const countryBadges = computed(() => {
  return countries.value.map(country => country.name)
})

const breadcrumbItems = computed(() =>
  [
    { label: 'Explore', to: '/' },
    //genre.value?.region ? { label: genre.value.region, to: '/' } : null,
    genre.value?.name ? { label: genre.value.name, to: route.fullPath, active: true } : null
    //Filer boolean fjerne alle falsy elementer. I dette tilfælde ville det være null værdier.
  ].filter(Boolean) as Array<{ label: string, to: string, active?: boolean }>
)

</script>


<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">
      <section class="border-b border-border bg-bg">
        <div class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:py-10">
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

          <HeroSection
            v-else-if="genre"
            :bread-crumb-items="breadcrumbItems"
            :badges="countryBadges"
            :name="name"
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
          <GenrePanel
            :related-count="relatedCount"
            :contributor-count="contributorsCount"
            instrument-count="0"
          />
        </div>
        <GenreInfo />
      </section>
    </UContainer>
  </div>
</template>


<style scoped>

</style>
