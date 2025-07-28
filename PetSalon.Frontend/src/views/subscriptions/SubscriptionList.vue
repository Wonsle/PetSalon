<template>
  <div class="subscription-list-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>💰 包月方案管理</h2>
        <span class="total-count">共 {{ total }} 個方案</span>
      </div>
      <div class="header-right">
        <el-button type="primary" @click="openCreateDialog">
          <el-icon><Plus /></el-icon>
          新增包月方案
        </el-button>
      </div>
    </div>

    <!-- Search and Filter -->
    <div class="search-section">
      <el-row :gutter="16">
        <el-col :span="6">
          <el-input
            v-model="searchForm.keyword"
            placeholder="搜尋方案名稱或寵物名稱"
            clearable
            @input="handleSearch"
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-col>
        <el-col :span="4">
          <el-select
            v-model="searchForm.status"
            placeholder="方案狀態"
            clearable
            @change="handleSearch"
          >
            <el-option label="使用中" value="使用中" />
            <el-option label="已暫停" value="已暫停" />
            <el-option label="已完成" value="已完成" />
            <el-option label="已過期" value="已過期" />
          </el-select>
        </el-col>
        <el-col :span="6">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="開始日期"
            end-placeholder="結束日期"
            @change="handleDateChange"
            style="width: 100%"
          />
        </el-col>
        <el-col :span="4">
          <el-button @click="resetSearch">重置</el-button>
        </el-col>
      </el-row>
    </div>

    <!-- Quick Filter Tabs -->
    <div class="quick-tabs">
      <el-tabs v-model="activeTab" @tab-click="handleTabClick">
        <el-tab-pane label="全部" name="all" />
        <el-tab-pane label="使用中" name="active" />
        <el-tab-pane label="即將到期" name="expiring" />
        <el-tab-pane label="已過期" name="expired" />
        <el-tab-pane label="用完額度" name="exhausted" />
      </el-tabs>
    </div>

    <!-- Subscription Cards Grid -->
    <div class="subscription-grid" v-loading="loading">
      <div
        v-for="subscription in subscriptions"
        :key="subscription.id"
        class="subscription-card"
        @click="viewSubscription(subscription)"
      >
        <div class="card-header">
          <div class="subscription-title">
            <h3>{{ subscription.name }}</h3>
            <el-tag
              :type="getStatusType(subscription.status)"
              size="small"
            >
              {{ subscription.status }}
            </el-tag>
          </div>
          <div class="subscription-actions">
            <el-dropdown @command="handleCommand">
              <el-button type="text" :icon="MoreFilled" />
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item 
                    :command="{action: 'edit', data: subscription}"
                  >
                    編輯
                  </el-dropdown-item>
                  <el-dropdown-item 
                    v-if="subscription.status === '使用中'"
                    :command="{action: 'pause', data: subscription}"
                  >
                    暫停
                  </el-dropdown-item>
                  <el-dropdown-item 
                    v-if="subscription.status === '已暫停'"
                    :command="{action: 'resume', data: subscription}"
                  >
                    恢復
                  </el-dropdown-item>
                  <el-dropdown-item 
                    :command="{action: 'delete', data: subscription}"
                    divided
                  >
                    刪除
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </div>
        </div>

        <div class="card-body">
          <div class="pet-info">
            <div class="pet-avatar">
              <img
                v-if="subscription.petPhotoUrl"
                :src="subscription.petPhotoUrl"
                :alt="subscription.petName"
                class="pet-photo"
              />
              <div v-else class="pet-photo-placeholder">
                🐾
              </div>
            </div>
            <div class="pet-details">
              <p class="pet-name">{{ subscription.petName }}</p>
              <p class="owner-name">{{ subscription.ownerName }}</p>
              <p class="contact-phone">{{ subscription.contactPhone }}</p>
            </div>
          </div>

          <div class="subscription-details">
            <div class="detail-row">
              <span class="label">服務內容:</span>
              <span class="value">{{ subscription.serviceContent }}</span>
            </div>
            <div class="detail-row">
              <span class="label">方案期間:</span>
              <span class="value">
                {{ formatDate(subscription.startDate) }} ~ {{ formatDate(subscription.endDate) }}
              </span>
            </div>
            <div class="detail-row">
              <span class="label">使用次數:</span>
              <span class="value">
                <el-progress
                  :percentage="getUsagePercentage(subscription)"
                  :stroke-width="8"
                  :show-text="false"
                  class="usage-progress"
                />
                <span class="usage-text">
                  {{ subscription.usedTimes }} / {{ subscription.totalTimes }} 次
                </span>
              </span>
            </div>
            <div class="detail-row">
              <span class="label">剩餘天數:</span>
              <span class="value">
                <el-tag
                  :type="getRemainingDaysType(subscription.remainingDays)"
                  size="small"
                >
                  {{ subscription.remainingDays }} 天
                </el-tag>
              </span>
            </div>
          </div>
        </div>

        <div class="card-footer">
          <div class="price-info">
            <span class="total-price">總金額: NT$ {{ subscription.totalAmount.toLocaleString() }}</span>
            <span class="paid-amount">已付: NT$ {{ subscription.paidAmount.toLocaleString() }}</span>
          </div>
          <div class="payment-status">
            <el-tag
              :type="subscription.paidAmount >= subscription.totalAmount ? 'success' : 'warning'"
              size="small"
            >
              {{ subscription.paidAmount >= subscription.totalAmount ? '已付清' : '未付清' }}
            </el-tag>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && subscriptions.length === 0" class="empty-state">
      <el-empty description="尚無包月方案">
        <el-button type="primary" @click="openCreateDialog">
          新增第一個包月方案
        </el-button>
      </el-empty>
    </div>

    <!-- Pagination -->
    <div class="pagination-wrapper" v-if="total > pageSize">
      <el-pagination
        v-model:current-page="currentPage"
        v-model:page-size="pageSize"
        :page-sizes="[12, 24, 48]"
        :total="total"
        layout="total, sizes, prev, pager, next"
        @size-change="loadSubscriptions"
        @current-change="loadSubscriptions"
      />
    </div>

    <!-- Create/Edit Dialog -->
    <SubscriptionForm
      v-if="showDialog"
      :visible="showDialog"
      :subscription="selectedSubscription"
      @close="closeDialog"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search, MoreFilled } from '@element-plus/icons-vue'
import type { Subscription, SubscriptionSearchParams } from '@/types/subscription'
import { subscriptionApi } from '@/api/subscription'
import SubscriptionForm from '@/components/forms/SubscriptionForm.vue'

// Data
const subscriptions = ref<Subscription[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(12)
const loading = ref(false)
const showDialog = ref(false)
const selectedSubscription = ref<Subscription | null>(null)
const activeTab = ref('all')
const dateRange = ref<[Date, Date] | null>(null)

// Search form
const searchForm = reactive<SubscriptionSearchParams>({
  keyword: '',
  status: undefined,
  startDate: undefined,
  endDate: undefined
})

// Methods
const loadSubscriptions = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      page: currentPage.value,
      pageSize: pageSize.value
    }
    const response = await subscriptionApi.getSubscriptions(params)
    subscriptions.value = response.data
    total.value = response.total
  } catch (error) {
    ElMessage.error('載入包月方案失敗')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadSubscriptions()
}

const resetSearch = () => {
  searchForm.keyword = ''
  searchForm.status = undefined
  searchForm.startDate = undefined
  searchForm.endDate = undefined
  dateRange.value = null
  activeTab.value = 'all'
  handleSearch()
}

const handleDateChange = (dates: [Date, Date] | null) => {
  if (dates) {
    searchForm.startDate = dates[0].toISOString().split('T')[0]
    searchForm.endDate = dates[1].toISOString().split('T')[0]
  } else {
    searchForm.startDate = undefined
    searchForm.endDate = undefined
  }
  handleSearch()
}

const handleTabClick = (tab: any) => {
  const today = new Date()
  const in7Days = new Date(today)
  in7Days.setDate(today.getDate() + 7)
  
  switch (tab.name) {
    case 'all':
      searchForm.status = undefined
      break
    case 'active':
      searchForm.status = '使用中'
      break
    case 'expiring':
      // TODO: Add expiring filter logic
      searchForm.status = '使用中'
      break
    case 'expired':
      searchForm.status = '已過期'
      break
    case 'exhausted':
      // TODO: Add exhausted filter logic
      searchForm.status = '已完成'
      break
  }
  handleSearch()
}

const openCreateDialog = () => {
  selectedSubscription.value = null
  showDialog.value = true
}

const editSubscription = (subscription: Subscription) => {
  selectedSubscription.value = subscription
  showDialog.value = true
}

const viewSubscription = (subscription: Subscription) => {
  // TODO: Implement subscription detail view
  editSubscription(subscription)
}

const handleCommand = async (command: {action: string, data: Subscription}) => {
  const { action, data } = command
  
  switch (action) {
    case 'edit':
      editSubscription(data)
      break
    case 'pause':
      await pauseSubscription(data)
      break
    case 'resume':
      await resumeSubscription(data)
      break
    case 'delete':
      await deleteSubscription(data)
      break
  }
}

const pauseSubscription = async (subscription: Subscription) => {
  try {
    await subscriptionApi.updateSubscriptionStatus(subscription.id, '已暫停')
    ElMessage.success('包月方案已暫停')
    loadSubscriptions()
  } catch (error) {
    ElMessage.error('暫停方案失敗')
  }
}

const resumeSubscription = async (subscription: Subscription) => {
  try {
    await subscriptionApi.updateSubscriptionStatus(subscription.id, '使用中')
    ElMessage.success('包月方案已恢復')
    loadSubscriptions()
  } catch (error) {
    ElMessage.error('恢復方案失敗')
  }
}

const deleteSubscription = async (subscription: Subscription) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除包月方案「${subscription.name}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await subscriptionApi.deleteSubscription(subscription.id)
    ElMessage.success('刪除成功')
    loadSubscriptions()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗')
    }
  }
}

const closeDialog = () => {
  showDialog.value = false
  selectedSubscription.value = null
}

const handleFormSuccess = () => {
  closeDialog()
  loadSubscriptions()
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-TW')
}

const getStatusType = (status: string) => {
  switch (status) {
    case '使用中':
      return 'success'
    case '已暫停':
      return 'warning'
    case '已完成':
      return 'info'
    case '已過期':
      return 'danger'
    default:
      return 'info'
  }
}

const getUsagePercentage = (subscription: Subscription) => {
  if (subscription.totalTimes === 0) return 0
  return Math.round((subscription.usedTimes / subscription.totalTimes) * 100)
}

const getRemainingDaysType = (days: number) => {
  if (days <= 0) return 'danger'
  if (days <= 7) return 'warning'
  if (days <= 30) return 'primary'
  return 'success'
}

// Lifecycle
onMounted(() => {
  loadSubscriptions()
})
</script>

<style scoped>
.subscription-list-container {
  padding: 20px;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 1px solid #e4e7ed;
}

.header-left h2 {
  margin: 0;
  color: #303133;
  font-size: 24px;
}

.total-count {
  color: #909399;
  font-size: 14px;
  margin-left: 12px;
}

.search-section {
  margin-bottom: 24px;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
}

.quick-tabs {
  margin-bottom: 24px;
}

.subscription-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.subscription-card {
  border: 1px solid #e4e7ed;
  border-radius: 12px;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.subscription-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
  border-color: #409eff;
}

.card-header {
  padding: 16px 16px 0 16px;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.subscription-title h3 {
  margin: 0 0 8px 0;
  color: #303133;
  font-size: 16px;
  font-weight: 600;
}

.card-body {
  padding: 16px;
}

.pet-info {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
}

.pet-avatar {
  flex-shrink: 0;
}

.pet-photo {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid #e4e7ed;
}

.pet-photo-placeholder {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background: #f5f7fa;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  border: 2px solid #e4e7ed;
}

.pet-details {
  flex: 1;
}

.pet-name {
  margin: 0 0 4px 0;
  font-weight: 500;
  color: #303133;
}

.owner-name {
  margin: 0 0 4px 0;
  font-size: 14px;
  color: #606266;
}

.contact-phone {
  margin: 0;
  font-size: 12px;
  color: #909399;
}

.subscription-details {
  space-y: 8px;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.label {
  font-size: 14px;
  color: #606266;
  flex-shrink: 0;
  width: 80px;
}

.value {
  flex: 1;
  text-align: right;
  font-size: 14px;
  color: #303133;
}

.usage-progress {
  width: 80px;
  display: inline-block;
  margin-right: 8px;
}

.usage-text {
  font-size: 12px;
  color: #606266;
}

.card-footer {
  padding: 12px 16px;
  border-top: 1px solid #f0f0f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #fafafa;
  border-radius: 0 0 12px 12px;
}

.price-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.total-price {
  font-size: 14px;
  font-weight: 500;
  color: #303133;
}

.paid-amount {
  font-size: 12px;
  color: #909399;
}

.empty-state {
  text-align: center;
  padding: 60px 20px;
}

.pagination-wrapper {
  display: flex;
  justify-content: center;
  padding: 20px 0;
}

@media (max-width: 768px) {
  .subscription-grid {
    grid-template-columns: 1fr;
  }
  
  .page-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }
  
  .search-section .el-row {
    flex-direction: column;
    gap: 12px;
  }
}
</style>