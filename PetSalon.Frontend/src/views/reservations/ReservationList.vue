<template>
  <div class="reservation-list-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>📅 預約管理</h2>
        <span class="total-count">共 {{ total }} 筆預約</span>
      </div>
      <div class="header-right">
        <el-button type="primary" @click="openCreateDialog">
          <el-icon><Plus /></el-icon>
          新增預約
        </el-button>
        <el-button type="info" @click="viewCalendar">
          <el-icon><Calendar /></el-icon>
          行事曆檢視
        </el-button>
      </div>
    </div>

    <!-- Search and Filter -->
    <div class="search-section">
      <el-row :gutter="16">
        <el-col :span="6">
          <el-input
            v-model="searchForm.keyword"
            placeholder="搜尋寵物名稱或主人姓名"
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
            placeholder="預約狀態"
            clearable
            @change="handleSearch"
          >
            <el-option label="已預約" value="已預約" />
            <el-option label="已確認" value="已確認" />
            <el-option label="進行中" value="進行中" />
            <el-option label="已完成" value="已完成" />
            <el-option label="已取消" value="已取消" />
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
        <el-tab-pane label="今日" name="today" />
        <el-tab-pane label="明日" name="tomorrow" />
        <el-tab-pane label="本週" name="week" />
        <el-tab-pane label="待確認" name="pending" />
      </el-tabs>
    </div>

    <!-- Reservation Table -->
    <div class="table-container">
      <el-table
        :data="reservations"
        v-loading="loading"
        stripe
        @row-click="viewReservation"
        style="width: 100%"
      >
        <el-table-column prop="reserveDate" label="預約日期" width="120">
          <template #default="scope">
            {{ formatDate(scope.row.reserveDate) }}
          </template>
        </el-table-column>
        
        <el-table-column prop="reserveTime" label="預約時間" width="100">
          <template #default="scope">
            {{ formatTime(scope.row.reserveTime) }}
          </template>
        </el-table-column>
        
        <el-table-column prop="petName" label="寵物" width="120">
          <template #default="scope">
            <div class="pet-info">
              <span class="pet-name">{{ scope.row.petName }}</span>
              <span class="owner-name">{{ scope.row.ownerName }}</span>
            </div>
          </template>
        </el-table-column>
        
        <el-table-column prop="contactPhone" label="聯絡電話" width="140" />
        
        <el-table-column prop="serviceType" label="服務項目" width="140">
          <template #default="scope">
            <el-tag type="info" size="small">{{ scope.row.serviceType }}</el-tag>
          </template>
        </el-table-column>
        
        <el-table-column prop="designer" label="設計師" width="100" />
        
        <el-table-column prop="subscriptionName" label="使用方案" width="120">
          <template #default="scope">
            <el-tag
              v-if="scope.row.subscriptionName"
              type="warning"
              size="small"
            >
              {{ scope.row.subscriptionName }}
            </el-tag>
            <span v-else class="text-gray">單次服務</span>
          </template>
        </el-table-column>
        
        <el-table-column prop="status" label="狀態" width="100" align="center">
          <template #default="scope">
            <el-tag
              :type="getStatusType(scope.row.status)"
              size="small"
            >
              {{ scope.row.status }}
            </el-tag>
          </template>
        </el-table-column>
        
        <el-table-column prop="note" label="備註" show-overflow-tooltip />
        
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="scope">
            <el-button
              v-if="scope.row.status === '已預約'"
              type="success"
              size="small"
              @click.stop="confirmReservation(scope.row)"
            >
              確認
            </el-button>
            <el-button
              v-if="scope.row.status === '已確認'"
              type="warning"
              size="small"
              @click.stop="startService(scope.row)"
            >
              開始服務
            </el-button>
            <el-button
              v-if="scope.row.status === '進行中'"
              type="primary"
              size="small"
              @click.stop="completeService(scope.row)"
            >
              完成
            </el-button>
            <el-button
              type="primary"
              size="small"
              @click.stop="editReservation(scope.row)"
            >
              編輯
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click.stop="cancelReservation(scope.row)"
            >
              取消
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && reservations.length === 0" class="empty-state">
      <el-empty description="尚無預約資料">
        <el-button type="primary" @click="openCreateDialog">
          新增第一筆預約
        </el-button>
      </el-empty>
    </div>

    <!-- Pagination -->
    <div class="pagination-wrapper" v-if="total > pageSize">
      <el-pagination
        v-model:current-page="currentPage"
        v-model:page-size="pageSize"
        :page-sizes="[20, 50, 100]"
        :total="total"
        layout="total, sizes, prev, pager, next"
        @size-change="loadReservations"
        @current-change="loadReservations"
      />
    </div>

    <!-- Create/Edit Dialog -->
    <ReservationForm
      v-if="showDialog"
      :visible="showDialog"
      :reservation="selectedReservation"
      @close="closeDialog"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search, Calendar } from '@element-plus/icons-vue'
import type { Reservation, ReservationSearchParams } from '@/types/reservation'
import { reservationApi } from '@/api/reservation'
import ReservationForm from '@/components/forms/ReservationForm.vue'

const router = useRouter()

// Data
const reservations = ref<Reservation[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)
const loading = ref(false)
const showDialog = ref(false)
const selectedReservation = ref<Reservation | null>(null)
const activeTab = ref('all')
const dateRange = ref<[Date, Date] | null>(null)

// Search form
const searchForm = reactive<ReservationSearchParams>({
  keyword: '',
  status: undefined,
  startDate: undefined,
  endDate: undefined
})

// Methods
const loadReservations = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      page: currentPage.value,
      pageSize: pageSize.value
    }
    const response = await reservationApi.getReservations(params)
    reservations.value = response.data
    total.value = response.total
  } catch (error) {
    ElMessage.error('載入預約清單失敗')
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadReservations()
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
  const tomorrow = new Date(today)
  tomorrow.setDate(tomorrow.getDate() + 1)
  
  const startOfWeek = new Date(today)
  startOfWeek.setDate(today.getDate() - today.getDay())
  const endOfWeek = new Date(startOfWeek)
  endOfWeek.setDate(startOfWeek.getDate() + 6)
  
  switch (tab.name) {
    case 'all':
      searchForm.startDate = undefined
      searchForm.endDate = undefined
      searchForm.status = undefined
      dateRange.value = null
      break
    case 'today':
      searchForm.startDate = today.toISOString().split('T')[0]
      searchForm.endDate = today.toISOString().split('T')[0]
      searchForm.status = undefined
      dateRange.value = [today, today]
      break
    case 'tomorrow':
      searchForm.startDate = tomorrow.toISOString().split('T')[0]
      searchForm.endDate = tomorrow.toISOString().split('T')[0]
      searchForm.status = undefined
      dateRange.value = [tomorrow, tomorrow]
      break
    case 'week':
      searchForm.startDate = startOfWeek.toISOString().split('T')[0]
      searchForm.endDate = endOfWeek.toISOString().split('T')[0]
      searchForm.status = undefined
      dateRange.value = [startOfWeek, endOfWeek]
      break
    case 'pending':
      searchForm.startDate = undefined
      searchForm.endDate = undefined
      searchForm.status = '已預約'
      dateRange.value = null
      break
  }
  handleSearch()
}

const openCreateDialog = () => {
  selectedReservation.value = null
  showDialog.value = true
}

const editReservation = (reservation: Reservation) => {
  selectedReservation.value = reservation
  showDialog.value = true
}

const viewReservation = (reservation: Reservation) => {
  // TODO: Implement reservation detail view
  editReservation(reservation)
}

const viewCalendar = () => {
  router.push('/reservations/calendar')
}

const confirmReservation = async (reservation: Reservation) => {
  try {
    await reservationApi.updateReservationStatus(reservation.id, '已確認')
    ElMessage.success('預約已確認')
    loadReservations()
  } catch (error) {
    ElMessage.error('確認預約失敗')
  }
}

const startService = async (reservation: Reservation) => {
  try {
    await reservationApi.updateReservationStatus(reservation.id, '進行中')
    ElMessage.success('服務已開始')
    loadReservations()
  } catch (error) {
    ElMessage.error('開始服務失敗')
  }
}

const completeService = async (reservation: Reservation) => {
  try {
    await reservationApi.updateReservationStatus(reservation.id, '已完成')
    ElMessage.success('服務已完成')
    loadReservations()
  } catch (error) {
    ElMessage.error('完成服務失敗')
  }
}

const cancelReservation = async (reservation: Reservation) => {
  try {
    await ElMessageBox.confirm(
      `確定要取消預約「${reservation.petName}」嗎？`,
      '確認取消',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await reservationApi.updateReservationStatus(reservation.id, '已取消')
    ElMessage.success('預約已取消')
    loadReservations()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('取消預約失敗')
    }
  }
}

const closeDialog = () => {
  showDialog.value = false
  selectedReservation.value = null
}

const handleFormSuccess = () => {
  closeDialog()
  loadReservations()
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-TW')
}

const formatTime = (timeString: string) => {
  // Handle both TimeSpan format (HH:mm:ss) and time string
  if (timeString.includes(':')) {
    return timeString.substring(0, 5) // Take HH:mm part
  }
  return timeString
}

const getStatusType = (status: string) => {
  switch (status) {
    case '已預約':
      return 'info'
    case '已確認':
      return 'warning'
    case '進行中':
      return 'primary'
    case '已完成':
      return 'success'
    case '已取消':
      return 'danger'
    default:
      return 'info'
  }
}

// Lifecycle
onMounted(() => {
  loadReservations()
})
</script>

<style scoped>
.reservation-list-container {
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

.header-right {
  display: flex;
  gap: 12px;
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

.table-container {
  margin-bottom: 24px;
}

.pet-info {
  display: flex;
  flex-direction: column;
}

.pet-name {
  font-weight: 500;
  color: #303133;
}

.owner-name {
  font-size: 12px;
  color: #909399;
}

.text-gray {
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

:deep(.el-table__row) {
  cursor: pointer;
}

:deep(.el-table__row:hover) {
  background-color: #f5f7fa;
}
</style>