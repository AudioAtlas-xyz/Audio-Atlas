<script setup lang="ts">
import GlassPanel from '@/components/GlassPanel.vue'
import GlassButton from '@/components/GlassButton.vue'
import AccountDetails from '@/components/AccountDetails.vue'
import { useAuth } from '@/composables/useAuth'
import { useUIState } from '@/composables/useUIState'

const { user, logout } = useAuth()
const { showAccount, openAccount, closeAccount } = useUIState()

const emit = defineEmits<{
  (e: 'login'): void
}>()
</script>

<template>
  <header class="app-header">
    <GlassPanel class="nav-panel">
      <div class="left">
        <NuxtLink to="/" class="brand">
          Audio Atlas
        </NuxtLink>
      </div>

      <nav class="nav-links">
        <NuxtLink to="/explore">Explore</NuxtLink>
        <NuxtLink to="/about">About</NuxtLink>
      </nav>

      <div class="right">
        <SearchBar />

        <template v-if="user">
          <span
            class="user-name"
            @click="openAccount"
          >
            {{ user.username || user.email }}
          </span>

          <GlassButton @click="logout">
            Logout
          </GlassButton>
        </template>

        <GlassButton
          v-else
          variant="primary"
          @click="emit('login')"
        >
          Sign in
        </GlassButton>
      </div>
    </GlassPanel>
  </header>

  <AccountDetails
    :open="showAccount"
    @close="closeAccount"
  />
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

.nav-panel {
  display: flex;
  align-items: center;

  width: 100%;
  max-width: 90rem;
  height: 3.5rem;
  padding: 0 3rem;

  border-bottom: 1px solid rgba(141, 219, 230, 0.15);
}

.left {
  flex: 1;
  display: flex;
  justify-content: flex-start;
}

.nav-links {
  flex: 1;
  display: flex;
  align-items: center;
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

.right {
  flex: 1;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 0.75rem;
}

/* 👇 renamed + clickable */
.user-name {
  cursor: pointer;
  color: #8ddbe6;
  font-size: 0.9rem;
}

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