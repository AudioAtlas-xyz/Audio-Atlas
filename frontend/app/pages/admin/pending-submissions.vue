<script setup lang="ts">
import { ref, computed, reactive } from 'vue'
import { useHead, useAsyncData } from '#imports'
import { useApi } from '@/composables/useApi'
import type { PendingSubmissionResponse } from '~/types/pendingSubmissionResponse'
import type { Country } from '~/types/country'
import type { Genre } from '~/types/genre'
import type { Instrument } from '~/types/instrument'

definePageMeta({ middleware: 'admin' })
useHead({ title: 'Submission Queue | Audio Atlas Admin' })

const { api } = useApi()

// ── Reference data ──────────────────────────────────────────────────────────

const { data: countriesData } = await useAsyncData<Country[]>(
  'sub-countries',
  () => api('/countries/all'),
  { default: () => [] }
)

const { data: genresData } = await useAsyncData<Genre[]>(
  'sub-genres',
  () => api('/genres'),
  { default: () => [] }
)

const { data: instrumentData } = await useAsyncData<Instrument[]>(
  'sub-instruments',
  () => api('/instruments'),
  { default: () => [] }
)

// ── Rejection reasons ────────────────────────────────────────────────────────

interface RejectionReason { code: string; label: string }

const { data: rejectionReasonsData } = await useAsyncData<RejectionReason[]>(
  'rejection-reasons',
  () => api('/submissions/rejection-reasons'),
  { default: () => [] }
)

const rejectionReasonOptions = computed(() =>
  (rejectionReasonsData.value ?? []).map(r => ({ label: r.label, value: r.code }))
)

// ── Pending submissions ──────────────────────────────────────────────────────

const {
  data: pendingSubmissions,
  pending: queueLoading,
  error: queueError,
  refresh: refreshQueue
} = await useAsyncData<PendingSubmissionResponse[]>(
  'pending-submissions',
  () => api<PendingSubmissionResponse[]>('/submissions/pending'),
  { server: false, default: () => [] }
)

// ── Helpers ──────────────────────────────────────────────────────────────────

function formatDate(iso: string) {
  return new Date(iso).toLocaleString(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short'
  })
}

function getCountryNames(ids: string[]): string[] {
  return ids.map(id => (countriesData.value ?? []).find(c => c.id === id)?.name).filter(Boolean) as string[]
}

function getGenreNames(ids: string[]): string[] {
  return ids.map(id => (genresData.value ?? []).find(g => g.id === id)?.name).filter(Boolean) as string[]
}

function getInstrumentNames(ids: string[]): string[] {
  return ids.map(id => (instrumentData.value ?? []).find(i => i.id === id)?.type).filter(Boolean) as string[]
}

// ── Search/filter ─────────────────────────────────────────────────────────────

const search = ref('')

const visibleSubmissions = computed(() => {
  const q = search.value.trim().toLowerCase()
  const all = [...(pendingSubmissions.value ?? [])].sort(
    (a, b) => new Date(a.submittedAt).getTime() - new Date(b.submittedAt).getTime()
  )
  return q ? all.filter(s => s.newGenreName?.toLowerCase().includes(q)) : all
})

// ── Action state ──────────────────────────────────────────────────────────────

const loadingIds = reactive(new Set<string>())
const rejectReasonMap = reactive(new Map<string, string>())

const toast = useToast()

async function approve(id: string) {
  loadingIds.add(id)
  try {
    await api(`/submissions/${id}/approve`, { method: 'POST' })
    pendingSubmissions.value = pendingSubmissions.value?.filter(s => s.id !== id) ?? []
    toast.add({ title: 'Approved', color: 'success' })
  }
  catch (err) {
    toast.add({
      title: 'Approval failed',
      description: err instanceof Error ? err.message : 'Unexpected error.',
      color: 'error'
    })
  }
  finally {
    loadingIds.delete(id)
  }
}

async function reject(id: string) {
  const code = rejectReasonMap.get(id)
  if (!code) {
    toast.add({ title: 'Select a rejection reason first', color: 'warning' })
    return
  }
  loadingIds.add(id)
  try {
    await api(`/submissions/${id}/reject`, {
      method: 'POST',
      body: { rejectionReasonCode: code }
    })
    pendingSubmissions.value = pendingSubmissions.value?.filter(s => s.id !== id) ?? []
    toast.add({ title: 'Rejected', color: 'neutral' })
  }
  catch (err) {
    toast.add({
      title: 'Rejection failed',
      description: err instanceof Error ? err.message : 'Unexpected error.',
      color: 'error'
    })
  }
  finally {
    loadingIds.delete(id)
  }
}

// ── Edit slideover ────────────────────────────────────────────────────────────

const editOpen = ref(false)
const editTarget = ref<PendingSubmissionResponse | null>(null)
const editLoading = ref(false)
const editError = ref<string | null>(null)

interface EditForm {
  newGenreName: string
  description: string
  isSensitive: boolean
  sensitiveDescription: string
  playlistLink: string
  startDate: string
  endDate: string
  aliases: string
  sourceLinks: string
  countryIds: string[]
  instrumentIds: string[]
  similarGenreIds: string[]
  subGenreIds: string[]
  predecessorGenreIds: string[]
}

const editForm = ref<EditForm>({
  newGenreName: '',
  description: '',
  isSensitive: false,
  sensitiveDescription: '',
  playlistLink: '',
  startDate: '',
  endDate: '',
  aliases: '',
  sourceLinks: '',
  countryIds: [],
  instrumentIds: [],
  similarGenreIds: [],
  subGenreIds: [],
  predecessorGenreIds: []
})

function openEdit(submission: PendingSubmissionResponse) {
  editTarget.value = submission
  editForm.value = {
    newGenreName: submission.newGenreName ?? '',
    description: submission.description ?? '',
    isSensitive: submission.isSensitive,
    sensitiveDescription: submission.sensitiveDescription ?? '',
    playlistLink: submission.playlistLink ?? '',
    startDate: submission.startDate ?? '',
    endDate: submission.endDate ?? '',
    aliases: submission.aliases.join('\n'),
    sourceLinks: submission.sourceLinks.join('\n'),
    countryIds: [...submission.countryIds],
    instrumentIds: [...submission.instrumentIds],
    similarGenreIds: [...submission.similarGenreIds],
    subGenreIds: [...submission.subGenreIds],
    predecessorGenreIds: [...submission.predecessorGenreIds]
  }
  editError.value = null
  editOpen.value = true
}

async function saveEdit() {
  const target = editTarget.value
  if (!target) return
  editLoading.value = true
  editError.value = null
  try {
    const aliasLines = editForm.value.aliases.split('\n').map(s => s.trim()).filter(Boolean)
    const sourceLines = editForm.value.sourceLinks.split('\n').map(s => s.trim()).filter(Boolean)

    await api(`/submissions/${target.id}`, {
      method: 'PUT',
      body: {
        newGenreName: editForm.value.newGenreName,
        description: editForm.value.description,
        isSensitive: editForm.value.isSensitive,
        sensitiveDescription: editForm.value.isSensitive ? editForm.value.sensitiveDescription : '',
        playlistLink: editForm.value.playlistLink || null,
        startDate: editForm.value.startDate || null,
        endDate: editForm.value.endDate || null,
        aliases: aliasLines,
        sourceLinks: sourceLines,
        countryIds: editForm.value.countryIds,
        instrumentIds: editForm.value.instrumentIds,
        similarGenreIds: editForm.value.similarGenreIds,
        subGenreIds: editForm.value.subGenreIds,
        predecessorGenreIds: editForm.value.predecessorGenreIds
      }
    })

    // Patch the local list in-place using map to avoid index mutation
    const patches = {
      newGenreName: editForm.value.newGenreName,
      description: editForm.value.description,
      isSensitive: editForm.value.isSensitive,
      sensitiveDescription: editForm.value.sensitiveDescription,
      playlistLink: editForm.value.playlistLink,
      startDate: editForm.value.startDate || null,
      endDate: editForm.value.endDate || null,
      aliases: aliasLines,
      sourceLinks: sourceLines,
      countryIds: editForm.value.countryIds,
      instrumentIds: editForm.value.instrumentIds,
      similarGenreIds: editForm.value.similarGenreIds,
      subGenreIds: editForm.value.subGenreIds,
      predecessorGenreIds: editForm.value.predecessorGenreIds
    }

    pendingSubmissions.value = (pendingSubmissions.value ?? []).map(s =>
      s.id === target.id ? { ...s, ...patches } : s
    )

    toast.add({ title: 'Submission updated', color: 'success' })
    editOpen.value = false
  }
  catch (err) {
    editError.value = err instanceof Error ? err.message : 'Failed to save changes.'
  }
  finally {
    editLoading.value = false
  }
}

// ── Select-menu options ───────────────────────────────────────────────────────

const countryOptions = computed(() =>
  (countriesData.value ?? []).map(c => ({ label: c.name, value: c.id }))
)

const genreOptions = computed(() =>
  (genresData.value ?? []).map(g => ({ label: g.name, value: g.id }))
)

const instrumentOptions = computed(() =>
  (instrumentData.value ?? []).map(i => ({ label: i.type, value: i.id }))
)

// ── Breadcrumbs ───────────────────────────────────────────────────────────────

const breadcrumbItems = [
  { label: 'Explore', to: '/' },
  { label: 'Admin', to: '/admin' },
  { label: 'Submissions', to: '/admin/pending-submissions', active: true }
]
</script>

<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">

      <!-- HERO ────────────────────────────────────────────────────────────── -->
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
                Submission queue
              </h1>
              <p class="max-w-[40rem] text-sm text-[#7a84a8]">
                Review, edit, approve, or reject genre submissions from contributors.
              </p>
            </div>

            <p class="font-mono text-[11px] uppercase tracking-[0.18em] text-[#373d5a]">
              {{ visibleSubmissions.length }} of {{ pendingSubmissions?.length ?? 0 }} pending shown
            </p>
          </div>
        </div>
      </section>

      <!-- QUEUE ───────────────────────────────────────────────────────────── -->
      <section class="mx-auto flex max-w-[1200px] flex-col gap-6 px-6 py-8 sm:px-10 lg:py-10">

        <!-- Search + refresh -->
        <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <UInput
            v-model="search"
            placeholder="Search by genre name"
            icon="i-lucide-search"
            class="w-full sm:max-w-sm"
          />
          <UButton
            icon="i-lucide-refresh-cw"
            color="neutral"
            variant="outline"
            :loading="queueLoading"
            :disabled="queueLoading"
            aria-label="Refresh queue"
            @click="refreshQueue()"
          />
        </div>

        <!-- Error state -->
        <UAlert
          v-if="queueError"
          color="error"
          variant="soft"
          title="Could not load submissions"
          :description="queueError.message || 'Unknown error'"
        >
          <template #actions>
            <UButton color="neutral" variant="outline" size="xs" @click="refreshQueue()">
              Retry
            </UButton>
          </template>
        </UAlert>

        <!-- Empty state -->
        <div
          v-else-if="!queueLoading && visibleSubmissions.length === 0"
          class="rounded-md border border-border bg-surface px-6 py-12 text-center"
        >
          <p class="text-sm text-[#6f789b]">
            {{ search ? 'No submissions match your search.' : 'No pending submissions.' }}
          </p>
        </div>

        <!-- Submission list -->
        <div v-else class="flex flex-col gap-3">
          <UCollapsible
            v-for="item in visibleSubmissions"
            :key="item.id"
            class="overflow-hidden rounded-md border border-border bg-surface"
          >
            <!-- Header row -->
            <UButton
              variant="ghost"
              block
              :ui="{ base: 'rounded-none px-5 py-4 hover:bg-surface-2' }"
            >
              <div class="flex w-full items-center justify-between gap-4">
                <div class="min-w-0 flex-1">
                  <p class="truncate font-semibold text-space-50">
                    {{ item.newGenreName }}
                  </p>
                  <p class="mt-0.5 font-mono text-[11px] text-[#7a84a8]">
                    @{{ item.accountUsername }} · {{ formatDate(item.submittedAt) }}
                  </p>
                </div>

                <div class="flex shrink-0 items-center gap-2">
                  <div class="hidden flex-wrap justify-end gap-1 sm:flex">
                    <UBadge
                      v-for="name in getCountryNames(item.countryIds).slice(0, 3)"
                      :key="name"
                      variant="subtle"
                      color="neutral"
                      size="xs"
                      class="border border-border text-[#8ddbe6]"
                    >
                      {{ name }}
                    </UBadge>
                    <UBadge
                      v-if="getCountryNames(item.countryIds).length > 3"
                      variant="subtle"
                      color="neutral"
                      size="xs"
                      class="border border-border text-[#7a84a8]"
                    >
                      +{{ getCountryNames(item.countryIds).length - 3 }}
                    </UBadge>
                  </div>
                  <UIcon name="i-lucide-chevron-down" class="h-4 w-4 text-[#7a84a8] transition-transform group-data-[state=open]:rotate-180" />
                </div>
              </div>
            </UButton>

            <!-- Expanded content -->
            <template #content>
              <div class="border-t border-border">
                <div class="grid gap-6 p-5 lg:grid-cols-[minmax(0,1fr)_280px]">

                  <!-- Left: details -->
                  <div class="space-y-4">
                    <div class="grid gap-3 sm:grid-cols-2">
                      <div>
                        <p class="mb-1 font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Description</p>
                        <p class="text-sm leading-relaxed text-[#b9c6df]">{{ item.description || '—' }}</p>
                      </div>

                      <div class="space-y-3">
                        <div>
                          <p class="mb-1 font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Countries</p>
                          <p class="text-sm text-space-50">{{ getCountryNames(item.countryIds).join(', ') || '—' }}</p>
                        </div>
                        <div>
                          <p class="mb-1 font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Instruments</p>
                          <p class="text-sm text-space-50">{{ getInstrumentNames(item.instrumentIds).join(', ') || '—' }}</p>
                        </div>
                      </div>
                    </div>

                    <div class="grid gap-3 sm:grid-cols-3">
                      <div>
                        <p class="mb-1 font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Similar genres</p>
                        <p class="text-sm text-space-50">{{ getGenreNames(item.similarGenreIds).join(', ') || '—' }}</p>
                      </div>
                      <div>
                        <p class="mb-1 font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Subgenres</p>
                        <p class="text-sm text-space-50">{{ getGenreNames(item.subGenreIds).join(', ') || '—' }}</p>
                      </div>
                      <div>
                        <p class="mb-1 font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Predecessors</p>
                        <p class="text-sm text-space-50">{{ getGenreNames(item.predecessorGenreIds).join(', ') || '—' }}</p>
                      </div>
                    </div>

                    <div class="grid gap-3 sm:grid-cols-2">
                      <div v-if="item.aliases.length">
                        <p class="mb-1 font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Aliases</p>
                        <p class="text-sm text-space-50">{{ item.aliases.join(', ') }}</p>
                      </div>
                      <div v-if="item.playlistLink">
                        <p class="mb-1 font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Playlist</p>
                        <a
                          :href="item.playlistLink"
                          target="_blank"
                          rel="noopener"
                          class="truncate text-sm text-aurora hover:opacity-70"
                        >
                          {{ item.playlistLink }}
                        </a>
                      </div>
                    </div>

                    <div v-if="item.sourceLinks.length">
                      <p class="mb-1 font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Sources</p>
                      <ul class="space-y-0.5">
                        <li v-for="link in item.sourceLinks" :key="link">
                          <a
                            :href="link"
                            target="_blank"
                            rel="noopener"
                            class="truncate text-sm text-aurora hover:opacity-70"
                          >
                            {{ link }}
                          </a>
                        </li>
                      </ul>
                    </div>

                    <div v-if="item.isSensitive" class="rounded border border-[#ff6b6b]/20 bg-[#ff6b6b]/5 px-3 py-2">
                      <p class="font-mono text-[10px] uppercase tracking-[0.15em] text-[#ff6b6b]">Sensitive content</p>
                      <p class="mt-1 text-sm text-[#b9c6df]">{{ item.sensitiveDescription || '—' }}</p>
                    </div>
                  </div>

                  <!-- Right: actions -->
                  <div class="flex flex-col gap-3 lg:border-l lg:border-border lg:pl-6">
                    <p class="font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Actions</p>

                    <UButton
                      color="primary"
                      variant="solid"
                      block
                      :loading="loadingIds.has(item.id)"
                      @click="approve(item.id)"
                    >
                      Approve
                    </UButton>

                    <UButton
                      color="neutral"
                      variant="outline"
                      block
                      @click="openEdit(item)"
                    >
                      Edit before approving
                    </UButton>

                    <div class="space-y-2 rounded-md border border-border bg-surface-2 p-3">
                      <p class="font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Reject</p>
                      <USelectMenu
                        :model-value="rejectReasonMap.get(item.id)"
                        :items="rejectionReasonOptions"
                        value-key="value"
                        placeholder="Choose reason…"
                        class="w-full"
                        @update:model-value="(v) => rejectReasonMap.set(item.id, v as string)"
                      />
                      <UButton
                        color="error"
                        variant="soft"
                        block
                        :disabled="!rejectReasonMap.get(item.id)"
                        :loading="loadingIds.has(item.id)"
                        @click="reject(item.id)"
                      >
                        Confirm reject
                      </UButton>
                    </div>

                    <p class="font-mono text-[10px] text-[#373d5a]">
                      ID: {{ item.id }}
                    </p>
                  </div>

                </div>
              </div>
            </template>
          </UCollapsible>
        </div>

      </section>
    </UContainer>

    <!-- EDIT SLIDEOVER ───────────────────────────────────────────────────── -->
    <USlideover
      :open="editOpen"
      :ui="{
        overlay: 'z-[120]',
        content: 'z-[121] border-l border-border bg-bg text-space-50 shadow-2xl shadow-black/40 w-full sm:max-w-xl',
        body: 'p-0'
      }"
      @update:open="(v) => !v && (editOpen = false)"
    >
      <template #body>
        <div class="flex h-full flex-col">
          <!-- Slideover header -->
          <div class="flex items-center justify-between border-b border-border px-6 py-4">
            <div>
              <p class="font-mono text-[10px] uppercase tracking-[0.15em] text-[#4a6070]">Edit submission</p>
              <p class="mt-0.5 font-semibold text-space-50">{{ editForm.newGenreName || 'Unnamed genre' }}</p>
            </div>
            <UButton icon="i-heroicons-x-mark" variant="ghost" color="primary" @click="editOpen = false" />
          </div>

          <!-- Form -->
          <div class="flex-1 overflow-y-auto px-6 py-5">
            <div class="space-y-5">
              <UAlert
                v-if="editError"
                color="error"
                variant="soft"
                title="Save failed"
                :description="editError"
              />

              <!-- Genre name -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Genre name
                </label>
                <UInput v-model="editForm.newGenreName" class="w-full" />
              </div>

              <!-- Description -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Description
                </label>
                <UTextarea v-model="editForm.description" :rows="5" class="w-full" />
              </div>

              <!-- Dates -->
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                    Start date
                  </label>
                  <UInput v-model="editForm.startDate" placeholder="YYYY-MM-DD" class="w-full" />
                </div>
                <div>
                  <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                    End date
                  </label>
                  <UInput v-model="editForm.endDate" placeholder="YYYY-MM-DD" class="w-full" />
                </div>
              </div>

              <!-- Countries -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Countries
                </label>
                <USelectMenu
                  v-model="editForm.countryIds"
                  :items="countryOptions"
                  value-key="value"
                  multiple
                  class="w-full"
                />
              </div>

              <!-- Instruments -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Instruments
                </label>
                <USelectMenu
                  v-model="editForm.instrumentIds"
                  :items="instrumentOptions"
                  value-key="value"
                  multiple
                  class="w-full"
                />
              </div>

              <!-- Similar genres -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Similar genres
                </label>
                <USelectMenu
                  v-model="editForm.similarGenreIds"
                  :items="genreOptions"
                  value-key="value"
                  multiple
                  class="w-full"
                />
              </div>

              <!-- Subgenres -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Subgenres
                </label>
                <USelectMenu
                  v-model="editForm.subGenreIds"
                  :items="genreOptions"
                  value-key="value"
                  multiple
                  class="w-full"
                />
              </div>

              <!-- Predecessor genres -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Predecessor genres
                </label>
                <USelectMenu
                  v-model="editForm.predecessorGenreIds"
                  :items="genreOptions"
                  value-key="value"
                  multiple
                  class="w-full"
                />
              </div>

              <!-- Playlist link -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Playlist link
                </label>
                <UInput v-model="editForm.playlistLink" placeholder="https://…" class="w-full" />
              </div>

              <!-- Aliases -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Aliases <span class="normal-case tracking-normal text-[#4a6070]">(one per line)</span>
                </label>
                <UTextarea v-model="editForm.aliases" :rows="3" class="w-full" />
              </div>

              <!-- Source links -->
              <div>
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Source links <span class="normal-case tracking-normal text-[#4a6070]">(one per line)</span>
                </label>
                <UTextarea v-model="editForm.sourceLinks" :rows="3" class="w-full" />
              </div>

              <!-- Sensitive toggle -->
              <div class="flex items-center justify-between rounded-md border border-border bg-surface-2 px-4 py-3">
                <div>
                  <p class="text-sm font-medium text-space-50">Sensitive content</p>
                  <p class="text-xs text-[#7a84a8]">Flag this submission as culturally sensitive</p>
                </div>
                <UToggle v-model="editForm.isSensitive" />
              </div>

              <div v-if="editForm.isSensitive">
                <label class="mb-1.5 block font-mono text-[10px] uppercase tracking-[0.15em] text-[#7a84a8]">
                  Sensitivity description
                </label>
                <UTextarea v-model="editForm.sensitiveDescription" :rows="3" class="w-full" />
              </div>
            </div>
          </div>

          <!-- Footer -->
          <div class="border-t border-border px-6 py-4">
            <div class="flex gap-3">
              <UButton
                color="primary"
                variant="solid"
                class="flex-1"
                :loading="editLoading"
                @click="saveEdit"
              >
                Save changes
              </UButton>
              <UButton
                color="neutral"
                variant="outline"
                :disabled="editLoading"
                @click="editOpen = false"
              >
                Cancel
              </UButton>
            </div>
          </div>
        </div>
      </template>
    </USlideover>
  </div>
</template>
