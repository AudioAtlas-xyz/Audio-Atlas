<script setup>
import Globe from "./../components/Globe.vue";
</script>



<template>
  <ClientOnly>
    <Globe />
  </ClientOnly>


  <UButton to="/genres">
    Genre Detail Page
  </UButton>

</template>

<style>
#app {
  padding: 0;
  margin: 0;
}
</style>
