<script setup lang="ts">
/**
 * `/` is now the globe, so this route only exists to keep older links working —
 * bookmarks, the post-submission link in FormSubmittedCard, and anything already
 * indexed. Redirecting rather than duplicating avoids two URLs serving the same
 * content and splitting SEO signals.
 */
definePageMeta({
  redirect: '/'
})
</script>

<template>
  <div />
</template>
