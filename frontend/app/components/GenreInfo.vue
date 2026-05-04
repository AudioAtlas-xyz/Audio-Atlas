<script setup lang="ts">
import type { Genre } from "~/types/genre"

const props = defineProps<{
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

      <div v-if="props.isSensitive" :class="$style.culturalsensitivitynotice">
        <div :class="$style.spansensIconmargin">
          <div :class="$style.symbol">◎</div>
        </div>

        <div :class="$style.divsensBody">
          <div :class="$style.divsensTitle">
            <div :class="$style.noticeTitle">
              Cultural SENSITIVITY notice
            </div>
          </div>

          <div :class="$style.divsensText">
            <div :class="$style.noticeText">
              {{ props.sensitivityDescription || 'This genre includes culturally sensitive material.' }}
            </div>
          </div>
        </div>
      </div>

      <div :class="$style.genreoverview">
        <div :class="$style.overview">
          <div :class="$style.sectionTitle">Overview</div>
        </div>

        <div :class="$style.divsensTitle">
          <div :class="$style.sectionText">
            {{ props.pageDescription }}
          </div>
        </div>
      </div>

      <div :class="$style.sectiondivider" />

      <RelatedGenres
        :parent-genres="props.parentGenres"
        :similar-genres="props.similarGenres"
        :sub-genres="props.subGenres"
      />

    </div>
  </div>
</template>

<style module>
.genreinfo {
  width: 100%;
  min-height: 100%;
  position: relative;
  box-sizing: border-box;

  padding: 2rem 2.5rem 3rem;

  text-align: left;
  font-size: 0.875rem;
  color: #e4e8f5;
  font-family: 'Space Grotesk';
}

/*
 * NOTE: the previous version of `.genreinfo` declared
 * `display: grid; grid-template-columns: 1fr 320px;`
 * with `.divcontent` placed in column 1 — leaving column 2
 * permanently empty. The 320px gutter wasted the right half of
 * the page allocated to the overview text. The outer page grid
 * in `pages/genres.vue` already handles the Contributors/Sources
 * sidebar, so we render GenreInfo as a full-width block here.
 */
.divcontent {
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: stretch;
  gap: 2rem;
}

.culturalsensitivitynotice {
  width: 100%;
  background: linear-gradient(135deg, #0a1a3a 0%, #1a3a6e 100%);
  border-left: 3px solid #3de8c8;

  display: flex;
  align-items: flex-start;
  padding: 1rem 1.25rem;
  gap: 0.75rem;
}

.spansensIconmargin {
  padding-top: 0.062rem;
}

.symbol {
  line-height: 0.875rem;
  font-weight: 300;
}

.divsensBody {
  display: flex;
  flex-direction: column;
  gap: 0.2rem;

  width: 100%;
  max-width: 32rem;

  font-size: 0.6rem;
  color: #60b8ff;
  font-family: 'Space Mono';
}

.noticeTitle {
  letter-spacing: 1.15px;
  text-transform: uppercase;
}

.divsensText {
  font-size: 0.8rem;
  color: #507090;
  font-family: 'Space Grotesk';
}

.noticeText {
  line-height: 1.32rem;
  font-weight: 300;
}

/* OVERVIEW */
.genreoverview {
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 0.7rem;

  font-size: 0.863rem;
  color: #7a84a8;
}

.overview {
  border-bottom: 1px solid #1c2038;
  padding-bottom: 0.5rem;

  font-size: 0.581rem;
  color: #373d5a;
  font-family: 'Space Mono';
}

.sectionTitle {
  letter-spacing: 1.49px;
  text-transform: uppercase;
}

.sectionText {
  line-height: 1.6rem;
  font-weight: 300;
}

.sectiondivider {
  width: 100%;
  border-top: 1px solid #1c2038;
}
</style>