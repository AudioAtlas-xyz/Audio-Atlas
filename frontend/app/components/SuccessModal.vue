<script setup>
const emit = defineEmits(['close'])
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

      <div class="checkmark">✓</div>

      <h2>Account created</h2>
      <p>You’re ready to explore Audio Atlas.</p>
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
  position: relative;
  width: 420px;
  padding: 2rem;
  border-radius: 18px;
  background: #050816;
  border: 1px solid rgba(120, 150, 255, 0.18);
  text-align: center;
  color: #eef2ff;

  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.5);

  animation: scaleIn 0.2s ease;
}

.close {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: transparent;
  border: none;
  color: #eef2ff;
  font-size: 1.2rem;
  cursor: pointer;
}

.checkmark {
  width: 54px;
  height: 54px;
  margin: 0 auto 1rem;
  border: 2px solid #44f0c4;
  border-radius: 50%;
  color: #44f0c4;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.4rem;
  font-weight: bold;

  animation: popIn 0.25s ease;
}

h2 {
  margin: 0 0 0.5rem;
  font-size: 1.4rem;
}

p {
  margin: 0;
  color: #6b7395;
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

@keyframes popIn {
  from {
    transform: scale(0.6);
    opacity: 0;
  }
  to {
    transform: scale(1);
    opacity: 1;
  }
}
</style>