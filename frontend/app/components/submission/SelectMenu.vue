<script setup lang="ts">

const props = defineProps<{
  itemList: { label: string; value: string }[]
  modelValue: { label: string; value: string }[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: { label: string; value: string }[]): void
}>()


</script>

<template>
  <USelectMenu
    :items="props.itemList"
    :model-value="props.modelValue"
    @update:model-value="emit('update:modelValue', $event)"
    multiple
  />
</template>

<style scoped>

</style>
