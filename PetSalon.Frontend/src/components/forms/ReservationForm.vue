<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? '編輯預約' : '新增預約'"
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
      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="寵物" prop="petId">
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
        </el-col>
        <el-col :span="12">
          <el-form-item label="使用方案">
            <el-select
              v-model="form.subscriptionId"
              placeholder="請選擇方案 (可選)"
              clearable
              :disabled="!availableSubscriptions.length"
            >
              <el-option
                v-for="subscription in availableSubscriptions"
                :key="subscription.id"
                :label="`${subscription.name} (剩餘 ${subscription.remainingTimes} 次)`"
                :value="subscription.id"
              />
            </el-select>
            <div v-if="!form.subscriptionId" class="form-tip">
              未選擇方案將以單次服務計費
            </div>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="預約日期" prop="reserveDate">
            <el-date-picker
              v-model="form.reserveDate"
              type="date"
              placeholder="請選擇日期"
              style="width: 100%"
              :disabled-date="disabledDate"
              @change="checkTimeAvailability"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="預約時間" prop="reserveTime">
            <el-time-select
              v-model="form.reserveTime"
              start="09:00"
              step="00:30"
              end="18:00"
              placeholder="請選擇時間"
              style="width: 100%"
              @change="checkTimeAvailability"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- Time Availability Check -->
      <div v-if="availabilityMessage" class="availability-message">
        <el-alert
          :title="availabilityMessage"
          :type="isTimeAvailable ? 'success' : 'warning'"
          :closable="false"
          show-icon
        />
      </div>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="服務項目" prop="serviceType">
            <el-select v-model="form.serviceType" placeholder="請選擇服務項目">
              <el-option label="基礎洗澡" value="基礎洗澡" />
              <el-option label="精緻美容" value="精緻美容" />
              <el-option label="造型設計" value="造型設計" />
              <el-option label="指甲修剪" value="指甲修剪" />
              <el-option label="耳朵清潔" value="耳朵清潔" />
              <el-option label="全套護理" value="全套護理" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="指定設計師" prop="designer">
            <el-select v-model="form.designer" placeholder="請選擇設計師">
              <el-option label="不指定" value="" />
              <el-option label="王美美" value="王美美" />
              <el-option label="李小花" value="李小花" />
              <el-option label="張設計" value="張設計" />
              <el-option label="陳專業" value="陳專業" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="備註">
        <el-input
          v-model="form.note"
          type="textarea"
          :rows="3"
          placeholder="請輸入特殊需求或備註"
        />
      </el-form-item>

      <!-- Selected Pet Info -->
      <div v-if="selectedPet" class="pet-info-card">
        <el-card>
          <template #header>
            <span>寵物資訊</span>
          </template>
          <div class="pet-details">
            <p><strong>寵物名稱:</strong> {{ selectedPet.name }}</p>
            <p><strong>品種:</strong> {{ selectedPet.breedName }}</p>
            <p><strong>年齡:</strong> {{ selectedPet.age }} 歲</p>
            <p><strong>主人:</strong> {{ selectedPet.ownerName }}</p>
            <p><strong>聯絡電話:</strong> {{ selectedPet.contactPhone }}</p>
          </div>
        </el-card>
      </div>
    </el-form>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button 
          type="primary" 
          :loading="submitting" 
          :disabled="!isTimeAvailable && !!availabilityMessage"
          @click="handleSubmit"
        >
          {{ isEdit ? '更新' : '預約' }}
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, computed, onMounted } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import type { Reservation, ReservationCreateRequest, ReservationUpdateRequest } from '@/types/reservation'
import type { Pet } from '@/types/pet'
import { reservationApi } from '@/api/reservation'
import { petApi } from '@/api/pet'

interface Props {
  visible: boolean
  reservation?: Reservation | null
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
const availabilityMessage = ref('')
const isTimeAvailable = ref(true)

// Data
const pets = ref<Pet[]>([])
const selectedPet = ref<Pet | null>(null)
const availableSubscriptions = ref<any[]>([])

// Computed
const isEdit = computed(() => !!props.reservation)

// Form data
const form = reactive<ReservationCreateRequest>({
  petId: 0,
  subscriptionId: undefined,
  reserveDate: '',
  reserveTime: '',
  serviceType: '',
  designer: '',
  note: ''
})

// Form rules
const rules: FormRules = {
  petId: [
    { required: true, message: '請選擇寵物', trigger: 'change' }
  ],
  reserveDate: [
    { required: true, message: '請選擇預約日期', trigger: 'change' }
  ],
  reserveTime: [
    { required: true, message: '請選擇預約時間', trigger: 'change' }
  ],
  serviceType: [
    { required: true, message: '請選擇服務項目', trigger: 'change' }
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
    availableSubscriptions.value = []
    return
  }
  
  try {
    // Load pet details
    selectedPet.value = await petApi.getPet(petId)
    
    // Load available subscriptions for this pet
    // TODO: Implement subscription API
    availableSubscriptions.value = []
  } catch (error) {
    ElMessage.error('載入寵物資訊失敗')
  }
}

const disabledDate = (date: Date) => {
  // Disable past dates
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  return date < today
}

const checkTimeAvailability = async () => {
  if (!form.reserveDate || !form.reserveTime) {
    availabilityMessage.value = ''
    isTimeAvailable.value = true
    return
  }
  
  try {
    const dateStr = new Date(form.reserveDate).toISOString().split('T')[0]
    const response = await reservationApi.checkAvailability(dateStr, form.reserveTime)
    
    if (response.available) {
      availabilityMessage.value = '此時段可以預約'
      isTimeAvailable.value = true
    } else {
      availabilityMessage.value = `此時段已有 ${response.conflicts.length} 個預約，建議選擇其他時間`
      isTimeAvailable.value = false
    }
  } catch (error) {
    availabilityMessage.value = ''
    isTimeAvailable.value = true
  }
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    const valid = await formRef.value.validate()
    if (!valid) return
    
    submitting.value = true
    
    const requestData = {
      ...form,
      reserveDate: new Date(form.reserveDate).toISOString().split('T')[0]
    }
    
    if (isEdit.value && props.reservation) {
      const updateData: ReservationUpdateRequest = {
        ...requestData,
        id: props.reservation.id
      }
      await reservationApi.updateReservation(updateData)
      ElMessage.success('更新成功')
    } else {
      await reservationApi.createReservation(requestData)
      ElMessage.success('預約成功')
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
  availableSubscriptions.value = []
  pets.value = []
  availabilityMessage.value = ''
  isTimeAvailable.value = true
}

// Watch for reservation changes
watch(() => props.reservation, async (newReservation) => {
  if (newReservation) {
    Object.assign(form, {
      petId: newReservation.petId,
      subscriptionId: newReservation.subscriptionId,
      reserveDate: new Date(newReservation.reserveDate),
      reserveTime: newReservation.reserveTime,
      serviceType: newReservation.serviceType,
      designer: newReservation.designer,
      note: newReservation.note
    })
    
    // Load pet info
    if (newReservation.petId) {
      try {
        selectedPet.value = await petApi.getPet(newReservation.petId)
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
  }
})
</script>

<style scoped>
.form-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

.availability-message {
  margin: 16px 0;
}

.pet-info-card {
  margin-top: 16px;
}

.pet-details p {
  margin: 8px 0;
  color: #606266;
}

.pet-details strong {
  color: #303133;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>