    <script setup lang="ts">
const props = defineProps<{
  badges: string[]
  name: string
}>()
</script>

<template>
    <div class="space-y-5">

      <div class="space-y-4">
        <h1 class="font-display text-5xl tracking-[-0.04em] text-space-50 sm:text-[52px]">
          {{ props.name }}
        </h1>

        <div class="flex flex-wrap items-center gap-2">
          <UBadge
            v-for="badge in props.badges"
            :key="badge"
            color="neutral"
            variant="outline"
            class="rounded-full border-[#7a84a8] px-2 py-1 text-[10px] font-medium uppercase tracking-[0.18em] text-space-50"
          >
            {{ badge }}
          </UBadge>
        </div>
      </div>
    </div>
</template>
