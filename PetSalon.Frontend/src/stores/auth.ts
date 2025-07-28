import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, LoginCredentials } from '@/types/auth'
import { authApi } from '@/api/auth'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(localStorage.getItem('token'))
  const user = ref<User | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value)
  const currentUser = computed(() => user.value)

  // Actions
  const login = async (credentials: LoginCredentials) => {
    try {
      // For now, mock the API response until backend is fully implemented
      // This allows frontend functionality testing
      const mockUsers: Record<string, User> = {
        'admin': {
          id: 1,
          userName: 'admin',
          name: '系統管理員',
          email: 'admin@example.com',
          roles: ['Admin']
        },
        'manager': {
          id: 2,
          userName: 'manager',
          name: '店長',
          email: 'manager@example.com',
          roles: ['Manager']
        },
        'stylist': {
          id: 3,
          userName: 'stylist',
          name: '設計師',
          email: 'stylist@example.com',
          roles: ['Designer']
        }
      }
      
      // Mock validation
      const mockUser = mockUsers[credentials.userName]
      if (mockUser && (
        (credentials.userName === 'admin' && credentials.password === 'admin123') ||
        (credentials.userName === 'manager' && credentials.password === 'manager123') ||
        (credentials.userName === 'stylist' && credentials.password === 'stylist123')
      )) {
        const mockToken = `mock-token-${Date.now()}`
        token.value = mockToken
        user.value = mockUser
        localStorage.setItem('token', mockToken)
        localStorage.setItem('user', JSON.stringify(mockUser))
        return { success: true }
      } else {
        return { 
          success: false, 
          message: '帳號或密碼錯誤' 
        }
      }
      
      // TODO: Replace with actual API call when backend is ready
      // const response = await authApi.login(credentials)
      // token.value = response.token
      // user.value = response.user
      // localStorage.setItem('token', response.token)
      // return { success: true }
    } catch (error: any) {
      return { 
        success: false, 
        message: error.response?.data?.message || '登入失敗' 
      }
    }
  }

  const logout = () => {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  const refreshUser = async () => {
    if (!token.value) return
    
    try {
      // If using mock token, preserve the current user data
      if (token.value.startsWith('mock-token-')) {
        // User data is already set, no need to refresh from API
        return
      }
      
      // TODO: Uncomment when backend is ready
      // const userData = await authApi.getCurrentUser()
      // user.value = userData
    } catch (error) {
      logout()
    }
  }

  // Initialize
  const initialize = async () => {
    if (token.value) {
      // Try to restore user data from localStorage first
      const storedUser = localStorage.getItem('user')
      if (storedUser) {
        try {
          user.value = JSON.parse(storedUser)
        } catch (error) {
          console.error('Failed to parse stored user data:', error)
        }
      }
      
      // Then refresh from server if needed
      await refreshUser()
    }
  }

  return {
    token,
    user,
    isAuthenticated,
    currentUser,
    login,
    logout,
    refreshUser,
    initialize
  }
})