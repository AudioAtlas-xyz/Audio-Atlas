  <script setup lang="ts">
    import { ref } from 'vue';
    import { slideover } from '#build/ui';
    import type { Country } from '~/types/country';
    import * as http from "node:http";
    const route = useRoute()
    const countryId = computed(() => route.query.country as string | undefined)
    const { data, pending, error } = await useAsyncData<Country>('country', () => $fetch<Country>('http://localhost:5085/api/countries/${countryId.value}') )
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
