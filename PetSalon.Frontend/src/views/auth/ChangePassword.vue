<template>
  <main class="change-password-page">
    <section class="change-password-card" aria-labelledby="change-password-title">
      <header>
        <div class="brand-mark" aria-hidden="true">🐾</div>
        <h1 id="change-password-title">設定新的管理員密碼</h1>
        <p>這是第一次登入。完成密碼更新後，請使用新密碼重新登入。</p>
      </header>

      <form novalidate @submit.prevent="submit">
        <label for="currentPassword">目前密碼</label>
        <input
          id="currentPassword"
          v-model="form.currentPassword"
          data-testid="current-password"
          type="password"
          autocomplete="current-password"
          required
        />

        <label for="newPassword">新密碼</label>
        <input
          id="newPassword"
          v-model="form.newPassword"
          data-testid="new-password"
          type="password"
          autocomplete="new-password"
          minlength="12"
          required
        />
        <small>至少 12 個字元，且不可使用 password。</small>

        <label for="confirmPassword">確認新密碼</label>
        <input
          id="confirmPassword"
          v-model="form.confirmPassword"
          data-testid="confirm-password"
          type="password"
          autocomplete="new-password"
          required
        />

        <p v-if="message" class="form-message" role="alert">{{ message }}</p>

        <button type="submit" :disabled="submitting">
          {{ submitting ? '更新中…' : '更新密碼' }}
        </button>
      </form>
    </section>
  </main>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { authApi } from '@/api/auth'
import { useAuthStore } from '@/stores/auth'
import type { ChangePasswordRequest } from '@/types/auth'

const router = useRouter()
const authStore = useAuthStore()
const submitting = ref(false)
const message = ref('')
const form = reactive<ChangePasswordRequest>({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const validate = () => {
  if (!form.currentPassword || !form.newPassword || !form.confirmPassword) {
    return '請完整填寫所有欄位'
  }
  if (form.newPassword.length < 12) {
    return '新密碼至少需要 12 個字元'
  }
  if (form.newPassword.toLowerCase() === 'password') {
    return '新密碼不可使用 password'
  }
  if (form.newPassword !== form.confirmPassword) {
    return '確認密碼與新密碼不一致'
  }
  return ''
}

const submit = async () => {
  message.value = validate()
  if (message.value) return

  const restrictedToken = authStore.passwordChangeToken
  if (!restrictedToken) {
    message.value = '改密碼憑證已過期，請重新登入'
    await router.replace({ name: 'Login' })
    return
  }

  submitting.value = true
  try {
    await authApi.changePassword({ ...form }, restrictedToken)
    authStore.clearPasswordChangeSession()
    await router.replace({ name: 'Login' })
  } catch (error: any) {
    if (error.response?.status === 401) {
      authStore.clearPasswordChangeSession()
      message.value = '改密碼憑證已過期，請重新登入'
      await router.replace({ name: 'Login' })
    } else {
      message.value = error.response?.data?.message || '密碼更新失敗，請確認輸入內容'
    }
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.change-password-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 24px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.change-password-card {
  width: min(100%, 440px);
  padding: 32px;
  border-radius: 16px;
  background: var(--p-surface-0, #fff);
  box-shadow: 0 18px 50px rgb(38 24 79 / 24%);
}

header { text-align: center; }
.brand-mark { font-size: 2rem; }
h1 { margin: 8px 0; font-size: 1.5rem; }
header p, small { color: var(--p-text-muted-color, #667085); }
form { display: grid; gap: 10px; margin-top: 24px; }
label { margin-top: 8px; font-weight: 600; }
input {
  width: 100%;
  padding: 12px;
  border: 1px solid var(--p-content-border-color, #d0d5dd);
  border-radius: 8px;
  font: inherit;
}
input:focus { outline: 2px solid var(--p-primary-color, #6366f1); outline-offset: 1px; }
.form-message { margin: 6px 0 0; color: var(--p-red-600, #dc2626); }
button {
  margin-top: 12px;
  padding: 12px;
  border: 0;
  border-radius: 8px;
  background: var(--p-primary-color, #6366f1);
  color: #fff;
  font: inherit;
  font-weight: 700;
  cursor: pointer;
}
button:disabled { cursor: wait; opacity: 0.65; }
</style>
