<script setup lang="ts">
import { computed } from 'vue'
import type { PipelinePanel } from '~/types/dashboard'

const props = defineProps<{
  data: PipelinePanel
  earliestReviewAt?: string | null
}>()

const dateFormatter = new Intl.DateTimeFormat('en-US', {
  month: 'short',
  day: 'numeric',
  year: 'numeric'
})

function formatDate(iso: string | null | undefined): string {
  if (!iso) return '—'
  return dateFormatter.format(new Date(iso))
}

function delta(current: number, previous: number): string {
  const diff = current - previous
  if (diff > 0) return `+${diff}`
  if (diff < 0) return String(diff)
  return '—'
}

function deltaColor(current: number, previous: number): string {
  if (current > previous) return 'text-aurora'
  if (current < previous) return 'text-[#e05570]'
  return 'text-[#7a84a8]'
}

const collectingSince = computed(() =>
  props.earliestReviewAt ? `Collecting since ${formatDate(props.earliestReviewAt)}` : null
)

const workloadItems = computed(() =>
  props.data.curatorWorkload.map(c => ({
    label: c.reviewerUsername ?? c.reviewerId,
    value: c.decisions
  }))
)

const rejectionItems = computed(() =>
  props.data.rejectionBreakdown.map(r => ({ label: r.label, value: r.count }))
)

const oldestAgeFormatted = computed(() => {
  const days = props.data.oldestPendingAgeDays
  if (days == null) return null
  if (days < 1) return 'less than a day'
  return `${Math.floor(days)} day${Math.floor(days) === 1 ? '' : 's'}`
})
</script>

<template>
  <div class="space-y-6">
    <div class="border-b border-border pb-4">
      <h2 class="font-display text-[22px] tracking-[-0.02em] text-space-50">Pipeline</h2>
      <p v-if="collectingSince" class="mt-1 text-xs text-[#7a84a8]">{{ collectingSince }}</p>
      <p v-else class="mt-1 text-xs text-[#373d5a]">No reviewed submissions yet</p>
    </div>

    <!-- This month vs last month -->
    <div class="grid gap-4 sm:grid-cols-2">
      <!-- Approvals -->
      <div class="rounded-md border border-border bg-surface p-5">
        <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Approved</p>
        <div class="mt-2 flex items-end gap-3">
          <p
            class="font-display text-3xl tracking-[-0.02em] text-space-50"
            :aria-label="`${data.approvedThisMonth} approvals this month`"
          >
            {{ data.approvedThisMonth.toLocaleString() }}
          </p>
          <span
            class="mb-0.5 font-mono text-xs"
            :class="deltaColor(data.approvedThisMonth, data.approvedLastMonth)"
            :aria-label="`${delta(data.approvedThisMonth, data.approvedLastMonth)} vs last month`"
          >
            {{ delta(data.approvedThisMonth, data.approvedLastMonth) }}
          </span>
        </div>
        <p class="mt-1 text-xs text-[#7a84a8]">
          {{ data.approvedLastMonth.toLocaleString() }} last month
        </p>
      </div>

      <!-- Rejections -->
      <div class="rounded-md border border-border bg-surface p-5">
        <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Rejected</p>
        <div class="mt-2 flex items-end gap-3">
          <p
            class="font-display text-3xl tracking-[-0.02em] text-space-50"
            :aria-label="`${data.rejectedThisMonth} rejections this month`"
          >
            {{ data.rejectedThisMonth.toLocaleString() }}
          </p>
          <span
            class="mb-0.5 font-mono text-xs"
            :class="deltaColor(data.rejectedLastMonth, data.rejectedThisMonth)"
            :aria-label="`${delta(data.rejectedThisMonth, data.rejectedLastMonth)} vs last month`"
          >
            {{ delta(data.rejectedThisMonth, data.rejectedLastMonth) }}
          </span>
        </div>
        <p class="mt-1 text-xs text-[#7a84a8]">
          {{ data.rejectedLastMonth.toLocaleString() }} last month
        </p>
      </div>
    </div>

    <!-- Stats row -->
    <div class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <!-- Approval rate -->
      <div class="rounded-md border border-border bg-surface p-5">
        <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Approval rate</p>
        <p
          class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50"
          :aria-label="data.approvalRate != null ? `${data.approvalRate}% approval rate` : 'Approval rate not yet available'"
        >
          {{ data.approvalRate != null ? `${data.approvalRate}%` : '—' }}
        </p>
        <p v-if="collectingSince" class="mt-1 text-[11px] text-[#373d5a]">{{ collectingSince }}</p>
        <p v-else class="mt-1 text-[11px] text-[#373d5a]">No reviewed submissions yet</p>
      </div>

      <!-- Median review time -->
      <div class="rounded-md border border-border bg-surface p-5">
        <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Median review time</p>
        <p
          class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50"
          :aria-label="data.medianTimeToReviewHours != null ? `${data.medianTimeToReviewHours} hours median review time` : 'Median review time not yet available'"
        >
          {{ data.medianTimeToReviewHours != null ? `${data.medianTimeToReviewHours}h` : '—' }}
        </p>
        <p v-if="collectingSince" class="mt-1 text-[11px] text-[#373d5a]">{{ collectingSince }}</p>
        <p v-else class="mt-1 text-[11px] text-[#373d5a]">No reviewed submissions yet</p>
      </div>

      <!-- Oldest pending -->
      <div class="rounded-md border border-border bg-surface p-5">
        <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Oldest pending</p>
        <p
          class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50"
          :aria-label="oldestAgeFormatted ? `Oldest pending submission: ${oldestAgeFormatted}` : 'No pending submissions'"
        >
          {{ oldestAgeFormatted ?? '—' }}
        </p>
        <p class="mt-1 text-[11px] text-[#7a84a8]">
          {{ data.queueDepth.toLocaleString() }} pending total
        </p>
      </div>
    </div>

    <!-- Sensitivity holds callout -->
    <div
      v-if="data.sensitivityHolds > 0"
      class="flex items-start gap-3 rounded-md border border-[#5c2070] bg-[#110810] p-4"
      role="alert"
    >
      <UIcon name="i-lucide-shield-alert" class="mt-0.5 h-4 w-4 shrink-0 text-[#e060d8]" aria-hidden="true" />
      <div>
        <p class="text-sm font-medium text-[#e060d8]">
          {{ data.sensitivityHolds }} sensitivity {{ data.sensitivityHolds === 1 ? 'hold' : 'holds' }}
        </p>
        <p class="mt-0.5 text-xs text-[#7a84a8]">
          Submissions flagged for sensitivity review — requires manual assessment.
        </p>
      </div>
    </div>

    <!-- Curator workload + rejection breakdown -->
    <div class="grid gap-5 lg:grid-cols-2">
      <div class="rounded-md border border-border bg-surface p-5">
        <p class="mb-4 font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Curator workload</p>
        <AdminBarChart
          v-if="workloadItems.length"
          :items="workloadItems"
          aria-label="Decisions per curator"
        />
        <p v-else class="text-xs text-[#6f789b]">
          {{ collectingSince ? 'No decisions in the selected window' : 'No reviews recorded yet' }}
        </p>
      </div>

      <div class="rounded-md border border-border bg-surface p-5">
        <p class="mb-4 font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Rejection reasons</p>
        <AdminBarChart
          v-if="rejectionItems.length"
          :items="rejectionItems"
          color="var(--color-rust)"
          aria-label="Rejections by reason"
        />
        <p v-else class="text-xs text-[#6f789b]">No rejections in the selected window</p>
      </div>
    </div>
  </div>
</template>
