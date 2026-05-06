<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useHead, useAsyncData } from '#imports'
import { useApi } from '@/composables/useApi'
import { useAuth } from '@/composables/useAuth'
import type { AdminUserRow, AdminUserRole } from '~/types/admin'

// Page is gated by middleware/admin.ts; backend re-checks the role.
definePageMeta({ middleware: 'admin' })

useHead({ title: 'Users | Audio Atlas Admin' })

// data fetch
const { api } = useApi()
const { user: currentUser } = useAuth()

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
  { label: 'Contributor', value: 'Contributor' }
] as const

// role-change dropdown — same items, no "All" option
const assignableRoleOptions = [
  { label: 'Admin', value: 'Admin' },
  { label: 'Curator', value: 'Curator' },
  { label: 'Banned', value: 'Banned' },
  { label: 'Contributor', value: 'Contributor' }
] as const

// sort state
type SortKey = 'memberSince' | 'role'
type SortDir = 'asc' | 'desc'

const sortKey = ref<SortKey>('memberSince')
const sortDir = ref<SortDir>('desc')

// Role sort priority — matches the backend's display priority so
// "asc" puts admins on top.
const ROLE_PRIORITY: Record<AdminUserRole, number> = {
  Admin:       0,
  Banned:      1,
  Curator:     2,
  Contributor: 3
}

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

  rows = [...rows].sort((a, b) => {
    const dir = sortDir.value === 'asc' ? 1 : -1

    if (sortKey.value === 'role') {
      return (ROLE_PRIORITY[a.role] - ROLE_PRIORITY[b.role]) * dir
    }

    // memberSince — null sinks to bottom regardless of direction
    if (!a.memberSince && !b.memberSince) return 0
    if (!a.memberSince) return 1
    if (!b.memberSince) return -1
    return a.memberSince.localeCompare(b.memberSince) * dir
  })

  return rows
})

// pagination — client-side, 100 rows per page
const PAGE_SIZE = 100
const currentPage = ref(1)

const totalPages = computed(() =>
  Math.max(1, Math.ceil(visibleUsers.value.length / PAGE_SIZE))
)

const pagedUsers = computed(() => {
  const start = (currentPage.value - 1) * PAGE_SIZE
  return visibleUsers.value.slice(start, start + PAGE_SIZE)
})

watch([search, roleFilter, sortKey, sortDir], () => {
  currentPage.value = 1
})

watch(totalPages, (n) => {
  if (currentPage.value > n) currentPage.value = n
})

function goPrev() {
  if (currentPage.value > 1) currentPage.value -= 1
}

function goNext() {
  if (currentPage.value < totalPages.value) currentPage.value += 1
}

// role change
const changingId = ref<string | null>(null)
const rowErrors = ref<Record<string, string>>({})

async function changeRole(row: AdminUserRow, newRole: AdminUserRole) {
  if (newRole === row.role) return
  if (changingId.value) return

  // Self-demote guard mirrors the backend 409. Belt-and-suspenders.
  if (currentUser.value?.userId === row.id && newRole !== 'Admin' && row.role === 'Admin') {
    rowErrors.value[row.id] = 'You cannot demote yourself.'
    return
  }

  changingId.value = row.id
  delete rowErrors.value[row.id]

  const previous = row.role
  row.role = newRole // optimistic

  try {
    await api(`/admin/users/${row.id}/role`, {
      method: 'PUT',
      body: { role: newRole }
    })
  } catch (err: any) {
    row.role = previous // revert
    rowErrors.value[row.id] =
      err?.data?.message
      ?? err?.message
      ?? 'Failed to change role.'
  } finally {
    changingId.value = null
  }
}

// signup stat — counts users joined in the last 30 days based on
// memberSince. Null memberSince doesn't count.
const THIRTY_DAYS_MS = 30 * 24 * 60 * 60 * 1000

const signupsLast30Days = computed(() => {
  const cutoff = Date.now() - THIRTY_DAYS_MS
  return users.value.filter((u) => {
    if (!u.memberSince) return false
    return new Date(u.memberSince).getTime() >= cutoff
  }).length
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

function isSelfRow(row: AdminUserRow) {
  return currentUser.value?.userId === row.id
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

      <!-- hero -->
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
              · {{ signupsLast30Days }} new in last 30 days
            </p>
          </div>

        </div>
      </section>

      <!-- Filter and table -->
      <section class="mx-auto flex max-w-[1200px] flex-col gap-6 px-6 py-8 sm:px-10 lg:py-10">

        <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <UInput
            v-model="search"
            placeholder="Search by username"
            icon="i-lucide-search"
            class="w-full sm:max-w-sm"
          />

          <div class="flex items-center gap-2">
            <USelectMenu
              v-model="roleFilter"
              :items="roleOptions"
              value-key="value"
              class="w-full sm:w-48"
            />

            <UButton
              icon="i-lucide-refresh-cw"
              color="neutral"
              variant="outline"
              :loading="pending"
              :disabled="pending"
              aria-label="Refresh users"
              @click="refresh()"
            />
          </div>
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
                <th
                  class="cursor-pointer select-none px-4 py-3 font-mono text-[10px] uppercase tracking-[0.18em] text-[#7a84a8] hover:text-aurora"
                  @click="toggleSort('role')"
                >
                  Role {{ sortIndicator('role') }}
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
                v-for="row in pagedUsers"
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
                  <div class="flex flex-col gap-1">
                <USelectMenu
                  :model-value="row.role"
                :items="assignableRoleOptions"
                value-key="value"
                :disabled="isSelfRow(row) || changingId === row.id"
                :loading="changingId === row.id"
                :ui="{
                  base: `
                  inline-flex items-center gap-1.5
                  rounded-full border
                  px-2.5 py-1

                  text-[13px] font-normal
                  whitespace-nowrap

                  transition-colors
                  ${roleBadgeClass(row.role)}
                `,
                trailingIcon: 'i-heroicons-chevron-down-20-solid'
                }"
                class="inline-flex w-fit max-w-[140px]"
                @update:model-value="(v) => changeRole(row, v as AdminUserRole)"
              />

                    <p
                      v-if="rowErrors[row.id]"
                      class="text-[11px] text-[#ff6b6b]"
                    >
                      {{ rowErrors[row.id] }}
                    </p>
                  </div>
                </td>
                <td class="px-4 py-3 text-[13px] text-[#7a84a8]">
                  {{ formatDate(row.memberSince) }}
                </td>
              </tr>

              <tr v-if="!pagedUsers.length">
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

        <!-- pagination -->
        <div
          v-if="totalPages > 1"
          class="flex items-center justify-between gap-3 font-mono text-[11px] uppercase tracking-[0.18em] text-[#7a84a8]"
        >
          <span>
            Page {{ currentPage }} of {{ totalPages }}
          </span>

          <div class="flex items-center gap-2">
            <UButton
              color="neutral"
              variant="outline"
              size="xs"
              :disabled="currentPage === 1"
              @click="goPrev"
            >
              Previous
            </UButton>
            <UButton
              color="neutral"
              variant="outline"
              size="xs"
              :disabled="currentPage === totalPages"
              @click="goNext"
            >
              Next
            </UButton>
          </div>
        </div>

      </section>

    </UContainer>
  </div>
</template>
