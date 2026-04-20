<script setup>
const emit = defineEmits(['click'])

defineProps({
  tag: {
    type: String,
    default: 'button'
  },
  variant: {
    type: String,
    default: 'glass' // glass | primary
  }
})
</script>

<template>
  <component
    :is="tag"
    class="glass-btn"
    :class="variant"
    @click="$emit('click')"
  >
    <slot />
  </component>
</template>

<style scoped>
.glass-btn {
  padding: 0.35rem 0.85rem;
  border: 1px solid rgba(141, 219, 230, 0.25);
  border-radius: 0.45rem;
  background: rgba(141, 219, 230, 0.08);
  color: #8ddbe6;
  font-size: 0.76rem;
  font-weight: 600;
  letter-spacing: 0.015em;
  cursor: pointer;
  white-space: nowrap;

  transition:
    background 0.2s ease,
    border-color 0.2s ease,
    color 0.2s ease,
    transform 0.15s ease,
    opacity 0.15s ease;
}

.glass-btn:hover {
  background: rgba(141, 219, 230, 0.16);
  border-color: rgba(141, 219, 230, 0.4);
  color: #b5eaf2;
}

.glass-btn:active {
  background: rgba(141, 219, 230, 0.22);
}

/* PRIMARY VARIANT */
.glass-btn.primary {
  background-color: #3DE8C8;
  color: #02070a;
  border: none;
}

.glass-btn.primary:hover {
  opacity: 0.9;
  transform: translateY(-3px);
}

.glass-btn.primary:active {
  transform: translateY(0);
}
</style>