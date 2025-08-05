<template>
  <div class="reservation-create">
    <div class="page-header">
      <div class="header-content">
        <h1>新增預約</h1>
        <p>建立新的寵物美容預約</p>
      </div>
    </div>

    <div class="content-wrapper">
      <Card>
        <template #content>
          <ReservationForm
            :visible="true"
            @close="handleClose"
            @success="handleSuccess"
          />
        </template>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useToast } from 'primevue/usetoast'
import ReservationForm from '@/components/ReservationForm.vue'

const router = useRouter()
const toast = useToast()

const handleClose = () => {
  router.push('/reservations')
}

const handleSuccess = () => {
  toast.add({
    severity: 'success',
    summary: '建立成功',
    detail: '預約已成功建立',
    life: 3000
  })
  router.push('/reservations')
}
</script>

<style scoped>
.reservation-create {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.page-header {
  background: var(--p-surface-0);
  border-bottom: 1px solid var(--p-surface-border);
  padding: 2rem;
}

.header-content h1 {
  margin: 0 0 0.5rem 0;
  color: var(--p-text-color);
  font-size: 2rem;
  font-weight: 600;
}

.header-content p {
  margin: 0;
  color: var(--p-text-color-secondary);
  font-size: 1rem;
}

.content-wrapper {
  flex: 1;
  padding: 2rem;
  overflow-y: auto;
}

@media (max-width: 768px) {
  .page-header {
    padding: 1rem;
  }
  
  .header-content h1 {
    font-size: 1.5rem;
  }
  
  .content-wrapper {
    padding: 1rem;
  }
}
</style>