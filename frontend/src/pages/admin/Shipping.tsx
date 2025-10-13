import { useEffect, useState } from 'react'
import api from '../../services/api'
import { Order, OrderStatus } from '../../types'

export default function AdminShipping() {
  const [orders, setOrders] = useState<Order[]>([])
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null)
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [trackingNumber, setTrackingNumber] = useState('')
  const [carrierName, setCarrierName] = useState('Colissimo')
  const [estimatedDeliveryDays, setEstimatedDeliveryDays] = useState(3)

  useEffect(() => {
    fetchOrders()
  }, [])

  const fetchOrders = async () => {
    try {
      const response = await api.get<Order[]>('/orders')
      // Filter orders that need shipping (Processing or Shipped status)
      const shippableOrders = response.data.filter(
        o => o.status === OrderStatus.Processing ||
             o.status === OrderStatus.Shipped ||
             o.status === OrderStatus.Delivered
      )
      setOrders(shippableOrders)
    } catch (error) {
      console.error('Failed to fetch orders:', error)
    } finally {
      setLoading(false)
    }
  }

  const fetchDelayedOrders = async () => {
    try {
      const response = await api.get<Order[]>('/shipping/delayed-orders')
      setOrders(response.data)
    } catch (error) {
      console.error('Failed to fetch delayed orders:', error)
    }
  }

  const handleUpdateShipping = async () => {
    if (!selectedOrder || !trackingNumber.trim()) {
      alert('Veuillez remplir tous les champs')
      return
    }

    try {
      await api.put(`/shipping/orders/${selectedOrder.id}/shipping`, {
        trackingNumber,
        carrierName,
        estimatedDeliveryDays,
      })
      alert('Informations de suivi mises à jour avec succès')
      setShowModal(false)
      setTrackingNumber('')
      setCarrierName('Colissimo')
      setEstimatedDeliveryDays(3)
      fetchOrders()
    } catch (error) {
      alert('Erreur lors de la mise à jour')
    }
  }

  const getStatusColor = (status: OrderStatus) => {
    const colors = {
      [OrderStatus.Pending]: 'bg-yellow-100 text-yellow-800',
      [OrderStatus.Processing]: 'bg-blue-100 text-blue-800',
      [OrderStatus.Shipped]: 'bg-purple-100 text-purple-800',
      [OrderStatus.Delivered]: 'bg-green-100 text-green-800',
      [OrderStatus.Cancelled]: 'bg-red-100 text-red-800',
      [OrderStatus.ReturnRequested]: 'bg-orange-100 text-orange-800',
      [OrderStatus.Returned]: 'bg-gray-100 text-gray-800',
    }
    return colors[status] || 'bg-gray-100 text-gray-800'
  }

  const getStatusText = (status: OrderStatus) => {
    const statusNames = {
      [OrderStatus.Pending]: 'En attente',
      [OrderStatus.Processing]: 'En traitement',
      [OrderStatus.Shipped]: 'Expédiée',
      [OrderStatus.Delivered]: 'Livrée',
      [OrderStatus.Cancelled]: 'Annulée',
      [OrderStatus.ReturnRequested]: 'Retour demandé',
      [OrderStatus.Returned]: 'Retournée',
    }
    return statusNames[status] || 'Inconnu'
  }

  const calculateDaysUntilDelivery = (estimatedDate?: string) => {
    if (!estimatedDate) return null
    const estimated = new Date(estimatedDate)
    const today = new Date()
    const diffTime = estimated.getTime() - today.getTime()
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
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-3xl font-bold">Gestion des expéditions et suivi</h1>
        <button
          onClick={fetchDelayedOrders}
          className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
        >
          Voir les retards
        </button>
      </div>

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Commande</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Statut</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Transporteur</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">N° de suivi</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Expédié le</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Livraison estimée</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Délai</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {orders.length === 0 ? (
              <tr>
                <td colSpan={8} className="px-6 py-8 text-center text-gray-500">
                  Aucune commande à expédier
                </td>
              </tr>
            ) : (
              orders.map((order) => {
                const daysUntilDelivery = calculateDaysUntilDelivery(order.estimatedDeliveryDate)
                return (
                  <tr key={order.id} className={order.isDeliveryDelayed ? 'bg-red-50' : ''}>
                    <td className="px-6 py-4 font-mono text-sm">#{order.id.slice(-8)}</td>
                    <td className="px-6 py-4">
                      <span className={`px-2 py-1 rounded-full text-xs font-semibold ${getStatusColor(order.status)}`}>
                        {getStatusText(order.status)}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-sm">{order.carrierName || '-'}</td>
                    <td className="px-6 py-4 text-sm font-mono">{order.trackingNumber || '-'}</td>
                    <td className="px-6 py-4 text-sm">
                      {order.shippedAt ? new Date(order.shippedAt).toLocaleDateString('fr-FR') : '-'}
                    </td>
                    <td className="px-6 py-4 text-sm">
                      {order.estimatedDeliveryDate ? (
                        <div>
                          <div>{new Date(order.estimatedDeliveryDate).toLocaleDateString('fr-FR')}</div>
                          {daysUntilDelivery !== null && daysUntilDelivery >= 0 && (
                            <div className="text-xs text-blue-600">Dans {daysUntilDelivery} jours</div>
                          )}
                        </div>
                      ) : '-'}
                    </td>
                    <td className="px-6 py-4">
                      {order.isDeliveryDelayed && (
                        <span className="px-2 py-1 bg-red-100 text-red-800 text-xs rounded-full font-semibold">
                          ⚠️ Retard
                        </span>
                      )}
                    </td>
                    <td className="px-6 py-4 text-right">
                      <button
                        onClick={() => {
                          setSelectedOrder(order)
                          setTrackingNumber(order.trackingNumber || '')
                          setCarrierName(order.carrierName || 'Colissimo')
                          setEstimatedDeliveryDays(order.estimatedDeliveryDays || 3)
                          setShowModal(true)
                        }}
                        className="text-sm px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700"
                      >
                        {order.trackingNumber ? 'Modifier' : 'Ajouter suivi'}
                      </button>
                    </td>
                  </tr>
                )
              })
            )}
          </tbody>
        </table>
      </div>

      {/* Modal pour ajouter/modifier le suivi */}
      {showModal && selectedOrder && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-lg w-full mx-4">
            <h2 className="text-2xl font-bold mb-4">Informations de suivi</h2>

            <div className="mb-4">
              <p className="text-sm text-gray-600 mb-2">
                <strong>Commande:</strong> #{selectedOrder.id.slice(-8)}
              </p>
              <p className="text-sm text-gray-600 mb-2">
                <strong>Montant:</strong> ${selectedOrder.totalAmount.toFixed(2)}
              </p>
            </div>

            <div className="mb-4">
              <label className="block text-sm font-medium mb-2">Transporteur</label>
              <select
                value={carrierName}
                onChange={(e) => setCarrierName(e.target.value)}
                className="w-full border border-gray-300 rounded p-2 text-sm"
              >
                <option value="Colissimo">Colissimo</option>
                <option value="Chronopost">Chronopost</option>
                <option value="UPS">UPS</option>
                <option value="DHL">DHL</option>
                <option value="FedEx">FedEx</option>
                <option value="La Poste">La Poste</option>
              </select>
            </div>

            <div className="mb-4">
              <label className="block text-sm font-medium mb-2">Numéro de suivi</label>
              <input
                type="text"
                value={trackingNumber}
                onChange={(e) => setTrackingNumber(e.target.value)}
                className="w-full border border-gray-300 rounded p-2 text-sm"
                placeholder="Entrez le numéro de suivi"
              />
            </div>

            <div className="mb-4">
              <label className="block text-sm font-medium mb-2">Délai de livraison estimé (jours)</label>
              <input
                type="number"
                value={estimatedDeliveryDays}
                onChange={(e) => setEstimatedDeliveryDays(parseInt(e.target.value) || 3)}
                min="1"
                max="30"
                className="w-full border border-gray-300 rounded p-2 text-sm"
              />
            </div>

            <div className="flex justify-end gap-2">
              <button
                onClick={() => {
                  setShowModal(false)
                  setTrackingNumber('')
                  setCarrierName('Colissimo')
                  setEstimatedDeliveryDays(3)
                }}
                className="px-4 py-2 border border-gray-300 rounded hover:bg-gray-50"
              >
                Annuler
              </button>
              <button
                onClick={handleUpdateShipping}
                className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
              >
                Enregistrer
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
