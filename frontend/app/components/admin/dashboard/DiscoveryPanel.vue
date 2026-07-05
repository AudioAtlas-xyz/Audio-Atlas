<script setup lang="ts">
import type { DiscoveryPanel } from '~/types/dashboard'

defineProps<{
  data: DiscoveryPanel
}>()
</script>

<template>
  <div class="space-y-6">
    <div class="border-b border-border pb-4">
      <h2 class="font-display text-[22px] tracking-[-0.02em] text-space-50">Discovery</h2>
      <p class="mt-1 text-xs text-[#7a84a8]">Search patterns from the public search bar</p>
    </div>

    <div class="grid gap-5 lg:grid-cols-2">
      <!-- Zero-result searches — demand worklist, prominent -->
      <div class="rounded-md border border-[#5a1a5c] bg-[#0d080e] p-5 space-y-4">
        <div class="flex items-start justify-between gap-2">
          <div>
            <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#e060d8]">Zero-result searches</p>
            <p class="mt-1 text-[11px] text-[#7a84a8]">Demand worklist — these feed the content roadmap</p>
          </div>
          <UIcon name="i-lucide-search-x" class="mt-0.5 h-4 w-4 shrink-0 text-[#e060d8]" aria-hidden="true" />
        </div>

        <template v-if="data.zeroResultSearches.length">
          <ol
            class="space-y-1.5"
            aria-label="Top zero-result search terms"
          >
            <li
              v-for="(term, i) in data.zeroResultSearches"
              :key="term.term"
              class="flex items-center gap-3"
            >
              <span class="w-5 shrink-0 font-mono text-[11px] text-[#5a1a5c]">{{ i + 1 }}</span>
              <span class="flex-1 text-sm text-space-50">{{ term.term }}</span>
              <span
                class="font-mono text-xs text-[#e060d8]"
                :aria-label="`searched ${term.frequency} times`"
              >×{{ term.frequency.toLocaleString() }}</span>
            </li>
          </ol>
        </template>
        <p v-else class="text-xs text-[#6f789b]">No zero-result searches in this window</p>
      </div>

      <!-- Top searches -->
      <div class="rounded-md border border-border bg-surface p-5 space-y-4">
        <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Top searches</p>

        <template v-if="data.topSearches.length">
          <ol
            class="space-y-1.5"
            aria-label="Top search terms"
          >
            <li
              v-for="(term, i) in data.topSearches"
              :key="term.term"
              class="flex items-center gap-3"
            >
              <span class="w-5 shrink-0 font-mono text-[11px] text-[#373d5a]">{{ i + 1 }}</span>
              <span class="flex-1 text-sm text-space-50">{{ term.term }}</span>
              <span
                class="font-mono text-xs text-aurora"
                :aria-label="`searched ${term.frequency} times`"
              >×{{ term.frequency.toLocaleString() }}</span>
            </li>
          </ol>
        </template>
        <p v-else class="text-xs text-[#6f789b]">No search data in this window</p>
      </div>
    </div>
  </div>
</template>
