<script setup lang="ts">
defineProps<{ genreName: string }>()
defineEmits<{ (e: 'submit-another'): void }>()
</script>

<template>
  <div :class="$style.card">
    <div :class="$style.iconWrap">
      <span :class="$style.check">✓</span>
    </div>

    <p :class="$style.heading">
      Thanks! {{ genreName }} is now in review.
    </p>

    <p :class="$style.body">
      Your submission has been received and will be reviewed by our curatorial
      team. You'll be notified when it's approved or if the team has questions.
    </p>

    <div :class="$style.actions">
      <NuxtLink to="/explore">
        <UButton :class="$style.exploreBtn">
          Continue exploring
        </UButton>
      </NuxtLink>
      <UButton style="background-color: #3DE8C8; color: #02070a; font-weight: 600;" @click="$emit('submit-another')">
        Submit another genre
      </UButton>
    </div>
  </div>
</template>

<style module>
.card {
  width: min(420px, calc(100% - 3rem));
  margin: 8rem auto 4rem;
  padding: 2.5rem 2rem;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.25rem;
  text-align: center;
  font-family: 'Space Grotesk', sans-serif;
}

.iconWrap {
  width: 52px;
  height: 52px;
  border-radius: 50%;
  background-color: rgba(61, 232, 200, 0.08);
  border: 1px solid #3de8c8;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.check {
  font-size: 24px;
  color: #3de8c8;
  line-height: 1;
}

.heading {
  margin: 0;
  font-size: 18px;
  font-weight: 500;
  color: #e4e8f5;
  line-height: 1.4;
}

.body {
  margin: 0;
  font-size: 13px;
  font-weight: 300;
  line-height: 1.75;
  color: #7a84a8;
}

.actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  width: 100%;
}

.exploreBtn {
  width: 100%;
  justify-content: center;
  background: transparent;
  border: 1px solid rgba(141, 219, 230, 0.25);
  color: #8ddbe6;
}

.exploreBtn:hover {
  background: rgba(141, 219, 230, 0.07);
}
</style>
