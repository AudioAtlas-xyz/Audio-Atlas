<script setup>
defineProps({
  username: {
    type: String,
    default: ''
  }
})
</script>

<template>
  <div v-if="username" class="banner">
    Welcome back, {{ username }} 👋
  </div>
</template>

<style scoped>
.banner {
  position: fixed;
  top: 5rem;
  left: 50%;
  transform: translateX(-50%);

  padding: 0.6rem 1.2rem;
  border-radius: 999px;

  background: rgba(68, 240, 196, 0.1);
  border: 1px solid rgba(68, 240, 196, 0.3);

  color: #44f0c4;
  font-size: 0.85rem;
  font-weight: 600;

  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);

  z-index: 1000;

  animation: fadeIn 0.25s ease;
  pointer-events: none;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translate(-50%, -10px);
  }
  to {
    opacity: 1;
    transform: translate(-50%, 0);
  }
}
</style>