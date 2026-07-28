<script setup lang="ts">
import { computed } from 'vue'
import { useAsyncData, useApi } from '#imports'
import type { Grouping } from '~/types/grouping'

/**
 * Sideways and upward navigation for browse pages.
 *
 * A browse page is addressed by a single grouping name that may be a continent
 * or a region, so the page cannot tell on its own where it sits in the
 * hierarchy. This resolves that from the taxonomy and offers:
 *   - on a continent page: its regions
 *   - on a region page: a link up to the continent, plus its sibling regions
 */
const props = defineProps<{ grouping: string }>()

const { api } = useApi()

// Shared key: the taxonomy is identical for every browse page, so Nuxt reuses
// the payload rather than refetching as the user moves between groupings.
const { data: groupings } = await useAsyncData<Grouping[]>(
  'country-groupings',
  () => api<Grouping[]>('/countries/groupings'),
  { default: () => [] }
)

/** The continent this page belongs to, whether it *is* one or sits inside one. */
const continent = computed(() =>
  groupings.value.find(g => g.continent === props.grouping)
  ?? groupings.value.find(g => g.regions.some(r => r.region === props.grouping))
)

const isContinent = computed(() => continent.value?.continent === props.grouping)

/**
 * South America's sole region is itself named "South America", so listing it
 * would render a single chip duplicating the page you are already on. Treat a
 * lone region that matches its continent as nothing to navigate between.
 */
const regions = computed(() => {
  const c = continent.value
  if (!c) return []
  if (c.regions.length === 1 && c.regions[0]?.region === c.continent) return []
  return c.regions
})

/** Unknown groupings (a stale link, say) render nothing rather than an empty bar. */
const show = computed(() => !!continent.value && (regions.value.length > 0 || !isContinent.value))

const regionHref = (name: string) => `/browse/${encodeURIComponent(name)}`
</script>

<template>
  <nav
    v-if="show"
    class="border-b border-border bg-surface"
    :aria-label="isContinent ? 'Regions in this continent' : 'Related regions'"
  >
    <div class="mx-auto flex max-w-[1200px] flex-col gap-3 px-6 py-4 sm:px-10">
      <!-- Up to the continent, from a region page -->
      <div
        v-if="!isContinent"
        class="flex items-center gap-2"
      >
        <NuxtLink
          :to="regionHref(continent!.continent)"
          class="group inline-flex items-center gap-1.5 rounded-md border border-border bg-surface-2 px-3 py-1.5 text-xs text-label transition-colors hover:border-aurora hover:text-aurora"
        >
          <span aria-hidden="true">&larr;</span>
          <span>All of {{ continent!.continent }}</span>
          <span class="font-mono text-meta group-hover:text-aurora">{{ continent!.genreCount }}</span>
        </NuxtLink>
      </div>

      <!-- Sibling regions, or this continent's regions -->
      <div
        v-if="regions.length"
        class="flex flex-col gap-2"
      >
        <p class="font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
          {{ isContinent ? `Regions in ${continent!.continent}` : `Other regions in ${continent!.continent}` }}
        </p>
        <ul class="flex flex-wrap gap-2">
          <li
            v-for="region in regions"
            :key="region.region"
          >
            <span
              v-if="region.region === props.grouping"
              aria-current="page"
              class="inline-flex items-center gap-1.5 rounded-md border border-aurora bg-surface-2 px-3 py-1.5 text-xs text-aurora"
            >
              {{ region.region }}
              <span class="font-mono">{{ region.genreCount }}</span>
            </span>
            <NuxtLink
              v-else
              :to="regionHref(region.region)"
              class="group inline-flex items-center gap-1.5 rounded-md border border-border bg-surface-2 px-3 py-1.5 text-xs text-label transition-colors hover:border-aurora hover:text-aurora"
            >
              {{ region.region }}
              <span class="font-mono text-meta group-hover:text-aurora">{{ region.genreCount }}</span>
            </NuxtLink>
          </li>
        </ul>
      </div>
    </div>
  </nav>
</template>
