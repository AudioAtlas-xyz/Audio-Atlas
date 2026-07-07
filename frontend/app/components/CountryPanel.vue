<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import type { Country } from '~/types/country'
import { useApi } from '@/composables/useApi'

const props = defineProps<{
  countryId: string
  open: boolean
}>()

const emit = defineEmits(['close'])

const { api } = useApi()

const countryPageHref = computed(() => `/CountryPage?countryId=${props.countryId}`)

const data = ref<Country | null>(null)
const pending = ref(false)
const error = ref<Error | null>(null)


const loadCountry = async () => {
  if (!props.countryId || !props.open) return

  pending.value = true
  error.value = null

  try {
    data.value = await api<Country>(`/countries/${props.countryId}`)
  } catch (caughtError: any) {
    data.value = null
    error.value =
      caughtError instanceof Error
        ? caughtError
        : new Error('Error loading country details.')
  } finally {
    pending.value = false
  }
}

watch(
  () => [props.open, props.countryId] as const,
  ([open, countryId]) => {
    if (open && countryId) {
      loadCountry()
    }
  },
  { immediate: true }
)

const errorMessage = computed(() => {
  if (!error.value) return ''
  return error.value.message || 'Error loading country details.'
})
</script>

<template>
  <USlideover
    :open="open"
    :close="false"
    :ui="{
      overlay: 'z-[120]',
      content: 'z-[121] border-l border-border bg-bg text-space-50 shadow-2xl shadow-black/40',
      body: 'p-0 bg-bg text-space-50'
    }"
    @update:open="(value) => !value && emit('close')"
  >
    <template #body>
      <div class="min-h-full bg-bg text-space-50">
      <!--  <div class="flex items-center justify-between border-b border-border bg-surface px-6 py-4">-->

        <!--</div>-->

        <div class="space-y-6 p-6">
          <div v-if="pending" class="space-y-4">
            <USkeleton class="h-9 w-40 rounded-md bg-surface-2" />
            <USkeleton class="h-20 w-full rounded-md bg-surface-2" />
            <USkeleton class="h-6 w-24 rounded-md bg-surface-2" />
            <USkeleton class="h-32 w-full rounded-md bg-surface-2" />
          </div>

          <UAlert
            v-else-if="error"
            color="error"
            variant="soft"
            title="Could not load country data"
            :description="errorMessage"
          />

          <div v-else-if="data" class="space-y-6">
            <section class="space-y-3">
              <div class="flex items-start justify-between gap-4">
                <NuxtLink :to="countryPageHref" class="text-3xl font-bold text-space-50 transition hover:text-aurora">
                  {{ data.name }}
                </NuxtLink>
                <UButton icon="i-heroicons-x-mark" variant="ghost" color="primary" @click="emit('close')" />
              </div>
              <div v-if="data.region || data.continent" class="flex flex-wrap gap-2">
                <NuxtLink
                  v-if="data.region"
                  :to="`/browse/${encodeURIComponent(data.region)}`"
                  class="transition-opacity hover:opacity-70"
                >
                  <UBadge variant="subtle" color="neutral" class="bg-black border-white text-white">
                    {{ data.region }}
                  </UBadge>
                </NuxtLink>
                <NuxtLink
                  v-if="data.continent"
                  :to="`/browse/${encodeURIComponent(data.continent)}`"
                  class="transition-opacity hover:opacity-70"
                >
                  <UBadge variant="subtle" color="neutral" class="bg-black border-white text-white">
                    {{ data.continent }}
                  </UBadge>
                </NuxtLink>
              </div>
              <p class="text-sm leading-6 text-[#b9c6df]">
                {{ data.description || 'No description has been added for this country yet.' }}
              </p>


            </section>

            <section class="space-y-3">
              <div class="flex items-center justify-between">
                <h4 class="text-lg font-semibold text-space-50">
                  Genres
                </h4>
                <span class="text-xs text-[#7f8aa8]">
                  {{ data.genres?.length ?? 0 }} total
                </span>
              </div>

              <UAlert
                v-if="!data.genres?.length"
                color="neutral"
                variant="soft"
                title="No genres yet"
                description="No genres have been added for this country yet."
              />

              <div v-else class="grid gap-3">
                <NuxtLink
                  v-for="genre in data.genres"
                  :key="genre.id"
                  :to="`/genres?genreId=${genre.id}`"
                  class="block transition-all duration-200 hover:-translate-y-0.5 hover:border-[#8ddbe6] hover:shadow-[0_0_15px_rgba(141,219,230,0.15)]"
                >
                <!--class="border border-border bg-surface shadow-none"-->

                <UCard class="border border-border bg-surface shadow-none transition-all duration-200 hover:border-aurora hover:text-aurora hover:bg-aurora-dim"

                >
                  <div class="space-y-2">
                    <h5 class="text-base font-semibold text-space-50">
                      {{ genre.name }}
                    </h5>
                    <p v-if="genre.summary || genre.description" class="text-sm leading-6 text-[#b9c6df]">
                      {{ genre.summary || genre.description }}
                    </p>

                  </div>
                </UCard>
                </NuxtLink>
              </div>
            </section>
          </div>
          <div class="absolute bottom-0 left-0 right-0 p-4 bg-bg border-t border-border">
            <NuxtLink :to="countryPageHref" class="block">
              <UButton
                block
                color="secondary"
                class="w-full bg-aurora text-bg"
              >
                Open Country Page →
              </UButton>
            </NuxtLink>
          </div>
        </div>
      </div>
    </template>
  </USlideover>
</template>
