<template>
  <div class="subscription-dashboard">
    <div class="grid">
      <!-- 包月統計卡片 -->
      <div class="col-12 lg:col-3 md:col-6">
        <Card class="stat-card active-subscriptions">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon">
                <i class="pi pi-check-circle"></i>
              </div>
              <div class="stat-info">
                <div class="stat-value">{{ stats.activeSubscriptions || 0 }}</div>
                <div class="stat-label">啟用中的包月</div>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <div class="col-12 lg:col-3 md:col-6">
        <Card class="stat-card expiring-soon">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon">
                <i class="pi pi-exclamation-triangle"></i>
              </div>
              <div class="stat-info">
                <div class="stat-value">{{ stats.expiringSoon || 0 }}</div>
                <div class="stat-label">即將到期</div>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <div class="col-12 lg:col-3 md:col-6">
        <Card class="stat-card revenue-this-month">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon">
                <i class="pi pi-dollar"></i>
              </div>
              <div class="stat-info">
                <div class="stat-value">NT$ {{ (stats.monthlyRevenue || 0).toLocaleString() }}</div>
                <div class="stat-label">本月包月收入</div>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <div class="col-12 lg:col-3 md:col-6">
        <Card class="stat-card usage-rate">
          <template #content>
            <div class="stat-content">
              <div class="stat-icon">
                <i class="pi pi-chart-line"></i>
              </div>
              <div class="stat-info">
                <div class="stat-value">{{ stats.averageUsageRate || 0 }}%</div>
                <div class="stat-label">平均使用率</div>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <!-- 即將到期提醒 -->
      <div class="col-12 lg:col-8">
        <Card>
          <template #header>
            <div class="card-header">
              <h3>⚠️ 即將到期的包月方案</h3>
              <Button
                icon="pi pi-refresh"
                severity="secondary"
                text
                @click="loadExpiringSubscriptions"
                :loading="loadingExpiring"
              />
            </div>
          </template>
          <template #content>
            <div v-if="expiringSubscriptions.length === 0" class="empty-state">
              <i class="pi pi-check-circle" style="font-size: 2rem; color: var(--p-green-500);"></i>
              <p>目前沒有即將到期的包月方案</p>
            </div>
            <div v-else>
              <DataTable
                :value="expiringSubscriptions"
                :rows="5"
                size="small"
                :loading="loadingExpiring"
              >
                <Column field="petName" header="寵物名稱" />
                <Column field="subscriptionType" header="方案類型">
                  <template #body="{ data }">
                    <Tag :value="data.subscriptionType" severity="info" />
                  </template>
                </Column>
                <Column field="endDate" header="到期日期">
                  <template #body="{ data }">
                    {{ formatDate(data.endDate) }}
                  </template>
                </Column>
                <Column field="daysLeft" header="剩餘天數">
                  <template #body="{ data }">
                    <Tag
                      :value="`${data.daysLeft}天`"
                      :severity="data.daysLeft <= 3 ? 'danger' : 'warning'"
                    />
                  </template>
                </Column>
                <Column field="remainingUsage" header="剩餘次數">
                  <template #body="{ data }">
                    {{ data.remainingUsage }}次
                  </template>
                </Column>
                <Column header="操作">
                  <template #body="{ data }">
                    <Button
                      icon="pi pi-phone"
                      size="small"
                      severity="info"
                      text
                      @click="contactCustomer(data)"
                      v-tooltip="'聯絡客戶'"
                    />
                  </template>
                </Column>
              </DataTable>
            </div>
          </template>
        </Card>
      </div>

      <!-- 包月使用趨勢圖表 -->
      <div class="col-12 lg:col-4">
        <Card>
          <template #header>
            <div class="card-header">
              <h3>📊 使用率分布</h3>
            </div>
          </template>
          <template #content>
            <div class="usage-chart">
              <Chart type="doughnut" :data="usageChartData" :options="chartOptions" />
              <div class="chart-legend">
                <div class="legend-item">
                  <span class="legend-color high-usage"></span>
                  <span>高使用率 (>80%)</span>
                </div>
                <div class="legend-item">
                  <span class="legend-color medium-usage"></span>
                  <span>中使用率 (40-80%)</span>
                </div>
                <div class="legend-item">
                  <span class="legend-color low-usage"></span>
                  <span>低使用率 (<40%)</span>
                </div>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <!-- 包月銷售趨勢 -->
      <div class="col-12">
        <Card>
          <template #header>
            <div class="card-header">
              <h3>📈 包月銷售趨勢</h3>
              <div class="header-actions">
                <Select
                  v-model="selectedPeriod"
                  :options="periodOptions"
                  option-label="label"
                  option-value="value"
                  @change="loadSalesTrend"
                />
              </div>
            </div>
          </template>
          <template #content>
            <Chart
              type="line"
              :data="salesChartData"
              :options="salesChartOptions"
              :loading="loadingSales"
            />
          </template>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useToast } from 'primevue/usetoast'
import dayjs from 'dayjs'

// Composables
const toast = useToast()

// 定義類型介面
interface StatsData {
  activeSubscriptions: number
  expiringSoon: number
  monthlyRevenue: number
  averageUsageRate: number
}

interface ExpiringSubscription {
  petName: string
  subscriptionType: string
  endDate: string
  daysLeft: number
  remainingUsage: number
}

interface ChartDataset {
  label?: string
  data: number[]
  backgroundColor?: string | string[]
  borderColor?: string
  borderWidth?: number
  tension?: number
}

interface ChartData {
  labels: string[]
  datasets: ChartDataset[]
}

// State
const stats = ref<StatsData>({
  activeSubscriptions: 0,
  expiringSoon: 0,
  monthlyRevenue: 0,
  averageUsageRate: 0
})

const expiringSubscriptions = ref<ExpiringSubscription[]>([])
const loadingExpiring = ref(false)
const loadingSales = ref(false)
const selectedPeriod = ref('3month')

// Chart Data
const usageChartData = ref<ChartData>({
  labels: ['高使用率', '中使用率', '低使用率'],
  datasets: [{
    data: [30, 45, 25],
    backgroundColor: [
      '#FF6B6B',
      '#4ECDC4',
      '#45B7D1'
    ]
  }]
})

const salesChartData = ref<ChartData>({
  labels: [],
  datasets: [{
    label: '包月銷售額',
    data: [],
    borderColor: '#4ECDC4',
    backgroundColor: 'rgba(78, 205, 196, 0.1)',
    tension: 0.4
  }]
})

// Options
const periodOptions = [
  { label: '最近3個月', value: '3month' },
  { label: '最近6個月', value: '6month' },
  { label: '最近1年', value: '1year' }
]

const chartOptions = {
  responsive: true,
  plugins: {
    legend: {
      display: false
    }
  }
}

const salesChartOptions = {
  responsive: true,
  scales: {
    y: {
      beginAtZero: true,
      ticks: {
        callback: function(value: any) {
          return 'NT$ ' + value.toLocaleString()
        }
      }
    }
  },
  plugins: {
    legend: {
      display: false
    }
  }
}

// Methods
const loadStats = async () => {
  try {
    // TODO: 調用 API 載入統計資料
    stats.value = {
      activeSubscriptions: 128,
      expiringSoon: 8,
      monthlyRevenue: 450000,
      averageUsageRate: 75
    }
  } catch (error) {
    console.error('載入統計資料失敗:', error)
  }
}

const loadExpiringSubscriptions = async () => {
  loadingExpiring.value = true
  try {
    // TODO: 調用 API 載入即將到期的包月方案
    expiringSubscriptions.value = [
      {
        petName: '小白',
        subscriptionType: 'BATH',
        endDate: '2025-08-05',
        daysLeft: 6,
        remainingUsage: 3
      },
      {
        petName: '咪咪',
        subscriptionType: 'GROOM',
        endDate: '2025-08-03',
        daysLeft: 4,
        remainingUsage: 1
      }
    ]
  } catch (error) {
    console.error('載入即將到期的包月方案失敗:', error)
  } finally {
    loadingExpiring.value = false
  }
}

const loadSalesTrend = async () => {
  loadingSales.value = true
  try {
    // TODO: 調用 API 載入銷售趨勢資料
    const months = []
    const data = []

    for (let i = 0; i < 6; i++) {
      months.unshift(dayjs().subtract(i, 'month').format('MM月'))
      data.unshift(Math.floor(Math.random() * 100000) + 300000)
    }

    salesChartData.value = {
      labels: months,
      datasets: [{
        label: '包月銷售額',
        data: data,
        borderColor: '#4ECDC4',
        backgroundColor: 'rgba(78, 205, 196, 0.1)',
        tension: 0.4
      }]
    }
  } catch (error) {
    console.error('載入銷售趨勢失敗:', error)
  } finally {
    loadingSales.value = false
  }
}

const formatDate = (dateStr: string) => {
  return dayjs(dateStr).format('MM/DD')
}

const contactCustomer = (subscription: any) => {
  const petName = subscription.petName || subscription.name || `寵物 #${subscription.petId}`
  toast.add({
    severity: 'info',
    summary: '聯絡客戶',
    detail: `準備聯絡 ${petName} 的主人進行續約`,
    life: 3000
  })
}

// Lifecycle
onMounted(() => {
  loadStats()
  loadExpiringSubscriptions()
  loadSalesTrend()
})
</script>

<style scoped>
.subscription-dashboard {
  padding: 1rem;
}

.grid {
  display: grid;
  grid-template-columns: repeat(12, 1fr);
  gap: 1rem;
}

.col-12 { grid-column: span 12; }
.lg\:col-3 { grid-column: span 3; }
.lg\:col-4 { grid-column: span 4; }
.lg\:col-8 { grid-column: span 8; }
.md\:col-6 { grid-column: span 6; }

@media (max-width: 1024px) {
  .lg\:col-3, .lg\:col-4, .lg\:col-8 {
    grid-column: span 12;
  }
}

@media (max-width: 768px) {
  .md\:col-6 {
    grid-column: span 12;
  }
}

.stat-card {
  border: none;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  transition: transform 0.2s;
}

.stat-card:hover {
  transform: translateY(-2px);
}

.stat-content {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: white;
}

.active-subscriptions .stat-icon {
  background: linear-gradient(135deg, #4ECDC4, #44A08D);
}

.expiring-soon .stat-icon {
  background: linear-gradient(135deg, #FFD93D, #FF8C42);
}

.revenue-this-month .stat-icon {
  background: linear-gradient(135deg, #6BCF7F, #4D9DE0);
}

.usage-rate .stat-icon {
  background: linear-gradient(135deg, #E15FED, #6BCF7F);
}

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 1.8rem;
  font-weight: 700;
  color: var(--p-text-color);
  margin-bottom: 0.25rem;
}

.stat-label {
  font-size: 0.875rem;
  color: var(--p-text-color-secondary);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
}

.card-header h3 {
  margin: 0;
  color: var(--p-text-color);
}

.header-actions {
  display: flex;
  gap: 0.5rem;
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: var(--p-text-color-secondary);
}

.usage-chart {
  text-align: center;
}

.chart-legend {
  margin-top: 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.legend-color {
  width: 12px;
  height: 12px;
  border-radius: 2px;
}

.high-usage { background-color: #FF6B6B; }
.medium-usage { background-color: #4ECDC4; }
.low-usage { background-color: #45B7D1; }
</style>
