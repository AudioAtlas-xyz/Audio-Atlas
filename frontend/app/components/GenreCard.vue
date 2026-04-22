<script setup lang="ts">
import type { Genre } from '~/types/genre'
import type { Country } from '~/types/country'

const props = defineProps<{
  genre: Genre
}>()

console.log('🎵 GenreCard:', props.genre.name)
console.log('   Countries:', props.genre.countries)
console.log('   Countries length:', props.genre.countries?.length)

const metaItems = computed(() =>
  [props.genre.aliases?.[0]].filter((value): value is string => Boolean(value))
)

const countries = computed(()=> props.genre.countries ?? [])

const countryBadges = computed(() => {
  return countries.value.map(country => country.name)
})

</script>

<template>
  <UCard
    class="border border-border bg-surface shadow-none"
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
              :key="meta"
              color="neutral"
              variant="subtle"
              class="px-2 py-1 text-[10px] uppercase tracking-[0.18em]"
            >
              {{ meta }}
            </UBadge>
          </div>
        </div>


      </div>
    </template>

    <div class="text-sm leading-7 text-[#8b94b5]">
      <div class="flex flex-wrap items-center gap-2">
        <UBadge
          v-for="country in countryBadges"
          :key="country"
          color="neutral"
          variant="outline"
          class="rounded-full border-[#7a84a8] px-2 py-1 text-[10px] font-medium uppercase tracking-[0.18em] text-space-50"
          >
          {{ country }}
        </UBadge>
      </div>
    </div>

    <div class="flex items-center justify-between border-t border-border pt-4 text-[11px] text-[#4f587a]">
      <UButton :to="`/genres?genreId=${props.genre.id}`" variant="link" class="text-aurora">
        See Genre Detail →
      </UButton>
    </div>
  </UCard>
</template>
