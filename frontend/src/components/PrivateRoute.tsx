import { Navigate } from 'react-router-dom'
import useAuthStore from '../stores/authStore'
import { ReactNode } from 'react'

interface PrivateRouteProps {
  children: ReactNode
  adminOnly?: boolean
}

export default function PrivateRoute({ children, adminOnly = false }: PrivateRouteProps) {
  const { isAuthenticated, user } = useAuthStore()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  if (adminOnly && user?.role !== 'Admin') {
    return <Navigate to="/" replace />
  }

  return <>{children}</>
}
