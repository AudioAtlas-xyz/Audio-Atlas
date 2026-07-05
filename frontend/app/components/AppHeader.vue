<script setup lang="ts">
import GlassPanel from '@/components/GlassPanel.vue'
import GlassButton from '@/components/GlassButton.vue'
import AccountDetails from '@/components/UserFlow/AccountDetails.vue'
import { useAuth } from '@/composables/useAuth'
import { useUIState } from '@/composables/useUIState'

const { user, logout, isAdmin } = useAuth()
const {
  showAccount,
  openAccount,
  closeAccount,
  triggerAppBanner
} = useUIState()

const handleUsernameUpdated = (u: string) => {
  triggerAppBanner(`Username updated to ${u} ✨`)
}

const emit = defineEmits<{
  (e: 'login'): void
  (e: 'account-deleted'): void
}>()

const menuOpen = ref(false)
const route = useRoute()

// Close drawer on navigation
watch(() => route.path, () => { menuOpen.value = false })

// Lock body scroll while drawer is open
watch(menuOpen, (open) => {
  if (typeof document !== 'undefined') {
    document.body.style.overflow = open ? 'hidden' : ''
  }
})

onBeforeUnmount(() => {
  if (typeof document !== 'undefined') {
    document.body.style.overflow = ''
  }
})
</script>

<template>
  <header class="app-header">
    <GlassPanel class="nav-panel">
      <!-- Brand — always visible -->
      <div class="left">
        <NuxtLink to="/" class="brand" @click="menuOpen = false">
          Audio Atlas
        </NuxtLink>
      </div>

      <!-- Desktop nav -->
      <nav class="nav-links">
        <NuxtLink to="/explore">Explore</NuxtLink>
        <NuxtLink to="/about">About</NuxtLink>
        <NuxtLink to="/privacy-policy">Privacy Policy</NuxtLink>
        <NuxtLink v-if="isAdmin" to="/admin" class="nav-admin">Admin</NuxtLink>
        <NuxtLink v-if="user" to="/submission-form" class="nav-contribute">Contribute</NuxtLink>
        <button v-else class="nav-contribute" @click="emit('login')">Contribute</button>
      </nav>

      <!-- Desktop right -->
      <div class="right">
        <SearchBar />
        <template v-if="user">
          <span class="user-name" @click="openAccount">
            {{ user.username || user.email }}
          </span>
          <GlassButton @click="() => logout()">Logout</GlassButton>
        </template>
        <GlassButton v-else variant="primary" @click="emit('login')">
          Sign in
        </GlassButton>
      </div>

      <!-- Mobile: hamburger / close -->
      <button
        class="mobile-menu-btn"
        :aria-label="menuOpen ? 'Close menu' : 'Open menu'"
        @click="menuOpen = !menuOpen"
      >
        <UIcon
          :name="menuOpen ? 'i-lucide-x' : 'i-lucide-menu'"
          class="mobile-menu-btn__icon"
        />
      </button>
    </GlassPanel>

    <!-- Mobile drawer -->
    <Transition name="drawer">
      <div v-if="menuOpen" class="mobile-drawer">

        <!-- Search -->
        <div class="drawer-search">
          <SearchBar />
        </div>

        <!-- Nav links -->
        <nav class="drawer-nav">
          <NuxtLink to="/explore" class="drawer-link" @click="menuOpen = false">
            <UIcon name="i-lucide-globe" class="drawer-link__icon" />
            Explore
          </NuxtLink>
          <NuxtLink to="/about" class="drawer-link" @click="menuOpen = false">
            <UIcon name="i-lucide-info" class="drawer-link__icon" />
            About
          </NuxtLink>
          <NuxtLink to="/privacy-policy" class="drawer-link" @click="menuOpen = false">
            <UIcon name="i-lucide-shield" class="drawer-link__icon" />
            Privacy Policy
          </NuxtLink>
          <NuxtLink
            v-if="isAdmin"
            to="/admin"
            class="drawer-link drawer-link--admin"
            @click="menuOpen = false"
          >
            <UIcon name="i-lucide-settings" class="drawer-link__icon" />
            Admin
          </NuxtLink>
        </nav>

        <div class="drawer-divider" />

        <!-- Account section -->
        <div class="drawer-account">
          <template v-if="user">
            <NuxtLink
              to="/submission-form"
              class="drawer-contribute"
              @click="menuOpen = false"
            >
              Contribute a genre
            </NuxtLink>
            <button
              class="drawer-link drawer-link--account"
              @click="openAccount(); menuOpen = false"
            >
              <UIcon name="i-lucide-user" class="drawer-link__icon" />
              {{ user.username || user.email }}
            </button>
            <button
              class="drawer-link drawer-link--logout"
              @click="() => { logout(); menuOpen = false }"
            >
              <UIcon name="i-lucide-log-out" class="drawer-link__icon" />
              Logout
            </button>
          </template>

          <template v-else>
            <button
              class="drawer-contribute"
              @click="emit('login'); menuOpen = false"
            >
              Contribute a genre
            </button>
            <GlassButton
              variant="primary"
              class="drawer-signin"
              @click="emit('login'); menuOpen = false"
            >
              Sign in
            </GlassButton>
          </template>
        </div>

      </div>
    </Transition>
  </header>

  <AccountDetails
    :open="showAccount"
    @close="closeAccount"
    @username-updated="handleUsernameUpdated"
    @account-deleted="emit('account-deleted')"
  />
</template>

<style scoped>
/* ── Shared ────────────────────────────────────────────────── */

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
  display: grid;
  align-items: center;
  width: 100%;
  max-width: 90rem;
  height: 3.5rem;
  border-bottom: 1px solid rgba(141, 219, 230, 0.15);
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
  white-space: nowrap;
}

.brand:hover {
  background: rgba(255, 255, 255, 0.05);
  opacity: 0.9;
}

/* ── Desktop layout ────────────────────────────────────────── */

@media (min-width: 768px) {
  .nav-panel {
    grid-template-columns: 1fr auto 1fr;
    padding: 0 3rem;
  }

  .left {
    display: flex;
    justify-content: flex-start;
  }

  .nav-links {
    display: flex;
    align-items: center;
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

  .nav-links :deep(a.nav-admin) {
    color: #8ddbe6;
    text-shadow: 0 0 8px rgba(61, 232, 200, 0.25);
  }

  .nav-contribute {
    font-size: 0.9rem;
    color: #3de8c8;
    font-weight: 600;
    text-decoration: none;
    background: none;
    border: none;
    padding: 0;
    cursor: pointer;
    transition: opacity 0.2s ease;
  }

  .nav-contribute:hover {
    opacity: 0.7;
  }

  .right {
    display: flex;
    justify-content: flex-end;
    align-items: center;
    gap: 0.75rem;
  }

  .user-name {
    cursor: pointer;
    color: #8ddbe6;
    font-size: 0.9rem;
    white-space: nowrap;
  }

  .mobile-menu-btn {
    display: none;
  }
}

/* ── Mobile layout ─────────────────────────────────────────── */

@media (max-width: 767px) {
  .nav-panel {
    grid-template-columns: 1fr auto;
    padding: 0 1.25rem;
  }

  .left {
    display: flex;
    align-items: center;
  }

  .nav-links,
  .right {
    display: none;
  }

  .mobile-menu-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 2.25rem;
    height: 2.25rem;
    background: none;
    border: none;
    border-radius: 8px;
    color: #8ddbe6;
    cursor: pointer;
    transition: background 0.15s ease;
    flex-shrink: 0;
  }

  .mobile-menu-btn:hover {
    background: rgba(255, 255, 255, 0.06);
  }

  .mobile-menu-btn__icon {
    width: 1.3rem;
    height: 1.3rem;
  }
}

/* ── Mobile drawer ─────────────────────────────────────────── */

.mobile-drawer {
  position: fixed;
  top: 3.5rem;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 49;
  background: #07080f;
  border-top: 1px solid rgba(141, 219, 230, 0.12);
  padding: 1.25rem 1.25rem 2rem;
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  overflow-y: auto;
}

/* Never show the drawer on desktop */
@media (min-width: 768px) {
  .mobile-drawer {
    display: none;
  }
}

.drawer-search :deep(.search-wrapper) {
  width: 100%;
}

.drawer-nav {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.drawer-link {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.85rem 1rem;
  border-radius: 12px;
  font-size: 1rem;
  color: #8ddbe6;
  text-decoration: none;
  background: none;
  border: none;
  width: 100%;
  text-align: left;
  cursor: pointer;
  transition: background 0.15s ease, color 0.15s ease;
}

.drawer-link:hover,
.drawer-link:active {
  background: rgba(141, 219, 230, 0.08);
  color: #dffaff;
}

.drawer-link--admin {
  color: #3de8c8;
}

.drawer-link--account {
  color: #8ddbe6;
  font-weight: 500;
}

.drawer-link--logout {
  color: #8a93b8;
}

.drawer-link__icon {
  width: 1.1rem;
  height: 1.1rem;
  flex-shrink: 0;
}

.drawer-divider {
  height: 1px;
  background: rgba(141, 219, 230, 0.08);
}

.drawer-account {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-top: auto;
}

.drawer-contribute {
  display: block;
  width: 100%;
  padding: 0.85rem 1rem;
  border-radius: 12px;
  text-align: center;
  font-size: 1rem;
  font-weight: 600;
  color: #3de8c8;
  background: rgba(61, 232, 200, 0.08);
  border: 1px solid rgba(61, 232, 200, 0.2);
  text-decoration: none;
  cursor: pointer;
  transition: background 0.15s ease;
}

.drawer-contribute:hover,
.drawer-contribute:active {
  background: rgba(61, 232, 200, 0.14);
}

.drawer-signin {
  width: 100%;
}

/* ── Drawer transition ─────────────────────────────────────── */

.drawer-enter-active,
.drawer-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}

.drawer-enter-from,
.drawer-leave-to {
  opacity: 0;
  transform: translateY(-6px);
}
</style>
