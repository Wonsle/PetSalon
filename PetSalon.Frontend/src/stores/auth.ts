import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { User, LoginCredentials } from '@/types/auth'
import { authApi } from '@/api/auth'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(localStorage.getItem('token'))
  const passwordChangeToken = ref<string | null>(localStorage.getItem('passwordChangeToken'))
  const user = ref<User | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value && !!user.value && !passwordChangeToken.value)
  const currentUser = computed(() => user.value)

  // Actions
  const login = async (credentials: LoginCredentials) => {
    try {
      const response = await authApi.login(credentials)

      if (response.requiresPasswordChange) {
        token.value = null
        user.value = null
        passwordChangeToken.value = response.token
        localStorage.removeItem('token')
        localStorage.removeItem('user')
        localStorage.setItem('passwordChangeToken', response.token)

        return { success: true, nextRouteName: 'ChangePassword' as const }
      }

      token.value = response.token
      user.value = response.user
      passwordChangeToken.value = null
      localStorage.setItem('token', response.token)
      localStorage.setItem('user', JSON.stringify(response.user))
      localStorage.removeItem('passwordChangeToken')

      return { success: true, nextRouteName: 'Dashboard' as const }
    } catch (error: any) {
      return { 
        success: false, 
        message: error.response?.data?.message || '登入失敗' 
      }
    }
  }

  const logout = () => {
    token.value = null
    passwordChangeToken.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    localStorage.removeItem('passwordChangeToken')
  }

  const clearPasswordChangeSession = () => {
    passwordChangeToken.value = null
    localStorage.removeItem('passwordChangeToken')
  }

  const refreshUser = async () => {
    if (!token.value) return
    
    try {
      const userData = await authApi.getCurrentUser()
      user.value = userData
      localStorage.setItem('user', JSON.stringify(userData))
    } catch (error) {
      logout()
    }
  }

  // Initialize
  const initialize = async () => {
    if (passwordChangeToken.value) return

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
    passwordChangeToken,
    user,
    isAuthenticated,
    currentUser,
    login,
    logout,
    clearPasswordChangeSession,
    refreshUser,
    initialize
  }
})
