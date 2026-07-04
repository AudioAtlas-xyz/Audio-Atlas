<script setup lang="ts">
import type { Genre } from '~/types/genre'

defineProps<{
  similarGenres: Genre[]
  parentGenres: Genre[]
  subGenres: Genre[]
}>()
</script>

<template>
  <div class="grid w-full gap-2 sm:grid-cols-3">
    <div
      v-for="{ label, genres } in [
        { label: 'Parent Genres', genres: parentGenres },
        { label: 'Sub Genres', genres: subGenres },
        { label: 'Similar Genres', genres: similarGenres }
      ]"
      :key="label"
      class="overflow-hidden rounded-[6px] border border-[#1c2038] bg-[#0d0f1a]"
    >
      <div class="border-b border-[#1c2038] px-4 py-2">
        <span class="font-mono text-[0.65rem] uppercase tracking-[1.25px] text-[#8a93b8]">{{ label }}</span>
      </div>

      <div v-if="genres.length" class="divide-y divide-[#1c2038]">
        <NuxtLink
          v-for="genre in genres"
          :key="genre.id"
          :to="`/genres?genreId=${genre.id}`"
          class="block px-4 py-2.5 text-sm text-space-50 transition-colors hover:bg-[#111420] hover:text-aurora"
        >
          {{ genre.name }}
          <p v-if="genre.countries?.length" class="mt-0.5 text-[10px] text-[#4f587a]">
            {{ genre.countries.map(c => c.name).join(', ') }}
          </p>
        </NuxtLink>
      </div>

      <p v-else class="px-4 py-4 text-xs text-[#4f587a]">
        None
      </p>
    </div>
  </div>
</template>
