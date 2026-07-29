<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRoute, useAsyncData, useHead, useApi } from '#imports'
import type { Genre } from '~/types/genre'
import type { Country } from '~/types/country'
import type { Instrument } from '~/types/instrument'
import { useAuth } from '~/composables/useAuth'

const route = useRoute()
const { api } = useApi()
const { user } = useAuth()

const genreId = computed(() => {
  const raw = route.query.genreId
  return typeof raw === 'string' && raw.length > 0 ? raw : undefined
})

const { data: genre, pending: genrePending, error: genreError } = await useAsyncData<Genre | null>(
  () => `suggest-genre-${genreId.value ?? 'none'}`,
  async () => {
    if (!genreId.value) return null
    return api<Genre>(`/genres/${genreId.value}`)
  },
  { default: () => null }
)

const { data: countriesData } = await useAsyncData<Country[]>(
  'suggest-countries',
  () => api('/countries/all'),
  { default: () => [] }
)

const { data: genresData } = await useAsyncData<Genre[]>(
  'suggest-genres',
  () => api('/genres'),
  { default: () => [] }
)

const { data: instrumentData } = await useAsyncData<Instrument[]>(
  'suggest-instruments',
  () => api('/instruments'),
  { default: () => [] }
)

useHead(() => ({
  title: genre.value?.name
    ? `Suggest edit: ${genre.value.name} | Audio Atlas`
    : 'Suggest edit | Audio Atlas'
}))

// ── Select options ────────────────────────────────────────────────────────────

const countryOptions = computed(() =>
  (countriesData.value ?? []).map(c => ({ label: c.name, value: c.id }))
)

const genreOptions = computed(() =>
  (genresData.value ?? []).map(g => ({ label: g.name, value: g.id }))
)

const instrumentOptions = computed(() =>
  (instrumentData.value ?? []).map(i => ({ label: i.type, value: i.id }))
)

// ── Form state (pre-populated from current genre) ────────────────────────────

const description = ref('')
const exampleSongYoutubeId = ref('')
const startDate = ref('')
const isSensitive = ref(false)
const sensitiveDescription = ref('')
const aliasText = ref('')
const sourceText = ref('')
const countryIds = ref<string[]>([])
const instrumentIds = ref<string[]>([])
const similarGenreIds = ref<string[]>([])
const subGenreIds = ref<string[]>([])
const predecessorGenreIds = ref<string[]>([])

// Populate form once genre data arrives
watch(genre, (g) => {
  if (!g) return
  description.value = g.description ?? ''
  exampleSongYoutubeId.value = g.exampleSongYoutubeId ?? ''
  startDate.value = g.startYear ? `${g.startYear}-01-01` : ''
  isSensitive.value = g.isSensitive ?? false
  sensitiveDescription.value = g.sensitiveDescription ?? ''
  aliasText.value = (g.aliases ?? []).map(a => a.alias).join('\n')
  sourceText.value = (g.sources ?? []).map(s => s.sourceLink).join('\n')
  countryIds.value = g.countries.map(c => c.id)
  instrumentIds.value = g.instruments.map(i => i.id)
  similarGenreIds.value = g.similarGenres.map(s => s.id)
  subGenreIds.value = g.subGenres.map(s => s.id)
  predecessorGenreIds.value = g.parentGenres.map(p => p.id)
}, { immediate: true })

// ── Submission ────────────────────────────────────────────────────────────────

const submitting = ref(false)
const submitError = ref<string | null>(null)
const submitted = ref(false)

async function submit() {
  if (!genreId.value) return

  submitting.value = true
  submitError.value = null

  try {
    const aliases = aliasText.value.split('\n').map(s => s.trim()).filter(Boolean)
    const sourceLinks = sourceText.value.split('\n').map(s => s.trim()).filter(Boolean)

    await api(`/submissions/suggest/${genreId.value}`, {
      method: 'POST',
      body: {
        description: description.value.trim() || null,
        exampleSongYoutubeId: exampleSongYoutubeId.value.trim() || null,
        startDate: startDate.value || null,
        isSensitive: isSensitive.value,
        sensitiveDescription: isSensitive.value ? sensitiveDescription.value.trim() || null : null,
        aliases,
        sourceLinks,
        countryIds: countryIds.value,
        instrumentIds: instrumentIds.value,
        similarGenreIds: similarGenreIds.value,
        subGenreIds: subGenreIds.value,
        predecessorGenreIds: predecessorGenreIds.value
      }
    })

    submitted.value = true
  }
  catch (err) {
    submitError.value = err instanceof Error ? err.message : 'Something went wrong. Please try again.'
  }
  finally {
    submitting.value = false
  }
}

const genreHref = computed(() =>
  genreId.value ? `/genres?genreId=${genreId.value}` : '/'
)

const canSuggest = computed(() =>
  !!user.value && !user.value.roles?.includes('Banned')
)

const breadcrumbItems = computed(() => [
  { label: 'Explore', to: '/' },
  genre.value?.name
    ? { label: genre.value.name, to: genreHref.value }
    : null,
  { label: 'Suggest edit', to: route.fullPath, active: true }
].filter(Boolean) as Array<{ label: string; to: string; active?: boolean }>)
</script>

<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">

      <!-- HERO -->
      <section class="border-b border-border bg-bg">
        <div class="mx-auto max-w-[800px] px-6 py-8 sm:px-10 lg:py-10 space-y-4">
          <UBreadcrumb :items="breadcrumbItems">
            <template #item-label="{ item, active }">
              <span :class="active ? 'text-aurora font-semibold' : 'text-muted'">
                {{ item.label }}
              </span>
            </template>
          </UBreadcrumb>

          <div v-if="genrePending" class="space-y-3">
            <USkeleton class="h-12 w-56 rounded-md bg-surface-2" />
            <USkeleton class="h-4 w-80 rounded-md bg-surface-2" />
          </div>

          <div v-else-if="genre">
            <h1 class="font-display text-4xl tracking-[-0.03em] text-space-50">
              Suggest an edit
            </h1>
            <p class="mt-2 text-sm text-meta">
              Proposing changes to
              <NuxtLink :to="genreHref" class="text-aurora hover:opacity-70 underline decoration-dotted">
                {{ genre.name }}
              </NuxtLink>.
              A curator will review your suggestion before it goes live.
            </p>
          </div>

          <UAlert
            v-else-if="genreError"
            color="error"
            variant="soft"
            title="Could not load genre"
            :description="genreError.message"
          />

          <UAlert
            v-else
            color="warning"
            variant="soft"
            title="No genre specified"
            description="Open this page with a ?genreId=... query parameter."
          />
        </div>
      </section>

      <!-- FORM -->
      <section class="mx-auto max-w-[800px] px-6 py-8 sm:px-10 lg:py-10">

        <!-- Not logged in -->
        <UAlert
          v-if="!user"
          color="warning"
          variant="soft"
          title="Sign in required"
          description="You must be signed in to suggest edits."
        />

        <!-- Banned -->
        <UAlert
          v-else-if="!canSuggest"
          color="error"
          variant="soft"
          title="Account restricted"
          description="Your account is not able to submit suggestions."
        />

        <!-- Success -->
        <div v-else-if="submitted" class="rounded-md border border-border bg-surface p-8 text-center space-y-4">
          <p class="font-display text-2xl text-aurora">Suggestion submitted!</p>
          <p class="text-sm text-meta">
            A curator will review your suggestion. Thank you for contributing to Audio Atlas.
          </p>
          <NuxtLink :to="genreHref">
            <UButton color="primary" variant="solid">
              Back to {{ genre?.name ?? 'genre' }}
            </UButton>
          </NuxtLink>
        </div>

        <!-- Form -->
        <form v-else-if="genre && canSuggest" class="space-y-6" @submit.prevent="submit">

          <UAlert
            v-if="submitError"
            color="error"
            variant="soft"
            title="Submission failed"
            :description="submitError"
          />

          <p class="text-xs text-meta">
            Only fill in the fields you want to change. Empty fields will be ignored during approval.
            Relation fields (countries, instruments, etc.) will replace existing ones if you include any entries.
          </p>

          <!-- Description -->
          <div>
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Description
            </label>
            <UTextarea v-model="description" :rows="6" class="w-full" />
          </div>

          <!-- Playlist link -->
          <div>
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Playlist link
            </label>
            <UInput v-model="exampleSongYoutubeId" placeholder="https://www.youtube.com/watch?v=…" class="w-full" />
          </div>

          <!-- Start date -->
          <div class="max-w-[200px]">
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Start date
            </label>
            <UInput v-model="startDate" placeholder="YYYY-MM-DD" class="w-full" />
          </div>

          <!-- Countries -->
          <div>
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Countries
            </label>
            <USelectMenu
              :model-value="countryOptions.filter(o => countryIds.includes(o.value))"
              :items="countryOptions"
              multiple
              class="w-full"
              @update:model-value="(items) => countryIds = items.map(i => i.value)"
            />
          </div>

          <!-- Instruments -->
          <div>
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Instruments
            </label>
            <USelectMenu
              :model-value="instrumentOptions.filter(o => instrumentIds.includes(o.value))"
              :items="instrumentOptions"
              multiple
              class="w-full"
              @update:model-value="(items) => instrumentIds = items.map(i => i.value)"
            />
          </div>

          <!-- Similar genres -->
          <div>
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Similar genres
            </label>
            <USelectMenu
              :model-value="genreOptions.filter(o => similarGenreIds.includes(o.value))"
              :items="genreOptions"
              multiple
              searchable
              class="w-full"
              @update:model-value="(items) => similarGenreIds = items.map(i => i.value)"
            />
          </div>

          <!-- Subgenres -->
          <div>
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Subgenres
            </label>
            <USelectMenu
              :model-value="genreOptions.filter(o => subGenreIds.includes(o.value))"
              :items="genreOptions"
              multiple
              searchable
              class="w-full"
              @update:model-value="(items) => subGenreIds = items.map(i => i.value)"
            />
          </div>

          <!-- Predecessor genres -->
          <div>
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Predecessor genres
            </label>
            <USelectMenu
              :model-value="genreOptions.filter(o => predecessorGenreIds.includes(o.value))"
              :items="genreOptions"
              multiple
              searchable
              class="w-full"
              @update:model-value="(items) => predecessorGenreIds = items.map(i => i.value)"
            />
          </div>

          <!-- Aliases -->
          <div>
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Aliases <span class="normal-case tracking-normal text-label">(one per line)</span>
            </label>
            <UTextarea v-model="aliasText" :rows="3" class="w-full" />
          </div>

          <!-- Source links -->
          <div>
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Source links <span class="normal-case tracking-normal text-label">(one per line)</span>
            </label>
            <UTextarea v-model="sourceText" :rows="3" class="w-full" />
          </div>

          <!-- Sensitive toggle -->
          <div class="flex items-center justify-between rounded-md border border-border bg-surface-2 px-4 py-3">
            <div>
              <p class="text-sm font-medium text-space-50">Sensitive content</p>
              <p class="text-xs text-meta">Flag this genre as culturally sensitive</p>
            </div>
            <UToggle v-model="isSensitive" />
          </div>

          <div v-if="isSensitive">
            <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
              Sensitivity description
            </label>
            <UTextarea v-model="sensitiveDescription" :rows="3" class="w-full" />
          </div>

          <!-- Submit -->
          <div class="flex gap-3 pt-2">
            <UButton
              type="submit"
              color="primary"
              variant="solid"
              :loading="submitting"
            >
              Submit suggestion
            </UButton>
            <NuxtLink :to="genreHref">
              <UButton color="neutral" variant="outline" :disabled="submitting">
                Cancel
              </UButton>
            </NuxtLink>
          </div>

        </form>

      </section>

    </UContainer>
  </div>
</template>
