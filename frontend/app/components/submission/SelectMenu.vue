<script setup lang="ts">
import { ref, computed } from 'vue'

const search = ref('')

const props = withDefaults(defineProps<{
  itemList: { label: string; value: string }[]
  modelValue: string[]
}>(), {
  modelValue: () => []
})



const emit = defineEmits<{
  (e: 'update:modelValue', value: string[]): void
}>()

const selectedItems = computed(() => {
  return props.modelValue
    .map(id => props.itemList.find(item => item.value === id))
    .filter((item): item is { label: string; value: string } => item !== undefined)
})

</script>

<template>
  <USelectMenu
    :items="props.itemList"
    :model-value="selectedItems  "
    v-model:search="search"
    search-attribute="label"
    @update:model-value="(items) => emit('update:modelValue', items.map(item => item.value))"
    multiple
    creatable
    value-attribute="value"
    option-attribute="label"
    chips
  />
</template>

<style scoped>

</style>
