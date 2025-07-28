import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// Layout和主要頁面 (已轉換為PrimeVue)
import Dashboard from '@/views/Dashboard.vue'
import Login from '@/views/auth/Login.vue'

// Pet Management (已轉換為PrimeVue)
import PetList from '@/views/pets/PetList.vue'
import PetCreate from '@/views/pets/PetCreate.vue'

// 注意: 以下模組尚未轉換為PrimeVue，暫時移除路由
// - PetEdit.vue (已刪除)
// - 所有Contact相關頁面 (使用Element Plus)
// - 所有Reservation相關頁面 (使用Element Plus)
// - 所有Subscription相關頁面 (使用Element Plus)
// - 所有Financial相關頁面 (使用Element Plus)
// - 所有Settings相關頁面 (使用Element Plus)
// - 所有Permission相關頁面 (使用Element Plus)
// - Login.vue (使用Element Plus)

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    redirect: '/dashboard'
  },
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { requiresAuth: false }
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: Dashboard,
    meta: { requiresAuth: true }
  },
  // Pet Management (已轉換為PrimeVue)
  {
    path: '/pets',
    name: 'PetList',
    component: PetList,
    meta: { requiresAuth: true }
  },
  {
    path: '/pets/create',
    name: 'PetCreate',
    component: PetCreate,
    meta: { requiresAuth: true }
  },
  // 佔位符路由 - 功能開發中
  {
    path: '/reservations',
    name: 'ReservationList',
    component: () => import('@/views/reservations/ReservationList.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/contacts',
    name: 'ContactList',
    component: () => import('@/views/contacts/ContactList.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/subscriptions',
    name: 'SubscriptionList',
    component: () => import('@/views/subscriptions/SubscriptionList.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/income',
    name: 'IncomeList',
    component: () => import('@/views/financial/IncomeList.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/settings/services',
    name: 'ServiceSettings',
    component: () => import('@/views/settings/ServiceSettings.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/settings/system-codes',
    name: 'SystemCodeSettings',
    component: () => import('@/views/settings/SystemCodeSettings.vue'),
    meta: { requiresAuth: true }
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

// Navigation guards
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login')
  } else if (to.path === '/login' && authStore.isAuthenticated) {
    next('/dashboard')
  } else {
    next()
  }
})

export default router