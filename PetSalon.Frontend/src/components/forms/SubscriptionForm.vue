<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? '編輯包月方案' : '新增包月方案'"
    width="700px"
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
      <el-form-item label="方案名稱" prop="name">
        <el-input v-model="form.name" placeholder="請輸入方案名稱" />
      </el-form-item>

      <el-form-item label="選擇寵物" prop="petId">
        <el-select
          v-model="form.petId"
          placeholder="請選擇寵物"
          filterable
          remote
          :remote-method="searchPets"
          :loading="petLoading"
          @change="handlePetChange"
        >
          <el-option
            v-for="pet in pets"
            :key="pet.id"
            :label="`${pet.name} (${pet.ownerName})`"
            :value="pet.id"
          />
        </el-select>
      </el-form-item>

      <!-- Selected Pet Info -->
      <div v-if="selectedPet" class="pet-info-card">
        <el-card>
          <template #header>
            <span>寵物資訊</span>
          </template>
          <div class="pet-details">
            <div class="pet-summary">
              <div class="pet-avatar">
                <img
                  v-if="selectedPet.photoUrl"
                  :src="selectedPet.photoUrl"
                  :alt="selectedPet.name"
                  class="pet-photo"
                />
                <div v-else class="pet-photo-placeholder">
                  🐾
                </div>
              </div>
              <div class="pet-info">
                <p><strong>寵物:</strong> {{ selectedPet.name }}</p>
                <p><strong>品種:</strong> {{ selectedPet.breedName }}</p>
                <p><strong>主人:</strong> {{ selectedPet.ownerName }}</p>
                <p><strong>電話:</strong> {{ selectedPet.contactPhone }}</p>
              </div>
            </div>
          </div>
        </el-card>
      </div>

      <el-form-item label="服務內容" prop="serviceContent">
        <el-select v-model="form.serviceContent" placeholder="請選擇服務內容">
          <el-option label="基礎洗澡套餐" value="基礎洗澡套餐" />
          <el-option label="精緻美容套餐" value="精緻美容套餐" />
          <el-option label="全套護理套餐" value="全套護理套餐" />
          <el-option label="造型設計套餐" value="造型設計套餐" />
          <el-option label="自訂服務套餐" value="自訂服務套餐" />
        </el-select>
      </el-form-item>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="服務次數" prop="totalTimes">
            <el-input-number
              v-model="form.totalTimes"
              :min="1"
              :max="100"
              controls-position="right"
              style="width: 100%"
              @change="calculateAmount"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="單次價格" prop="unitPrice">
            <el-input-number
              v-model="unitPrice"
              :min="0"
              :precision="0"
              controls-position="right"
              style="width: 100%"
              @change="calculateAmount"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="方案總額" prop="totalAmount">
            <el-input-number
              v-model="form.totalAmount"
              :min="0"
              :precision="0"
              controls-position="right"
              style="width: 100%"
            />
            <div class="price-tip">
              建議價格: NT$ {{ suggestedPrice.toLocaleString() }}
            </div>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="已付金額" prop="paidAmount">
            <el-input-number
              v-model="form.paidAmount"
              :min="0"
              :max="form.totalAmount"
              :precision="0"
              controls-position="right"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="開始日期" prop="startDate">
            <el-date-picker
              v-model="form.startDate"
              type="date"
              placeholder="請選擇開始日期"
              style="width: 100%"
              :disabled-date="disabledStartDate"
              @change="handleStartDateChange"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="結束日期" prop="endDate">
            <el-date-picker
              v-model="form.endDate"
              type="date"
              placeholder="請選擇結束日期"
              style="width: 100%"
              :disabled-date="disabledEndDate"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- Duration Info -->
      <div v-if="form.startDate && form.endDate" class="duration-info">
        <el-alert
          :title="`方案期間: ${getDurationDays()} 天`"
          type="info"
          :closable="false"
          show-icon
        />
      </div>

      <el-form-item label="備註">
        <el-input
          v-model="form.note"
          type="textarea"
          :rows="3"
          placeholder="請輸入方案說明或特殊條件"
        />
      </el-form-item>

      <!-- Summary Card -->
      <div v-if="form.totalTimes && form.totalAmount" class="summary-card">
        <el-card>
          <template #header>
            <span>方案摘要</span>
          </template>
          <div class="summary-content">
            <div class="summary-row">
              <span>服務次數:</span>
              <span>{{ form.totalTimes }} 次</span>
            </div>
            <div class="summary-row">
              <span>單次價格:</span>
              <span>NT$ {{ unitPrice.toLocaleString() }}</span>
            </div>
            <div class="summary-row">
              <span>方案總額:</span>
              <span class="highlight">NT$ {{ form.totalAmount.toLocaleString() }}</span>
            </div>
            <div class="summary-row">
              <span>已付金額:</span>
              <span>NT$ {{ form.paidAmount.toLocaleString() }}</span>
            </div>
            <div class="summary-row">
              <span>未付金額:</span>
              <span class="unpaid">NT$ {{ (form.totalAmount - form.paidAmount).toLocaleString() }}</span>
            </div>
            <div v-if="discountAmount > 0" class="summary-row">
              <span>優惠金額:</span>
              <span class="discount">-NT$ {{ discountAmount.toLocaleString() }}</span>
            </div>
          </div>
        </el-card>
      </div>
    </el-form>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">
          {{ isEdit ? '更新' : '建立方案' }}
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import type { Subscription, SubscriptionCreateRequest, SubscriptionUpdateRequest } from '@/types/subscription'
import type { Pet } from '@/types/pet'
import { subscriptionApi } from '@/api/subscription'
import { petApi } from '@/api/pet'

interface Props {
  visible: boolean
  subscription?: Subscription | null
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
const petLoading = ref(false)
const unitPrice = ref(500) // Default unit price

// Data
const pets = ref<Pet[]>([])
const selectedPet = ref<Pet | null>(null)

// Computed
const isEdit = computed(() => !!props.subscription)

const suggestedPrice = computed(() => {
  const basePrice = unitPrice.value * form.totalTimes
  // Apply bulk discount for more services
  if (form.totalTimes >= 10) {
    return Math.round(basePrice * 0.85) // 15% discount
  } else if (form.totalTimes >= 5) {
    return Math.round(basePrice * 0.9) // 10% discount
  }
  return basePrice
})

const discountAmount = computed(() => {
  const basePrice = unitPrice.value * form.totalTimes
  return Math.max(0, basePrice - form.totalAmount)
})

// Form data
const form = reactive<SubscriptionCreateRequest>({
  name: '',
  petId: 0,
  serviceContent: '',
  totalTimes: 5,
  totalAmount: 0,
  paidAmount: 0,
  startDate: '',
  endDate: '',
  note: ''
})

// Form rules
const rules: FormRules = {
  name: [
    { required: true, message: '請輸入方案名稱', trigger: 'blur' },
    { min: 2, max: 50, message: '方案名稱長度應為 2-50 個字符', trigger: 'blur' }
  ],
  petId: [
    { required: true, message: '請選擇寵物', trigger: 'change' }
  ],
  serviceContent: [
    { required: true, message: '請選擇服務內容', trigger: 'change' }
  ],
  totalTimes: [
    { required: true, message: '請輸入服務次數', trigger: 'blur' }
  ],
  totalAmount: [
    { required: true, message: '請輸入方案總額', trigger: 'blur' }
  ],
  startDate: [
    { required: true, message: '請選擇開始日期', trigger: 'change' }
  ],
  endDate: [
    { required: true, message: '請選擇結束日期', trigger: 'change' }
  ]
}

// Methods
const searchPets = async (query: string) => {
  if (!query) {
    pets.value = []
    return
  }
  
  petLoading.value = true
  try {
    const response = await petApi.getPets({ keyword: query, pageSize: 20 })
    pets.value = response.data
  } catch (error) {
    ElMessage.error('搜尋寵物失敗')
  } finally {
    petLoading.value = false
  }
}

const handlePetChange = async (petId: number) => {
  if (!petId) {
    selectedPet.value = null
    return
  }
  
  try {
    selectedPet.value = await petApi.getPet(petId)
    // Auto-generate subscription name
    if (!form.name && selectedPet.value) {
      form.name = `${selectedPet.value.name} - ${form.serviceContent || '包月方案'}`
    }
  } catch (error) {
    ElMessage.error('載入寵物資訊失敗')
  }
}

const calculateAmount = () => {
  if (form.totalTimes && unitPrice.value) {
    form.totalAmount = suggestedPrice.value
    // Auto set paid amount to total amount for convenience
    if (form.paidAmount === 0) {
      form.paidAmount = form.totalAmount
    }
  }
}

const disabledStartDate = (date: Date) => {
  // Disable past dates (except today)
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  return date < today
}

const disabledEndDate = (date: Date) => {
  if (!form.startDate) return false
  const startDate = new Date(form.startDate)
  return date <= startDate
}

const handleStartDateChange = () => {
  if (form.startDate && !form.endDate) {
    // Auto set end date to 3 months later
    const start = new Date(form.startDate)
    const end = new Date(start)
    end.setMonth(end.getMonth() + 3)
    form.endDate = end.toISOString().split('T')[0]
  }
}

const getDurationDays = () => {
  if (!form.startDate || !form.endDate) return 0
  const start = new Date(form.startDate)
  const end = new Date(form.endDate)
  return Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24))
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    const valid = await formRef.value.validate()
    if (!valid) return
    
    submitting.value = true
    
    const requestData = {
      ...form,
      startDate: new Date(form.startDate).toISOString().split('T')[0],
      endDate: new Date(form.endDate).toISOString().split('T')[0]
    }
    
    if (isEdit.value && props.subscription) {
      const updateData: SubscriptionUpdateRequest = {
        ...requestData,
        id: props.subscription.id
      }
      await subscriptionApi.updateSubscription(updateData)
      ElMessage.success('更新成功')
    } else {
      await subscriptionApi.createSubscription(requestData)
      ElMessage.success('建立成功')
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
  selectedPet.value = null
  pets.value = []
  unitPrice.value = 500
}

// Watch for service content changes to update name
watch(() => [form.serviceContent, selectedPet.value], () => {
  if (selectedPet.value && form.serviceContent && !isEdit.value) {
    form.name = `${selectedPet.value.name} - ${form.serviceContent}`
  }
})

// Watch for subscription changes
watch(() => props.subscription, async (newSubscription) => {
  if (newSubscription) {
    Object.assign(form, {
      name: newSubscription.name,
      petId: newSubscription.petId,
      serviceContent: newSubscription.serviceContent,
      totalTimes: newSubscription.totalTimes,
      totalAmount: newSubscription.totalAmount,
      paidAmount: newSubscription.paidAmount,
      startDate: new Date(newSubscription.startDate),
      endDate: new Date(newSubscription.endDate),
      note: newSubscription.note
    })
    
    // Calculate unit price from existing data
    if (newSubscription.totalTimes > 0) {
      unitPrice.value = Math.round(newSubscription.totalAmount / newSubscription.totalTimes)
    }
    
    // Load pet info
    if (newSubscription.petId) {
      try {
        selectedPet.value = await petApi.getPet(newSubscription.petId)
        pets.value = [selectedPet.value]
      } catch (error) {
        console.error('載入寵物資訊失敗:', error)
      }
    }
  } else {
    resetForm()
  }
}, { immediate: true })

// Watch for dialog visibility
watch(() => props.visible, (visible) => {
  if (!visible) {
    resetForm()
  } else {
    // Set default dates if creating new subscription
    if (!isEdit.value) {
      const today = new Date()
      form.startDate = today.toISOString().split('T')[0]
      
      const endDate = new Date(today)
      endDate.setMonth(endDate.getMonth() + 3)
      form.endDate = endDate.toISOString().split('T')[0]
    }
  }
})

// Auto calculate amount when times or unit price changes
watch(() => [form.totalTimes, unitPrice.value], () => {
  calculateAmount()
})
</script>

<style scoped>
.pet-info-card {
  margin: 16px 0;
}

.pet-summary {
  display: flex;
  align-items: center;
  gap: 16px;
}

.pet-avatar {
  flex-shrink: 0;
}

.pet-photo {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid #e4e7ed;
}

.pet-photo-placeholder {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: #f5f7fa;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  border: 2px solid #e4e7ed;
}

.pet-info p {
  margin: 4px 0;
  color: #606266;
}

.pet-info strong {
  color: #303133;
}

.price-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

.duration-info {
  margin: 16px 0;
}

.summary-card {
  margin-top: 16px;
}

.summary-content {
  space-y: 8px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
  padding: 4px 0;
}

.summary-row:last-child {
  margin-bottom: 0;
  padding-top: 8px;
  border-top: 1px solid #f0f0f0;
}

.highlight {
  font-weight: 600;
  color: #409eff;
}

.unpaid {
  color: #f56c6c;
  font-weight: 500;
}

.discount {
  color: #67c23a;
  font-weight: 500;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>