/**
 * Mock 系統代碼資料
 * 提供各種系統設定代碼，包含品種、性別、服務類型、預約狀態等
 */

export interface SystemCode {
  id: number
  codeType: string
  code: string
  name: string
  displayName: string
  sort?: number
}

// 系統代碼資料
const mockSystemCodes: SystemCode[] = [
  // 品種代碼 (Breed)
  { id: 1, codeType: 'Breed', code: 'POODLE', name: '貴賓犬', displayName: '貴賓犬', sort: 1 },
  { id: 2, codeType: 'Breed', code: 'GOLDEN_RETRIEVER', name: '黃金獵犬', displayName: '黃金獵犬', sort: 2 },
  { id: 3, codeType: 'Breed', code: 'SHIBA_INU', name: '柴犬', displayName: '柴犬', sort: 3 },
  { id: 4, codeType: 'Breed', code: 'FRENCH_BULLDOG', name: '法國鬥牛犬', displayName: '法國鬥牛犬', sort: 4 },
  { id: 5, codeType: 'Breed', code: 'POMERANIAN', name: '博美犬', displayName: '博美犬', sort: 5 },
  { id: 6, codeType: 'Breed', code: 'CORGI', name: '柯基犬', displayName: '柯基犬', sort: 6 },
  { id: 7, codeType: 'Breed', code: 'HUSKY', name: '哈士奇', displayName: '哈士奇', sort: 7 },
  { id: 8, codeType: 'Breed', code: 'MALTESE', name: '瑪爾濟斯', displayName: '瑪爾濟斯', sort: 8 },
  { id: 9, codeType: 'Breed', code: 'CHIHUAHUA', name: '吉娃娃', displayName: '吉娃娃', sort: 9 },
  { id: 10, codeType: 'Breed', code: 'SCHNAUZER', name: '雪納瑞', displayName: '雪納瑞', sort: 10 },

  // 性別代碼 (Gender)
  { id: 11, codeType: 'Gender', code: 'MALE', name: '公', displayName: '公', sort: 1 },
  { id: 12, codeType: 'Gender', code: 'FEMALE', name: '母', displayName: '母', sort: 2 },

  // 服務類型 (ServiceType)
  { id: 13, codeType: 'ServiceType', code: 'BATH', name: '洗澡', displayName: '洗澡', sort: 1 },
  { id: 14, codeType: 'ServiceType', code: 'GROOMING', name: '美容', displayName: '美容', sort: 2 },
  { id: 15, codeType: 'ServiceType', code: 'NAIL_TRIM', name: '指甲修剪', displayName: '指甲修剪', sort: 3 },
  { id: 16, codeType: 'ServiceType', code: 'STYLING', name: '造型', displayName: '造型', sort: 4 },
  { id: 17, codeType: 'ServiceType', code: 'SPA', name: 'SPA護理', displayName: 'SPA護理', sort: 5 },
  { id: 18, codeType: 'ServiceType', code: 'MONTHLY_PACKAGE', name: '包月方案', displayName: '包月方案', sort: 6 },

  // 預約狀態 (ReservationStatus)
  { id: 19, codeType: 'ReservationStatus', code: 'PENDING', name: '待確認', displayName: '待確認', sort: 1 },
  { id: 20, codeType: 'ReservationStatus', code: 'CONFIRMED', name: '已確認', displayName: '已確認', sort: 2 },
  { id: 21, codeType: 'ReservationStatus', code: 'IN_PROGRESS', name: '進行中', displayName: '進行中', sort: 3 },
  { id: 22, codeType: 'ReservationStatus', code: 'COMPLETED', name: '已完成', displayName: '已完成', sort: 4 },
  { id: 23, codeType: 'ReservationStatus', code: 'CANCELLED', name: '已取消', displayName: '已取消', sort: 5 },
  { id: 24, codeType: 'ReservationStatus', code: 'NO_SHOW', name: '未出席', displayName: '未出席', sort: 6 },

  // 關係類型 (Relationship)
  { id: 25, codeType: 'Relationship', code: 'OWNER', name: '飼主', displayName: '飼主', sort: 1 },
  { id: 26, codeType: 'Relationship', code: 'FATHER', name: '父親', displayName: '父親', sort: 2 },
  { id: 27, codeType: 'Relationship', code: 'MOTHER', name: '母親', displayName: '母親', sort: 3 },
  { id: 28, codeType: 'Relationship', code: 'BROTHER', name: '兄弟', displayName: '兄弟', sort: 4 },
  { id: 29, codeType: 'Relationship', code: 'SISTER', name: '姐妹', displayName: '姐妹', sort: 5 },
  { id: 30, codeType: 'Relationship', code: 'FAMILY', name: '家人', displayName: '家人', sort: 6 },
  { id: 31, codeType: 'Relationship', code: 'FRIEND', name: '朋友', displayName: '朋友', sort: 7 },
  { id: 32, codeType: 'Relationship', code: 'CAREGIVER', name: '照護者', displayName: '照護者', sort: 8 },

  // 付款方式 (PaymentType)
  { id: 33, codeType: 'PaymentType', code: 'CASH', name: '現金', displayName: '現金', sort: 1 },
  { id: 34, codeType: 'PaymentType', code: 'CREDIT_CARD', name: '信用卡', displayName: '信用卡', sort: 2 },
  { id: 35, codeType: 'PaymentType', code: 'BANK_TRANSFER', name: '轉帳', displayName: '轉帳', sort: 3 },
  { id: 36, codeType: 'PaymentType', code: 'LINE_PAY', name: 'LINE Pay', displayName: 'LINE Pay', sort: 4 },
]

/**
 * 取得指定類型的所有系統代碼
 */
export function getSystemCodesByType(codeType: string): SystemCode[] {
  return mockSystemCodes
    .filter(code => code.codeType === codeType)
    .sort((a, b) => (a.sort || 0) - (b.sort || 0))
}

/**
 * 取得特定系統代碼
 */
export function getSystemCode(codeType: string, code: string): SystemCode | undefined {
  return mockSystemCodes.find(sc => sc.codeType === codeType && sc.code === code)
}

/**
 * 取得所有系統代碼類型
 */
export function getAllSystemCodeTypes(): string[] {
  const types = new Set<string>()
  mockSystemCodes.forEach(code => types.add(code.codeType))
  return Array.from(types).sort()
}

/**
 * 根據代碼取得顯示名稱
 */
export function getSystemCodeName(codeType: string, code: string): string {
  const systemCode = getSystemCode(codeType, code)
  return systemCode?.displayName || code
}

/**
 * 取得所有系統代碼
 */
export function getAllSystemCodes(): SystemCode[] {
  return [...mockSystemCodes]
}

/**
 * 新增系統代碼
 */
export function addSystemCode(systemCode: Omit<SystemCode, 'id'>): SystemCode {
  const maxId = Math.max(...mockSystemCodes.map(c => c.id), 0)
  const newCode: SystemCode = {
    id: maxId + 1,
    ...systemCode
  }
  mockSystemCodes.push(newCode)
  return newCode
}

/**
 * 更新系統代碼
 */
export function updateSystemCode(id: number, updates: Partial<SystemCode>): SystemCode | null {
  const index = mockSystemCodes.findIndex(c => c.id === id)
  if (index === -1) return null

  mockSystemCodes[index] = { ...mockSystemCodes[index], ...updates }
  return mockSystemCodes[index]
}

/**
 * 刪除系統代碼
 */
export function deleteSystemCode(id: number): boolean {
  const index = mockSystemCodes.findIndex(c => c.id === id)
  if (index === -1) return false

  mockSystemCodes.splice(index, 1)
  return true
}
