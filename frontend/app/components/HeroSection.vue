    <script setup lang="ts">
const { breadCrumbItems, badges, name } = defineProps<{
  breadCrumbItems: Array<{ label: string, to: string, active?: boolean }>
  badges: string[]
  name: string
}>()
</script>

<template>

        <div class="flex items-center justify-between gap-4">
            <UBreadcrumb
            v-if="breadCrumbItems.length"
            :items="breadCrumbItems"
            >
                <template #item-label="{ item, active }">
                    <span :class="active ? 'text-aurora font-semibold' : 'text-muted'">
                    {{ item.label }}
                    </span>
                </template>
            </UBreadcrumb>
        </div>

    <div class="space-y-5">

      <div class="space-y-4">
        <h1 class="font-display text-5xl tracking-[-0.04em] text-space-50 sm:text-[52px]">
          {{ name }}
        </h1>

        <div class="flex flex-wrap items-center gap-2">
          <UBadge
            v-for="badge in badges"
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

