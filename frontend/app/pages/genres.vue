<script setup lang="ts">
import type { Genre } from '~/types/genres'

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

      return $fetch<GenrePageData>(`http://localhost:5085/api/countries/${genreId.value}`)
    },
    {
      watch: [genreId],
      default: () => null
    })

const genre = computed(() => {data.value})

</script>


<template>
  <UContainer>
    <GenrePanel />
    <GenreInfo />
  </UContainer>
</template>


<style scoped>

</style>
