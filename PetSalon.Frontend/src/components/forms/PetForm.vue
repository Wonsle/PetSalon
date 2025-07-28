<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? '編輯寵物' : '新增寵物'"
    width="600px"
    :before-close="handleClose"
    @update:model-value="$emit('close')"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
      @submit.prevent="handleSubmit"
    >
      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="寵物名稱" prop="name">
            <el-input v-model="form.name" placeholder="請輸入寵物名稱" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="品種" prop="breed">
            <el-select v-model="form.breed" placeholder="請選擇品種" filterable>
              <el-option
                v-for="breed in breeds"
                :key="breed.id"
                :label="breed.name"
                :value="breed.id"
              />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="8">
          <el-form-item label="年齡" prop="age">
            <el-input-number
              v-model="form.age"
              :min="0"
              :max="30"
              controls-position="right"
              class="w-full"
            />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="性別" prop="gender">
            <el-radio-group v-model="form.gender">
              <el-radio value="M">公</el-radio>
              <el-radio value="F">母</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="體重(kg)" prop="weight">
            <el-input-number
              v-model="form.weight"
              :min="0"
              :precision="1"
              :step="0.1"
              controls-position="right"
              class="w-full"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="毛色" prop="color">
        <el-input v-model="form.color" placeholder="請輸入毛色" />
      </el-form-item>

      <el-form-item label="主人" prop="ownerId">
        <el-select
          v-model="form.ownerId"
          placeholder="請選擇主人"
          filterable
          remote
          :remote-method="searchOwners"
          :loading="ownerLoading"
        >
          <el-option
            v-for="owner in owners"
            :key="owner.id"
            :label="`${owner.name} (${owner.phone})`"
            :value="owner.id"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="寵物照片">
        <el-upload
          class="pet-photo-uploader"
          :show-file-list="false"
          :before-upload="beforeUpload"
          :on-success="handlePhotoSuccess"
          :on-error="handlePhotoError"
          action="#"
          :http-request="uploadPhoto"
        >
          <img v-if="photoUrl" :src="photoUrl" class="photo-preview" />
          <el-icon v-else class="photo-uploader-icon">
            <Plus />
          </el-icon>
        </el-upload>
        <div class="upload-tip">
          建議上傳 JPG/PNG 格式圖片，且不超過 2MB
        </div>
      </el-form-item>

      <el-form-item label="備註">
        <el-input
          v-model="form.note"
          type="textarea"
          :rows="3"
          placeholder="請輸入備註"
        />
      </el-form-item>
    </el-form>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">
          {{ isEdit ? '更新' : '新增' }}
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, onMounted, computed } from 'vue'
import { ElMessage, type FormInstance, type FormRules, type UploadRequestOptions } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import type { Pet, PetCreateRequest, PetUpdateRequest } from '@/types/pet'
import { petApi } from '@/api/pet'
import { commonApi } from '@/api/common'

interface Props {
  visible: boolean
  pet?: Pet | null
}

interface Emits {
  (e: 'close'): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Refs
const formRef = ref<FormInstance>()
const submitting = ref(false)
const ownerLoading = ref(false)
const photoUrl = ref('')
const uploadedPhoto = ref<File | null>(null)

// Data
const breeds = ref<any[]>([])
const owners = ref<any[]>([])

// Computed
const isEdit = computed(() => !!props.pet)

// Form data
const form = reactive<PetCreateRequest>({
  name: '',
  breed: 0,
  age: 1,
  gender: 'M',
  color: '',
  weight: 0,
  note: '',
  ownerId: 0
})

// Form rules
const rules: FormRules = {
  name: [
    { required: true, message: '請輸入寵物名稱', trigger: 'blur' },
    { min: 1, max: 50, message: '名稱長度應為 1-50 個字符', trigger: 'blur' }
  ],
  breed: [
    { required: true, message: '請選擇品種', trigger: 'change' }
  ],
  age: [
    { required: true, message: '請輸入年齡', trigger: 'blur' }
  ],
  gender: [
    { required: true, message: '請選擇性別', trigger: 'change' }
  ],
  color: [
    { required: true, message: '請輸入毛色', trigger: 'blur' }
  ],
  weight: [
    { required: true, message: '請輸入體重', trigger: 'blur' }
  ],
  ownerId: [
    { required: true, message: '請選擇主人', trigger: 'change' }
  ]
}

// Methods
const loadBreeds = async () => {
  try {
    breeds.value = await commonApi.getBreeds()
  } catch (error) {
    ElMessage.error('載入品種清單失敗')
  }
}

const searchOwners = async (query: string) => {
  if (!query) {
    owners.value = []
    return
  }
  
  ownerLoading.value = true
  try {
    // TODO: Implement contact person search API
    // const response = await contactApi.searchContacts({ keyword: query, limit: 20 })
    // owners.value = response.data
    owners.value = [] // Placeholder
  } catch (error) {
    ElMessage.error('搜尋主人失敗')
  } finally {
    ownerLoading.value = false
  }
}

const beforeUpload = (file: File) => {
  const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png'
  const isLt2M = file.size / 1024 / 1024 < 2

  if (!isJpgOrPng) {
    ElMessage.error('只能上傳 JPG/PNG 格式的圖片')
    return false
  }
  if (!isLt2M) {
    ElMessage.error('上傳圖片大小不能超過 2MB')
    return false
  }
  return true
}

const uploadPhoto = async (options: UploadRequestOptions) => {
  try {
    const response = await commonApi.uploadFile(options.file as File, 'pet')
    photoUrl.value = response.url
    uploadedPhoto.value = options.file as File
    options.onSuccess(response)
  } catch (error) {
    options.onError(error as Error)
  }
}

const handlePhotoSuccess = () => {
  ElMessage.success('照片上傳成功')
}

const handlePhotoError = () => {
  ElMessage.error('照片上傳失敗')
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    const valid = await formRef.value.validate()
    if (!valid) return
    
    submitting.value = true
    
    const requestData = {
      ...form,
      photo: uploadedPhoto.value || undefined
    }
    
    if (isEdit.value && props.pet) {
      const updateData: PetUpdateRequest = {
        ...requestData,
        id: props.pet.id
      }
      await petApi.updatePet(updateData)
      ElMessage.success('更新成功')
    } else {
      await petApi.createPet(requestData)
      ElMessage.success('新增成功')
    }
    
    emit('success')
  } catch (error: any) {
    ElMessage.error(error.response?.data?.message || '操作失敗')
  } finally {
    submitting.value = false
  }
}

const handleClose = () => {
  emit('close')
}

const resetForm = () => {
  if (formRef.value) {
    formRef.value.resetFields()
  }
  photoUrl.value = ''
  uploadedPhoto.value = null
  owners.value = []
}

// Watch for pet changes
watch(() => props.pet, (newPet) => {
  if (newPet) {
    Object.assign(form, {
      name: newPet.name,
      breed: newPet.breed,
      age: newPet.age,
      gender: newPet.gender,
      color: newPet.color,
      weight: newPet.weight,
      note: newPet.note,
      ownerId: newPet.ownerId
    })
    photoUrl.value = newPet.photoUrl || ''
    
    // Load owner info
    if (newPet.ownerId) {
      owners.value = [{
        id: newPet.ownerId,
        name: newPet.ownerName,
        phone: newPet.contactPhone
      }]
    }
  } else {
    resetForm()
  }
}, { immediate: true })

// Watch for dialog visibility
watch(() => props.visible, (visible) => {
  if (visible) {
    loadBreeds()
  } else {
    resetForm()
  }
})

// Lifecycle
onMounted(() => {
  loadBreeds()
})
</script>

<style scoped>
.w-full {
  width: 100%;
}

.pet-photo-uploader {
  display: inline-block;
}

.pet-photo-uploader :deep(.el-upload) {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: 0.2s;
  width: 120px;
  height: 120px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.pet-photo-uploader :deep(.el-upload:hover) {
  border-color: #409eff;
}

.photo-uploader-icon {
  font-size: 28px;
  color: #8c939d;
}

.photo-preview {
  width: 120px;
  height: 120px;
  object-fit: cover;
  border-radius: 6px;
}

.upload-tip {
  font-size: 12px;
  color: #999;
  margin-top: 8px;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>