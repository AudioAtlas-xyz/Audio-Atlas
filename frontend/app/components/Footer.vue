<template>
  <footer class="app-footer">
    <div class="footer-inner">
      <span class="footer-license">CC BY-NC-SA 4.0</span>
      <nav class="footer-links">
        <NuxtLink to="/about">About</NuxtLink>
        <NuxtLink to="/privacy-policy">Privacy Policy</NuxtLink>
        <a
          href="https://github.com/AudioAtlas-xyz/Audio-Atlas"
          target="_blank"
          rel="noopener"
        >GitHub</a>
      </nav>
    </div>
  </footer>
</template>

<style scoped>
.app-footer {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  z-index: 10;
  height: 2.25rem;
  background: rgba(2, 7, 10, 0.8);
  backdrop-filter: blur(8px);
  border-top: 1px solid rgba(141, 219, 230, 0.08);
}

.footer-inner {
  display: flex;
  align-items: center;
  justify-content: space-between;
  max-width: 90rem;
  height: 100%;
  margin: 0 auto;
  padding: 0 1.5rem;
}

.footer-license {
  font-family: 'Space Mono', monospace;
  font-size: 0.65rem;
  color: #4a6070;
}

.footer-links {
  display: flex;
  align-items: center;
  gap: 1.25rem;
}

.footer-links a {
  font-size: 0.7rem;
  color: #4a6070;
  text-decoration: none;
  transition: color 0.15s ease;
}

.footer-links a:hover {
  color: #8ddbe6;
}
</style>
