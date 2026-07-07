<script setup lang="ts">
import { ref, reactive, computed, useTemplateRef } from 'vue'
import { useAsyncData, useApi } from '#imports'
import type {Genre} from "~/types/genre";
import Stepper from '../components/submission/Stepper.vue';
import SubmissionHeader from "~/components/submission/SubmissionHeader.vue";
const step = ref(3);
const { api } = useApi()

const { data } = await useAsyncData<Genre[]>('submission-form3-genres', () => api('/genres/all'))

const state = reactive({
  genresEvolvedfrom: [],
  genresRiseTo: [],
  simiargenres: []
})

const genreNames = computed(() =>
  data.value?.map(c => ({
    label: c.name,
    value: c.name
  })) ?? []
)

const form = useTemplateRef('form' )
</script>

<template>
  <SubmissionHeader/>
  <Stepper :current-state="step" />
  <div :class="$style.genreconnectionsformcard">
    <UForm ref="form" :state="state" />
    <UContainer style="padding: 3rem;">
    <div :class="$style.connections">Connections</div>
    <h1>Link this genre to others and add your sources. All fields are optional. </h1>

     <UFormField label="EVOLVED FROM" field="name">
        <SubmissionSelectMenu
          v-model="state.genresEvolvedfrom"
          :itemList ="genreNames"
          class="w-full"/>
      </UFormField>
      <div :class="$style.extraText">Genres this one grew out of or was heavily influenced by.</div>

      <UFormField label="GAVE RISE TO" field="name">
        <SubmissionSelectMenu
          v-model="state.genresRiseTo"
          :itemList ="genreNames"
          class="w-full"/>
      </UFormField>
      <div :class="$style.extraText">Genres that branched off or emerged from this one.</div>

      <UFormField label="SIMILAR TO" field="name">
        <SubmissionSelectMenu
          v-model="state.simiargenres"
          :itemList ="genreNames"
          class="w-full"/>
      </UFormField>
      <div :class="$style.extraText">Genres with a similar sound, feel, or cultural context.</div>

      <UFormField label="SOURCES">
        <UInput placeholder="https://..." style="color: #7a84a8" class="w-full"/>
      </UFormField>
      <div :class="$style.extraText">Wikipedia, academic papers, news articles, documentaries - anything that supports your submission. </div>

      <div :class="$style.contributorprofilelink">
        <div :class="$style.addanothersourcelink">+ Add another source</div>
      </div>

    </UContainer>
  </div>
</template>

<style module>
.genreconnectionsformcard {
  width: 50%;
  height: 37.5rem;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  overflow: hidden;
  text-align: left;
  font-size: 0.75rem;
  color: #7a84a8;
  font-family: Space_Grotesk;
  margin: 2rem auto 0 auto;
}
h1 {
  color: #7a84a8;
}
.connections {
  width: 4.813rem;
  position: relative;
  font-size: 1.25rem;
  font-weight: 500;
  font-family: 'Space Grotesk';
  color: #e4e8f5;
  text-align: left;
  display: inline-block;
}

UFormField{
  width: 100%;
}
UInput{
  width: 100%;
}
.extraText {
width: 616px;
height: 16px;
position: relative;
font-size: 12px;
line-height: 16px;
font-family: 'Space Grotesk';
color: #252c48;
text-align: left;
display: inline-block;
}

.contributorprofilelink {
position: relative;
width: 100%;
display: flex;
align-items: center;
justify-content: flex-start;
text-align: left;
font-size: 10.1px;
color: #3de8c8;
font-family: 'Space Mono';
}

.addanothersourcelink {
position: relative;
font-size: 10.1px;
letter-spacing: 0.61px;
line-height: 16.13px;
font-family: 'Space Mono';
color: #3de8c8;
text-align: left;
}
</style>
