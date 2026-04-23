<script setup lang="ts">
import { useHead } from '#imports'

/**
 * Set page title for About page
 */
useHead({
  title: 'About - Audio Atlas'
})
</script>

<template>
  <!-- Main page wrapper -->
  <main class="about-page">
    <div class="about-container">

      <!-- Page title -->
      <h1>About</h1>

      <!-- Intro section -->
      <section>
        <h2>Audio Atlas</h2>
        <p>
          Audio Atlas is an interactive, community-driven map of the world’s music.
          It connects genres across countries, cultures, and histories, and makes
          musical discovery visual, explorable, and collaborative.
        </p>
      </section>

      <!-- How it works -->
      <section>
        <h2>How it Works</h2>

        <!-- Discover -->
        <h3>Discover</h3>
        <p>
          Browse the interactive globe. Click a country to see its genres. Dive into any genre
          to learn about its history, sound, instruments, and connections to other genres.
        </p>

        <!-- Contribute -->
        <h3>Contribute</h3>
        <p>
          Create a free account with GitHub or Google.
          Read our
          <NuxtLink to="/contribution-guidelines" class="link-green">
            Contribution Guidelines
          </NuxtLink>
          and
          <NuxtLink to="/privacy-policy" class="link-green">
            Privacy Policy
          </NuxtLink>.
          Then submit genres using our structured form.
          Submissions are reviewed for quality and cultural sensitivity before going live.
        </p>

        <!-- Connect -->
        <h3>Connect</h3>
        <p>
          Follow genre-to-genre links to trace how music styles influence each other across borders.
          Explore predecessors, sub-genres, and related genres worldwide.
        </p>
      </section>

      <!-- Open source section -->
      <section>
        <h2>Open Source</h2>
        <p>
          Audio Atlas is fully open source. The code is licensed under MIT,
          and all contributed content is licensed under CC BY-SA 4.0.
        </p>
        <p>
          We believe knowledge about the world’s music should be freely accessible.
        </p>

        <p>
          <a
            href="https://github.com/AudioAtlas-xyz/Audio-Atlas"
            target="_blank"
            rel="noopener noreferrer"
            class="link-green"
          >
            View on GitHub ↗
          </a>
        </p>
      </section>

      <!-- Contact section -->
      <section>
        <h2>Contact</h2>
        <p>
          Questions, feedback, or cultural sensitivity concerns:
        </p>

        <p>
          <a href="mailto:projectaudioatlas@gmail.com" class="link-green">
            projectaudioatlas@gmail.com
          </a>
        </p>
      </section>

    </div>
  </main>
</template>

<style scoped>
/* Page background and layout */
.about-page {
  min-height: 100vh;
  background: #02070a;
  color: #eef2ff;
  padding: 2rem 1.5rem 4rem;
}

/* Centered content container */
.about-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 2.5rem;

  background: #050816;
  border: 1px solid rgba(120, 150, 255, 0.18);
  border-radius: 24px;

  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.45);
}

/* Main heading */
h1 {
  margin-top: 0;
  font-size: 2.5rem;
}

/* Section spacing */
section {
  margin-top: 2rem;
}

/* Section titles */
h2 {
  color: #44f0c4;
  margin-bottom: 0.75rem;
}

/* Subsection titles */
h3 {
  margin-top: 1.5rem;
  color: #eef2ff;
}

/* Paragraph styling */
p {
  line-height: 1.7;
  color: #d7ddff;
}

/* Remove default link styling */
a {
  text-decoration: none;
}

/* Consistent green link style */
.link-green {
  color: #44f0c4;
  font-weight: 500;
  cursor: pointer;
  transition: opacity 0.2s ease, text-decoration 0.2s ease;
}

.link-green:hover {
  opacity: 0.85;
  text-decoration: underline;
}
</style>