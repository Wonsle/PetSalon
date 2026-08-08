import { beforeEach, describe, expect, it, vi } from 'vitest'
import { flushPromises, mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import ChangePassword from './ChangePassword.vue'
import { authApi } from '@/api/auth'
import { useAuthStore } from '@/stores/auth'

const replaceMock = vi.fn()

vi.mock('vue-router', () => ({
  useRouter: () => ({ replace: replaceMock })
}))

vi.mock('@/api/auth', () => ({
  authApi: {
    changePassword: vi.fn()
  }
}))

const changePasswordMock = vi.mocked(authApi.changePassword)

const mountView = () => mount(ChangePassword)

const fillForm = async (
  wrapper: ReturnType<typeof mountView>,
  currentPassword: string,
  newPassword: string,
  confirmPassword: string
) => {
  await wrapper.get('[data-testid="current-password"]').setValue(currentPassword)
  await wrapper.get('[data-testid="new-password"]').setValue(newPassword)
  await wrapper.get('[data-testid="confirm-password"]').setValue(confirmPassword)
  await wrapper.get('form').trigger('submit')
  await flushPromises()
}

describe('ChangePassword', () => {
  beforeEach(() => {
    localStorage.clear()
    localStorage.setItem('passwordChangeToken', 'restricted-token')
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('rejects a weak password before calling the API', async () => {
    const wrapper = mountView()

    await fillForm(wrapper, 'password', 'too-short', 'too-short')

    expect(wrapper.text()).toContain('新密碼至少需要 12 個字元')
    expect(changePasswordMock).not.toHaveBeenCalled()
  })

  it('rejects a mismatched confirmation before calling the API', async () => {
    const wrapper = mountView()

    await fillForm(wrapper, 'password', 'Petsalon-2026!', 'Petsalon-2027!')

    expect(wrapper.text()).toContain('確認密碼與新密碼不一致')
    expect(changePasswordMock).not.toHaveBeenCalled()
  })

  it('clears the restricted token and returns to login after success', async () => {
    changePasswordMock.mockResolvedValue(undefined)
    const wrapper = mountView()
    const store = useAuthStore()

    await fillForm(wrapper, 'password', 'Petsalon-2026!', 'Petsalon-2026!')

    expect(changePasswordMock).toHaveBeenCalledWith({
      currentPassword: 'password',
      newPassword: 'Petsalon-2026!',
      confirmPassword: 'Petsalon-2026!'
    }, 'restricted-token')
    expect(store.passwordChangeToken).toBeNull()
    expect(localStorage.getItem('passwordChangeToken')).toBeNull()
    expect(replaceMock).toHaveBeenCalledWith({ name: 'Login' })
  })

  it('clears an expired token, reports the failure, and returns to login', async () => {
    changePasswordMock.mockRejectedValue({ response: { status: 401 } })
    const wrapper = mountView()
    const store = useAuthStore()

    await fillForm(wrapper, 'password', 'Petsalon-2026!', 'Petsalon-2026!')

    expect(wrapper.text()).toContain('改密碼憑證已過期，請重新登入')
    expect(store.passwordChangeToken).toBeNull()
    expect(replaceMock).toHaveBeenCalledWith({ name: 'Login' })
  })
})
