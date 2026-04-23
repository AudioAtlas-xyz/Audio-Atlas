<script setup lang="ts">
import { computed } from 'vue'
import type { GenreSource } from '~/types/genre'

/**
 * Props: list of sources related to a genre
 */
const props = defineProps<{
  sources: GenreSource[]
}>()

/**
 * Limit displayed sources to first 5 entries
 */
const sourceRows = computed(() => {
  return props.sources.slice(0, 5)
})
</script>

<template>
  <!-- Card container -->
  <UCard
    class="border border-border bg-surface shadow-none"
    :ui="{
      header: 'border-b border-border bg-surface-2 px-4 py-3',
      body: 'px-0 py-0',
      footer: 'hidden'
    }"
  >
    <!-- Header -->
    <template #header>
      <div class="flex items-center justify-between">
        <p class="text-sm text-space-50">
          Sources
        </p>
        <p class="font-mono text-[11px] text-[#373d5a]">
          {{ props.sources.length }}
        </p>
      </div>
    </template>

    <!-- Source list -->
    <div v-if="sourceRows.length" class="divide-y divide-border">
      <div
        v-for="(source, index) in sourceRows"
        :key="index"
        class="flex items-center justify-between gap-3 px-4 py-3"
      >
        <div class="min-w-0">
          <ULink
            :to="source.sourceLink"
            class="truncate font-mono text-[11px] tracking-[0.12em] text-aurora hover:text-aurora"
          >
            {{ source.sourceLink }}
          </ULink>
        </div>
      </div>
    </div>

    <!-- Empty state -->
    <div v-else class="px-4 py-5 text-sm text-[#6f789b]">
      Sources will appear here if available.
    </div>
  </UCard>
</template>

<style module>
/**
 * Legacy styles (currently unused in template)
 * Can be removed if not referenced elsewhere
 */
.sourcelist {
  width: 100%;
  position: relative;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  text-align: left;
  font-size: 9px;
  color: #373d5a;
  font-family: 'Space Mono';
}
</style>
