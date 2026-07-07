<script setup lang="ts">
import { computed } from 'vue'
import type { CataloguePanel } from '~/types/dashboard'

const props = defineProps<{
  data: CataloguePanel
}>()

const coveragePct = computed(() => {
  const { total, withGenres } = props.data.countryCoverage
  if (!total) return 0
  return Math.round((withGenres / total) * 100)
})

const contentReadyPct = computed(() => {
  const { ready, notReady } = props.data.contentGate
  const total = ready + notReady
  if (!total) return 0
  return Math.round((ready / total) * 100)
})

const geographicItems = computed(() =>
  props.data.geographicBalance.map(x => ({ label: x.label, value: x.count }))
)
</script>

<template>
  <div class="space-y-6">
    <div class="border-b border-border pb-4">
      <h2 class="font-display text-[22px] tracking-[-0.02em] text-space-50">Catalogue</h2>
      <p class="mt-1 text-xs text-[#7a84a8]">
        {{ data.genreCountryLinkCount.toLocaleString() }} genre–country links ·
        <NuxtLink to="/admin/genres" class="underline decoration-dotted hover:text-aurora transition-colors">
          {{ data.totalGenres.toLocaleString() }} genres in scope
        </NuxtLink>
      </p>
    </div>

    <div class="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
      <!-- Geographic balance -->
      <div class="rounded-md border border-border bg-surface p-5">
        <p class="mb-4 font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Geographic balance</p>
        <AdminBarChart
          :items="geographicItems"
          aria-label="Genres per continent"
        />
      </div>

      <!-- Country coverage -->
      <div class="rounded-md border border-border bg-surface p-5 space-y-3">
        <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Country coverage</p>

        <div>
          <p
            class="font-display text-3xl tracking-[-0.02em] text-space-50"
            :aria-label="`${coveragePct}% country coverage`"
          >
            {{ coveragePct }}%
          </p>
          <p class="mt-0.5 text-xs text-[#7a84a8]">
            {{ data.countryCoverage.withGenres }} of {{ data.countryCoverage.total }} countries have at least one genre
          </p>
        </div>

        <template v-if="data.countryCoverage.gapList.length">
          <div>
            <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#e05570]">
              Gap worklist · {{ data.countryCoverage.gapList.length }} countries
            </p>
            <ul
              class="mt-2 max-h-40 space-y-1 overflow-y-auto"
              aria-label="Countries with no genres"
            >
              <li
                v-for="country in data.countryCoverage.gapList"
                :key="country"
                class="flex items-center gap-1.5 text-xs text-[#7a84a8]"
              >
                <span class="h-1 w-1 shrink-0 rounded-full bg-[#e05570]" aria-hidden="true" />
                {{ country }}
              </li>
            </ul>
          </div>
        </template>
        <p v-else class="text-xs text-aurora">All countries have at least one genre</p>
      </div>

      <!-- Data completeness -->
      <div class="rounded-md border border-border bg-surface p-5 space-y-4">
        <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Data completeness</p>

        <!-- Content gate progress -->
        <div>
          <div class="mb-1 flex items-center justify-between">
            <span class="text-xs text-space-300">Review-ready</span>
            <span
              class="font-mono text-xs text-aurora"
              :aria-label="`${contentReadyPct}% of genres are review-ready`"
            >{{ contentReadyPct }}%</span>
          </div>
          <div class="h-1 overflow-hidden rounded-full bg-surface-3" aria-hidden="true">
            <div
              class="h-full rounded-full bg-aurora transition-[width] duration-500"
              :style="{ width: `${contentReadyPct}%` }"
              role="progressbar"
              :aria-valuenow="contentReadyPct"
              aria-valuemax="100"
              :aria-label="`${contentReadyPct}% review-ready`"
            />
          </div>
          <p class="mt-1 text-[11px] text-[#7a84a8]">
            {{ data.contentGate.ready.toLocaleString() }} ready ·
            <NuxtLink
              to="/admin/genres?filter=below-gate"
              class="underline decoration-dotted hover:text-aurora transition-colors"
            >{{ data.contentGate.notReady.toLocaleString() }} not ready</NuxtLink>
          </p>
        </div>

        <!-- Completeness flags -->
        <ul class="space-y-2.5" aria-label="Data completeness flags">
          <li class="flex items-center justify-between gap-2">
            <NuxtLink
              to="/admin/genres?filter=orphans"
              class="text-xs text-space-300 underline decoration-dotted hover:text-aurora transition-colors"
            >Orphan genres (no country)</NuxtLink>
            <NuxtLink
              to="/admin/genres?filter=orphans"
              class="font-mono text-xs transition-colors hover:underline"
              :class="data.dataCompleteness.orphanGenres > 0 ? 'text-[#e05570]' : 'text-aurora'"
              :aria-label="`${data.dataCompleteness.orphanGenres} orphan genres`"
            >{{ data.dataCompleteness.orphanGenres.toLocaleString() }}</NuxtLink>
          </li>
          <li class="flex items-center justify-between gap-2">
            <NuxtLink
              to="/admin/genres?filter=missing-note"
              class="text-xs text-space-300 underline decoration-dotted hover:text-aurora transition-colors"
            >Missing origins note</NuxtLink>
            <NuxtLink
              to="/admin/genres?filter=missing-note"
              class="font-mono text-xs transition-colors hover:underline"
              :class="data.dataCompleteness.missingOriginsNote > 0 ? 'text-[#e05570]' : 'text-aurora'"
              :aria-label="`${data.dataCompleteness.missingOriginsNote} missing origins notes`"
            >{{ data.dataCompleteness.missingOriginsNote.toLocaleString() }}</NuxtLink>
          </li>
          <li class="flex items-center justify-between gap-2">
            <span class="text-xs text-space-300">Missing media link</span>
            <span
              class="font-mono text-xs"
              :class="data.dataCompleteness.missingMedia > 0 ? 'text-[#e05570]' : 'text-aurora'"
              :aria-label="`${data.dataCompleteness.missingMedia} missing media links`"
            >{{ data.dataCompleteness.missingMedia.toLocaleString() }}</span>
          </li>
        </ul>
      </div>
    </div>

    <!-- Regions breakdown (collapsible) -->
    <details class="group">
      <summary class="cursor-pointer select-none list-none font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a] hover:text-[#7a84a8]">
        <span class="group-open:hidden">▶ Genres by region</span>
        <span class="hidden group-open:inline">▼ Genres by region</span>
      </summary>
      <div class="mt-4 max-w-sm">
        <AdminBarChart
          :items="data.genresByRegion.map(x => ({ label: x.label, value: x.count }))"
          :max-items="15"
          aria-label="Genres per region"
          color="var(--color-starlight)"
        />
      </div>
    </details>
  </div>
</template>
