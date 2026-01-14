import { useEffect, useState } from 'react'
import api from '../services/api'
import { Order, OrderStatus, ReturnStatus } from '../types'
import { FiPackage, FiTruck, FiCheck, FiX, FiRotateCcw, FiAlertCircle, FiClock } from 'react-icons/fi'

export default function Orders() {
  const [orders, setOrders] = useState<Order[]>([])
  const [loading, setLoading] = useState(true)
  const [showReturnModal, setShowReturnModal] = useState(false)
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null)
  const [returnReason, setReturnReason] = useState('')
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    fetchOrders()
  }, [])

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

  const handleRequestReturn = (order: Order) => {
    setSelectedOrder(order)
    setShowReturnModal(true)
  }

  const submitReturn = async () => {
    if (!selectedOrder || !returnReason.trim()) return

    setSubmitting(true)
    try {
      await api.post(`/returns/orders/${selectedOrder.id}/request`, {
        reason: returnReason
      })

      // Rafraîchir les commandes
      await fetchOrders()

      // Fermer le modal et réinitialiser
      setShowReturnModal(false)
      setSelectedOrder(null)
      setReturnReason('')

      alert('Votre demande de retour a été enregistrée avec succès !')
    } catch (error: any) {
      const message = error.response?.data?.message || 'Erreur lors de la demande de retour'
      alert(message)
    } finally {
      setSubmitting(false)
    }
  }

  const getRemainingDays = (returnDeadline?: string) => {
    if (!returnDeadline) return null
    const deadline = new Date(returnDeadline)
    const now = new Date()
    const diff = deadline.getTime() - now.getTime()
    const days = Math.ceil(diff / (1000 * 60 * 60 * 24))
    return days > 0 ? days : 0
  }

  const getStatusColor = (status: OrderStatus) => {
    switch (status) {
      case OrderStatus.Pending:
        return 'bg-yellow-100 text-yellow-800'
      case OrderStatus.Processing:
        return 'bg-blue-100 text-blue-800'
      case OrderStatus.Shipped:
        return 'bg-purple-100 text-purple-800'
      case OrderStatus.Delivered:
        return 'bg-green-100 text-green-800'
      case OrderStatus.Cancelled:
        return 'bg-red-100 text-red-800'
      case OrderStatus.ReturnRequested:
        return 'bg-orange-100 text-orange-800'
      case OrderStatus.Returned:
        return 'bg-gray-100 text-gray-800'
      default:
        return 'bg-gray-100 text-gray-800'
    }
  }

  const getStatusIcon = (status: OrderStatus) => {
    switch (status) {
      case OrderStatus.Pending:
        return <FiPackage />
      case OrderStatus.Processing:
        return <FiPackage />
      case OrderStatus.Shipped:
        return <FiTruck />
      case OrderStatus.Delivered:
        return <FiCheck />
      case OrderStatus.Cancelled:
        return <FiX />
      case OrderStatus.ReturnRequested:
        return <FiRotateCcw />
      case OrderStatus.Returned:
        return <FiRotateCcw />
      default:
        return <FiPackage />
    }
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

  const getReturnStatusText = (status: ReturnStatus) => {
    const statusNames = {
      [ReturnStatus.None]: '',
      [ReturnStatus.Requested]: 'Demandé',
      [ReturnStatus.Approved]: 'Approuvé',
      [ReturnStatus.InTransit]: 'En transit',
      [ReturnStatus.Received]: 'Reçu',
      [ReturnStatus.Refunded]: 'Remboursé',
      [ReturnStatus.Rejected]: 'Refusé',
    }
    return statusNames[status] || ''
  }

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="animate-pulse space-y-4">
          {[...Array(3)].map((_, i) => (
            <div key={i} className="h-48 bg-gray-200 rounded-lg"></div>
          ))}
        </div>
      </div>
    )
  }

  if (orders.length === 0) {
    return (
      <div className="container mx-auto px-4 py-16 text-center">
        <FiPackage size={80} className="mx-auto text-gray-300 mb-4" />
        <h2 className="text-2xl font-bold mb-4">Aucune commande</h2>
        <p className="text-gray-600 mb-8">Vous n'avez pas encore passé de commande</p>
        <a
          href="/products"
          className="inline-block px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700"
        >
          Voir les produits
        </a>
      </div>
    )
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-8">Mes Commandes</h1>

      <div className="space-y-4">
        {orders.map((order) => (
          <div key={order.id} className="bg-white rounded-lg shadow p-6">
            <div className="flex flex-wrap items-center justify-between mb-4 gap-4">
              <div>
                <p className="text-sm text-gray-600">Commande #{order.id.slice(-8)}</p>
                <p className="text-sm text-gray-600">
                  {new Date(order.createdAt).toLocaleDateString('fr-FR')}
                </p>
              </div>

              <div className="flex items-center gap-4">
                <span className={`px-3 py-1 rounded-full text-sm font-semibold flex items-center gap-2 ${getStatusColor(order.status)}`}>
                  {getStatusIcon(order.status)}
                  {getStatusText(order.status)}
                </span>
                <span className="text-xl font-bold text-blue-600">
                  ${order.totalAmount.toFixed(2)}
                </span>
              </div>
            </div>

            <div className="border-t pt-4">
              <h3 className="font-semibold mb-2">Articles</h3>
              <div className="space-y-2">
                {order.items.map((item, index) => (
                  <div key={index} className="flex justify-between text-sm">
                    <span>
                      {item.productName} × {item.quantity}
                    </span>
                    <span className="font-semibold">
                      ${(item.price * item.quantity).toFixed(2)}
                    </span>
                  </div>
                ))}
              </div>
            </div>

            <div className="border-t mt-4 pt-4">
              <h3 className="font-semibold mb-2">Adresse de livraison</h3>
              <p className="text-sm text-gray-600">
                {order.shippingAddress.street}<br />
                {order.shippingAddress.city}, {order.shippingAddress.state} {order.shippingAddress.zipCode}<br />
                {order.shippingAddress.country}
              </p>
            </div>

            {/* Section suivi de livraison */}
            {(order.trackingNumber || order.shippedAt || order.isDeliveryDelayed) && (
              <div className="border-t mt-4 pt-4 bg-purple-50 p-4 rounded-md">
                <div className="flex items-center gap-2 mb-3">
                  <FiTruck className="text-purple-600" />
                  <span className="font-semibold text-purple-900">Suivi de livraison</span>
                </div>

                {order.trackingNumber && (
                  <div className="mb-2">
                    <p className="text-sm text-purple-700">
                      <strong>Numéro de suivi :</strong> {order.trackingNumber}
                    </p>
                    {order.carrierName && (
                      <p className="text-sm text-purple-700">
                        <strong>Transporteur :</strong> {order.carrierName}
                      </p>
                    )}
                  </div>
                )}

                {order.shippedAt && (
                  <p className="text-xs text-purple-600 mb-1">
                    Expédiée le {new Date(order.shippedAt).toLocaleDateString('fr-FR')}
                  </p>
                )}

                {order.estimatedDeliveryDate && !order.deliveredAt && (
                  <div className="flex items-center gap-2 mt-2">
                    <FiClock className="text-purple-600" />
                    <p className="text-sm text-purple-700">
                      Livraison estimée : <strong>{new Date(order.estimatedDeliveryDate).toLocaleDateString('fr-FR')}</strong>
                    </p>
                  </div>
                )}

                {order.isDeliveryDelayed && (
                  <div className="mt-2 p-2 bg-red-100 border border-red-300 rounded-md">
                    <p className="text-sm text-red-700 flex items-center gap-2">
                      <FiAlertCircle />
                      <strong>Retard de livraison détecté</strong>
                    </p>
                  </div>
                )}
              </div>
            )}

            {/* Section retour */}
            {order.status === OrderStatus.Delivered && order.canReturn && (
              <div className="border-t mt-4 pt-4 bg-blue-50 p-4 rounded-md">
                <div className="flex items-center justify-between">
                  <div>
                    <div className="flex items-center gap-2 mb-2">
                      <FiAlertCircle className="text-blue-600" />
                      <span className="font-semibold text-blue-900">Droit de rétractation</span>
                    </div>
                    <p className="text-sm text-blue-700">
                      Vous avez encore <strong>{getRemainingDays(order.returnDeadline)} jours</strong> pour retourner cette commande
                    </p>
                    <p className="text-xs text-blue-600 mt-1">
                      Date limite : {order.returnDeadline && new Date(order.returnDeadline).toLocaleDateString('fr-FR')}
                    </p>
                  </div>
                  <button
                    onClick={() => handleRequestReturn(order)}
                    className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 flex items-center gap-2"
                  >
                    <FiRotateCcw />
                    Demander un retour
                  </button>
                </div>
              </div>
            )}

            {/* Affichage du statut de retour si demandé */}
            {order.returnStatus !== ReturnStatus.None && (
              <div className="border-t mt-4 pt-4 bg-orange-50 p-4 rounded-md">
                <div className="flex items-center gap-2 mb-2">
                  <FiRotateCcw className="text-orange-600" />
                  <span className="font-semibold text-orange-900">Statut du retour</span>
                </div>
                <p className="text-sm text-orange-700">
                  <strong>Statut :</strong> {getReturnStatusText(order.returnStatus)}
                </p>
                {order.returnReason && (
                  <p className="text-sm text-orange-700 mt-1">
                    <strong>Raison :</strong> {order.returnReason}
                  </p>
                )}
                {order.returnRequestedAt && (
                  <p className="text-xs text-orange-600 mt-1">
                    Demandé le {new Date(order.returnRequestedAt).toLocaleDateString('fr-FR')}
                  </p>
                )}
              </div>
            )}
          </div>
        ))}
      </div>

      {/* Modal de demande de retour */}
      {showReturnModal && selectedOrder && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-md w-full p-6">
            <h2 className="text-2xl font-bold mb-4">Demander un retour</h2>

            <div className="mb-4">
              <p className="text-sm text-gray-600 mb-2">
                Commande #{selectedOrder.id.slice(-8)}
              </p>
              <p className="text-sm text-gray-600">
                Montant : <strong>${selectedOrder.totalAmount.toFixed(2)}</strong>
              </p>
            </div>

            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Raison du retour <span className="text-red-500">*</span>
              </label>
              <textarea
                value={returnReason}
                onChange={(e) => setReturnReason(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                rows={4}
                placeholder="Veuillez expliquer la raison de votre retour..."
                required
              />
            </div>

            <div className="bg-blue-50 p-3 rounded-md mb-4">
              <p className="text-xs text-blue-700">
                <strong>Information :</strong> Conformément au droit de rétractation, vous disposez de 14 jours
                à compter de la réception de votre commande pour nous retourner les articles.
              </p>
            </div>

            <div className="flex gap-3">
              <button
                onClick={() => {
                  setShowReturnModal(false)
                  setSelectedOrder(null)
                  setReturnReason('')
                }}
                className="flex-1 px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-50"
                disabled={submitting}
              >
                Annuler
              </button>
              <button
                onClick={submitReturn}
                disabled={!returnReason.trim() || submitting}
                className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed"
              >
                {submitting ? 'Envoi...' : 'Confirmer le retour'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
