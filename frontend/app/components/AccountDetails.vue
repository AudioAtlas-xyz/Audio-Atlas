<script setup lang="ts">
import { ref, watch } from 'vue'
import GlassPanel from '@/components/GlassPanel.vue'
import GlassButton from '@/components/GlassButton.vue'
import { useAuth } from '@/composables/useAuth'

const props = defineProps<{
  open: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()

const { user, logout } = useAuth()

const username = ref(user.value?.username || '')

watch(() => props.open, (val) => {
  if (val) {
    username.value = user.value?.username || ''
  }
})

function close() {
  emit('close')
}

function save() {
  // TODO: hook to API
  console.log('save username:', username.value)
}
</script>

<template>
  <transition name="fade">
    <div v-if="open" class="overlay" @click.self="close">
      <GlassPanel class="modal">
        <h2 class="title">Account details</h2>

        <!-- USERNAME -->
        <div class="section">
          <label>USERNAME</label>
          <input v-model="username" class="input" />
        </div>

        <!-- CONNECTED ACCOUNTS -->
        <div class="section">
          <p class="section-title">Connected accounts</p>

          <div class="account-row">
            <span>GitHub</span>
            <span class="muted">@{{ user?.username }}</span>
            <GlassButton size="sm">Connected</GlassButton>
          </div>

          <div class="account-row">
            <span>Google</span>
            <span class="muted">{{ user?.email }}</span>
            <GlassButton size="sm">Connected</GlassButton>
          </div>
        </div>

        <!-- ACTIONS -->
        <div class="actions">
          <GlassButton @click="save">
            Save changes
          </GlassButton>

          <GlassButton variant="danger" @click="logout">
            Delete Account
          </GlassButton>
        </div>
      </GlassPanel>
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
  align-items: flex-start;

  padding-top: 6rem;

  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(6px);
}

.modal {
  width: 100%;
  max-width: 720px;

  padding: 1.5rem 2rem;

  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.title {
  font-size: 1.4rem;
  font-weight: 600;
  color: #e6faff;
}

.section {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.section-title {
  color: #9fdbe6;
  font-size: 0.9rem;
}

label {
  font-size: 0.75rem;
  color: #7fbac6;
  letter-spacing: 0.08em;
}

.input {
  height: 2.5rem;
  border-radius: 10px;
  padding: 0 0.75rem;

  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(141, 219, 230, 0.2);

  color: white;
}

.account-row {
  display: grid;
  grid-template-columns: 1fr auto auto;
  align-items: center;

  padding: 0.6rem 0.75rem;
  border-radius: 10px;

  background: rgba(255, 255, 255, 0.03);
}

.muted {
  color: #7aaeb8;
  font-size: 0.85rem;
}

.actions {
  display: flex;
  justify-content: space-between;
  margin-top: 0.5rem;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>