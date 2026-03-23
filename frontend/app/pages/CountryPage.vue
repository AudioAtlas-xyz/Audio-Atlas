  <script setup lang="ts">
import type { Country } from '~/types/country';

  const route = useRoute()

  const countryId = computed(() => route.query.country as string | undefined)

  const { data, pending, error } = await useAsyncData<Country>('country', () =>
    $fetch<Country>(`http://localhost:5085/api/countries/${countryId.value}`)
  )

  </script>

  <template>

        


  </template>