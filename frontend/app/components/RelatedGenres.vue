<script setup lang="ts">
import type { Genre } from '~/types/genre.ts'
import { GenreCard } from "#components";

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
      <div :class="$style.divsideCardHead2">
        <div :class="$style.span3">
          <div :class="$style.cardTitle">SIMILAR GENRES</div>
        </div>
        <div :class="$style.span4" />
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
  display: flex;
  align-items: center;
  padding: 9.6px 16px;
  gap: 126.5px;
  font-size: 9px;
  color: #373d5a;
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
.prosecard {
  width: 192px;
  height: 78px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 24px;
}
.titleAndDescription {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 4px;
  flex-shrink: 0;
}
.title {
  align-self: stretch;
  position: relative;
  line-height: 16px;
  font-weight: 600;
}
.loremIpsumDolor {
  align-self: stretch;
  position: relative;
  line-height: 20px;
  font-family: 'Space Grotesk';
  color: #252c48;
}
.divsideCardHead2 {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 9.6px 16px;
  gap: 20px;
  font-size: 9px;
  color: #373d5a;
  font-family: 'Space Mono';
}
.span3 {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.span4 {
  height: 13px;
  width: 0px;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

</style>
