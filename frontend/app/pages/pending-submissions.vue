<script setup lang="ts">
const items = Array.from({ length: 30 }, (_, i) => ({
  id: i + 1,
  title: `Item ${i + 1}`,
  description: `Description for item ${i + 1}`
}))
</script>


<template>
  <UContainer class="max-w-5xl mx-auto">
    <h1>Pending Submissions</h1>
    <UScrollArea
      v-slot="{ item, index }"
      :items="items"
      class="w-full h-125"
    >
      <UPageCard
        v-bind="item"
        :variant="index % 2 === 0 ? 'soft' : 'outline'"
        class="rounded-none text-white"


      />
    </UScrollArea>
  </UContainer>


</template>


<style scoped>
h1{
  margin: 2rem;
  color: white;
  font-size: 3.5rem;
  font-weight: 850;
  line-height: 0.88;
}
</style>
