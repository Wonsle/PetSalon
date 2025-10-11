import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'

export default defineConfig(({ mode }) => {
  // In mock mode, don't use proxy - MSW will handle API requests
  const useMock = mode === 'mock'

  return {
    plugins: [
      vue(),
      vueDevTools(),
      AutoImport({
        imports: ['vue', 'vue-router', 'pinia', '@vueuse/core'],
        dts: true,
      }),
      Components({
        dts: true,
      }),
    ],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    },
    server: {
      host: '127.0.0.1',
      port: 3000,
      // Only use proxy when NOT in mock mode
      proxy: useMock ? undefined : {
        '/api': {
          target: 'http://localhost:5150',
          changeOrigin: true,
          secure: false
        }
      }
    }
  }
})
