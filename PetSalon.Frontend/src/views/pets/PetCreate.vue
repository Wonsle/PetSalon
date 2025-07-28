<template>
  <div class="pet-create">
    <div class="header">
      <h1>新增寵物</h1>
      <div class="header-actions">
        <Button
          label="返回"
          icon="pi pi-arrow-left"
          severity="secondary"
          @click="$router.back()"
        />
        <Button
          label="儲存"
          icon="pi pi-check"
          :loading="submitLoading"
          @click="handleSubmit"
        />
      </div>
    </div>

    <Card class="form-card">
      <template #content>
        <div class="form-container">
          <div class="form-section">
            <h3>基本資訊</h3>

            <div class="form-field">
              <label for="petName">寵物名稱 *</label>
              <InputText
                id="petName"
                v-model="formData.petName"
                placeholder="請輸入寵物名稱"
                maxlength="50"
                :class="{ 'p-invalid': errors.petName }"
                @blur="validateField('petName')"
              />
              <small v-if="errors.petName" class="p-error">{{ errors.petName }}</small>
            </div>

            <div class="form-field">
              <label for="breed">品種 *</label>
              <SystemCodeSelect
                v-model="formData.breed"
                code-type="Breed"
                placeholder="請選擇品種"
                :class="{ 'p-invalid': errors.breed }"
                @change="validateField('breed')"
              />
              <small v-if="errors.breed" class="p-error">{{ errors.breed }}</small>
            </div>

            <div class="form-row">
              <div class="form-field">
                <label for="gender">性別 *</label>
                <SystemCodeSelect
                  v-model="formData.gender"
                  code-type="Gender"
                  placeholder="請選擇性別"
                  :class="{ 'p-invalid': errors.gender }"
                  @change="validateField('gender')"
                />
                <small v-if="errors.gender" class="p-error">{{ errors.gender }}</small>
              </div>
              <div class="form-field">
                <label for="birthDay">生日</label>
                <Calendar
                  id="birthDay"
                  v-model="formData.birthDay"
                  placeholder="請選擇生日"
                  date-format="yy/mm/dd"
                  :max-date="new Date()"
                />
              </div>
            </div>

            <div class="form-row">
              <div class="form-field">
                <label for="normalPrice">一般價格</label>
                <InputNumber
                  id="normalPrice"
                  v-model="formData.normalPrice"
                  :min="0"
                  :max-fraction-digits="0"
                  placeholder="請輸入價格"
                  suffix=" 元"
                />
              </div>
              <div class="form-field">
                <label for="subscriptionPrice">包月價格</label>
                <InputNumber
                  id="subscriptionPrice"
                  v-model="formData.subscriptionPrice"
                  :min="0"
                  :max-fraction-digits="0"
                  placeholder="請輸入價格"
                  suffix=" 元"
                />
              </div>
            </div>
          </div>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import { petApi } from '@/api/pet'
import { SystemCodeSelect } from '@/components/common'
import type { PetCreateRequest } from '@/types/pet'
import Calendar from 'primevue/calendar'

const router = useRouter()
const toast = useToast()

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

// Form errors
const errors = reactive({
  petName: '',
  breed: '',
  gender: ''
})

// Validation methods
const validateField = (field: 'petName' | 'breed' | 'gender') => {
  errors[field] = ''

  if (field === 'petName') {
    if (!formData.petName) {
      errors.petName = '請輸入寵物名稱'
    } else if (formData.petName.length < 1 || formData.petName.length > 50) {
      errors.petName = '寵物名稱長度應為 1-50 個字符'
    }
  }

  if (field === 'breed') {
    if (!formData.breed) {
      errors.breed = '請選擇品種'
    }
  }

  if (field === 'gender') {
    if (!formData.gender) {
      errors.gender = '請選擇性別'
    }
  }
}

const validateForm = () => {
  validateField('petName')
  validateField('breed')
  validateField('gender')
  return !errors.petName && !errors.breed && !errors.gender
}

// Methods
const handleSubmit = async () => {
  if (!validateForm()) {
    toast.add({
      severity: 'warn',
      summary: '表單驗證',
      detail: '請檢查必填欄位',
      life: 3000
    })
    return
  }

  try {
    submitLoading.value = true

    await petApi.createPet(formData)

    toast.add({
      severity: 'success',
      summary: '成功',
      detail: '寵物新增成功',
      life: 3000
    })
    router.push('/pets')
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '新增寵物失敗',
      life: 5000
    })
    console.error('Failed to create pet:', error)
  } finally {
    submitLoading.value = false
  }
}
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
  color: var(--p-text-color);
}

.header-actions {
  display: flex;
  gap: 12px;
}

.form-card {
  margin-bottom: 20px;
}

.form-container {
  max-width: 600px;
}

.form-section {
  margin-bottom: 24px;
}

.form-section h3 {
  margin: 0 0 20px 0;
  color: var(--p-primary-color);
  border-bottom: 2px solid var(--p-primary-color);
  padding-bottom: 8px;
  font-size: 18px;
  font-weight: 600;
}

.form-field {
  margin-bottom: 20px;
}

.form-field label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: var(--p-text-color);
}

.form-field .p-inputtext,
.form-field .p-calendar,
.form-field .p-inputnumber,
.form-field .p-dropdown {
  width: 100%;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.p-error {
  color: var(--p-red-500);
  font-size: 12px;
  margin-top: 4px;
  display: block;
}

@media (max-width: 768px) {
  .header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  .header-actions {
    width: 100%;
    justify-content: flex-end;
  }

  .form-row {
    grid-template-columns: 1fr;
  }
}
</style>