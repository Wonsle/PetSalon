<template>
  <div class="subscription-manager">
    <div class="section-header">
      <h3>包月方案管理</h3>
      <el-button type="primary" size="small" @click="showAddDialog = true">
        <el-icon><Plus /></el-icon>
        新增包月方案
      </el-button>
    </div>

    <!-- 包月方案列表 -->
    <div v-loading="loading" class="subscription-list">
      <el-empty v-if="!loading && subscriptions.length === 0" description="尚無包月方案" />
      
      <el-card
        v-for="subscription in subscriptions"
        :key="subscription.subscriptionId"
        class="subscription-card"
        :class="{ 'expired': subscription.isExpired, 'inactive': !subscription.isActive }"
      >
        <div class="subscription-content">
          <div class="subscription-info">
            <div class="subscription-header">
              <span class="status-badge" :class="getStatusClass(subscription.status)">
                {{ getStatusText(subscription.status) }}
              </span>
              <span v-if="subscription.isExpired" class="expired-badge">已過期</span>
            </div>
            
            <div class="subscription-details">
              <div class="detail-row">
                <span class="label">方案期間：</span>
                <span>{{ formatDate(subscription.startDate) }} ~ {{ formatDate(subscription.endDate) }}</span>
              </div>
              <div class="detail-row">
                <span class="label">使用次數：</span>
                <span>
                  {{ subscription.usedCount }} / 
                  {{ subscription.totalUsageLimit === 0 ? '不限' : subscription.totalUsageLimit }}
                  <span v-if="subscription.totalUsageLimit > 0" class="remaining">
                    (剩餘 {{ subscription.remainingUsage }} 次)
                  </span>
                </span>
              </div>
              <div class="detail-row">
                <span class="label">方案價格：</span>
                <span class="price">NT$ {{ subscription.subscriptionPrice?.toLocaleString() || 0 }}</span>
              </div>
              <div v-if="subscription.daysUntilExpiry >= 0 && subscription.daysUntilExpiry <= 30" class="detail-row">
                <span class="label">剩餘天數：</span>
                <span class="days-remaining" :class="{ 'warning': subscription.daysUntilExpiry <= 7 }">
                  {{ subscription.daysUntilExpiry }} 天
                </span>
              </div>
              <div v-if="subscription.notes" class="detail-row">
                <span class="label">備註：</span>
                <span>{{ subscription.notes }}</span>
              </div>
            </div>
          </div>
          
          <div class="subscription-actions">
            <el-button size="small" @click="editSubscription(subscription)">
              <el-icon><Edit /></el-icon>
              編輯
            </el-button>
            <el-button 
              size="small" 
              type="danger" 
              @click="deleteSubscription(subscription.subscriptionId)"
            >
              <el-icon><Delete /></el-icon>
              刪除
            </el-button>
          </div>
        </div>
      </el-card>
    </div>

    <!-- 新增/編輯包月方案對話框 -->
    <el-dialog
      v-model="showAddDialog"
      :title="editingSubscription ? '編輯包月方案' : '新增包月方案'"
      width="600px"
      @closed="resetForm"
    >
      <el-form
        ref="formRef"
        :model="form"
        :rules="formRules"
        label-width="120px"
      >
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="開始日期" prop="startDate">
              <el-date-picker
                v-model="form.startDate"
                type="date"
                placeholder="請選擇開始日期"
                class="w-full"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="結束日期" prop="endDate">
              <el-date-picker
                v-model="form.endDate"
                type="date"
                placeholder="請選擇結束日期"
                class="w-full"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="使用次數限制" prop="totalUsageLimit">
              <el-input-number
                v-model="form.totalUsageLimit"
                :min="0"
                :max="999"
                placeholder="0=不限次數"
                class="w-full"
              />
              <div class="form-tip">設為 0 表示期間內不限使用次數</div>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="方案價格" prop="subscriptionPrice">
              <el-input-number
                v-model="form.subscriptionPrice"
                :min="0"
                :precision="0"
                placeholder="請輸入價格"
                class="w-full"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item v-if="editingSubscription" label="狀態" prop="status">
          <el-select v-model="form.status" placeholder="請選擇狀態" class="w-full">
            <el-option label="啟用" value="ACTIVE" />
            <el-option label="暫停" value="SUSPENDED" />
            <el-option label="已完成" value="COMPLETED" />
            <el-option label="已取消" value="CANCELLED" />
          </el-select>
        </el-form-item>

        <el-form-item label="備註">
          <el-input
            v-model="form.notes"
            type="textarea"
            :rows="3"
            placeholder="請輸入備註"
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="showAddDialog = false">取消</el-button>
          <el-button type="primary" :loading="submitting" @click="handleSubmit">
            {{ editingSubscription ? '更新' : '新增' }}
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Plus, Edit, Delete } from '@element-plus/icons-vue'
import { subscriptionApi } from '@/api/subscription'
import type { Subscription, SubscriptionCreateRequest, SubscriptionUpdateRequest } from '@/types/subscription'

interface Props {
  petId: number
}

const props = defineProps<Props>()

// 響應式數據
const loading = ref(false)
const showAddDialog = ref(false)
const submitting = ref(false)
const subscriptions = ref<Subscription[]>([])
const editingSubscription = ref<Subscription | null>(null)

// 表單相關
const formRef = ref<FormInstance>()
const form = reactive({
  startDate: null as Date | null,
  endDate: null as Date | null,
  totalUsageLimit: 0,
  subscriptionPrice: 0,
  status: 'ACTIVE',
  notes: ''
})

const formRules: FormRules = {
  startDate: [
    { required: true, message: '請選擇開始日期', trigger: 'change' }
  ],
  endDate: [
    { required: true, message: '請選擇結束日期', trigger: 'change' }
  ],
  subscriptionPrice: [
    { required: true, message: '請輸入方案價格', trigger: 'blur' },
    { type: 'number', min: 0, message: '價格不能小於0', trigger: 'blur' }
  ]
}

// 方法
const loadSubscriptions = async () => {
  if (!props.petId) return
  
  loading.value = true
  try {
    subscriptions.value = await subscriptionApi.getSubscriptionsByPet(props.petId)
  } catch (error) {
    console.error('Load subscriptions error:', error)
    ElMessage.error('載入包月方案失敗')
  } finally {
    loading.value = false
  }
}

const editSubscription = (subscription: Subscription) => {
  editingSubscription.value = subscription
  Object.assign(form, {
    startDate: new Date(subscription.startDate),
    endDate: new Date(subscription.endDate),
    totalUsageLimit: subscription.totalUsageLimit,
    subscriptionPrice: subscription.subscriptionPrice,
    status: subscription.status,
    notes: subscription.notes || ''
  })
  showAddDialog.value = true
}

const deleteSubscription = async (subscriptionId: number) => {
  try {
    await ElMessageBox.confirm(
      '確定要刪除此包月方案嗎？刪除後無法恢復。',
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    await subscriptionApi.deleteSubscription(subscriptionId)
    ElMessage.success('刪除成功')
    await loadSubscriptions()
  } catch (error: any) {
    if (error !== 'cancel') {
      console.error('Delete subscription error:', error)
      ElMessage.error('刪除失敗')
    }
  }
}

const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    const valid = await formRef.value.validate()
    if (!valid) return

    submitting.value = true

    if (editingSubscription.value) {
      // 更新包月方案
      const updateData: SubscriptionUpdateRequest = {
        subscriptionId: editingSubscription.value.subscriptionId,
        startDate: form.startDate?.toISOString().split('T')[0],
        endDate: form.endDate?.toISOString().split('T')[0],
        totalUsageLimit: form.totalUsageLimit,
        subscriptionPrice: form.subscriptionPrice,
        status: form.status,
        notes: form.notes
      }
      await subscriptionApi.updateSubscription(updateData)
      ElMessage.success('更新成功')
    } else {
      // 新增包月方案
      const createData: SubscriptionCreateRequest = {
        petId: props.petId,
        startDate: form.startDate!.toISOString().split('T')[0],
        endDate: form.endDate!.toISOString().split('T')[0],
        subscriptionDate: new Date().toISOString().split('T')[0],
        totalUsageLimit: form.totalUsageLimit,
        subscriptionPrice: form.subscriptionPrice,
        status: 'ACTIVE',
        notes: form.notes
      }
      await subscriptionApi.createSubscription(createData)
      ElMessage.success('新增成功')
    }

    showAddDialog.value = false
    await loadSubscriptions()
  } catch (error) {
    console.error('Submit subscription error:', error)
    ElMessage.error('操作失敗')
  } finally {
    submitting.value = false
  }
}

const resetForm = () => {
  editingSubscription.value = null
  Object.assign(form, {
    startDate: null,
    endDate: null,
    totalUsageLimit: 0,
    subscriptionPrice: 0,
    status: 'ACTIVE',
    notes: ''
  })
  if (formRef.value) {
    formRef.value.clearValidate()
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-TW')
}

const getStatusClass = (status: string) => {
  const statusMap: Record<string, string> = {
    'ACTIVE': 'active',
    'SUSPENDED': 'suspended',
    'COMPLETED': 'completed',
    'CANCELLED': 'cancelled'
  }
  return statusMap[status] || 'unknown'
}

const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'ACTIVE': '啟用中',
    'SUSPENDED': '已暫停',
    'COMPLETED': '已完成',
    'CANCELLED': '已取消'
  }
  return statusMap[status] || status
}

// 生命週期
onMounted(() => {
  loadSubscriptions()
})
</script>

<style scoped>
.subscription-manager {
  margin-top: 20px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.section-header h3 {
  margin: 0;
  color: #409EFF;
  border-bottom: 2px solid #409EFF;
  padding-bottom: 8px;
  font-size: 16px;
}

.subscription-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.subscription-card {
  transition: all 0.3s ease;
}

.subscription-card.expired {
  opacity: 0.7;
  border-color: #F56C6C;
}

.subscription-card.inactive {
  opacity: 0.8;
  border-color: #E6A23C;
}

.subscription-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.subscription-info {
  flex: 1;
}

.subscription-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
}

.status-badge {
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.active {
  background: #f0f9ff;
  color: #67c23a;
  border: 1px solid #67c23a;
}

.status-badge.suspended {
  background: #fdf6ec;
  color: #e6a23c;
  border: 1px solid #e6a23c;
}

.status-badge.completed {
  background: #f4f4f5;
  color: #909399;
  border: 1px solid #909399;
}

.status-badge.cancelled {
  background: #fef0f0;
  color: #f56c6c;
  border: 1px solid #f56c6c;
}

.expired-badge {
  padding: 2px 8px;
  background: #fef0f0;
  color: #f56c6c;
  border: 1px solid #f56c6c;
  border-radius: 4px;
  font-size: 12px;
}

.subscription-details {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.detail-row {
  display: flex;
  align-items: center;
  font-size: 14px;
}

.label {
  font-weight: 500;
  color: #606266;
  min-width: 80px;
}

.price {
  font-weight: 600;
  color: #409EFF;
}

.remaining {
  color: #67c23a;
  font-weight: 500;
}

.days-remaining {
  font-weight: 500;
}

.days-remaining.warning {
  color: #e6a23c;
}

.subscription-actions {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-left: 16px;
}

.w-full {
  width: 100%;
}

.form-tip {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>