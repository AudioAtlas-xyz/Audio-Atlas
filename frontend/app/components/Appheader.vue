<script setup>
import GlassPanel from '@/components/GlassPanel.vue'
import GlassButton from '@/components/GlassButton.vue'

defineProps({
  visible: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['login'])
</script>

<template>
  <header
    class="app-header"
    :class="{ 'is-visible': visible }"
  >
    <GlassPanel>
      <img src="/logo.png" alt="Audio Atlas" class="brand-logo" />
      <span class="brand-name">Audio Atlas</span>
      <GlassButton @click="emit('login')">Sign in</GlassButton>
    </GlassPanel>
  </header>
</template>

<style scoped>
.app-header {
  position: fixed;
  z-index: 10;
  top: 0.85rem;
  right: 0.85rem;
  opacity: 0;
  transform: translateY(-8px);
  transition:
    opacity 0.5s cubic-bezier(0.16, 1, 0.3, 1),
    transform 0.5s cubic-bezier(0.16, 1, 0.3, 1);
  pointer-events: none;
}

.app-header.is-visible {
  opacity: 1;
  transform: translateY(0);
  pointer-events: auto;
}

.brand {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-decoration: none;
}

.brand-logo {
  width: 1.35rem;
  height: 1.35rem;
  object-fit: contain;
  flex-shrink: 0;
}

.brand-name {
  font-size: 0.82rem;
  font-weight: 650;
  letter-spacing: 0.02em;
  color: rgba(238, 252, 248, 0.88);
  white-space: nowrap;
}
</style>