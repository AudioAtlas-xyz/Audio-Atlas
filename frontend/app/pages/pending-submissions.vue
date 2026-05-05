<script setup lang="ts">
import type { PendingSubmissionResponse } from '~/types/pendingSubmissionResponse'

const { data, pending, error } = await useAsyncData<PendingSubmissionResponse[]>(
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
          countryIds: ['dk', 'se'],
          instrumentsIds: ['piano', 'saxophone', 'synth'],
          similarGenreIds: ['jazz', 'ambient'],
          subGenreIds: ['nu-jazz'],
          predecessorGenreIds: ['jazz-fusion']
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
          countryIds: ['de'],
          instrumentsIds: ['synth', 'drum_machine'],
          similarGenreIds: ['darkwave', 'industrial'],
          subGenreIds: ['electro-pop'],
          predecessorGenreIds: []
        },
      ]
    }
)

const items = Array.from({ length: 30 }, (_, i) => ({
  id: i + 1,
  title: `Item ${i + 1}`,
  description: `Description for item ${i + 1}`
}))
</script>


<template>
  <UContainer class="max-w-5xl mx-auto">
    <h1>Pending Submissions</h1>
    <UScrollArea
      v-slot="{ item, index }"
      :items="data"
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
            <p><strong>Account: </strong>{{item.accountUsername}}</p>
            <p><strong>Description: </strong>{{item.description}}</p>
            <p><strong>Sensitive: </strong>{{item.isSensitive}}</p>
            <p><strong>Sensitive description: </strong>{{item.sensitiveDescription}}</p>
            <p><strong>Playlist link: </strong>{{item.playlistLink}}</p>
            <p><strong>Source link: </strong>{{item.sourceLinks}}</p>
            <p><strong>Aliases: </strong>{{item.aliases}}</p>
            <p><strong>Countries: </strong>{{item.countryIds}}</p>

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
