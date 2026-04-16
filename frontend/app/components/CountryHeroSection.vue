<script setup lang="ts">
const { breadCrumbItems, locationBadges, countryName, description } = defineProps<{
  locationBadges: string[]
  countryName: string
  description: string
  breadCrumbItems: Array<{ label: string, to: string, active?: boolean }>
}>()
</script>

<template>
  <div class="grid gap-6 lg:grid-cols-[minmax(0,1fr)_320px] lg:items-start">
    <div class="space-y-5">

      <HeroSection
            :badges="locationBadges"
            :name="countryName"
            :bread-crumb-items="breadCrumbItems"
      />

        <p class="max-w-[680px] text-sm leading-8 text-[#7a84a8]">
          {{ description }}
        </p>

    </div>

    <div class="space-y-3 lg:pt-[72px]">
      <UButton
        block
        size="lg"
        color="primary"
        class="justify-center bg-aurora px-4 py-2 font-medium text-bg hover:bg-aurora rounded-xl"
      >
        + Add a genre from {{ countryName }}
      </UButton>
      <p class="text-[11px] text-[#373d5a]">
        Know a {{ countryName }} genre we&apos;ve missed?
      </p>
    </div>
  </div>
</template>
