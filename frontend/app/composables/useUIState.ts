// Module-level so the timer is shared across every useUIState() call,
// preventing back-to-back banners from clobbering each other.
let bannerTimeout: ReturnType<typeof setTimeout> | null = null

export const useUIState = () => {
  const showAccount = useState<boolean>('ui_account', () => false)

  const openAccount = () => {
    showAccount.value = true
  }

  const closeAccount = () => {
    showAccount.value = false
  }

  // onboarding state
  const pendingRegistrationId = useState<string | null>('ui_pending_id', () => null)
  const suggestedUsername = useState<string | null>('ui_suggested_username', () => null)

  const openOnboarding = (id: string | null, username: string | null) => {
    pendingRegistrationId.value = id
    suggestedUsername.value = username
  }

  const showAppBanner = useState<boolean>('ui_app_banner', () => false)
  const bannerMessage = useState<string | null>('ui_banner_message', () => null)

  const triggerAppBanner = (message: string) => {
    // Cancel any banner that's still on screen so a fresh
    // message gets a full 3 s, and the previous timer can't
    // clear it early.
    if (bannerTimeout) {
      clearTimeout(bannerTimeout)
      bannerTimeout = null
    }

    bannerMessage.value = message
    showAppBanner.value = true

    bannerTimeout = setTimeout(() => {
      showAppBanner.value = false
      bannerMessage.value = null
      bannerTimeout = null
    }, 3000)
  }

  return {
    showAccount,
    openAccount,
    closeAccount,

    pendingRegistrationId,
    suggestedUsername,
    openOnboarding,

    showAppBanner,
    bannerMessage,
    triggerAppBanner
  }
}
