import axios from '@/utils/axios'
import type { LoginCredentials, LoginResponse, User } from '@/types/auth'

export const authApi = {
  async login(credentials: LoginCredentials): Promise<LoginResponse> {
    // Transform the request to match backend expectations
    const loginRequest = {
      UserName: credentials.userName,
      Password: credentials.password
    }
    const response = await axios.post('/api/account/login', loginRequest)
    return response.data
  },

  async getCurrentUser(): Promise<User> {
    const response = await axios.get('/api/account/profile')
    return response.data
  },

  async refreshToken(): Promise<string> {
    const response = await axios.post('/api/account/refresh')
    return response.data.token
  }
}