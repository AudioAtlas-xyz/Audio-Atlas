<script setup>
import { useRouter, useRoute } from 'vue-router'
import { useHead } from '#imports'

const router = useRouter()
const route = useRoute()

useHead({
  title: 'Signing in...'
})

if (process.client) {
  const token = route.query.token
  const newUser = route.query.newUser

  if (token) {
    localStorage.setItem('token', token)

    if (newUser === 'false') {
      localStorage.setItem('showLoginBanner', 'true')
    }

    if (newUser === 'true') {
      localStorage.setItem('showUsernameModal', 'true')
    }

    window.history.replaceState({}, '', '/')

    router.replace('/')
  }
}
</script>

<template>
  <div style="display:none;"></div>
</template>