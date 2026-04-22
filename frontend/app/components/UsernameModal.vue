<script setup>
import { ref, watch } from 'vue'
import GlassButton from '@/components/GlassButton.vue'

const config = useRuntimeConfig()

const props = defineProps({
  pendingRegistrationId: {
    type: String,
    required: false
  }
})

const emit = defineEmits(['close', 'finished'])

const username = ref('')
const acceptedTerms = ref(false)
const loading = ref(false)
const usernameStatus = ref(null)

let debounceTimeout = null

watch(username, (value) => {
  usernameStatus.value = null

  if (!value || value.length < 3 || value.length > 20) {
    usernameStatus.value = 'invalid'
    return
  }

  clearTimeout(debounceTimeout)

  debounceTimeout = setTimeout(async () => {
    try {
      usernameStatus.value = 'checking'

      const res = await $fetch(`${config.public.apiBase}/api/auth/check-username`, {
        params: { username: value }
      })

      usernameStatus.value = res.available ? 'available' : 'taken'
    } catch {
      usernameStatus.value = 'invalid'
    }
  }, 400)
})

const finish = async () => {
  if (
    !props.pendingRegistrationId ||
    loading.value ||
    usernameStatus.value !== 'available'
  ) return

  try {
    loading.value = true

    const response = await $fetch(`${config.public.apiBase}/api/auth/complete-onboarding`, {
      method: 'POST',
      body: {
        pendingRegistrationId: props.pendingRegistrationId,
        username: username.value,
        acceptedContributionGuidelines: acceptedTerms.value,
        acceptedPrivacyPolicy: acceptedTerms.value
      }
    })

    localStorage.setItem('token', response.token)
    emit('finished')
  } catch (err) {
    console.error(err)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="overlay" role="dialog" aria-modal="true" @click.self="emit('close')">
    <div class="modal">
      <button class="close" aria-label="Close" @click="emit('close')">×</button>

      <h2>Choose a username</h2>

      <input
        v-model="username"
        type="text"
        placeholder="e.g. xxAfrobeats67Xx"
        class="username-input"
      >

      <p class="status" v-if="usernameStatus === 'checking'">
        Checking username...
      </p>

      <p class="status success" v-if="usernameStatus === 'available'">
        Username available ✓
      </p>

      <p class="status error" v-if="usernameStatus === 'taken'">
        Username is already taken
      </p>

      <p class="status error" v-if="usernameStatus === 'invalid'">
        3–20 characters, letters/numbers/underscore only
      </p>

      <label class="checkbox-row">
        <input v-model="acceptedTerms" type="checkbox">

        <span>
          By creating an account, I agree to the
          <a href="/contribution-guidelines" target="_blank">contribution guidelines</a>
          and
          <a href="/privacy-policy" target="_blank">privacy policy</a>.
        </span>
      </label>

      <div class="buttons">
        <GlassButton @click="emit('close')">
          Cancel
        </GlassButton>

        <GlassButton
          variant="primary"
          :disabled="loading || !acceptedTerms || usernameStatus !== 'available'"
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
  -webkit-backdrop-filter: blur(8px);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal {
  position: relative;
  width: 420px;
  padding: 1.75rem;
  border-radius: 18px;
  background: #050816;
  border: 1px solid rgba(120, 150, 255, 0.18);
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);
  color: #eef2ff;
}

.close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  border: none;
  background: transparent;
  color: #d7ddff;
  font-size: 1.2rem;
  cursor: pointer;
}

h2 {
  margin: 0 0 1.25rem;
  font-size: 0.72rem;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: #f4f6ff;
}

.username-input {
  width: 100%;
  padding: 0.85rem 0.95rem;
  border-radius: 8px;
  border: 1px solid rgba(117, 137, 230, 0.45);
  background: transparent;
  color: #eef2ff;
  font-size: 0.9rem;
  box-sizing: border-box;
}

.username-input::placeholder {
  color: #65709b;
}

.status {
  margin-top: 0.55rem;
  font-size: 0.75rem;
}

.status.success {
  color: #44f0c4;
}

.status.error {
  color: #ff6b6b;
}

.checkbox-row {
  display: flex;
  align-items: flex-start;
  gap: 0.65rem;
  margin-top: 1rem;
  font-size: 0.78rem;
  line-height: 1.4;
  color: #cfd5f7;
}

.checkbox-row input {
  margin-top: 0.15rem;
  accent-color: #44f0c4;
}

.checkbox-row a {
  color: #eef2ff;
  text-decoration: underline;
}

.buttons {
  display: flex;
  justify-content: space-between;
  margin-top: 1.5rem;
}
</style>