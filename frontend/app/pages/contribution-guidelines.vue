<script setup>
import { useHead } from '#imports'

useHead({
  title: 'Contribution Guidelines - Audio Atlas'
})
</script>

<template>
  <main class="legal-page">
    <div class="legal-container">
      <h1>Contribution Guidelines</h1>

      <p class="meta">
        Last updated: March 2026
      </p>

      <blockquote>
        Thank you for helping build the living map of the world's music.
      </blockquote>

      <section>
        <h2>Code of Conduct</h2>
        <p>
          Audio Atlas is dedicated to making global music culture more discoverable and interconnected. We expect all contributors to treat each other — and the musical cultures represented in this project — with respect.
        </p>
        <p>
          We do not tolerate harassment, discrimination, or disrespectful behaviour of any kind.
        </p>
      </section>

      <section>
        <h2>How to Contribute</h2>

        <h3>Contributing Genre Data</h3>
        <p>
          You can contribute either through the website contribution form or directly through the GitHub repository.
        </p>

        <ul>
          <li>Genre name and aliases</li>
          <li>Country or region of origin</li>
          <li>Short description</li>
          <li>Related genres</li>
          <li>Instruments where applicable</li>
        </ul>

        <h3>Contributing Code</h3>
        <ol>
          <li>Check existing issues</li>
          <li>Fork the repository</li>
          <li>Create a feature branch</li>
          <li>Write clear, documented code</li>
          <li>Test your changes</li>
          <li>Submit a pull request</li>
        </ol>
      </section>

      <section>
        <h2>Pull Request Process</h2>
        <ul>
          <li>Keep PRs focused</li>
          <li>Write a clear description</li>
          <li>Ensure the build passes</li>
          <li>Maintain accessibility</li>
          <li>Be responsive to feedback</li>
        </ul>
      </section>

      <section>
        <h2>Cultural Sensitivity Guidelines</h2>
        <p>
          Audio Atlas documents music from cultures around the world. This comes with responsibility.
        </p>

        <ul>
          <li>Respect cultural origins</li>
          <li>Use appropriate terminology</li>
          <li>Handle sacred and ceremonial music with care</li>
          <li>Do not speculate</li>
          <li>Acknowledge disputed origins</li>
          <li>Cite your sources</li>
        </ul>
      </section>

      <section>
        <h2>Licensing & Copyright</h2>

        <table>
          <thead>
            <tr>
              <th>What</th>
              <th>Licence</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Source code</td>
              <td>MIT License</td>
            </tr>
            <tr>
              <td>Genre data and content</td>
              <td>CC BY-NC-SA 4.0</td>
            </tr>
          </tbody>
        </table>
      </section>

      <section>
        <h2>Maintainers</h2>
        <ul>
          <li>Jed Anang — Product owner, genre data, project strategy</li>
          <li>Christophe Berbec — Design, interaction, visual experience</li>
        </ul>

        <p>
          Contact:
          <a href="mailto:projectaudioatlas@gmail.com">projectaudioatlas@gmail.com</a>
        </p>
      </section>
    </div>
  </main>
</template>

<style scoped>
.legal-page {
  min-height: 100vh;
  background: #02070a;
  color: #eef2ff;
  padding: 2rem 1.5rem 4rem;
}

.legal-container {
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

.meta {
  color: #8a92b8;
  margin-bottom: 2rem;
}

blockquote {
  margin: 0 0 2rem;
  padding: 1rem 1.25rem;
  border-left: 3px solid #44f0c4;
  background: rgba(68, 240, 196, 0.08);
  color: #d6fff5;
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

p,
li {
  line-height: 1.7;
  color: #d7ddff;
}

ul,
ol {
  padding-left: 1.5rem;
}

a {
  color: #8b8dff;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

th,
td {
  border: 1px solid rgba(120, 150, 255, 0.18);
  padding: 0.75rem;
  text-align: left;
}

th {
  background: rgba(120, 150, 255, 0.08);
}
</style>

