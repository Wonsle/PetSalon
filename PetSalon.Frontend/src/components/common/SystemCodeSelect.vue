<template>
  <Select
    :modelValue="modelValue"
    @update:modelValue="handleChange"
    :options="options"
    :placeholder="placeholder"
    :filter="filterable"
    :loading="loading"
    :disabled="disabled"
    :showClear="clearable"
    :size="size"
    optionLabel="name"
    optionValue="code"
    class="w-full"
  />
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useToast } from 'primevue/usetoast'
import { commonApi, type SystemCode } from '@/api/common'

interface Props {
  modelValue?: string
  codeType: string
  placeholder?: string
  filterable?: boolean
  disabled?: boolean
  clearable?: boolean
  size?: 'small' | 'large'
}

interface Emits {
  (e: 'update:modelValue', value: string): void
  (e: 'change', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '請選擇',
  filterable: true,
  disabled: false,
  clearable: false,
  size: undefined
})

const emit = defineEmits<Emits>()
const toast = useToast()

const loading = ref(false)
const systemCodes = ref<SystemCode[]>([])

const options = computed(() => {
  return systemCodes.value
    .filter(code => code.isActive)
    .sort((a, b) => a.sort - b.sort)
    .map(code => ({
      name: code.name,
      code: code.code,
      disabled: !code.isActive
    }))
})

const loadSystemCodes = async () => {
  if (!props.codeType) return

  try {
    loading.value = true
    const codes = await commonApi.getSystemCodes(props.codeType)
    systemCodes.value = codes
  } catch (error) {
    toast.add({
      severity: 'error',
      summary: '載入失敗',
      detail: `載入${props.codeType}代碼失敗`,
      life: 3000
    })
    console.error(`Failed to load system codes for ${props.codeType}:`, error)
  } finally {
    loading.value = false
  }
}

// Watch codeType changes
watch(() => props.codeType, () => {
  loadSystemCodes()
}, { immediate: true })

const handleChange = (value: string) => {
  emit('update:modelValue', value)
  emit('change', value)
}

onMounted(() => {
  loadSystemCodes()
})
</script>

<style scoped>
.w-full {
  width: 100%;
}

/* 確保下拉選單選項能完整顯示長文字 */
:deep(.p-select-overlay) {
  /* 設定最小寬度與觸發元素相同 */
  min-width: max-content;
}

:deep(.p-select-option) {
  /* 允許選項文字換行 */
  white-space: normal;
  word-wrap: break-word;
  line-height: 1.5;
  padding: 0.75rem 1rem;
}

:deep(.p-select-option-label) {
  /* 確保選項標籤文字能完整顯示 */
  white-space: normal;
  word-wrap: break-word;
  overflow-wrap: break-word;
}

/* 已選中的項目顯示樣式 */
:deep(.p-select-label) {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* 調整下拉選單面板寬度，確保能容納長文字 */
:deep(.p-select-overlay .p-select-list) {
  max-width: 400px;
  min-width: 200px;
}

/* 行動裝置優化 */
@media (max-width: 576px) {
  :deep(.p-select-overlay .p-select-list) {
    max-width: calc(100vw - 2rem);
  }
}
</style>