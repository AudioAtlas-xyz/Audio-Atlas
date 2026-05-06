<script setup lang="ts">
import { computed } from 'vue'
import type { Genre } from '~/types/genre'

const props = defineProps<{
  similarGenres: Genre[]
  parentGenres: Genre[]
  subGenres: Genre[]
}>()

const sections = computed(() => [
  { title: 'Parent Genres', items: props.parentGenres },
  { title: 'Sub Genres', items: props.subGenres },
  { title: 'Similar Genres', items: props.similarGenres }
])
</script>

<template>
  <div :class="$style.relatedgenres">
    <div
      v-for="section in sections"
      :key="section.title"
      :class="$style.card"
    >
      <!-- HEADER -->
      <div :class="$style.header">
        <span :class="$style.title">
          {{ section.title }}
        </span>
      </div>

      <!-- CONTENT -->
      <div v-if="section.items.length">
        <GenreCard
          v-for="genre in section.items"
          :key="genre.id"
          :genre="genre"
        />
      </div>

      <!-- EMPTY STATE -->
      <div v-else :class="$style.empty">
        No genres available
      </div>
    </div>
  </div>
</template>

<style module>
.relatedgenres {
  width: 100%;
  display: flex;
  gap: 0.5rem;
}

/* CARD */
.card {
  flex: 1;
  border-radius: 10px;
  background: #0d0f1a;
  border: 1px solid #1c2038;
  display: flex;
  flex-direction: column;
}

/* HEADER */
.header {
  border-bottom: 1px solid #1c2038;
  padding: 0.6rem 1rem;

  font-size: 0.65rem;
  color: #8a93b8;
  font-family: 'Space Mono';
  letter-spacing: 0.1em;
  text-transform: uppercase;
}

/* TITLE */
.title {
  display: block;
}

/* EMPTY */
.empty {
  padding: 1rem;
  font-size: 0.8rem;
  color: #6f789b;
}
</style>