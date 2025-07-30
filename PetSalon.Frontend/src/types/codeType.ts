/**
 * 代碼類型相關的 TypeScript 類型定義
 */

export interface CodeType {
  /** 代碼類型唯一識別碼 */
  id: number;
  /** 代碼類型代碼 */
  codeType: string;
  /** 代碼類型名稱 */
  name: string;
  /** 描述說明 */
  description?: string;
  /** 建立者 */
  createUser?: string;
  /** 建立時間 */
  createTime?: string;
  /** 修改者 */
  modifyUser?: string;
  /** 修改時間 */
  modifyTime?: string;
  /** 完整顯示名稱 */
  displayName?: string;
}

export interface CreateOrUpdateCodeTypeDto {
  /** 代碼類型代碼 */
  codeType: string;
  /** 代碼類型名稱 */
  name: string;
  /** 描述說明 */
  description?: string;
}

export interface CodeTypeSearchParams {
  /** 搜尋關鍵字 */
  keyword?: string;
  /** 頁碼 */
  page?: number;
  /** 每頁數量 */
  pageSize?: number;
}

export interface CodeTypeListResponse {
  /** 代碼類型列表 */
  items: CodeType[];
  /** 總數量 */
  total: number;
  /** 當前頁碼 */
  currentPage: number;
  /** 每頁數量 */
  pageSize: number;
  /** 總頁數 */
  totalPages: number;
}

export interface CodeTypeExistsResponse {
  /** 是否存在 */
  exists: boolean;
}

export interface ApiResponse<T = any> {
  /** 是否成功 */
  success: boolean;
  /** 回應資料 */
  data?: T;
  /** 錯誤訊息 */
  message?: string;
  /** 錯誤詳細資訊 */
  detail?: string;
}
