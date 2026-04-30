<script setup lang="ts">
import type {Instrument} from "~/types/instrument";

const props = defineProps<{
  instrument: Instrument[]
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
    <template #header>
      <div class="flex items-center justify-between">
        <p class="text-sm text-space-50">
          Instruments
        </p>
        <p class="font-mono text-[11px] text-[#373d5a]">
          {{ props.instrument.length }}
        </p>
      </div>
    </template>

    <div v-if="instrument.length" class="divide-y divide-border">
      <div
        v-for="it in instrument"
        :key="it.type"
        class="flex items-center justify-between gap-3 px-4 py-3"
      >
        <div class="flex min-w-0 items-center gap-3">

          <div class="min-w-0">
            {{ it.type }}
          </div>
        </div>
      </div>
    </div>

    <div v-else class="px-4 py-5 text-sm text-[#6f789b]">
      Currently no instruments included for this genre.
    </div>
  </UCard>
</template>
