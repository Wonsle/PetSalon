/**
 * Mock 聯絡人資料
 * 提供聯絡人的 CRUD 操作和查詢功能
 */

import type { Contact, ContactCreateRequest, ContactUpdateRequest, ContactSearchParams, ContactListResponse } from '@/types/contact'

// 聯絡人資料
const mockContacts: Contact[] = [
  {
    contactPersonId: 1,
    name: '王小明',
    nickName: '小明',
    contactNumber: '0912-345-678',
    relatedPets: []
  },
  {
    contactPersonId: 2,
    name: '李小美',
    nickName: '小美',
    contactNumber: '0923-456-789',
    relatedPets: []
  },
  {
    contactPersonId: 3,
    name: '張大同',
    contactNumber: '0934-567-890',
    relatedPets: []
  },
  {
    contactPersonId: 4,
    name: '陳雅婷',
    nickName: '雅婷',
    contactNumber: '0945-678-901',
    relatedPets: []
  },
  {
    contactPersonId: 5,
    name: '林志明',
    contactNumber: '0956-789-012',
    relatedPets: []
  },
  {
    contactPersonId: 6,
    name: '黃淑芬',
    nickName: '芬姐',
    contactNumber: '0967-890-123',
    relatedPets: []
  },
  {
    contactPersonId: 7,
    name: '吳佳蓉',
    contactNumber: '0978-901-234',
    relatedPets: []
  },
  {
    contactPersonId: 8,
    name: '劉建宏',
    nickName: '建宏',
    contactNumber: '0989-012-345',
    relatedPets: []
  },
  {
    contactPersonId: 9,
    name: '鄭美玲',
    contactNumber: '0910-123-456',
    relatedPets: []
  },
  {
    contactPersonId: 10,
    name: '謝家豪',
    nickName: '豪哥',
    contactNumber: '0921-234-567',
    relatedPets: []
  },
  {
    contactPersonId: 11,
    name: '周詩涵',
    contactNumber: '0932-345-678',
    relatedPets: []
  },
  {
    contactPersonId: 12,
    name: '蔡志強',
    contactNumber: '0943-456-789',
    relatedPets: []
  },
  {
    contactPersonId: 13,
    name: '許雅芳',
    nickName: '雅芳',
    contactNumber: '0954-567-890',
    relatedPets: []
  },
  {
    contactPersonId: 14,
    name: '楊宗憲',
    contactNumber: '0965-678-901',
    relatedPets: []
  },
  {
    contactPersonId: 15,
    name: '賴文馨',
    nickName: '文馨',
    contactNumber: '0976-789-012',
    relatedPets: []
  },
  {
    contactPersonId: 16,
    name: '徐國偉',
    contactNumber: '0987-890-123',
    relatedPets: []
  },
  {
    contactPersonId: 17,
    name: '潘怡君',
    contactNumber: '0998-901-234',
    relatedPets: []
  }
]

/**
 * 取得聯絡人列表（支援分頁和搜尋）
 */
export function getMockContacts(params?: ContactSearchParams): ContactListResponse {
  let filteredContacts = [...mockContacts]

  // 關鍵字搜尋
  if (params?.keyword) {
    const keyword = params.keyword.toLowerCase()
    filteredContacts = filteredContacts.filter(contact =>
      contact.name.toLowerCase().includes(keyword) ||
      contact.nickName?.toLowerCase().includes(keyword) ||
      contact.contactNumber.includes(keyword)
    )
  }

  // 姓名搜尋
  if (params?.name) {
    filteredContacts = filteredContacts.filter(contact =>
      contact.name.includes(params.name!)
    )
  }

  // 電話搜尋
  if (params?.contactNumber) {
    filteredContacts = filteredContacts.filter(contact =>
      contact.contactNumber.includes(params.contactNumber!)
    )
  }

  // 分頁
  const page = params?.page || 1
  const pageSize = params?.pageSize || 10
  const startIndex = (page - 1) * pageSize
  const endIndex = startIndex + pageSize

  return {
    data: filteredContacts.slice(startIndex, endIndex),
    total: filteredContacts.length,
    page,
    pageSize
  }
}

/**
 * 根據 ID 取得單一聯絡人
 */
export function getMockContactById(id: number): Contact | undefined {
  return mockContacts.find(contact => contact.contactPersonId === id)
}

/**
 * 新增聯絡人
 */
export function createMockContact(contact: ContactCreateRequest): Contact {
  const maxId = Math.max(...mockContacts.map(c => c.contactPersonId), 0)
  const newContact: Contact = {
    contactPersonId: maxId + 1,
    ...contact,
    relatedPets: []
  }
  mockContacts.push(newContact)
  return newContact
}

/**
 * 更新聯絡人
 */
export function updateMockContact(id: number, contact: ContactUpdateRequest): Contact | null {
  const index = mockContacts.findIndex(c => c.contactPersonId === id)
  if (index === -1) return null

  const existingContact = mockContacts[index]
  mockContacts[index] = {
    ...existingContact,
    name: contact.name,
    nickName: contact.nickName,
    contactNumber: contact.contactNumber
  }
  return mockContacts[index]
}

/**
 * 刪除聯絡人
 */
export function deleteMockContact(id: number): boolean {
  const index = mockContacts.findIndex(c => c.contactPersonId === id)
  if (index === -1) return false

  mockContacts.splice(index, 1)
  return true
}

/**
 * 更新聯絡人的關聯寵物資訊
 * （這個函數會被 pets.ts 調用）
 */
export function updateContactRelatedPets(contactId: number, relatedPets: Contact['relatedPets']): void {
  const contact = mockContacts.find(c => c.contactPersonId === contactId)
  if (contact) {
    contact.relatedPets = relatedPets
  }
}

/**
 * 取得所有聯絡人（不分頁）
 */
export function getAllMockContacts(): Contact[] {
  return [...mockContacts]
}
