<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'

/**
 * Two-step destructive confirmation modal.
 *
 * Flow:
 *   1. Caller mounts this modal.
 *   2. User must tick "I understand this is permanent".
 *   3. Continue button is disabled until the box is checked AND `loading`
 *      is false.
 *   4. On Continue, we emit `confirm` and switch to the disabled spinner
 *      label until the parent unmounts us.
 *
 * The parent owns the actual delete API call (so the parent can react to
 * success/failure with the right side-effects: logout, success modal, etc.).
 */
const props = defineProps<{
  /** Disables Continue while the parent's delete request is in flight. */
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'confirm'): void
}>()

const CONFIRM_PHRASE = 'delete my account'
const typedPhrase = ref('')

const canConfirm = () =>
  typedPhrase.value.trim().toLowerCase() === CONFIRM_PHRASE && !props.loading

const onConfirm = () => {
  if (!canConfirm()) return
  emit('confirm')
}

/* ESC closes — but only when not mid-request, to avoid orphaned API calls. */
const handleKey = (e: KeyboardEvent) => {
  if (e.key === 'Escape' && !props.loading) emit('close')
}

onMounted(() => window.addEventListener('keydown', handleKey))
onUnmounted(() => window.removeEventListener('keydown', handleKey))
</script>

<template>
  <div
    class="overlay"
    role="dialog"
    aria-modal="true"
    aria-labelledby="delete-confirm-title"
    @click.self="!loading && emit('close')"
  >
    <div class="modal">
      <button
        class="close"
        aria-label="Close"
        :disabled="loading"
        @click="emit('close')"
      >
        ×
      </button>

      <!-- Warning icon -->
      <div class="warning-icon" aria-hidden="true">!</div>

      <h2 id="delete-confirm-title">Delete account</h2>
      <p class="body">
        Are you sure you want to delete your account?
        This cannot be undone.
      </p>

      <div class="phrase-row">
        <p class="phrase-label">
          Type <strong>delete my account</strong> to confirm:
        </p>
        <input
          v-model="typedPhrase"
          class="phrase-input"
          type="text"
          placeholder="delete my account"
          :disabled="loading"
          autocomplete="off"
          spellcheck="false"
        >
      </div>

      <div class="buttons">
        <button
          class="cancel"
          :disabled="loading"
          @click="emit('close')"
        >
          Cancel
        </button>

        <button
          class="continue"
          :disabled="!canConfirm()"
          @click="onConfirm"
        >
          {{ loading ? 'Deleting...' : 'Continue' }}
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  z-index: 1100; /* sits above AccountDetails (z-index 100) */

  display: flex;
  justify-content: center;
  align-items: center;

  background: rgba(2, 6, 18, 0.65);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);

  animation: fadeIn 0.18s ease;
}

.modal {
  position: relative;
  width: 440px;
  padding: 1.75rem 2rem 1.5rem;
  border-radius: 18px;

  background:
    radial-gradient(
      1200px 400px at 50% -200px,
      rgba(201, 75, 91, 0.08),
      transparent
    ),
    #050816;

  border: 1px solid rgba(201, 75, 91, 0.25);
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.6);

  color: #eef2ff;
  text-align: center;

  display: flex;
  flex-direction: column;
  gap: 1rem;

  animation: scaleIn 0.18s ease;
}

.close {
  position: absolute;
  top: 0.85rem;
  right: 1rem;

  background: none;
  border: none;
  color: #7aaeb8;
  font-size: 1.2rem;
  cursor: pointer;
}

.close:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.warning-icon {
  width: 48px;
  height: 48px;
  margin: 0 auto;

  border: 2px solid #ff6b6b;
  border-radius: 50%;

  display: flex;
  align-items: center;
  justify-content: center;

  color: #ff6b6b;
  font-size: 1.6rem;
  font-weight: bold;

  box-shadow: 0 0 12px rgba(255, 107, 107, 0.35);
}

h2 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.body {
  margin: 0;
  color: #9aa3b8;
  font-size: 0.9rem;
  line-height: 1.45;
}

.phrase-row {
  display: flex;
  flex-direction: column;
  gap: 0.45rem;
  margin-top: 0.25rem;
  text-align: left;
}

.phrase-label {
  margin: 0;
  font-size: 0.85rem;
  color: #9aa3b8;
}

.phrase-label strong {
  color: #dffaff;
}

.phrase-input {
  width: 100%;
  padding: 0.55rem 0.85rem;
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(201, 75, 91, 0.3);
  color: #dffaff;
  font-size: 0.9rem;
  box-sizing: border-box;
}

.phrase-input:focus {
  outline: none;
  border-color: rgba(201, 75, 91, 0.7);
  box-shadow: 0 0 0 1px rgba(201, 75, 91, 0.3);
}

.phrase-input:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.buttons {
  display: flex;
  justify-content: space-between;
  gap: 0.75rem;
  margin-top: 0.5rem;
}

.cancel,
.continue {
  flex: 1;
  padding: 0.65rem 1rem;
  border-radius: 10px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: opacity 0.15s ease, transform 0.15s ease;
}

.cancel {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(141, 219, 230, 0.2);
  color: #dffaff;
}

.cancel:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.08);
}

.continue {
  background: #c94b5b;
  border: none;
  color: #fff;
}

.continue:hover:not(:disabled) {
  transform: translateY(-1px);
}

.cancel:disabled,
.continue:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
}

@keyframes fadeIn {
  from { opacity: 0 }
  to   { opacity: 1 }
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
