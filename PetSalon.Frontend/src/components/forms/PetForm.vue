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
      <!-- 寵物選擇控制項 -->
      <el-form-item v-if="!isEdit" label="選擇寵物">
        <el-select
          ref="petSelectRef"
          v-model="selectedPetId"
          placeholder="請選擇寵物或新增新寵物"
          filterable
          remote
          :remote-method="searchPets"
          :loading="petLoading"
          clearable
          @change="handlePetSelect"
          @focus="handlePetSelectFocus"
          @blur="handlePetSelectBlur"
        >
          <el-option
            v-for="pet in availablePets"
            :key="pet.petId"
            :label="`${pet.petName} (${pet.breed})`"
            :value="pet.petId"
          />
        </el-select>
        <div class="select-tip">
          選擇現有寵物可快速填入資料，或直接填寫表單新增寵物
        </div>
      </el-form-item>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="寵物名稱" prop="name">
            <el-input v-model="form.name" placeholder="請輸入寵物名稱" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="品種" prop="breed">
            <SystemCodeSelect
              v-model="form.breed"
              code-type="Breed"
              placeholder="請選擇品種"
              clearable
              @change="handleBreedChange"
              ref="breedSelectRef"
            />
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
            <SystemCodeSelect
              v-model="form.gender"
              code-type="Gender"
              placeholder="請選擇性別"
              clearable
              @change="handleGenderChange"
              ref="genderSelectRef"
            />
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
        <div class="photo-upload-container">
          <!-- 照片預覽區域 -->
          <div v-if="photoUrl || form.photo" class="photo-preview-container">
            <img
              :src="photoUrl || form.photo"
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
          建議上傳 JPG/PNG 格式圖片，檔案大小不超過 5MB
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

      <!-- 聯絡人管理 (僅在編輯模式顯示) -->
      <div v-if="isEdit && props.pet?.petId">
        <el-divider />
        <PetContactsManager
          :pet-id="props.pet.petId"
          @contacts-updated="handleContactsUpdated"
        />
      </div>
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
import type { Pet, PetUpdateRequest } from '@/types/pet'
import { petApi } from '@/api/pet'
import { commonApi } from '@/api/common'
import PetContactsManager from './PetContactsManager.vue'
import { SystemCodeSelect } from '@/components/common'

// 支援 PetViewModel 或 Pet 型別
type PetLike = {
  id?: number
  petId?: number
  name?: string
  petName?: string
  breed?: string | number
  breedName?: string
  age?: number
  gender?: string
  color?: string
  weight?: number
  note?: string
  ownerId?: number
  ownerName?: string
  contactPhone?: string
  photoUrl?: string
  photo?: string
  [key: string]: any
}
interface Props {
  visible: boolean
  pet?: PetLike | null
}

interface Emits {
  (e: 'close'): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Refs
const formRef = ref<FormInstance>()
const petSelectRef = ref()
const breedSelectRef = ref()
const genderSelectRef = ref()
const fileInputRef = ref<HTMLInputElement>()
const submitting = ref(false)
const ownerLoading = ref(false)
const petLoading = ref(false)
const photoUrl = ref('')
const uploadedPhoto = ref<File | null>(null)
const selectedPetId = ref<number | null>(null)

// Data
const owners = ref<any[]>([])
const availablePets = ref<Pet[]>([])

// Computed
const isEdit = computed(() => !!props.pet)

// Form data
const form = reactive({
  petName: '',
  name: '',
  breed: '',
  age: 0,
  gender: 'M',
  color: '',
  weight: 0,
  ownerId: null as number | null,
  note: '',
  photo: '',
  birthDay: undefined as Date | undefined,
  normalPrice: undefined as number | undefined,
  subscriptionPrice: undefined as number | undefined
})

// Form rules
const rules: FormRules = {
  name: [
    { required: true, message: '請輸入寵物名稱', trigger: 'blur' },
    { min: 1, max: 50, message: '名稱長度應為 1-50 個字符', trigger: 'blur' }
  ],
  petName: [
    { required: true, message: '請輸入寵物名稱', trigger: 'blur' },
    { min: 1, max: 50, message: '名稱長度應為 1-50 個字符', trigger: 'blur' }
  ],
  breed: [
    { required: true, message: '請選擇品種', trigger: 'change' }
  ],
  gender: [
    { required: true, message: '請選擇性別', trigger: 'change' }
  ]
  // 移除非必要欄位的驗證規則，避免不必要的錯誤訊息
}

// Methods

const searchOwners = async (query: string) => {
  if (!query) {
    owners.value = []
    return
  }

  ownerLoading.value = true
  try {
    // 暫時使用模擬資料，待聯絡人 API 完成後更新
    if (query.length >= 2) {
      owners.value = [
        { id: 1, name: '張三', phone: '0912345678' },
        { id: 2, name: '李四', phone: '0923456789' }
      ].filter(owner =>
        owner.name.includes(query) ||
        owner.phone.includes(query)
      )
    } else {
      owners.value = []
    }
  } catch (error) {
    console.error('Search owners error:', error)
    ElMessage.error('搜尋主人失敗')
  } finally {
    ownerLoading.value = false
  }
}

// 寵物搜尋方法
const searchPets = async (query: string) => {
  if (!query) {
    availablePets.value = []
    return
  }

  petLoading.value = true
  try {
    const response = await petApi.getPets({
      keyword: query,
      pageSize: 20
    })
    availablePets.value = response.data
  } catch (error) {
    console.error('Search pets error:', error)
    ElMessage.error('搜尋寵物失敗')
  } finally {
    petLoading.value = false
  }
}

// 寵物選擇處理
const handlePetSelect = async (petId: number | null) => {
  if (!petId) {
    resetForm()
    return
  }

  try {
    const pet = await petApi.getPet(petId)
    Object.assign(form, {
      petName: pet.petName,
      name: pet.petName,
      breed: pet.breed,
      gender: pet.gender,
      birthDay: pet.birthDay ? new Date(pet.birthDay) : undefined,
      normalPrice: pet.normalPrice,
      subscriptionPrice: pet.subscriptionPrice
    })
    ElMessage.success('已載入寵物資料')
  } catch (error) {
    console.error('Load pet error:', error)
    ElMessage.error('載入寵物資料失敗')
  }
}

// 寵物選擇控制項焦點處理
const handlePetSelectFocus = () => {
  // 確保控制項獲得焦點時保持狀態
  if (petSelectRef.value) {
    petSelectRef.value.focus()
  }
}

const handlePetSelectBlur = () => {
  // 失去焦點時的處理，可以在這裡加入驗證邏輯
}

// 品種選擇處理
const handleBreedChange = (value: string) => {
  // 品種選擇後保持選中狀態
  form.breed = value
  // 觸發表單驗證
  if (formRef.value) {
    formRef.value.validateField('breed')
  }
}

// 性別選擇處理
const handleGenderChange = (value: string) => {
  // 性別選擇後保持選中狀態
  form.gender = value
  // 觸發表單驗證
  if (formRef.value) {
    formRef.value.validateField('gender')
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
    const response = await commonApi.uploadFile(options.file as File, 'pets')
    photoUrl.value = response.url
    form.photo = response.url
    uploadedPhoto.value = options.file as File
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
        petId: props.pet.petId || props.pet.id || 0
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

// 聯絡人更新處理
const handleContactsUpdated = () => {
  // 聯絡人資料更新後的處理，可以在這裡重新載入寵物資料
  ElMessage.success('聯絡人資料已更新')
}

// 照片處理方法
const changePhoto = () => {
  if (fileInputRef.value) {
    fileInputRef.value.click()
  }
}

const removePhoto = () => {
  photoUrl.value = ''
  form.photo = ''
  uploadedPhoto.value = null
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
    const response = await commonApi.uploadFile(file, 'pets')
    photoUrl.value = response.url
    form.photo = response.url
    uploadedPhoto.value = file
    ElMessage.success('照片上傳成功')
  } catch (error) {
    console.error('Upload photo error:', error)
    ElMessage.error('照片上傳失敗')
  }
}

const resetForm = () => {
  if (formRef.value) {
    formRef.value.resetFields()
  }
  Object.assign(form, {
    petName: '',
    name: '',
    breed: '',
    age: 0,
    gender: 'M',
    color: '',
    weight: 0,
    ownerId: null,
    note: '',
    photo: '',
    birthDay: undefined,
    normalPrice: undefined,
    subscriptionPrice: undefined
  })
  photoUrl.value = ''
  uploadedPhoto.value = null
  owners.value = []
  availablePets.value = []
  selectedPetId.value = null
}

// Watch for pet changes
watch(() => props.pet, (newPet) => {
  console.log('PetForm - watching pet change:', newPet)
  if (newPet) {
    // 確保所有欄位都正確對應
    Object.assign(form, {
      petName: newPet.petName || newPet.name || '',
      name: newPet.petName || newPet.name || '',
      breed: newPet.breed || '', // 使用原始的 breed ID
      age: newPet.age || 0,
      gender: newPet.gender || 'M',
      color: newPet.color || '',
      weight: newPet.weight || 0,
      ownerId: newPet.ownerId || null,
      note: newPet.note || '',
      photo: newPet.photo || newPet.photoUrl || '',
      birthDay: newPet.birthDay ? new Date(newPet.birthDay) : undefined,
      normalPrice: newPet.normalPrice || undefined,
      subscriptionPrice: newPet.subscriptionPrice || undefined
    })
    photoUrl.value = newPet.photoUrl || newPet.photo || ''

    console.log('PetForm - form data after assignment:', form)

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
  if (!visible) {
    resetForm()
  }
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
  width: 120px;
  height: 120px;
  object-fit: cover;
  border-radius: 6px;
  border: 1px solid #dcdfe6;
}

.photo-actions {
  display: flex;
  gap: 8px;
  margin-top: 8px;
}

.upload-area {
  width: 120px;
  height: 120px;
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
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

.upload-text {
  font-size: 12px;
  color: #8c939d;
  margin-top: 8px;
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

.select-tip {
  font-size: 12px;
  color: #999;
  margin-top: 4px;
}

/* 確保寵物選擇控制項在選擇後保持焦點樣式 */
.el-select.is-focus :deep(.el-input__wrapper) {
  box-shadow: 0 0 0 1px #409eff inset;
}

.el-select :deep(.el-input__wrapper) {
  transition: box-shadow 0.2s;
}

.el-select:hover :deep(.el-input__wrapper) {
  box-shadow: 0 0 0 1px #c0c4cc inset;
}
</style>