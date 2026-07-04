<script setup lang="ts">
import type {Genre} from "~/types/genre";

const { isSensitive, sensitivityDescription, pageDescription, similarGenres, parentGenres, subGenres } = defineProps<{
  isSensitive: boolean
  sensitivityDescription?: string
  pageDescription: string
  similarGenres: Genre[]
  parentGenres: Genre[]
  subGenres: Genre[]
}>()
</script>

<template>
  <div :class="$style.genreinfo">
    <div :class="$style.divcontent">
      <div v-if="isSensitive" :class="$style.culturalsensitivitynotice">
        <div :class="$style.spansensIconmargin">
          <div :class="$style.symbol">◎</div>
        </div>
        <div :class="$style.divsensBody">
          <div :class="$style.divsensTitle">
            <div :class="$style.noticeTitle">Cultural SENSITIVITY notice</div>
          </div>
          <div :class="$style.divsensText">
            <div :class="$style.noticeText">{{sensitivityDescription}}
            </div>
          </div>
        </div>
      </div>
      <div :class="$style.genreoverview">
        <div :class="$style.overview">
          <div :class="$style.sectionTitle">Overview</div>
        </div>
        <div :class="$style.divsensTitle">
          <div :class="$style.sectionText">{{pageDescription}}
          </div>
        </div>
      </div>
      <div :class="$style.sectiondivider" />

      <RelatedGenres
      :parent-genres="parentGenres"
      :similar-genres="similarGenres"
      :sub-genres="subGenres"
      />

      </div>
    </div>
</template>
<style  module>
.genreinfo {
  width: 100%;
  position: relative;
  box-sizing: border-box;
  grid-template-columns: 1fr 320px;
  gap: 3rem;
  padding-left: 2.5rem;
  padding-right: 2.5rem;
  padding-top: 2rem;
  padding-bottom: 3rem;
  text-align: left;
  font-size: 0.875rem;
  color: #e4e8f5;
  font-family: 'Space Grotesk';
}

.divcontent {
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0rem 0rem 2.5rem;
  gap: 2rem;
  grid-column: 1;
  grid-row: 1;
  flex-shrink: 0;
}
.culturalsensitivitynotice {
  align-self: stretch;
  background: linear-gradient(135deg, #0a1a3a 0%, #1a3a6e 100%);
  border-left: 3px solid #3de8c8;
  display: flex;
  align-items: flex-start;
  padding: 1rem 1.25rem;
  gap: 0.75rem;
  border-radius: 0px;
}
.spansensIconmargin {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0.062rem 0rem 0rem;
}
.symbol {
  position: relative;
  line-height: 0.875rem;
  font-weight: 300;
}
.divsensBody {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.187rem;
  min-width: 32.727rem;
  font-size: 0.6rem;
  color: #60b8ff;
  font-family: 'Space Mono';
}
.divsensTitle {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.noticeTitle {
  position: relative;
  letter-spacing: 1.15px;
  text-transform: uppercase;
}
.divsensText {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  font-size: 0.8rem;
  color: #507090;
  font-family: 'Space Grotesk';
}
.noticeText {
  position: relative;
  line-height: 1.32rem;
  font-weight: 300;
}
.genreoverview {
  align-self: stretch;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0rem 0rem 0.5rem;
  gap: 0.681rem;
  font-size: 0.863rem;
  color: #FFFFFF;
}
.overview {
  align-self: stretch;
  border-bottom: 1px solid #1c2038;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 0rem 0rem 0.5rem;
  font-size: 0.7rem;
  color: #7a84a8;
  font-family: 'Space Mono';
}
.sectionTitle {
  position: relative;
  letter-spacing: 1.49px;
  text-transform: uppercase;
}
.sectionText {
  position: relative;
  line-height: 1.591rem;
  font-weight: 300;
  font-size: 0.875rem;
}
.sectiondivider {
  align-self: stretch;
  height: 0.063rem;
  position: relative;
  border-top: 1px solid #1c2038;
  box-sizing: border-box;
  overflow: hidden;
  flex-shrink: 0;
  width: 100%;
}

</style>
