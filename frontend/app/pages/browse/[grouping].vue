<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useAsyncData, useHead } from '#imports'
import { useApi } from '@/composables/useApi'
import type { Genre } from '~/types/genre'

const route = useRoute()
const { api } = useApi()

const grouping = computed(() => route.params.grouping as string)

const { data: genres, pending, error } = await useAsyncData<Genre[]>(
  () => `browse-${grouping.value}`,
  () => api<Genre[]>(`/genres/browse/${encodeURIComponent(grouping.value)}`),
  { default: () => [] }
)

useHead(() => ({
  title: grouping.value ? `${grouping.value} | Audio Atlas` : 'Browse | Audio Atlas'
}))
</script>

<template>
  <div class="bg-bg text-space-50">

    <!-- HERO -->
    <section class="border-b border-border bg-bg">
      <div class="mx-auto max-w-[1200px] px-6 py-8 sm:px-10 lg:py-10">
        <h1 class="font-display text-5xl tracking-[-0.04em] text-space-50 sm:text-[52px]">
          {{ grouping }}
        </h1>
        <p class="mt-3 text-sm text-[#7f8aa8]">
          <template v-if="!pending">
            {{ genres.length }} {{ genres.length === 1 ? 'genre' : 'genres' }} documented from {{ grouping }}
          </template>
        </p>
      </div>
    </section>

    <!-- GENRE GRID -->
    <section class="mx-auto max-w-[1200px] px-6 py-8 sm:px-10 lg:py-10">

      <!-- LOADING -->
      <div v-if="pending" class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <USkeleton v-for="i in 9" :key="i" class="h-20 rounded-md bg-surface-2" />
      </div>

      <!-- ERROR -->
      <UAlert
        v-else-if="error"
        color="error"
        variant="soft"
        title="Could not load genres"
        :description="error.message"
      />

      <!-- EMPTY -->
      <UAlert
        v-else-if="!genres.length"
        color="neutral"
        variant="soft"
        title="No genres found"
        :description="`No genres have been documented for ${grouping} yet.`"
      />

      <!-- GENRES -->
      <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <NuxtLink
          v-for="genre in genres"
          :key="genre.id"
          :to="`/genres?genreId=${genre.id}`"
          class="group block rounded-md border border-border bg-surface px-4 py-3 transition-colors hover:border-aurora hover:bg-surface-2"
        >
          <p class="font-display text-base text-space-50 transition-colors group-hover:text-aurora">
            {{ genre.name }}
          </p>
          <p v-if="genre.countries?.length" class="mt-1 text-[11px] text-[#4f587a]">
            {{ genre.countries.map(c => c.name).join(' · ') }}
          </p>
        </NuxtLink>
      </div>

    </section>
  </div>
</template>
