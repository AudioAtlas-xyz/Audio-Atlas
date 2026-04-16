<script setup lang="ts">
import type { ContributorSummary } from '~/types/country'

const props = defineProps<{
  contributors: ContributorSummary[]
}>()

const contributorRows = computed(() =>
  props.contributors.slice(0, 3).map(contributor => ({
    ...contributor,
    genresLabel: formatGenreCount(contributor.genresCount ?? 0)
  }))
)

function getContributorInitials(contributor: ContributorSummary) {
  const source = contributor.username || '?'

  return source
    .split(/\s+/)
    .map(part => part[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()
}

function formatGenreCount(count: number) {
  return `${count} genre${count === 1 ? '' : 's'}`
}
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
          Contributors
        </p>
        <p class="font-mono text-[11px] text-[#373d5a]">
          {{ props.contributors.length }}
        </p>
      </div>
    </template>

    <div v-if="contributorRows.length" class="divide-y divide-border">
      <div
        v-for="contributor in contributorRows"
        :key="contributor.id"
        class="flex items-center justify-between gap-3 px-4 py-3"
      >
        <div class="flex min-w-0 items-center gap-3">
          <UAvatar
            :src="contributor.avatarUrl"
            :alt="contributor.username"
            :text="getContributorInitials(contributor)"
            size="sm"
            class="ring-1 ring-aurora"
          />

          <div class="min-w-0">
            <ULink
              :to="`/contributors/${contributor.id}`"
              class="truncate font-mono text-[11px] tracking-[0.12em] text-aurora hover:text-aurora"
            >
              @{{ contributor.username }}
            </ULink>
            <p v-if="contributor.displayName" class="truncate text-xs text-[#5d678c]">
              {{ contributor.displayName }}
            </p>
          </div>
        </div>

        <p class="shrink-0 text-[11px] text-[#373d5a]">
          {{ contributor.genresLabel }}
        </p>
      </div>
    </div>

    <div v-else class="px-4 py-5 text-sm text-[#6f789b]">
      Contributors will appear here when the backend includes contributor summaries.
    </div>
  </UCard>
</template>
