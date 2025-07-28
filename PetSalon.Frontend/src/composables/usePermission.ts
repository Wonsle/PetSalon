import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { checkPermission } from '@/directives/permission'

/**
 * 權限管理組合式函數
 */
export function usePermission() {
  const authStore = useAuthStore()
  
  /**
   * 當前用戶角色
   */
  const userRoles = computed(() => authStore.currentUser?.roles || [])
  
  /**
   * 檢查是否有權限
   * @param permission 權限字符串或數組
   * @returns 是否有權限
   */
  const hasPermission = (permission: string | string[]): boolean => {
    return checkPermission(permission, userRoles.value)
  }
  
  /**
   * 檢查是否有任一權限
   * @param permissions 權限數組
   * @returns 是否有任一權限
   */
  const hasAnyPermission = (permissions: string[]): boolean => {
    return permissions.some(permission => hasPermission(permission))
  }
  
  /**
   * 檢查是否有所有權限
   * @param permissions 權限數組
   * @returns 是否有所有權限
   */
  const hasAllPermissions = (permissions: string[]): boolean => {
    return permissions.every(permission => hasPermission(permission))
  }
  
  /**
   * 檢查是否是指定角色
   * @param role 角色名稱
   * @returns 是否是指定角色
   */
  const hasRole = (role: string): boolean => {
    return userRoles.value.includes(role)
  }
  
  /**
   * 檢查是否有任一角色
   * @param roles 角色數組
   * @returns 是否有任一角色
   */
  const hasAnyRole = (roles: string[]): boolean => {
    return roles.some(role => hasRole(role))
  }
  
  /**
   * 檢查是否是管理員
   */
  const isAdmin = computed(() => hasRole('Admin'))
  
  /**
   * 檢查是否是經理
   */
  const isManager = computed(() => hasRole('Manager') || isAdmin.value)
  
  /**
   * 檢查是否是設計師
   */
  const isDesigner = computed(() => hasRole('Designer') || isManager.value)
  
  /**
   * 獲取用戶最高權限等級
   */
  const getHighestRole = computed(() => {
    if (isAdmin.value) return 'Admin'
    if (isManager.value) return 'Manager'
    if (isDesigner.value) return 'Designer'
    return 'Guest'
  })
  
  /**
   * 權限級別映射
   */
  const roleLevels: Record<string, number> = {
    'Admin': 1,
    'Manager': 2,
    'Designer': 3,
    'Guest': 4
  }
  
  /**
   * 檢查是否有足夠的權限等級
   * @param requiredLevel 所需等級
   * @returns 是否有足夠權限
   */
  const hasPermissionLevel = (requiredLevel: number): boolean => {
    const currentLevel = roleLevels[getHighestRole.value] || 4
    return currentLevel <= requiredLevel
  }
  
  /**
   * 根據權限過濾菜單項
   * @param menuItems 菜單項數組
   * @returns 過濾後的菜單項
   */
  const filterMenuByPermission = <T extends { permission?: string | string[] }>(menuItems: T[]): T[] => {
    return menuItems.filter(item => {
      if (!item.permission) return true
      return hasPermission(item.permission)
    })
  }
  
  /**
   * 權限守衛 - 用於路由守衛
   * @param requiredPermission 所需權限
   * @returns 是否可以訪問
   */
  const canAccess = (requiredPermission?: string | string[]): boolean => {
    if (!requiredPermission) return true
    return hasPermission(requiredPermission)
  }
  
  return {
    userRoles,
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
    hasAnyRole,
    isAdmin,
    isManager,
    isDesigner,
    getHighestRole,
    hasPermissionLevel,
    filterMenuByPermission,
    canAccess
  }
}