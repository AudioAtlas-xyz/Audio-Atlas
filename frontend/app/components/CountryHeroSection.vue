<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import type { useAuth } from '~/composables/useAuth'
import LoginModal from "~/components/UserFlow/LoginModal.vue";

const { breadCrumbItems, locationBadges, countryName, description } = defineProps<{
  locationBadges: string[]
  countryName: string
  description: string
  breadCrumbItems: Array<{ label: string, to: string, active?: boolean }>
}>()

const { user } = useAuth()

// Is the user logged in?
const isLoggedIn = computed(() => !!user.value)

// Should LoginModal be shown?
const showLogin = ref(false)


const handleButtonClick = () => {
  if (isLoggedIn.value) {
    navigateTo('/submission-form')
  } else {
    showLogin.value = true
  }
}

const closeModal = () => {
  showLogin.value = false
}

</script>

<template>
  <div class="grid gap-6 lg:grid-cols-[minmax(0,1fr)_320px] lg:items-start">
    <div class="space-y-5">

      <HeroSection
            :badges="locationBadges"
            :name="countryName"
            :bread-crumb-items="breadCrumbItems"
      />

        <p class="max-w-[680px] text-sm leading-8 text-[#7a84a8]">
          {{ description }}
        </p>

    </div>

    <div class="space-y-3 lg:pt-[72px]">
      <UButton
        block
        size="lg"
        color="primary"
        class="justify-center bg-aurora px-4 py-2 font-medium text-bg hover:bg-aurora rounded-xl cursor-pointer"
        @click="handleButtonClick"
      >
        + Add a genre from {{ countryName }}
      </UButton>
      <p class="text-[11px] text-[#373d5a]">
        Know a {{ countryName }} genre we&apos;ve missed?
      </p>

      <div v-if="showLogin" class="overlay">
        <LoginModal
        @close="closeModal"
        ></LoginModal>
      </div>
    </div>
  </div>
</template>
