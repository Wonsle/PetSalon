import axios from '@/utils/axios'
import type { 
  Permission, 
  Role, 
  UserRole,
  RoleCreateRequest, 
  RoleUpdateRequest,
  UserRoleAssignRequest,
  PermissionGroup,
  RoleSearchParams,
  UserRoleSearchParams,
  RoleListResponse,
  UserRoleListResponse
} from '@/types/permission'

export const permissionApi = {
  // Permission APIs
  async getPermissions(): Promise<Permission[]> {
    const response = await axios.get('/api/permission')
    return response.data
  },

  async getPermissionGroups(): Promise<PermissionGroup[]> {
    const response = await axios.get('/api/permission/groups')
    return response.data
  },

  // Role APIs
  async getRoles(params: RoleSearchParams): Promise<RoleListResponse> {
    const response = await axios.get('/api/role', { params })
    return response.data
  },

  async getRole(id: number): Promise<Role> {
    const response = await axios.get(`/api/role/${id}`)
    return response.data
  },

  async createRole(data: RoleCreateRequest): Promise<Role> {
    const response = await axios.post('/api/role', data)
    return response.data
  },

  async updateRole(data: RoleUpdateRequest): Promise<Role> {
    const response = await axios.put(`/api/role/${data.id}`, data)
    return response.data
  },

  async deleteRole(id: number): Promise<void> {
    await axios.delete(`/api/role/${id}`)
  },

  async toggleRoleStatus(id: number, isActive: boolean): Promise<void> {
    await axios.patch(`/api/role/${id}/status`, { isActive })
  },

  // User Role APIs
  async getUserRoles(params: UserRoleSearchParams): Promise<UserRoleListResponse> {
    const response = await axios.get('/api/user-role', { params })
    return response.data
  },

  async assignUserRoles(data: UserRoleAssignRequest): Promise<void> {
    await axios.post('/api/user-role/assign', data)
  },

  async revokeUserRole(userId: number, roleId: number): Promise<void> {
    await axios.delete(`/api/user-role/${userId}/${roleId}`)
  },

  async getUserPermissions(userId: number): Promise<Permission[]> {
    const response = await axios.get(`/api/user-role/${userId}/permissions`)
    return response.data
  },

  // Utility APIs
  async checkPermission(permission: string): Promise<boolean> {
    try {
      const response = await axios.get(`/api/permission/check/${permission}`)
      return response.data.hasPermission
    } catch {
      return false
    }
  }
}