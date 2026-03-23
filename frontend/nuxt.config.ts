// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  modules: [
    '@nuxt/ui',
    '@nuxtjs/google-fonts'
  ],

  googleFonts: {
    families: {
      'Space Grotesk': [300, 400, 500, 600],
      'Space Mono': [400, 700]
    }
  },


  css: ['~/assets/css/main.css'],

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
  }
})
