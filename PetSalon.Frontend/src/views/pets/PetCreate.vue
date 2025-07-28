<template>
  <div class="pet-create">
    <div class="header">
      <h1>新增寵物</h1>
      <div class="header-actions">
        <el-button @click="$router.back()">
          <el-icon><ArrowLeft /></el-icon>
          返回
        </el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleSubmit">
          <el-icon><Check /></el-icon>
          儲存
        </el-button>
      </div>
    </div>

    <el-card class="form-card">
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
        label-position="top"
      >
        <el-row :gutter="20">
          <!-- 基本資訊 -->
          <el-col :span="12">
            <div class="form-section">
              <h3>基本資訊</h3>
              
              <el-form-item label="寵物名稱" prop="petName">
                <el-input v-model="formData.petName" placeholder="請輸入寵物名稱" maxlength="50" show-word-limit />
              </el-form-item>

              <el-form-item label="品種" prop="breed">
                <SystemCodeSelect
                  v-model="formData.breed"
                  code-type="Breed"
                  placeholder="請選擇品種"
                />
              </el-form-item>

              <el-row :gutter="10">
                <el-col :span="12">
                  <el-form-item label="性別" prop="gender">
                    <SystemCodeSelect
                      v-model="formData.gender"
                      code-type="Gender"
                      placeholder="請選擇性別"
                    />
                  </el-form-item>
                </el-col>
                <el-col :span="12">
                  <el-form-item label="生日" prop="birthDay">
                    <el-date-picker 
                      v-model="formData.birthDay" 
                      type="date"
                      placeholder="請選擇生日"
                      class="w-full"
                    />
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row :gutter="10">
                <el-col :span="12">
                  <el-form-item label="一般價格" prop="normalPrice">
                    <el-input-number 
                      v-model="formData.normalPrice" 
                      :min="0" 
                      :precision="0" 
                      placeholder="元"
                      class="w-full"
                    />
                  </el-form-item>
                </el-col>
                <el-col :span="12">
                  <el-form-item label="包月價格" prop="subscriptionPrice">
                    <el-input-number 
                      v-model="formData.subscriptionPrice" 
                      :min="0" 
                      :precision="0" 
                      placeholder="元"
                      class="w-full"
                    />
                  </el-form-item>
                </el-col>
              </el-row>

            </div>
          </el-col>

        </el-row>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules, type UploadRequestOptions } from 'element-plus'
import { petApi } from '@/api/pet'
import { SystemCodeSelect } from '@/components/common'
import type { PetCreateRequest } from '@/types/pet'

const router = useRouter()

// Form reference
const formRef = ref<FormInstance>()

// Loading states
const submitLoading = ref(false)

// Form data
const formData = reactive<PetCreateRequest>({
  petName: '',
  breed: '',
  gender: '',
  birthDay: undefined,
  normalPrice: undefined,
  subscriptionPrice: undefined
})

// Form validation rules
const formRules: FormRules = {
  petName: [
    { required: true, message: '請輸入寵物名稱', trigger: 'blur' },
    { min: 1, max: 50, message: '寵物名稱長度應為 1-50 個字符', trigger: 'blur' }
  ],
  breed: [
    { required: true, message: '請選擇品種', trigger: 'change' }
  ],
  gender: [
    { required: true, message: '請選擇性別', trigger: 'change' }
  ]
}

// Methods
const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    const valid = await formRef.value.validate()
    if (!valid) return

    submitLoading.value = true

    await petApi.createPet(formData)
    
    ElMessage.success('寵物新增成功')
    router.push('/pets')
  } catch (error) {
    ElMessage.error('新增寵物失敗')
    console.error('Failed to create pet:', error)
  } finally {
    submitLoading.value = false
  }
}

// No initialization needed - SystemCodeSelect handles its own data loading
</script>

<style scoped>
.pet-create {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.header h1 {
  margin: 0;
  color: #333;
}

.header-actions {
  display: flex;
  gap: 12px;
}

.form-card {
  margin-bottom: 20px;
}

.form-section {
  margin-bottom: 20px;
}

.form-section h3 {
  margin: 0 0 16px 0;
  color: #409EFF;
  border-bottom: 2px solid #409EFF;
  padding-bottom: 8px;
  font-size: 16px;
}

.w-full {
  width: 100%;
}

.contact-option {
  display: flex;
  flex-direction: column;
}

.contact-name {
  font-weight: 500;
  color: #333;
}

.contact-phone {
  font-size: 12px;
  color: #666;
}

.contact-info {
  margin-top: 16px;
  padding: 16px;
  background-color: #f8f9fa;
  border-radius: 6px;
}

.pet-photo-uploader {
  text-align: center;
}

.pet-photo-uploader .photo-preview {
  width: 200px;
  height: 200px;
  object-fit: cover;
  border-radius: 8px;
  border: 2px dashed #dcdfe6;
}

.pet-photo-uploader .photo-uploader-icon {
  font-size: 32px;
  color: #8c939d;
  width: 200px;
  height: 200px;
  line-height: 200px;
  border: 2px dashed #dcdfe6;
  border-radius: 8px;
  transition: border-color 0.3s;
}

.pet-photo-uploader .photo-uploader-icon:hover {
  border-color: #409EFF;
  color: #409EFF;
}

.photo-uploader-text {
  margin-top: 8px;
  color: #666;
  font-size: 14px;
}

.upload-tip {
  margin-top: 8px;
  color: #999;
  font-size: 12px;
  text-align: center;
}

:deep(.el-form-item__label) {
  font-weight: 500;
  color: #333;
}

:deep(.el-divider__text) {
  color: #666;
  font-weight: 500;
}

:deep(.el-descriptions__title) {
  color: #333;
  font-weight: 500;
  margin-bottom: 12px;
}
</style>