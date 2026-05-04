<script setup lang="ts">
/**
 * AppBanner is a "dumb" presentational component:
 * - It renders whatever `message` is passed in.
 * - When `message` becomes null/empty, the banner fades out.
 *
 * Auto-hide timing is owned by `useUIState.triggerAppBanner`,
 * so this component does NOT run its own setTimeout. That avoids
 * the double-timer race that previously cut off the fade animation.
 */
defineProps<{
  message?: string | null
}>()
</script>

<template>
  <transition name="fade">
    <div v-if="message" class="banner">
      {{ message }}
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
