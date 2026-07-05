<script setup lang="ts">
import type { CostPanel } from '~/types/dashboard'

defineProps<{
  data: CostPanel
}>()
</script>

<template>
  <div class="space-y-6">
    <div class="border-b border-border pb-4">
      <h2 class="font-display text-[22px] tracking-[-0.02em] text-space-50">Cost</h2>
    </div>

    <div class="max-w-xs rounded-md border border-border bg-surface p-5">
      <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Azure monthly spend</p>
      <p
        class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50"
        :aria-label="data.azureMonthlySpend != null
          ? `Azure monthly spend: $${data.azureMonthlySpend}`
          : 'Azure monthly spend not configured'"
      >
        {{ data.azureMonthlySpend != null ? `$${data.azureMonthlySpend.toLocaleString()}` : '—' }}
      </p>
      <p class="mt-1 text-[11px] text-[#373d5a]">
        Source: {{ data.source }} · update via <code class="text-aurora">Admin:AzureMonthlySpend</code> in config
      </p>
    </div>
  </div>
</template>
