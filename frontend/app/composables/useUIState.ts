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

  const showLoginBanner = useState<boolean>('ui_login_banner', () => false)

  const triggerLoginBanner = () => {
    showLoginBanner.value = true

    setTimeout(() => {
      showLoginBanner.value = false
    }, 3000)
  }

  return {
    showAccount,
    openAccount,
    closeAccount,

    pendingRegistrationId,
    suggestedUsername,
    openOnboarding,

    showLoginBanner,
    triggerLoginBanner
  }
}