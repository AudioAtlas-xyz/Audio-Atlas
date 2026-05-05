/**
 * Route guard for admin-only pages.
 *
 * Opt a page in with:
 *   definePageMeta({ middleware: 'admin' })
 *
 * Behaviour:
 *  - Skipped during SSR — `useAuth` reads the JWT from localStorage,
 *    which only exists on the client. If we redirected server-side
 *    we'd bounce every direct nav to /admin away.
 *  - On the client, ensures `user` is populated (fetches it if not),
 *    then redirects to `/` for non-admins.
 *
 * Note: this is UI gating only. The real authorisation lives on the
 * backend — admin endpoints must still be decorated with
 * `[Authorize(Roles = "Admin")]` since clients can bypass middleware.
 */
export default defineNuxtRouteMiddleware(async () => {
  if (process.server) return

  const { user, fetchUser, isAdmin } = useAuth()

  // Populate the user from `/auth/me` if we have a token but no user
  // state yet (e.g. on a hard refresh straight to /admin).
  if (!user.value) {
    await fetchUser()
  }

  if (!isAdmin.value) {
    return navigateTo('/')
  }
})
