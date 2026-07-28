<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { useHead, useAsyncData, useRoute, useRouter } from '#imports'
import { useApi } from '@/composables/useApi'
import { useAuth } from '@/composables/useAuth'
import type {
  AdminGenreRow,
  AdminGenrePageResponse,
  AdminGenreDetail,
  AdminGenreLookup
} from '~/types/adminGenre'

definePageMeta({ middleware: 'curator' })
useHead({ title: 'Genre Catalogue | Audio Atlas Admin' })

const route = useRoute()
const router = useRouter()
const { api } = useApi()
const { user } = useAuth()

const isAdmin = computed(() => user.value?.roles?.includes('Admin') ?? false)

// ── URL-synced filter state ──────────────────────────────────────────────────

// Query values are `string | null | (string | null)[]` — a repeated param such
// as ?filter=a&filter=b yields an array — so narrow rather than cast.
const search = ref(typeof route.query.search === 'string' ? route.query.search : '')
// 'missing-note' is a legacy alias for 'below-gate'; normalise it so older
// dashboard links and bookmarks still highlight the right option.
const rawFilter = typeof route.query.filter === 'string' ? route.query.filter : 'all'
const filter = ref(rawFilter === 'missing-note' ? 'below-gate' : rawFilter)
const page = ref(Number(route.query.page) || 1)

const filterOptions = [
  { label: 'All genres',       value: 'all' },
  { label: 'Active only',      value: 'active' },
  { label: 'Archived',         value: 'archived' },
  { label: 'No description',   value: 'below-gate' },
  { label: 'No countries',     value: 'orphans' },
  { label: 'No sources',       value: 'missing-sources' },
  { label: 'Sensitive, no note', value: 'sensitive-incomplete' }
]

let searchDebounce: ReturnType<typeof setTimeout> | null = null

function onSearchInput(val: string) {
  search.value = val
  if (searchDebounce) clearTimeout(searchDebounce)
  searchDebounce = setTimeout(() => {
    page.value = 1
    pushUrl()
  }, 300)
}

function onFilterChange(val: string) {
  filter.value = val
  page.value = 1
  pushUrl()
}

function pushUrl() {
  router.replace({
    query: {
      ...(search.value ? { search: search.value } : {}),
      ...(filter.value !== 'all' ? { filter: filter.value } : {}),
      ...(page.value > 1 ? { page: String(page.value) } : {})
    }
  })
}

// ── Paginated genre list ─────────────────────────────────────────────────────

const {
  data: pageData,
  pending: listLoading,
  error: listError,
  refresh: refreshList
} = await useAsyncData<AdminGenrePageResponse>(
  'admin-genres',
  () => api<AdminGenrePageResponse>('/admin/genres', {
    query: {
      page: page.value,
      pageSize: 50,
      search: search.value || undefined,
      filter: filter.value
    }
  }),
  { server: false, default: () => ({ items: [], totalCount: 0, page: 1, pageSize: 50 }) }
)

const genres = computed<AdminGenreRow[]>(() => pageData.value?.items ?? [])
const totalCount = computed(() => pageData.value?.totalCount ?? 0)
const totalPages = computed(() => Math.max(1, Math.ceil(totalCount.value / 50)))

watch([page, filter], () => {
  closePanel()
  refreshList()
})

// ── Lookup data (for edit dropdowns) ────────────────────────────────────────

const { data: genreLookupData } = await useAsyncData<AdminGenreLookup[]>(
  'admin-genre-lookup',
  () => api<AdminGenreLookup[]>('/admin/genres/lookup'),
  { server: false, default: () => [] }
)

const { data: countryLookupData } = await useAsyncData<AdminGenreLookup[]>(
  'admin-countries-lookup',
  () => api<AdminGenreLookup[]>('/admin/genres/countries-lookup'),
  { server: false, default: () => [] }
)

const { data: instrumentLookupData } = await useAsyncData<AdminGenreLookup[]>(
  'admin-instruments-lookup',
  () => api<AdminGenreLookup[]>('/admin/genres/instruments-lookup'),
  { server: false, default: () => [] }
)

const genreOptions = computed(() =>
  (genreLookupData.value ?? []).map(g => ({ label: g.name, value: g.id }))
)
const countryOptions = computed(() =>
  (countryLookupData.value ?? []).map(c => ({ label: c.name, value: c.id }))
)
const instrumentOptions = computed(() =>
  (instrumentLookupData.value ?? []).map(i => ({ label: i.name, value: i.id }))
)

// ── Expand / edit panel ──────────────────────────────────────────────────────

const expandedId = ref<string | null>(null)
const detailLoading = ref(false)
const detailError = ref<string | null>(null)
const detail = ref<AdminGenreDetail | null>(null)

interface EditForm {
  name: string
  description: string
  startYear: string
  isSensitive: boolean
  sensitiveDescription: string
  playlistLink: string
  countryIds: string[]
  instrumentIds: string[]
  similarGenreIds: string[]
  subGenreIds: string[]
  parentGenreIds: string[]
  aliases: string[]
  sources: string[]
}

const editForm = reactive<EditForm>({
  name: '', description: '', startYear: '',
  isSensitive: false, sensitiveDescription: '', playlistLink: '',
  countryIds: [], instrumentIds: [],
  similarGenreIds: [], subGenreIds: [], parentGenreIds: [],
  aliases: [], sources: []
})

const saving = ref(false)
const saveError = ref<string | null>(null)
const saveSuccess = ref(false)

const actionLoading = ref(false)
const actionError = ref<string | null>(null)
const deleteConfirming = ref(false)

async function toggleRow(id: string) {
  if (expandedId.value === id) {
    closePanel()
    return
  }
  closePanel()
  expandedId.value = id
  detailLoading.value = true
  detailError.value = null
  try {
    detail.value = await api<AdminGenreDetail>(`/admin/genres/${id}`)
    populateForm(detail.value)
  } catch (e: any) {
    detailError.value = e?.data?.message ?? e?.message ?? 'Failed to load genre detail.'
  } finally {
    detailLoading.value = false
  }
}

function closePanel() {
  expandedId.value = null
  detail.value = null
  saveError.value = null
  saveSuccess.value = false
  actionError.value = null
  deleteConfirming.value = false
}

function populateForm(d: AdminGenreDetail) {
  editForm.name = d.name
  editForm.description = d.description ?? ''
  editForm.startYear = d.startYear != null ? String(d.startYear) : ''
  editForm.isSensitive = d.isSensitive
  editForm.sensitiveDescription = d.sensitiveDescription ?? ''
  editForm.playlistLink = d.playlistLink ?? ''
  editForm.countryIds = d.countries.map(c => c.id)
  editForm.instrumentIds = d.instruments.map(i => i.id)
  editForm.similarGenreIds = d.similarGenres.map(g => g.id)
  editForm.subGenreIds = d.subGenres.map(g => g.id)
  editForm.parentGenreIds = d.parentGenres.map(g => g.id)
  editForm.aliases = d.aliases.length ? [...d.aliases] : ['']
  editForm.sources = d.sources.length ? [...d.sources] : ['']
}

async function save() {
  if (!detail.value || !expandedId.value) return

  // Mirrors the server rule: countryIds is a full replacement list, so saving an
  // empty one would strip every country and orphan the genre. Checked here too
  // to give immediate feedback rather than a round-trip 400.
  if (editForm.countryIds.length === 0) {
    saveError.value = 'At least one country is required.'
    saveSuccess.value = false
    return
  }

  saving.value = true
  saveError.value = null
  saveSuccess.value = false

  const startYearNum = editForm.startYear.trim()
    ? parseInt(editForm.startYear, 10)
    : null

  const body = {
    name: editForm.name.trim(),
    description: editForm.description.trim() || null,
    startYear: startYearNum,
    isSensitive: editForm.isSensitive,
    sensitiveDescription: editForm.sensitiveDescription.trim() || null,
    playlistLink: editForm.playlistLink.trim() || null,
    countryIds: editForm.countryIds,
    instrumentIds: editForm.instrumentIds,
    similarGenreIds: editForm.similarGenreIds,
    subGenreIds: editForm.subGenreIds,
    parentGenreIds: editForm.parentGenreIds,
    aliases: editForm.aliases.filter(a => a.trim()),
    sources: editForm.sources.filter(s => s.trim())
  }

  try {
    await api(`/admin/genres/${expandedId.value}`, {
      method: 'PUT',
      body,
      headers: { 'If-Match': detail.value.rowVersion }
    })
    saveSuccess.value = true
    await refreshList()
    // Reload detail to get updated rowVersion
    detail.value = await api<AdminGenreDetail>(`/admin/genres/${expandedId.value}`)
    populateForm(detail.value)
  } catch (e: any) {
    if (e?.statusCode === 409 || e?.response?.status === 409) {
      saveError.value = 'Conflict: someone else edited this genre. Reload to see the latest version.'
    } else {
      saveError.value = e?.data?.message ?? e?.message ?? 'Save failed.'
    }
  } finally {
    saving.value = false
  }
}

async function archive() {
  if (!expandedId.value) return
  actionLoading.value = true
  actionError.value = null
  try {
    await api(`/admin/genres/${expandedId.value}/archive`, { method: 'POST' })
    await refreshList()
    closePanel()
  } catch (e: any) {
    actionError.value = e?.data?.message ?? e?.message ?? 'Archive failed.'
  } finally {
    actionLoading.value = false
  }
}

async function restore() {
  if (!expandedId.value) return
  actionLoading.value = true
  actionError.value = null
  try {
    await api(`/admin/genres/${expandedId.value}/restore`, { method: 'POST' })
    await refreshList()
    closePanel()
  } catch (e: any) {
    actionError.value = e?.data?.message ?? e?.message ?? 'Restore failed.'
  } finally {
    actionLoading.value = false
  }
}

async function hardDelete() {
  if (!expandedId.value) return
  actionLoading.value = true
  actionError.value = null
  deleteConfirming.value = false
  try {
    await api(`/admin/genres/${expandedId.value}`, { method: 'DELETE' })
    await refreshList()
    closePanel()
  } catch (e: any) {
    actionError.value = e?.data?.message ?? e?.message ?? 'Delete failed.'
  } finally {
    actionLoading.value = false
  }
}

// ── Alias / source list helpers ──────────────────────────────────────────────

function addAlias() { editForm.aliases.push('') }
function removeAlias(i: number) { editForm.aliases.splice(i, 1) }
function addSource() { editForm.sources.push('') }
function removeSource(i: number) { editForm.sources.splice(i, 1) }

// ── Pagination ───────────────────────────────────────────────────────────────

function goPage(n: number) {
  page.value = n
  pushUrl()
}

// ── Formatting helpers ───────────────────────────────────────────────────────

const dateFmt = new Intl.DateTimeFormat('en-US', { dateStyle: 'medium' })
function fmtDate(iso: string | null | undefined): string {
  if (!iso) return '—'
  return dateFmt.format(new Date(iso))
}

const breadcrumbItems = [
  { label: 'Explore', to: '/' },
  { label: 'Admin', to: '/admin' },
  { label: 'Genre catalogue', to: '/admin/genres', active: true }
]
</script>

<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">

      <!-- HERO -->
      <section class="border-b border-border bg-bg">
        <div class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:py-10">
          <div class="space-y-6">
            <UBreadcrumb :items="breadcrumbItems">
              <template #item-label="{ item, active }">
                <span :class="active ? 'text-aurora font-semibold' : 'text-muted'">
                  {{ item.label }}
                </span>
              </template>
            </UBreadcrumb>

            <div class="space-y-4">
              <h1 class="font-display text-5xl tracking-[-0.04em] text-space-50 sm:text-[52px]">
                Genre catalogue
              </h1>
              <p class="max-w-[40rem] text-sm text-meta">
                Edit genre data, fix quality gaps, manage relations, and archive or permanently remove entries.
              </p>
            </div>

            <p class="font-mono text-[11px] uppercase tracking-[0.18em] text-meta">
              {{ totalCount }} {{ filter === 'all' ? 'genres total' : 'genres match filter' }}
            </p>
          </div>
        </div>
      </section>

      <!-- FILTERS + TABLE -->
      <section class="mx-auto flex max-w-[1200px] flex-col gap-6 px-6 py-8 sm:px-10 lg:py-10">

        <!-- Filter bar -->
        <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <UInput
            :model-value="search"
            placeholder="Search by name"
            icon="i-lucide-search"
            class="w-full sm:max-w-sm"
            @update:model-value="onSearchInput"
          />
          <div class="flex items-center gap-2">
            <USelectMenu
              :model-value="filterOptions.find(o => o.value === filter)"
              :items="filterOptions"
              class="w-full sm:w-52"
              @update:model-value="(o: any) => onFilterChange(o.value)"
            />
            <UButton
              icon="i-lucide-refresh-cw"
              color="neutral"
              variant="outline"
              :loading="listLoading"
              aria-label="Refresh"
              @click="refreshList()"
            />
          </div>
        </div>

        <!-- Error -->
        <UAlert
          v-if="listError"
          color="error"
          variant="soft"
          title="Could not load genres"
          :description="listError.message ?? 'Unknown error'"
        >
          <template #actions>
            <UButton color="neutral" variant="outline" size="xs" @click="refreshList()">
              Retry
            </UButton>
          </template>
        </UAlert>

        <!-- Table -->
        <div v-else class="overflow-x-auto rounded-md border border-border bg-surface">
          <table class="w-full border-collapse">
            <thead class="bg-surface-2">
              <tr class="text-left">
                <th class="px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Name</th>
                <th class="px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Countries</th>
                <th class="px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Desc</th>
                <th class="px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Status</th>
                <th class="px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Last edited</th>
              </tr>
            </thead>

            <tbody>
              <!-- Empty state -->
              <tr v-if="!listLoading && genres.length === 0">
                <td colspan="6" class="px-4 py-10 text-center text-sm text-label">
                  No genres match the current filters.
                </td>
              </tr>

              <template v-for="row in genres" :key="row.id">
                <!-- Data row -->
                <tr
                  class="cursor-pointer border-t border-border transition hover:bg-surface-2"
                  :class="expandedId === row.id ? 'bg-surface-2' : ''"
                  @click="toggleRow(row.id)"
                >
                  <td class="px-4 py-3 font-mono text-[12px] tracking-[0.04em] text-aurora">
                    {{ row.name }}
                  </td>
                  <td class="px-4 py-3 text-[12px] text-meta">
                    {{ row.countryNames.slice(0, 3).join(', ') }}{{ row.countryNames.length > 3 ? ` +${row.countryNames.length - 3}` : '' }}
                    <span v-if="row.countryNames.length === 0" class="text-[#ff6b6b]">none</span>
                  </td>
                  <td class="px-4 py-3 text-[12px]">
                    <span :class="row.hasDescription ? 'text-[#3de8c8]' : 'text-[#ff6b6b]'">
                      {{ row.hasDescription ? '✓' : '✗' }}
                    </span>
                  </td>
                  <td class="px-4 py-3">
                    <span
                      class="rounded-full border px-2 py-0.5 font-mono text-[10px] uppercase tracking-[0.12em]"
                      :class="row.isArchived
                        ? 'border-[#7a84a8] text-meta'
                        : 'border-[#3de8c8] text-[#3de8c8]'"
                    >
                      {{ row.isArchived ? 'archived' : 'active' }}
                    </span>
                  </td>
                  <td class="px-4 py-3 text-[12px] text-meta">
                    {{ fmtDate(row.lastEditedAt) }}
                  </td>
                </tr>

                <!-- Expand panel -->
                <tr v-if="expandedId === row.id" :key="`${row.id}-panel`">
                  <td colspan="5" class="border-t border-border bg-[#0a0c18] p-0">

                    <!-- Loading -->
                    <div v-if="detailLoading" class="flex items-center gap-3 px-6 py-6 text-sm text-meta">
                      <UIcon name="i-lucide-loader" class="animate-spin" />
                      Loading…
                    </div>

                    <!-- Detail error -->
                    <div v-else-if="detailError" class="px-6 py-6">
                      <p class="text-sm text-[#ff6b6b]">{{ detailError }}</p>
                    </div>

                    <!-- Edit form -->
                    <div v-else-if="detail" class="px-6 py-6">
                      <div class="grid gap-6 lg:grid-cols-2">

                        <!-- LEFT COL: text fields -->
                        <div class="flex flex-col gap-4">

                          <!-- Name -->
                          <div>
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Name</label>
                            <UInput v-model="editForm.name" class="w-full" />
                          </div>

                          <!-- Description -->
                          <div>
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Description</label>
                            <UTextarea v-model="editForm.description" :rows="4" class="w-full" />
                          </div>

                          <!-- Start year + playlist -->
                          <div class="grid grid-cols-2 gap-3">
                            <div>
                              <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Start year</label>
                              <UInput v-model="editForm.startYear" type="number" class="w-full" />
                            </div>
                            <div>
                              <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Playlist link</label>
                              <UInput v-model="editForm.playlistLink" class="w-full" />
                            </div>
                          </div>

                          <!-- Sensitive -->
                          <div class="flex items-center gap-3">
                            <UToggle v-model="editForm.isSensitive" />
                            <span class="text-sm text-meta">Sensitive content</span>
                          </div>
                          <div v-if="editForm.isSensitive">
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Sensitive description</label>
                            <UTextarea v-model="editForm.sensitiveDescription" :rows="2" class="w-full" />
                          </div>

                          <!-- Aliases -->
                          <div>
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Aliases</label>
                            <div class="flex flex-col gap-1.5">
                              <div
                                v-for="(alias, i) in editForm.aliases"
                                :key="i"
                                class="flex items-center gap-2"
                              >
                                <UInput
                                  :model-value="alias"
                                  class="flex-1"
                                  @update:model-value="(v: string) => editForm.aliases[i] = v"
                                />
                                <UButton
                                  icon="i-lucide-x"
                                  color="neutral"
                                  variant="ghost"
                                  size="xs"
                                  @click="removeAlias(i)"
                                />
                              </div>
                              <UButton
                                color="neutral"
                                variant="ghost"
                                size="xs"
                                icon="i-lucide-plus"
                                class="self-start font-mono text-[10px] uppercase tracking-[0.12em] text-aurora"
                                @click="addAlias"
                              >
                                Add alias
                              </UButton>
                            </div>
                          </div>

                          <!-- Sources -->
                          <div>
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Sources</label>
                            <div class="flex flex-col gap-1.5">
                              <div
                                v-for="(src, i) in editForm.sources"
                                :key="i"
                                class="flex items-center gap-2"
                              >
                                <UInput
                                  :model-value="src"
                                  class="flex-1"
                                  placeholder="https://…"
                                  @update:model-value="(v: string) => editForm.sources[i] = v"
                                />
                                <UButton
                                  icon="i-lucide-x"
                                  color="neutral"
                                  variant="ghost"
                                  size="xs"
                                  @click="removeSource(i)"
                                />
                              </div>
                              <UButton
                                color="neutral"
                                variant="ghost"
                                size="xs"
                                icon="i-lucide-plus"
                                class="self-start font-mono text-[10px] uppercase tracking-[0.12em] text-aurora"
                                @click="addSource"
                              >
                                Add source
                              </UButton>
                            </div>
                          </div>
                        </div>

                        <!-- RIGHT COL: relations -->
                        <div class="flex flex-col gap-4">

                          <!-- Countries -->
                          <div>
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Countries</label>
                            <USelectMenu
                              :model-value="countryOptions.filter(o => editForm.countryIds.includes(o.value))"
                              :items="countryOptions"
                              multiple
                              searchable
                              class="w-full"
                              @update:model-value="(items: any[]) => editForm.countryIds = items.map(i => i.value)"
                            />
                          </div>

                          <!-- Instruments -->
                          <div>
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Instruments</label>
                            <USelectMenu
                              :model-value="instrumentOptions.filter(o => editForm.instrumentIds.includes(o.value))"
                              :items="instrumentOptions"
                              multiple
                              searchable
                              class="w-full"
                              @update:model-value="(items: any[]) => editForm.instrumentIds = items.map(i => i.value)"
                            />
                          </div>

                          <!-- Similar genres -->
                          <div>
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Similar genres</label>
                            <USelectMenu
                              :model-value="genreOptions.filter(o => editForm.similarGenreIds.includes(o.value))"
                              :items="genreOptions"
                              multiple
                              searchable
                              class="w-full"
                              @update:model-value="(items: any[]) => editForm.similarGenreIds = items.map(i => i.value)"
                            />
                          </div>

                          <!-- Sub-genres -->
                          <div>
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Sub-genres</label>
                            <USelectMenu
                              :model-value="genreOptions.filter(o => editForm.subGenreIds.includes(o.value))"
                              :items="genreOptions"
                              multiple
                              searchable
                              class="w-full"
                              @update:model-value="(items: any[]) => editForm.subGenreIds = items.map(i => i.value)"
                            />
                          </div>

                          <!-- Parent genres -->
                          <div>
                            <label class="mb-1 block font-mono text-[10px] uppercase tracking-[0.18em] text-meta">Parent genres</label>
                            <USelectMenu
                              :model-value="genreOptions.filter(o => editForm.parentGenreIds.includes(o.value))"
                              :items="genreOptions"
                              multiple
                              searchable
                              class="w-full"
                              @update:model-value="(items: any[]) => editForm.parentGenreIds = items.map(i => i.value)"
                            />
                          </div>

                          <!-- Audit info -->
                          <div class="mt-2 rounded-md border border-border bg-surface p-3 text-[11px] text-meta">
                            <p v-if="detail.lastEditedAt">
                              Last edited {{ fmtDate(detail.lastEditedAt) }}
                              <template v-if="detail.lastEditedByUsername"> by {{ detail.lastEditedByUsername }}</template>
                            </p>
                            <p v-if="detail.isArchived && detail.archivedAt">
                              Archived {{ fmtDate(detail.archivedAt) }}
                              <template v-if="detail.archivedByUsername"> by {{ detail.archivedByUsername }}</template>
                            </p>
                            <p v-if="!detail.lastEditedAt && !detail.isArchived" class="text-meta">
                              No edits recorded.
                            </p>
                          </div>
                        </div>
                      </div>

                      <!-- Save feedback -->
                      <div v-if="saveError" class="mt-4">
                        <p class="text-sm text-[#ff6b6b]">{{ saveError }}</p>
                      </div>
                      <div v-if="saveSuccess" class="mt-4">
                        <p class="text-sm text-[#3de8c8]">Saved.</p>
                      </div>
                      <div v-if="actionError" class="mt-4">
                        <p class="text-sm text-[#ff6b6b]">{{ actionError }}</p>
                      </div>

                      <!-- Action bar -->
                      <div class="mt-6 flex flex-wrap items-center gap-3 border-t border-border pt-5">

                        <UButton
                          :loading="saving"
                          :disabled="saving || actionLoading"
                          color="primary"
                          @click="save"
                        >
                          Save changes
                        </UButton>

                        <UButton
                          color="neutral"
                          variant="outline"
                          :disabled="saving || actionLoading"
                          @click="closePanel"
                        >
                          Cancel
                        </UButton>

                        <div class="ml-auto flex items-center gap-2">
                          <!-- Archive / Restore -->
                          <UButton
                            v-if="!detail.isArchived"
                            color="neutral"
                            variant="outline"
                            :loading="actionLoading"
                            :disabled="saving || actionLoading"
                            icon="i-lucide-archive"
                            @click="archive"
                          >
                            Archive
                          </UButton>
                          <UButton
                            v-else
                            color="neutral"
                            variant="outline"
                            :loading="actionLoading"
                            :disabled="saving || actionLoading"
                            icon="i-lucide-archive-restore"
                            @click="restore"
                          >
                            Restore
                          </UButton>

                          <!-- Hard delete (Admin only) -->
                          <template v-if="isAdmin">
                            <template v-if="!deleteConfirming">
                              <UButton
                                color="error"
                                variant="outline"
                                :disabled="saving || actionLoading"
                                icon="i-lucide-trash-2"
                                @click="deleteConfirming = true"
                              >
                                Delete
                              </UButton>
                            </template>
                            <template v-else>
                              <span class="text-xs text-[#ff6b6b]">Permanently delete?</span>
                              <UButton
                                color="error"
                                :loading="actionLoading"
                                :disabled="actionLoading"
                                size="xs"
                                @click="hardDelete"
                              >
                                Confirm
                              </UButton>
                              <UButton
                                color="neutral"
                                variant="ghost"
                                size="xs"
                                @click="deleteConfirming = false"
                              >
                                Cancel
                              </UButton>
                            </template>
                          </template>
                        </div>
                      </div>
                    </div>
                  </td>
                </tr>
              </template>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        <div
          v-if="totalPages > 1"
          class="flex items-center justify-between gap-3 font-mono text-[11px] uppercase tracking-[0.18em] text-meta"
        >
          <span>Page {{ page }} of {{ totalPages }}</span>
          <div class="flex items-center gap-2">
            <UButton
              color="neutral"
              variant="outline"
              size="xs"
              :disabled="page === 1"
              @click="goPage(page - 1)"
            >
              Previous
            </UButton>
            <UButton
              color="neutral"
              variant="outline"
              size="xs"
              :disabled="page === totalPages"
              @click="goPage(page + 1)"
            >
              Next
            </UButton>
          </div>
        </div>

      </section>
    </UContainer>
  </div>
</template>
