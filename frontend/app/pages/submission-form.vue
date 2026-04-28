<script setup lang="ts">
import type { Country } from '~/types/country'
import type {Genre} from "~/types/genre";
import Stepper from '../components/submission/Stepper.vue';
const step = ref(1);

const { data } = await useFetch<Country[]>('/api/countries/all')

const state = reactive({
  countries: []
})

function nextStep() {
  currentStep.value++
}

function prevStep() {
  currentStep.value--
}

const countryNames = computed(() =>
  data.value?.map(c => ({
    label: c.name,
    value: c.name
  })) ?? []
)

//refs (reactive)
const currentStep = ref(1)
const sensitive = ref(false)

</script>

<template>
  <div v-if="currentStep === 1">
  <Stepper :current-state="currentStep" />
    <div :class="$style.genreidentityformcard">
      <UContainer style="padding: 3rem;">
      <div :class="$style.identity">Identity</div>
      <h1>Name this genre and tell us where it comes from.</h1>

        <div :class="$style.formFields">
          <UFormField label="GENRE NAME" required>
            <UInput placeholder="e.g Afrobeats" class="w-full"/>
            <h1> Name of the genre. </h1>
          </UFormField>


          <UFormField label="ALIASES" hint="(Optional)">
            <UInput placeholder="Add an alias and press enter" class="w-full"></UInput>
            <h1> Alternative names, transliterations or regional names. </h1>
          </UFormField>


          <UFormField label="COUNTRY / COUNTRIES OF ORIGIN" field="name" required>
            <SubmissionSelectMenu
              v-model="state.countries"
              :itemList ="countryNames"
              class="w-full"/>
            <h1> Select all countries where this genre originated - not just where it became popular. </h1>
          </UFormField>
        </div>
        <div :class="$style.buttonRow" style="color: #3de8c8">
          <UButton @click="nextStep">
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
          <UButton @click="prevStep">
            <UIcon name="i-heroicons-arrow-left-20-solid" />
            Back
          </UButton>

          <UButton @click="nextStep">
            Next: Connections
            <UIcon name="i-heroicons-arrow-right-20-solid" />
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

</style>
