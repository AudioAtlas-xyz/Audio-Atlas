<script setup lang="ts">
import type { Genre } from '~/types/genre'

const { similarGenres, parentGenres, subGenres } = defineProps<{
  similarGenres: Genre[]
  parentGenres: Genre[]
  subGenres: Genre[]
}>()


</script>

<template>
  <div :class="$style.relatedgenres">
    <div :class="$style.parentgenrecard">
      <div :class="$style.parentgenre">
        <div :class="$style.span">
          <div :class="$style.cardTitle">PARENT GENRES</div>
        </div>
        <div :class="$style.a" />
      </div>

      <GenreCard
        v-for="genre in parentGenres"
        :key="genre.id"
        :genre="genre"
      />


    </div>
    <div :class="$style.parentgenrecard">
      <div :class="$style.parentgenre">
        <div :class="$style.span">
          <div :class="$style.cardTitle">SUB GENRES</div>
        </div>
        <div :class="$style.a" />
      </div>

      <GenreCard
        v-for="genre in subGenres"
        :key="genre.id"
        :genre="genre"
      />


    </div>
    <div :class="$style.parentgenrecard">
      <div :class="$style.parentgenre">
        <div :class="$style.span">
          <div :class="$style.cardTitle">SIMILAR GENRES</div>
        </div>
        <div :class="$style.a" />
      </div>

      <GenreCard
        v-for="genre in similarGenres"
        :key="genre.id"
        :genre="genre"
      />


    </div>
  </div>
</template>
<style  module>.relatedgenres {
  width: 100%;
  position: relative;
  display: flex;
  align-items: flex-start;
  gap: 6px;
  text-align: left;
  font-size: 12px;
  color: #e4e8f5;
  font-family: 'DM Sans';
}
.parentgenrecard {
  flex: 1;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.parentgenre {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  padding: 0.6rem 1rem;
  display: flex;

  font-size: 0.7rem;
  color: #8a93b8;
  font-family: 'Space Mono';
}
.span {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  flex-shrink: 0;
}
.cardTitle {
  position: relative;
  letter-spacing: 1.25px;
  text-transform: uppercase;
}
.a {
  height: 14px;
  width: 64px;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0px 0px 1px;
  box-sizing: border-box;
  flex-shrink: 0;
}

</style>
