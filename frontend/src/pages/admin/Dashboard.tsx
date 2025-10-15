import { Link } from 'react-router-dom'
import { FiPackage, FiShoppingBag, FiUsers, FiDollarSign, FiRotateCcw, FiHeadphones, FiShield, FiTruck } from 'react-icons/fi'
import { useEffect, useState } from 'react'
import api from '../../services/api'
import { Product, Order, User } from '../../types'

interface Stats {
  productsCount: number
  ordersCount: number
  usersCount: number
  totalRevenue: number
}

export default function AdminDashboard() {
  const [stats, setStats] = useState<Stats>({
    productsCount: 0,
    ordersCount: 0,
    usersCount: 0,
    totalRevenue: 0
  })
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchStats()
  }, [])

  const fetchStats = async () => {
    try {
      const [productsRes, ordersRes] = await Promise.all([
        api.get<Product[]>('/products'),
        api.get<Order[]>('/orders')
      ])

      const totalRevenue = ordersRes.data.reduce((sum, order) => sum + order.totalAmount, 0)

      setStats({
        productsCount: productsRes.data.length,
        ordersCount: ordersRes.data.length,
        usersCount: 0, // TODO: Add users endpoint
        totalRevenue
      })
    } catch (error) {
      console.error('Failed to fetch stats:', error)
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="animate-pulse space-y-4">
          <div className="h-8 bg-gray-200 rounded w-64 mb-8"></div>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {[...Array(4)].map((_, i) => (
              <div key={i} className="h-32 bg-gray-200 rounded"></div>
            ))}
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-8">Tableau de bord Admin</h1>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Produits</p>
              <p className="text-3xl font-bold">{stats.productsCount}</p>
            </div>
            <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
              <FiPackage className="text-blue-600" size={24} />
            </div>
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Commandes</p>
              <p className="text-3xl font-bold">{stats.ordersCount}</p>
            </div>
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <FiShoppingBag className="text-green-600" size={24} />
            </div>
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Utilisateurs</p>
              <p className="text-3xl font-bold">{stats.usersCount || '-'}</p>
            </div>
            <div className="w-12 h-12 bg-purple-100 rounded-full flex items-center justify-center">
              <FiUsers className="text-purple-600" size={24} />
            </div>
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-gray-600 text-sm">Revenus</p>
              <p className="text-3xl font-bold">${stats.totalRevenue.toFixed(2)}</p>
            </div>
            <div className="w-12 h-12 bg-yellow-100 rounded-full flex items-center justify-center">
              <FiDollarSign className="text-yellow-600" size={24} />
            </div>
          </div>
        </div>
      </div>

      {/* Quick Actions */}
      <h2 className="text-2xl font-bold mb-4">Gestion générale</h2>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <Link
          to="/admin/products"
          className="bg-white p-6 rounded-lg shadow hover:shadow-md transition"
        >
          <div className="flex items-center gap-3 mb-2">
            <FiPackage className="text-blue-600" size={24} />
            <h3 className="font-semibold text-lg">Gérer les produits</h3>
          </div>
          <p className="text-gray-600 text-sm">Ajouter, modifier ou supprimer des produits</p>
        </Link>

        <Link
          to="/admin/orders"
          className="bg-white p-6 rounded-lg shadow hover:shadow-md transition"
        >
          <div className="flex items-center gap-3 mb-2">
            <FiShoppingBag className="text-green-600" size={24} />
            <h3 className="font-semibold text-lg">Gérer les commandes</h3>
          </div>
          <p className="text-gray-600 text-sm">Voir et mettre à jour les commandes</p>
        </Link>

        <Link
          to="/admin/packages"
          className="bg-white p-6 rounded-lg shadow hover:shadow-md transition"
        >
          <div className="flex items-center gap-3 mb-2">
            <FiTruck className="text-orange-600" size={24} />
            <h3 className="font-semibold text-lg">Préparer les colis</h3>
          </div>
          <p className="text-gray-600 text-sm">Générer les étiquettes et expédier</p>
        </Link>

        <Link
          to="/admin/users"
          className="bg-white p-6 rounded-lg shadow hover:shadow-md transition"
        >
          <div className="flex items-center gap-3 mb-2">
            <FiUsers className="text-purple-600" size={24} />
            <h3 className="font-semibold text-lg">Gérer les utilisateurs</h3>
          </div>
          <p className="text-gray-600 text-sm">Voir et gérer les comptes utilisateurs</p>
        </Link>
      </div>

      {/* Legal Compliance Section */}
      <h2 className="text-2xl font-bold mb-4">Conformité légale</h2>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <Link
          to="/admin/returns"
          className="bg-white p-6 rounded-lg shadow hover:shadow-md transition"
        >
          <div className="flex items-center gap-3 mb-2">
            <FiRotateCcw className="text-orange-600" size={24} />
            <h3 className="font-semibold text-lg">Retours (14j)</h3>
          </div>
          <p className="text-gray-600 text-sm">Gérer les demandes de rétractation</p>
        </Link>

        <Link
          to="/admin/shipping"
          className="bg-white p-6 rounded-lg shadow hover:shadow-md transition"
        >
          <div className="flex items-center gap-3 mb-2">
            <FiTruck className="text-blue-600" size={24} />
            <h3 className="font-semibold text-lg">Livraisons</h3>
          </div>
          <p className="text-gray-600 text-sm">Suivi des expéditions et retards</p>
        </Link>

        <Link
          to="/admin/support"
          className="bg-white p-6 rounded-lg shadow hover:shadow-md transition"
        >
          <div className="flex items-center gap-3 mb-2">
            <FiHeadphones className="text-green-600" size={24} />
            <h3 className="font-semibold text-lg">Support (SAV)</h3>
          </div>
          <p className="text-gray-600 text-sm">Tickets clients et assistance</p>
        </Link>

        <Link
          to="/admin/warranty"
          className="bg-white p-6 rounded-lg shadow hover:shadow-md transition"
        >
          <div className="flex items-center gap-3 mb-2">
            <FiShield className="text-purple-600" size={24} />
            <h3 className="font-semibold text-lg">Garantie (2 ans)</h3>
          </div>
          <p className="text-gray-600 text-sm">Réclamations de garantie légale</p>
        </Link>
      </div>
    </div>
  )
}
