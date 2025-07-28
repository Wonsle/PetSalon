<template>
  <div class="contact-create-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>新增聯絡人</h2>
        <el-breadcrumb separator=">">
          <el-breadcrumb-item :to="{ path: '/contacts' }">聯絡人管理</el-breadcrumb-item>
          <el-breadcrumb-item>新增聯絡人</el-breadcrumb-item>
        </el-breadcrumb>
      </div>
      <div class="header-right">
        <el-button @click="$router.back()">
          <el-icon><ArrowLeft /></el-icon>
          返回
        </el-button>
      </div>
    </div>

    <!-- Form -->
    <el-card>
      <el-form
        ref="formRef"
        :model="form"
        :rules="rules"
        label-width="120px"
        @submit.prevent="handleSubmit"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="姓名" prop="name">
              <el-input v-model="form.name" placeholder="請輸入姓名" maxlength="50" show-word-limit />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="暱稱" prop="nickName">
              <el-input v-model="form.nickName" placeholder="請輸入暱稱(選填)" maxlength="50" show-word-limit />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="聯絡電話" prop="contactNumber">
          <el-input v-model="form.contactNumber" placeholder="請輸入聯絡電話" maxlength="20" show-word-limit />
        </el-form-item>

        <!-- Submit Buttons -->
        <el-form-item>
          <el-button type="primary" :loading="submitting" @click="handleSubmit">
            新增聯絡人
          </el-button>
          <el-button @click="$router.back()">取消</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { ArrowLeft } from '@element-plus/icons-vue'
import type { ContactCreateRequest } from '@/types/contact'
import { contactApi } from '@/api/contact'

const router = useRouter()

// Refs
const formRef = ref<FormInstance>()
const submitting = ref(false)

// Form data
const form = reactive<ContactCreateRequest>({
  name: '',
  nickName: '',
  contactNumber: ''
})

// Form rules
const rules: FormRules = {
  name: [
    { required: true, message: '請輸入姓名', trigger: 'blur' },
    { min: 1, max: 50, message: '姓名長度應為 1-50 個字符', trigger: 'blur' }
  ],
  nickName: [
    { max: 50, message: '暱稱長度不能超過50個字符', trigger: 'blur' }
  ],
  contactNumber: [
    { required: true, message: '請輸入聯絡電話', trigger: 'blur' },
    { min: 1, max: 20, message: '聯絡電話長度應為 1-20 個字符', trigger: 'blur' }
  ]
}

// Methods
const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    const valid = await formRef.value.validate()
    if (!valid) return

    submitting.value = true

    await contactApi.createContact(form)
    ElMessage.success('新增成功')
    router.push('/contacts')
  } catch (error: any) {
    console.error('Submit error:', error)
    ElMessage.error(error.response?.data?.message || '新增失敗')
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.contact-create-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 20px;
}

.header-left h2 {
  margin: 0 0 8px 0;
  color: #303133;
}
</style>