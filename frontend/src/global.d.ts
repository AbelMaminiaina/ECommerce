import type useAuthStore from './stores/authStore'

declare global {
  interface Window {
    useAuthStore: typeof useAuthStore
  }
}

export {}
