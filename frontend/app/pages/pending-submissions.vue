<script setup lang="ts">
import type { PendingSubmissionResponse } from '~/types/pendingSubmissionResponse'
import type { Country } from '~/types/country'
import type { Genre } from '~/types/genre'
import type {Instrument} from "~/types/instrument";

const { api } = useApi()

const { data: countriesData } = await useFetch<Country[]>('/api/countries/all')
const { data: genresData } = await useFetch<Genre[]>('/api/genres')
const { data: instrumentData } = await useFetch<Instrument[]>('/api/instruments')

const { data: pendingSubmissions} = await useAsyncData<PendingSubmissionResponse[]>(
  'pending-submissions',
    () => api<PendingSubmissionResponse[]>(`/submissions/pending`),
    {
      default: () => [
        {
          id: '1',
          accountId: 'account_1',
          accountUsername: 'annanassen',
          newGenreName: 'Neo Jazz Fusion',
          startDate: '2025-01-10',
          endDate: null,
          description: 'A modern blend of jazz, electronic, and ambient textures.',
          isSensitive: false,
          sensitiveDescription: null,
          playlistLink: 'https://spotify.com',
          sourceLinks: ['https://wikipedia.com'],
          aliases: ['Neo Jazz', 'Future Jazz'],
          countryIds: ['f5a26844-13e1-4a6d-b0b6-001c40be4665', '8512c1ce-d6ee-4190-bf79-04482caa4d75'],
          instrumentIds: ['cd8958bb-0ece-4602-8437-2d145fc90084'],
          similarGenreIds: ['b5eaf5eb-5eee-400f-844a-000a73fd52ed', '4d65e956-46c0-4b3e-b070-00547d590679'],
          subGenreIds: ['982a1419-e87b-4047-b429-0ca8e2c94ea1'],
          predecessorGenreIds: []
        },
        {
          id: '2',
          accountId: 'account_2',
          accountUsername: 'djThit',
          newGenreName: 'Darkwave Industrial Pop',
          startDate: '2024-11-03',
          endDate: null,
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam aliquam lectus tempor, consequat magna id, aliquam tortor. Cras eu leo eget nibh bibendum maximus eu id orci. Sed pulvinar diam eros, suscipit bibendum augue facilisis luctus. Praesent auctor mauris in iaculis luctus. Praesent condimentum tellus non mauris pharetra, sed tempus.',
          isSensitive: true,
          sensitiveDescription: 'Contains references to dystopian themes and aggressive sound design.',
          playlistLink: 'https://spotify.com',
          sourceLinks: [],
          aliases: ['Dark Pop', 'Industrial Wave'],
          countryIds: ['a7e452a0-cff9-47d9-9dc6-11ba380d5198'],
          instrumentIds: ['6bcecefc-7a79-48f0-9c6d-00e36b2d3e80', '2a4b4d71-ead8-4603-9fcc-0edbe9abd1fd'],
          similarGenreIds: ['4d65e956-46c0-4b3e-b070-00547d590679', 'b3290861-bb97-4e5f-8c85-01958797e917'],
          subGenreIds: ['699a27d4-bf1f-46ad-a77a-067d8bcdbd8a'],
          predecessorGenreIds: ['8f006b3a-93ac-4a16-8adf-0af747a40c2f']
        },
      ]
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
