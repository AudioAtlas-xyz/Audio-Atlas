<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuth } from '@/composables/useAuth'

/**
 * Initialize user session on app load
 */
const { fetchUser } = useAuth()

onMounted(async () => {
  await fetchUser()
})
</script>

<template>
  <UApp>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </UApp>
</template>