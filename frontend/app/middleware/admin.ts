// Route guard for admin pages. Use via:
//   definePageMeta({ middleware: 'admin' })
//
// Skipped on the server (token lives in localStorage). Backend endpoints
// must still be protected with [Authorize(Roles = "Admin")] — this is
// just UI gating.
export default defineNuxtRouteMiddleware(async () => {
  if (process.server) return

  const { user, fetchUser, isAdmin } = useAuth()

  if (!user.value) {
    await fetchUser()
  }

  if (!isAdmin.value) {
    return navigateTo('/')
  }
})
