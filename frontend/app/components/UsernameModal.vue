<script setup lang="ts">
import { ref, watch, computed, onMounted, onUnmounted, nextTick } from 'vue'
import GlassButton from '@/components/GlassButton.vue'
import { useAuth } from '@/composables/useAuth'
import { useApi } from '@/composables/useApi'

/**
 * API
 */
const { api } = useApi()

/**
 * Global auth state
 */
const { fetchUser } = useAuth()

const {
  pendingRegistrationId,
  suggestedUsername
} = useUIState()

/**
 * Emits
 */
const emit = defineEmits(['close', 'finished'])

/**
 * Local state
 */
const username = ref('')
const acceptedGuidelines = ref(false)
const acceptedPrivacy = ref(false)
const loading = ref(false)

const usernameStatus = ref<
  'idle' | 'checking' | 'available' | 'taken' | 'invalid'
>('idle')

/**
 * Input ref (auto focus)
 */
const inputRef = ref<HTMLInputElement | null>(null)

/**
 * Validation helpers
 */
const isValidUsername = (val: string) =>
  /^[a-zA-Z0-9_]{3,20}$/.test(val)

/**
 * Debounce + race control
 */
let debounceTimeout: ReturnType<typeof setTimeout> | null = null
let currentRequest = 0
let lastValue = ''

/**
 * Initialize username
 */
onMounted(async () => {
  if (suggestedUsername.value) {
    username.value = suggestedUsername.value
  }

  await nextTick()
  inputRef.value?.focus()

  window.addEventListener('keydown', handleKey)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKey)
})

/**
 * ESC close
 */
const handleKey = (e: KeyboardEvent) => {
  if (e.key === 'Escape') emit('close')
}

/**
 * Username validation
 */
watch(username, (value) => {
  if (debounceTimeout) clearTimeout(debounceTimeout)

  if (value === lastValue) return
  lastValue = value

  if (!isValidUsername(value)) {
    usernameStatus.value = 'invalid'
    return
  }

  debounceTimeout = setTimeout(async () => {
    const requestId = ++currentRequest

    try {
      usernameStatus.value = 'checking'

      const res = await api<{ available: boolean }>(
        `/auth/check-username`,
        {
          params: { username: value }
        }
      )

      if (requestId !== currentRequest) return

      usernameStatus.value = res.available ? 'available' : 'taken'
    } catch {
      if (requestId !== currentRequest) return
      usernameStatus.value = 'invalid'
    }
  }, 400)
})

/**
 * UI helpers
 */
const statusText = computed(() => {
  switch (usernameStatus.value) {
    case 'checking': return 'Checking username...'
    case 'available': return 'Username available ✓'
    case 'taken': return 'Username is already taken'
    case 'invalid': return '3–20 characters, letters/numbers/underscore only'
    default: return ' '
  }
})

const statusClass = computed(() => ({
  success: usernameStatus.value === 'available',
  error:
    usernameStatus.value === 'taken' ||
    usernameStatus.value === 'invalid'
}))

const hasAcceptedAll = computed(() =>
  acceptedGuidelines.value && acceptedPrivacy.value
)

const canSubmit = computed(() =>
  !loading.value &&
  hasAcceptedAll.value &&
  usernameStatus.value === 'available'
)

/**
 * Complete onboarding
 */
const finish = async () => {
  const id = pendingRegistrationId.value
  if (!canSubmit.value || !id) return

  try {
    loading.value = true

    const response = await api<{ token: string }>(
      `/auth/complete-onboarding`,
      {
        method: 'POST',
        body: {
          pendingRegistrationId: id,
          username: username.value,
          acceptedContributionGuidelines: acceptedGuidelines.value,
          acceptedPrivacyPolicy: acceptedPrivacy.value
        }
      }
    )

    localStorage.setItem('token', response.token)

    await fetchUser()

    pendingRegistrationId.value = null
    suggestedUsername.value = null

    emit('finished')
  } catch (err) {
    console.error('Onboarding failed:', err)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div
    class="overlay"
    role="dialog"
    aria-modal="true"
    @click.self="emit('close')"
  >
    <div class="modal">
      <button class="close" @click="emit('close')">×</button>

      <h2>Choose a username</h2>

      <input
        ref="inputRef"
        v-model="username"
        type="text"
        placeholder="e.g. xxAfrobeats67Xx"
        class="username-input"
        @keyup.enter="canSubmit && finish()"
      >

      <div class="status-wrapper">
        <p class="status" :class="statusClass">
          <span
            v-if="usernameStatus === 'checking'"
            class="spinner"
          />
          {{ statusText }}
        </p>
      </div>

      <div class="checkbox-group">
        <label class="checkbox-row">
          <input v-model="acceptedGuidelines" type="checkbox">
          <span>
            I agree to the
            <a href="/contribution-guidelines" target="_blank" class="link-green">
              Contribution Guidelines
            </a>
          </span>
        </label>

        <label class="checkbox-row">
          <input v-model="acceptedPrivacy" type="checkbox">
          <span>
            I agree to the
            <a href="/privacy-policy" target="_blank" class="link-green">
              Privacy Policy
            </a>
          </span>
        </label>
      </div>

      <p class="hint" v-if="!hasAcceptedAll">
        Please accept both policies to continue
      </p>

      <div class="buttons">
        <GlassButton @click="emit('close')">
          Cancel
        </GlassButton>

        <GlassButton
          variant="primary"
          :disabled="!canSubmit"
          @click="finish"
        >
          {{ loading ? 'Creating...' : 'Finish' }}
        </GlassButton>
      </div>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
  backdrop-filter: blur(8px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal {
  width: 480px;
  min-height: 280px;
  padding: 2rem;
  border-radius: 18px;
  background: #050816;
  border: 1px solid rgba(120, 150, 255, 0.18);
  color: #eef2ff;
}

.close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: none;
  border: none;
  color: #fff;
  cursor: pointer;
}

.username-input {
  width: 100%;
  padding: 0.85rem;
  border-radius: 8px;
  border: 1px solid rgba(117, 137, 230, 0.45);
  background: transparent;
  color: #eef2ff;
}

.status-wrapper {
  min-height: 22px;
  margin-top: 0.5rem;
}

.status {
  font-size: 0.75rem;
  display: flex;
  align-items: center;
  gap: 6px;
}

.status.success {
  color: #44f0c4;
}

.status.error {
  color: #ff6b6b;
}

.spinner {
  width: 12px;
  height: 12px;
  border: 2px solid rgba(255,255,255,0.2);
  border-top: 2px solid #44f0c4;
  border-radius: 50%;
  animation: spin 0.6s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.checkbox-group {
  margin-top: 1rem;
}

.checkbox-row {
  display: flex;
  gap: 0.6rem;
  margin-top: 0.5rem;
  font-size: 0.78rem;
}

.checkbox-row input {
  accent-color: #44f0c4;
  cursor: pointer;
}

.hint {
  font-size: 0.7rem;
  color: #aaa;
  margin-top: 0.5rem;
}

.buttons {
  display: flex;
  justify-content: space-between;
  margin-top: 1.5rem;
}

.link-green {
  color: #44f0c4;
  text-decoration: none;
  font-weight: 500;
  cursor: pointer;
  transition: opacity 0.2s ease, text-decoration 0.2s ease;
}

.link-green:hover {
  opacity: 0.85;
  text-decoration: underline;
}
</style>