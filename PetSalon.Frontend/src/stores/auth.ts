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
      const response = await authApi.login(credentials)
      token.value = response.token
      user.value = response.user
      localStorage.setItem('token', response.token)
      return { success: true }
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
  }

  const refreshUser = async () => {
    if (!token.value) return
    
    try {
      const userData = await authApi.getCurrentUser()
      user.value = userData
    } catch (error) {
      logout()
    }
  }

  // Initialize
  const initialize = async () => {
    if (token.value) {
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