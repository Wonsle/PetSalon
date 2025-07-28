<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <h1>🐾 Amada Pet Grooming</h1>
        <p>寵物美容管理系統</p>
      </div>
      
      <el-form
        ref="loginFormRef"
        :model="loginForm"
        :rules="loginRules"
        label-width="80px"
        class="login-form"
        @submit.prevent="handleLogin"
      >
        <el-form-item label="帳號" prop="userName">
          <el-input
            v-model="loginForm.userName"
            placeholder="請輸入帳號"
            size="large"
            :prefix-icon="User"
          />
        </el-form-item>
        
        <el-form-item label="密碼" prop="password">
          <el-input
            v-model="loginForm.password"
            type="password"
            placeholder="請輸入密碼"
            size="large"
            :prefix-icon="Lock"
            show-password
            @keyup.enter="handleLogin"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button
            type="primary"
            size="large"
            :loading="loading"
            @click="handleLogin"
            class="login-button"
          >
            <span v-if="!loading">登入</span>
            <span v-else>登入中...</span>
          </el-button>
        </el-form-item>
      </el-form>
      
      <!-- Demo accounts info -->
      <div class="demo-info">
        <el-divider>測試帳號</el-divider>
        <div class="demo-accounts">
          <div class="demo-account" @click="quickLogin('admin', 'admin123')">
            <strong>管理員:</strong> admin / admin123
          </div>
          <div class="demo-account" @click="quickLogin('manager', 'manager123')">
            <strong>店長:</strong> manager / manager123
          </div>
          <div class="demo-account" @click="quickLogin('stylist', 'stylist123')">
            <strong>設計師:</strong> stylist / stylist123
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { User, Lock } from '@element-plus/icons-vue'
import { useAuthStore } from '@/stores/auth'
import type { LoginCredentials } from '@/types/auth'

const router = useRouter()
const authStore = useAuthStore()

// Form reference
const loginFormRef = ref<FormInstance>()

// Form data
const loginForm = reactive<LoginCredentials>({
  userName: '',
  password: ''
})

// Loading state
const loading = ref(false)

// Form validation rules
const loginRules: FormRules = {
  userName: [
    { required: true, message: '請輸入帳號', trigger: 'blur' },
    { min: 3, max: 20, message: '帳號長度應為 3-20 個字符', trigger: 'blur' }
  ],
  password: [
    { required: true, message: '請輸入密碼', trigger: 'blur' },
    { min: 6, max: 50, message: '密碼長度應為 6-50 個字符', trigger: 'blur' }
  ]
}

// Handle login
const handleLogin = async () => {
  if (!loginFormRef.value) return
  
  try {
    const valid = await loginFormRef.value.validate()
    if (!valid) return
    
    loading.value = true
    
    const result = await authStore.login(loginForm)
    
    if (result.success) {
      ElMessage.success('登入成功')
      router.push('/dashboard')
    } else {
      ElMessage.error(result.message || '登入失敗')
    }
  } catch (error) {
    ElMessage.error('登入過程中發生錯誤')
  } finally {
    loading.value = false
  }
}

// Demo account quick login
const quickLogin = (userName: string, password: string) => {
  loginForm.userName = userName
  loginForm.password = password
  handleLogin()
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
  background: white;
  border-radius: 12px;
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
  padding: 40px;
  width: 100%;
  max-width: 400px;
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-header h1 {
  color: #409EFF;
  margin: 0 0 8px 0;
  font-size: 28px;
}

.login-header p {
  color: #666;
  margin: 0;
  font-size: 14px;
}

.login-form {
  margin-bottom: 20px;
}

.login-button {
  width: 100%;
  height: 40px;
  font-size: 16px;
}

.demo-info {
  margin-top: 20px;
}

.demo-accounts {
  font-size: 12px;
  color: #666;
}

.demo-account {
  margin: 4px 0;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.demo-account:hover {
  background-color: #f5f5f5;
}

.demo-account strong {
  color: #409EFF;
}

:deep(.el-form-item__label) {
  color: #333;
  font-weight: 500;
}
</style>