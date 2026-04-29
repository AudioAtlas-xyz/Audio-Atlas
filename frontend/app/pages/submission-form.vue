<script setup lang="ts">
import type { Country } from '~/types/country'
import type {Genre} from "~/types/genre";
 //npm install @johmun/vue-tags-input
import Stepper from '../components/submission/Stepper.vue';
import SubmissionHeader from "~/components/submission/SubmissionHeader.vue";
const step = ref(1);

const { data: countriesData } = await useFetch<Country[]>('/api/countries/all')
const { data: genresData } = await useFetch<Genre[]>('/api/genres')

const state = reactive({
  countries: []
})

const genrestate = reactive({
  genresEvolvedfrom: [],
  genresRiseTo: [],
  simiargenres: []
})

const genreNames = computed(() =>
  genresData.value?.map(c => c.name) ?? []
)

function nextStep() {
  currentStep.value++
}

function prevStep() {
  currentStep.value--
}

const countryNames = computed(() =>
  countriesData.value?.map(c => c.name) ?? []
)

//refs (reactive)
const currentStep = ref(1)
const sensitive = ref(false)

const submissionData = reactive ({
  // Step 1 fields:
  genreName: '',
  aliases: [], //may need to be changed to GenreAlias[]
  origin: [], //names right now and not country objects
  //Step 2 fields:

})

</script>

<template>

  <h1> Temporary text just to see if the data gets submitted </h1>
  <p> Current Genre Name: {{submissionData.genreName}}</p>
  <p> Current Aliases given: {{submissionData.aliases}}</p>
  <p> Current Origin(s): {{submissionData.origin}}</p>

  <SubmissionHeader/>
  <div v-if="currentStep === 1">
  <Stepper :current-state="currentStep" />
    <div :class="$style.genreidentityformcard">
      <UContainer style="padding: 3rem;">
      <div :class="$style.identity">Identity</div>
      <h1>Name this genre and tell us where it comes from.</h1>

        <div :class="$style.formFields">
          <UFormField label="GENRE NAME" required>
            <UInput v-model="submissionData.genreName" placeholder="e.g Afrobeats" class="w-full"/>
            <h1> Name of the genre. </h1>
          </UFormField>

          <UFormField label="ALIASES" hint="(Optional)">
            <UInputTags v-model="submissionData.aliases" placeholder="Add an alias and press enter" class="w-full"/>
            <h1> Alternative names, transliterations or regional names. </h1>
          </UFormField>


          <UFormField label="COUNTRY / COUNTRIES OF ORIGIN" field="name" required>
            <SubmissionSelectMenu
              v-model="submissionData.origin"
              :itemList ="countryNames"
              class="w-full"/>
            <h1> Select all countries where this genre originated - not just where it became popular. </h1>
          </UFormField>
        </div>
        <div :class="$style.buttonAlone">
          <UButton @click="nextStep" style="background-color: #3DE8C8">
            Next: About
            <UIcon name="i-heroicons-arrow-right-20-solid" />
          </UButton>
        </div>

      </UContainer>
    </div>
  </div>

  <div v-if="currentStep === 2">
    <Stepper :current-state="currentStep" />
    <div :class="$style.genreidentityformcard">
      <UContainer style="padding: 3rem;">
        <div :class="$style.identity">About</div>
        <h1>Describe the genre's sound, origins, and cultural significance. </h1>

        <div :class="$style.formFields">
          <UFormField label="DESCRIPTION" required>
            <UTextarea placeholder="Describe the genre: it's sound, cultural context, history, and it's significance. " class="w-full" />
            <h1>Min. 100 characters.</h1>
          </UFormField>


          <UFormField label="INSTRUMENTS" hint="(Optional)">
            <SubmissionSelectMenu
              v-model="state.countries"
              :itemList ="countryNames"
              class="w-full"/>
            <h1>Traditional and modern instruments associated with this genre</h1>
          </UFormField>

          <UFormField label="EXAMPLE PLAYLIST" hint="(Optional)">
            <UInput placeholder="e.g., https://open.spotify.com/playlist/..." class="w-full"></UInput>
            <h1>Link to a representative link.</h1>
          </UFormField>

          <UFormField>
          <UCheckbox v-model="sensitive" icon="ic:round-music-note" label="This genre may involve sacred or ceremonial traditions" :ui="{base: 'rounded-full', indicator: 'rounded-full'}"/>
            <h1> Check this if the genre has cultural or religious significance that should be noted for respectful representation. </h1>
          </UFormField>

          <UFormField v-if="sensitive" label="CULTURAL SENSITIVITY DESCRIPTION" field="name" required>
            <UTextarea placeholder="What makes this genre culturally sensitive?" class="w-full"/>
            <h1> Describe how the genre may be culturally sensitive, sacred, or ceremonial. </h1>
          </UFormField>
        </div>
        <div :class="$style.buttonRow" style="color: #3de8c8">
          <UButton @click="prevStep" style="background-color: #8899FF">
            <UIcon name="i-heroicons-arrow-left-20-solid" />
            Back
          </UButton>

          <UButton @click="nextStep" style="background-color: #3DE8C8">
            Next: Connections
            <UIcon name="i-heroicons-arrow-right-20-solid" />
          </UButton>
        </div>

      </UContainer>
    </div>
  </div>

  <div v-if="currentStep === 3">
    <Stepper :current-state="currentStep" />
    <div :class="$style.genreidentityformcard">
      <UForm ref="form" :state="state" />
      <UContainer style="padding: 3rem;">
        <div :class="$style.identity">Connections</div>
        <h1>Link this genre to others and add your sources. All fields are optional. </h1>

        <div :class="$style.formFields">
        <UFormField label="EVOLVED FROM" field="name">
          <SubmissionSelectMenu
            v-model="genrestate.genresEvolvedfrom"
            :itemList="genreNames"
            class="w-full"/>
          <h1>Genres this one grew out of or was heavily influenced by.</h1>
        </UFormField>

        <UFormField label="GAVE RISE TO" field="name">
          <SubmissionSelectMenu
            v-model="genrestate.genresRiseTo"
            :itemList ="genreNames"
            class="w-full"/>
          <h1>Genres that branched off or emerged from this one.</h1>
        </UFormField>

        <UFormField label="SIMILAR TO" field="name">
          <SubmissionSelectMenu
            v-model="genrestate.simiargenres"
            :itemList ="genreNames"
            class="w-full"/>
          <h1>Genres with a similar sound, feel, or cultural context.</h1>
        </UFormField>

        <UFormField label="SOURCES">
          <UInput placeholder="https://..." style="color: #7a84a8" class="w-full"/>
          <h1>Wikipedia, academic papers, news articles, documentaries - anything that supports your submission. </h1>
        </UFormField>

        <div :class="$style.contributorprofilelink">
          <div :class="$style.addanothersourcelink">+ Add another source</div>
        </div>
        </div>

        <div :class="$style.buttonRow">
          <UButton @click="prevStep" style="background-color: #8899FF">
            <UIcon name="i-heroicons-arrow-left-20-solid" />
            Back
          </UButton>

          <UButton @click="nextStep" style="background-color: #3DE8C8">
            Submit for review
          </UButton>
        </div>
      </UContainer>
    </div>

  </div>

</template>

<style module>
.genreidentityformcard {
  width: 50%;
  height: 37.5rem;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  overflow: auto;
  text-align: left;
  font-size: 0.75rem;
  color: #7a84a8;
  font-family: Space_Grotesk;
  margin: 2rem auto 0 auto;
}
.identity {
  width: 4.813rem;
  position: relative;
  font-size: 1.25rem;
  font-weight: 500;
  font-family: 'Space Grotesk';
  color: #e4e8f5;
  text-align: left;
  display: inline-block;
}

h1 {
  color: #7a84a8;
}

.formFields {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;  /* Adjust this value for more/less space */
  margin-top: 1.5rem;
}

.buttonRow {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 7rem;
}

.buttonAlone {
  display: flex;
  justify-content: right;
  align-items: center;
  margin-top: 7rem;
}

</style>
