<script setup>
defineProps({
  tag: {
    type: String,
    default: 'div'
  }
})
</script>

<template>
  <component
    :is="tag"
    class="glass-panel"
    v-bind="$attrs"
  >
    <slot />
  </component>
</template>

<style scoped>
.glass-panel {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.7rem 0.85rem 0.7rem 1rem;
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 0.7rem;
  background: rgba(8, 24, 28, 0.55);
  backdrop-filter: blur(18px) saturate(1.4);
  -webkit-backdrop-filter: blur(18px) saturate(1.4);
  box-shadow:
    0 4px 24px rgba(0, 0, 0, 0.35),
    inset 0 0.5px 0 rgba(255, 255, 255, 0.08);

  transition:
    transform 0.15s ease,
    box-shadow 0.2s ease;
}

.glass-panel:hover {
  transform: translateY(-1px);
  box-shadow:
    0 6px 28px rgba(0, 0, 0, 0.4),
    inset 0 0.5px 0 rgba(255, 255, 255, 0.1);
}
</style>