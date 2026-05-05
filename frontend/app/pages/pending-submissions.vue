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
      <UCollapsible class="flex flex-col gap-2 ">
        <UButton
          size="xl"
          label="open"
          trailing-icon="i-lucide-chevron-down"
          :ui="{
          trailingIcon: 'group-data-[state=open]:rotate-180 transition-transform duration-200'
          }"



          block
        />

        <template #content>
          <Placeholder class="h-48" />
          <UPageCard
            v-bind="item"
            :variant="index % 2 === 0 ? 'soft' : 'outline'"
            class="rounded-none text-white"
          />
        </template>
      </UCollapsible>
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
