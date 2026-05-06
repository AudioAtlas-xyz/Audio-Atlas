<script setup lang="ts">
import { computed, ref } from 'vue'
import { useHead, useAsyncData } from '#imports'
import { useApi } from '@/composables/useApi'
import type { AdminUserRow, AdminUserRole } from '~/types/admin'

// Page is gated by middleware/admin.ts; backend re-checks the role.
definePageMeta({ middleware: 'admin' })

useHead({ title: 'Users | Audio Atlas Admin' })

// data fetch
const { api } = useApi()

const {
  data: usersData,
  pending,
  error,
  refresh
} = await useAsyncData<AdminUserRow[]>(
  'admin-users',
  () => api<AdminUserRow[]>('/admin/users'),
  { default: () => [] }
)

const users = computed<AdminUserRow[]>(() => usersData.value ?? [])

// filter state
const search = ref('')
const roleFilter = ref<'All' | AdminUserRole>('All')

const roleOptions = [
  { label: 'All roles', value: 'All' },
  { label: 'Admin', value: 'Admin' },
  { label: 'Curator', value: 'Curator' },
  { label: 'Banned', value: 'Banned' },
  { label: 'Member', value: 'Member' }
] as const

// sort state — only memberSince is sortable for now
type SortKey = 'memberSince'
type SortDir = 'asc' | 'desc'

const sortKey = ref<SortKey>('memberSince')
const sortDir = ref<SortDir>('desc')

function toggleSort(key: SortKey) {
  if (sortKey.value === key) {
    sortDir.value = sortDir.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortKey.value = key
    sortDir.value = 'desc'
  }
}

// filter + sort
const visibleUsers = computed(() => {
  const q = search.value.trim().toLowerCase()
  const role = roleFilter.value

  // username-only search; emails are intentionally excluded
  let rows = users.value.filter((u) => {
    if (role !== 'All' && u.role !== role) return false
    if (q && !u.username.toLowerCase().includes(q)) return false
    return true
  })

  // null memberSince sinks to bottom
  rows = [...rows].sort((a, b) => {
    const dir = sortDir.value === 'asc' ? 1 : -1

    if (!a.memberSince && !b.memberSince) return 0
    if (!a.memberSince) return 1
    if (!b.memberSince) return -1
    return a.memberSince.localeCompare(b.memberSince) * dir
  })

  return rows
})

// helpers
const dateFormatter = new Intl.DateTimeFormat('en-US', {
  month: 'short',
  day: 'numeric',
  year: 'numeric'
})

function formatDate(iso: string | null | undefined): string {
  if (!iso) return '—'
  return dateFormatter.format(new Date(iso))
}

function roleBadgeClass(role: AdminUserRole): string {
  switch (role) {
    case 'Admin':   return 'border-[#3DE8C8] text-[#3DE8C8]'
    case 'Curator': return 'border-[#8ddbe6] text-[#8ddbe6]'
    case 'Banned':  return 'border-[#ff6b6b] text-[#ff6b6b]'
    default:        return 'border-[#7a84a8] text-[#7a84a8]'
  }
}

function sortIndicator(key: SortKey): string {
  if (sortKey.value !== key) return ''
  return sortDir.value === 'asc' ? '↑' : '↓'
}

const breadcrumbItems = [
  { label: 'Explore', to: '/' },
  { label: 'Admin', to: '/admin' },
  { label: 'Users', to: '/admin/users', active: true }
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
                Registered users
              </h1>

              <p class="max-w-[40rem] text-sm text-[#7a84a8]">
                Browse every account on Audio Atlas. Filter by role to spot
                active contributors, recent signups, or moderation cases.
              </p>
            </div>

            <p class="font-mono text-[11px] uppercase tracking-[0.18em] text-[#373d5a]">
              {{ visibleUsers.length }} of {{ users.length }} users shown
            </p>
          </div>

        </div>
      </section>

      <!-- FILTERS + TABLE -->
      <section class="mx-auto flex max-w-[1200px] flex-col gap-6 px-6 py-8 sm:px-10 lg:py-10">

        <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <UInput
            v-model="search"
            placeholder="Search by username"
            icon="i-lucide-search"
            class="w-full sm:max-w-sm"
          />

          <USelectMenu
            v-model="roleFilter"
            :items="roleOptions"
            value-key="value"
            class="w-full sm:w-48"
          />
        </div>

        <UAlert
          v-if="error"
          color="error"
          variant="soft"
          title="Could not load users"
          :description="error.message || 'Unknown error'"
          :ui="{ actions: 'mt-2' }"
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

        <div
          v-else
          class="overflow-x-auto rounded-md border border-border bg-surface"
        >
          <table class="w-full border-collapse">
            <thead class="bg-surface-2">
              <tr class="text-left">
                <th class="px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-[#7a84a8]">
                  Username
                </th>
                <th class="px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-[#7a84a8]">
                  Email
                </th>
                <th class="px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-[#7a84a8]">
                  Role
                </th>
                <th
                  class="cursor-pointer select-none px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-[#7a84a8] hover:text-aurora"
                  @click="toggleSort('memberSince')"
                >
                  Member since {{ sortIndicator('memberSince') }}
                </th>
              </tr>
            </thead>

            <tbody>
              <tr
                v-for="row in visibleUsers"
                :key="row.id"
                class="border-t border-border transition hover:bg-surface-2"
              >
                <td class="px-4 py-3 font-mono text-[12px] tracking-[0.06em] text-aurora">
                  @{{ row.username }}
                </td>
                <td class="px-4 py-3 text-[13px] text-space-50">
                  {{ row.email }}
                </td>
                <td class="px-4 py-3">
                  <UBadge
                    color="neutral"
                    variant="outline"
                    :class="`rounded-full px-2 py-0.5 text-[10px] font-medium uppercase tracking-[0.18em] ${roleBadgeClass(row.role)}`"
                  >
                    {{ row.role }}
                  </UBadge>
                </td>
                <td class="px-4 py-3 text-[13px] text-[#7a84a8]">
                  {{ formatDate(row.memberSince) }}
                </td>
              </tr>

              <tr v-if="!visibleUsers.length">
                <td colspan="4" class="px-4 py-10 text-center text-sm text-[#6f789b]">
                  <template v-if="pending">
                    Loading users…
                  </template>
                  <template v-else-if="users.length === 0">
                    No users to show yet.
                  </template>
                  <template v-else>
                    No users match the current filters.
                  </template>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

      </section>

    </UContainer>
  </div>
</template>
