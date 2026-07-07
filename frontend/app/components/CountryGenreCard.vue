<script setup lang="ts">
import { computed } from 'vue'
import type { NestedGenre } from '~/types/nestedgenre'

const props = defineProps<{
  genre: NestedGenre
}>()

const metaItems = computed(() =>
  [props.genre.region, props.genre.aliases?.[0]].filter((value): value is string => Boolean(value))
)

const SUMMARY_LENGTH = 160

const summary = computed(() => {
  const desc = props.genre.description?.trim()
  if (!desc) return null
  return desc.length > SUMMARY_LENGTH ? desc.slice(0, SUMMARY_LENGTH).trimEnd() + '…' : desc
})

const statusTone = computed(() => {
  if (!props.genre.status) {
    return 'neutral'
  }

  switch (props.genre.status.toLowerCase()) {
    case 'documented':
    case 'verified':
      return 'primary'
    case 'emerging':
      return 'info'
    case 'disputed':
      return 'warning'
    default:
      return 'neutral'
  }
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

        <UBadge
          v-if="props.genre.status"
          :color="statusTone"
          variant="soft"
          class="px-2 py-1 text-[10px] uppercase tracking-[0.18em]"
        >
          {{ props.genre.status }}
        </UBadge>
      </div>
    </template>

    <p v-if="summary" class="text-sm leading-7 text-[#8b94b5]">
      {{ summary }}
    </p>

    <div class="flex items-center justify-between border-t border-border pt-4 text-[11px] text-[#4f587a]">
    <UButton :to="`/genres?genreId=${props.genre.id}`" variant="link" class="text-aurora">
      See Genre Detail →
    </UButton>
    </div>
  </UCard>
</template>
