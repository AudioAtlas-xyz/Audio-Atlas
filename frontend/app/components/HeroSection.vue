<script setup lang="ts">
const props = defineProps<{
  breadCrumbItems: Array<{ label: string; to: string; active?: boolean }>
  badges: string[]
  name?: string
}>()
</script>

<template>
  <div class="space-y-6">

    <!-- BREADCRUMB -->
    <UBreadcrumb
      v-if="props.breadCrumbItems?.length"
      :items="props.breadCrumbItems"
    >
      <template #item-label="{ item, active }">
        <span :class="active ? 'text-aurora font-semibold' : 'text-muted'">
          {{ item.label }}
        </span>
      </template>
    </UBreadcrumb>

    <!-- TITLE + BADGES -->
    <div class="space-y-4">
      <h1 class="font-display text-5xl tracking-[-0.04em] text-space-50 sm:text-[52px]">
        {{ props.name || '—' }}
      </h1>

      <div
        v-if="props.badges?.length"
        class="flex flex-wrap items-center gap-2"
      >
        <UBadge
          v-for="(badge, i) in props.badges"
          :key="`${badge}-${i}`"
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