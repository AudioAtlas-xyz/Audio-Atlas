<script setup lang="ts">

const props = defineProps({
  item:

})

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

