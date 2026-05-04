<script setup lang="ts">
import type { Genre } from '~/types/genre'
import { useApi } from '@/composables/useApi'

const { api } = useApi()

const searchTerm = ref('')
const loading = ref(false)
const genres = ref<Genre[]>([])
const hasSearched = ref(false)

let timeout: ReturnType<typeof setTimeout> | null = null

watch(searchTerm, (value) => {
  if (timeout) clearTimeout(timeout)

  hasSearched.value = false

  if (!value || value.trim().length < 2) {
    genres.value = []
    loading.value = false
    return
  }

  timeout = setTimeout(async () => {
    loading.value = true

    try {
      genres.value = await api<Genre[]>(
        `/genres/search/${encodeURIComponent(value)}`
      )
    } catch (error) {
      console.error('Search failed:', error)
      genres.value = []
    } finally {
      loading.value = false
      hasSearched.value = true
    }
  }, 200)
})
</script>

<template>

<div class="search-wrapper">
    <UInput
    v-model="searchTerm"
    placeholder="Search genres..."
    icon="i-lucide-search"
    class="search-input"
    />

    <div
    v-if="genres.length > 0"
    class="search-dropdown"
    >
    <NuxtLink
        v-for="genre in genres"
        :key="genre.id"
        :to="`/genres?genreId=${genre.id}`"
        class="search-result"
    >
        

        <strong>{{ genre.name }} | {{ genre.countries?.[0]?.name || 'Unknown' }}</strong>
        <span>{{ genre.summary }}</span>
    </NuxtLink>
    </div>
    <div
    v-else-if="hasSearched && !loading && searchTerm.trim().length >= 2"
    class="search-dropdown"
    >
        <div class="search-result">
            <strong>No genres found</strong>
        </div>  
    </div>
</div>
</template>

<style>
.search-wrapper {
  position: relative;
  width: 18rem;
  flex-shrink: 0;
}

.search-input {
  width: 100%;
}

.search-dropdown {
  position: absolute;
  top: calc(100% + 0.5rem);
  left: 0;
  right: 0;
  z-index: 100;

  max-height: 24rem;
  overflow-y: auto;

  padding: 0.5rem;
  border-radius: 14px;
  border: 1px solid rgba(141, 219, 230, 0.18);
  background: rgba(4, 18, 22, 0.96);
  backdrop-filter: blur(16px);
}

.search-result {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;

  padding: 0.75rem;
  border-radius: 10px;

  color: #8ddbe6;
  text-decoration: none;
}

.search-result:hover {
  background: rgba(255, 255, 255, 0.06);
}

.search-result strong {
  color: #3DE8C8;
  font-size: 0.9rem;
}

.search-result span {
  color: rgba(221, 245, 248, 0.75);
  font-size: 0.8rem;
  line-height: 1.3;
}

</style>