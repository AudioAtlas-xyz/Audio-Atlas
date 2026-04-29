<script setup lang="ts">
import { ref, watch } from 'vue'

const props = defineProps<{
  username?: string
}>()

const visible = ref(false)

watch(
  () => props.username,
  (val) => {
    if (!val) return

    visible.value = true

    // auto-hide after 3s
    setTimeout(() => {
      visible.value = false
    }, 3000)
  },
  { immediate: true }
)
</script>

<template>
  <transition name="fade">
    <div v-if="visible" class="banner">
      Welcome back, {{ props.username }} 👋
    </div>
  </transition>
</template>

<style scoped>
.banner {
  position: fixed;
  top: 5.5rem;
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
  pointer-events: none;
}

/* animation */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.25s ease;
}

.fade-enter-from {
  opacity: 0;
  transform: translate(-50%, -10px);
}

.fade-leave-to {
  opacity: 0;
  transform: translate(-50%, -10px);
}
</style>