<template>
  <div class="contact-create-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>新增聯絡人</h2>
        <nav class="breadcrumb">
          <span class="breadcrumb-item" @click="$router.push('/contacts')">聯絡人管理</span>
          <i class="pi pi-chevron-right breadcrumb-separator"></i>
          <span class="breadcrumb-current">新增聯絡人</span>
        </nav>
      </div>
      <div class="header-right">
        <Button
          label="返回"
          icon="pi pi-arrow-left"
          severity="secondary"
          @click="$router.back()"
        />
      </div>
    </div>

    <!-- Form -->
    <Card>
      <template #content>
        <form @submit.prevent="handleSubmit">
          <div class="grid">
            <div class="col-6">
              <div class="field">
                <label class="label">姓名 *</label>
                <InputText
                  v-model="form.name"
                  placeholder="請輸入姓名"
                  maxlength="50"
                  :invalid="!!errors.name"
                />
                <small v-if="errors.name" class="p-error">{{ errors.name }}</small>
              </div>
            </div>
            <div class="col-6">
              <div class="field">
                <label class="label">暱稱</label>
                <InputText
                  v-model="form.nickName"
                  placeholder="請輸入暱稱(選填)"
                  maxlength="50"
                />
              </div>
            </div>
          </div>

          <div class="field">
            <label class="label">聯絡電話 *</label>
            <InputText
              v-model="form.contactNumber"
              placeholder="請輸入聯絡電話"
              maxlength="20"
              :invalid="!!errors.contactNumber"
            />
            <small v-if="errors.contactNumber" class="p-error">{{ errors.contactNumber }}</small>
          </div>

          <!-- Submit Buttons -->
          <div class="form-actions">
            <Button
              label="新增聯絡人"
              type="submit"
              :loading="submitting"
            />
            <Button
              label="取消"
              severity="secondary"
              @click="$router.back()"
            />
          </div>
        </form>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import type { ContactCreateRequest } from '@/types/contact'
import { contactApi } from '@/api/contact'

const router = useRouter()
const toast = useToast()

// Refs
const submitting = ref(false)

// Form data
const form = reactive<ContactCreateRequest>({
  name: '',
  nickName: '',
  contactNumber: ''
})

// Validation errors
const errors = reactive({
  name: '',
  contactNumber: ''
})

// Validation function
const validateForm = () => {
  let isValid = true
  
  // Reset errors
  errors.name = ''
  errors.contactNumber = ''

  // Validate name
  if (!form.name.trim()) {
    errors.name = '請輸入姓名'
    isValid = false
  } else if (form.name.length > 50) {
    errors.name = '姓名長度不能超過50個字符'
    isValid = false
  }

  // Validate contact number
  if (!form.contactNumber.trim()) {
    errors.contactNumber = '請輸入聯絡電話' 
    isValid = false
  } else if (form.contactNumber.length > 20) {
    errors.contactNumber = '聯絡電話長度不能超過20個字符'
    isValid = false
  }

  return isValid
}

// Methods
const handleSubmit = async () => {
  if (!validateForm()) return

  submitting.value = true

  try {
    await contactApi.createContact(form)
    toast.add({
      severity: 'success',
      summary: '成功',
      detail: '新增成功',
      life: 3000
    })
    router.push('/contacts')
  } catch (error: any) {
    console.error('Submit error:', error)
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: error.response?.data?.message || '新增失敗',
      life: 3000
    })
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
  color: var(--p-text-color);
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
}

.breadcrumb-item {
  cursor: pointer;
  color: var(--p-primary-color);
  text-decoration: none;
}

.breadcrumb-item:hover {
  text-decoration: underline;
}

.breadcrumb-separator {
  font-size: 0.75rem;
}

.breadcrumb-current {
  color: var(--p-text-color);
}

.field {
  margin-bottom: 1rem;
}

.label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: var(--p-text-color);
}

.grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1rem;
}

.col-6 {
  grid-column: span 1;
}

@media (max-width: 768px) {
  .grid {
    grid-template-columns: 1fr;
  }
  
  .col-6 {
    grid-column: span 1;
  }
  
  .page-header {
    flex-direction: column;
    gap: 1rem;
    align-items: stretch;
  }
}

.form-actions {
  display: flex;
  gap: 12px;
  margin-top: 2rem;
  padding-top: 1rem;
  border-top: 1px solid var(--p-surface-200);
}

.p-error {
  color: var(--p-red-500);
  font-size: 0.875rem;
}
</style>