<template>
  <div class="income-list-container">
    <!-- Header -->
    <div class="page-header">
      <div class="header-left">
        <h2>💰 收入管理</h2>
        <span class="total-count">共 {{ total }} 筆收入</span>
      </div>
      <div class="header-right">
        <el-button type="primary" @click="openCreateDialog">
          <el-icon><Plus /></el-icon>
          新增收入
        </el-button>
        <el-button type="success" @click="exportData">
          <el-icon><Download /></el-icon>
          匯出資料
        </el-button>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-section">
      <el-row :gutter="16">
        <el-col :span="6">
          <el-card class="stats-card">
            <div class="stats-content">
              <div class="stats-icon income">
                <el-icon><Money /></el-icon>
              </div>
              <div class="stats-info">
                <p class="stats-label">今日收入</p>
                <p class="stats-value">NT$ {{ todayIncome.toLocaleString() }}</p>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stats-card">
            <div class="stats-content">
              <div class="stats-icon month">
                <el-icon><Calendar /></el-icon>
              </div>
              <div class="stats-info">
                <p class="stats-label">本月收入</p>
                <p class="stats-value">NT$ {{ monthIncome.toLocaleString() }}</p>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stats-card">
            <div class="stats-content">
              <div class="stats-icon count">
                <el-icon><DocumentChecked /></el-icon>
              </div>
              <div class="stats-info">
                <p class="stats-label">今日筆數</p>
                <p class="stats-value">{{ todayCount }} 筆</p>
              </div>
            </div>
          </el-card>
        </el-col>
        <el-col :span="6">
          <el-card class="stats-card">
            <div class="stats-content">
              <div class="stats-icon avg">
                <el-icon><TrendCharts /></el-icon>
              </div>
              <div class="stats-info">
                <p class="stats-label">平均單價</p>
                <p class="stats-value">NT$ {{ avgAmount.toLocaleString() }}</p>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>

    <!-- Search and Filter -->
    <div class="search-section">
      <el-row :gutter="16">
        <el-col :span="6">
          <el-input
            v-model="searchForm.keyword"
            placeholder="搜尋客戶或寵物名稱"
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
            v-model="searchForm.type"
            placeholder="收入類型"
            clearable
            @change="handleSearch"
          >
            <el-option label="服務收入" value="服務收入" />
            <el-option label="包月收入" value="包月收入" />
            <el-option label="商品銷售" value="商品銷售" />
            <el-option label="其他收入" value="其他收入" />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-select
            v-model="searchForm.paymentMethod"
            placeholder="付款方式"
            clearable
            @change="handleSearch"
          >
            <el-option label="現金" value="現金" />
            <el-option label="信用卡" value="信用卡" />
            <el-option label="轉帳" value="轉帳" />
            <el-option label="電子支付" value="電子支付" />
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
        <el-tab-pane label="本週" name="week" />
        <el-tab-pane label="本月" name="month" />
        <el-tab-pane label="服務收入" name="service" />
        <el-tab-pane label="包月收入" name="subscription" />
      </el-tabs>
    </div>

    <!-- Income Table -->
    <div class="table-container">
      <el-table
        :data="incomes"
        v-loading="loading"
        stripe
        @row-click="viewIncome"
        style="width: 100%"
        :summary-method="getSummaries"
        show-summary
      >
        <el-table-column prop="incomeDate" label="收入日期" width="120">
          <template #default="scope">
            {{ formatDate(scope.row.incomeDate) }}
          </template>
        </el-table-column>
        
        <el-table-column prop="type" label="收入類型" width="100">
          <template #default="scope">
            <el-tag :type="getTypeColor(scope.row.type)" size="small">
              {{ scope.row.type }}
            </el-tag>
          </template>
        </el-table-column>
        
        <el-table-column prop="customerName" label="客戶" width="120">
          <template #default="scope">
            <div class="customer-info">
              <span class="customer-name">{{ scope.row.customerName }}</span>
              <span v-if="scope.row.petName" class="pet-name">{{ scope.row.petName }}</span>
            </div>
          </template>
        </el-table-column>
        
        <el-table-column prop="description" label="項目描述" show-overflow-tooltip />
        
        <el-table-column prop="amount" label="金額" width="120" align="right">
          <template #default="scope">
            <span class="amount-text">NT$ {{ scope.row.amount.toLocaleString() }}</span>
          </template>
        </el-table-column>
        
        <el-table-column prop="paymentMethod" label="付款方式" width="100">
          <template #default="scope">
            <el-tag :type="getPaymentMethodColor(scope.row.paymentMethod)" size="small">
              {{ scope.row.paymentMethod }}
            </el-tag>
          </template>
        </el-table-column>
        
        <el-table-column prop="designer" label="服務人員" width="100" />
        
        <el-table-column prop="reservationId" label="關聯預約" width="100">
          <template #default="scope">
            <el-link
              v-if="scope.row.reservationId"
              type="primary"
              @click.stop="viewReservation(scope.row.reservationId)"
            >
              #{{ scope.row.reservationId }}
            </el-link>
            <span v-else class="text-gray">-</span>
          </template>
        </el-table-column>
        
        <el-table-column prop="note" label="備註" show-overflow-tooltip />
        
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="scope">
            <el-button
              type="primary"
              size="small"
              @click.stop="editIncome(scope.row)"
            >
              編輯
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click.stop="deleteIncome(scope.row)"
            >
              刪除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>

    <!-- Empty State -->
    <div v-if="!loading && incomes.length === 0" class="empty-state">
      <el-empty description="尚無收入記錄">
        <el-button type="primary" @click="openCreateDialog">
          新增第一筆收入
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
        @size-change="loadIncomes"
        @current-change="loadIncomes"
      />
    </div>

    <!-- Create/Edit Dialog -->
    <IncomeForm
      v-if="showDialog"
      :visible="showDialog"
      :income="selectedIncome"
      @close="closeDialog"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { 
  Plus, 
  Search, 
  Download, 
  Money, 
  Calendar, 
  DocumentChecked, 
  TrendCharts 
} from '@element-plus/icons-vue'
import type { Income, IncomeSearchParams } from '@/types/income'
import { incomeApi } from '@/api/income'
import IncomeForm from '@/components/forms/IncomeForm.vue'

const router = useRouter()

// Data
const incomes = ref<Income[]>([])
const total = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)
const loading = ref(false)
const showDialog = ref(false)
const selectedIncome = ref<Income | null>(null)
const activeTab = ref('all')
const dateRange = ref<[Date, Date] | null>(null)

// Statistics
const todayIncome = ref(0)
const monthIncome = ref(0)
const todayCount = ref(0)
const avgAmount = ref(0)

// Search form
const searchForm = reactive<IncomeSearchParams>({
  keyword: '',
  type: undefined,
  paymentMethod: undefined,
  startDate: undefined,
  endDate: undefined
})

// Methods
const loadIncomes = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      page: currentPage.value,
      pageSize: pageSize.value
    }
    const response = await incomeApi.getIncomes(params)
    incomes.value = response.data
    total.value = response.total
  } catch (error) {
    ElMessage.error('載入收入記錄失敗')
  } finally {
    loading.value = false
  }
}

const loadStatistics = async () => {
  try {
    const stats = await incomeApi.getStatistics()
    todayIncome.value = stats.todayIncome
    monthIncome.value = stats.monthIncome
    todayCount.value = stats.todayCount
    avgAmount.value = stats.avgAmount
  } catch (error) {
    console.error('載入統計資料失敗:', error)
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadIncomes()
}

const resetSearch = () => {
  searchForm.keyword = ''
  searchForm.type = undefined
  searchForm.paymentMethod = undefined
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
  const startOfWeek = new Date(today)
  startOfWeek.setDate(today.getDate() - today.getDay())
  const endOfWeek = new Date(startOfWeek)
  endOfWeek.setDate(startOfWeek.getDate() + 6)
  
  const startOfMonth = new Date(today.getFullYear(), today.getMonth(), 1)
  const endOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0)
  
  switch (tab.name) {
    case 'all':
      searchForm.startDate = undefined
      searchForm.endDate = undefined
      searchForm.type = undefined
      dateRange.value = null
      break
    case 'today':
      searchForm.startDate = today.toISOString().split('T')[0]
      searchForm.endDate = today.toISOString().split('T')[0]
      searchForm.type = undefined
      dateRange.value = [today, today]
      break
    case 'week':
      searchForm.startDate = startOfWeek.toISOString().split('T')[0]
      searchForm.endDate = endOfWeek.toISOString().split('T')[0]
      searchForm.type = undefined
      dateRange.value = [startOfWeek, endOfWeek]
      break
    case 'month':
      searchForm.startDate = startOfMonth.toISOString().split('T')[0]
      searchForm.endDate = endOfMonth.toISOString().split('T')[0]
      searchForm.type = undefined
      dateRange.value = [startOfMonth, endOfMonth]
      break
    case 'service':
      searchForm.startDate = undefined
      searchForm.endDate = undefined
      searchForm.type = '服務收入'
      dateRange.value = null
      break
    case 'subscription':
      searchForm.startDate = undefined
      searchForm.endDate = undefined
      searchForm.type = '包月收入'
      dateRange.value = null
      break
  }
  handleSearch()
}

const openCreateDialog = () => {
  selectedIncome.value = null
  showDialog.value = true
}

const editIncome = (income: Income) => {
  selectedIncome.value = income
  showDialog.value = true
}

const viewIncome = (income: Income) => {
  // TODO: Implement income detail view
  editIncome(income)
}

const viewReservation = (reservationId: number) => {
  router.push(`/reservations?id=${reservationId}`)
}

const deleteIncome = async (income: Income) => {
  try {
    await ElMessageBox.confirm(
      `確定要刪除收入記錄「${income.description}」嗎？`,
      '確認刪除',
      {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    await incomeApi.deleteIncome(income.id)
    ElMessage.success('刪除成功')
    loadIncomes()
    loadStatistics()
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('刪除失敗')
    }
  }
}

const exportData = async () => {
  try {
    const params = {
      ...searchForm,
      export: true
    }
    await incomeApi.exportIncomes(params)
    ElMessage.success('匯出成功')
  } catch (error) {
    ElMessage.error('匯出失敗')
  }
}

const closeDialog = () => {
  showDialog.value = false
  selectedIncome.value = null
}

const handleFormSuccess = () => {
  closeDialog()
  loadIncomes()
  loadStatistics()
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-TW')
}

const getTypeColor = (type: string) => {
  switch (type) {
    case '服務收入':
      return 'primary'
    case '包月收入':
      return 'warning'
    case '商品銷售':
      return 'success'
    case '其他收入':
      return 'info'
    default:
      return ''
  }
}

const getPaymentMethodColor = (method: string) => {
  switch (method) {
    case '現金':
      return 'success'
    case '信用卡':
      return 'primary'
    case '轉帳':
      return 'warning'
    case '電子支付':
      return 'info'
    default:
      return ''
  }
}

const getSummaries = (param: any) => {
  const { columns, data } = param
  const sums: string[] = []
  columns.forEach((column: any, index: number) => {
    if (index === 0) {
      sums[index] = '合計'
      return
    }
    if (column.property === 'amount') {
      const values = data.map((item: any) => Number(item[column.property]))
      if (!values.every((value: any) => Number.isNaN(value))) {
        const sum = values.reduce((prev: number, curr: number) => {
          const value = Number(curr)
          if (!Number.isNaN(value)) {
            return prev + curr
          } else {
            return prev
          }
        }, 0)
        sums[index] = `NT$ ${sum.toLocaleString()}`
      } else {
        sums[index] = 'N/A'
      }
    } else {
      sums[index] = ''
    }
  })
  return sums
}

// Lifecycle
onMounted(() => {
  loadIncomes()
  loadStatistics()
})
</script>

<style scoped>
.income-list-container {
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

.stats-section {
  margin-bottom: 24px;
}

.stats-card {
  height: 100px;
}

.stats-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.stats-icon {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 16px;
  font-size: 24px;
  color: white;
}

.stats-icon.income {
  background: linear-gradient(135deg, #67c23a, #85ce61);
}

.stats-icon.month {
  background: linear-gradient(135deg, #409eff, #66b1ff);
}

.stats-icon.count {
  background: linear-gradient(135deg, #e6a23c, #ebb563);
}

.stats-icon.avg {
  background: linear-gradient(135deg, #909399, #b1b3b8);
}

.stats-info {
  flex: 1;
}

.stats-label {
  margin: 0 0 4px 0;
  font-size: 14px;
  color: #909399;
}

.stats-value {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #303133;
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

.customer-info {
  display: flex;
  flex-direction: column;
}

.customer-name {
  font-weight: 500;
  color: #303133;
}

.pet-name {
  font-size: 12px;
  color: #909399;
}

.amount-text {
  font-weight: 600;
  color: #67c23a;
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