<script setup lang="ts">
import { computed, ref } from 'vue'

/**
 * One representative song per genre, embedded so users can hear the music
 * without leaving the site.
 *
 * Loads as a facade: nothing is requested from any Google domain until the user
 * clicks play. That keeps roughly a megabyte of player JavaScript off every
 * genre page view, and means no third-party cookie is set for visitors who
 * never listen — which is what lets us embed at all without a consent banner.
 *
 * The prop is a bare video ID, validated server-side by YouTubeVideo. The embed
 * URL is built here from that ID and is never taken from stored input, so a
 * submission cannot choose the host the iframe points at.
 */
const props = defineProps<{
  videoId?: string | null
  genreName?: string
  suggestHref?: string | null
}>()

const playing = ref(false)

// undefined rather than null so these can bind straight to src/href attributes.
const embedUrl = computed(() =>
  props.videoId ? `https://www.youtube-nocookie.com/embed/${props.videoId}?autoplay=1&rel=0` : undefined
)

const watchUrl = computed(() =>
  props.videoId ? `https://www.youtube.com/watch?v=${props.videoId}` : undefined
)

const title = computed(() =>
  props.genreName ? `Example song for ${props.genreName}` : 'Example song'
)
</script>

<template>
  <section class="rounded-md border border-border bg-surface">
    <header class="border-b border-border px-4 py-3">
      <h2 class="font-mono text-[10px] uppercase tracking-[0.15em] text-meta">
        Example song
      </h2>
    </header>

    <!-- Has a song -->
    <div
      v-if="videoId"
      class="p-4"
    >
      <div class="relative aspect-video w-full overflow-hidden rounded-md border border-border bg-surface-2">
        <iframe
          v-if="playing"
          :src="embedUrl"
          :title="title"
          class="absolute inset-0 h-full w-full"
          loading="lazy"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          referrerpolicy="strict-origin-when-cross-origin"
          allowfullscreen
        />

        <!-- Facade: no third-party request until this is clicked -->
        <button
          v-else
          type="button"
          class="group absolute inset-0 flex h-full w-full flex-col items-center justify-center gap-3 transition-colors hover:bg-surface-3"
          :aria-label="`Play ${title}`"
          @click="playing = true"
        >
          <span
            class="flex h-12 w-12 items-center justify-center rounded-full border border-aurora text-aurora transition-colors group-hover:bg-aurora group-hover:text-bg"
            aria-hidden="true"
          >
            <UIcon
              name="i-lucide-play"
              class="h-5 w-5"
            />
          </span>
          <span class="text-xs text-label">Play on YouTube</span>
        </button>
      </div>

      <p class="mt-3 text-[11px] text-meta">
        Loaded from YouTube only when you press play.
        <a
          :href="watchUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="underline decoration-dotted transition-colors hover:text-aurora"
        >Watch on YouTube</a>
      </p>
    </div>

    <!-- No song yet -->
    <div
      v-else
      class="px-4 py-5"
    >
      <p class="text-sm text-label">
        No example song yet.
      </p>
      <NuxtLink
        v-if="suggestHref"
        :to="suggestHref"
        class="mt-2 inline-block text-xs text-meta underline decoration-dotted transition-colors hover:text-aurora"
      >
        Suggest one
      </NuxtLink>
    </div>
  </section>
</template>
