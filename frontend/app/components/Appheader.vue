<script setup>
import GlassPanel from '@/components/GlassPanel.vue'
import GlassButton from '@/components/GlassButton.vue'

const emit = defineEmits(['login'])
</script>

<template>
  <header class="app-header">
    <GlassPanel class="nav-panel">

      <!-- LEFT -->
      <div class="left">
        <NuxtLink to="/" class="brand">
          Audio Atlas
        </NuxtLink>
      </div>

      <!-- CENTER -->
      <nav class="nav-links">
        <NuxtLink to="/explore">Explore</NuxtLink>
        <NuxtLink to="/about">About</NuxtLink>
      </nav>

      <!-- RIGHT -->
      <div class="right">
        <GlassButton
          variant="primary"
          @click="() => { console.log('CLICK'); $emit('login') }">
          Sign in
        </GlassButton>
      </div>

    </GlassPanel>
  </header>
</template>

<style scoped>
.app-header {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;

  z-index: 50;

  display: flex;
  justify-content: center;
}

/* Container */
.nav-panel {
  display: flex;
  align-items: center;

  width: 100%;
  max-width: 90rem;
  height: 3.5rem;
  padding: 0 3rem;

  border-bottom: 1px solid rgba(141, 219, 230, 0.15);
}

/* LEFT */
.left {
  flex: 1;
  display: flex;
  justify-content: flex-start;
}

/* CENTER */
.nav-links {
  flex: 1;
  display: flex;
  justify-content: center;
  gap: 2rem;
}

.nav-links :deep(a) {
  font-size: 0.9rem;
  color: #8ddbe6;
  text-decoration: none;
  transition: opacity 0.2s ease;
}

.nav-links :deep(a:hover) {
  opacity: 0.7;
}

/* RIGHT */
.right {
  flex: 1;
  display: flex;
  justify-content: flex-end;
}

/* BRAND */
.brand {
  padding: 0.4rem 0.8rem;
  border-radius: 10px;

  font-size: 0.95rem;
  font-weight: 700;
  color: #3DE8C8;
  text-decoration: none;

  text-shadow: 0 0 10px rgba(61, 232, 200, 0.3);

  transition: background 0.2s ease, opacity 0.2s ease;
}

.brand:hover {
  background: rgba(255, 255, 255, 0.05);
  opacity: 0.9;
}
</style>