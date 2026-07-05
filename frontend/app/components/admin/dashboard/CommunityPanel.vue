<script setup lang="ts">
import { computed } from 'vue'
import type { CommunityPanel } from '~/types/dashboard'

const props = defineProps<{
  data: CommunityPanel
}>()

const roleItems = computed(() =>
  props.data.usersByRole.map(r => ({ label: r.label, value: r.count }))
)

const retentionTotal = computed(() =>
  props.data.contributorRetention.repeat + props.data.contributorRetention.oneTime
)

const repeatPct = computed(() => {
  if (!retentionTotal.value) return 0
  return Math.round((props.data.contributorRetention.repeat / retentionTotal.value) * 100)
})
</script>

<template>
  <div class="space-y-6">
    <div class="border-b border-border pb-4">
      <h2 class="font-display text-[22px] tracking-[-0.02em] text-space-50">Community</h2>
    </div>

    <div class="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
      <!-- Users by role -->
      <div class="rounded-md border border-border bg-surface p-5">
        <p class="mb-4 font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Users by role</p>
        <AdminBarChart
          v-if="roleItems.length"
          :items="roleItems"
          color="var(--color-starlight)"
          aria-label="Users per role"
        />
        <p v-else class="text-xs text-[#6f789b]">No users yet</p>
      </div>

      <!-- Signups + active contributors -->
      <div class="rounded-md border border-border bg-surface p-5 space-y-4">
        <div>
          <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">New signups this month</p>
          <p
            class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50"
            :aria-label="`${data.newSignupsThisMonth} new signups this month`"
          >
            {{ data.newSignupsThisMonth.toLocaleString() }}
          </p>
        </div>
        <div class="border-t border-border pt-4">
          <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Active contributors</p>
          <p
            class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50"
            :aria-label="`${data.activeContributors} active contributors in the last 30 days`"
          >
            {{ data.activeContributors.toLocaleString() }}
          </p>
          <p class="mt-0.5 text-[11px] text-[#7a84a8]">submitted in last 30 days</p>
        </div>
      </div>

      <!-- Retention -->
      <div class="rounded-md border border-border bg-surface p-5 space-y-3">
        <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Contributor retention</p>

        <template v-if="retentionTotal > 0">
          <div>
            <div class="mb-1 flex items-center justify-between">
              <span class="text-xs text-space-300">Repeat contributors</span>
              <span
                class="font-mono text-xs text-aurora"
                :aria-label="`${repeatPct}% repeat contributors`"
              >{{ repeatPct }}%</span>
            </div>
            <div class="h-1 overflow-hidden rounded-full bg-surface-3" aria-hidden="true">
              <div
                class="h-full rounded-full bg-aurora transition-[width] duration-500"
                :style="{ width: `${repeatPct}%` }"
                role="progressbar"
                :aria-valuenow="repeatPct"
                aria-valuemax="100"
                :aria-label="`${repeatPct}% repeat contributors`"
              />
            </div>
          </div>

          <ul class="space-y-1.5 text-xs">
            <li class="flex justify-between">
              <span class="text-space-300">Repeat (2+ submissions)</span>
              <span class="font-mono text-aurora">{{ data.contributorRetention.repeat.toLocaleString() }}</span>
            </li>
            <li class="flex justify-between">
              <span class="text-space-300">One-time</span>
              <span class="font-mono text-[#7a84a8]">{{ data.contributorRetention.oneTime.toLocaleString() }}</span>
            </li>
          </ul>
        </template>
        <p v-else class="text-xs text-[#6f789b]">No submission data yet</p>
      </div>
    </div>

    <!-- Top contributors -->
    <div v-if="data.topContributors.length" class="rounded-md border border-border bg-surface overflow-hidden">
      <p class="border-b border-border px-5 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">
        Top contributors
      </p>
      <ul aria-label="Top contributors by submission count">
        <li
          v-for="(c, i) in data.topContributors"
          :key="c.accountId"
          class="flex items-center justify-between border-b border-border px-5 py-3 last:border-b-0"
        >
          <div class="flex items-center gap-3">
            <span class="w-5 shrink-0 font-mono text-[11px] text-[#373d5a]">{{ i + 1 }}</span>
            <span class="text-sm text-space-50">{{ c.username ?? c.accountId }}</span>
          </div>
          <span
            class="font-mono text-xs text-aurora"
            :aria-label="`${c.submissionCount} submissions`"
          >{{ c.submissionCount.toLocaleString() }}</span>
        </li>
      </ul>
    </div>
  </div>
</template>
