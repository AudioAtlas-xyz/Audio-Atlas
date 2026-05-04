<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

/**
 * Generic success popup. Caller supplies `title` and `message`; the layout
 * (checkmark, blur backdrop, auto-close after 2 s) stays the same.
 */
withDefaults(
  defineProps<{
    title?: string
    message?: string
  }>(),
  {
    title: 'Success',
    message: ''
  }
)

/**
 * Emits event for closing the modal
 */
const emit = defineEmits(['close'])

/**
 * Ref for focus management
 */
const modalRef = ref<HTMLElement | null>(null)

/**
 * ESC key handler
 */
const handleKey = (e: KeyboardEvent) => {
  if (e.key === 'Escape') emit('close')
}

onMounted(() => {
  window.addEventListener('keydown', handleKey)

  // focus modal for accessibility
  modalRef.value?.focus()

  // optional auto-close after 2s
  setTimeout(() => {
    emit('close')
  }, 2000)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKey)
})
</script>

<template>
  <!-- Overlay background -->
  <div
    class="overlay"
    role="dialog"
    aria-modal="true"
    aria-labelledby="success-title"
    @click.self="emit('close')"
  >
    <div
      ref="modalRef"
      class="modal"
      tabindex="-1"
    >
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
      <h2 id="success-title">{{ title }}</h2>
      <p v-if="message">{{ message }}</p>
    </div>
  </div>
</template>

<style scoped>
/* Overlay */
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

/* Modal */
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

/* Close button */
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

/* Checkmark */
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

  box-shadow: 0 0 12px rgba(68, 240, 196, 0.4);

  animation: popIn 0.25s ease;
}

/* Title */
h2 {
  margin: 0 0 0.5rem;
  font-size: 1.4rem;
}

/* Subtitle */
p {
  margin: 0;
  color: #6b7395;
}

/* Animations */
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