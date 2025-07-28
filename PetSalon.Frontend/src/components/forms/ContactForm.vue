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
            <el-input v-model="form.name" placeholder="請輸入姓名" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="電話" prop="phone">
            <el-input v-model="form.phone" placeholder="請輸入電話號碼" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="信箱" prop="email">
            <el-input v-model="form.email" placeholder="請輸入電子信箱" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="狀態" prop="status">
            <el-select v-model="form.status" placeholder="請選擇狀態">
              <el-option label="一般客戶" value="客戶" />
              <el-option label="VIP客戶" value="VIP" />
              <el-option label="潛在客戶" value="潛在" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="地址" prop="address">
        <el-input v-model="form.address" placeholder="請輸入地址" />
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
  phone: '',
  email: '',
  address: '',
  note: '',
  status: '客戶'
})

// Form rules
const rules: FormRules = {
  name: [
    { required: true, message: '請輸入姓名', trigger: 'blur' },
    { min: 1, max: 50, message: '姓名長度應為 1-50 個字符', trigger: 'blur' }
  ],
  phone: [
    { required: true, message: '請輸入電話號碼', trigger: 'blur' },
    { pattern: /^[\d\-\+\(\)\s]+$/, message: '請輸入有效的電話號碼', trigger: 'blur' }
  ],
  email: [
    { type: 'email', message: '請輸入有效的電子信箱', trigger: 'blur' }
  ],
  status: [
    { required: true, message: '請選擇狀態', trigger: 'change' }
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
        id: props.contact.id
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
      phone: newContact.phone,
      email: newContact.email,
      address: newContact.address,
      note: newContact.note,
      status: newContact.status
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
</style>