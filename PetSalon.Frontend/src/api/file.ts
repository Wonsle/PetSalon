import axios from '@/utils/axios'

/**
 * 檔案上傳請求參數
 */
export interface FileUploadRequest {
  file: File
  entityType: string
  entityId: number
  attachmentType?: string
}

/**
 * 檔案附件 DTO
 */
export interface FileAttachmentDto {
  fileId: number
  originalFileName: string
  storedFileName: string
  filePath: string
  fileSize: number
  mimeType: string
  extension: string
  fileHash: string
  entityType: string
  entityId: number
  attachmentType: string
  displayOrder: number
  isActive: boolean
  createUser: string
  createTime: string
  modifyUser: string
  modifyTime: string
}

/**
 * 檔案 API 服務
 */
export const fileApi = {
  /**
   * 上傳檔案
   * @param request 上傳請求參數
   * @returns 檔案附件資訊
   */
  async uploadFile(request: FileUploadRequest): Promise<FileAttachmentDto> {
    const formData = new FormData()
    formData.append('file', request.file)
    formData.append('entityType', request.entityType)
    formData.append('entityId', request.entityId.toString())

    if (request.attachmentType) {
      formData.append('attachmentType', request.attachmentType)
    }

    const response = await axios.post('/api/file/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data
  },

  /**
   * 取得實體的所有檔案
   * @param entityType 實體類型（如：Pet, ContactPerson）
   * @param entityId 實體ID
   * @param attachmentType 附件類型（可選）
   * @returns 檔案附件列表
   */
  async getEntityFiles(
    entityType: string,
    entityId: number,
    attachmentType?: string
  ): Promise<FileAttachmentDto[]> {
    const params: Record<string, any> = {}
    if (attachmentType) {
      params.attachmentType = attachmentType
    }

    const response = await axios.get(`/api/file/${entityType}/${entityId}`, { params })
    return response.data
  },

  /**
   * 取得特定檔案資訊
   * @param fileId 檔案ID
   * @returns 檔案附件資訊
   */
  async getFile(fileId: number): Promise<FileAttachmentDto> {
    const response = await axios.get(`/api/file/${fileId}`)
    return response.data
  },

  /**
   * 刪除檔案（軟刪除）
   * @param fileId 檔案ID
   */
  async deleteFile(fileId: number): Promise<void> {
    await axios.delete(`/api/file/${fileId}`)
  },

  /**
   * 永久刪除檔案（包含實體檔案）
   * @param fileId 檔案ID
   */
  async permanentlyDeleteFile(fileId: number): Promise<void> {
    await axios.delete(`/api/file/${fileId}/permanent`)
  },

  /**
   * 更新檔案顯示順序
   * @param fileId 檔案ID
   * @param displayOrder 新的顯示順序
   */
  async updateDisplayOrder(fileId: number, displayOrder: number): Promise<void> {
    await axios.put(`/api/file/${fileId}/order`, displayOrder)
  }
}
