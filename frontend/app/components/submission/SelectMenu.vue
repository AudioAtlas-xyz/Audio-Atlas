<script setup lang="ts">
import { ref } from 'vue'

const search = ref('')

const props = defineProps<{
  itemList: string[]
  modelValue: string[]
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string[]): void
}>()

const addTag = () => {
  const value = search.value.trim()
  if (!value) return

  const exists = props.modelValue.includes(value)
  if (exists) return

  emit('update:modelValue', [
    ...props.modelValue,
    value
  ])

  search.value = ''
}

const removeTag = (val: string) => {
  emit(
    'update:modelValue',
    props.modelValue
  )
}


</script>

<template>
  <USelectMenu
    :items="props.itemList"
    :model-value="props.modelValue  "
    v-model:search="search"
    @update:model-value="emit('update:modelValue', $event)"
    @keyup.enter="addTag"
    multiple
    creatable
    chips
  />
</template>

<style scoped>

</style>
