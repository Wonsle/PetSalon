/**
 * Mock 寵物資料
 * 提供寵物的 CRUD 操作和查詢功能
 */

import type { Pet, PetCreateRequest, PetUpdateRequest, PetSearchParams, PetListResponse } from '@/types/pet'
import { getSystemCodeName } from './systemCodes'
import { updateContactRelatedPets, getMockContactById } from './contacts'

// 寵物資料
const mockPets: Pet[] = [
  {
    petId: 1,
    petName: '小白',
    breed: 'POODLE',
    gender: 'MALE',
    birthDay: '2020-01-15',
    normalPrice: 800,
    subscriptionPrice: 600,
    photoUrl: '/uploads/pets/pet1.jpg',
    createUser: 'admin',
    createTime: '2024-01-01T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-01-01T00:00:00',
    primaryContact: {
      contactPersonId: 1,
      name: '王小明',
      phone: '0912-345-678',
      relationship: 'OWNER'
    }
  },
  {
    petId: 2,
    petName: '球球',
    breed: 'GOLDEN_RETRIEVER',
    gender: 'FEMALE',
    birthDay: '2019-05-20',
    normalPrice: 1200,
    subscriptionPrice: 1000,
    photoUrl: '/uploads/pets/pet2.jpg',
    createUser: 'admin',
    createTime: '2024-01-05T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-01-05T00:00:00',
    primaryContact: {
      contactPersonId: 2,
      name: '李小美',
      phone: '0923-456-789',
      relationship: 'OWNER'
    }
  },
  {
    petId: 3,
    petName: '柴柴',
    breed: 'SHIBA_INU',
    gender: 'MALE',
    birthDay: '2021-03-10',
    normalPrice: 900,
    subscriptionPrice: 700,
    photoUrl: '/uploads/pets/pet3.jpg',
    createUser: 'admin',
    createTime: '2024-01-10T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-01-10T00:00:00',
    primaryContact: {
      contactPersonId: 3,
      name: '張大同',
      phone: '0934-567-890',
      relationship: 'OWNER'
    }
  },
  {
    petId: 4,
    petName: '布丁',
    breed: 'FRENCH_BULLDOG',
    gender: 'MALE',
    birthDay: '2020-08-25',
    normalPrice: 1000,
    subscriptionPrice: 800,
    photoUrl: '/uploads/pets/pet4.jpg',
    createUser: 'admin',
    createTime: '2024-01-15T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-01-15T00:00:00',
    primaryContact: {
      contactPersonId: 4,
      name: '陳雅婷',
      phone: '0945-678-901',
      relationship: 'OWNER'
    }
  },
  {
    petId: 5,
    petName: '妞妞',
    breed: 'POMERANIAN',
    gender: 'FEMALE',
    birthDay: '2022-02-14',
    normalPrice: 850,
    subscriptionPrice: 650,
    photoUrl: '/uploads/pets/pet5.jpg',
    createUser: 'admin',
    createTime: '2024-01-20T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-01-20T00:00:00',
    primaryContact: {
      contactPersonId: 5,
      name: '林志明',
      phone: '0956-789-012',
      relationship: 'OWNER'
    }
  },
  {
    petId: 6,
    petName: '豆豆',
    breed: 'CORGI',
    gender: 'MALE',
    birthDay: '2019-11-30',
    normalPrice: 950,
    subscriptionPrice: 750,
    photoUrl: '/uploads/pets/pet6.jpg',
    createUser: 'admin',
    createTime: '2024-02-01T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-02-01T00:00:00',
    primaryContact: {
      contactPersonId: 6,
      name: '黃淑芬',
      phone: '0967-890-123',
      relationship: 'OWNER'
    }
  },
  {
    petId: 7,
    petName: '哈利',
    breed: 'HUSKY',
    gender: 'MALE',
    birthDay: '2018-07-08',
    normalPrice: 1500,
    subscriptionPrice: 1200,
    photoUrl: '/uploads/pets/pet7.jpg',
    createUser: 'admin',
    createTime: '2024-02-05T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-02-05T00:00:00',
    primaryContact: {
      contactPersonId: 7,
      name: '吳佳蓉',
      phone: '0978-901-234',
      relationship: 'OWNER'
    }
  },
  {
    petId: 8,
    petName: '雪莉',
    breed: 'MALTESE',
    gender: 'FEMALE',
    birthDay: '2021-06-12',
    normalPrice: 800,
    subscriptionPrice: 600,
    photoUrl: '/uploads/pets/pet8.jpg',
    createUser: 'admin',
    createTime: '2024-02-10T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-02-10T00:00:00',
    primaryContact: {
      contactPersonId: 8,
      name: '劉建宏',
      phone: '0989-012-345',
      relationship: 'OWNER'
    }
  },
  {
    petId: 9,
    petName: '皮皮',
    breed: 'CHIHUAHUA',
    gender: 'FEMALE',
    birthDay: '2022-04-20',
    normalPrice: 700,
    subscriptionPrice: 550,
    photoUrl: '/uploads/pets/pet9.jpg',
    createUser: 'admin',
    createTime: '2024-02-15T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-02-15T00:00:00',
    primaryContact: {
      contactPersonId: 9,
      name: '鄭美玲',
      phone: '0910-123-456',
      relationship: 'OWNER'
    }
  },
  {
    petId: 10,
    petName: '麥克',
    breed: 'SCHNAUZER',
    gender: 'MALE',
    birthDay: '2020-09-15',
    normalPrice: 900,
    subscriptionPrice: 700,
    photoUrl: '/uploads/pets/pet10.jpg',
    createUser: 'admin',
    createTime: '2024-02-20T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-02-20T00:00:00',
    primaryContact: {
      contactPersonId: 10,
      name: '謝家豪',
      phone: '0921-234-567',
      relationship: 'OWNER'
    }
  },
  {
    petId: 11,
    petName: '可可',
    breed: 'POODLE',
    gender: 'FEMALE',
    birthDay: '2021-01-25',
    normalPrice: 850,
    subscriptionPrice: 650,
    photoUrl: '/uploads/pets/pet11.jpg',
    createUser: 'admin',
    createTime: '2024-03-01T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-03-01T00:00:00',
    primaryContact: {
      contactPersonId: 11,
      name: '周詩涵',
      phone: '0932-345-678',
      relationship: 'OWNER'
    }
  },
  {
    petId: 12,
    petName: '旺財',
    breed: 'GOLDEN_RETRIEVER',
    gender: 'MALE',
    birthDay: '2019-12-05',
    normalPrice: 1200,
    subscriptionPrice: 1000,
    photoUrl: '/uploads/pets/pet12.jpg',
    createUser: 'admin',
    createTime: '2024-03-05T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-03-05T00:00:00',
    primaryContact: {
      contactPersonId: 12,
      name: '蔡志強',
      phone: '0943-456-789',
      relationship: 'OWNER'
    }
  },
  {
    petId: 13,
    petName: 'lucky',
    breed: 'SHIBA_INU',
    gender: 'MALE',
    birthDay: '2020-10-18',
    normalPrice: 900,
    subscriptionPrice: 700,
    photoUrl: '/uploads/pets/pet13.jpg',
    createUser: 'admin',
    createTime: '2024-03-10T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-03-10T00:00:00',
    primaryContact: {
      contactPersonId: 13,
      name: '許雅芳',
      phone: '0954-567-890',
      relationship: 'OWNER'
    }
  },
  {
    petId: 14,
    petName: '甜甜',
    breed: 'POMERANIAN',
    gender: 'FEMALE',
    birthDay: '2021-07-22',
    normalPrice: 850,
    subscriptionPrice: 650,
    photoUrl: '/uploads/pets/pet14.jpg',
    createUser: 'admin',
    createTime: '2024-03-15T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-03-15T00:00:00',
    primaryContact: {
      contactPersonId: 14,
      name: '楊宗憲',
      phone: '0965-678-901',
      relationship: 'OWNER'
    }
  },
  {
    petId: 15,
    petName: '虎斑',
    breed: 'CORGI',
    gender: 'MALE',
    birthDay: '2020-05-14',
    normalPrice: 950,
    subscriptionPrice: 750,
    photoUrl: '/uploads/pets/pet15.jpg',
    createUser: 'admin',
    createTime: '2024-03-20T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-03-20T00:00:00',
    primaryContact: {
      contactPersonId: 15,
      name: '賴文馨',
      phone: '0976-789-012',
      relationship: 'OWNER'
    }
  },
  {
    petId: 16,
    petName: '小虎',
    breed: 'HUSKY',
    gender: 'MALE',
    birthDay: '2019-03-28',
    normalPrice: 1500,
    subscriptionPrice: 1200,
    photoUrl: '/uploads/pets/pet16.jpg',
    createUser: 'admin',
    createTime: '2024-04-01T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-04-01T00:00:00',
    primaryContact: {
      contactPersonId: 16,
      name: '徐國偉',
      phone: '0987-890-123',
      relationship: 'OWNER'
    }
  },
  {
    petId: 17,
    petName: '米糕',
    breed: 'MALTESE',
    gender: 'MALE',
    birthDay: '2021-11-08',
    normalPrice: 800,
    subscriptionPrice: 600,
    photoUrl: '/uploads/pets/pet17.jpg',
    createUser: 'admin',
    createTime: '2024-04-05T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-04-05T00:00:00',
    primaryContact: {
      contactPersonId: 17,
      name: '潘怡君',
      phone: '0998-901-234',
      relationship: 'OWNER'
    }
  },
  {
    petId: 18,
    petName: 'Nana',
    breed: 'CHIHUAHUA',
    gender: 'FEMALE',
    birthDay: '2022-01-30',
    normalPrice: 700,
    subscriptionPrice: 550,
    photoUrl: '/uploads/pets/pet18.jpg',
    createUser: 'admin',
    createTime: '2024-04-10T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-04-10T00:00:00',
    primaryContact: {
      contactPersonId: 1,
      name: '王小明',
      phone: '0912-345-678',
      relationship: 'OWNER'
    }
  },
  {
    petId: 19,
    petName: '威利',
    breed: 'SCHNAUZER',
    gender: 'MALE',
    birthDay: '2020-06-16',
    normalPrice: 900,
    subscriptionPrice: 700,
    photoUrl: '/uploads/pets/pet19.jpg',
    createUser: 'admin',
    createTime: '2024-04-15T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-04-15T00:00:00',
    primaryContact: {
      contactPersonId: 2,
      name: '李小美',
      phone: '0923-456-789',
      relationship: 'OWNER'
    }
  },
  {
    petId: 20,
    petName: '小乖',
    breed: 'FRENCH_BULLDOG',
    gender: 'FEMALE',
    birthDay: '2021-09-12',
    normalPrice: 1000,
    subscriptionPrice: 800,
    photoUrl: '/uploads/pets/pet20.jpg',
    createUser: 'admin',
    createTime: '2024-04-20T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-04-20T00:00:00',
    primaryContact: {
      contactPersonId: 3,
      name: '張大同',
      phone: '0934-567-890',
      relationship: 'OWNER'
    }
  },
  {
    petId: 21,
    petName: '樂樂',
    breed: 'POODLE',
    gender: 'MALE',
    birthDay: '2022-05-08',
    normalPrice: 800,
    subscriptionPrice: 600,
    photoUrl: '/uploads/pets/pet21.jpg',
    createUser: 'admin',
    createTime: '2024-05-01T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-05-01T00:00:00',
    primaryContact: {
      contactPersonId: 4,
      name: '陳雅婷',
      phone: '0945-678-901',
      relationship: 'OWNER'
    }
  },
  {
    petId: 22,
    petName: 'Cookie',
    breed: 'CORGI',
    gender: 'FEMALE',
    birthDay: '2019-08-20',
    normalPrice: 950,
    subscriptionPrice: 750,
    photoUrl: '/uploads/pets/pet22.jpg',
    createUser: 'admin',
    createTime: '2024-05-05T00:00:00',
    modifyUser: 'admin',
    modifyTime: '2024-05-05T00:00:00',
    primaryContact: {
      contactPersonId: 5,
      name: '林志明',
      phone: '0956-789-012',
      relationship: 'OWNER'
    }
  }
]

// 初始化聯絡人的關聯寵物資訊
function initializeContactRelations(): void {
  mockPets.forEach(pet => {
    if (pet.primaryContact) {
      const contact = getMockContactById(pet.primaryContact.contactPersonId)
      if (contact && !contact.relatedPets) {
        contact.relatedPets = []
      }
      if (contact && contact.relatedPets) {
        const existingRelation = contact.relatedPets.find(r => r.petId === pet.petId)
        if (!existingRelation) {
          contact.relatedPets.push({
            petRelationId: pet.petId,
            petId: pet.petId,
            petName: pet.petName,
            breed: pet.breed,
            gender: pet.gender,
            relationshipType: pet.primaryContact.relationship,
            relationshipTypeName: getSystemCodeName('Relationship', pet.primaryContact.relationship),
            sort: 1
          })
        }
      }
    }
  })
}

// 執行初始化
initializeContactRelations()

/**
 * 取得寵物列表（支援分頁和搜尋）
 */
export function getMockPets(params?: PetSearchParams): PetListResponse {
  let filteredPets = [...mockPets]

  // 關鍵字搜尋
  if (params?.keyword) {
    const keyword = params.keyword.toLowerCase()
    filteredPets = filteredPets.filter(pet =>
      pet.petName.toLowerCase().includes(keyword) ||
      pet.primaryContact?.name.toLowerCase().includes(keyword) ||
      pet.primaryContact?.phone.includes(keyword)
    )
  }

  // 品種搜尋
  if (params?.breed) {
    filteredPets = filteredPets.filter(pet => pet.breed === params.breed)
  }

  // 性別搜尋
  if (params?.gender) {
    filteredPets = filteredPets.filter(pet => pet.gender === params.gender)
  }

  // 飼主搜尋
  if (params?.ownerId) {
    filteredPets = filteredPets.filter(pet => pet.primaryContact?.contactPersonId === params.ownerId)
  }

  // 分頁
  const page = params?.page || 1
  const pageSize = params?.pageSize || 10
  const startIndex = (page - 1) * pageSize
  const endIndex = startIndex + pageSize

  // 添加別名屬性
  const petsWithAliases = filteredPets.map(pet => ({
    ...pet,
    id: pet.petId,
    name: pet.petName,
    breedName: getSystemCodeName('Breed', pet.breed),
    ownerName: pet.primaryContact?.name,
    contactPhone: pet.primaryContact?.phone
  }))

  return {
    data: petsWithAliases.slice(startIndex, endIndex),
    total: filteredPets.length,
    page,
    pageSize
  }
}

/**
 * 根據 ID 取得單一寵物
 */
export function getMockPetById(id: number): Pet | undefined {
  const pet = mockPets.find(pet => pet.petId === id)
  if (!pet) return undefined

  return {
    ...pet,
    id: pet.petId,
    name: pet.petName,
    breedName: getSystemCodeName('Breed', pet.breed),
    ownerName: pet.primaryContact?.name,
    contactPhone: pet.primaryContact?.phone
  }
}

/**
 * 根據聯絡人 ID 取得寵物列表
 */
export function getMockPetsByContactId(contactId: number): Pet[] {
  return mockPets
    .filter(pet => pet.primaryContact?.contactPersonId === contactId)
    .map(pet => ({
      ...pet,
      id: pet.petId,
      name: pet.petName,
      breedName: getSystemCodeName('Breed', pet.breed),
      ownerName: pet.primaryContact?.name,
      contactPhone: pet.primaryContact?.phone
    }))
}

/**
 * 新增寵物
 */
export function createMockPet(pet: PetCreateRequest): Pet {
  const maxId = Math.max(...mockPets.map(p => p.petId), 0)
  const now = new Date().toISOString()

  const newPet: Pet = {
    petId: maxId + 1,
    petName: pet.petName,
    breed: pet.breed,
    gender: pet.gender,
    birthDay: pet.birthDay ? new Date(pet.birthDay).toISOString().split('T')[0] : undefined,
    normalPrice: pet.normalPrice,
    subscriptionPrice: pet.subscriptionPrice,
    createUser: 'admin',
    createTime: now,
    modifyUser: 'admin',
    modifyTime: now
  }

  mockPets.push(newPet)
  return {
    ...newPet,
    id: newPet.petId,
    name: newPet.petName,
    breedName: getSystemCodeName('Breed', newPet.breed)
  }
}

/**
 * 更新寵物
 */
export function updateMockPet(id: number, pet: PetUpdateRequest): Pet | null {
  const index = mockPets.findIndex(p => p.petId === id)
  if (index === -1) return null

  const existingPet = mockPets[index]
  const now = new Date().toISOString()

  mockPets[index] = {
    ...existingPet,
    petName: pet.petName,
    breed: pet.breed,
    gender: pet.gender,
    birthDay: pet.birthDay ? new Date(pet.birthDay).toISOString().split('T')[0] : existingPet.birthDay,
    normalPrice: pet.normalPrice ?? existingPet.normalPrice,
    subscriptionPrice: pet.subscriptionPrice ?? existingPet.subscriptionPrice,
    modifyUser: 'admin',
    modifyTime: now
  }

  return {
    ...mockPets[index],
    id: mockPets[index].petId,
    name: mockPets[index].petName,
    breedName: getSystemCodeName('Breed', mockPets[index].breed),
    ownerName: mockPets[index].primaryContact?.name,
    contactPhone: mockPets[index].primaryContact?.phone
  }
}

/**
 * 刪除寵物
 */
export function deleteMockPet(id: number): boolean {
  const index = mockPets.findIndex(p => p.petId === id)
  if (index === -1) return false

  mockPets.splice(index, 1)
  return true
}

/**
 * 取得所有寵物（不分頁）
 */
export function getAllMockPets(): Pet[] {
  return mockPets.map(pet => ({
    ...pet,
    id: pet.petId,
    name: pet.petName,
    breedName: getSystemCodeName('Breed', pet.breed),
    ownerName: pet.primaryContact?.name,
    contactPhone: pet.primaryContact?.phone
  }))
}
