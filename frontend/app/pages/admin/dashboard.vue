<script setup lang="ts">
import { reactive, computed, watch } from 'vue'
import { useHead, useAsyncData, useRoute, useRouter } from '#imports'
import { useApi } from '@/composables/useApi'
import { useAuth } from '@/composables/useAuth'
import type { Country } from '~/types/country'
import type { DashboardResponse } from '~/types/dashboard'

definePageMeta({ middleware: 'curator' })

useHead({ title: 'Metrics dashboard | Audio Atlas Admin' })

const { api } = useApi()
const { user } = useAuth()
const route = useRoute()
const router = useRouter()

// ── Filter state, initialised from URL ────────────────────────────────────────
const filters = reactive({
  continent: (route.query.continent as string | null | undefined) ?? '',
  region:    (route.query.region as string | null | undefined) ?? '',
  country:   (route.query.country as string | null | undefined) ?? '',
  role:      (route.query.role as string | null | undefined) ?? '',
  from:      (route.query.from as string | null | undefined) ?? '',
  to:        (route.query.to as string | null | undefined) ?? ''
})

// Cascade resets
watch(() => filters.continent, () => {
  filters.region = ''
  filters.country = ''
})
watch(() => filters.region, () => {
  filters.country = ''
})

// Sync filters → URL
watch(
  () => ({ ...filters }),
  (f) => {
    const q: Record<string, string> = {}
    if (f.continent) q.continent = f.continent
    if (f.region)    q.region    = f.region
    if (f.country)   q.country   = f.country
    if (f.role)      q.role      = f.role
    if (f.from)      q.from      = f.from
    if (f.to)        q.to        = f.to
    router.replace({ query: q })
  },
  { deep: true }
)

function resetFilters() {
  filters.continent = ''
  filters.region    = ''
  filters.country   = ''
  filters.role      = ''
  filters.from      = ''
  filters.to        = ''
}

const hasActiveFilters = computed(() =>
  Object.values(filters).some(v => v !== '')
)

// ── Countries data for geography cascade ──────────────────────────────────────
const { data: countriesData } = await useAsyncData<Country[]>(
  'countries-all',
  () => api<Country[]>('/countries/all'),
  { server: false, default: () => [] }
)

const continentOptions = computed(() => {
  const uniq = [...new Set(
    (countriesData.value ?? []).map(c => c.continent).filter(Boolean)
  )].sort()
  return [
    { label: 'All continents', value: '' },
    ...uniq.map(c => ({ label: c, value: c }))
  ]
})

const regionOptions = computed(() => {
  const base = [{ label: 'All regions', value: '' }]
  if (!filters.continent) return base
  const uniq = [...new Set(
    (countriesData.value ?? [])
      .filter(c => c.continent === filters.continent)
      .map(c => c.region)
      .filter(Boolean)
  )].sort()
  return [...base, ...uniq.map(r => ({ label: r, value: r }))]
})

const countryOptions = computed(() => {
  const base = [{ label: 'All countries', value: '' }]
  if (!filters.region) return base
  const names = (countriesData.value ?? [])
    .filter(c => c.region === filters.region)
    .map(c => c.name)
    .filter(Boolean)
    .sort()
  return [...base, ...names.map(n => ({ label: n, value: n }))]
})

const roleFilterOptions = [
  { label: 'All roles', value: '' },
  { label: 'Admin', value: 'Admin' },
  { label: 'Curator', value: 'Curator' },
  { label: 'Banned', value: 'Banned' },
  { label: 'Contributor', value: 'Contributor' }
]

// ── Dashboard data fetch ───────────────────────────────────────────────────────
const queryString = computed(() => {
  const p = new URLSearchParams()
  if (filters.continent) p.set('continent', filters.continent)
  if (filters.region)    p.set('region',    filters.region)
  if (filters.country)   p.set('country',   filters.country)
  if (filters.role)      p.set('role',      filters.role)
  if (filters.from)      p.set('from',      filters.from)
  if (filters.to)        p.set('to',        filters.to)
  const s = p.toString()
  return s ? `?${s}` : ''
})

const {
  data,
  pending,
  error,
  refresh
} = await useAsyncData<DashboardResponse>(
  'admin-dashboard',
  () => api<DashboardResponse>(`/admin/dashboard${queryString.value}`),
  {
    server: false,
    watch: [queryString],
    default: () => null as unknown as DashboardResponse
  }
)

// ── Headline stats ─────────────────────────────────────────────────────────────
const greeting = computed(() =>
  user.value?.username ? `Signed in as ${user.value.username}` : 'Signed in'
)

const dateFormatter = new Intl.DateTimeFormat('en-US', {
  month: 'short', day: 'numeric', year: 'numeric'
})

function formatDate(iso: string | null | undefined): string {
  if (!iso) return '—'
  return dateFormatter.format(new Date(iso))
}

const collectingSince = computed(() =>
  data.value?.earliestReviewAt
    ? `Data since ${formatDate(data.value.earliestReviewAt)}`
    : null
)

const breadcrumbItems = [
  { label: 'Explore', to: '/' },
  { label: 'Admin', to: '/admin' },
  { label: 'Metrics dashboard', to: '/admin/dashboard', active: true }
]
</script>

<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">

      <!-- HERO ────────────────────────────────────────────────────────────────── -->
      <section class="border-b border-border bg-bg">
        <div class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:py-10">
          <div class="space-y-6">
            <UBreadcrumb :items="breadcrumbItems">
              <template #item-label="{ item, active }">
                <span :class="active ? 'text-aurora font-semibold' : 'text-muted'">{{ item.label }}</span>
              </template>
            </UBreadcrumb>

            <div class="space-y-4">
              <h1 class="font-display text-5xl tracking-[-0.04em] text-space-50 sm:text-[52px]">
                Metrics dashboard
              </h1>
              <p class="max-w-[40rem] text-sm text-[#7a84a8]">
                Monitor catalogue coverage, the submission pipeline, community growth, and search demand.
              </p>
            </div>

            <p class="font-mono text-[11px] uppercase tracking-[0.18em] text-[#373d5a]">
              {{ greeting }}
              <template v-if="collectingSince"> · {{ collectingSince }}</template>
            </p>
          </div>
        </div>
      </section>

      <!-- CONTENT ─────────────────────────────────────────────────────────────── -->
      <section class="mx-auto flex max-w-[1200px] flex-col gap-12 px-6 py-8 sm:px-10 lg:py-10">

        <!-- FILTER BAR ──────────────────────────────────────────────────────────── -->
        <div class="space-y-3">
          <div class="flex flex-wrap items-end gap-3">
            <!-- Geography cascade -->
            <div class="flex flex-col gap-1">
              <label class="font-mono text-[10px] uppercase tracking-[0.1em] text-[#373d5a]">Continent</label>
              <USelectMenu
                v-model="filters.continent"
                :items="continentOptions"
                value-key="value"
                placeholder="All continents"
                class="w-44"
              />
            </div>

            <div class="flex flex-col gap-1">
              <label class="font-mono text-[10px] uppercase tracking-[0.1em] text-[#373d5a]">Region</label>
              <USelectMenu
                v-model="filters.region"
                :items="regionOptions"
                value-key="value"
                placeholder="All regions"
                :disabled="!filters.continent"
                class="w-48"
              />
            </div>

            <div class="flex flex-col gap-1">
              <label class="font-mono text-[10px] uppercase tracking-[0.1em] text-[#373d5a]">Country</label>
              <USelectMenu
                v-model="filters.country"
                :items="countryOptions"
                value-key="value"
                placeholder="All countries"
                :disabled="!filters.region"
                class="w-44"
              />
            </div>

            <!-- Role -->
            <div class="flex flex-col gap-1">
              <label class="font-mono text-[10px] uppercase tracking-[0.1em] text-[#373d5a]">Role</label>
              <USelectMenu
                v-model="filters.role"
                :items="roleFilterOptions"
                value-key="value"
                placeholder="All roles"
                class="w-36"
              />
            </div>

            <!-- Date range -->
            <div class="flex flex-col gap-1">
              <label class="font-mono text-[10px] uppercase tracking-[0.1em] text-[#373d5a]">From</label>
              <UInput
                v-model="filters.from"
                type="date"
                class="w-36"
                aria-label="Filter from date"
              />
            </div>

            <div class="flex flex-col gap-1">
              <label class="font-mono text-[10px] uppercase tracking-[0.1em] text-[#373d5a]">To</label>
              <UInput
                v-model="filters.to"
                type="date"
                class="w-36"
                aria-label="Filter to date"
              />
            </div>

            <!-- Actions -->
            <div class="flex items-end gap-2">
              <UButton
                color="neutral"
                variant="outline"
                :loading="pending"
                :disabled="pending"
                icon="i-lucide-refresh-cw"
                aria-label="Refresh dashboard data"
                @click="refresh()"
              />
              <UButton
                v-if="hasActiveFilters"
                color="neutral"
                variant="ghost"
                size="sm"
                icon="i-lucide-x"
                aria-label="Reset all filters"
                @click="resetFilters"
              >
                Reset
              </UButton>
            </div>
          </div>

          <p v-if="hasActiveFilters" class="font-mono text-[11px] text-[#373d5a]">
            Filters active — some panels are scoped to this selection
          </p>
        </div>

        <!-- LOADING ─────────────────────────────────────────────────────────────── -->
        <div v-if="pending && !data" class="flex items-center gap-3 py-16 text-[#7a84a8]">
          <UIcon name="i-lucide-loader-circle" class="h-5 w-5 animate-spin" aria-hidden="true" />
          <span class="text-sm">Loading dashboard data…</span>
        </div>

        <!-- ERROR ───────────────────────────────────────────────────────────────── -->
        <UAlert
          v-else-if="error && !data"
          color="error"
          variant="soft"
          title="Could not load dashboard"
          :description="(error as Error)?.message ?? 'Unknown error'"
        >
          <template #actions>
            <UButton
              color="neutral"
              variant="outline"
              size="xs"
              @click="refresh()"
            >
              Retry
            </UButton>
          </template>
        </UAlert>

        <!-- DASHBOARD DATA ───────────────────────────────────────────────────────── -->
        <template v-else-if="data">

          <!-- HEADLINE STAT CARDS ─────────────────────────────────────────────── -->
          <div
            class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3"
            aria-label="Headline metrics"
          >
            <!-- Total genres -->
            <div
              class="rounded-md border border-border bg-surface p-5"
              role="region"
              aria-label="Total genres in scope"
            >
              <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Total genres</p>
              <p class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50">
                {{ data.catalogue.totalGenres.toLocaleString() }}
              </p>
            </div>

            <!-- Queue depth -->
            <div
              class="rounded-md border border-border bg-surface p-5"
              role="region"
              aria-label="Pending submission queue depth"
            >
              <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Queue depth</p>
              <div class="mt-2 flex items-end gap-3">
                <p class="font-display text-3xl tracking-[-0.02em] text-space-50">
                  {{ data.pipeline.queueDepth.toLocaleString() }}
                </p>
                <span
                  v-if="data.pipeline.sensitivityHolds > 0"
                  class="mb-0.5 font-mono text-xs text-[#e060d8]"
                  :aria-label="`${data.pipeline.sensitivityHolds} sensitivity holds`"
                >
                  +{{ data.pipeline.sensitivityHolds }} holds
                </span>
              </div>
              <p class="mt-1 text-[11px] text-[#7a84a8]">pending submissions</p>
            </div>

            <!-- Approval rate -->
            <div
              class="rounded-md border border-border bg-surface p-5"
              role="region"
              :aria-label="data.pipeline.approvalRate != null ? `Approval rate: ${data.pipeline.approvalRate}%` : 'Approval rate not yet available'"
            >
              <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Approval rate</p>
              <p class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50">
                {{ data.pipeline.approvalRate != null ? `${data.pipeline.approvalRate}%` : '—' }}
              </p>
              <p class="mt-1 text-[11px] text-[#7a84a8]">approved vs reviewed</p>
            </div>

            <!-- Active contributors -->
            <div
              class="rounded-md border border-border bg-surface p-5"
              role="region"
              aria-label="Active contributors in the last 30 days"
            >
              <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Active contributors</p>
              <p class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50">
                {{ data.community.activeContributors.toLocaleString() }}
              </p>
              <p class="mt-1 text-[11px] text-[#7a84a8]">submitted in last 30 days</p>
            </div>

            <!-- New signups this month -->
            <div
              class="rounded-md border border-border bg-surface p-5"
              role="region"
              aria-label="New signups this month"
            >
              <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">New signups</p>
              <p class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50">
                {{ data.community.newSignupsThisMonth.toLocaleString() }}
              </p>
              <p class="mt-1 text-[11px] text-[#7a84a8]">this calendar month</p>
            </div>

            <!-- Country coverage -->
            <div
              class="rounded-md border border-border bg-surface p-5"
              role="region"
              aria-label="Country coverage percentage"
            >
              <p class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">Country coverage</p>
              <p class="mt-2 font-display text-3xl tracking-[-0.02em] text-space-50">
                {{
                  data.catalogue.countryCoverage.total
                    ? `${Math.round((data.catalogue.countryCoverage.withGenres / data.catalogue.countryCoverage.total) * 100)}%`
                    : '—'
                }}
              </p>
              <p class="mt-1 text-[11px] text-[#7a84a8]">
                {{ data.catalogue.countryCoverage.withGenres }} / {{ data.catalogue.countryCoverage.total }} countries
              </p>
            </div>
          </div>

          <!-- PANELS ──────────────────────────────────────────────────────────── -->
          <AdminDashboardCataloguePanel :data="data.catalogue" />

          <AdminDashboardPipelinePanel
            :data="data.pipeline"
            :earliest-review-at="data.earliestReviewAt"
          />

          <AdminDashboardCommunityPanel :data="data.community" />

          <AdminDashboardDiscoveryPanel :data="data.discovery" />

          <AdminDashboardCostPanel :data="data.cost" />

        </template>

      </section>
    </UContainer>
  </div>
</template>
