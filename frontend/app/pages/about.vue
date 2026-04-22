<script setup>
</script>

<template>
  <main class="about-page">
    <div class="about-container">

      <h1>About</h1>

      <section>
        <h2>Audio Atlas</h2>
        <p>
          Audio Atlas is an interactive, community-driven map of the world’s music. It connects genres across countries, cultures, and histories, and makes musical discovery visual, explorable, and collaborative.        </p>
      </section>

      <section>
        <h2>How it Works</h2>

        <h3>Discover</h3>
        <p>
          Browse the interactive globe. Click a country to see its genres. Dive into any genre to learn about its history, sound, instruments, and connections to other genres.
        </p>

        <h3>Contribute</h3>
        <p>
          Create a free account with GitHub or Google. Submit genres using our structured form. Submissions are reviewed for quality and cultural sensitivity before going live.
        </p>

        <h3>Connect</h3>
        <p>
          Follow genre-to-genre links to trace how music styles influence each other across borders. Explore predecessors, sub-genres, and related genres worldwide.
        </p>
      </section>

      <section>
        <h2>Open Source</h2>
        <p>
          Audio Atlas is fully open source. The code is licensed under MIT, and all contributed content is licensed under CC BY-SA 4.0.
        </p>
        <p>
          We believe knowledge about the world’s music should be freely accessible.
        </p>

        <p>
          <a
            href="https://github.com/AudioAtlas-xyz/Audio-Atlas"
            target="_blank"
            rel="noopener noreferrer"
          >
            View on GitHub →
          </a>
        </p>
      </section>

      <section>
        <h2>Contact</h2>
        <p>
          Questions, feedback, or cultural sensitivity concerns:
        </p>

        <p>
          <a href="mailto:projectaudioatlas@gmail.com">
            projectaudioatlas@gmail.com
          </a>
        </p>
      </section>

    </div>
  </main>
</template>

<style scoped>
.about-page {
  min-height: 100vh;
  background: #02070a;
  color: #eef2ff;
  padding: 2rem 1.5rem 4rem;
}

.about-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 2.5rem;

  background: #050816;
  border: 1px solid rgba(120, 150, 255, 0.18);
  border-radius: 24px;

  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.45);
}

h1 {
  margin-top: 0;
  font-size: 2.5rem;
}

section {
  margin-top: 2rem;
}

h2 {
  color: #44f0c4;
  margin-bottom: 0.75rem;
}

h3 {
  margin-top: 1.5rem;
  color: #eef2ff;
}

p {
  line-height: 1.7;
  color: #d7ddff;
}

a {
  color: #8b8dff;
  text-decoration: none;
}

a:hover {
  opacity: 0.7;
}
</style>