<template>
  <div class="login-container">
    <Card class="login-card">
      <template #header>
        <div class="login-header">
          <h1>🐾 Amada Pet Grooming</h1>
          <p>寵物美容管理系統</p>
        </div>
      </template>

      <div class="login-form">
        <div class="form-field">
          <label for="userName">帳號</label>
          <InputText
            id="userName"
            v-model="loginForm.userName"
            placeholder="請輸入帳號"
            :class="{ 'p-invalid': errors.userName }"
            @blur="validateField('userName')"
          />
          <small v-if="errors.userName" class="p-error">{{ errors.userName }}</small>
        </div>

        <div class="form-field">
          <label for="password">密碼</label>
          <Password
            id="password"
            v-model="loginForm.password"
            placeholder="請輸入密碼"
            :class="{ 'p-invalid': errors.password }"
            :feedback="false"
            toggle-mask
            @blur="validateField('password')"
            @keyup.enter="handleLogin"
          />
          <small v-if="errors.password" class="p-error">{{ errors.password }}</small>
        </div>

        <Button
          label="登入"
          :loading="loading"
          @click="handleLogin"
          class="login-button"
          size="large"
        />
      </div>

    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { useAuthStore } from '@/stores/auth'
import type { LoginCredentials } from '@/types/auth'
import Password from 'primevue/password'

const router = useRouter()
const authStore = useAuthStore()
const toast = useToast()

// Form data
const loginForm = reactive<LoginCredentials>({
  userName: '',
  password: ''
})

// Loading state
const loading = ref(false)

// Form errors
const errors = reactive({
  userName: '',
  password: ''
})

// Validation rules
const validateField = (field: 'userName' | 'password') => {
  errors[field] = ''

  if (field === 'userName') {
    if (!loginForm.userName) {
      errors.userName = '請輸入帳號'
    } else if (loginForm.userName.length < 3 || loginForm.userName.length > 20) {
      errors.userName = '帳號長度應為 3-20 個字符'
    }
  }

  if (field === 'password') {
    if (!loginForm.password) {
      errors.password = '請輸入密碼'
    } else if (loginForm.password.length < 6 || loginForm.password.length > 50) {
      errors.password = '密碼長度應為 6-50 個字符'
    }
  }
}

const validateForm = () => {
  validateField('userName')
  validateField('password')
  return !errors.userName && !errors.password
}

// Handle login
const handleLogin = async () => {
  if (!validateForm()) return

  try {
    loading.value = true

    const result = await authStore.login(loginForm)

    if (result.success) {
      toast.add({
        severity: 'success',
        summary: '登入成功',
        detail: '歡迎回來！',
        life: 3000
      })
      await router.push({ name: result.nextRouteName })
    } else {
      toast.add({
        severity: 'error',
        summary: '登入失敗',
        detail: result.message || '請檢查帳號密碼',
        life: 5000
      })
    }
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '登入過程中發生錯誤',
      life: 5000
    })
  } finally {
    loading.value = false
  }
}

</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

.login-card {
  width: 100%;
  max-width: 400px;
}

.login-header {
  text-align: center;
}

.login-header h1 {
  color: var(--p-primary-color);
  margin: 0 0 8px 0;
  font-size: 28px;
}

.login-header p {
  color: var(--p-text-muted-color);
  margin: 0;
  font-size: 14px;
}

.login-form {
  padding: 20px 0;
}

.form-field {
  margin-bottom: 24px;
}

.form-field label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: var(--p-text-color);
}

.form-field .p-inputtext,
.form-field .p-password {
  width: 100%;
}

.login-button {
  width: 100%;
  margin-top: 8px;
}

.demo-accounts {
  font-size: 12px;
  color: var(--p-text-muted-color);
}

.demo-account {
  margin: 4px 0;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.demo-account:hover {
  background-color: var(--p-content-hover-background);
}

.demo-account strong {
  color: var(--p-primary-color);
}

.p-error {
  color: var(--p-red-500);
  font-size: 12px;
  margin-top: 4px;
  display: block;
}
</style>
