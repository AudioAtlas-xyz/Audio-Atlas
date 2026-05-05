<script setup lang="ts">
import { ref } from 'vue'

const search = ref('')

const props = withDefaults(defineProps<{
  itemList: string[]
  modelValue: string[]
}>(), {
  modelValue: () => []
})



const emit = defineEmits<{
  (e: 'update:modelValue', value: string[]): void
}>()


</script>

<template>
  <USelectMenu
    :items="props.itemList"
    :model-value="props.modelValue  "
    v-model:search="search"
    @update:model-value="emit('update:modelValue', $event)"
    multiple
    creatable
    value-attribute="value"
    chips
  />
</template>

<style scoped>

</style>
