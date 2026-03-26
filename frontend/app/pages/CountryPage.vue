  <script setup lang="ts">
import type { Country } from '~/types/country';

  const route = useRoute();

  const countryId = computed(() => route.query.country as string | undefined);


  const { data, pending, error } = await useAsyncData<Country>('country', () =>
    $fetch<Country>(`http://localhost:5085/api/countries/${countryId.value}`)
  );

  const genreCount = computed(() => data.value?.genres.length ?? 0);



  const cards = ref([
      {
        title: 'Genres',
        description: String(genreCount.value)
      },
      {
        title: 'Contributors',
        description: '400'
      }
  ]);
  </script>

  <template>
    <UPage class="max-w-[1040px] mx-auto px-4 sm:px-6 md:px-10">

      <UPageHeader 
        class="py-15 border-0">
          <template #title>
            <h1 class="text-5xl">{{ data?.name }}</h1>
          </template>
          <template #description>
            <p>
              [Region placeholder] • 
              {{ genreCount }} 
              genre{{ genreCount > 1 ? "s" : "" }}
            </p>
          </template>
      </UPageHeader>

      <UPageGrid class="gap-0 md:grid-cols-2">
        <UPageCard
          v-for="(card, index) in cards"
          :key="index"
          class="rounded-none first:rounded-l-md last:rounded-r-md border-r-0 bg-[#131624]"
        >
          <div class="px-1 py-1">
            <p class="text-2xl font-semibold leading-none"> 
              {{ card.description }}
            </p>
            <p class="mt-1 text-xs uppercase tracking-wider text-gray-500">
              {{ card.title }}
            </p>
          </div>
        </UPageCard>
      </UPageGrid>

      <div class="flex flex-col sm:flex-row gap-4 mt-6">

        <UButton
          color="primary"
          variant="solid"
          label="Submit Genre"
          size="lg"
        />

          <UButton
          color="primary"
          variant="solid"
          label="Submit Report"
          size="lg"
        />


      </div>
    </UPage>
  </template>