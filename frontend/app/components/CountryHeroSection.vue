<script setup lang="ts">
const props = defineProps<{
  locationBadges: string[]
  countryName: string
  description: string
}>()
</script>

<template>
  <div class="grid gap-6 lg:grid-cols-[minmax(0,1fr)_320px] lg:items-start">
    <div class="space-y-5">
      <div class="flex flex-wrap items-center gap-2">
        <UBadge
          v-for="badge in props.locationBadges"
          :key="badge"
          color="neutral"
          variant="outline"
          class="rounded-full border-[#7a84a8] px-2 py-1 text-[10px] font-medium uppercase tracking-[0.18em] text-space-50"
        >
          {{ badge }}
        </UBadge>
      </div>

      <div class="space-y-4">
        <h1 class="font-display text-5xl tracking-[-0.04em] text-space-50 sm:text-[52px]">
          {{ props.countryName }}
        </h1>
        <p class="max-w-[680px] text-sm leading-8 text-[#7a84a8]">
          {{ props.description }}
        </p>
      </div>
    </div>

    <div class="space-y-3 lg:pt-[72px]">
      <UButton
        block
        size="lg"
        color="primary"
        class="justify-center bg-aurora px-4 py-2 font-medium text-bg hover:bg-aurora"
      >
        + Add a genre from {{ props.countryName }}
      </UButton>
      <p class="text-[11px] text-[#373d5a]">
        Know a {{ props.countryName }} genre we&apos;ve missed?
      </p>
    </div>
  </div>
</template>
