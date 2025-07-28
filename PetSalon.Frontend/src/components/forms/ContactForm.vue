<template>
  <Dialog
    :visible="visible"
    :header="isEdit ? '編輯聯絡人' : '新增聯絡人'"
    :style="{ width: '600px' }"
    :modal="true"
    @update:visible="$emit('close')"
  >
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

      <!-- Show related pets in edit mode -->
      <div v-if="isEdit && props.contact?.relatedPets && props.contact.relatedPets.length > 0" class="related-pets-section">
        <Divider align="left">
          <span class="divider-text">關聯寵物</span>
        </Divider>
        <div class="pet-list">
          <Tag
            v-for="pet in props.contact.relatedPets"
            :key="pet.petRelationId"
            :value="`${pet.petName} (${pet.breed})`"
            severity="info"
            class="pet-tag"
          />
        </div>
      </div>

      <!-- Dialog Footer -->
      <div class="dialog-footer">
        <Button
          label="取消"
          severity="secondary"
          @click="handleClose"
        />
        <Button
          :label="isEdit ? '更新' : '新增'"
          :loading="submitting"
          type="submit"
        />
      </div>
    </form>
  </Dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
import type { Contact, ContactCreateRequest, ContactUpdateRequest } from '@/types/contact'
import { contactApi } from '@/api/contact'

interface Props {
  visible: boolean
  contact?: Contact | null
}

interface Emits {
  (e: 'close'): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const toast = useToast()

// Refs
const submitting = ref(false)

// Computed
const isEdit = computed(() => !!props.contact)

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

// Validation rules
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
    if (isEdit.value && props.contact) {
      const updateData: ContactUpdateRequest = {
        ...form,
        contactPersonId: props.contact.contactPersonId
      }
      await contactApi.updateContact(updateData)
      toast.add({
        severity: 'success',
        summary: '成功',
        detail: '更新成功',
        life: 3000
      })
    } else {
      await contactApi.createContact(form)
      toast.add({
        severity: 'success',
        summary: '成功',
        detail: '新增成功',
        life: 3000
      })
    }

    emit('success')
  } catch (error: any) {
    toast.add({
      severity: 'error',
      summary: '錯誤',
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

const resetForm = () => {
  Object.assign(form, {
    name: '',
    nickName: '',
    contactNumber: ''
  })
  
  // Reset errors
  Object.keys(errors).forEach(key => {
    errors[key as keyof typeof errors] = ''
  })
}

// Watch for contact changes
watch(() => props.contact, (newContact) => {
  if (newContact) {
    Object.assign(form, {
      name: newContact.name,
      nickName: newContact.nickName || '',
      contactNumber: newContact.contactNumber
    })
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
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 1.5rem;
  padding-top: 1rem;
  border-top: 1px solid var(--p-surface-200);
}

.related-pets-section {
  margin: 16px 0;
}

.divider-text {
  font-weight: 600;
  color: var(--p-text-color);
}

.pet-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 1rem;
}

.pet-tag {
  margin: 2px 0;
}

.p-error {
  color: var(--p-red-500);
  font-size: 0.875rem;
}
</style>