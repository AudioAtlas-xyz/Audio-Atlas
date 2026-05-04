<script setup lang="ts">
import type { Component } from 'vue'

/**
 * Props for flexible button component
 * - tag: HTML tag or component (defaults to button)
 * - variant: visual style (e.g. "glass", "primary")
 * - disabled: disables interaction
 */
const props = defineProps<{
  tag?: string | Component
  variant?: string
  disabled?: boolean
}>()

/**
 * Emit click event to parent
 */
const emit = defineEmits<{
  (e: 'click', event: MouseEvent): void
}>()

/**
 * Handle click:
 * Prevent emitting if disabled
 */
const handleClick = (e: MouseEvent) => {
  if (props.disabled) return
  emit('click', e)
}
</script>

<template>
  <!-- Dynamic button element -->
  <component
    :is="props.tag || 'button'"
    class="glass-btn"
    :class="[props.variant || 'glass', { disabled: props.disabled }]"
    :disabled="props.tag === 'button' ? props.disabled : undefined"
    :type="props.tag === 'button' ? 'button' : undefined"
    :aria-disabled="props.disabled"
    @click="handleClick"
  >
    <!-- Button content -->
    <slot />
  </component>
</template>

<style scoped>
/* Base glass button style */
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

/* Hover state */
.glass-btn:hover {
  background: rgba(141, 219, 230, 0.16);
  border-color: rgba(141, 219, 230, 0.4);
  color: #b5eaf2;
}

/* Active state */
.glass-btn:active {
  background: rgba(141, 219, 230, 0.22);
}

/* Primary variant */
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

/* Disabled state */
.glass-btn.disabled,
.glass-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
  transform: none;
}

.glass-btn.disabled:hover {
  background: inherit;
  border-color: inherit;
  color: inherit;
}
</style>