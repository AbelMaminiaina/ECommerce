import { useEffect, useState } from 'react'
import api from '../../services/api'
import { Order, OrderStatus } from '../../types'
import { FiEye, FiX } from 'react-icons/fi'

export default function AdminOrders() {
  const [orders, setOrders] = useState<Order[]>([])
  const [filteredOrders, setFilteredOrders] = useState<Order[]>([])
  const [loading, setLoading] = useState(true)
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null)
  const [showDetailModal, setShowDetailModal] = useState(false)
  const [filterStatus, setFilterStatus] = useState<OrderStatus | 'all'>('all')
  const [searchTerm, setSearchTerm] = useState('')

  useEffect(() => {
    fetchOrders()
  }, [])

  useEffect(() => {
    filterOrders()
  }, [orders, filterStatus, searchTerm])

  const fetchOrders = async () => {
    try {
      const response = await api.get<Order[]>('/orders')
      setOrders(response.data)
    } catch (error) {
      console.error('Failed to fetch orders:', error)
    } finally {
      setLoading(false)
    }
  }

  const filterOrders = () => {
    let result = orders

    // Filter by status
    if (filterStatus !== 'all') {
      result = result.filter(o => o.status === filterStatus)
    }

    // Filter by search term (order ID or user ID)
    if (searchTerm.trim()) {
      const term = searchTerm.toLowerCase()
      result = result.filter(o =>
        o.id.toLowerCase().includes(term) ||
        o.userId.toLowerCase().includes(term)
      )
    }

    setFilteredOrders(result)
  }

  const handleStatusChange = async (orderId: string, newStatus: OrderStatus) => {
    try {
      await api.put(`/orders/${orderId}/status`, newStatus)
      setOrders(orders.map(o => o.id === orderId ? { ...o, status: newStatus } : o))
      alert('Statut mis à jour avec succès')
    } catch (error) {
      alert('Erreur lors de la mise à jour du statut')
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

  const getPaymentStatusText = (status: number) => {
    const statuses = ['En attente', 'Payé', 'Échoué', 'Remboursé']
    return statuses[status] || 'Inconnu'
  }

  const getPaymentStatusColor = (status: number) => {
    const colors = ['bg-yellow-100 text-yellow-800', 'bg-green-100 text-green-800', 'bg-red-100 text-red-800', 'bg-gray-100 text-gray-800']
    return colors[status] || 'bg-gray-100 text-gray-800'
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
      <h1 className="text-3xl font-bold mb-8">Gestion des commandes</h1>

      {/* Filters */}
      <div className="bg-white rounded-lg shadow p-4 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {/* Search */}
          <div>
            <label className="block text-sm font-medium mb-2">Recherche (ID commande ou client)</label>
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Rechercher..."
              className="w-full border border-gray-300 rounded px-3 py-2"
            />
          </div>

          {/* Status Filter */}
          <div>
            <label className="block text-sm font-medium mb-2">Filtrer par statut</label>
            <div className="flex flex-wrap gap-2">
              <button
                onClick={() => setFilterStatus('all')}
                className={`px-3 py-1 rounded text-sm ${filterStatus === 'all' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'}`}
              >
                Toutes ({orders.length})
              </button>
              <button
                onClick={() => setFilterStatus(OrderStatus.Pending)}
                className={`px-3 py-1 rounded text-sm ${filterStatus === OrderStatus.Pending ? 'bg-yellow-600 text-white' : 'bg-yellow-100 text-yellow-800'}`}
              >
                En attente ({orders.filter(o => o.status === OrderStatus.Pending).length})
              </button>
              <button
                onClick={() => setFilterStatus(OrderStatus.Processing)}
                className={`px-3 py-1 rounded text-sm ${filterStatus === OrderStatus.Processing ? 'bg-blue-600 text-white' : 'bg-blue-100 text-blue-800'}`}
              >
                En traitement ({orders.filter(o => o.status === OrderStatus.Processing).length})
              </button>
              <button
                onClick={() => setFilterStatus(OrderStatus.Shipped)}
                className={`px-3 py-1 rounded text-sm ${filterStatus === OrderStatus.Shipped ? 'bg-purple-600 text-white' : 'bg-purple-100 text-purple-800'}`}
              >
                Expédiées ({orders.filter(o => o.status === OrderStatus.Shipped).length})
              </button>
              <button
                onClick={() => setFilterStatus(OrderStatus.Delivered)}
                className={`px-3 py-1 rounded text-sm ${filterStatus === OrderStatus.Delivered ? 'bg-green-600 text-white' : 'bg-green-100 text-green-800'}`}
              >
                Livrées ({orders.filter(o => o.status === OrderStatus.Delivered).length})
              </button>
              <button
                onClick={() => setFilterStatus(OrderStatus.Cancelled)}
                className={`px-3 py-1 rounded text-sm ${filterStatus === OrderStatus.Cancelled ? 'bg-red-600 text-white' : 'bg-red-100 text-red-800'}`}
              >
                Annulées ({orders.filter(o => o.status === OrderStatus.Cancelled).length})
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Orders Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Client</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Total</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Paiement</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Statut</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {filteredOrders.length === 0 ? (
              <tr>
                <td colSpan={7} className="px-6 py-8 text-center text-gray-500">
                  {searchTerm || filterStatus !== 'all' ? 'Aucune commande trouvée avec ces filtres' : 'Aucune commande'}
                </td>
              </tr>
            ) : (
              filteredOrders.map((order) => (
                <tr key={order.id}>
                  <td className="px-6 py-4 font-mono text-sm">#{order.id.slice(-8)}</td>
                  <td className="px-6 py-4 text-sm">
                    {new Date(order.createdAt).toLocaleDateString('fr-FR')}
                  </td>
                  <td className="px-6 py-4 text-sm font-mono">{order.userId.slice(-8)}</td>
                  <td className="px-6 py-4 font-semibold">${order.totalAmount.toFixed(2)}</td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${getPaymentStatusColor(order.paymentStatus)}`}>
                      {getPaymentStatusText(order.paymentStatus)}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${getStatusColor(order.status)}`}>
                      {getStatusText(order.status)}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex items-center justify-end gap-2">
                      <button
                        onClick={() => {
                          setSelectedOrder(order)
                          setShowDetailModal(true)
                        }}
                        className="p-2 text-blue-600 hover:bg-blue-50 rounded"
                        title="Voir les détails"
                      >
                        <FiEye />
                      </button>
                      <select
                        value={order.status}
                        onChange={(e) => handleStatusChange(order.id, parseInt(e.target.value) as OrderStatus)}
                        className="text-sm border border-gray-300 rounded px-2 py-1"
                      >
                        <option value={OrderStatus.Pending}>En attente</option>
                        <option value={OrderStatus.Processing}>En traitement</option>
                        <option value={OrderStatus.Shipped}>Expédiée</option>
                        <option value={OrderStatus.Delivered}>Livrée</option>
                        <option value={OrderStatus.Cancelled}>Annulée</option>
                        <option value={OrderStatus.ReturnRequested}>Retour demandé</option>
                        <option value={OrderStatus.Returned}>Retournée</option>
                      </select>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Order Detail Modal */}
      {showDetailModal && selectedOrder && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg p-6 max-w-3xl w-full max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-2xl font-bold">Détails de la commande #{selectedOrder.id.slice(-8)}</h2>
              <button onClick={() => setShowDetailModal(false)} className="text-gray-500 hover:text-gray-700">
                <FiX size={24} />
              </button>
            </div>

            <div className="space-y-6">
              {/* Order Info */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-gray-600">Date de commande</p>
                  <p className="font-semibold">{new Date(selectedOrder.createdAt).toLocaleString('fr-FR')}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">ID Client</p>
                  <p className="font-mono">{selectedOrder.userId}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Statut commande</p>
                  <span className={`inline-block px-3 py-1 rounded-full text-sm font-semibold ${getStatusColor(selectedOrder.status)}`}>
                    {getStatusText(selectedOrder.status)}
                  </span>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Statut paiement</p>
                  <span className={`inline-block px-3 py-1 rounded-full text-sm font-semibold ${getPaymentStatusColor(selectedOrder.paymentStatus)}`}>
                    {getPaymentStatusText(selectedOrder.paymentStatus)}
                  </span>
                </div>
              </div>

              {/* Items */}
              <div>
                <h3 className="font-semibold text-lg mb-2">Produits</h3>
                <div className="border rounded-lg overflow-hidden">
                  <table className="w-full">
                    <thead className="bg-gray-50">
                      <tr>
                        <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Produit</th>
                        <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Prix</th>
                        <th className="px-4 py-2 text-left text-xs font-medium text-gray-500">Quantité</th>
                        <th className="px-4 py-2 text-right text-xs font-medium text-gray-500">Sous-total</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y">
                      {selectedOrder.items.map((item, idx) => (
                        <tr key={idx}>
                          <td className="px-4 py-2 text-sm">{item.productName}</td>
                          <td className="px-4 py-2 text-sm">${item.price.toFixed(2)}</td>
                          <td className="px-4 py-2 text-sm">{item.quantity}</td>
                          <td className="px-4 py-2 text-sm text-right font-semibold">${(item.price * item.quantity).toFixed(2)}</td>
                        </tr>
                      ))}
                      <tr className="bg-gray-50 font-bold">
                        <td colSpan={3} className="px-4 py-2 text-right">Total</td>
                        <td className="px-4 py-2 text-right">${selectedOrder.totalAmount.toFixed(2)}</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              {/* Shipping Address */}
              <div>
                <h3 className="font-semibold text-lg mb-2">Adresse de livraison</h3>
                <div className="bg-gray-50 p-4 rounded">
                  <p>{selectedOrder.shippingAddress.street}</p>
                  <p>{selectedOrder.shippingAddress.city}, {selectedOrder.shippingAddress.state} {selectedOrder.shippingAddress.zipCode}</p>
                  <p>{selectedOrder.shippingAddress.country}</p>
                </div>
              </div>

              {/* Shipping Info */}
              {selectedOrder.trackingNumber && (
                <div>
                  <h3 className="font-semibold text-lg mb-2">Informations d'expédition</h3>
                  <div className="bg-blue-50 p-4 rounded">
                    <p className="text-sm"><strong>Transporteur:</strong> {selectedOrder.carrierName}</p>
                    <p className="text-sm"><strong>Numéro de suivi:</strong> <span className="font-mono">{selectedOrder.trackingNumber}</span></p>
                    {selectedOrder.estimatedDeliveryDate && (
                      <p className="text-sm"><strong>Livraison estimée:</strong> {new Date(selectedOrder.estimatedDeliveryDate).toLocaleDateString('fr-FR')}</p>
                    )}
                  </div>
                </div>
              )}

              {/* Return Info */}
              {selectedOrder.returnStatus > 0 && (
                <div>
                  <h3 className="font-semibold text-lg mb-2">Informations de retour</h3>
                  <div className="bg-orange-50 p-4 rounded">
                    <p className="text-sm"><strong>Raison:</strong> {selectedOrder.returnReason || '-'}</p>
                    {selectedOrder.returnDeadline && (
                      <p className="text-sm"><strong>Date limite:</strong> {new Date(selectedOrder.returnDeadline).toLocaleDateString('fr-FR')}</p>
                    )}
                  </div>
                </div>
              )}
            </div>

            <div className="flex justify-end mt-6">
              <button
                onClick={() => setShowDetailModal(false)}
                className="px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700"
              >
                Fermer
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
