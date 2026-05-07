<script setup lang="ts">
import type { PendingSubmissionResponse } from '~/types/pendingSubmissionResponse'
import type { Country } from '~/types/country'
import type { Genre } from '~/types/genre'
import type {Instrument} from "~/types/instrument";

const { api } = useApi()

const { data: countriesData } = await useAsyncData<Country[]>('pending-countries', () => api('/countries/all'))
const { data: genresData } = await useAsyncData<Genre[]>('pending-genres', () => api('/genres'))
const { data: instrumentData } = await useAsyncData<Instrument[]>('pending-instruments', () => api('/instruments'))

const { data: pendingSubmissions, error} = await useAsyncData<PendingSubmissionResponse[]>(
  'pending-submissions',
    () => api<PendingSubmissionResponse[]>(`/submissions/pending`),
    {
      server: false,
      default: () => []

    }
)

function sortByDate(submissions: PendingSubmissionResponse[]) {
  return [...submissions].sort((a, b) => {
    const timeA = a.startDate ? new Date(a.startDate).getTime() : 0
    const timeB = b.startDate ? new Date(b.startDate).getTime() : 0
    return timeA - timeB
  })
}

function getCountryNames(ids: string[]) {
  const countries = countriesData.value ?? []
  return ids
    .map(id => countries.find(c => c.id === id)?.name).filter(Boolean)
}

function getGenreNames(ids: string[]) {
  const genres = genresData.value ?? []
  return ids
    .map(id => genres.find(c => c.id === id)?.name).filter(Boolean)
}

function getInstrumentNames(ids: string[]) {
  const instruments = instrumentData.value ?? []
  return ids
    .map(id => instruments.find(c => c.id === id)?.type).filter(Boolean)
}

</script>


<template>
  <UContainer class="max-w-5xl mx-auto">
    <h1>Pending Submissions</h1>
    <UScrollArea
      v-slot="{ item, index }"
      :items="sortByDate(pendingSubmissions)"
      class="w-full h-125"
    >
      <UCollapsible class="flex flex-col">
        <UButton
          size="xl"
          :label="item.newGenreName!"
          variant="subtle"
          trailing-icon="i-lucide-chevron-down"
          :ui="{
          trailingIcon: 'group-data-[state=open]:rotate-180 transition-transform duration-200'
          }"
          block
        >
          <div class="flex flex-col items-start">
            <span class="font-medium">{{ item.newGenreName }}</span>
            <span class="text-sm text-gray-500">{{item.accountUsername}}</span>
            <span class="text-sm text-gray-500"> {{item.startDate}}</span>
            <span class="text-sm text-gray-500">{{getCountryNames(item.countryIds).join(', ')}}</span>
          </div>
        </UButton>

        <template #content>
          <div class="collapsibleCard">
            <h2>{{item.newGenreName}}</h2>
            <p><strong>Submission Id: </strong>{{item.id}}</p>
            <p><strong>Contributor username: </strong>{{item.accountUsername}}</p>
            <p><strong>Description: </strong>{{item.description}}</p>
            <p><strong>Sensitive: </strong>{{item.isSensitive}}</p>
            <p><strong>Sensitive description: </strong>{{item.sensitiveDescription}}</p>
            <p><strong>Playlist link: </strong>{{item.playlistLink}}</p>
            <p><strong>Source link: </strong>{{item.sourceLinks}}</p>
            <p><strong>Aliases: </strong>{{item.aliases}}</p>
            <p><strong>Countries: </strong>
              {{getCountryNames(item.countryIds).join(', ')}}</p>
            <p><strong>Instruments: </strong>
              {{getInstrumentNames(item.instrumentIds).join(', ')}}</p>
            <p><strong>Similar genres: </strong>
              {{getGenreNames(item.similarGenreIds).join(', ')}}</p>
            <p><strong>Subgenres: </strong>
              {{getGenreNames(item.subGenreIds).join(', ')}}</p>
            <p><strong>Predecessor genres: </strong>
              {{getGenreNames(item.predecessorGenreIds).join(', ')}}</p>

          </div>
        </template>
      </UCollapsible>
    </UScrollArea>
  </UContainer>


</template>


<style scoped>
h1{
  margin: 2rem;
  color: white;
  font-size: 3.5rem;
  font-weight: 850;
  line-height: 0.88;
}
h2{
  margin: 2rem;
  font-size: 2rem;
}
p{
  margin: 0.5rem;
  margin-left: 2rem;
  margin-right: 2rem;
}
.collapsibleCard {
  background-color: #1f2937;
}
</style>
