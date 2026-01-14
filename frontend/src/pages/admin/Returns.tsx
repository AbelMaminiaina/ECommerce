import { useEffect, useState } from 'react'
import api from '../../services/api'
import { Order, ReturnStatus } from '../../types'

export default function AdminReturns() {
  const [returns, setReturns] = useState<Order[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchReturns()
  }, [])

  const fetchReturns = async () => {
    try {
      const response = await api.get<Order[]>('/returns')
      setReturns(response.data)
    } catch (error) {
      console.error('Failed to fetch returns:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleStatusChange = async (orderId: string, newStatus: ReturnStatus) => {
    try {
      await api.put(`/returns/orders/${orderId}/status`, { status: newStatus })
      setReturns(returns.map(r => r.id === orderId ? { ...r, returnStatus: newStatus } : r))
      alert('Statut de retour mis à jour avec succès')
    } catch (error) {
      alert('Erreur lors de la mise à jour du statut')
    }
  }

  const getReturnStatusColor = (status: ReturnStatus) => {
    const colors = {
      [ReturnStatus.None]: 'bg-gray-100 text-gray-800',
      [ReturnStatus.Requested]: 'bg-yellow-100 text-yellow-800',
      [ReturnStatus.Approved]: 'bg-blue-100 text-blue-800',
      [ReturnStatus.InTransit]: 'bg-purple-100 text-purple-800',
      [ReturnStatus.Received]: 'bg-green-100 text-green-800',
      [ReturnStatus.Refunded]: 'bg-green-100 text-green-800',
      [ReturnStatus.Rejected]: 'bg-red-100 text-red-800',
    }
    return colors[status] || 'bg-gray-100 text-gray-800'
  }

  const getReturnStatusText = (status: ReturnStatus) => {
    const statusNames = {
      [ReturnStatus.None]: 'Aucun',
      [ReturnStatus.Requested]: 'Demandé',
      [ReturnStatus.Approved]: 'Approuvé',
      [ReturnStatus.InTransit]: 'En transit',
      [ReturnStatus.Received]: 'Reçu',
      [ReturnStatus.Refunded]: 'Remboursé',
      [ReturnStatus.Rejected]: 'Rejeté',
    }
    return statusNames[status] || 'Inconnu'
  }

  const calculateRemainingDays = (returnDeadline?: string) => {
    if (!returnDeadline) return null
    const deadline = new Date(returnDeadline)
    const today = new Date()
    const diffTime = deadline.getTime() - today.getTime()
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
    return diffDays
  }

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="animate-pulse space-y-4">
          {[...Array(5)].map((_, i) => (
            <div key={i} className="h-32 bg-gray-200 rounded"></div>
          ))}
        </div>
      </div>
    )
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-8">Gestion des retours (Droit de rétractation - 14 jours)</h1>

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full min-w-[1000px]">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Commande</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date livraison</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date limite</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Raison</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Montant</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Statut retour</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {returns.length === 0 ? (
              <tr>
                <td colSpan={7} className="px-6 py-8 text-center text-gray-500">
                  Aucune demande de retour
                </td>
              </tr>
            ) : (
              returns.map((order) => {
                const remainingDays = calculateRemainingDays(order.returnDeadline)
                return (
                  <tr key={order.id}>
                    <td className="px-6 py-4 font-mono text-sm">#{order.id.slice(-8)}</td>
                    <td className="px-6 py-4 text-sm">
                      {order.deliveredAt ? new Date(order.deliveredAt).toLocaleDateString('fr-FR') : '-'}
                    </td>
                    <td className="px-6 py-4 text-sm">
                      {order.returnDeadline ? (
                        <div>
                          <div>{new Date(order.returnDeadline).toLocaleDateString('fr-FR')}</div>
                          {remainingDays !== null && remainingDays >= 0 && (
                            <div className="text-xs text-orange-600">
                              {remainingDays} jours restants
                            </div>
                          )}
                          {remainingDays !== null && remainingDays < 0 && (
                            <div className="text-xs text-red-600">
                              Expiré
                            </div>
                          )}
                        </div>
                      ) : '-'}
                    </td>
                    <td className="px-6 py-4 text-sm max-w-xs truncate">
                      {order.returnReason || '-'}
                    </td>
                    <td className="px-6 py-4 font-semibold">${order.totalAmount.toFixed(2)}</td>
                    <td className="px-6 py-4">
                      <span className={`px-2 py-1 rounded-full text-xs font-semibold ${getReturnStatusColor(order.returnStatus)}`}>
                        {getReturnStatusText(order.returnStatus)}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      <select
                        value={order.returnStatus}
                        onChange={(e) => handleStatusChange(order.id, parseInt(e.target.value) as ReturnStatus)}
                        className="text-sm border border-gray-300 rounded px-2 py-1"
                        disabled={order.returnStatus === ReturnStatus.Refunded || order.returnStatus === ReturnStatus.Rejected}
                      >
                        <option value={ReturnStatus.Requested}>Demandé</option>
                        <option value={ReturnStatus.Approved}>Approuvé</option>
                        <option value={ReturnStatus.InTransit}>En transit</option>
                        <option value={ReturnStatus.Received}>Reçu</option>
                        <option value={ReturnStatus.Refunded}>Remboursé</option>
                        <option value={ReturnStatus.Rejected}>Rejeté</option>
                      </select>
                    </td>
                  </tr>
                )
              })
            )}
          </tbody>
        </table>
        </div>
      </div>
    </div>
  )
}
