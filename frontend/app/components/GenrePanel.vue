<script setup lang="ts">
const props = defineProps<{
  relatedCount: string
  contributorCount: string
  instrumentCount: string
}>()
</script>

<template>
  <div :class="$style.genrepanel">
    <div :class="$style.wrapper">

      <div :class="$style.genrestats">
        <div :class="$style.prosecard">
          <b :class="$style.title">{{ props.relatedCount }}</b>
          <span :class="$style.label">Related Genres</span>
        </div>

        <div :class="$style.prosecard">
          <b :class="$style.title">{{ props.contributorCount }}</b>
          <span :class="$style.label">Contributors</span>
        </div>

        <div :class="$style.prosecard">
          <b :class="$style.title">{{ props.instrumentCount }}</b>
          <span :class="$style.label">Instruments</span>
        </div>
      </div>

      <div :class="$style.actions">
        <NuxtLink to="/" :class="$style.button">
          Suggest Edits
        </NuxtLink>
      </div>

    </div>
  </div>
</template>

<style module>
.genrepanel {
  width: 100%;
}

.wrapper {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.genrestats {
  display: flex;
  gap: 1rem;

  border: 1px solid #1c2038;
  border-radius: 10px;

  overflow: hidden;
  background: rgba(255, 255, 255, 0.02);
}

.prosecard {
  flex: 1;

  display: flex;
  flex-direction: column;
  justify-content: center;

  padding: 1.25rem;

  border-right: 1px solid #1c2038;
}

.prosecard:last-child {
  border-right: none;
}

.title {
  font-size: 1.6rem;
  font-weight: 600;
  color: #e4e8f5;
}

.label {
  font-size: 0.75rem;
  color: #6f789b;
  margin-top: 0.2rem;
  letter-spacing: 0.04em;
}

.actions {
  display: flex;
}

.button {
  background: #3de8c8;
  color: #02211c;

  padding: 0.6rem 1.2rem;
  border-radius: 10px;

  font-size: 0.85rem;
  font-weight: 500;

  transition: all 0.15s ease;
}

.button:hover {
  background: #4af7d7;
}
</style>