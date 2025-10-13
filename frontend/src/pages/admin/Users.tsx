import { useState, useEffect } from 'react'
import { FiSearch, FiEdit2, FiTrash2, FiUserCheck, FiUserX } from 'react-icons/fi'
import api from '../../services/api'
import { User } from '../../types'

interface UserStats {
  totalUsers: number
  activeUsers: number
  inactiveUsers: number
  admins: number
  customers: number
}

export default function AdminUsers() {
  const [users, setUsers] = useState<User[]>([])
  const [stats, setStats] = useState<UserStats | null>(null)
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedUser, setSelectedUser] = useState<User | null>(null)
  const [showEditModal, setShowEditModal] = useState(false)

  useEffect(() => {
    fetchUsers()
    fetchStats()
  }, [])

  const fetchUsers = async () => {
    try {
      setLoading(true)
      const response = await api.get<User[]>('/users')
      setUsers(response.data)
    } catch (error) {
      console.error('Erreur lors du chargement des utilisateurs:', error)
    } finally {
      setLoading(false)
    }
  }

  const fetchStats = async () => {
    try {
      const response = await api.get<UserStats>('/users/stats')
      setStats(response.data)
    } catch (error) {
      console.error('Erreur lors du chargement des statistiques:', error)
    }
  }

  const handleSearch = async () => {
    if (!searchTerm.trim()) {
      fetchUsers()
      return
    }

    try {
      setLoading(true)
      const response = await api.get<User[]>(`/users/search?term=${searchTerm}`)
      setUsers(response.data)
    } catch (error) {
      console.error('Erreur lors de la recherche:', error)
    } finally {
      setLoading(false)
    }
  }

  const toggleUserStatus = async (userId: string, currentStatus: boolean) => {
    try {
      await api.put(`/users/${userId}/status`, { isActive: !currentStatus })
      fetchUsers()
      fetchStats()
    } catch (error) {
      console.error('Erreur lors de la modification du statut:', error)
      alert('Erreur lors de la modification du statut')
    }
  }

  const toggleUserRole = async (userId: string, currentRole: string) => {
    const newRole = currentRole === 'Admin' ? 'Customer' : 'Admin'

    if (!confirm(`Voulez-vous vraiment changer le rôle de cet utilisateur en ${newRole} ?`)) {
      return
    }

    try {
      await api.put(`/users/${userId}/role`, { role: newRole })
      fetchUsers()
      fetchStats()
    } catch (error) {
      console.error('Erreur lors de la modification du rôle:', error)
      alert('Erreur lors de la modification du rôle')
    }
  }

  const deleteUser = async (userId: string, userEmail: string) => {
    if (!confirm(`Voulez-vous vraiment supprimer l'utilisateur ${userEmail} ?\nCette action est irréversible.`)) {
      return
    }

    try {
      await api.delete(`/users/${userId}`)
      fetchUsers()
      fetchStats()
    } catch (error: any) {
      console.error('Erreur lors de la suppression:', error)
      alert(error.response?.data?.message || 'Erreur lors de la suppression')
    }
  }

  const filteredUsers = users.filter(user =>
    user.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.firstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    user.lastName.toLowerCase().includes(searchTerm.toLowerCase())
  )

  if (loading && users.length === 0) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="flex justify-center items-center h-64">
          <div className="text-gray-600">Chargement...</div>
        </div>
      </div>
    )
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-8">Gestion des utilisateurs</h1>

      {/* Statistiques */}
      {stats && (
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4 mb-8">
          <div className="bg-blue-500 text-white rounded-lg shadow p-6">
            <div className="text-2xl font-bold">{stats.totalUsers}</div>
            <div className="text-sm opacity-90">Total</div>
          </div>
          <div className="bg-green-500 text-white rounded-lg shadow p-6">
            <div className="text-2xl font-bold">{stats.activeUsers}</div>
            <div className="text-sm opacity-90">Actifs</div>
          </div>
          <div className="bg-gray-500 text-white rounded-lg shadow p-6">
            <div className="text-2xl font-bold">{stats.inactiveUsers}</div>
            <div className="text-sm opacity-90">Inactifs</div>
          </div>
          <div className="bg-purple-500 text-white rounded-lg shadow p-6">
            <div className="text-2xl font-bold">{stats.admins}</div>
            <div className="text-sm opacity-90">Admins</div>
          </div>
          <div className="bg-indigo-500 text-white rounded-lg shadow p-6">
            <div className="text-2xl font-bold">{stats.customers}</div>
            <div className="text-sm opacity-90">Clients</div>
          </div>
        </div>
      )}

      {/* Barre de recherche */}
      <div className="bg-white rounded-lg shadow p-6 mb-8">
        <div className="flex gap-4">
          <div className="flex-1 relative">
            <input
              type="text"
              placeholder="Rechercher par email, nom ou prénom..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <FiSearch className="absolute right-3 top-3 text-gray-400" />
          </div>
          <button
            onClick={handleSearch}
            className="px-6 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
          >
            Rechercher
          </button>
          {searchTerm && (
            <button
              onClick={() => {
                setSearchTerm('')
                fetchUsers()
              }}
              className="px-6 py-2 bg-gray-500 text-white rounded-lg hover:bg-gray-600"
            >
              Réinitialiser
            </button>
          )}
        </div>
      </div>

      {/* Liste des utilisateurs */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Utilisateur
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Email
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Téléphone
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Rôle
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Statut
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Date d'inscription
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredUsers.length === 0 ? (
                <tr>
                  <td colSpan={7} className="px-6 py-8 text-center text-gray-500">
                    Aucun utilisateur trouvé
                  </td>
                </tr>
              ) : (
                filteredUsers.map((user) => (
                  <tr key={user.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        <div className="flex-shrink-0 h-10 w-10 bg-blue-500 rounded-full flex items-center justify-center text-white font-semibold">
                          {user.firstName[0]}{user.lastName[0]}
                        </div>
                        <div className="ml-4">
                          <div className="text-sm font-medium text-gray-900">
                            {user.firstName} {user.lastName}
                          </div>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-900">{user.email}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-900">{user.phoneNumber}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <button
                        onClick={() => toggleUserRole(user.id, user.role)}
                        className={`px-3 py-1 rounded-full text-xs font-semibold ${
                          user.role === 'Admin'
                            ? 'bg-purple-100 text-purple-800 hover:bg-purple-200'
                            : 'bg-blue-100 text-blue-800 hover:bg-blue-200'
                        }`}
                      >
                        {user.role}
                      </button>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <button
                        onClick={() => toggleUserStatus(user.id, user.isActive ?? true)}
                        className={`px-3 py-1 rounded-full text-xs font-semibold ${
                          user.isActive !== false
                            ? 'bg-green-100 text-green-800 hover:bg-green-200'
                            : 'bg-red-100 text-red-800 hover:bg-red-200'
                        }`}
                      >
                        {user.isActive !== false ? 'Actif' : 'Inactif'}
                      </button>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {user.createdAt ? new Date(user.createdAt).toLocaleDateString('fr-FR') : 'N/A'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => toggleUserStatus(user.id, user.isActive ?? true)}
                          className="text-blue-600 hover:text-blue-900"
                          title={user.isActive !== false ? 'Désactiver' : 'Activer'}
                        >
                          {user.isActive !== false ? <FiUserX /> : <FiUserCheck />}
                        </button>
                        <button
                          onClick={() => deleteUser(user.id, user.email)}
                          className="text-red-600 hover:text-red-900"
                          title="Supprimer"
                        >
                          <FiTrash2 />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {/* Résumé */}
      <div className="mt-4 text-sm text-gray-600">
        Affichage de {filteredUsers.length} utilisateur(s)
      </div>
    </div>
  )
}
