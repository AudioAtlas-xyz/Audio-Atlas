<script setup lang="ts">
import type { Instrument } from "~/types/instrument"

const props = defineProps<{
  instruments: Instrument[]
}>()
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
    <!-- HEADER -->
    <template #header>
      <div class="flex items-center justify-between">
        <p class="text-sm text-space-50">
          Instruments
        </p>
        <p class="font-mono text-[11px] text-meta">
          {{ props.instruments.length }}
        </p>
      </div>
    </template>

    <!-- LIST -->
    <div class="divide-y divide-border">
      <div
        v-for="(it, i) in props.instruments"
        :key="it.id || `${it.type}-${i}`"
        class="flex items-center justify-between gap-3 px-4 py-3"
      >
        <div class="flex min-w-0 items-center gap-3">
          <div class="min-w-0 truncate text-space-50">
            {{ it.type }}
          </div>
        </div>
      </div>
    </div>
  </UCard>
</template>
