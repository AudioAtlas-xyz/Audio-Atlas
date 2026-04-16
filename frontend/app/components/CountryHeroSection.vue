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

      <HeroSection
            :badges="props.locationBadges"
            :name="props.countryName"
      />

        <p class="max-w-[680px] text-sm leading-8 text-[#7a84a8]">
          {{ props.description }}
        </p>

    </div>

    <div class="space-y-3 lg:pt-[72px]">
      <UButton
        block
        size="lg"
        color="primary"
        class="justify-center bg-aurora px-4 py-2 font-medium text-bg hover:bg-aurora rounded-xl"
      >
        + Add a genre from {{ props.countryName }}
      </UButton>
      <p class="text-[11px] text-[#373d5a]">
        Know a {{ props.countryName }} genre we&apos;ve missed?
      </p>
    </div>
  </div>
</template>
