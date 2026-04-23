import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import AutoImport from 'unplugin-auto-import/vite'

const rootDir = fileURLToPath(new URL('.', import.meta.url))
const appDir = fileURLToPath(new URL('./app', import.meta.url))
const nuxtImportsShim = fileURLToPath(new URL('./tests/support/nuxt-imports.ts', import.meta.url))

export default defineConfig({
  plugins: [
    vue(),
    AutoImport({
      dts: false,
      imports: [
        {
          vue: ['computed']
        },
        {
          '#imports': ['useRoute', 'useRuntimeConfig', 'useAsyncData', 'useHead', '$fetch']
        }
      ]
    })
  ],
  resolve: {
    alias: {
      '~': appDir,
      '@': appDir,
      '#imports': nuxtImportsShim
    }
  },
  test: {
    environment: 'happy-dom',
    include: ['tests/**/*.test.ts'],
    root: rootDir
  }
})
