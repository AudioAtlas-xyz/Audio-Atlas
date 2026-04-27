
<script setup>
import Stepper from './../components/Stepper.vue';
const step = ref(1);
</script>

<template>
  <div>
    <Stepper :current-state="step" />
  </div>
</template>
