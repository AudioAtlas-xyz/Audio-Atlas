<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, computed } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { useApi } from '@/composables/useApi'
import DeleteAccountConfirmModal from '@/components/UserFlow/DeleteAccountConfirmModal.vue'

const props = defineProps<{ open: boolean }>()
const emit = defineEmits<{
  (e: 'close'): void
  (e: 'username-updated', username: string): void
  (e: 'account-deleted'): void
}>()

const { user, logout } = useAuth()
const { api } = useApi()

// Local editable username
const username = ref('')
// Store original username to avoid unnecessary updates
const originalUsername = ref('')

// Status for username validation UI
const status = ref<'idle' | 'checking' | 'available' | 'taken' | 'error'>('idle')
const message = ref('')

// Delete state
const deleting = ref(false)
const showDeleteConfirm = ref(false)

// Derived connection states
const isGoogleConnected = computed(() =>
  user.value?.provider?.toLowerCase() === 'google'
)

const isGitHubConnected = computed(() =>
  user.value?.provider?.toLowerCase() === 'github'
)

/**
 * When modal opens:
 * - Sync username from user
 * - Reset validation state
 */
watch(
  () => props.open,
  (open) => {
    if (!open) return

    const current = user.value?.username || ''
    username.value = current
    originalUsername.value = current

    status.value = 'idle'
    message.value = ''
  },
  { immediate: true }
)

/**
 * Debounced username availability check
 * Includes race protection to avoid outdated responses overwriting state
 */
let timeout: ReturnType<typeof setTimeout> | null = null
let requestId = 0

watch(username, (val) => {
  // Reset if empty
  if (!val) {
    status.value = 'idle'
    message.value = ''
    return
  }

  // Skip check if unchanged
  if (val === originalUsername.value) {
    status.value = 'idle'
    message.value = ''
    return
  }

  // Clear previous debounce
  if (timeout) clearTimeout(timeout)

  // Track request to avoid race conditions
  const currentRequest = ++requestId

  timeout = setTimeout(async () => {
    status.value = 'checking'

    try {
      const res = await api<{ available: boolean; message: string }>(
        `/auth/check-username?username=${val}`
      )

      // Ignore outdated responses
      if (currentRequest !== requestId) return

      status.value = res.available ? 'available' : 'taken'
      message.value = res.message
    } catch {
      if (currentRequest !== requestId) return

      status.value = 'error'
      message.value = 'Error checking username'
    }
  }, 300)
})

/**
 * Save username if valid and changed
 */
const save = async () => {
  if (
    status.value === 'checking' ||
    status.value === 'taken' ||
    username.value === originalUsername.value
  ) return

  try {
    const res = await api<{ username: string }>(`/auth/username`, {
      method: 'PUT',
      body: { username: username.value }
    })

    // Update local user state
  if (user.value) {
    user.value.username = res.username
  }

  // emit event BEFORE closing
  emit('username-updated', res.username)

close()
  } catch (err: any) {
    console.error(err)
    status.value = 'error'
    message.value = err?.data || 'Failed to update username'
  }
}

/**
 * Open the custom confirmation modal. The actual API call happens in
 * `confirmDelete` once the user ticks the acknowledgement box and clicks
 * Continue inside DeleteAccountConfirmModal.
 */
const requestDelete = () => {
  if (deleting.value) return
  showDeleteConfirm.value = true
}

const cancelDelete = () => {
  if (deleting.value) return
  showDeleteConfirm.value = false
}

/**
 * Delete account permanently. Called by DeleteAccountConfirmModal's `confirm`
 * event. On success: log out, close both modals, and emit `account-deleted`
 * so the parent (AppHeader → default.vue) can show the "Account deleted"
 * SuccessModal.
 */
const confirmDelete = async () => {
  if (deleting.value) return
  deleting.value = true

  try {
    await api('/user/delete', { method: 'DELETE' })

    // Clear local session before tearing down the UI.
    await logout()

    showDeleteConfirm.value = false
    emit('account-deleted')
    close()
  } catch (err) {
    console.error(err)
    // Keep the confirm modal open on failure so the user can retry or cancel.
    alert('Failed to delete account')
  } finally {
    deleting.value = false
  }
}

/**
 * Emit close event
 */
const close = () => emit('close')

/**
 * Allow ESC to close modal
 */
const handleKey = (e: KeyboardEvent) => {
  if (e.key === 'Escape') close()
}

onMounted(() => window.addEventListener('keydown', handleKey))
onUnmounted(() => window.removeEventListener('keydown', handleKey))
</script>

<template>
  <transition name="fade">
    <div v-if="open" class="overlay" @click.self="close">
      <div class="modal">
        <h2 class="title">Account details</h2>

        <div class="divider" />

        <!-- Username -->
        <div class="section">
          <label>Username</label>

          <input v-model="username" class="input" />

          <p v-if="status === 'checking'" class="hint">Checking...</p>
          <p v-else-if="status === 'available'" class="hint success">{{ message }}</p>
          <p v-else-if="status === 'taken'" class="hint error">{{ message }}</p>
          <p v-else-if="status === 'error'" class="hint error">{{ message }}</p>
        </div>

        <div class="divider" />

        <!-- Connected accounts -->
        <div class="section">
          <p class="section-title">Connected accounts</p>

          <div class="account-row">
            <span class="provider">GitHub</span>
            <span class="muted">
              {{ isGitHubConnected ? user?.username : 'Not signed in' }}
            </span>

            <button :class="isGitHubConnected ? 'connected' : 'disconnected'">
              {{ isGitHubConnected ? 'Connected' : 'Disconnected' }}
            </button>
          </div>

          <div class="account-row">
            <span class="provider">Google</span>
            <span class="muted">
              {{ isGoogleConnected ? user?.email : 'Not signed in' }}
            </span>

            <button :class="isGoogleConnected ? 'connected' : 'disconnected'">
              {{ isGoogleConnected ? 'Connected' : 'Disconnected' }}
            </button>
          </div>
        </div>

        <div class="divider" />

        <!-- Actions -->
        <div class="actions">
          <button
            class="save"
            :disabled="status === 'checking' || status === 'taken'"
            @click="save"
          >
            Save changes
          </button>

          <button
            class="delete"
            :disabled="deleting"
            @click="requestDelete"
          >
            {{ deleting ? 'Deleting...' : 'Delete Account' }}
          </button>
        </div>

        <!-- Destructive confirmation overlay. Mounted inside AccountDetails so
             ESC / overlay-click on this modal doesn't unmount AccountDetails
             underneath. The confirm modal sits at z-index 1100, above this one. -->
        <DeleteAccountConfirmModal
          v-if="showDeleteConfirm"
          :loading="deleting"
          @close="cancelDelete"
          @confirm="confirmDelete"
        />
      </div>
    </div>
  </transition>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  z-index: 100;
  display: flex;
  justify-content: center;
  align-items: center;
  background: rgba(2, 6, 18, 0.55);
  backdrop-filter: blur(12px);
}

.modal {
  width: 100%;
  max-width: 720px;
  padding: 1.75rem 2rem;
  border-radius: 18px;
  background:
    radial-gradient(
      1200px 400px at 50% -200px,
      rgba(61, 232, 200, 0.06),
      transparent
    ),
    #040713;
  border: 1px solid rgba(141, 219, 230, 0.08);
  box-shadow:
    0 20px 60px rgba(0, 0, 0, 0.6),
    inset 0 0 0 1px rgba(255, 255, 255, 0.02);
  display: flex;
  flex-direction: column;
  gap: 1.35rem;
}

.title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #e8f6fb;
}

.section {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.section-title {
  color: #8ec7d2;
  font-size: 0.9rem;
}

.provider {
  font-weight: 500;
  color: #dffaff;
}

.muted {
  color: #6f9ea7;
  font-size: 0.85rem;
}

.divider {
  height: 1px;
  background: rgba(141, 219, 230, 0.08);
}

.input {
  height: 2.6rem;
  border-radius: 12px;
  padding: 0 0.9rem;
  background: rgba(255, 255, 255, 0.015);
  border: 1px solid rgba(141, 219, 230, 0.15);
  color: #dffaff;
}

.input:focus {
  outline: none;
  border-color: rgba(61, 232, 200, 0.5);
  box-shadow: 0 0 0 1px rgba(61, 232, 200, 0.25);
}

.hint {
  font-size: 0.75rem;
}

.success {
  color: #3de8c8;
}

.error {
  color: #ff6b6b;
}

.account-row {
  display: grid;
  grid-template-columns: 1fr auto auto;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem 1rem;
  border-radius: 14px;
  background: rgba(255, 255, 255, 0.025);
  border: 1px solid rgba(141, 219, 230, 0.06);
}

.connected {
  background: transparent;
  border: 1px solid #3de8c8;
  color: #3de8c8;
  border-radius: 999px;
  padding: 0.35rem 0.9rem;
  font-size: 0.8rem;
}

.disconnected {
  background: transparent;
  border: 1px solid #ff6b6b;
  color: #ff6b6b;
  border-radius: 999px;
  padding: 0.35rem 0.9rem;
  font-size: 0.8rem;
}

.actions {
  display: flex;
  justify-content: space-between;
}

.save {
  background: #3de8c8;
  color: #02211c;
  border-radius: 10px;
  padding: 0.6rem 1rem;
}

.save:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.delete {
  background: #c94b5b;
  color: white;
  border-radius: 10px;
  padding: 0.6rem 1rem;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.18s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>