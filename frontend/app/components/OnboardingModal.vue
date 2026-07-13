<script setup lang="ts">
const emit = defineEmits<{ (e: 'start'): void }>()
</script>

<template>
  <Teleport to="body">
    <div class="overlay">
      <div class="modal">

        <p class="eyebrow">Welcome to</p>
        <h2 class="title">Audio Atlas</h2>
        <p class="tagline">A living map of the world's music</p>

        <ul class="features">
          <li class="feature">
            <UIcon name="i-lucide-globe" class="feature__icon" />
            <span>Spin the globe and click any country to discover its genres</span>
          </li>
          <li class="feature">
            <UIcon name="i-lucide-search" class="feature__icon" />
            <span>Search across cultures, eras, and continents</span>
          </li>
          <li class="feature">
            <UIcon name="i-lucide-pencil" class="feature__icon" />
            <span>Sign in to contribute genres and suggest edits</span>
          </li>
        </ul>

        <button class="cta" @click="emit('start')">
          Start exploring
          <UIcon name="i-lucide-arrow-right" class="cta__icon" />
        </button>

      </div>
    </div>
  </Teleport>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  z-index: 100;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(2, 7, 10, 0.72);
  backdrop-filter: blur(6px);
  padding: 1.5rem;
}

.modal {
  width: 100%;
  max-width: 440px;
  background: #0d1220;
  border: 1px solid rgba(141, 219, 230, 0.15);
  border-radius: 16px;
  padding: 2.5rem 2.25rem;
  display: flex;
  flex-direction: column;
  gap: 0;
  box-shadow: 0 24px 64px rgba(0, 0, 0, 0.6);
}

.eyebrow {
  margin: 0 0 0.15rem;
  font-size: 0.7rem;
  font-weight: 700;
  letter-spacing: 0.16em;
  text-transform: uppercase;
  color: #4a6070;
}

.title {
  margin: 0 0 0.5rem;
  font-size: 2.25rem;
  font-weight: 850;
  line-height: 1;
  color: #3de8c8;
  text-shadow: 0 0 24px rgba(61, 232, 200, 0.25);
}

.tagline {
  margin: 0 0 2rem;
  font-size: 0.9rem;
  color: #8ddbe6;
}

.features {
  list-style: none;
  margin: 0 0 2.25rem;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.feature {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  font-size: 0.875rem;
  color: #c5d4e8;
  line-height: 1.45;
}

.feature__icon {
  width: 1rem;
  height: 1rem;
  color: #3de8c8;
  flex-shrink: 0;
  margin-top: 0.15rem;
}

.cta {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.75rem 1.5rem;
  border-radius: 10px;
  border: none;
  background: #3de8c8;
  color: #02070a;
  font-size: 0.9rem;
  font-weight: 700;
  cursor: pointer;
  transition: opacity 0.2s ease, transform 0.15s ease;
}

.cta:hover {
  opacity: 0.88;
  transform: translateY(-1px);
}

.cta:active {
  opacity: 1;
  transform: translateY(0);
}

.cta__icon {
  width: 1rem;
  height: 1rem;
}
</style>
