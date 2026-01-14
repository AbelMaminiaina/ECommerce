import { create } from 'zustand'
import api from '../services/api'
import { User, RegisterData, AuthResponse } from '../types'

interface AuthState {
  user: User | null
  accessToken: string | null
  refreshToken: string | null
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<{ success: boolean; message?: string }>
  register: (userData: RegisterData) => Promise<{ success: boolean; message?: string }>
  logout: () => void
  checkAuth: () => Promise<void>
  setAuth: (user: User, accessToken: string, refreshToken: string) => void
}

const useAuthStore = create<AuthState>((set) => ({
  user: null,
  accessToken: localStorage.getItem('accessToken'),
  refreshToken: localStorage.getItem('refreshToken'),
  isAuthenticated: !!localStorage.getItem('accessToken'),

  login: async (email: string, password: string) => {
    try {
      const response = await api.post<AuthResponse>('/auth/login', { email, password })
      const { accessToken, refreshToken, user } = response.data

      localStorage.setItem('accessToken', accessToken)
      localStorage.setItem('refreshToken', refreshToken)

      set({
        user,
        accessToken,
        refreshToken,
        isAuthenticated: true,
      })

      return { success: true }
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Login failed',
      }
    }
  },

  register: async (userData: RegisterData) => {
    try {
      const response = await api.post<AuthResponse>('/auth/register', userData)
      const { accessToken, refreshToken, user } = response.data

      localStorage.setItem('accessToken', accessToken)
      localStorage.setItem('refreshToken', refreshToken)

      set({
        user,
        accessToken,
        refreshToken,
        isAuthenticated: true,
      })

      return { success: true }
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Registration failed',
      }
    }
  },

  logout: () => {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')

    set({
      user: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,
    })
  },

  checkAuth: async () => {
    const token = localStorage.getItem('accessToken')
    if (!token) {
      set({ isAuthenticated: false, user: null })
      return
    }

    try {
      // Try to get user profile to verify token
      const response = await api.get<User>('/auth/profile')
      set({ user: response.data, isAuthenticated: true })
    } catch (error) {
      set({ isAuthenticated: false, user: null })
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
    }
  },

  setAuth: (user: User, accessToken: string, refreshToken: string) => {
    set({
      user,
      accessToken,
      refreshToken,
      isAuthenticated: true,
    })
  },
}))

// Expose the store on window for use in api interceptors
if (typeof window !== 'undefined') {
  (window as any).useAuthStore = useAuthStore
}

export default useAuthStore
