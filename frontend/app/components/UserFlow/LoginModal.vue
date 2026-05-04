<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'

const emit = defineEmits<{
  (e: 'close'): void
}>()

const config = useRuntimeConfig()

const loginWithGoogle = () => {
  window.location.href = `${config.public.backendBaseUrl}/api/auth/login/google`
}

const loginWithGithub = () => {
  window.location.href = `${config.public.backendBaseUrl}/api/auth/login/github`
}

/* ESC support */
const handleKey = (e: KeyboardEvent) => {
  if (e.key === 'Escape') emit('close')
}

onMounted(() => window.addEventListener('keydown', handleKey))
onUnmounted(() => window.removeEventListener('keydown', handleKey))
</script>

<template>
  <div
    class="overlay"
    role="dialog"
    aria-modal="true"
    @click.self="emit('close')"
  >
    <div class="modal">
      <!-- CLOSE -->
      <button
        class="close"
        aria-label="Close"
        @click="emit('close')"
      >
        ×
      </button>

      <!-- TITLE -->
      <div class="header">
        <h2>Log in</h2>
        <p>Log in to contribute</p>
      </div>

      <!-- ACTIONS -->
      <div class="actions">
        <button class="oauth-button google" @click="loginWithGoogle">
          Continue with Google
        </button>

        <button class="oauth-button github" @click="loginWithGithub">
          Continue with GitHub
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  z-index: 1000;

  display: flex;
  justify-content: center;
  align-items: center;

  background: rgba(2, 6, 18, 0.6);
  backdrop-filter: blur(12px);
}

.modal {
  width: 420px;
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
  gap: 1.25rem;

  animation: scaleIn 0.18s ease;
}

/* HEADER */
.header h2 {
  font-size: 1.3rem;
  font-weight: 600;
  color: #e8f6fb;
}

.header p {
  font-size: 0.85rem;
  color: #7aaeb8;
}

/* CLOSE */
.close {
  position: absolute;
  top: 1rem;
  right: 1rem;

  background: none;
  border: none;
  color: #7aaeb8;
  font-size: 1.2rem;
  cursor: pointer;
}

/* ACTIONS */
.actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

/* BUTTONS */
.oauth-button {
  padding: 0.75rem;
  border-radius: 10px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.15s ease;
}

/* Google */
.oauth-button.google {
  background: #3de8c8;
  color: #02211c;
}

.oauth-button.google:hover {
  transform: translateY(-2px);
}

/* GitHub */
.oauth-button.github {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(141, 219, 230, 0.2);
  color: #dffaff;
}

.oauth-button.github:hover {
  background: rgba(255, 255, 255, 0.08);
}

@keyframes scaleIn {
  from {
    transform: scale(0.95);
    opacity: 0;
  }
  to {
    transform: scale(1);
    opacity: 1;
  }
}
</style>