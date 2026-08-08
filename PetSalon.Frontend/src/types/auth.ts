export interface User {
  id: number
  userName: string
  name: string
  email?: string
  roles: string[]
  permissions: string[]
  lastLogin?: string
}

export interface LoginCredentials {
  userName: string
  password: string
}

export interface LoginResponse {
  token: string
  user: User
  expiresIn: number
  requiresPasswordChange: boolean
}

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
  confirmPassword: string
}
