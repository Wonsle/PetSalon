<template>
  <div id="app">
    <el-container class="layout-container">
      <!-- Header -->
      <el-header class="header">
        <div class="header-content">
          <div class="logo">
            <h1>🐾 Amada Pet Grooming</h1>
          </div>
          <div class="header-actions">
            <el-button type="primary" @click="$router.push('/login')" v-if="!isLoggedIn">
              登入
            </el-button>
            <el-dropdown v-else>
              <span class="el-dropdown-link">
                {{ currentUser?.name || '使用者' }}
                <el-icon class="el-icon--right">
                  <arrow-down />
                </el-icon>
              </span>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item @click="logout">登出</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </div>
        </div>
      </el-header>

      <el-container>
        <!-- Sidebar -->
        <el-aside class="sidebar" v-if="isLoggedIn">
          <el-menu
            default-active="dashboard"
            class="el-menu-vertical"
            :router="true"
            background-color="#545c64"
            text-color="#fff"
            active-text-color="#ffd04b"
          >
            <el-menu-item index="/dashboard">
              <el-icon><House /></el-icon>
              <span>儀表板</span>
            </el-menu-item>
            
            <el-sub-menu index="reservations">
              <template #title>
                <el-icon><Calendar /></el-icon>
                <span>預約管理</span>
              </template>
              <el-menu-item index="/reservations">預約清單</el-menu-item>
              <el-menu-item index="/reservations/calendar">預約行事曆</el-menu-item>
              <el-menu-item index="/reservations/create">新增預約</el-menu-item>
            </el-sub-menu>

            <el-sub-menu index="pets">
              <template #title>
                <el-icon><User /></el-icon>
                <span>寵物管理</span>
              </template>
              <el-menu-item index="/pets">寵物清單</el-menu-item>
              <el-menu-item index="/pets/create">新增寵物</el-menu-item>
            </el-sub-menu>

            <el-sub-menu index="contacts">
              <template #title>
                <el-icon><UserFilled /></el-icon>
                <span>聯絡人管理</span>
              </template>
              <el-menu-item index="/contacts">聯絡人清單</el-menu-item>
              <el-menu-item index="/contacts/create">新增聯絡人</el-menu-item>
            </el-sub-menu>

            <el-sub-menu index="subscriptions">
              <template #title>
                <el-icon><CreditCard /></el-icon>
                <span>包月管理</span>
              </template>
              <el-menu-item index="/subscriptions">包月清單</el-menu-item>
              <el-menu-item index="/subscriptions/create">新增包月</el-menu-item>
            </el-sub-menu>

            <el-sub-menu index="financial">
              <template #title>
                <el-icon><Money /></el-icon>
                <span>財務管理</span>
              </template>
              <el-menu-item index="/income">收入管理</el-menu-item>
              <el-menu-item index="/expenses">支出管理</el-menu-item>
              <el-menu-item index="/reports">財務報表</el-menu-item>
            </el-sub-menu>

            <el-sub-menu index="permissions" v-if="hasPermission('role:view')">
              <template #title>
                <el-icon><Lock /></el-icon>
                <span>權限管理</span>
              </template>
              <el-menu-item index="/permissions/roles">角色管理</el-menu-item>
              <el-menu-item index="/permissions/user-roles">用戶角色</el-menu-item>
            </el-sub-menu>

            <el-sub-menu index="settings">
              <template #title>
                <el-icon><Setting /></el-icon>
                <span>系統設定</span>
              </template>
              <el-menu-item index="/settings/services">服務項目</el-menu-item>
              <el-menu-item index="/settings/system-codes">系統代碼</el-menu-item>
              <el-menu-item index="/settings/users">使用者管理</el-menu-item>
            </el-sub-menu>
          </el-menu>
        </el-aside>

        <!-- Main Content -->
        <el-main class="main-content">
          <RouterView />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { RouterView } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

const isLoggedIn = computed(() => authStore.isAuthenticated)
const currentUser = computed(() => authStore.currentUser)

const logout = () => {
  authStore.logout()
}
</script>

<style scoped>
.layout-container {
  height: 100vh;
}

.header {
  background-color: #409EFF;
  color: white;
  display: flex;
  align-items: center;
  padding: 0 20px;
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
}

.sidebar {
  width: 220px !important;
  background-color: #545c64;
}

.main-content {
  background-color: #f5f5f5;
  padding: 20px;
}

.el-dropdown-link {
  cursor: pointer;
  color: white;
  display: flex;
  align-items: center;
}
</style>