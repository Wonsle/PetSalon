import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { authApi } from '@/api/auth'
import { useAuthStore } from '@/stores/auth'
import type { LoginResponse } from '@/types/auth'

vi.mock('@/api/auth', () => ({
  authApi: {
    login: vi.fn(),
    getCurrentUser: vi.fn()
  }
}))

const loginMock = vi.mocked(authApi.login)

const response = (requiresPasswordChange: boolean): LoginResponse => ({
  token: requiresPasswordChange ? 'restricted-token' : 'general-token',
  expiresIn: requiresPasswordChange ? 900 : 28800,
  requiresPasswordChange,
  user: {
    id: 7,
    userName: 'admin',
    name: 'admin',
    roles: requiresPasswordChange ? [] : ['RenamedRole'],
    permissions: requiresPasswordChange ? [] : ['auth.login', 'finance.read']
  }
})

describe('auth store', () => {
  beforeEach(() => {
    localStorage.clear()
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('keeps a password-change token out of the general authenticated session', async () => {
    loginMock.mockResolvedValue(response(true))
    const store = useAuthStore()

    const result = await store.login({ userName: 'admin', password: 'password' })

    expect(result).toEqual({ success: true, nextRouteName: 'ChangePassword' })
    expect(store.isAuthenticated).toBe(false)
    expect(store.token).toBeNull()
    expect(store.user).toBeNull()
    expect(store.passwordChangeToken).toBe('restricted-token')
    expect(localStorage.getItem('token')).toBeNull()
    expect(localStorage.getItem('user')).toBeNull()
    expect(localStorage.getItem('passwordChangeToken')).toBe('restricted-token')
  })

  it('creates a general session only when password change is not required', async () => {
    loginMock.mockResolvedValue(response(false))
    const store = useAuthStore()

    const result = await store.login({ userName: 'admin', password: 'Petsalon-2026!' })

    expect(result).toEqual({ success: true, nextRouteName: 'Dashboard' })
    expect(store.isAuthenticated).toBe(true)
    expect(store.token).toBe('general-token')
    expect(store.user?.permissions).toEqual(['auth.login', 'finance.read'])
    expect(store.passwordChangeToken).toBeNull()
    expect(localStorage.getItem('passwordChangeToken')).toBeNull()
  })

  it('restores a pending password-change flow without refreshing a business profile', async () => {
    localStorage.setItem('passwordChangeToken', 'restricted-token')
    const store = useAuthStore()

    await store.initialize()

    expect(store.passwordChangeToken).toBe('restricted-token')
    expect(store.isAuthenticated).toBe(false)
    expect(authApi.getCurrentUser).not.toHaveBeenCalled()
  })
})
