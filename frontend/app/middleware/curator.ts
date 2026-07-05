// Route guard for pages accessible to Admin and Curator roles.
// Use via: definePageMeta({ middleware: 'curator' })
//
// Skipped on server — token lives in localStorage. Backend endpoints
// must independently enforce [Authorize(Roles = "Admin,Curator")].
export default defineNuxtRouteMiddleware(async () => {
  if (process.server) return

  const { user, fetchUser, isAdmin, isCurator } = useAuth()

  if (!user.value) {
    await fetchUser()
  }

  if (!isAdmin.value && !isCurator.value) {
    return navigateTo('/')
  }
})
