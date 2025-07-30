<template>
  <div class="service-settings">
    <Card>
      <template #header>
        <div class="header">
          <h2>⚙️ 系統設定</h2>
        </div>
      </template>
      <template #content>
        <div class="settings-grid">
        <!-- System Code Settings -->
          <Card class="setting-card" @click="goToCodeTypes">
            <template #content>
              <div class="setting-content">
                <div class="setting-icon">
                  <i class="pi pi-cog" style="font-size: 2rem; color: var(--p-primary-500);"></i>
                </div>
                <div class="setting-info">
                  <h3>代碼類別維護</h3>
                  <p>管理系統代碼類別</p>
                  <p class="status available">✅ 可用</p>
                </div>
                <div class="setting-action">
                  <i class="pi pi-chevron-right"></i>
                </div>
              </div>
            </template>
          </Card>
          <!-- System Code Settings -->
          <Card class="setting-card" @click="goToSystemCodes">
            <template #content>
              <div class="setting-content">
                <div class="setting-icon">
                  <i class="pi pi-cog" style="font-size: 2rem; color: var(--p-primary-500);"></i>
                </div>
                <div class="setting-info">
                  <h3>系統代碼維護</h3>
                  <p>管理品種、性別、服務類型等系統代碼</p>
                  <p class="status available">✅ 可用</p>
                </div>
                <div class="setting-action">
                  <i class="pi pi-chevron-right"></i>
                </div>
              </div>
            </template>
          </Card>

          <!-- User Settings - Coming Soon -->
          <Card class="setting-card disabled">
            <template #content>
              <div class="setting-content">
                <div class="setting-icon">
                  <i class="pi pi-users" style="font-size: 2rem; color: var(--p-surface-400);"></i>
                </div>
                <div class="setting-info">
                  <h3>使用者設定</h3>
                  <p>管理使用者帳號、權限和角色</p>
                  <p class="status coming-soon">🚧 開發中</p>
                </div>
              </div>
            </template>
          </Card>

          <!-- Backup & Restore - Coming Soon -->
          <Card class="setting-card disabled">
            <template #content>
              <div class="setting-content">
                <div class="setting-icon">
                  <i class="pi pi-database" style="font-size: 2rem; color: var(--p-surface-400);"></i>
                </div>
                <div class="setting-info">
                  <h3>備份與還原</h3>
                  <p>資料庫備份、還原和匯出功能</p>
                  <p class="status coming-soon">🚧 開發中</p>
                </div>
              </div>
            </template>
          </Card>

          <!-- Email Settings - Coming Soon -->
          <Card class="setting-card disabled">
            <template #content>
              <div class="setting-content">
                <div class="setting-icon">
                  <i class="pi pi-envelope" style="font-size: 2rem; color: var(--p-surface-400);"></i>
                </div>
                <div class="setting-info">
                  <h3>郵件設定</h3>
                  <p>SMTP設定、郵件範本管理</p>
                  <p class="status coming-soon">🚧 開發中</p>
                </div>
              </div>
            </template>
          </Card>

          <!-- System Logs - Coming Soon -->
          <Card class="setting-card disabled">
            <template #content>
              <div class="setting-content">
                <div class="setting-icon">
                  <i class="pi pi-file-o" style="font-size: 2rem; color: var(--p-surface-400);"></i>
                </div>
                <div class="setting-info">
                  <h3>系統日誌</h3>
                  <p>查看系統操作記錄和錯誤日誌</p>
                  <p class="status coming-soon">🚧 開發中</p>
                </div>
              </div>
            </template>
          </Card>

          <!-- General Settings - Coming Soon -->
          <Card class="setting-card disabled">
            <template #content>
              <div class="setting-content">
                <div class="setting-icon">
                  <i class="pi pi-sliders-h" style="font-size: 2rem; color: var(--p-surface-400);"></i>
                </div>
                <div class="setting-info">
                  <h3>一般設定</h3>
                  <p>系統名稱、logo、時區等基本設定</p>
                  <p class="status coming-soon">🚧 開發中</p>
                </div>
              </div>
            </template>
          </Card>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const goToCodeTypes = async () => {
  try {
    await router.push('/settings/code-types')
  } catch (error) {
    console.error('Navigation to code-types failed:', error)
  }
}

const goToSystemCodes = async () => {
  try {
    await router.push('/settings/system-codes')
  } catch (error) {
    console.error('Navigation to system-codes failed:', error)
  }
}
</script>

<style scoped>
.service-settings {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header h2 {
  margin: 0;
  color: var(--p-text-color);
}

.settings-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 20px;
  padding: 20px 0;
}

.setting-card {
  cursor: pointer;
  transition: all 0.3s ease;
  border: 1px solid var(--p-surface-border);
}

.setting-card:hover:not(.disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-color: var(--p-primary-400);
}

.setting-card.disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.setting-content {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 20px;
}

.setting-icon {
  flex-shrink: 0;
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--p-surface-100);
  border-radius: 50%;
}

.setting-info {
  flex: 1;
}

.setting-info h3 {
  margin: 0 0 8px 0;
  color: var(--p-text-color);
  font-size: 1.1rem;
  font-weight: 600;
}

.setting-info p {
  margin: 0 0 4px 0;
  color: var(--p-text-muted-color);
  font-size: 0.9rem;
}

.status {
  font-size: 0.8rem;
  font-weight: 500;
}

.status.available {
  color: var(--p-green-600);
}

.status.coming-soon {
  color: var(--p-orange-600);
}

.setting-action {
  flex-shrink: 0;
  color: var(--p-text-muted-color);
  font-size: 1.2rem;
}

.setting-card.disabled .setting-action {
  display: none;
}

@media (max-width: 768px) {
  .settings-grid {
    grid-template-columns: 1fr;
  }

  .setting-content {
    padding: 16px;
  }

  .setting-icon {
    width: 50px;
    height: 50px;
  }

  .setting-icon i {
    font-size: 1.5rem !important;
  }
}
</style>
