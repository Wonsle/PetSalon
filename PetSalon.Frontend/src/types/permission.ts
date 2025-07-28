export interface Permission {
  id: number
  code: string
  name: string
  description: string
  module: string
  action: string
  resource: string
  isActive: boolean
  createTime: string
  updateTime: string
}

export interface Role {
  id: number
  name: string
  code: string
  description: string
  level: number
  permissions: Permission[]
  isActive: boolean
  isSystem: boolean
  createTime: string
  updateTime: string
}

export interface UserRole {
  id: number
  userId: number
  userName: string
  userDisplayName: string
  roleId: number
  roleName: string
  roleCode: string
  assignedBy: string
  assignedTime: string
  expiryDate?: string
  isActive: boolean
}

export interface RoleCreateRequest {
  name: string
  code: string
  description: string
  level: number
  permissionIds: number[]
}

export interface RoleUpdateRequest extends RoleCreateRequest {
  id: number
}

export interface UserRoleAssignRequest {
  userId: number
  roleIds: number[]
  expiryDate?: string
}

export interface PermissionGroup {
  module: string
  permissions: Permission[]
}

export interface RoleSearchParams {
  keyword?: string
  level?: number
  isActive?: boolean
  page?: number
  pageSize?: number
}

export interface UserRoleSearchParams {
  keyword?: string
  roleId?: number
  isActive?: boolean
  page?: number
  pageSize?: number
}

export interface RoleListResponse {
  data: Role[]
  total: number
  page: number
  pageSize: number
}

export interface UserRoleListResponse {
  data: UserRole[]
  total: number
  page: number
  pageSize: number
}