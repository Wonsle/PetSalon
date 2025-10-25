<template>
  <div id="app">
    <div class="layout-container">
      <!-- Header -->
      <header class="header">
        <div class="header-content">
          <div class="logo">
            <h1>🐾 Amada Pet Grooming</h1>
          </div>

          <!-- Desktop MegaMenu -->
          <div class="desktop-menu" v-if="isLoggedIn">
            <MegaMenu :model="megaMenuItems" class="main-megamenu" />
          </div>

          <!-- Mobile Hamburger Button -->
          <div class="mobile-menu-button" v-if="isLoggedIn">
            <Button
              icon="pi pi-bars"
              @click="toggleMobileMenu"
              text
              class="hamburger-button"
            />
          </div>

          <div class="header-actions">
            <Button
              label="登入"
              @click="$router.push('/login')"
              v-if="!isLoggedIn"
            />
            <div v-else class="user-menu-container">
              <Button
                :label="currentUser?.name || '使用者'"
                icon="pi pi-chevron-down"
                @click="toggleUserMenu"
                text
                class="user-menu-button"
              />
              <Menu
                ref="userMenu"
                :model="userMenuItems"
                :popup="true"
              />
            </div>
          </div>
        </div>
      </header>

      <!-- Mobile Sidebar Menu -->
      <Sidebar v-model:visible="mobileMenuVisible" :baseZIndex="1000">
        <h3 class="mobile-menu-title">功能選單</h3>
        <PanelMenu :model="mobileMenuItems" class="mobile-menu" />
      </Sidebar>

      <!-- Main Content -->
      <main class="main-content">
        <RouterView />
      </main>

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
const mobileMenuVisible = ref(false)
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

// MegaMenu items for desktop
const megaMenuItems = ref([
  {
    label: '儀表板',
    icon: 'pi pi-home',
    command: () => router.push('/dashboard')
  },
  {
    label: '客戶管理',
    icon: 'pi pi-users',
    items: [
      [
        {
          label: '寵物相關',
          items: [
            {
              label: '寵物管理',
              icon: 'pi pi-users',
              description: '管理寵物資料、品種、照片等',
              command: () => router.push('/pets')
            }
          ]
        },
        {
          label: '聯絡人相關',
          items: [
            {
              label: '聯絡人管理',
              icon: 'pi pi-user',
              description: '管理飼主和聯絡人資訊',
              command: () => router.push('/contacts')
            }
          ]
        }
      ]
    ]
  },
  {
    label: '業務管理',
    icon: 'pi pi-briefcase',
    items: [
      [
        {
          label: '預約與訂閱',
          items: [
            {
              label: '預約管理',
              icon: 'pi pi-calendar',
              description: '處理預約排程和服務安排',
              command: () => router.push('/reservations')
            },
            {
              label: '包月管理',
              icon: 'pi pi-credit-card',
              description: '管理包月方案和訂閱',
              command: () => router.push('/subscriptions')
            }
          ]
        }
      ]
    ]
  },
  {
    label: '財務管理',
    icon: 'pi pi-dollar',
    command: () => router.push('/income')
  },
  {
    label: '系統設定',
    icon: 'pi pi-cog',
    command: () => router.push('/settings/services')
  }
])

// Mobile menu items (flat structure for Sidebar)
const mobileMenuItems = ref([
  {
    key: 'dashboard',
    label: '儀表板',
    icon: 'pi pi-home',
    command: () => {
      router.push('/dashboard')
      mobileMenuVisible.value = false
    }
  },
  {
    key: 'pets',
    label: '寵物管理',
    icon: 'pi pi-users',
    command: () => {
      router.push('/pets')
      mobileMenuVisible.value = false
    }
  },
  {
    key: 'contacts',
    label: '聯絡人管理',
    icon: 'pi pi-user',
    command: () => {
      router.push('/contacts')
      mobileMenuVisible.value = false
    }
  },
  {
    key: 'reservations',
    label: '預約管理',
    icon: 'pi pi-calendar',
    command: () => {
      router.push('/reservations')
      mobileMenuVisible.value = false
    }
  },
  {
    key: 'subscriptions',
    label: '包月管理',
    icon: 'pi pi-credit-card',
    command: () => {
      router.push('/subscriptions')
      mobileMenuVisible.value = false
    }
  },
  {
    key: 'financial',
    label: '財務管理',
    icon: 'pi pi-dollar',
    command: () => {
      router.push('/income')
      mobileMenuVisible.value = false
    }
  },
  {
    key: 'settings',
    label: '系統設定',
    icon: 'pi pi-cog',
    command: () => {
      router.push('/settings/services')
      mobileMenuVisible.value = false
    }
  }
])

const toggleUserMenu = (event: Event) => {
  userMenu.value.toggle(event)
}

const toggleMobileMenu = () => {
  mobileMenuVisible.value = !mobileMenuVisible.value
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
  padding: 0.75rem 2rem;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  z-index: 1000;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  gap: 2rem;
}

.logo h1 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  white-space: nowrap;
}

/* Desktop MegaMenu */
.desktop-menu {
  flex: 1;
  display: flex;
  justify-content: center;
}

.main-megamenu {
  background: transparent !important;
  border: none !important;
}

.main-megamenu :deep(.p-menubar) {
  background: transparent !important;
  border: none !important;
  padding: 0;
}

.main-megamenu :deep(.p-menubar-root-list) {
  gap: 0.5rem;
}

.main-megamenu :deep(.p-menubar-item-link) {
  color: white !important;
  padding: 0.75rem 1rem !important;
  border-radius: 6px;
  transition: all 0.2s ease;
}

.main-megamenu :deep(.p-menubar-item-link:hover) {
  background: rgba(255, 255, 255, 0.15) !important;
}

.main-megamenu :deep(.p-menubar-item-link:focus) {
  box-shadow: 0 0 0 0.2rem rgba(255, 255, 255, 0.3) !important;
}

.main-megamenu :deep(.p-megamenu-panel) {
  margin-top: 0.5rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.main-megamenu :deep(.p-megamenu-submenu-label) {
  font-weight: 600;
  color: #495057;
  padding: 0.75rem 1rem;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.main-megamenu :deep(.p-megamenu-submenu .p-menuitem-link) {
  color: #495057 !important;
  padding: 0.75rem 1rem !important;
  border-radius: 6px;
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
}

.main-megamenu :deep(.p-megamenu-submenu .p-menuitem-link:hover) {
  background: #f8f9fa !important;
}

.main-megamenu :deep(.p-megamenu-submenu .p-menuitem-icon) {
  font-size: 1.25rem;
  color: #409EFF;
  margin-top: 0.125rem;
}

.main-megamenu :deep(.p-megamenu-submenu .p-menuitem-text) {
  font-weight: 500;
  font-size: 0.95rem;
}

/* Mobile Hamburger Button */
.mobile-menu-button {
  display: none;
}

.hamburger-button {
  color: white !important;
  border: none !important;
  background: transparent !important;
  font-size: 1.5rem !important;
  padding: 0.5rem !important;
}

.hamburger-button:hover {
  background: rgba(255, 255, 255, 0.15) !important;
  border-radius: 6px;
}

/* Mobile Sidebar Menu */
.mobile-menu-title {
  margin: 0 0 1rem 0;
  padding-bottom: 1rem;
  border-bottom: 1px solid #e9ecef;
  color: #495057;
}

.mobile-menu {
  border: none;
  width: 100%;
}

.mobile-menu :deep(.p-panelmenu-panel) {
  border: none;
  margin-bottom: 0.5rem;
}

.mobile-menu :deep(.p-panelmenu-header-content) {
  border-radius: 6px;
  padding: 0.75rem 1rem;
  background-color: #f8f9fa;
  transition: all 0.2s ease;
}

.mobile-menu :deep(.p-panelmenu-header-content:hover) {
  background-color: #e9ecef;
}

.mobile-menu :deep(.p-menuitem-link) {
  padding: 0.75rem 1rem;
  border-radius: 6px;
  transition: all 0.2s ease;
}

.mobile-menu :deep(.p-menuitem-link:hover) {
  background-color: #e9ecef;
}

/* Header Actions */
.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-menu-container {
  position: relative;
  display: flex;
  align-items: center;
}

.user-menu-button {
  color: white !important;
  border: none !important;
  background: transparent !important;
}

.user-menu-button:hover {
  background: rgba(255, 255, 255, 0.1) !important;
  border-radius: 6px;
}

/* Main Content */
.main-content {
  flex: 1;
  background-color: #f8f9fa;
  padding: 2rem;
  overflow-y: auto;
}

/* 響應式設計 */
@media (max-width: 768px) {
  .header {
    padding: 0.75rem 1rem;
  }

  .header-content {
    gap: 1rem;
  }

  .logo h1 {
    font-size: 1.25rem;
  }

  /* 隱藏桌面選單 */
  .desktop-menu {
    display: none !important;
  }

  /* 顯示漢堡按鈕 */
  .mobile-menu-button {
    display: block;
  }

  .main-content {
    padding: 1rem;
  }
}

/* 平板尺寸 */
@media (min-width: 769px) and (max-width: 1024px) {
  .main-megamenu :deep(.p-menubar-item-link) {
    padding: 0.5rem 0.75rem !important;
    font-size: 0.9rem;
  }
}
</style>