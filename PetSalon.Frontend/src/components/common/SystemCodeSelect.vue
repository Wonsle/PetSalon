<template>
  <el-select
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    :placeholder="placeholder"
    :filterable="filterable"
    :loading="loading"
    :disabled="disabled"
    :clearable="clearable"
    :size="size"
    class="w-full"
  >
    <el-option
      v-for="item in options"
      :key="item.code"
      :label="item.name"
      :value="item.code"
      :disabled="!item.isActive"
    />
  </el-select>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { commonApi, type SystemCode } from '@/api/common'

interface Props {
  modelValue?: string
  codeType: string
  placeholder?: string
  filterable?: boolean
  disabled?: boolean
  clearable?: boolean
  size?: 'large' | 'default' | 'small'
}

interface Emits {
  (e: 'update:modelValue', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '請選擇',
  filterable: true,
  disabled: false,
  clearable: false,
  size: 'default'
})

const emit = defineEmits<Emits>()

const loading = ref(false)
const systemCodes = ref<SystemCode[]>([])

const options = computed(() => {
  return systemCodes.value
    .filter(code => code.isActive)
    .sort((a, b) => a.sort - b.sort)
})

const loadSystemCodes = async () => {
  if (!props.codeType) return
  
  try {
    loading.value = true
    const codes = await commonApi.getSystemCodes(props.codeType)
    systemCodes.value = codes
  } catch (error) {
    ElMessage.error(`載入${props.codeType}代碼失敗`)
    console.error(`Failed to load system codes for ${props.codeType}:`, error)
  } finally {
    loading.value = false
  }
}

// Watch codeType changes
watch(() => props.codeType, () => {
  loadSystemCodes()
}, { immediate: true })

onMounted(() => {
  loadSystemCodes()
})
</script>

<style scoped>
.w-full {
  width: 100%;
}
</style>