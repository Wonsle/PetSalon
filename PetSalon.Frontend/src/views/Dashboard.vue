<template>
  <div class="dashboard">
    <h1>儀表板</h1>

    <!-- Stats Cards -->
    <div class="stats-grid">
      <Card class="stat-card">
        <template #content>
          <div class="stat-content">
            <div class="stat-icon calendar">
              <i class="pi pi-calendar" style="font-size: 2rem"></i>
            </div>
            <div class="stat-info">
              <h3>{{ todayReservations }}</h3>
              <p>今日預約</p>
            </div>
          </div>
        </template>
      </Card>

      <Card class="stat-card">
        <template #content>
          <div class="stat-content">
            <div class="stat-icon pets">
              <i class="pi pi-users" style="font-size: 2rem"></i>
            </div>
            <div class="stat-info">
              <h3>{{ totalPets }}</h3>
              <p>總寵物數</p>
            </div>
          </div>
        </template>
      </Card>

      <Card class="stat-card">
        <template #content>
          <div class="stat-content">
            <div class="stat-icon revenue">
              <i class="pi pi-dollar" style="font-size: 2rem"></i>
            </div>
            <div class="stat-info">
              <h3>NT$ {{ monthlyRevenue?.toLocaleString() }}</h3>
              <p>本月收入</p>
            </div>
          </div>
        </template>
      </Card>

      <Card class="stat-card">
        <template #content>
          <div class="stat-content">
            <div class="stat-icon subscriptions">
              <i class="pi pi-credit-card" style="font-size: 2rem"></i>
            </div>
            <div class="stat-info">
              <h3>{{ activeSubscriptions }}</h3>
              <p>有效包月</p>
            </div>
          </div>
        </template>
      </Card>
    </div>

    <!-- Today's Reservations -->
    <div class="content-grid">
      <Card class="reservations-card">
        <template #header>
          <div class="card-header">
            <span>今日預約</span>
            <Button
              label="新增預約"
              icon="pi pi-plus"
              @click="$router.push('/reservations/create')"
              size="small"
            />
          </div>
        </template>
        <template #content>
          <DataTable :value="todayReservationList" :loading="loading" stripedRows>
            <Column field="time" header="時間" style="width: 80px">
              <template #body="{ data }">
                {{ formatTime(data.reserverTime) }}
              </template>
            </Column>
            <Column field="petName" header="寵物名稱" style="width: 120px" />
            <Column field="contact" header="主要聯絡人" style="width: 120px">
              <template #body="{ data }">
                <div class="contact-info">
                  <div class="contact-name">{{ data.primaryContactName }}</div>
                  <div class="contact-phone">{{ data.primaryContactPhone }}</div>
                </div>
              </template>
            </Column>
            <Column field="services" header="服務項目">
              <template #body="{ data }">
                <div class="service-tags">
                  <Tag
                    v-for="service in data.services"
                    :key="service"
                    :value="service"
                    severity="info"
                    class="mr-1"
                  />
                </div>
              </template>
            </Column>
            <Column field="status" header="狀態" style="width: 100px">
              <template #body="{ data }">
                <Tag
                  :value="getStatusName(data.status)"
                  :severity="getStatusSeverity(data.status)"
                />
              </template>
            </Column>
            <Column header="操作" style="width: 120px">
              <template #body="{ data }">
                <Button
                  label="編輯"
                  size="small"
                  text
                  @click="editReservation(data.id)"
                />
              </template>
            </Column>
          </DataTable>
        </template>
      </Card>

      <Card class="subscriptions-card">
        <template #header>
          <span>即將到期的包月</span>
        </template>
        <template #content>
          <div v-if="expiringSubscriptions.length === 0" class="no-data">
            <p>暫無即將到期的包月</p>
          </div>

          <div v-else class="expiring-list">
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
              <Button
                label="續約"
                size="small"
                severity="warning"
                @click="renewSubscription(subscription.id)"
              />
            </div>
          </div>
        </template>
      </Card>
    </div>

    <!-- Quick Actions -->
    <Card class="quick-actions-card">
      <template #header>
        <span>快速操作</span>
      </template>
      <template #content>
        <div class="action-buttons">
          <Button
            label="新增預約"
            icon="pi pi-plus"
            @click="$router.push('/reservations/create')"
            size="large"
          />

          <Button
            label="新增寵物"
            icon="pi pi-users"
            severity="success"
            @click="$router.push('/pets/create')"
            size="large"
          />

          <Button
            label="新增聯絡人"
            icon="pi pi-user"
            severity="info"
            @click="$router.push('/contacts/create')"
            size="large"
          />

          <Button
            label="新增包月"
            icon="pi pi-credit-card"
            severity="warning"
            @click="$router.push('/subscriptions/create')"
            size="large"
          />
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import dayjs from 'dayjs'
import { petApi } from '@/api/pet'

const router = useRouter()
const toast = useToast()

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

const getStatusSeverity = (status: string) => {
  const statusMap: Record<string, string> = {
    'PENDING': 'secondary',
    'CONFIRMED': 'success',
    'IN_PROGRESS': 'warning',
    'COMPLETED': 'info',
    'CANCELLED': 'danger',
    'NO_SHOW': 'danger'
  }
  return statusMap[status] || 'secondary'
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
    toast.add({
      severity: 'error',
      summary: '錯誤',
      detail: '載入儀表板資料失敗',
      life: 5000
    })
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

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
  margin-bottom: 24px;
}

.stat-card .stat-content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.stat-icon.calendar {
  background: linear-gradient(135deg, #409eff, #66b1ff);
}

.stat-icon.pets {
  background: linear-gradient(135deg, #67c23a, #85ce61);
}

.stat-icon.revenue {
  background: linear-gradient(135deg, #e6a23c, #ebb563);
}

.stat-icon.subscriptions {
  background: linear-gradient(135deg, #f56c6c, #f78989);
}

.stat-info h3 {
  margin: 0;
  font-size: 24px;
  font-weight: bold;
  color: var(--p-text-color);
}

.stat-info p {
  margin: 4px 0 0 0;
  color: var(--p-text-muted-color);
  font-size: 14px;
}

.content-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
  margin-bottom: 24px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.no-data {
  text-align: center;
  color: var(--p-text-muted-color);
  padding: 20px;
}

.expiring-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.expiring-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  border: 1px solid var(--p-content-border-color);
  border-radius: 6px;
}

.pet-info {
  flex: 1;
}

.expire-date {
  margin: 4px 0 0 0;
  font-size: 12px;
  color: var(--p-text-muted-color);
}

.days-left {
  color: var(--p-orange-500);
  font-weight: bold;
}

.action-buttons {
  display: flex;
  gap: 16px;
  justify-content: center;
  flex-wrap: wrap;
}

.mr-1 {
  margin-right: 4px;
}

.service-tags {
  display: flex;
  gap: 4px;
  flex-wrap: wrap;
}

.contact-info {
  line-height: 1.2;
}

.contact-name {
  font-weight: 500;
  color: var(--p-text-color);
}

.contact-phone {
  font-size: 12px;
  color: var(--p-text-muted-color);
}

@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }

  .content-grid {
    grid-template-columns: 1fr;
  }

  .action-buttons {
    flex-direction: column;
    align-items: center;
  }
}
</style>