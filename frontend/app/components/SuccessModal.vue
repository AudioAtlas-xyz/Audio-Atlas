<script setup lang="ts">
/**
 * Emits event for closing the modal
 */
const emit = defineEmits(['close'])
</script>

<template>
  <!-- Overlay background (click outside to close) -->
  <div
    class="overlay"
    role="dialog"
    aria-modal="true"
    @click.self="emit('close')"
  >
    <div class="modal">
      <!-- Close button -->
      <button
        class="close"
        aria-label="Close"
        @click="emit('close')"
      >
        ×
      </button>

      <!-- Success indicator -->
      <div class="checkmark">✓</div>

      <!-- Success message -->
      <h2>Account created</h2>
      <p>You’re ready to explore Audio Atlas.</p>
    </div>
  </div>
</template>

<style scoped>
/* Fullscreen overlay with blur background */
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

/* Modal container */
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

/* Close button styling */
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

/* Animated success checkmark */
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

/* Title */
h2 {
  margin: 0 0 0.5rem;
  font-size: 1.4rem;
}

/* Subtitle text */
p {
  margin: 0;
  color: #6b7395;
}

/* Fade-in animation */
@keyframes fadeIn {
  from { opacity: 0 }
  to { opacity: 1 }
}

/* Scale-in animation */
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

/* Checkmark pop animation */
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