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
    '@nuxtjs/google-fonts'
  ],

  googleFonts: {
    families: {
      'Space Grotesk': [300, 400, 500, 600],
      'Space Mono': [400, 700]
    }
  },

  css: ['~/assets/css/main.css'],

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

  devtools: {
    enabled: true
  },

  routeRules: {
    '/': { prerender: true },
    '/privacy-policy': { prerender: true },
    '/about': { prerender: true },
    '/contribution-guidelines': { prerender: true },
    '/explore': { prerender: true },
    '/genres': { prerender: true },
    '/auth/callback': { ssr: false }
  },

  colorMode: {
    preference: 'dark',
    fallback: 'dark'
  },

  compatibilityDate: '2025-01-15',

  eslint: {
    config: {
      stylistic: {
        commaDangle: 'never',
        braceStyle: '1tbs'
      }
    }
  },

  runtimeConfig: {
    apiProxyTarget,

    public: {
      apiBase,
      backendBaseUrl
    }
  }
})
