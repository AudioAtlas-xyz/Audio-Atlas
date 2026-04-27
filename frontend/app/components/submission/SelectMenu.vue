<script setup lang="ts">


const props = defineProps<{
  itemList: string[]
}>()

const value1 = ref<string[]>([])

</script>

<template>
  <USelectMenu v-model="value1" multiple :items="itemList" />
</template>

<style scoped>

</style>
