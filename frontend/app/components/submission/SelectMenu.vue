<script setup lang="ts">


const props = defineProps<{
  itemList: {label: string ; value: string[]}[]
}>()

const value1 = ref<string[]>([])

</script>

<template>
  <USelectMenu v-model="value1" multiple :items="props.itemList" />
</template>

<style scoped>

</style>
