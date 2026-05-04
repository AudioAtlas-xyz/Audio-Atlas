<script setup lang="ts">
import type { Component } from 'vue'

type Variant = 'default' | 'subtle' | 'strong'
type Hover = boolean

const props = withDefaults(defineProps<{
  tag?: string | Component
  variant?: Variant
  hover?: Hover
}>(), {
  tag: 'div',
  variant: 'default',
  hover: true
})
</script>

<template>
  <component
    :is="props.tag"
    class="glass-panel"
    :class="[
      props.variant,
      { hover: props.hover }
    ]"
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

  border-radius: 0.7rem;

  backdrop-filter: blur(18px) saturate(1.4);
  -webkit-backdrop-filter: blur(18px) saturate(1.4);

  transition:
    transform 0.15s ease,
    box-shadow 0.2s ease,
    background 0.2s ease,
    border 0.2s ease;
}

/* ===== VARIANTS ===== */

/* Default (your current style) */
.glass-panel.default {
  border: 1px solid rgba(255, 255, 255, 0.1);
  background: rgba(8, 24, 28, 0.55);

  box-shadow:
    0 4px 24px rgba(0, 0, 0, 0.35),
    inset 0 0.5px 0 rgba(255, 255, 255, 0.08);
}

/* Subtle (good for nav/header) */
.glass-panel.subtle {
  border: 1px solid rgba(255, 255, 255, 0.06);
  background: rgba(8, 24, 28, 0.35);

  box-shadow:
    0 2px 12px rgba(0, 0, 0, 0.25);
}

/* Strong (modals / focus areas) */
.glass-panel.strong {
  border: 1px solid rgba(141, 219, 230, 0.15);
  background: rgba(8, 24, 28, 0.75);

  box-shadow:
    0 12px 40px rgba(0, 0, 0, 0.6),
    inset 0 0.5px 0 rgba(255, 255, 255, 0.12);
}

.glass-panel.hover:hover {
  transform: translateY(-1px);
  box-shadow:
    0 6px 28px rgba(0, 0, 0, 0.4),
    inset 0 0.5px 0 rgba(255, 255, 255, 0.1);
}
</style>