<script setup>
import { ref } from 'vue'

const props = defineProps({
  pendingRegistrationId: {
    type: String,
    required: true
  }
})

const emit = defineEmits(['close', 'finished'])

const username = ref('')
const acceptedTerms = ref(false)

const finish = async () => {
  const response = await $fetch('http://localhost:5000/api/auth/complete-onboarding', {
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
}
</script>

<template>
  <div class="overlay" @click.self="emit('close')">
    <div class="modal">
      <button class="close" @click="emit('close')">×</button>

      <h2>Choose a username</h2>

      <input
        v-model="username"
        type="text"
        placeholder="e.g. xxAfrobeats67Xx"
        class="username-input"
      >

      <p class="status">
        Username available.
      </p>

      <label class="checkbox-row">
        <input v-model="acceptedTerms" type="checkbox">

        <span>
          By creating an account, I agree to the
          <a href="/contribution-guidelines" target="_blank">
            contribution guidelines
          </a>
          and
          <a href="/privacy-policy" target="_blank">
            privacy policy
          </a>.
        </span>
      </label>

      <div class="buttons">
        <button class="cancel" @click="emit('close')">
          ← Cancel
        </button>

        <button
          class="finish"
          :disabled="!acceptedTerms"
          @click="finish"
        >
          Finish →
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
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
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.45);
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
  color: #44f0c4;
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

.cancel,
.finish {
  border: none;
  border-radius: 8px;
  padding: 0.5rem 1rem;
  font-size: 0.78rem;
  font-weight: 600;
  cursor: pointer;
}

.cancel {
  background: #8b8dff;
  color: #101322;
}

.finish {
  background: #44f0c4;
  color: #071117;
}

.finish:disabled {
  opacity: 0.45;
  cursor: not-allowed;
}
</style>