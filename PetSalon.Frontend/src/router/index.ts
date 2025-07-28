import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// Layout
import Dashboard from '@/views/Dashboard.vue'

// Auth
import Login from '@/views/auth/Login.vue'

// Pet Management
import PetList from '@/views/pets/PetList.vue'
import PetCreate from '@/views/pets/PetCreate.vue'
import PetEdit from '@/views/pets/PetEdit.vue'

// Contact Management
import ContactList from '@/views/contacts/ContactList.vue'
import ContactCreate from '@/views/contacts/ContactCreate.vue'
import ContactEdit from '@/views/contacts/ContactEdit.vue'

// Reservation Management
import ReservationList from '@/views/reservations/ReservationList.vue'
import ReservationCalendar from '@/views/reservations/ReservationCalendar.vue'
import ReservationCreate from '@/views/reservations/ReservationCreate.vue'
import ReservationEdit from '@/views/reservations/ReservationEdit.vue'

// Subscription Management
import SubscriptionList from '@/views/subscriptions/SubscriptionList.vue'
import SubscriptionCreate from '@/views/subscriptions/SubscriptionCreate.vue'
import SubscriptionEdit from '@/views/subscriptions/SubscriptionEdit.vue'

// Financial Management
import IncomeList from '@/views/financial/IncomeList.vue'
import ExpenseList from '@/views/financial/ExpenseList.vue'
import Reports from '@/views/financial/Reports.vue'

// Settings
import ServiceSettings from '@/views/settings/ServiceSettings.vue'
import SystemCodeSettings from '@/views/settings/SystemCodeSettings.vue'
import UserSettings from '@/views/settings/UserSettings.vue'

// Permission Management
import RoleList from '@/views/permissions/RoleList.vue'
import UserRoleList from '@/views/permissions/UserRoleList.vue'

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
  // Pet Management
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
  {
    path: '/pets/:id/edit',
    name: 'PetEdit',
    component: PetEdit,
    meta: { requiresAuth: true }
  },
  // Contact Management
  {
    path: '/contacts',
    name: 'ContactList',
    component: ContactList,
    meta: { requiresAuth: true }
  },
  {
    path: '/contacts/create',
    name: 'ContactCreate',
    component: ContactCreate,
    meta: { requiresAuth: true }
  },
  {
    path: '/contacts/:id/edit',
    name: 'ContactEdit',
    component: ContactEdit,
    meta: { requiresAuth: true }
  },
  // Reservation Management
  {
    path: '/reservations',
    name: 'ReservationList',
    component: ReservationList,
    meta: { requiresAuth: true }
  },
  {
    path: '/reservations/calendar',
    name: 'ReservationCalendar',
    component: ReservationCalendar,
    meta: { requiresAuth: true }
  },
  {
    path: '/reservations/create',
    name: 'ReservationCreate',
    component: ReservationCreate,
    meta: { requiresAuth: true }
  },
  {
    path: '/reservations/:id/edit',
    name: 'ReservationEdit',
    component: ReservationEdit,
    meta: { requiresAuth: true }
  },
  // Subscription Management
  {
    path: '/subscriptions',
    name: 'SubscriptionList',
    component: SubscriptionList,
    meta: { requiresAuth: true }
  },
  {
    path: '/subscriptions/create',
    name: 'SubscriptionCreate',
    component: SubscriptionCreate,
    meta: { requiresAuth: true }
  },
  {
    path: '/subscriptions/:id/edit',
    name: 'SubscriptionEdit',
    component: SubscriptionEdit,
    meta: { requiresAuth: true }
  },
  // Financial Management
  {
    path: '/income',
    name: 'IncomeList',
    component: IncomeList,
    meta: { requiresAuth: true }
  },
  {
    path: '/expenses',
    name: 'ExpenseList',
    component: ExpenseList,
    meta: { requiresAuth: true }
  },
  {
    path: '/reports',
    name: 'Reports',
    component: Reports,
    meta: { requiresAuth: true }
  },
  // Settings
  {
    path: '/settings/services',
    name: 'ServiceSettings',
    component: ServiceSettings,
    meta: { requiresAuth: true }
  },
  {
    path: '/settings/system-codes',
    name: 'SystemCodeSettings',
    component: SystemCodeSettings,
    meta: { requiresAuth: true }
  },
  {
    path: '/settings/users',
    name: 'UserSettings',
    component: UserSettings,
    meta: { requiresAuth: true }
  },
  // Permission Management
  {
    path: '/permissions/roles',
    name: 'RoleList',
    component: RoleList,
    meta: { requiresAuth: true, permission: 'role:view' }
  },
  {
    path: '/permissions/user-roles',
    name: 'UserRoleList',
    component: UserRoleList,
    meta: { requiresAuth: true, permission: 'user-role:view' }
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