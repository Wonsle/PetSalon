<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? '編輯聯絡人' : '新增聯絡人'"
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

      <!-- Show related pets in edit mode -->
      <div v-if="isEdit && props.contact?.relatedPets && props.contact.relatedPets.length > 0" class="related-pets-section">
        <el-divider content-position="left">關聯寵物</el-divider>
        <div class="pet-list">
          <el-tag
            v-for="pet in props.contact.relatedPets"
            :key="pet.petRelationId"
            size="default"
            class="pet-tag"
          >
            {{ pet.petName }} ({{ pet.breed }})
          </el-tag>
        </div>
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
import { ref, reactive, watch, computed } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
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

// Refs
const formRef = ref<FormInstance>()
const submitting = ref(false)

// Computed
const isEdit = computed(() => !!props.contact)

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

    if (isEdit.value && props.contact) {
      const updateData: ContactUpdateRequest = {
        ...form,
        contactPersonId: props.contact.contactPersonId
      }
      await contactApi.updateContact(updateData)
      ElMessage.success('更新成功')
    } else {
      await contactApi.createContact(form)
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
.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.related-pets-section {
  margin: 16px 0;
}

.pet-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.pet-tag {
  margin: 2px 0;
}
</style>