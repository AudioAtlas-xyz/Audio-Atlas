<script setup lang="ts">
import type { Country } from '~/types/country'
import type {Genre} from "~/types/genre";
import type { CreateSubmissionRequest } from "~/types/CreateSubmissionRequest"
import * as z from 'zod'
 //npm install @johmun/vue-tags-input
import Stepper from '../components/submission/Stepper.vue';
import SubmissionHeader from "~/components/submission/SubmissionHeader.vue";
import type {Instrument} from "~/types/instrument";
const { api } = useApi();
const step = ref(1);

const { data: countriesData } = await useFetch<Country[]>('/api/countries/all')
const { data: genresData } = await useFetch<Genre[]>('/api/genres')
const { data: instrumentData } = await useFetch<Instrument[]>('/api/instruments')

const state = reactive({
  countries: []
})

const step1Schema = z.object({
  NewGenreName: z.string().min(1,'Genre name is required'),
  Aliases: z.array(z.string()).optional(),
  CountryIds: z.array(z.string()).min(1, 'At least one country is required'),
})

const step2Schema = z.object({
  Description: z.string().min(100,'Description is required (min. 100 characters)'),
  InstrumentIds: z.array(z.string()).optional(),
  PlaylistLink: z.string().optional(),
  IsSensitive: z.boolean(),
  SensitiveDescription: z.string().optional(),
}).superRefine((data, ctx) => { //sensitiveDescription is required only when sensitive=true
  if (data.IsSensitive && !data.SensitiveDescription?.trim()){
    ctx.addIssue({
      code: "custom",
      path: ['sensitiveDescription'],
      message: 'Cultural sensitivity description is required',
    })
  }
})

const step3Schema = z.object({
  PredecessorGenreIds: z.array(z.string()).optional(),
  SubGenreIds: z.array(z.string()).optional(),
  SimilarGenreIds: z.array(z.string()).optional(),
  SourceLinks: z.array(z.string()).optional(),
})

const isSubmitting = ref(false)
const submitError = ref<string | null>(null)
type ValidStep = 1 | 2 | 3 | 4 | 5;

function nextStep() {
  currentStep.value++
}

function prevStep() {
  currentStep.value--
}

function goToStep(step: ValidStep){
  currentStep.value = step;
}
async function submitForm() {
  isSubmitting.value = true
  submitError.value = null

  const payload: CreateSubmissionRequest = {
    NewGenreName: submissionData.NewGenreName,
    Aliases: submissionData.Aliases,
    CountryIds: submissionData.CountryIds,
    Description: submissionData.Description,
    PlaylistLink: submissionData.PlaylistLink,
    IsSensitive: submissionData.IsSensitive,
    SensitiveDescription: submissionData.SensitiveDescription,
    PredecessorGenreIds: submissionData.PredecessorGenreIds,
    SubGenreIds: submissionData.SubGenreIds,
    SimilarGenreIds: submissionData.SimilarGenreIds,
    SourceLinks: submissionData.SourceLinks,
    StartDate: submissionData.StartDate ? submissionData.StartDate : null,
    EndDate: submissionData.EndDate ? submissionData.EndDate : null,
    InstrumentIds: submissionData.InstrumentIds,
  }

  try {
    console.log('Submitting payload:', JSON.stringify(payload, null, 2))
    const response = await api('/submissions', {
      method: 'POST',
      body: payload,
    })
    // handle success – e.g., redirect or show success message
    console.log('Submission successful', response)
    // navigate to a thank you page or reset form
    goToStep(5)
  } catch (err: any) {
    console.error('Submission failed', err)
    console.error('Status:', err.status)
    console.error('Response body:', err.data)
    console.error('Validation errors:', err.data?.errors)
    submitError.value = err.data?.message || 'Failed to submit. Please try again.'
  } finally {
    isSubmitting.value = false
  }
}

const genreNames = computed(() =>
  genresData.value?.map(g => ({ label: g.name, value: g.id })) ?? []
)


function submitNewGenre(){
  //først sikre sig at man starter på en ny submission
  //redirect til step 1
  goToStep(1)

}
const countryNames = computed(() =>
  countriesData.value?.map(c => ({ label: c.name, value: c.id })) ?? []
)

const instrumentNames = computed(() =>
  instrumentData.value?.map(i => ({ label: i.type, value: i.id })) ?? []
)

// we need to be able to map from id to name otherwise on the last page the ids will be displayed instead of the names
// so the user won't know what instruments they inserted unless they go back.
const countryNameMap = computed(() => {
  const map: Record<string, string> = {}
  countriesData.value?.forEach(c => { map[c.id] = c.name })
  return map
})

const genreNameMap = computed(() => {
  const map: Record<string, string> = {}
  genresData.value?.forEach(g => { map[g.id] = g.name })
  return map
})

const instrumentNameMap = computed(() => {
  const map: Record<string, string> = {}
  instrumentData.value?.forEach(i => { map[i.id] = i.type })
  return map
})


//refs (reactive)
const currentStep = ref(1)
const sensitive = ref(false)

//All of the fields have been renamed to match the backend's "CreateSubmissionRequestDTO"
const submissionData = reactive ({
  // Step 1 fields:
  NewGenreName: '',
  Aliases: [] as string[],
  CountryIds: [] as string[], //origin
  //Step 2 fields:
  Description: '',
  InstrumentIds: [] as string[], //doesnt exist in backend DTO
  PlaylistLink: '',
  IsSensitive: false,
  SensitiveDescription: '', //doesnt exist in backend DTO
  //step 3 fields:
  PredecessorGenreIds: [], //evolved from
  SubGenreIds: [], //gave rise to
  SimilarGenreIds: [], //similar genres
  SourceLinks: [] as string[], //should be a list at some point
  // 'step 4' fields:
  StartDate: '',
  EndDate: '',
})

</script>

<template>

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
          <UFormField label="GENRE NAME"  name="NewGenreName" required>
            <UInput v-model="submissionData.NewGenreName" placeholder="e.g Afrobeats" class="w-full"/>
            <h1> Name of the genre. </h1>
          </UFormField>

          <UFormField label="ALIASES" name="Aliases" hint="(Optional)">
            <UInputTags v-model="submissionData.Aliases" placeholder="Add an alias and press enter" class="w-full"/>
            <h1> Alternative names, transliterations or regional names. </h1>
          </UFormField>


          <UFormField label="COUNTRY / COUNTRIES OF ORIGIN" field="name" name="CountryIds" required>
            <SubmissionSelectMenu
              v-model="submissionData.CountryIds"
              :itemList="countryNames"
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
            <UFormField label="DESCRIPTION" name="Description" required>
              <UTextarea v-model="submissionData.Description" placeholder="Describe the genre: it's sound, cultural context, history, and it's significance. " class="w-full" />
              <h1>Min. 100 characters.</h1>
            </UFormField>


            <UFormField label="INSTRUMENTS" name="InstrumentIds" hint="(Optional)">
              <SubmissionSelectMenu
                v-model="submissionData.InstrumentIds"
                :itemList ="instrumentNames"
                class="w-full"/>
              <h1>Traditional and modern instruments associated with this genre</h1>
            </UFormField>

            <UFormField label="EXAMPLE PLAYLIST" name="PlaylistLink" hint="(Optional)">
              <UInput v-model="submissionData.PlaylistLink" placeholder="e.g., https://open.spotify.com/playlist/..." class="w-full"></UInput>
              <h1>Link to a representative link.</h1>
            </UFormField>

            <UFormField>
            <UCheckbox v-model="submissionData.IsSensitive" icon="ic:round-music-note" label="This genre may involve sacred or ceremonial traditions" name="IsSensitive" :ui="{base: 'rounded-full', indicator: 'rounded-full'}"/>
              <h1> Check this if the genre has cultural or religious significance that should be noted for respectful representation. </h1>
            </UFormField>

            <UFormField v-if="submissionData.IsSensitive" label="CULTURAL SENSITIVITY DESCRIPTION" name="sensitiveDescription" field="name" required>
              <UTextarea v-model="submissionData.SensitiveDescription" placeholder="What makes this genre culturally sensitive?" class="w-full"/>
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
            <UFormField label="EVOLVED FROM" field="name" name="PredecessorGenreIds">
              <!--- changed genrestate to submissionData --->
              <SubmissionSelectMenu
                v-model="submissionData.PredecessorGenreIds"
                :itemList="genreNames"
                class="w-full"/>
              <h1>Genres this one grew out of or was heavily influenced by.</h1>
            </UFormField>

            <UFormField label="GAVE RISE TO" field="name" name="SubGenreIds">
              <!--- changed genrestate to submissionData --->
              <SubmissionSelectMenu
                v-model="submissionData.SubGenreIds"
                :itemList ="genreNames"
                class="w-full"/>
              <h1>Genres that branched off or emerged from this one.</h1>
            </UFormField>

            <UFormField label="SIMILAR TO" field="name" name="SimilarGenreIds">
              <!--- changed genrestate to submissionData --->
              <SubmissionSelectMenu
                v-model="submissionData.SimilarGenreIds"
                :itemList ="genreNames"
                class="w-full"/>
              <h1>Genres with a similar sound, feel, or cultural context.</h1>
            </UFormField>

            <UFormField label="SOURCES" name="SourceLinks">
              <UInputTags v-model="submissionData.SourceLinks" placeholder="Add a link 'https://...' and press enter" style="color: #7a84a8" class="w-full"/>
              <h1>Wikipedia, academic papers, news articles, documentaries - anything that supports your submission. </h1>
            </UFormField>

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

        <div :class = "$style.formFields">
          <div :class="$style.reviewStepWrapper">
            <div :class="$style.buttonRowheadline">
              <div :class="$style.reviewSubject">IDENTITY</div>
              <button type = "button" @click="goToStep(1)" :class="$style.editButton">
                Edit
              </button>
            </div>

            <div :class="$style.subjectName">Genre Name</div>
            <div :class = "$style.genreNames">{{submissionData.NewGenreName}}</div>

            <div :class="$style.subjectName">Aliases</div>
            <div :class="$style.wrapper">
              <span
                v-for="(item, index) in submissionData.Aliases"
                :key="index"
                :class="$style.chip"
                >
                {{ item }}
                </span>
            </div>

            <div :class="$style.subjectName">Countries of origin</div>
            <div :class="$style.wrapper">
              <span
                v-for="(item, index) in submissionData.CountryIds"
                :key="index"
                :class="$style.chip"
                >
                {{ countryNameMap[item] ?? item }}
                </span>
            </div>
          </div>

          <USeparator orientation="horizontal" class="my-8" />

          <div :class="$style.reviewStepWrapper">
              <div :class="$style.buttonRowheadline">
                <div :class="$style.reviewSubject">ABOUT</div>
                <button type = "button" @click="goToStep(2)" :class="$style.editButton">
                  Edit
                </button>
              </div>
            <div :class="$style.subjectName">Description</div>
            <div :class = "$style.textfield">{{submissionData.Description}}</div>
            <div :class="$style.subjectName">Instruments</div>
            <div :class="$style.wrapper">
              <span
                v-for="(item, index) in submissionData.InstrumentIds"
                :key="index"
                :class="$style.chip"
                >
                {{ instrumentNameMap[item] ?? item }}
                </span>
            </div>

            <div :class="$style.subjectName">Playlist</div>
            <div :class = "$style.textfield">{{submissionData.PlaylistLink}}</div>
          </div>

          <USeparator orientation="horizontal" class="my-8" />

          <div :class="$style.reviewStepWrapper">
             <div :class="$style.buttonRowheadline">
              <div :class="$style.reviewSubject">CONNECTIONS</div>
              <button type = "button" @click="goToStep(3)" :class="$style.editButton">
                Edit
              </button>
            </div>
            <div :class="$style.subjectName">Evolved from</div>
            <div :class="$style.wrapper">
              <span
                v-for="(item, index) in submissionData.PredecessorGenreIds"
                :key="index"
                :class="$style.chip"
                >
                {{ genreNameMap[item] ?? item }}
                </span>
            </div>
            <div :class="$style.subjectName"> Gave rise to </div>
            <div :class="$style.wrapper">
              <span
                v-for="(item, index) in submissionData.SubGenreIds"
                :key="index"
                :class="$style.chip"
                >
                {{ genreNameMap[item] ?? item }}
                </span>
            </div>
            <div :class="$style.subjectName">Similar to </div>
            <div :class="$style.wrapper">
              <span
                v-for="(item, index) in submissionData.SimilarGenreIds"
                :key="index"
                :class="$style.chip"
                >
                {{ genreNameMap[item] ?? item }}
                </span>
            </div>
            <div :class="$style.subjectName">Sources</div>
            <div :class = "$style.wrapper">
              <span
                v-for="(item, index) in submissionData.SourceLinks"
                :key="index"
                :class="$style.chip">
              {{ item }}
              </span>
            </div>

          </div>
        </div>

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

            <UButton type="submit" @click="submitForm" style="background-color: #3DE8C8">
              Submit for review
            </UButton>
          </div>

      </UContainer>
    </div>
  </div>

  <div v-if="currentStep === 5">
    <UContainer :class ="$style.formsubmittedcard">
    <div class="relative flex items-center justify-center mb-6">
      <div :class="$style.ellipse">
        <div :class="$style.checkSymbol">✓</div>
      </div>
    </div>

    <div :class="$style.thanks">Thanks! {{submissionData.NewGenreName}} is now in review.</div>
    <div :class="$style.yourSubmissionHas">Your submission has been received and will be reviewed by our curatorial team. You'll be notified when it's approved or if the team has questions.</div>

    <div :class="$style.buttonAlone2">
      <UButton @click="submitNewGenre" style="background-color: #3DE8C8">
        Submit another genre
      </UButton>
    </div>
  </UContainer>
  </div>
</template>

<style module>
.genreidentityformcard {
  width: 50%;
  border-radius: 6px;
  background-color: #0d0f1a;
  border: 1px solid #1c2038;
  box-sizing: border-box;
  text-align: left;
  font-size: 0.75rem;
  color: #7a84a8;
  font-family: Space_Grotesk;
  margin: 2rem auto 4rem auto;
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
  color: #7a84a8;
  text-align: left;
  display: inline-block;
}

h1 {
  color: #7a84a8;
}

.formFields {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  margin-top: 1.5rem;
}

.formsubmittedcard {
width: 20%;
height: 380px;
position: relative;
border-radius: 6px;
background-color: #0d0f1a;
border: 1px solid #1c2038;
box-sizing: border-box;
overflow: hidden;
text-align: center;
font-size: 22px;
color: #3de8c8;
font-family: 'Space Grotesk';
margin: 10rem auto 0 auto;
}

.ellipse {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background-color: rgba(61, 232, 200, 0.08);
  border: 1px solid #3de8c8;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 44px auto 24px auto;
}
.checkSymbol {
  font-size: 24px;
  font-family: 'Space Grotesk';
  color: #3de8c8;
  line-height: 1;
}
.thanks {
position: absolute;
top: 124px;
left: 6%;
font-size: 18px;
font-weight: 500;
color: #e4e8f5;
display: inline-block;
width: 339px;
height: 20px;
}
.yourSubmissionHas {
position: absolute;
top: 156px;
left: 12.5%;
font-size: 13px;
line-height: 178%;
font-weight: 300;
color: #7a84a8;
display: inline-block;
width: 279px;
height: 20px;
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

.buttonRowheadline {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.buttonAlone {
  display: flex;
  justify-content: right;
  align-items: center;
  margin-top: 7rem;
}

.buttonAlone2 {
display: flex;
justify-content: center;
align-items: center;
margin-top: 10rem;
}

.editButton{
  width: 23px;
  height: 15px;
  position: relative;
  font-size: 12px;
  font-family: 'Space Grotesk';
  color: #3de8c8;
  text-align: left;
  display: inline-block;
}
.reviewStepWrapper {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 0px;
  width: 100%;
  align-items: stretch;
}

.reviewSubject {
  font-size: 12px;
  letter-spacing: 0.2em;
  font-family: 'Space Mono', monospace;
  color: #8899FF;
  text-transform: uppercase;
}

.subjectName {
  width: auto;
  height: 14px;
  font-size: 13px;
  font-family: 'Space Grotesk';
  color: #FFFFFF;
}

.textfield {
  width: 100%;
  height: auto;
  min-height: 20px;
  position: relative;
  font-size: 13px;
  line-height: 175%;
  font-weight: 300;
  font-family: 'Space Grotesk';
  color: #7a84a8;
  text-align: left;
  display: block;
  word-wrap: break-word;
  background-color: #14192E;
}



.genreNames{
  width: 100%;
  height: 20px;
  position: relative;
  font-size: 11px;
  line-height: 175%;
  font-weight: 300;
  font-family: 'Space Grotesk';
  color: #7a84a8;
  text-align: left;
  display: inline-block;
  background-color: #14192E;

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

.chip {
width: fit-content;
padding: 0 12px;
height: 24px;
display: flex;
align-items: center;
justify-content: center;
position: relative;
border-radius: 12px;
background-color: rgba(136, 153, 255, 0.12);
border: 1px solid #8899ff;
box-sizing: border-box;
overflow: hidden;
text-align: left;
font-size: 11px;
color: #8899ff;
font-family: 'Space Grotesk';
}

.wrapper {
  display: flex;
  flex-wrap: wrap;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 4px;
}

UButton {
  cursor: pointer;
}

button {
  cursor: pointer;
}

</style>
