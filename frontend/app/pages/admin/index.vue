<script setup lang="ts">
import { computed } from 'vue'
import { useHead } from '#imports'
import { useAuth } from '@/composables/useAuth'

/**
 * Admin landing page.
 *
 * Gated by the `admin` route middleware — non-admins are redirected
 * to `/` before this component ever mounts. Backend endpoints called
 * from anywhere under /admin must additionally be protected with
 * `[Authorize(Roles = "Admin")]` since middleware is client-side only.
 */
definePageMeta({
  middleware: 'admin'
})

useHead({
  title: 'Admin | Audio Atlas'
})

const { user } = useAuth()

/**
 * Breadcrumbs — mirrors the pattern used on `pages/genres.vue` and
 * `pages/CountryPage.vue` so admins land in a familiar visual frame.
 */
const breadcrumbItems = [
  { label: 'Explore', to: '/' },
  { label: 'Admin', to: '/admin', active: true }
]

/**
 * Admin tools — each entry renders as a card linking to a sub-page.
 * Add new admin features here; the grid lays them out automatically.
 */
const adminTools = [
  {
    label: 'Users',
    title: 'Registered users',
    description:
      'Browse all registered accounts, filter by role, identify active contributors.',
    to: '/admin/users',
    icon: 'i-lucide-users'
  },
  {
    label: 'Submissions',
    title: 'Submission queue',
    description:
      'View a list of submissions submitted by contributors.',
    to: '/admin/pending-submissions',
    icon: 'i-lucide-clipboard-list'
  }
]

const greeting = computed(() =>
  user.value?.username ? `Signed in as ${user.value.username}` : 'Signed in'
)
</script>

<template>
  <div class="bg-bg text-space-50">
    <UContainer class="px-0 sm:px-0">

      <!-- HERO -->
      <section class="border-b border-border bg-bg">
        <div class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:py-10">

          <div class="space-y-6">
            <!-- BREADCRUMB -->
            <UBreadcrumb :items="breadcrumbItems">
              <template #item-label="{ item, active }">
                <span :class="active ? 'text-aurora font-semibold' : 'text-muted'">
                  {{ item.label }}
                </span>
              </template>
            </UBreadcrumb>

            <!-- TITLE + BADGES -->
            <div class="space-y-4">
              <h1 class="font-display text-5xl tracking-[-0.04em] text-space-50 sm:text-[52px]">
                Audio Atlas Admin
              </h1>

              <p class="max-w-[40rem] text-sm text-[#7a84a8]">
                Tools for monitoring community growth, identifying active
                contributors, and moderating content across the atlas.
              </p>
            </div>

            <!-- META ROW -->
            <p class="font-mono text-[11px] uppercase tracking-[0.18em] text-[#373d5a]">
              {{ greeting }}
            </p>
          </div>

        </div>
      </section>

      <!-- CONTENT -->
      <section class="mx-auto flex max-w-[1200px] flex-col gap-8 px-6 py-8 sm:px-10 lg:py-10">
        <div class="border-b border-border pb-4">
          <h2 class="font-display text-[28px] tracking-[-0.02em] text-space-50">
            Tools
          </h2>
          <p class="mt-2 text-xs text-[#373d5a]">
            {{ adminTools.length }} admin {{ adminTools.length === 1 ? 'tool' : 'tools' }} available
          </p>
        </div>

        <!-- TOOL GRID -->
        <div class="grid gap-5 md:grid-cols-2 lg:grid-cols-3">
          <NuxtLink
            v-for="tool in adminTools"
            :key="tool.to"
            :to="tool.to"
            class="group flex flex-col gap-3 rounded-md border border-border bg-surface p-5 transition hover:border-aurora/40"
          >
            <!-- ICON + EYEBROW -->
            <div class="flex items-center justify-between">
              <UIcon
                :name="tool.icon"
                class="h-5 w-5 text-aurora"
              />
              <span class="font-mono text-[10px] uppercase tracking-[0.18em] text-[#373d5a]">
                {{ tool.label }}
              </span>
            </div>

            <!-- BODY -->
            <div class="space-y-1.5">
              <p class="font-display text-lg tracking-[-0.01em] text-space-50">
                {{ tool.title }}
              </p>
              <p class="text-xs leading-relaxed text-[#7a84a8]">
                {{ tool.description }}
              </p>
            </div>
          </NuxtLink>
        </div>
      </section>

    </UContainer>
  </div>
</template>
