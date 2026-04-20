<script setup>
const emit = defineEmits(['close'])
</script>

<template>
  <div class="overlay" @click.self="emit('close')">
    <div class="modal">
      <button class="close" @click="emit('close')">×</button>

      <div class="checkmark">✓</div>

      <h2>Account created successfully!</h2>
      <p>Discover music genres now!</p>
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
  padding: 2rem;
  border-radius: 18px;
  background: #050816;
  border: 1px solid rgba(120, 150, 255, 0.18);
  text-align: center;
  color: #eef2ff;
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
}

h2 {
  margin: 0 0 0.5rem;
  font-size: 1.4rem;
}

p {
  margin: 0;
  color: #6b7395;
}
</style>