<script setup>
const emit = defineEmits(['close'])

const config = useRuntimeConfig()

const loginWithGoogle = () => {
  window.location.href = `${config.public.apiBase}/api/auth/login/google`
}

const loginWithGithub = () => {
  window.location.href = `${config.public.apiBase}/api/auth/login/github`
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
      <button
        class="close"
        aria-label="Close"
        @click="emit('close')"
      >
        ×
      </button>

      <h2>Log in</h2>
      <p>Log in to contribute</p>

      <button class="oauth-button" @click="loginWithGoogle">
        Continue with Google
      </button>

      <button class="oauth-button github" @click="loginWithGithub">
        Continue with GitHub
      </button>
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

  animation: fadeIn 0.2s ease;
}

.modal {
  width: 420px;
  padding: 2rem;
  border-radius: 20px;
  background: #050816;
  color: white;
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 1rem;

  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);

  animation: scaleIn 0.2s ease;
}

.close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: none;
  border: none;
  color: white;
  font-size: 1.3rem;
  cursor: pointer;
}

.oauth-button {
  padding: 0.9rem;
  border-radius: 10px;
  border: none;
  cursor: pointer;
  background: #8ddbe6;
  color: #02070a;
  font-weight: 600;

  transition: transform 0.15s ease, opacity 0.15s ease;
}

.oauth-button:hover {
  transform: translateY(-2px);
  opacity: 0.9;
}

.oauth-button:active {
  transform: translateY(0);
}

.oauth-button.github {
  background: #24292e;
  color: white;
}

@keyframes fadeIn {
  from { opacity: 0 }
  to { opacity: 1 }
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