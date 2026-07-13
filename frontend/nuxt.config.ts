const productionBackendBaseUrl = 'https://api-audioatlas.azurewebsites.net'
const localBackendBaseUrl = 'http://localhost:5000'
const defaultBackendBaseUrl = process.env.NODE_ENV === 'production'
  ? productionBackendBaseUrl
  : localBackendBaseUrl
const backendBaseUrl = (process.env.NUXT_PUBLIC_BACKEND_BASE_URL || defaultBackendBaseUrl).replace(/\/+$/, '')
const apiBase = (process.env.NUXT_PUBLIC_API_BASE || (process.env.NODE_ENV === 'production'
  ? `${backendBaseUrl}/api`
  : '/api')).replace(/\/+$/, '')
const apiProxyTarget = (process.env.NUXT_API_PROXY_TARGET || defaultBackendBaseUrl).replace(/\/+$/, '')

export default defineNuxtConfig({
  modules: [
    '@nuxt/ui',
    '@nuxt/eslint',
    '@nuxtjs/google-fonts',
    '@nuxtjs/seo'
  ],

  app: {
    head: {
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'icon', type: 'image/svg+xml', href: '/favicon.svg' },
        { rel: 'icon', type: 'image/png', sizes: '96x96', href: '/favicon-96x96.png' },
        { rel: 'apple-touch-icon', sizes: '180x180', href: '/apple-touch-icon.png' },
        { rel: 'manifest', href: '/site.webmanifest' }
      ]
    }
  },

  devtools: {
    enabled: true
  },

  css: ['~/assets/css/main.css'],

  site: {
    url: 'https://audioatlas.xyz',
    name: 'Audio Atlas',
    description: 'Explore, discover and contribute to a living map of the world\'s music genres — from Afrobeats to Zydeco.',
    defaultLocale: 'en'
  },

  colorMode: {
    preference: 'dark',
    fallback: 'dark',
    storageKey: 'aa-color-mode'
  },

  runtimeConfig: {
    apiProxyTarget,

    public: {
      apiBase,
      backendBaseUrl
    }
  },

  routeRules: {
    '/': { prerender: true },
    '/privacy-policy': { prerender: true },
    '/about': { prerender: true },
    '/contribution-guidelines': { prerender: true },
    '/explore': { prerender: true },
    '/genres': { ssr: true },
    '/auth/callback': { ssr: false }
  },

  compatibilityDate: '2025-01-15',

  vite: {
    optimizeDeps: {
      include: [
        'globe.gl',
        '@vue/devtools-core',
        '@vue/devtools-kit',
        'three'
      ]
    }
  },

  eslint: {
    config: {
      stylistic: {
        commaDangle: 'never',
        braceStyle: '1tbs'
      }
    }
  },

  googleFonts: {
    families: {
      'Space Grotesk': [300, 400, 500, 600],
      'Space Mono': [400, 700]
    }
  },

  // Runtime OG image generation is disabled — we serve a static /og-image.png instead.
  ogImage: {
    enabled: false
  },

  // No static robots.txt exists. The robots module generates one and adds a Sitemap directive.
  // If AI content-signal directives are needed in future, configure them here via the `robots` key
  // rather than a static file so the Sitemap line is never lost.
  robots: {
    disallow: [],
    sitemap: 'https://audioatlas.xyz/sitemap.xml'
  },

  // Genre pages use /genres?genreId=... (query-string, not path segments) and are SSR not prerendered.
  // They cannot be auto-discovered by the sitemap crawler. They will be added in a future phase
  // once a /genres listing endpoint exists to enumerate them programmatically.
  sitemap: {
    strictNuxtContentPaths: false
  }
})
