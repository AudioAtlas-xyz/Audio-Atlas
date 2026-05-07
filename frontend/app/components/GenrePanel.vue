<script setup lang="ts">
const { relatedCount, contributorCount, instrumentCount } = defineProps<{
  relatedCount: string
  contributorCount: string
  instrumentCount: string
}>()
</script>

<template>
  <div :class="$style.genrepanel">
    <div :class="$style.divheroLeft">
      <div :class="$style.genrestats">
        <div :class="$style.prosecard">
          <div :class="$style.titleAndDescription">
            <b :class="$style.title">{{relatedCount}}</b>
            <div :class="$style.loremIpsumDolor">Related Genres</div>
          </div>
        </div>
        <div :class="$style.prosecard">
          <div :class="$style.titleAndDescription">
            <b :class="$style.title">{{contributorCount}}</b>
            <div :class="$style.loremIpsumDolor">Contributors</div>
          </div>
        </div>
        <div :class="$style.prosecard">
          <div :class="$style.titleAndDescription">
            <b :class="$style.title">{{instrumentCount}}</b>
            <div :class="$style.loremIpsumDolor">Instruments</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style module>.divheroLeft {
  width: 100%;
  height: 5.138rem;
  position: relative;
  text-align: left;
  font-size: 2.975rem;
  color: #e4e8f5;
  font-family: 'Space Grotesk';
}

.genrestats {
  position: absolute;
  width: fit-content;
  right: 27.3rem;
  left: 0.375rem;
  border-radius: 4px;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  height: 5rem;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  font-size: 1.875rem;
  font-family: DM_Sans;
}
.prosecard {
  height: 6.25rem;
  width: 12rem;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 1.5rem;
  flex-shrink: 0;
}
.titleAndDescription {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.25rem;
  flex-shrink: 0;
}
.title {
  align-self: stretch;
  position: relative;
  line-height: 2.25rem;
}
.loremIpsumDolor {
  align-self: stretch;
  position: relative;
  font-size: 0.75rem;
  line-height: 1.25rem;
  font-family: Space_Grotesk;
  color: #7a84a8;
}
</style>
