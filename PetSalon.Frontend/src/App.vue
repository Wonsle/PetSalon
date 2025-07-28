<template>
  <div id="app">
    <div class="layout-container">
      <!-- Header -->
      <header class="header">
        <div class="header-content">
          <div class="logo">
            <h1>🐾 Amada Pet Grooming</h1>
          </div>
          <div class="header-actions">
            <Button
              label="登入"
              @click="$router.push('/login')"
              v-if="!isLoggedIn"
            />
            <Menu
              v-else
              ref="userMenu"
              :model="userMenuItems"
              :popup="true"
            >
              <template #start>
                <Button
                  :label="currentUser?.name || '使用者'"
                  icon="pi pi-chevron-down"
                  @click="toggleUserMenu"
                  text
                  class="user-menu-button"
                />
              </template>
            </Menu>
          </div>
        </div>
      </header>

      <div class="layout-body">
        <!-- Sidebar -->
        <aside class="sidebar" v-if="isLoggedIn">
          <PanelMenu :model="menuItems" class="sidebar-menu" />
        </aside>

        <!-- Main Content -->
        <main class="main-content">
          <RouterView />
        </main>
      </div>

      <!-- Toast for notifications -->
      <Toast />
      <!-- Confirm Dialog -->
      <ConfirmDialog />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, nextTick } from 'vue'
import { RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { usePermission } from '@/composables/usePermission'
import { useToast } from 'primevue/usetoast'

const authStore = useAuthStore()
const { hasPermission } = usePermission()
const router = useRouter()
const userMenu = ref()
let toast: any = null

const isLoggedIn = computed(() => authStore.isAuthenticated)
const currentUser = computed(() => authStore.currentUser)

// User menu items
const userMenuItems = ref([
  {
    label: '登出',
    icon: 'pi pi-sign-out',
    command: () => logout()
  }
])

// Sidebar menu items (已轉換為PrimeVue的功能)
const menuItems = ref([
  {
    key: 'dashboard',
    label: '儀表板',
    icon: 'pi pi-home',
    command: () => router.push('/dashboard')
  },
  {
    key: 'pets',
    label: '寵物管理',
    icon: 'pi pi-users',
    items: [
      {
        key: 'pets-list',
        label: '寵物清單',
        command: () => router.push('/pets')
      },
      {
        key: 'pets-create',
        label: '新增寵物',
        command: () => router.push('/pets/create')
      }
    ]
  },
  {
    key: 'reservations',
    label: '預約管理',
    icon: 'pi pi-calendar',
    command: () => router.push('/reservations')
  },
  {
    key: 'contacts',
    label: '聯絡人管理',
    icon: 'pi pi-user',
    command: () => router.push('/contacts')
  },
  {
    key: 'subscriptions',
    label: '包月管理',
    icon: 'pi pi-credit-card',
    command: () => router.push('/subscriptions')
  },
  {
    key: 'financial',
    label: '財務管理',
    icon: 'pi pi-dollar',
    command: () => router.push('/income')
  },
  {
    key: 'settings',
    label: '系統設定',
    icon: 'pi pi-cog',
    command: () => router.push('/settings/services')
  }
])

const toggleUserMenu = (event: Event) => {
  userMenu.value.toggle(event)
}

const logout = () => {
  authStore.logout()
  router.push('/login')
  if (toast) {
    toast.add({
      severity: 'success',
      summary: '登出成功',
      detail: '您已成功登出系統',
      life: 3000
    })
  }
}

// Initialize auth store on app mount
onMounted(async () => {
  await authStore.initialize()
  // Setup toast after component is mounted
  await nextTick()
  toast = useToast()
  // Setup global toast for axios interceptors
  window.$toast = toast
})
</script>

<style scoped>
.layout-container {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.header {
  background-color: #409EFF;
  color: white;
  padding: 1rem 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  z-index: 1000;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

.logo h1 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-menu-button {
  color: white !important;
  border: none !important;
}

.layout-body {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.sidebar {
  width: 280px;
  background-color: #f8f9fa;
  border-right: 1px solid #e9ecef;
  overflow-y: auto;
}

.sidebar-menu {
  border: none;
  width: 100%;
}

.sidebar-menu :deep(.p-panelmenu-panel) {
  border: none;
}

.sidebar-menu :deep(.p-panelmenu-header-content) {
  border-radius: 0;
  padding: 1rem;
  background-color: transparent;
}

.sidebar-menu :deep(.p-panelmenu-content) {
  border: none;
  background-color: transparent;
}

.sidebar-menu :deep(.p-menuitem-link) {
  padding: 0.75rem 1rem 0.75rem 2rem;
  border-radius: 0;
}

.sidebar-menu :deep(.p-menuitem-link:hover) {
  background-color: #e9ecef;
}

.main-content {
  flex: 1;
  background-color: #f8f9fa;
  padding: 2rem;
  overflow-y: auto;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .layout-body {
    flex-direction: column;
  }

  .sidebar {
    width: 100%;
    height: auto;
  }

  .main-content {
    padding: 1rem;
  }

  .header {
    padding: 1rem;
  }

  .header-content {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }
}
</style>