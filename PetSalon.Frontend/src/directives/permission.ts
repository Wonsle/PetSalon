import type { App, Directive } from 'vue'
import { useAuthStore } from '@/stores/auth'

/**
 * 權限指令
 * 用法: v-permission="'role:create'" 或 v-permission="['role:create', 'role:update']"
 */
const permission: Directive = {
  mounted(el: HTMLElement, binding) {
    const { value } = binding
    const authStore = useAuthStore()
    
    if (!value) {
      throw new Error('權限指令需要提供權限值')
    }
    
    const hasPermission = checkPermission(value, authStore.currentUser?.roles || [])
    
    if (!hasPermission) {
      // 隱藏元素或禁用
      if (binding.modifiers.hide) {
        el.style.display = 'none'
      } else {
        el.style.pointerEvents = 'none'
        el.style.opacity = '0.5'
        el.setAttribute('disabled', 'true')
        // 如果是 Element Plus 組件，添加 disabled class
        el.classList.add('is-disabled')
      }
    }
  },
  
  updated(el: HTMLElement, binding) {
    const { value } = binding
    const authStore = useAuthStore()
    
    if (!value) return
    
    const hasPermission = checkPermission(value, authStore.currentUser?.roles || [])
    
    if (hasPermission) {
      // 恢復元素
      el.style.display = ''
      el.style.pointerEvents = ''
      el.style.opacity = ''
      el.removeAttribute('disabled')
      el.classList.remove('is-disabled')
    } else {
      // 隱藏或禁用元素
      if (binding.modifiers.hide) {
        el.style.display = 'none'
      } else {
        el.style.pointerEvents = 'none'
        el.style.opacity = '0.5'
        el.setAttribute('disabled', 'true')
        el.classList.add('is-disabled')
      }
    }
  }
}

/**
 * 檢查權限
 * @param permission 權限字符串或數組
 * @param userRoles 用戶角色數組
 * @returns 是否有權限
 */
function checkPermission(permission: string | string[], userRoles: string[]): boolean {
  if (!permission || !userRoles || userRoles.length === 0) {
    return false
  }
  
  // 超級管理員擁有所有權限
  if (userRoles.includes('Admin')) {
    return true
  }
  
  const permissions = Array.isArray(permission) ? permission : [permission]
  
  // 檢查是否有任一權限
  return permissions.some(perm => hasRolePermission(perm, userRoles))
}

/**
 * 檢查角色是否有特定權限
 * @param permission 權限字符串 (格式: module:action)
 * @param userRoles 用戶角色數組
 * @returns 是否有權限
 */
function hasRolePermission(permission: string, userRoles: string[]): boolean {
  const [module, action] = permission.split(':')
  
  // 角色權限映射
  const rolePermissions: Record<string, string[]> = {
    'Admin': ['*'], // 超級管理員有所有權限
    'Manager': [
      'pet:*', 'contact:*', 'reservation:*', 'subscription:*', 
      'income:*', 'expense:*', 'report:view'
    ],
    'Designer': [
      'pet:view', 'pet:update', 'contact:view', 'contact:update',
      'reservation:*', 'subscription:view', 'income:create', 'income:view'
    ]
  }
  
  for (const role of userRoles) {
    const permissions = rolePermissions[role] || []
    
    // 檢查是否有完全匹配的權限
    if (permissions.includes(permission)) {
      return true
    }
    
    // 檢查是否有模組級別的所有權限
    if (permissions.includes(`${module}:*`)) {
      return true
    }
    
    // 檢查是否有全部權限
    if (permissions.includes('*')) {
      return true
    }
  }
  
  return false
}

export default {
  install(app: App) {
    app.directive('permission', permission)
  }
}

export { checkPermission, hasRolePermission }