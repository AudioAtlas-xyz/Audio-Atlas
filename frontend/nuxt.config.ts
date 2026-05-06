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
    '/': { prerender: true }
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
    apiProxyTarget: process.env.NUXT_API_PROXY_TARGET || (process.env.NODE_ENV === 'production'
      ? 'api-audioatlas.azurewebsites.net'
      : 'http://localhost:5085'),

    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || '/api',
      backendBaseUrl: process.env.NUXT_PUBLIC_BACKEND_BASE_URL || (process.env.NODE_ENV === 'production'
        ? 'api-audioatlas.azurewebsites.net'
        : 'http://localhost:5085')
    }
  }
})
