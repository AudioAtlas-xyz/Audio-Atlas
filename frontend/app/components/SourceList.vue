<script setup lang="ts">
import type { GenreSource } from "~/types/genre";

const props = defineProps<{
  sources: GenreSource[]
}>()

const sourceRows = computed(() =>
  props.sources.slice(0, 5)
)
</script>

<template>
  <UCard
    class="border border-border bg-surface shadow-none"
    :ui="{
      header: 'border-b border-border bg-surface-2 px-4 py-3',
      body: 'px-0 py-0',
      footer: 'hidden'
    }"
  >
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
    <div v-else class="px-4 py-5 text-sm text-[#6f789b]">
      Sources will appear here if available.
    </div>
  </UCard>
</template>
