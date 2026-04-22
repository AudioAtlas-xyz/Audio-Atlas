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
  const pendingRegistrationId = route.query.pendingRegistrationId
  const suggestedUsername = route.query.suggestedUsername

  if (token) {
    localStorage.setItem('token', token)
  }

  if (newUser === 'false') {
    localStorage.setItem('showLoginBanner', 'true')
  }

  if (newUser === 'true') {
    localStorage.setItem('showUsernameModal', 'true')

    if (pendingRegistrationId) {
      localStorage.setItem('pendingRegistrationId', pendingRegistrationId)
    }

    if (suggestedUsername) {
      localStorage.setItem('suggestedUsername', suggestedUsername)
    }
  }

  window.history.replaceState({}, '', '/')

  router.replace('/')
}
</script>