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
 * Extract a clean host label from a URL.
 * - Strips a leading `www.`
 * - Falls back to the raw string if URL parsing fails
 *   so we never render an empty link
 */
function getHost(link: string): string {
  try {
    const url = new URL(link)
    return url.host.replace(/^www\./, '')
  } catch {
    return link
  }
}

/**
 * Limit displayed sources to first 5 entries and pre-compute
 * the host label so the template stays declarative.
 */
const sourceRows = computed(() =>
  props.sources.slice(0, 5).map(source => ({
    ...source,
    hostLabel: getHost(source.sourceLink)
  }))
)
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
        class="flex items-start gap-3 px-4 py-3"
      >
        <div class="min-w-0 flex-1">
          <!--
            We display only the host (e.g. `en.wikipedia.org`) instead
            of the full URL — full links were both visually noisy and
            prone to clipping inside this narrow card. The actual
            destination is preserved on the anchor's `:to`, and
            `title` shows the raw URL on hover for transparency.
            `target=_blank` opens external sources in a new tab so
            we don't lose the genre page.
          -->
          <ULink
            :to="source.sourceLink"
            :title="source.sourceLink"
            target="_blank"
            rel="noreferrer"
            class="block truncate font-mono text-[11px] leading-snug tracking-[0.12em] text-aurora hover:text-aurora"
          >
            {{ source.hostLabel }}
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
