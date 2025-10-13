import { create } from 'zustand'
import api from '../services/api'
import { Cart } from '../types'

interface CartState {
  cart: Cart | null
  loading: boolean
  error: string | null
  fetchCart: () => Promise<void>
  addToCart: (productId: string, quantity?: number) => Promise<{ success: boolean; message?: string }>
  updateCartItem: (productId: string, quantity: number) => Promise<{ success: boolean; message?: string }>
  removeFromCart: (productId: string) => Promise<{ success: boolean; message?: string }>
  clearCart: () => Promise<{ success: boolean; message?: string }>
}

const useCartStore = create<CartState>((set) => ({
  cart: null,
  loading: false,
  error: null,

  fetchCart: async () => {
    set({ loading: true, error: null })
    try {
      const response = await api.get<Cart>('/cart')
      set({ cart: response.data, loading: false })
    } catch (error: any) {
      set({ error: error.message, loading: false })
    }
  },

  addToCart: async (productId: string, quantity: number = 1) => {
    try {
      const response = await api.post<Cart>('/cart/items', { productId, quantity })
      set({ cart: response.data })
      return { success: true }
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to add to cart',
      }
    }
  },

  updateCartItem: async (productId: string, quantity: number) => {
    try {
      const response = await api.put<Cart>('/cart/items', { productId, quantity })
      set({ cart: response.data })
      return { success: true }
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to update cart',
      }
    }
  },

  removeFromCart: async (productId: string) => {
    try {
      const response = await api.delete<Cart>(`/cart/items/${productId}`)
      set({ cart: response.data })
      return { success: true }
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to remove item',
      }
    }
  },

  clearCart: async () => {
    try {
      await api.delete('/cart')
      set({ cart: null })
      return { success: true }
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to clear cart',
      }
    }
  },
}))

export default useCartStore
