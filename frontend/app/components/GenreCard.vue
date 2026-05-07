<script setup lang="ts">
import { computed } from 'vue'
import type { Genre } from '~/types/genre'

const props = defineProps<{
  genre: Genre
}>()

const metaItems = computed(() => props.genre.aliases ?? [])

const countries = computed(() => props.genre.countries ?? [])
</script>

<template>
  <UCard
    class="border border-border bg-surface shadow-none w-full"
    :ui="{
      header: 'border-b border-border bg-surface-2 px-4 py-3',
      body: 'space-y-4 px-4 py-4',
      footer: 'hidden'
    }"
  >
    <template #header>
      <div class="flex items-start justify-between gap-4">
        <div class="space-y-1">
          <h3 class="font-display text-lg text-space-50">
            {{ props.genre.name }}
          </h3>

          <div class="flex flex-wrap gap-2">
          <UBadge
            v-for="meta in metaItems"
            :key="meta.alias"
            color="neutral"
            variant="subtle"
            class="px-2 py-1 text-[10px] uppercase tracking-[0.18em]"
        >
          {{ meta.alias }}
        </UBadge>
        </div>
        </div>
      </div>
    </template>

    <div class="text-sm leading-7 text-[#8b94b5]">
      <div v-if="countries.length" class="flex flex-wrap items-center gap-2">
        <UBadge
          v-for="country in countries"
          :key="country.id"
          color="neutral"
          variant="outline"
          class="rounded-full border-[#7a84a8] px-2 py-1 text-[10px] font-medium uppercase tracking-[0.18em] text-space-50"
          >
          <UButton :to="`/CountryPage?countryId=${country.id}`" variant="link" class="text-aurora">
          {{ country.name }}
          </UButton>
        </UBadge>
      </div>

      <p v-else class="text-xs text-[#6f789b]">
        No countries associated
      </p>
    </div>

    <div class="flex items-center justify-between border-t border-border pt-4 text-[11px] text-[#4f587a]">
      <UButton
        :to="`/genres?genreId=${props.genre.id}`"
        variant="link"
        class="text-aurora"
      >
        See Genre Detail →
      </UButton>
    </div>
  </UCard>
</template>
