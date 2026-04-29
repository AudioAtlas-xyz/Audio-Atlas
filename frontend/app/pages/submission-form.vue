<script setup lang="ts">
import type { Country } from '~/types/country'
import type {Genre} from "~/types/genre";
import * as z from 'zod'
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

const step1Schema = z.object({
  genreName: z.string().min(1,'Genre name is required'),
  aliases: z.array(z.string()).optional(),
  origin: z.array(z.string()).min(1, 'At least one country is required'),
})

const step2Schema = z.object({
  description: z.string().min(100,'Description is required (min. 100 characters)'),
  instruments: z.array(z.string()).optional(),
  playlist: z.string().optional(),
  sensitive: z.boolean(),
  sensitiveDescription: z.string().optional(),
}).superRefine((data, ctx) => { //sensitiveDescription is required only when sensitive=true
  if (data.sensitive && !data.sensitiveDescription?.trim()){
    ctx.addIssue({
      code: "custom",
      path: ['sensitiveDescription'],
      message: 'Cultural sensitivity description is required',
    })
  }
})

const step3Schema = z.object({
  evolvedFrom: z.array(z.string()).optional(),
  gaveRiseTo: z.array(z.string()).optional(),
  similarTo: z.array(z.string()).optional(),
  sources: z.string().optional(),
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
  description: '',
  instruments: [],
  playlist: '',
  sensitive: false,
  sensitiveDescription: '',
  //step 3 fields:
  evolvedFrom: [],
  gaveRiseTo: [],
  similarTo: [],
  sources: '', //should be a list at some point
})

</script>

<template>

  <h1> Temporary text just to see if the data gets submitted </h1>
  <p> Current Genre Name: {{submissionData.genreName}}</p>
  <p> Current Aliases given: {{submissionData.aliases}}</p>
  <p> Current Origin(s): {{submissionData.origin}}</p>

  <div v-if="currentStep === 1">
    <SubmissionHeader/>
  <Stepper :current-state="currentStep" />
    <div :class="$style.genreidentityformcard">
      <UContainer style="padding: 3rem;">
      <div :class="$style.identity">Identity</div>
      <h1>Name this genre and tell us where it comes from.</h1>
      <UForm
        :state="submissionData"
        :schema="step1Schema"
        @submit="nextStep"
      >
        <div :class="$style.formFields">
          <UFormField label="GENRE NAME"  name="genreName" required>
            <UInput v-model="submissionData.genreName" placeholder="e.g Afrobeats" class="w-full"/>
            <h1> Name of the genre. </h1>
          </UFormField>

          <UFormField label="ALIASES" name="aliases" hint="(Optional)">
            <UInputTags v-model="submissionData.aliases" placeholder="Add an alias and press enter" class="w-full"/>
            <h1> Alternative names, transliterations or regional names. </h1>
          </UFormField>


          <UFormField label="COUNTRY / COUNTRIES OF ORIGIN" field="name" name="origin" required>
            <SubmissionSelectMenu
              v-model="submissionData.origin"
              :itemList ="countryNames"
              class="w-full"/>
            <h1> Select all countries where this genre originated - not just where it became popular. </h1>
          </UFormField>
        </div>
        <div :class="$style.buttonAlone">
          <UButton type="submit" style="background-color: #3DE8C8">
            Next: About
            <UIcon name="i-heroicons-arrow-right-20-solid" />
          </UButton>
        </div>
      </UForm>


      </UContainer>
    </div>
  </div>

  <div v-if="currentStep === 2">
    <SubmissionHeader/>
    <Stepper :current-state="currentStep" />
    <div :class="$style.genreidentityformcard">
      <UContainer style="padding: 3rem;">
        <div :class="$style.identity">About</div>
        <h1>Describe the genre's sound, origins, and cultural significance. </h1>
        <UForm
          :state="submissionData"
          :schema="step2Schema"
          @submit="nextStep"
        >
          <div :class="$style.formFields">
            <UFormField label="DESCRIPTION" name="description" required>
              <UTextarea v-model="submissionData.description" placeholder="Describe the genre: it's sound, cultural context, history, and it's significance. " class="w-full" />
              <h1>Min. 100 characters.</h1>
            </UFormField>


            <UFormField label="INSTRUMENTS" name="instruments" hint="(Optional)">
              <SubmissionSelectMenu
                v-model="state.countries"
                :itemList ="countryNames"
                class="w-full"/>
              <h1>Traditional and modern instruments associated with this genre</h1>
            </UFormField>

            <UFormField label="EXAMPLE PLAYLIST" name="playlist" hint="(Optional)">
              <UInput v-model="submissionData.playlist" placeholder="e.g., https://open.spotify.com/playlist/..." class="w-full"></UInput>
              <h1>Link to a representative link.</h1>
            </UFormField>

            <UFormField>
            <UCheckbox v-model="submissionData.sensitive" icon="ic:round-music-note" label="This genre may involve sacred or ceremonial traditions" name="sensitive" :ui="{base: 'rounded-full', indicator: 'rounded-full'}"/>
              <h1> Check this if the genre has cultural or religious significance that should be noted for respectful representation. </h1>
            </UFormField>

            <UFormField v-if="submissionData.sensitive" label="CULTURAL SENSITIVITY DESCRIPTION" name="sensitiveDescription" field="name" required>
              <UTextarea v-model="submissionData.sensitiveDescription" placeholder="What makes this genre culturally sensitive?" class="w-full"/>
              <h1> Describe how the genre may be culturally sensitive, sacred, or ceremonial. </h1>
            </UFormField>
          </div>
          <div :class="$style.buttonRow" style="color: #3de8c8">
            <UButton @click="prevStep" style="background-color: #8899FF">
              <UIcon name="i-heroicons-arrow-left-20-solid" />
              Back
            </UButton>

            <UButton type="submit" style="background-color: #3DE8C8">
              Next: Connections
              <UIcon name="i-heroicons-arrow-right-20-solid" />
            </UButton>
          </div>
        </UForm>
      </UContainer>
    </div>
  </div>

  <div v-if="currentStep === 3">
    <SubmissionHeader/>
    <Stepper :current-state="currentStep" />
    <div :class="$style.genreidentityformcard">
      <UForm ref="form" :state="state" />
      <UContainer style="padding: 3rem;">
        <div :class="$style.identity">Connections</div>
        <h1>Link this genre to others and add your sources. All fields are optional. </h1>

        <UForm
        :state="submissionData"
        :schema="step3Schema"
        @sumbit="nextStep"
        >
          <div :class="$style.formFields">
            <UFormField label="EVOLVED FROM" field="name" name="evolvedFrom">
              <!--- changed genrestate to submissionData --->
              <SubmissionSelectMenu
                v-model="submissionData.evolvedFrom"
                :itemList="genreNames"
                class="w-full"/>
              <h1>Genres this one grew out of or was heavily influenced by.</h1>
            </UFormField>

            <UFormField label="GAVE RISE TO" field="name" name="gaveRiseTo">
              <!--- changed genrestate to submissionData --->
              <SubmissionSelectMenu
                v-model="submissionData.gaveRiseTo"
                :itemList ="genreNames"
                class="w-full"/>
              <h1>Genres that branched off or emerged from this one.</h1>
            </UFormField>

            <UFormField label="SIMILAR TO" field="name" name="similarTo">
              <!--- changed genrestate to submissionData --->
              <SubmissionSelectMenu
                v-model="submissionData.similarTo"
                :itemList ="genreNames"
                class="w-full"/>
              <h1>Genres with a similar sound, feel, or cultural context.</h1>
            </UFormField>

            <UFormField label="SOURCES" name="sources">
              <UInput v-model="submissionData.sources" placeholder="https://..." style="color: #7a84a8" class="w-full"/>
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
        </UForm>

      </UContainer>
    </div>
  </div>

  <div v-if="currentStep === 4">
    <div :class="$style.genreidentityformcard">
      <UForm ref="form" :state="state" />
      <UContainer style="padding: 3rem;">
        <div :class="$style.identity">Review & submit</div>
        <h1>Check everything looks right before submitting for curator review. </h1>

        <div: class = "$style.formFields">
          <div :class="$style.reviewStepWrapper">
            <div :class="$style.reviewSubject">IDENTITY</div>
            <div :class="$style.subjectName">Genre name</div>
            <div :class="$style.genreNames">Afrobeats</div>
            <div :class="$style.subjectName">Aliases</div>
            <div :class="$style.subjectName">Countries of origin</div>
            </div>

          <USeparator orientation="horizontal" class="my-8" />

          <div :class="$style.reviewStepWrapper">
            <div :class="$style.reviewSubject">ABOUT</div>
            <div :class="$style.subjectName">Description</div>
            <div :class="$style.subjectName">Instruments</div>
            <div :class="$style.subjectName">Playlist</div>
          </div>

          <USeparator orientation="horizontal" class="my-8" />

          <div :class="$style.reviewStepWrapper">
            <div :class="$style.reviewSubject">CONNECTIONS</div>
            <div :class="$style.subjectName">Evolved from</div>
            <div :class="$style.subjectName"> Gave rise to </div>
            <div :class="$style.subjectName">Similar to </div>
            <div :class="$style.subjectName">Sources </div>

          </div>
        </div:>

        <USeparator orientation="horizontal" class="my-8" />

        <div :class="$style.licensenotice">
          <div :class="$style.rectangle" />
          <div :class="$style.bySubmittingYour">By submitting, your contribution will be licensed under CC BY-NC-SA 4.0 and attributed to your Audio Atlas account. Submissions are reviewed by curators before appearing on the map.</div>
        </div>

         <USeparator orientation="horizontal" class="my-8" />

         <div :class="$style.buttonRow2" style="color: #3de8c8">
            <UButton @click="prevStep" style="background-color: #8899FF">
              <UIcon name="i-heroicons-arrow-left-20-solid" />
              Back
            </UButton>

            <UButton type="submit" style="background-color: #3DE8C8">
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
  width: auto;
  position: relative;
  font-size: 1.25rem;
  font-weight: 500;
  font-family: 'Space Grotesk';
  color: #e4e8f5;
  text-align: left;
  display: inline-block;
}

.reviewSubject {
  width: 57px;
  height: 13px;
  position: relative;
  font-size: 9px;
  letter-spacing: 0.2em;
  font-family: 'Space Mono';
  color: #373d5a;
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

.buttonRow2 {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 2rem;

}

.buttonAlone {
  display: flex;
  justify-content: right;
  align-items: center;
  margin-top: 7rem;
}
.reviewStepWrapper {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 16px;
}

.reviewSubject {
  font-size: 9px;
  letter-spacing: 0.2em;
  font-family: 'Space Mono', monospace;
  color: #373d5a;
  text-transform: uppercase;
}

.subjectName {
  width: auto;
  height: 14px;
  font-size: 11px;
  font-family: 'Space Grotesk';
  color: #373d5a;
}

.genreNames{
  width: 616px;
  height: 20px;
  position: relative;
  font-size: 13px;
  line-height: 175%;
  font-weight: 300;
  font-family: 'Space Grotesk';
  color: #7a84a8;
  text-align: left;
  display: inline-block;

}

.licensenotice {
width: 100%;
height: 68px;
position: relative;
border-radius: 3px;
background-color: #131624;
border: 1px solid #1c2038;
box-sizing: border-box;
overflow: hidden;
text-align: left;
font-size: 11px;
color: #7a84a8;
font-family: 'Space Grotesk';
}

.rectangle {
position: absolute;
top: 0px;
left: 0px;
background-color: #3de8c8;
width: 3px;
height: 68px;
}
.bySubmittingYour {
position: absolute;
top: 12px;
left: 16px;
line-height: 178%;
font-weight: 300;
display: inline-block;
width: 584px;
height: 20px;
}

</style>
