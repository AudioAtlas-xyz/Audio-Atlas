<script setup lang="ts">
defineProps<{
  item: Record<string, unknown>
  index: number
}>()
</script>


<template>
  <UPageCard
    v-bind="item"
    :variant="index % 2 === 0 ? 'soft' : 'outline'"
    class="rounded-none text-white"
  />
</template>


<style scoped>


</style>

