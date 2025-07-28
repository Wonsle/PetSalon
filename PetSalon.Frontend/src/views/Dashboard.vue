<template>
  <div class="dashboard">
    <h1>儀表板</h1>

    <!-- Stats Cards -->
    <el-row :gutter="20" class="stats-row">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="40" color="#409EFF"><Calendar /></el-icon>
            </div>
            <div class="stat-info">
              <h3>{{ todayReservations }}</h3>
              <p>今日預約</p>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="40" color="#67C23A"><User /></el-icon>
            </div>
            <div class="stat-info">
              <h3>{{ totalPets }}</h3>
              <p>總寵物數</p>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="40" color="#E6A23C"><Money /></el-icon>
            </div>
            <div class="stat-info">
              <h3>NT$ {{ monthlyRevenue?.toLocaleString() }}</h3>
              <p>本月收入</p>
            </div>
          </div>
        </el-card>
      </el-col>

      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon">
              <el-icon size="40" color="#F56C6C"><CreditCard /></el-icon>
            </div>
            <div class="stat-info">
              <h3>{{ activeSubscriptions }}</h3>
              <p>有效包月</p>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- Today's Reservations -->
    <el-row :gutter="20" class="content-row">
      <el-col :span="16">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>今日預約</span>
              <el-button type="primary" @click="$router.push('/reservations/create')">
                新增預約
              </el-button>
            </div>
          </template>

          <el-table :data="todayReservationList" v-loading="loading">
            <el-table-column prop="time" label="時間" width="80">
              <template #default="{ row }">
                {{ formatTime(row.reserverTime) }}
              </template>
            </el-table-column>
            <el-table-column prop="petName" label="寵物名稱" width="120" />
            <el-table-column prop="contactName" label="主要聯絡人" width="120">
              <template #default="{ row }">
                <div class="contact-info">
                  <span class="contact-name">{{ row.primaryContactName }}</span>
                  <br />
                  <span class="contact-phone">{{ row.primaryContactPhone }}</span>
                </div>
              </template>
            </el-table-column>
            <el-table-column prop="services" label="服務項目">
              <template #default="{ row }">
                <el-tag
                  v-for="service in row.services"
                  :key="service"
                  size="small"
                  class="mr-1"
                >
                  {{ service }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="status" label="狀態" width="100">
              <template #default="{ row }">
                <el-tag
                  :type="getStatusType(row.status)"
                  size="small"
                >
                  {{ getStatusName(row.status) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="120">
              <template #default="{ row }">
                <el-button
                  size="small"
                  @click="editReservation(row.id)"
                >
                  編輯
                </el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>

      <el-col :span="8">
        <el-card>
          <template #header>
            <span>即將到期的包月</span>
          </template>

          <div v-if="expiringSubscriptions.length === 0" class="no-data">
            <p>暫無即將到期的包月</p>
          </div>

          <div v-else>
            <div
              v-for="subscription in expiringSubscriptions"
              :key="subscription.id"
              class="expiring-item"
            >
              <div class="pet-info">
                <strong>{{ subscription.petName }}</strong>
                <p class="expire-date">
                  到期日: {{ formatDate(subscription.endDate) }}
                  <span class="days-left">({{ subscription.daysLeft }}天)</span>
                </p>
              </div>
              <el-button size="small" type="warning" @click="renewSubscription(subscription.id)">
                續約
              </el-button>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- Quick Actions -->
    <el-row class="quick-actions">
      <el-col :span="24">
        <el-card>
          <template #header>
            <span>快速操作</span>
          </template>

          <div class="action-buttons">
            <el-button
              type="primary"
              size="large"
              @click="$router.push('/reservations/create')"
            >
              <el-icon><Plus /></el-icon>
              新增預約
            </el-button>

            <el-button
              type="success"
              size="large"
              @click="$router.push('/pets/create')"
            >
              <el-icon><User /></el-icon>
              新增寵物
            </el-button>

            <el-button
              type="info"
              size="large"
              @click="$router.push('/contacts/create')"
            >
              <el-icon><UserFilled /></el-icon>
              新增聯絡人
            </el-button>

            <el-button
              type="warning"
              size="large"
              @click="$router.push('/subscriptions/create')"
            >
              <el-icon><CreditCard /></el-icon>
              新增包月
            </el-button>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import dayjs from 'dayjs'
import { petApi } from '@/api/pet'
import { ElMessage } from 'element-plus'

const router = useRouter()

// Reactive data
const loading = ref(false)
const todayReservations = ref(0)
const totalPets = ref(0)
const monthlyRevenue = ref(0)
const activeSubscriptions = ref(0)
interface TodayReservation {
  id: number
  reserverTime: number
  petName: string
  primaryContactName: string
  primaryContactPhone: string
  contactName: string
  contactPhone: string
  services: string[]
  status: string
}

interface ExpiringSubscription {
  id: number
  petName: string
  endDate: string
  daysLeft: number
}

const todayReservationList = ref<TodayReservation[]>([])
const expiringSubscriptions = ref<ExpiringSubscription[]>([])

// Methods
const formatTime = (minutes: number) => {
  const hours = Math.floor(minutes / 60)
  const mins = minutes % 60
  return `${hours.toString().padStart(2, '0')}:${mins.toString().padStart(2, '0')}`
}

const formatDate = (date: string) => {
  return dayjs(date).format('YYYY/MM/DD')
}

const getStatusType = (status: string) => {
  const statusMap: Record<string, string> = {
    'PENDING': '',
    'CONFIRMED': 'success',
    'IN_PROGRESS': 'warning',
    'COMPLETED': 'info',
    'CANCELLED': 'danger',
    'NO_SHOW': 'danger'
  }
  return statusMap[status] || ''
}

const getStatusName = (status: string) => {
  const statusNameMap: Record<string, string> = {
    'PENDING': '待確認',
    'CONFIRMED': '已確認',
    'IN_PROGRESS': '進行中',
    'COMPLETED': '已完成',
    'CANCELLED': '已取消',
    'NO_SHOW': '未出現'
  }
  return statusNameMap[status] || status
}

const editReservation = (id: number) => {
  router.push(`/reservations/${id}/edit`)
}

const renewSubscription = (id: number) => {
  router.push(`/subscriptions/${id}/edit`)
}

const loadDashboardData = async () => {
  loading.value = true
  try {
    // 載入實際的寵物資料
    const petResponse = await petApi.getPets({ pageSize: 1000 })
    totalPets.value = petResponse.total

    // 載入今日預約資料（暫時使用模擬資料，待預約 API 完成）
    todayReservations.value = 8

    // 載入月收入資料（暫時使用模擬資料，待財務 API 完成）
    monthlyRevenue.value = 45000

    // 載入有效包月資料（暫時使用模擬資料，待包月 API 完成）
    activeSubscriptions.value = 23

    // 載入今日預約列表（暫時使用模擬資料）
    todayReservationList.value = [
      {
        id: 1,
        reserverTime: 540, // 9:00
        petName: '小白',
        primaryContactName: '王小明',
        primaryContactPhone: '0912345678',
        contactName: '王小明',
        contactPhone: '0912345678',
        services: ['洗澡', '美容'],
        status: 'CONFIRMED'
      },
      {
        id: 2,
        reserverTime: 660, // 11:00
        petName: '咪咪',
        primaryContactName: '李小華',
        primaryContactPhone: '0987654321',
        contactName: '李小華',
        contactPhone: '0987654321',
        services: ['美容', '貴賓腳'],
        status: 'PENDING'
      }
    ]

    expiringSubscriptions.value = [
      {
        id: 1,
        petName: '小黑',
        endDate: '2024-02-15',
        daysLeft: 3
      },
      {
        id: 2,
        petName: '球球',
        endDate: '2024-02-18',
        daysLeft: 6
      }
    ]
  } catch (error) {
    console.error('Failed to load dashboard data:', error)
    ElMessage.error('載入儀表板資料失敗')
    // 如果 API 失敗，使用預設值
    totalPets.value = 0
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadDashboardData()
})
</script>

<style scoped>
.dashboard {
  padding: 20px;
}

.stats-row {
  margin-bottom: 20px;
}

.stat-card .stat-content {
  display: flex;
  align-items: center;
}

.stat-icon {
  margin-right: 16px;
}

.stat-info h3 {
  margin: 0;
  font-size: 24px;
  font-weight: bold;
}

.stat-info p {
  margin: 4px 0 0 0;
  color: #666;
  font-size: 14px;
}

.content-row {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.no-data {
  text-align: center;
  color: #999;
  padding: 20px;
}

.expiring-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
}

.expiring-item:last-child {
  border-bottom: none;
}

.pet-info {
  flex: 1;
}

.expire-date {
  margin: 4px 0 0 0;
  font-size: 12px;
  color: #666;
}

.days-left {
  color: #E6A23C;
  font-weight: bold;
}

.quick-actions .action-buttons {
  display: flex;
  gap: 16px;
  justify-content: center;
}

.mr-1 {
  margin-right: 4px;
}

.contact-info {
  line-height: 1.2;
}

.contact-name {
  font-weight: 500;
  color: #303133;
}

.contact-phone {
  font-size: 12px;
  color: #909399;
}
</style>