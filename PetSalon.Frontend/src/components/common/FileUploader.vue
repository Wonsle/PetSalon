<template>
  <div class="file-uploader">
    <!-- 照片預覽區域 -->
    <div v-if="fileUrl" class="preview-container">
      <Image
        :src="fileUrl"
        alt="檔案預覽"
        width="120"
        height="120"
        preview
        class="preview-image"
      />
      <div class="actions">
        <Button
          label="更換"
          size="small"
          @click="triggerFileInput"
          :disabled="uploading"
        />
        <Button
          label="刪除"
          severity="danger"
          size="small"
          @click="handleDelete"
          :disabled="uploading"
        />
      </div>
    </div>

    <!-- 上傳按鈕區域 -->
    <div v-else class="upload-area">
      <Button
        :label="label"
        icon="pi pi-upload"
        @click="triggerFileInput"
        :disabled="uploading"
        :loading="uploading"
      />
    </div>

    <!-- 說明文字 -->
    <small v-if="displayHelpText" class="help-text">{{ displayHelpText }}</small>

    <!-- 隱藏的檔案輸入元件 -->
    <input
      ref="fileInputRef"
      type="file"
      :accept="acceptString"
      style="display: none"
      @change="handleFileChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useToast } from 'primevue/usetoast'
import { fileApi, type FileAttachmentDto } from '@/api/file'

interface Props {
  // 允許的副檔名陣列，例如：['jpg', 'png', 'gif']
  allowedExtensions: string[]
  // 最大檔案大小（MB）
  maxSizeInMB?: number
  // 實體類型
  entityType: string
  // 實體ID
  entityId: number
  // 附件類型
  attachmentType?: string
  // 按鈕文字
  label?: string
  // 說明文字
  helpText?: string
  // 現有檔案 URL
  modelValue?: string
}

const props = withDefaults(defineProps<Props>(), {
  maxSizeInMB: 5,
  attachmentType: 'Photo',
  label: '選擇檔案',
  helpText: ''
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'success', file: FileAttachmentDto): void
  (e: 'error', error: string): void
  (e: 'delete'): void
}>()

const toast = useToast()
const fileInputRef = ref<HTMLInputElement>()
const uploading = ref(false)
const fileUrl = ref(props.modelValue || '')
const currentFileId = ref<number | null>(null)

// 監聽 modelValue 變化
watch(() => props.modelValue, (newValue) => {
  fileUrl.value = newValue || ''
})

// 計算 accept 屬性字串
const acceptString = computed(() => {
  // 將 ['jpg', 'png'] 轉換為 '.jpg,.png'
  return props.allowedExtensions
    .map(ext => (ext.startsWith('.') ? ext : `.${ext}`))
    .join(',')
})

// 產生說明文字
const displayHelpText = computed(() => {
  if (props.helpText) return props.helpText
  const exts = props.allowedExtensions
    .map(ext => ext.replace('.', '').toUpperCase())
    .join(', ')
  return `支援 ${exts} 格式，大小不超過 ${props.maxSizeInMB}MB`
})

// 觸發檔案選擇
const triggerFileInput = () => {
  fileInputRef.value?.click()
}

// 驗證檔案
const validateFile = (file: File): boolean => {
  // 1. 檢查副檔名
  const fileName = file.name.toLowerCase()
  const isValidExt = props.allowedExtensions.some(ext => {
    const extension = ext.startsWith('.') ? ext : `.${ext}`
    return fileName.endsWith(extension.toLowerCase())
  })

  if (!isValidExt) {
    const exts = props.allowedExtensions
      .map(ext => ext.replace('.', '').toUpperCase())
      .join(', ')
    toast.add({
      severity: 'error',
      summary: '檔案格式錯誤',
      detail: `只允許 ${exts} 格式`,
      life: 3000
    })
    return false
  }

  // 2. 檢查檔案大小
  const maxBytes = props.maxSizeInMB * 1024 * 1024
  if (file.size > maxBytes) {
    toast.add({
      severity: 'error',
      summary: '檔案太大',
      detail: `檔案大小不能超過 ${props.maxSizeInMB}MB`,
      life: 3000
    })
    return false
  }

  return true
}

// 處理檔案選擇
const handleFileChange = async (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]

  if (!file) return

  // 前端驗證
  if (!validateFile(file)) {
    target.value = ''
    return
  }

  try {
    uploading.value = true

    // 上傳到後端
    const result = await fileApi.uploadFile({
      file,
      entityType: props.entityType,
      entityId: props.entityId,
      attachmentType: props.attachmentType
    })

    fileUrl.value = result.filePath
    currentFileId.value = result.fileId
    emit('update:modelValue', result.filePath)
    emit('success', result)

    toast.add({
      severity: 'success',
      summary: '上傳成功',
      detail: '檔案已成功上傳',
      life: 3000
    })
  } catch (error: any) {
    console.error('上傳失敗:', error)
    emit('error', error.message || '上傳失敗')
    toast.add({
      severity: 'error',
      summary: '上傳失敗',
      detail: error.response?.data?.message || error.message || '上傳失敗',
      life: 3000
    })
  } finally {
    uploading.value = false
    target.value = ''
  }
}

// 處理刪除
const handleDelete = async () => {
  try {
    if (currentFileId.value) {
      await fileApi.deleteFile(currentFileId.value)
    }

    fileUrl.value = ''
    currentFileId.value = null
    emit('update:modelValue', '')
    emit('delete')

    toast.add({
      severity: 'success',
      summary: '刪除成功',
      detail: '檔案已刪除',
      life: 3000
    })
  } catch (error: any) {
    console.error('刪除失敗:', error)
    toast.add({
      severity: 'error',
      summary: '刪除失敗',
      detail: error.message || '刪除失敗',
      life: 3000
    })
  }
}
</script>

<style scoped>
.file-uploader {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.preview-container {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.preview-image {
  border-radius: var(--p-border-radius);
  border: 1px solid var(--p-content-border-color);
  object-fit: cover;
}

.upload-area {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.actions {
  display: flex;
  gap: 0.5rem;
}

.help-text {
  color: var(--p-text-muted-color);
  font-size: 0.75rem;
  margin-top: 0.25rem;
}
</style>
