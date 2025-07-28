export interface User {
  id: number
  userName: string
  name?: string
  email?: string
  roles: string[]
  lastLogin?: Date
}

export interface LoginCredentials {
  userName: string
  password: string
}

export interface LoginResponse {
  token: string
  user: User
  expiresIn: number
}