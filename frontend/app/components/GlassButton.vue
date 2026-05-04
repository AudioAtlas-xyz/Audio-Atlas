<script setup lang="ts">
import type { Component } from 'vue'

type Variant = 'glass' | 'primary'
type Size = 'sm' | 'md'

const props = defineProps<{
  tag?: string | Component
  variant?: Variant
  size?: Size
  disabled?: boolean
}>()

const emit = defineEmits<{
  (e: 'click', event: MouseEvent): void
}>()

const handleClick = (e: MouseEvent) => {
  if (props.disabled) {
    e.preventDefault()
    return
  }
  emit('click', e)
}
</script>

<template>
  <component
    :is="props.tag || 'button'"
    class="glass-btn"
    :class="[
      props.variant || 'glass',
      props.size || 'md',
      { disabled: props.disabled }
    ]"
    :disabled="props.tag === 'button' ? props.disabled : undefined"
    :type="props.tag === 'button' ? 'button' : undefined"
    :aria-disabled="props.disabled"
    tabindex="0"
    @click="handleClick"
  >
    <slot />
  </component>
</template>

<style scoped>
.glass-btn {
  padding: 0.4rem 0.9rem;
  border: 1px solid rgba(141, 219, 230, 0.25);
  border-radius: 0.5rem;

  background: rgba(141, 219, 230, 0.08);
  color: #8ddbe6;

  font-weight: 600;
  letter-spacing: 0.02em;

  cursor: pointer;
  white-space: nowrap;

  transition: all 0.2s ease;
}

.glass-btn.sm {
  font-size: 0.72rem;
  padding: 0.25rem 0.7rem;
}

.glass-btn.md {
  font-size: 0.82rem;
}

.glass-btn:hover {
  background: rgba(141, 219, 230, 0.16);
  border-color: rgba(141, 219, 230, 0.4);
  color: #b5eaf2;
}

.glass-btn:active {
  background: rgba(141, 219, 230, 0.22);
}

.glass-btn.primary {
  background-color: #3de8c8;
  color: #02211c;
  border: none;
}

.glass-btn.primary:hover {
  transform: translateY(-2px);
  filter: brightness(1.05);
}

.glass-btn.primary:active {
  transform: translateY(0);
}

.glass-btn.disabled,
.glass-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
  pointer-events: none;
  transform: none;
}
</style>