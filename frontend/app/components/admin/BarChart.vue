<script setup lang="ts">
import { computed } from 'vue'

interface BarItem {
  label: string
  value: number
}

const props = defineProps<{
  items: BarItem[]
  ariaLabel?: string
  color?: string
  maxItems?: number
}>()

const displayItems = computed(() =>
  props.maxItems ? props.items.slice(0, props.maxItems) : props.items
)

const maxValue = computed(() =>
  Math.max(...displayItems.value.map(i => i.value), 1)
)

const barColor = computed(() => props.color ?? 'var(--color-aurora)')
</script>

<template>
  <div
    role="list"
    :aria-label="ariaLabel ?? 'Bar chart'"
    class="space-y-2.5"
  >
    <div
      v-for="item in displayItems"
      :key="item.label"
      role="listitem"
      class="space-y-1"
    >
      <div class="flex items-center justify-between gap-2">
        <span
          class="max-w-[70%] truncate text-xs text-space-300"
          :title="item.label"
        >{{ item.label }}</span>
        <span class="shrink-0 font-mono text-xs text-aurora">{{ item.value.toLocaleString() }}</span>
      </div>
      <div
        class="h-1 overflow-hidden rounded-full bg-surface-3"
        aria-hidden="true"
      >
        <div
          class="h-full rounded-full transition-[width] duration-500"
          :style="{
            width: `${(item.value / maxValue) * 100}%`,
            backgroundColor: barColor
          }"
          role="progressbar"
          :aria-valuenow="item.value"
          :aria-valuemax="maxValue"
          :aria-label="`${item.label}: ${item.value.toLocaleString()}`"
        />
      </div>
    </div>

    <p v-if="!displayItems.length" class="text-xs text-[#6f789b]">
      No data yet
    </p>
  </div>
</template>
