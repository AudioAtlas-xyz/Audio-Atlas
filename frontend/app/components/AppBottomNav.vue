<script setup lang="ts">
import { useAuth } from '@/composables/useAuth'
import { useUIState } from '@/composables/useUIState'

const { user } = useAuth()
const { openAccount } = useUIState()
const route = useRoute()

const emit = defineEmits<{
  (e: 'login'): void
}>()

// On the home page the nav only appears once the scroll intro finishes.
// On every other page it is always visible.
const introFinished = useState('scroll-intro-finished', () => false)
const visible = computed(() => route.path !== '/' || introFinished.value)
</script>

<template>
  <Transition name="bottom-nav">
    <nav v-if="visible" class="bottom-nav" aria-label="Mobile navigation">

      <NuxtLink to="/" class="nav-item" :class="{ active: route.path === '/' }">
        <UIcon name="i-lucide-globe" class="nav-item__icon" />
        <span>Explore</span>
      </NuxtLink>

      <NuxtLink to="/about" class="nav-item" :class="{ active: route.path === '/about' }">
        <UIcon name="i-lucide-info" class="nav-item__icon" />
        <span>About</span>
      </NuxtLink>

      <template v-if="user">
        <NuxtLink to="/submission-form" class="nav-item nav-item--contribute">
          <UIcon name="i-lucide-plus-circle" class="nav-item__icon" />
          <span>Contribute</span>
        </NuxtLink>

        <button class="nav-item" @click="openAccount">
          <UIcon name="i-lucide-user" class="nav-item__icon" />
          <span>Account</span>
        </button>
      </template>

      <template v-else>
        <button class="nav-item nav-item--contribute" @click="emit('login')">
          <UIcon name="i-lucide-plus-circle" class="nav-item__icon" />
          <span>Contribute</span>
        </button>

        <button class="nav-item" @click="emit('login')">
          <UIcon name="i-lucide-log-in" class="nav-item__icon" />
          <span>Sign in</span>
        </button>
      </template>

    </nav>
  </Transition>
</template>

<style scoped>
/* Never show on desktop */
@media (min-width: 768px) {
  .bottom-nav {
    display: none;
  }
}

.bottom-nav {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  z-index: 48;
  height: 3.75rem;
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  align-items: stretch;
  background: rgba(4, 7, 19, 0.96);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border-top: 1px solid rgba(141, 219, 230, 0.12);
  padding-bottom: env(safe-area-inset-bottom, 0px);
}

.nav-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.2rem;
  height: 100%;
  background: none;
  border: none;
  cursor: pointer;
  color: #8a93b8;
  font-size: 0.62rem;
  font-weight: 500;
  text-decoration: none;
  transition: color 0.15s ease;
  padding: 0;
}

.nav-item:hover,
.nav-item.active {
  color: #8ddbe6;
}

.nav-item--contribute {
  color: #3de8c8;
}

.nav-item--contribute:hover {
  opacity: 0.85;
}

.nav-item__icon {
  width: 1.35rem;
  height: 1.35rem;
}

/* Slide-up entrance */
.bottom-nav-enter-active,
.bottom-nav-leave-active {
  transition: opacity 0.25s ease, transform 0.25s ease;
}

.bottom-nav-enter-from,
.bottom-nav-leave-to {
  opacity: 0;
  transform: translateY(100%);
}
</style>
