import { ref, onMounted, onBeforeUnmount } from 'vue'

/**
 * Tracks scroll progress through a one-time intro animation.
 * Once progress reaches 1 the intro locks, scroll resets to top,
 * and the page overflow is disabled.
 *
 * @param {object} options
 * @param {number} [options.scrollMultiplier=0.85] - Fraction of viewport height used as scroll range
 * @returns {{ progress: Ref<number>, finished: Ref<boolean> }}
 */
export function useScrollIntro ({ scrollMultiplier = 0.85 } = {}) {
  const progress = ref(0)
  const finished = ref(false)
  let rafId = 0

  const lock = () => {
    if (finished.value) return

    finished.value = true
    progress.value = 1

    if (rafId) {
      window.cancelAnimationFrame(rafId)
      rafId = 0
    }

    document.documentElement.classList.add('globe-intro-complete')
    window.scrollTo({ top: 0, left: 0, behavior: 'instant' })
  }

  const update = () => {
    if (finished.value) return

    const range = Math.max(1, window.innerHeight * scrollMultiplier)
    const p = Math.min(1, Math.max(0, window.scrollY / range))

    progress.value = p
    if (p >= 1) lock()
  }

  const onScroll = () => {
    if (finished.value || rafId) return
    rafId = window.requestAnimationFrame(() => {
      rafId = 0
      update()
    })
  }

onMounted(() => {
  if ('scrollRestoration' in history) {
    history.scrollRestoration = 'manual'
  }
  window.scrollTo({ top: 0, left: 0, behavior: 'instant' })
  document.documentElement.classList.remove('globe-intro-complete')
  update()
  window.addEventListener('scroll', onScroll, { passive: true })
  window.addEventListener('resize', onScroll)
})

  onBeforeUnmount(() => {
    document.documentElement.classList.remove('globe-intro-complete')
    window.removeEventListener('scroll', onScroll)
    window.removeEventListener('resize', onScroll)
    if (rafId) {
      window.cancelAnimationFrame(rafId)
      rafId = 0
    }
  })

  return { progress, finished }
}