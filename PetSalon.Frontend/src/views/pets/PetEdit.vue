<template>
  <div class="pet-edit">
    <div class="header">
      <h1>編輯寵物</h1>
      <div class="header-actions">
        <el-button @click="$router.back()">
          <el-icon><ArrowLeft /></el-icon>
          返回
        </el-button>
        <el-button type="primary" :loading="submitLoading" @click="handleSubmit">
          <el-icon><Check /></el-icon>
          更新
        </el-button>
      </div>
    </div>

    <el-card class="form-card" v-loading="loading">
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
                  clearable
                />
              </el-form-item>

              <el-row :gutter="10">
                <el-col :span="12">
                  <el-form-item label="性別" prop="gender">
                    <SystemCodeSelect
                      v-model="formData.gender"
                      code-type="Gender"
                      placeholder="請選擇性別"
                      clearable
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
                <el-col :span="8">
                  <el-form-item label="年齡" prop="age">
                    <el-input-number
                      v-model="computedAge"
                      :min="0"
                      :max="30"
                      :precision="0"
                      placeholder="歲"
                      class="w-full"
                      disabled
                    />
                  </el-form-item>
                </el-col>
                <el-col :span="8">
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
                <el-col :span="8">
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

          <!-- 照片上傳 -->
          <el-col :span="12">
            <div class="form-section">
              <h3>寵物照片</h3>

              <el-form-item>
                <div class="photo-upload-container">
                  <!-- 照片預覽區域 -->
                  <div v-if="photoUrl" class="photo-preview-container">
                    <img
                      :src="photoUrl"
                      class="photo-preview"
                      alt="寵物照片預覽"
                    />
                    <div class="photo-actions">
                      <el-button
                        size="small"
                        type="primary"
                        @click="changePhoto"
                      >
                        更換照片
                      </el-button>
                      <el-button
                        size="small"
                        type="danger"
                        @click="removePhoto"
                      >
                        刪除照片
                      </el-button>
                    </div>
                  </div>

                  <!-- 上傳區域 -->
                  <el-upload
                    v-else
                    class="pet-photo-uploader"
                    :show-file-list="false"
                    :before-upload="beforeUpload"
                    :on-success="handlePhotoSuccess"
                    :on-error="handlePhotoError"
                    action="#"
                    :http-request="uploadPhoto"
                  >
                    <div class="upload-area">
                      <el-icon class="photo-uploader-icon">
                        <Plus />
                      </el-icon>
                      <div class="upload-text">點擊上傳照片</div>
                    </div>
                  </el-upload>

                  <!-- 隱藏的檔案選擇器，用於更換照片 -->
                  <input
                    ref="fileInputRef"
                    type="file"
                    accept="image/jpeg,image/png,image/jpg"
                    style="display: none"
                    @change="handleFileChange"
                  />
                </div>

                <div class="upload-tip">
                  建議上傳 JPG/PNG 格式圖片，且不超過 5MB（非必填項目）
                </div>
              </el-form-item>


            </div>
          </el-col>

        </el-row>
      </el-form>
    </el-card>

    <!-- 包月方案管理區域 -->
    <el-card class="subscription-card" v-if="petId">
      <SubscriptionManager :pet-id="petId" />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted, nextTick } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules, type UploadRequestOptions } from 'element-plus'
import { ArrowLeft, Check, Plus } from '@element-plus/icons-vue'
import { petApi } from '@/api/pet'
import { commonApi } from '@/api/common'
import { SystemCodeSelect } from '@/components/common'
import SubscriptionManager from '@/components/forms/SubscriptionManager.vue'
import type { PetUpdateRequest } from '@/types/pet'

const router = useRouter()
const route = useRoute()
const petId = Number(route.params.id)

// Validate petId
if (isNaN(petId) || petId <= 0) {
  ElMessage.error('無效的寵物ID')
  router.push('/pets')
}

// Form reference
const formRef = ref<FormInstance>()
const fileInputRef = ref<HTMLInputElement>()

// Loading states
const loading = ref(false)
const submitLoading = ref(false)
const photoUrl = ref('')

// Form data
const formData = reactive<PetUpdateRequest>({
  petId: petId,
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
const loadPet = async () => {
  if (!petId) {
    ElMessage.error('無效的寵物ID')
    router.back()
    return
  }

  loading.value = true
  try {
    const pet = await petApi.getPet(petId)

    // 確保資料正確綁定到表單
    formData.petId = pet.petId || petId
    formData.petName = pet.petName || ''
    formData.breed = pet.breed || ''
    formData.gender = pet.gender || ''
    formData.birthDay = pet.birthDay ? new Date(pet.birthDay) : undefined
    formData.normalPrice = pet.normalPrice
    formData.subscriptionPrice = pet.subscriptionPrice

    // 如果有照片URL，設定預覽
    if (pet.photoUrl) {
      photoUrl.value = pet.photoUrl
    }

    // 等待一點時間讓表單渲染完成，然後觸發重新驗證
    await nextTick()
    if (formRef.value) {
      formRef.value.clearValidate()
    }

  } catch (error) {
    ElMessage.error('載入寵物資料失敗')
    console.error('Failed to load pet:', error)
    router.back()
  } finally {
    loading.value = false
  }
}

const beforeUpload = (file: File) => {
  const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png'
  const isLt5M = file.size / 1024 / 1024 < 5

  if (!isJpgOrPng) {
    ElMessage.error('只能上傳 JPG/PNG 格式的圖片')
    return false
  }
  if (!isLt5M) {
    ElMessage.error('上傳圖片大小不能超過 5MB')
    return false
  }
  return true
}

const uploadPhoto = async (options: UploadRequestOptions) => {
  try {
    const response = await commonApi.uploadFile(options.file as File, 'pet')
    photoUrl.value = response.url
    options.onSuccess(response)
  } catch (error) {
    const uploadError = {
      status: 500,
      method: 'POST',
      url: '',
      message: error instanceof Error ? error.message : '上傳失敗'
    }
    options.onError(uploadError as any)
  }
}

const handlePhotoSuccess = () => {
  ElMessage.success('照片上傳成功')
}

const handlePhotoError = () => {
  ElMessage.error('照片上傳失敗')
}

const changePhoto = () => {
  if (fileInputRef.value) {
    fileInputRef.value.click()
  }
}

const removePhoto = () => {
  photoUrl.value = ''
  ElMessage.success('照片已移除')
}

const handleFileChange = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    const isValid = beforeUpload(file)
    if (isValid) {
      uploadPhotoFile(file)
    }
  }
  // 清空檔案選擇器
  target.value = ''
}

const uploadPhotoFile = async (file: File) => {
  try {
    const response = await commonApi.uploadFile(file, 'pet')
    photoUrl.value = response.url
    ElMessage.success('照片上傳成功')
  } catch (error) {
    console.error('Upload photo error:', error)
    ElMessage.error('照片上傳失敗')
  }
}

const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    const valid = await formRef.value.validate()
    if (!valid) return

    submitLoading.value = true

    // 包含照片URL的更新資料
    const updateData = {
      ...formData,
      photoUrl: photoUrl.value || undefined
    }

    await petApi.updatePet(updateData)

    // 更新成功後重新載入資料以確保所有欄位都是最新的
    await loadPet()

    ElMessage.success('寵物資料更新成功')
    // 不立即跳轉，讓用戶看到更新後的資料
    // router.push('/pets')
  } catch (error) {
    ElMessage.error('更新寵物資料失敗')
    console.error('Failed to update pet:', error)
  } finally {
    submitLoading.value = false
  }
}

// Computed properties
const computedAge = computed(() => {
  if (!formData.birthDay) return 0
  const today = new Date()
  const birthDate = new Date(formData.birthDay)
  let age = today.getFullYear() - birthDate.getFullYear()
  const monthDiff = today.getMonth() - birthDate.getMonth()
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    age--
  }
  return Math.max(0, age)
})

// Watchers
watch(() => formData.birthDay, (newBirthDay) => {
  // 當生日改變時，年齡會自動重新計算（因為使用了computed）
  // 這裡可以添加額外的邏輯，如果需要的話
  console.log('Birthday changed, age will be recalculated:', computedAge.value)
}, { deep: true })

// Lifecycle
onMounted(() => {
  loadPet()
})
</script>

<style scoped>
.pet-edit {
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

.subscription-card {
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

.photo-upload-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.photo-preview-container {
  position: relative;
  display: inline-block;
}

.photo-preview {
  width: 200px;
  height: 200px;
  object-fit: cover;
  border-radius: 8px;
  border: 2px solid #dcdfe6;
}

.photo-actions {
  display: flex;
  gap: 8px;
  margin-top: 8px;
}

.upload-area {
  width: 200px;
  height: 200px;
  border: 2px dashed #d9d9d9;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: border-color 0.2s;
}

.upload-area:hover {
  border-color: #409eff;
}

.photo-uploader-icon {
  font-size: 32px;
  color: #8c939d;
}

.upload-text {
  font-size: 14px;
  color: #8c939d;
  margin-top: 8px;
}

.upload-tip {
  font-size: 12px;
  color: #999;
  margin-top: 8px;
}

.pet-photo-uploader {
  display: inline-block;
}

.pet-photo-uploader :deep(.el-upload) {
  border: none;
}

:deep(.el-form-item__label) {
  font-weight: 500;
  color: #333;
}
</style>