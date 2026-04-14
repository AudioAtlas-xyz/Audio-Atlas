<script setup lang="ts">
import type { ContributorSummary, Country } from '~/types/country'
import type { NestedGenre } from '~/types/nestedgenre'

type CountryPageData = Country

const route = useRoute()

const countryId = computed(() => {
  const rawCountryId = route.query.countryId

  return typeof rawCountryId === 'string' && rawCountryId.length > 0 ? rawCountryId : undefined
})

const { data, pending, error } = await useAsyncData<CountryPageData | null>(
  'country-page',
  async () => {
    if (!countryId.value) {
      return null
    }

    return $fetch<CountryPageData>(`http://localhost:5085/api/countries/${countryId.value}`)
  },
  {
    watch: [countryId],
    default: () => null
  }
)

const country = computed(() => data.value)
const genres = computed(() => country.value?.genres ?? [])
const contributors = computed(() => country.value?.contributors ?? [])
const genreCount = computed(() => genres.value.length)
const contributorCount = computed(() => contributors.value.length)

const breadcrumbItems = computed(() =>
  [
    { label: 'Explore', to: '/' },
    country.value?.region ? { label: country.value.region, to: '/' } : null,
    country.value?.name ? { label: country.value.name, to: route.fullPath, active: true } : null
  ].filter(Boolean) as Array<{ label: string, to: string, active?: boolean }>
)

const locationBadges = computed(() =>
  [country.value?.region, country.value?.continent].filter((value): value is string => Boolean(value))
)

const contributorCardRows = computed(() =>
  contributors.value.slice(0, 3).map(contributor => ({
    ...contributor,
    genresLabel: formatGenreCount(contributor.genresCount ?? 0)
  }))
)

const pageDescription = computed(() => {
  if (!country.value?.description?.trim()) {
    return 'Country context from the Audio Atlas API will appear here once the backend payload is wired up.'
  }

  return country.value.description
})

useHead(() => ({
  title: country.value?.name ? `${country.value.name} | Audio Atlas` : 'Country | Audio Atlas'
}))

function getGenreSummary(genre: NestedGenre) {
  return genre.description?.trim() || 'Genre summary will appear here when the backend exposes it.'
}

function getGenreMeta(genre: NestedGenre) {
  return [genre.region, genre.aliases?.[0]].filter((value): value is string => Boolean(value))
}

function getContributorInitials(contributor: ContributorSummary) {
  const source = contributor.displayName || contributor.username || '?'

  return source
    .split(/\s+/)
    .map(part => part[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()
}

function formatGenreCount(count: number) {
  return `${count} genre${count === 1 ? '' : 's'}`
}

function getGenreStatusTone(status?: string) {
  if (!status) {
    return 'neutral'
  }

  switch (status.toLowerCase()) {
    case 'documented':
    case 'verified':
      return 'primary'
    case 'emerging':
      return 'info'
    case 'disputed':
      return 'warning'
    default:
      return 'neutral'
  }
}
</script>

<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">
      <section class="border-b border-border bg-bg">
        <div class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:px-[120px] lg:py-10">
          <div class="flex items-center justify-between gap-4">
            <UBreadcrumb
              v-if="breadcrumbItems.length"
              :items="breadcrumbItems"
              class="min-w-0"
            />

            <div class="hidden text-[11px] text-[#373d5a] lg:block">
              Know a genre we&apos;ve missed?
            </div>
          </div>

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

          <div v-else-if="country" class="grid gap-6 lg:grid-cols-[minmax(0,1fr)_320px] lg:items-start">
            <div class="space-y-5">
              <div class="flex flex-wrap items-center gap-2">
                <UBadge
                  v-for="badge in locationBadges"
                  :key="badge"
                  color="neutral"
                  variant="outline"
                  class="rounded-full border-[#7a84a8] px-2 py-1 text-[10px] font-medium uppercase tracking-[0.18em] text-space-50"
                >
                  {{ badge }}
                </UBadge>
              </div>

              <div class="space-y-4">
                <h1 class="font-display text-5xl tracking-[-0.04em] text-space-50 sm:text-[52px]">
                  {{ country.name }}
                </h1>
                <p class="max-w-[680px] text-sm leading-8 text-[#7a84a8]">
                  {{ pageDescription }}
                </p>
              </div>
            </div>

            <div class="space-y-3 lg:pt-[72px]">
              <UButton
                block
                size="lg"
                color="primary"
                class="justify-center bg-aurora px-4 py-2 font-medium text-bg hover:bg-aurora"
              >
                + Add a genre from {{ country.name }}
              </UButton>
              <p class="text-[11px] text-[#373d5a]">
                Know a {{ country.name }} genre we&apos;ve missed?
              </p>
            </div>
          </div>

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
              <UCard
                v-for="genre in genres"
                :key="genre.id"
                class="border border-border bg-surface shadow-none"
                :ui="{
                  header: 'border-b border-border bg-surface-2 px-4 py-3',
                  body: 'space-y-4 px-4 py-4',
                  footer: 'hidden'
                }"
              >
                <template #header>
                  <div class="flex items-start justify-between gap-4">
                    <div class="space-y-1">
                      <h3 class="font-display text-lg text-space-50">
                        {{ genre.name }}
                      </h3>
                      <div class="flex flex-wrap gap-2">
                        <UBadge
                          v-for="meta in getGenreMeta(genre)"
                          :key="meta"
                          color="neutral"
                          variant="subtle"
                          class="px-2 py-1 text-[10px] uppercase tracking-[0.18em]"
                        >
                          {{ meta }}
                        </UBadge>
                      </div>
                    </div>

                    <UBadge
                      v-if="genre.status"
                      :color="getGenreStatusTone(genre.status)"
                      variant="soft"
                      class="px-2 py-1 text-[10px] uppercase tracking-[0.18em]"
                    >
                      {{ genre.status }}
                    </UBadge>
                  </div>
                </template>

                <p class="text-sm leading-7 text-[#8b94b5]">
                  {{ getGenreSummary(genre) }}
                </p>

                <div class="flex items-center justify-between border-t border-border pt-4 text-[11px] text-[#4f587a]">
                  <span>ID {{ genre.id }}</span>
                  <span>{{ genre.contributorsCount ?? 0 }} contributors</span>
                </div>
              </UCard>
            </div>

            <UAlert
              v-else
              color="neutral"
              variant="soft"
              title="No genres documented yet"
              :description="country ? `No genres have been returned for ${country.name} yet.` : 'No genre data has been returned yet.'"
            />
          </div>

          <UCard
            class="border border-border bg-surface shadow-none"
            :ui="{
              header: 'border-b border-border bg-surface-2 px-4 py-3',
              body: 'px-0 py-0',
              footer: 'hidden'
            }"
          >
            <template #header>
              <div class="flex items-center justify-between">
                <p class="text-sm text-space-50">
                  Contributors
                </p>
                <p class="font-mono text-[11px] text-[#373d5a]">
                  {{ contributorCount }}
                </p>
              </div>
            </template>

            <div v-if="contributorCardRows.length" class="divide-y divide-border">
              <div
                v-for="contributor in contributorCardRows"
                :key="contributor.id"
                class="flex items-center justify-between gap-3 px-4 py-3"
              >
                <div class="flex min-w-0 items-center gap-3">
                  <UAvatar
                    :src="contributor.avatarUrl"
                    :alt="contributor.username"
                    :text="getContributorInitials(contributor)"
                    size="sm"
                    class="ring-1 ring-aurora"
                  />

                  <div class="min-w-0">
                    <ULink
                      :to="`/contributors/${contributor.id}`"
                      class="truncate font-mono text-[11px] tracking-[0.12em] text-aurora hover:text-aurora"
                    >
                      @{{ contributor.username }}
                    </ULink>
                    <p v-if="contributor.displayName" class="truncate text-xs text-[#5d678c]">
                      {{ contributor.displayName }}
                    </p>
                  </div>
                </div>

                <p class="shrink-0 text-[11px] text-[#373d5a]">
                  {{ contributor.genresLabel }}
                </p>
              </div>
            </div>

            <div v-else class="px-4 py-5 text-sm text-[#6f789b]">
              Contributors will appear here when the backend includes contributor summaries.
            </div>
          </UCard>
        </div>
      </section>
    </UContainer>
  </div>
</template>
