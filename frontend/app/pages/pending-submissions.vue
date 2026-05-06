<script setup lang="ts">
import type { PendingSubmissionResponse } from '~/types/pendingSubmissionResponse'
import type { Country } from '~/types/country'
import type { Genre } from '~/types/genre'
import type {Instrument} from "~/types/instrument";

const { data: countriesData } = await useFetch<Country[]>('/api/countries/all')
const { data: genresData } = await useFetch<Genre[]>('/api/genres')
const { data: instrumentData } = await useFetch<Instrument[]>('/api/instruments')


const { data: pendingSubmissions, pending, error } = await useAsyncData<PendingSubmissionResponse[]>(
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
          sourceLinks: 'https://wikipedia.com',
          aliases: ['Neo Jazz', 'Future Jazz'],
          countryIds: ['c6953991-34f0-4c5f-8c04-04b0e114a653', 'f710e095-92bf-419a-88b0-0d757e35bddb'],
          instrumentsIds: ['5f9067ca-f8fc-4c48-b594-01a421476f5a', 'ae53da46-a309-419f-83b7-1047c784047a'],
          similarGenreIds: ['5b818376-5dbf-4dde-9a77-000b9fbeed15', 'ambient'],
          subGenreIds: ['2b0f305b-2a08-411d-b223-015663d137d9','6067421d-7bf5-433b-8ab5-0fca12c60b08'],
          predecessorGenreIds: ['fe1f0b31-f92a-4554-af7e-1978089f3f8f']
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
          sourceLinks: null,
          aliases: ['Dark Pop', 'Industrial Wave'],
          countryIds: ['37e6403b-0ebc-4db4-be59-01990b572d0e'],
          instrumentsIds: ['fe1fa608-f3d3-44ec-985c-2b7776899023', '90750e67-432f-46fa-9ce4-2976d1772520'],
          similarGenreIds: ['0dfe7d28-0be3-44e1-9228-95c04ddfcdbf', 'd08ed902-5329-4432-80e6-fe5798a90664'],
          subGenreIds: ['4621a1de-9ea2-40e4-bb9b-1daba2938c96'],
          predecessorGenreIds: ['422fe4b1-9fff-4d24-b222-21ac48b16d85']
        },
      ]
    }
)

const items = Array.from({ length: 30 }, (_, i) => ({
  id: i + 1,
  title: `Item ${i + 1}`,
  description: `Description for item ${i + 1}`
}))

function getCountryNames(ids: string[]) {
  if (!countriesData.value) return []
  return ids
    .map(id => countriesData.value.find(c => c.id === id)?.name).filter(Boolean)
}

function getGenreNames(ids: string[]) {
  if (!countriesData.value) return []
  return ids
    .map(id => genresData.value.find(c => c.id === id)?.name).filter(Boolean)
}

function getInstrumentNames(ids: string[]) {
  if (!instrumentData.value) return []
  return ids
    .map(id => instrumentData.value.find(c => c.id === id)?.type).filter(Boolean)
}

</script>


<template>
  <UContainer class="max-w-5xl mx-auto">
    <h1>Pending Submissions</h1>
    <UScrollArea
      v-slot="{ item, index }"
      :items="pendingSubmissions"
      class="w-full h-125"
    >
      <UCollapsible class="flex flex-col">
        <UButton
          size="xl"
          :label="item.newGenreName"
          variant="subtle"
          trailing-icon="i-lucide-chevron-down"
          :ui="{
          trailingIcon: 'group-data-[state=open]:rotate-180 transition-transform duration-200'
          }"
          block
        />

        <template #content>
          <div class="collapsibleCard">
            <h2>{{item.newGenreName}}</h2>
            <p><strong>Submission Id: </strong>{{item.id}}</p>
            <p><strong>Account: </strong>{{item.accountUsername}}</p>
            <p><strong>Description: </strong>{{item.description}}</p>
            <p><strong>Sensitive: </strong>{{item.isSensitive}}</p>
            <p><strong>Sensitive description: </strong>{{item.sensitiveDescription}}</p>
            <p><strong>Playlist link: </strong>{{item.playlistLink}}</p>
            <p><strong>Source link: </strong>{{item.sourceLinks}}</p>
            <p><strong>Aliases: </strong>{{item.aliases}}</p>
            <p><strong>Countries: </strong>
              {{getCountryNames(item.countryIds).join(', ')}}</p>
            <p><strong>Instruments: </strong>
              {{getInstrumentNames(item.instrumentsIds).join(', ')}}</p>
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
