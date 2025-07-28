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
</style>