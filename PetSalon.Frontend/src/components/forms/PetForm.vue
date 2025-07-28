<template>
  <Dialog
    :visible="visible"
    :header="isEdit ? '編輯寵物' : '新增寵物'"
    :style="{ width: '600px' }"
    modal
    @update:visible="$emit('close')"
  >
    <form @submit.prevent="handleSubmit" class="pet-form">
      <!-- 寵物選擇控制項 -->
      <div v-if="!isEdit" class="form-field">
        <label class="form-label">選擇寵物</label>
        <Select
          ref="petSelectRef"
          v-model="selectedPetId"
          :options="availablePets"
          optionLabel="petName"
          optionValue="petId"
          placeholder="請選擇寵物或新增新寵物"
          filter
          :loading="petLoading"
          showClear
          @change="handlePetSelect"
          @focus="handlePetSelectFocus"
          @blur="handlePetSelectBlur"
          class="w-full"
        />
        <small class="form-help">
          選擇現有寵物可快速填入資料，或直接填寫表單新增寵物
        </small>
      </div>

      <div class="form-row">
        <div class="form-field">
          <label class="form-label required">寵物名稱</label>
          <InputText
            v-model="form.name"
            placeholder="請輸入寵物名稱"
            :invalid="!!errors.name"
          />
          <small v-if="errors.name" class="form-error">{{ errors.name }}</small>
        </div>
        <div class="form-field">
          <label class="form-label required">品種</label>
          <SystemCodeSelect
            v-model="form.breed"
            code-type="Breed"
            placeholder="請選擇品種"
            clearable
            @change="handleBreedChange"
            ref="breedSelectRef"
          />
          <small v-if="errors.breed" class="form-error">{{ errors.breed }}</small>
        </div>
      </div>

      <div class="form-row">
        <div class="form-field">
          <label class="form-label">年齡</label>
          <InputNumber
            v-model="form.age"
            :min="0"
            :max="30"
            showButtons
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label class="form-label required">性別</label>
          <SystemCodeSelect
            v-model="form.gender"
            code-type="Gender"
            placeholder="請選擇性別"
            clearable
            @change="handleGenderChange"
            ref="genderSelectRef"
          />
          <small v-if="errors.gender" class="form-error">{{ errors.gender }}</small>
        </div>
        <div class="form-field">
          <label class="form-label">體重(kg)</label>
          <InputNumber
            v-model="form.weight"
            :min="0"
            :minFractionDigits="1"
            :maxFractionDigits="1"
            :step="0.1"
            showButtons
            class="w-full"
          />
        </div>
      </div>

      <div class="form-row">
        <div class="form-field">
          <label class="form-label">單次價格</label>
          <InputNumber
            v-model="form.normalPrice"
            :min="0"
            :minFractionDigits="0"
            :maxFractionDigits="0"
            suffix=" 元"
            showButtons
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label class="form-label">包月價格</label>
          <InputNumber
            v-model="form.subscriptionPrice"
            :min="0"
            :minFractionDigits="0"
            :maxFractionDigits="0"
            suffix=" 元"
            showButtons
            class="w-full"
          />
        </div>
      </div>

      <div class="form-field">
        <label class="form-label">毛色</label>
        <InputText v-model="form.color" placeholder="請輸入毛色" />
      </div>

      <div class="form-field">
        <label class="form-label">主人（選填）</label>
        <Select
          v-model="form.ownerId"
          :options="owners"
          optionLabel="name"
          optionValue="id"
          placeholder="請選擇主人（選填）"
          filter
          showClear
          :loading="ownerLoading"
          @filter="searchOwners"
          class="w-full"
        />
      </div>

      <div class="form-field">
        <label class="form-label">寵物照片</label>
        <div class="photo-upload-container">
          <!-- 照片預覽區域 -->
          <div v-if="photoUrl || form.photo" class="photo-preview-container">
            <Image
              :src="photoUrl || form.photo"
              alt="寵物照片預覽"
              width="120"
              height="120"
              preview
              class="photo-preview"
            />
            <div class="photo-actions">
              <Button
                label="更換照片"
                size="small"
                @click="changePhoto"
              />
              <Button
                label="刪除照片"
                severity="danger"
                size="small"
                @click="removePhoto"
              />
            </div>
          </div>

          <!-- 上傳區域 -->
          <FileUpload
            v-else
            mode="basic"
            name="photo"
            accept="image/*"
            :maxFileSize="5000000"
            @upload="handlePhotoSuccess"
            @before-upload="beforeUpload"
            auto
            chooseLabel="選擇照片"
            class="pet-photo-uploader"
          />

          <!-- 隱藏的檔案選擇器，用於更換照片 -->
          <input
            ref="fileInputRef"
            type="file"
            accept="image/jpeg,image/png,image/jpg"
            style="display: none"
            @change="handleFileChange"
          />
        </div>

        <small class="form-help">
          建議上傳 JPG/PNG 格式圖片，檔案大小不超過 5MB
        </small>
      </div>

      <div class="form-field">
        <label class="form-label">備註</label>
        <Textarea
          v-model="form.note"
          rows="3"
          placeholder="請輸入備註"
          class="w-full"
        />
      </div>

      <!-- 聯絡人管理 (僅在編輯模式顯示) -->
      <div v-if="isEdit && props.pet?.petId" class="contacts-section">
        <Divider />
        <PetContactsManager
          :pet-id="props.pet.petId"
          @contacts-updated="handleContactsUpdated"
        />
      </div>
    </form>

    <template #footer>
      <div class="dialog-footer">
        <Button label="取消" severity="secondary" @click="handleClose" />
        <Button
          :label="isEdit ? '更新' : '新增'"
          :loading="submitting"
          @click="handleSubmit"
        />
      </div>
    </template>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, onMounted, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
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
const toast = useToast()

// Refs
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

// Form validation errors
const errors = ref<Record<string, string>>({})

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

// Form validation
const validateForm = () => {
  errors.value = {}

  if (!form.name) {
    errors.value.name = '請輸入寵物名稱'
  } else if (form.name.length < 1 || form.name.length > 50) {
    errors.value.name = '名稱長度應為 1-50 個字符'
  }

  if (!form.breed) {
    errors.value.breed = '請選擇品種'
  }

  if (!form.gender) {
    errors.value.gender = '請選擇性別'
  }

  return Object.keys(errors.value).length === 0
}

// Methods

const searchOwners = async (event: any) => {
  const query = event.value || event
  if (!query) {
    owners.value = []
    return
  }

  ownerLoading.value = true
  try {
    // 暫時使用模擬資料，待聯絡人 API 完成後更新
    if (query && query.length >= 2) {
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
    toast.add({
      severity: 'error',
      summary: '搜尋失敗',
      detail: '搜尋主人失敗',
      life: 3000
    })
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
    toast.add({
      severity: 'error',
      summary: '搜尋失敗',
      detail: '搜尋寵物失敗',
      life: 3000
    })
  } finally {
    petLoading.value = false
  }
}

// 寵物選擇處理
const handlePetSelect = async (event: any) => {
  const petId = event.value
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
    toast.add({
      severity: 'success',
      summary: '載入成功',
      detail: '已載入寵物資料',
      life: 3000
    })
  } catch (error) {
    console.error('Load pet error:', error)
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: '載入寵物資料失敗',
      life: 3000
    })
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
  form.breed = value
  // 清除相關錯誤
  if (errors.value.breed) {
    delete errors.value.breed
  }
}

// 性別選擇處理
const handleGenderChange = (value: string) => {
  form.gender = value
  // 清除相關錯誤
  if (errors.value.gender) {
    delete errors.value.gender
  }
}

const beforeUpload = (event: any) => {
  const file = event.files?.[0] || event
  const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png'
  const isLt5M = file.size / 1024 / 1024 < 5

  if (!isJpgOrPng) {
    toast.add({
      severity: 'error',
      summary: '檔案格式錯誤',
      detail: '只能上傳 JPG/PNG 格式的圖片',
      life: 3000
    })
    return false
  }
  if (!isLt5M) {
    toast.add({
      severity: 'error',
      summary: '檔案太大',
      detail: '上傳圖片大小不能超過 5MB',
      life: 3000
    })
    return false
  }
  return true
}

const uploadPhoto = async (file: File) => {
  try {
    const response = await commonApi.uploadFile(file, 'pets')
    photoUrl.value = response.url
    form.photo = response.url
    uploadedPhoto.value = file
    return response
  } catch (error) {
    throw error
  }
}

const handlePhotoSuccess = (event: any) => {
  toast.add({
    severity: 'success',
    summary: '上傳成功',
    detail: '照片上傳成功',
    life: 3000
  })
}

const handlePhotoError = () => {
  toast.add({
    severity: 'error',
    summary: '上傳失敗',
    detail: '照片上傳失敗',
    life: 3000
  })
}

const handleSubmit = async () => {
  if (!validateForm()) return

  try {
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
      toast.add({
        severity: 'success',
        summary: '更新成功',
        detail: '寵物資料已更新',
        life: 3000
      })
    } else {
      await petApi.createPet(requestData)
      toast.add({
        severity: 'success',
        summary: '新增成功',
        detail: '寵物已新增',
        life: 3000
      })
    }

    emit('success')
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: '操作失敗',
      detail: error.response?.data?.message || '操作失敗',
      life: 3000
    })
  } finally {
    submitting.value = false
  }
}

const handleClose = () => {
  emit('close')
}

// 聯絡人更新處理
const handleContactsUpdated = () => {
  toast.add({
    severity: 'success',
    summary: '更新成功',
    detail: '聯絡人資料已更新',
    life: 3000
  })
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
  toast.add({
    severity: 'success',
    summary: '移除成功',
    detail: '照片已移除',
    life: 3000
  })
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
    toast.add({
      severity: 'success',
      summary: '上傳成功',
      detail: '照片上傳成功',
      life: 3000
    })
  } catch (error) {
    console.error('Upload photo error:', error)
    toast.add({
      severity: 'error',
      summary: '上傳失敗',
      detail: '照片上傳失敗',
      life: 3000
    })
  }
}

const resetForm = () => {
  errors.value = {}
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

.pet-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-weight: 500;
  color: var(--p-text-color);
  font-size: 0.875rem;
}

.form-label.required::after {
  content: ' *';
  color: var(--p-red-500);
}

.form-help {
  font-size: 0.75rem;
  color: var(--p-text-muted-color);
  margin-top: 0.25rem;
}

.form-error {
  font-size: 0.75rem;
  color: var(--p-red-500);
  margin-top: 0.25rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-row.triple {
  grid-template-columns: 1fr 1fr 1fr;
}

.photo-upload-container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.photo-preview-container {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.5rem;
}

.photo-preview {
  border-radius: var(--p-border-radius);
  border: 1px solid var(--p-content-border-color);
  object-fit: cover;
}

.photo-actions {
  display: flex;
  gap: 0.5rem;
}

.pet-photo-uploader {
  width: 100%;
}

.pet-photo-uploader :deep(.p-fileupload-basic) {
  width: auto;
}

.contacts-section {
  margin-top: 1.5rem;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
  padding-top: 1rem;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .form-row {
    grid-template-columns: 1fr;
  }

  .form-row.triple {
    grid-template-columns: 1fr;
  }

  .pet-form {
    gap: 1rem;
  }

  .dialog-footer {
    flex-direction: column-reverse;
  }

  .dialog-footer .p-button {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .pet-form {
    gap: 0.75rem;
  }

  .form-field {
    gap: 0.25rem;
  }

  .photo-actions {
    flex-direction: column;
    width: 100%;
  }

  .photo-actions .p-button {
    width: 100%;
  }
}

/* PrimeVue 元件覆寫 */
.pet-form :deep(.p-inputtext),
.pet-form :deep(.p-inputnumber-input),
.pet-form :deep(.p-select),
.pet-form :deep(.p-textarea) {
  width: 100%;
}

.pet-form :deep(.p-inputnumber) {
  width: 100%;
}

.pet-form :deep(.p-select-dropdown) {
  width: 100%;
}

.pet-form :deep(.p-fileupload-basic .p-button) {
  width: auto;
  padding: 0.5rem 1rem;
}

/* 確保選擇框在小螢幕上正確顯示 */
@media (max-width: 576px) {
  .pet-form :deep(.p-select-overlay) {
    max-width: calc(100vw - 2rem);
  }
}
</style>