import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

// Create a global toast function that can be used without importing
declare global {
  interface Window {
    $toast?: {
      add: (options: {
        severity: string
        summary: string
        detail: string
        life: number
      }) => void
    }
  }
}

// Create axios instance
const instance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor
instance.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore()
    if (authStore.token) {
      config.headers.Authorization = `Bearer ${authStore.token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor
instance.interceptors.response.use(
  (response) => {
    return response
  },
  (error) => {
    const authStore = useAuthStore()

    // Function to show toast message
    const showToast = (severity: string, summary: string, detail: string) => {
      if (window.$toast) {
        window.$toast.add({ severity, summary, detail, life: 3000 })
      } else {
        // Fallback to console if toast is not available
        console.warn(`${severity.toUpperCase()}: ${summary} - ${detail}`)
      }
    }

    if (error.response) {
      const { status, data } = error.response

      switch (status) {
        case 401:
          showToast('error', '驗證失敗', '請重新登入')
          authStore.logout()
          window.location.href = '/login'
          break
        case 403:
          showToast('error', '權限不足', '您沒有權限執行此操作')
          break
        case 404:
          showToast('error', '資源不存在', '請求的資源未找到')
          break
        case 500:
          showToast('error', '伺服器錯誤', '伺服器發生內部錯誤')
          break
        default:
          showToast('error', '發生錯誤', data?.message || '未知錯誤')
      }
    } else {
      showToast('error', '網路錯誤', '請檢查網路連線')
    }

    return Promise.reject(error)
  }
)

export default instance