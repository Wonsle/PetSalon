<template>
  <el-dialog
    :model-value="visible"
    :title="isEdit ? '編輯收入記錄' : '新增收入記錄'"
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
          <el-form-item label="收入類型" prop="type">
            <el-select v-model="form.type" placeholder="請選擇收入類型">
              <el-option label="服務收入" value="服務收入" />
              <el-option label="包月收入" value="包月收入" />
              <el-option label="商品銷售" value="商品銷售" />
              <el-option label="其他收入" value="其他收入" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="收入日期" prop="incomeDate">
            <el-date-picker
              v-model="form.incomeDate"
              type="date"
              placeholder="請選擇日期"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="客戶姓名" prop="customerName">
            <el-input v-model="form.customerName" placeholder="請輸入客戶姓名" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="寵物名稱">
            <el-input v-model="form.petName" placeholder="請輸入寵物名稱(可選)" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="項目描述" prop="description">
        <el-input v-model="form.description" placeholder="請輸入服務項目或商品描述" />
      </el-form-item>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="收入金額" prop="amount">
            <el-input-number
              v-model="form.amount"
              :min="0"
              :precision="0"
              controls-position="right"
              style="width: 100%"
              placeholder="請輸入金額"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="付款方式" prop="paymentMethod">
            <el-select v-model="form.paymentMethod" placeholder="請選擇付款方式">
              <el-option label="現金" value="現金" />
              <el-option label="信用卡" value="信用卡" />
              <el-option label="轉帳" value="轉帳" />
              <el-option label="電子支付" value="電子支付" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="服務人員" prop="designer">
            <el-select v-model="form.designer" placeholder="請選擇服務人員">
              <el-option label="王美美" value="王美美" />
              <el-option label="李小花" value="李小花" />
              <el-option label="張設計" value="張設計" />
              <el-option label="陳專業" value="陳專業" />
              <el-option label="其他" value="其他" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="關聯預約">
            <el-input-number
              v-model="form.reservationId"
              :min="0"
              controls-position="right"
              style="width: 100%"
              placeholder="預約編號(可選)"
            />
            <div class="form-tip">
              關聯預約編號，用於追蹤服務來源
            </div>
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="備註">
        <el-input
          v-model="form.note"
          type="textarea"
          :rows="3"
          placeholder="請輸入備註信息"
        />
      </el-form-item>

      <!-- Quick Amount Buttons -->
      <div class="quick-amounts">
        <span class="quick-label">快速金額:</span>
        <el-button
          v-for="amount in quickAmounts"
          :key="amount"
          size="small"
          @click="setQuickAmount(amount)"
        >
          {{ amount }}
        </el-button>
      </div>

      <!-- Service Templates -->
      <div v-if="form.type === '服務收入'" class="service-templates">
        <span class="template-label">常用服務:</span>
        <el-select
          v-model="selectedTemplate"
          placeholder="選擇常用服務模板"
          @change="applyTemplate"
          style="width: 300px"
        >
          <el-option
            v-for="template in serviceTemplates"
            :key="template.name"
            :label="`${template.name} - NT$ ${template.price}`"
            :value="template"
          />
        </el-select>
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
import type { Income, IncomeCreateRequest, IncomeUpdateRequest } from '@/types/income'
import { incomeApi } from '@/api/income'

interface Props {
  visible: boolean
  income?: Income | null
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
const selectedTemplate = ref<any>(null)

// Computed
const isEdit = computed(() => !!props.income)

// Form data
const form = reactive<IncomeCreateRequest>({
  type: '服務收入',
  customerName: '',
  petName: '',
  description: '',
  amount: 0,
  paymentMethod: '現金',
  designer: '',
  incomeDate: new Date().toISOString().split('T')[0],
  reservationId: undefined,
  subscriptionId: undefined,
  note: ''
})

// Quick amount options
const quickAmounts = [300, 500, 800, 1000, 1500, 2000]

// Service templates
const serviceTemplates = [
  { name: '基礎洗澡', price: 500, description: '基礎洗澡服務' },
  { name: '精緻美容', price: 800, description: '精緻美容造型' },
  { name: '全套護理', price: 1200, description: '洗澡+美容+指甲+耳朵清潔' },
  { name: '造型設計', price: 1500, description: '專業造型設計' },
  { name: '指甲修剪', price: 200, description: '指甲修剪服務' },
  { name: '耳朵清潔', price: 150, description: '耳朵清潔護理' }
]

// Form rules
const rules: FormRules = {
  type: [
    { required: true, message: '請選擇收入類型', trigger: 'change' }
  ],
  customerName: [
    { required: true, message: '請輸入客戶姓名', trigger: 'blur' },
    { min: 1, max: 50, message: '客戶姓名長度應為 1-50 個字符', trigger: 'blur' }
  ],
  description: [
    { required: true, message: '請輸入項目描述', trigger: 'blur' },
    { min: 1, max: 200, message: '項目描述長度應為 1-200 個字符', trigger: 'blur' }
  ],
  amount: [
    { required: true, message: '請輸入收入金額', trigger: 'blur' },
    { type: 'number', min: 0.01, message: '金額必須大於 0', trigger: 'blur' }
  ],
  paymentMethod: [
    { required: true, message: '請選擇付款方式', trigger: 'change' }
  ],
  designer: [
    { required: true, message: '請選擇服務人員', trigger: 'change' }
  ],
  incomeDate: [
    { required: true, message: '請選擇收入日期', trigger: 'change' }
  ]
}

// Methods
const setQuickAmount = (amount: number) => {
  form.amount = amount
}

const applyTemplate = (template: any) => {
  if (template) {
    form.description = template.description
    form.amount = template.price
  }
  selectedTemplate.value = null
}

const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    const valid = await formRef.value.validate()
    if (!valid) return
    
    submitting.value = true
    
    const requestData = {
      ...form,
      incomeDate: new Date(form.incomeDate).toISOString().split('T')[0],
      reservationId: form.reservationId || undefined
    }
    
    if (isEdit.value && props.income) {
      const updateData: IncomeUpdateRequest = {
        ...requestData,
        id: props.income.id
      }
      await incomeApi.updateIncome(updateData)
      ElMessage.success('更新成功')
    } else {
      await incomeApi.createIncome(requestData)
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
  selectedTemplate.value = null
}

// Watch for income changes
watch(() => props.income, (newIncome) => {
  if (newIncome) {
    Object.assign(form, {
      type: newIncome.type,
      customerName: newIncome.customerName,
      petName: newIncome.petName || '',
      description: newIncome.description,
      amount: newIncome.amount,
      paymentMethod: newIncome.paymentMethod,
      designer: newIncome.designer,
      incomeDate: new Date(newIncome.incomeDate),
      reservationId: newIncome.reservationId,
      subscriptionId: newIncome.subscriptionId,
      note: newIncome.note
    })
  } else {
    resetForm()
    // Set default values for new income
    form.incomeDate = new Date().toISOString().split('T')[0]
    form.type = '服務收入'
    form.paymentMethod = '現金'
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

.quick-amounts {
  margin: 16px 0;
  padding: 12px;
  background: #f8f9fa;
  border-radius: 6px;
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.quick-label {
  font-size: 14px;
  color: #606266;
  margin-right: 8px;
}

.service-templates {
  margin: 16px 0;
  padding: 12px;
  background: #f0f9ff;
  border-radius: 6px;
  display: flex;
  align-items: center;
  gap: 12px;
}

.template-label {
  font-size: 14px;
  color: #606266;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>