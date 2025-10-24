/**
 * 資源 URL 處理工具
 * 用於將後端返回的相對路徑轉換為完整的資源 URL
 */

/**
 * 將相對路徑轉換為完整的資源 URL
 *
 * @param path - 資源路徑（可能是相對路徑或完整 URL）
 * @returns 完整的資源 URL
 *
 * @example
 * // 相對路徑
 * getResourceUrl('/uploads/pet/abc.jpeg')
 * // => 'http://localhost:5150/uploads/pet/abc.jpeg' (開發環境)
 *
 * @example
 * // 已經是完整 URL
 * getResourceUrl('https://cdn.example.com/image.jpg')
 * // => 'https://cdn.example.com/image.jpg'
 *
 * @example
 * // 空值處理
 * getResourceUrl(null) // => ''
 */
export function getResourceUrl(path: string | null | undefined): string {
  if (!path) return ''

  // 已經是完整 URL（http:// 或 https://），直接返回
  if (path.startsWith('http://') || path.startsWith('https://')) {
    return path
  }

  // 從環境變數獲取 API base URL
  const baseURL = import.meta.env.VITE_API_BASE_URL || ''

  // 確保路徑以 / 開頭
  const normalizedPath = path.startsWith('/') ? path : `/${path}`

  // 拼接完整 URL
  return `${baseURL}${normalizedPath}`
}

/**
 * 批量轉換資源 URL（用於列表場景）
 *
 * @param paths - 資源路徑陣列
 * @returns 完整的資源 URL 陣列（過濾掉空值）
 *
 * @example
 * getResourceUrls(['/uploads/1.jpg', null, '/uploads/2.jpg'])
 * // => ['http://localhost:5150/uploads/1.jpg', 'http://localhost:5150/uploads/2.jpg']
 */
export function getResourceUrls(paths: (string | null | undefined)[]): string[] {
  return paths.filter(Boolean).map(path => getResourceUrl(path!))
}

/**
 * 檢查是否為有效的資源 URL
 *
 * @param path - 要檢查的路徑
 * @returns 是否為有效的資源路徑
 */
export function isValidResourceUrl(path: string | null | undefined): boolean {
  if (!path) return false
  return path.startsWith('http://') || path.startsWith('https://') || path.startsWith('/')
}
