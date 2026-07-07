<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useAsyncData } from '#imports'
import type { Country } from '~/types/country'
import { useApi } from '@/composables/useApi'

const route = useRoute()
const { api } = useApi()

const countryId = computed(() => route.query.country as string | undefined)

const { data, pending, error } = await useAsyncData<Country | null>(
  () => `country-${countryId.value}`,
  async () => {
    if (!countryId.value) return null

    return api<Country>(`/countries/${countryId.value}`)
  },
  {
    watch: [countryId]
  }
)
</script>

  <template>
    <slideover dismissable="true" title="Country Details" :open="true" @close="$emit('close')">
      <template #content>
        <div v-if="pending">Loading...</div> <div v-else-if="error">Error loading country details.</div>
        <div v-else>
          <h2 class="text-2xl font-bold mb-4">{{ data?.name }}</h2>
          <p class="mb-2"><strong>Description:</strong> {{ data?.description }}</p>
          <h3 class="text-xl font-bold mb-2">Genres</h3>
          <ul class="list-disc pl-5"> <li v-for="genre in data?.genres" :key="genre.id" class="mb-1"> {{ genre.name }} </li> </ul>
        </div>
      </template>
    </slideover>
  </template>
