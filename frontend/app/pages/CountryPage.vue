<script setup lang="ts">
import type { Country } from '~/types/country'

//UseRoute() giver dig adgang til 'Route objekt.' Den indeholder path, query parameters, route params, full URL, navn på route.
//Vi skal bruge dette object for at få adgang til query parametrene, for at se hvilket ID de har anvendt.
const route = useRoute()

//Her trækker vi selve countryId ud af query parametrene
const countryId = computed(() => {
  //Vi kalder på countryId. Dermed SKAL query i URL være CountryPage?countryId=MIT_GUID (efter spørgsmålstegn)
  const rawCountryId = route.query.countryId

  //Vi tjekker om countryId er en string og indeholder noget tekst. Hvis ikke returnere vi undefined, da dette er en falsy som vi kan tjekke på senere.
  return typeof rawCountryId === 'string' && rawCountryId.length > 0 ? rawCountryId : undefined
})

//Her henter vi dataen fra backend API. I skal lave en DTO som matcher 1-1 med JSON respons. Så Kan Nuxt selv mappe respons to objektet.
//På denne måde kan i så refere til values på et objekt, istedet for json strings. DTO'en skal angives i både useAsyncData og i $fetch
const { data, pending, error } = await useAsyncData<Country | null>(
  //nedenstående string er en nøgle. Når vi henter data, cacher vi det hos klienten under denne nøgle. Hvis klienten
  //Forespørger samme land igen, henter vi ikke fra API'et, men henter fra cache. Så undgår vi stress på backend
  'country-page',

  //Async kald her er nødvendigt for at få et promise (pending objektet). Uden async kald står frontend aktivt og venter,
  //og loader ingen visuelle elementer.
  async () => {
    //Hvis countryId er undefined, returnere vi null. undefined er falsy, modsat et tom string er ikke falsy.
    //Derfor lavede vi et tjek tidligere, da hvis vi ikke havde sat den til undefined, ville denne være true.
    if (!countryId.value) {
      return null
    }

    //Her kalder vi selve backend, og indsætter countryId, vi har fået.
    return $fetch<Country>(`http://localhost:5085/api/countries/${countryId.value}`)
  },
  {
    //En watch holder øje med om countryId har ændret sig. Da vi cacher hele country-page siden og genbruger cache,
    //tjekker vi med en watch om countryId er det samme som tidligere, for hvis ikke skal vi ikke genbruge cache.
    // (Ellers ville vi få data for et land der ikke er det samme land ...)
    watch: [countryId],
    //Default værdi for hvad data skal være, inden vores promise returnere noget.
    default: () => null
  }
)

//Data er et ref objekt. Her hiver vi værdierne ud af ref objektet fra responsen, så vi kan bruge dem individuelt.
//Computed beregner en værdi ud fra en reactive state. Reactive state i nuxt betyder, at værdien kan ændre sig dynamisk under runtime.
//Vi bruger dem her, fordi alle nedenstående værdier formentlig er null, da vi ikke har fået en respons endnu.
//Så ved at bruge computed, vil variablerne nedenunder ændre sig til deres sande værdier så snart vi får en respons
//Fra vores backend.
const country = computed(() => data.value)
const genres = computed(() => country.value?.genres ?? [])
const contributors = computed(() => country.value?.contributors ?? [])
const genreCount = computed(() => genres.value.length)

//Vi laver en liste med alle vores regions og navne på lande. Lige nu er region sat til "/" som path, fordi vi ikke
//Har en region path.
//Denne er igen computed, så listen opdatere så snart vi har data fra vores backend dynamisk.
const breadcrumbItems = computed(() =>
  [
    { label: 'Explore', to: '/' },
    country.value?.region ? { label: country.value.region, to: '/' } : null,
    country.value?.name ? { label: country.value.name, to: route.fullPath, active: true } : null
    //Filer boolean fjerne alle falsy elementer. I dette tilfælde ville det være null værdier.
  ].filter(Boolean) as Array<{ label: string, to: string, active?: boolean }>
)


//Vi laver location badges. Dette er region og continent. Gør det samme som ovenstående, men filtrere på om string er empty.
const locationBadges = computed(() =>
  [country.value?.region, country.value?.continent].filter((value): value is string => Boolean(value))
)

//Viser page description. Har en default value.
const pageDescription = computed(() => {
  if (!country.value?.description?.trim()) {
    return 'Country context from the Audio Atlas API will appear here once the backend payload is wired up.'
  }

  return country.value.description
})

//Dette sætter titlen på selve browseren (det der står i tabben).
useHead(() => ({
  title: country.value?.name ? `${country.value.name} | Audio Atlas` : 'Country | Audio Atlas'
}))
</script>

<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">
      <section class="border-b border-border bg-bg">
        <div class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:px-[120px] lg:py-10">

          <div v-if="pending" class="grid gap-6 lg:grid-cols-[minmax(0,1fr)_320px] lg:items-start">
            <div class="space-y-4">
              <USkeleton class="h-14 w-64 rounded-md bg-surface-2" />
              <div class="flex gap-2">
                <USkeleton class="h-6 w-24 rounded-full bg-surface-2" />
                <USkeleton class="h-6 w-20 rounded-full bg-surface-2" />
              </div>
              <USkeleton class="h-20 w-full rounded-md bg-surface-2" />
            </div>

            <div class="space-y-3">
              <USkeleton class="h-10 w-full rounded-md bg-surface-2" />
              <USkeleton class="h-4 w-40 rounded-md bg-surface-2" />
            </div>
          </div>

          <CountryHeroSection
            v-else-if="country"
            :location-badges="locationBadges"
            :country-name="country.name"
            :description="pageDescription"
            :bread-crumb-items="breadcrumbItems"
          />

          <UAlert
            v-else-if="error"
            color="error"
            variant="soft"
            title="Could not load country data"
            :description="error.message"
          />

          <UAlert
            v-else
            color="warning"
            variant="soft"
            title="Missing countryId"
            description="Open this page with a ?countryId=... query so the page can request country data from the backend."
          />
        </div>
      </section>

      <section class="border-y border-border bg-surface">
        <div class="mx-auto max-w-[1200px] px-6 py-3 sm:px-10 lg:px-[120px]">
          <div class="space-y-1">
            <p class="font-display text-xl text-aurora">
              {{ genreCount }}
            </p>
            <p class="text-[11px] text-[#373d5a]">
              Genres documented
            </p>
          </div>
        </div>
      </section>

      <section class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:px-[120px] lg:py-10">
        <div class="border-b border-border pb-4">
          <h2 class="font-display text-[28px] tracking-[-0.02em] text-space-50">
            Genres
          </h2>
          <p class="mt-2 text-xs text-[#373d5a]">
            {{ genreCount }} {{ genreCount === 1 ? 'genre' : 'genres' }} from
            {{ country?.name || 'this country' }} documented in Audio Atlas
          </p>
        </div>

        <div class="grid gap-8 lg:grid-cols-[minmax(0,1fr)_300px] lg:items-start">
          <div>
            <div v-if="pending" class="grid gap-5 md:grid-cols-2">
              <USkeleton
                v-for="index in 4"
                :key="index"
                class="h-48 rounded-md bg-surface-2"
              />
            </div>

            <div v-else-if="genres.length" class="grid gap-5 md:grid-cols-2">
              <CountryGenreCard
                v-for="genre in genres"
                :key="genre.id"
                :genre="genre"
              />
            </div>

            <UAlert
              v-else
              color="neutral"
              variant="soft"
              title="No genres documented yet"
              :description="country ? `No genres have been returned for ${country.name} yet.` : 'No genre data has been returned yet.'"
            />
          </div>

          <CountryContributorsCard :contributors="contributors" />
        </div>
      </section>
    </UContainer>
  </div>
</template>
