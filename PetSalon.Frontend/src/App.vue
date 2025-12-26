<template>
  <div id="app">
    <div class="layout-container">
      <!-- Header -->
      <header class="header">
        <div class="header-content">
          <div class="logo">
            <h1>🐾 Amada Pet Grooming</h1>
          </div>

          <!-- Menu Button (Desktop & Mobile) -->
          <div class="menu-button" v-if="isLoggedIn">
            <Button
              icon="pi pi-bars"
              @click="toggleMenu"
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

      <!-- Sidebar Menu -->
      <Sidebar v-model:visible="menuVisible" :baseZIndex="1000">
        <h3 class="menu-title">功能選單</h3>
        <PanelMenu :model="menuItems" class="navigation-menu">
          <template #item="{ item }">
            <router-link v-if="item.route" v-slot="{ href, navigate }" :to="item.route" custom>
              <a
                v-ripple
                class="flex items-center cursor-pointer px-4 py-2"
                :href="href"
                @click="handleNavigation(navigate)"
              >
                <span :class="item.icon" />
                <span class="ml-2">{{ item.label }}</span>
              </a>
            </router-link>
            <a
              v-else
              v-ripple
              class="flex items-center cursor-pointer px-4 py-2"
            >
              <span :class="item.icon" />
              <span class="ml-2">{{ item.label }}</span>
              <span v-if="item.items" class="pi pi-angle-down text-primary ml-auto" />
            </a>
          </template>
        </PanelMenu>
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
const menuVisible = ref(false)
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

// Navigation menu items (using route property for proper routing)
const menuItems = ref([
  {
    key: 'dashboard',
    label: '儀表板',
    icon: 'pi pi-home',
    route: '/dashboard'
  },
  {
    key: 'pets',
    label: '寵物管理',
    icon: 'pi pi-users',
    route: '/pets'
  },
  {
    key: 'contacts',
    label: '聯絡人管理',
    icon: 'pi pi-user',
    route: '/contacts'
  },
  {
    key: 'reservations',
    label: '預約管理',
    icon: 'pi pi-calendar',
    route: '/reservations'
  },
  {
    key: 'subscriptions',
    label: '包月管理',
    icon: 'pi pi-credit-card',
    route: '/subscriptions'
  },
  {
    key: 'financial',
    label: '財務管理',
    icon: 'pi pi-dollar',
    route: '/income'
  },
  {
    key: 'settings',
    label: '系統設定',
    icon: 'pi pi-cog',
    route: '/settings/services'
  }
])

const toggleUserMenu = (event: Event) => {
  userMenu.value.toggle(event)
}

const toggleMenu = () => {
  menuVisible.value = !menuVisible.value
}

const handleNavigation = (navigate: () => void) => {
  navigate()
  menuVisible.value = false
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

/* Menu Button */
.menu-button {
  display: flex;
  align-items: center;
  margin-left: auto;
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

/* Sidebar Menu */
.menu-title {
  margin: 0 0 1rem 0;
  padding-bottom: 1rem;
  border-bottom: 1px solid #e9ecef;
  color: #495057;
  font-size: 1.25rem;
  font-weight: 600;
}

.navigation-menu {
  border: none;
  width: 100%;
}

.navigation-menu :deep(.p-panelmenu-panel) {
  border: none;
  margin-bottom: 0.5rem;
}

.navigation-menu :deep(.p-panelmenu-header-content) {
  border-radius: 6px;
  padding: 0.75rem 1rem;
  background-color: #f8f9fa;
  transition: all 0.2s ease;
}

.navigation-menu :deep(.p-panelmenu-header-content:hover) {
  background-color: #e9ecef;
}

.navigation-menu :deep(.p-menuitem-link),
.navigation-menu a {
  padding: 0.75rem 1rem;
  border-radius: 6px;
  transition: all 0.2s ease;
  color: #495057;
  text-decoration: none;
}

.navigation-menu :deep(.p-menuitem-link:hover),
.navigation-menu a:hover {
  background-color: #e9ecef;
}

.navigation-menu :deep(.p-menuitem-icon),
.navigation-menu .pi {
  color: #409EFF;
  margin-right: 0.5rem;
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

  .main-content {
    padding: 1rem;
  }
}
</style>