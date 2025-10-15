import { useEffect, useState } from 'react'
import api from '../../services/api'
import { Package, PackageStatus, CarrierType, CreatePackageData, GenerateLabelResponse, Order } from '../../types'
import { FiPackage, FiTruck, FiPrinter, FiX, FiDownload, FiCheck } from 'react-icons/fi'

export default function AdminPackages() {
  const [packages, setPackages] = useState<Package[]>([])
  const [filteredPackages, setFilteredPackages] = useState<Package[]>([])
  const [orders, setOrders] = useState<Order[]>([])
  const [loading, setLoading] = useState(true)
  const [selectedPackage, setSelectedPackage] = useState<Package | null>(null)
  const [showDetailModal, setShowDetailModal] = useState(false)
  const [showCreateModal, setShowCreateModal] = useState(false)
  const [filterStatus, setFilterStatus] = useState<PackageStatus | 'all'>('all')
  const [newPackage, setNewPackage] = useState<CreatePackageData>({
    orderId: '',
    weight: 0,
    length: 0,
    width: 0,
    height: 0,
    carrier: CarrierType.Colissimo,
    notes: ''
  })

  useEffect(() => {
    fetchPackages()
    fetchOrders()
  }, [])

  useEffect(() => {
    filterPackages()
  }, [packages, filterStatus])

  const fetchPackages = async () => {
    try {
      // Fetch all packages
      const response = await api.get<Package[]>('/packages')
      setPackages(response.data)
    } catch (error) {
      console.error('Failed to fetch packages:', error)
    } finally {
      setLoading(false)
    }
  }

  const fetchOrders = async () => {
    try {
      const response = await api.get<Order[]>('/orders')
      // Filter orders that are paid and not cancelled
      const validOrders = response.data.filter(o =>
        o.paymentStatus === 1 && // Completed
        (o.status === 0 || o.status === 1) // Pending or Processing
      )
      setOrders(validOrders)
    } catch (error) {
      console.error('Failed to fetch orders:', error)
    }
  }

  const filterPackages = () => {
    let result = packages

    if (filterStatus !== 'all') {
      result = result.filter(p => p.status === filterStatus)
    }

    setFilteredPackages(result)
  }

  const handleCreatePackage = async () => {
    try {
      console.log('Sending package data:', newPackage)
      const response = await api.post('/packages', newPackage)
      console.log('Package created:', response.data)
      alert('Colis créé avec succès')
      setShowCreateModal(false)
      fetchPackages()
      // Reset form
      setNewPackage({
        orderId: '',
        weight: 0,
        length: 0,
        width: 0,
        height: 0,
        carrier: CarrierType.Colissimo,
        notes: ''
      })
    } catch (error: any) {
      console.error('Error creating package:', error)
      console.error('Error response:', error.response?.data)
      const errorMessage = error.response?.data?.message || error.message || 'Erreur inconnue'
      alert(`Erreur: ${errorMessage}`)
    }
  }

  const handleMarkAsPreparing = async (packageId: string) => {
    try {
      await api.post(`/packages/${packageId}/prepare`)
      alert('Colis marqué comme en préparation')
      fetchPackages()
    } catch (error) {
      alert('Erreur lors de la mise à jour')
    }
  }

  const handleGenerateLabel = async (packageId: string) => {
    try {
      const response = await api.post<GenerateLabelResponse>(`/packages/${packageId}/generate-label`)
      alert(`Étiquette générée ! Numéro de suivi : ${response.data.trackingNumber}`)
      fetchPackages()
    } catch (error) {
      alert('Erreur lors de la génération de l\'étiquette')
    }
  }

  const handleMarkAsShipped = async (packageId: string) => {
    if (!confirm('Confirmer l\'expédition ? Le client recevra une notification automatiquement.')) {
      return
    }

    try {
      await api.post(`/packages/${packageId}/ship`)
      alert('Colis expédié ! Notification envoyée au client.')
      fetchPackages()
    } catch (error) {
      alert('Erreur lors de l\'expédition')
    }
  }

  const handleDownloadLabel = async (packageId: string) => {
    try {
      const response = await api.get(`/packages/${packageId}/label`, {
        responseType: 'blob'
      })

      // Créer un lien de téléchargement
      const url = window.URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')
      link.href = url
      link.setAttribute('download', `etiquette_${packageId}.pdf`)
      document.body.appendChild(link)
      link.click()
      link.remove()
      window.URL.revokeObjectURL(url)
    } catch (error) {
      alert('Erreur lors du téléchargement de l\'étiquette')
    }
  }

  const getStatusColor = (status: PackageStatus) => {
    const colors = {
      [PackageStatus.Pending]: 'bg-yellow-100 text-yellow-800',
      [PackageStatus.Preparing]: 'bg-blue-100 text-blue-800',
      [PackageStatus.ReadyToShip]: 'bg-purple-100 text-purple-800',
      [PackageStatus.Shipped]: 'bg-green-100 text-green-800',
      [PackageStatus.Delivered]: 'bg-teal-100 text-teal-800',
      [PackageStatus.Exception]: 'bg-red-100 text-red-800',
      [PackageStatus.Returned]: 'bg-gray-100 text-gray-800',
    }
    return colors[status] || 'bg-gray-100 text-gray-800'
  }

  const getStatusText = (status: PackageStatus) => {
    const statusNames = {
      [PackageStatus.Pending]: 'En attente',
      [PackageStatus.Preparing]: 'En préparation',
      [PackageStatus.ReadyToShip]: 'Prêt à expédier',
      [PackageStatus.Shipped]: 'Expédié',
      [PackageStatus.Delivered]: 'Livré',
      [PackageStatus.Exception]: 'Problème',
      [PackageStatus.Returned]: 'Retourné',
    }
    return statusNames[status] || 'Inconnu'
  }

  const getCarrierText = (carrier: CarrierType) => {
    const carriers = {
      [CarrierType.LaPoste]: 'La Poste',
      [CarrierType.Colissimo]: 'Colissimo',
      [CarrierType.Chronopost]: 'Chronopost',
      [CarrierType.MondialRelay]: 'Mondial Relay',
      [CarrierType.DHL]: 'DHL',
      [CarrierType.UPS]: 'UPS',
      [CarrierType.FedEx]: 'FedEx',
    }
    return carriers[carrier] || 'Inconnu'
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
        <h1 className="text-3xl font-bold">Préparation des colis</h1>
        <button
          onClick={() => setShowCreateModal(true)}
          className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 flex items-center gap-2"
        >
          <FiPackage /> Nouveau colis
        </button>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-lg shadow p-4 mb-6">
        <div className="flex flex-wrap gap-2">
          <button
            onClick={() => setFilterStatus('all')}
            className={`px-3 py-1 rounded text-sm ${filterStatus === 'all' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'}`}
          >
            Tous ({packages.length})
          </button>
          <button
            onClick={() => setFilterStatus(PackageStatus.Pending)}
            className={`px-3 py-1 rounded text-sm ${filterStatus === PackageStatus.Pending ? 'bg-yellow-600 text-white' : 'bg-yellow-100 text-yellow-800'}`}
          >
            En attente ({packages.filter(p => p.status === PackageStatus.Pending).length})
          </button>
          <button
            onClick={() => setFilterStatus(PackageStatus.Preparing)}
            className={`px-3 py-1 rounded text-sm ${filterStatus === PackageStatus.Preparing ? 'bg-blue-600 text-white' : 'bg-blue-100 text-blue-800'}`}
          >
            En préparation ({packages.filter(p => p.status === PackageStatus.Preparing).length})
          </button>
          <button
            onClick={() => setFilterStatus(PackageStatus.ReadyToShip)}
            className={`px-3 py-1 rounded text-sm ${filterStatus === PackageStatus.ReadyToShip ? 'bg-purple-600 text-white' : 'bg-purple-100 text-purple-800'}`}
          >
            Prêts à expédier ({packages.filter(p => p.status === PackageStatus.ReadyToShip).length})
          </button>
        </div>
      </div>

      {/* Packages Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full min-w-[1000px]">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Commande</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Dimensions</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Transporteur</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Statut</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Suivi</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {filteredPackages.length === 0 ? (
              <tr>
                <td colSpan={6} className="px-6 py-8 text-center text-gray-500">
                  Aucun colis {filterStatus !== 'all' ? 'avec ce statut' : ''}
                </td>
              </tr>
            ) : (
              filteredPackages.map((pkg) => (
                <tr key={pkg.id}>
                  <td className="px-6 py-4">
                    <div className="font-mono text-sm">#{pkg.orderId.slice(-8)}</div>
                    <div className="text-xs text-gray-500">{new Date(pkg.createdAt).toLocaleDateString('fr-FR')}</div>
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <div>{pkg.weight}kg</div>
                    <div className="text-xs text-gray-500">{pkg.length}×{pkg.width}×{pkg.height}cm</div>
                  </td>
                  <td className="px-6 py-4 text-sm">{getCarrierText(pkg.carrier)}</td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${getStatusColor(pkg.status)}`}>
                      {getStatusText(pkg.status)}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    {pkg.trackingNumber ? (
                      <div className="text-sm">
                        <div className="font-mono">{pkg.trackingNumber}</div>
                        {pkg.trackingNotificationSent && (
                          <div className="text-xs text-green-600 flex items-center gap-1">
                            <FiCheck size={12} /> Notifié
                          </div>
                        )}
                      </div>
                    ) : (
                      <span className="text-gray-400 text-sm">-</span>
                    )}
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex items-center justify-end gap-2">
                      {/* Action button based on status */}
                      {pkg.status === PackageStatus.Pending && (
                        <button
                          onClick={() => handleMarkAsPreparing(pkg.id)}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded"
                          title="Commencer la préparation"
                        >
                          <FiPackage />
                        </button>
                      )}
                      {/* Générer l'étiquette si pas encore fait */}
                      {!pkg.trackingNumber && (pkg.status === PackageStatus.Preparing || pkg.status === PackageStatus.Pending) && (
                        <button
                          onClick={() => handleGenerateLabel(pkg.id)}
                          className="p-2 text-purple-600 hover:bg-purple-50 rounded"
                          title="Générer l'étiquette"
                        >
                          <FiPrinter />
                        </button>
                      )}

                      {/* Télécharger l'étiquette si numéro de suivi existe */}
                      {pkg.trackingNumber && (
                        <button
                          onClick={() => handleDownloadLabel(pkg.id)}
                          className="p-2 bg-blue-600 text-white hover:bg-blue-700 rounded"
                          title="Télécharger l'étiquette PDF"
                        >
                          <FiDownload />
                        </button>
                      )}

                      {/* Expédier si prêt */}
                      {pkg.status === PackageStatus.ReadyToShip && pkg.trackingNumber && (
                        <button
                          onClick={() => handleMarkAsShipped(pkg.id)}
                          className="px-3 py-1 bg-green-600 text-white rounded hover:bg-green-700 text-sm flex items-center gap-1"
                          title="Marquer comme expédié"
                        >
                          <FiTruck size={14} /> Expédier
                        </button>
                      )}
                      <button
                        onClick={() => {
                          setSelectedPackage(pkg)
                          setShowDetailModal(true)
                        }}
                        className="p-2 text-gray-600 hover:bg-gray-50 rounded"
                        title="Voir les détails"
                      >
                        <FiPackage />
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

      {/* Create Package Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg p-6 max-w-md w-full">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-2xl font-bold">Créer un colis</h2>
              <button onClick={() => setShowCreateModal(false)} className="text-gray-500 hover:text-gray-700">
                <FiX size={24} />
              </button>
            </div>

            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">Commande</label>
                <select
                  value={newPackage.orderId}
                  onChange={(e) => setNewPackage({ ...newPackage, orderId: e.target.value })}
                  className="w-full border border-gray-300 rounded px-3 py-2"
                >
                  <option value="">Sélectionnez une commande...</option>
                  {orders.map(order => (
                    <option key={order.id} value={order.id}>
                      #{order.id.slice(-8)} - ${order.totalAmount.toFixed(2)} - {new Date(order.createdAt).toLocaleDateString('fr-FR')}
                    </option>
                  ))}
                </select>
                {orders.length === 0 && (
                  <p className="text-sm text-gray-500 mt-1">Aucune commande disponible (payées et non expédiées)</p>
                )}
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Poids (kg)</label>
                  <input
                    type="number"
                    step="0.1"
                    value={newPackage.weight}
                    onChange={(e) => setNewPackage({ ...newPackage, weight: parseFloat(e.target.value) })}
                    className="w-full border border-gray-300 rounded px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Transporteur</label>
                  <select
                    value={newPackage.carrier}
                    onChange={(e) => setNewPackage({ ...newPackage, carrier: parseInt(e.target.value) as CarrierType })}
                    className="w-full border border-gray-300 rounded px-3 py-2"
                  >
                    <option value={CarrierType.LaPoste}>La Poste</option>
                    <option value={CarrierType.Colissimo}>Colissimo</option>
                    <option value={CarrierType.Chronopost}>Chronopost</option>
                    <option value={CarrierType.MondialRelay}>Mondial Relay</option>
                    <option value={CarrierType.DHL}>DHL</option>
                    <option value={CarrierType.UPS}>UPS</option>
                    <option value={CarrierType.FedEx}>FedEx</option>
                  </select>
                </div>
              </div>

              <div className="grid grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Longueur (cm)</label>
                  <input
                    type="number"
                    value={newPackage.length}
                    onChange={(e) => setNewPackage({ ...newPackage, length: parseFloat(e.target.value) })}
                    className="w-full border border-gray-300 rounded px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Largeur (cm)</label>
                  <input
                    type="number"
                    value={newPackage.width}
                    onChange={(e) => setNewPackage({ ...newPackage, width: parseFloat(e.target.value) })}
                    className="w-full border border-gray-300 rounded px-3 py-2"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Hauteur (cm)</label>
                  <input
                    type="number"
                    value={newPackage.height}
                    onChange={(e) => setNewPackage({ ...newPackage, height: parseFloat(e.target.value) })}
                    className="w-full border border-gray-300 rounded px-3 py-2"
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Notes (optionnel)</label>
                <textarea
                  value={newPackage.notes}
                  onChange={(e) => setNewPackage({ ...newPackage, notes: e.target.value })}
                  className="w-full border border-gray-300 rounded px-3 py-2"
                  rows={3}
                />
              </div>
            </div>

            <div className="flex justify-end gap-2 mt-6">
              <button
                onClick={() => setShowCreateModal(false)}
                className="px-4 py-2 bg-gray-200 text-gray-700 rounded hover:bg-gray-300"
              >
                Annuler
              </button>
              <button
                onClick={handleCreatePackage}
                className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
              >
                Créer
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Package Detail Modal */}
      {showDetailModal && selectedPackage && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-2xl font-bold">Détails du colis</h2>
              <button onClick={() => setShowDetailModal(false)} className="text-gray-500 hover:text-gray-700">
                <FiX size={24} />
              </button>
            </div>

            <div className="space-y-6">
              {/* Package Info */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-gray-600">Commande</p>
                  <p className="font-mono">#{selectedPackage.orderId}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Statut</p>
                  <span className={`inline-block px-3 py-1 rounded-full text-sm font-semibold ${getStatusColor(selectedPackage.status)}`}>
                    {getStatusText(selectedPackage.status)}
                  </span>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Transporteur</p>
                  <p className="font-semibold">{getCarrierText(selectedPackage.carrier)}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Poids</p>
                  <p className="font-semibold">{selectedPackage.weight} kg</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Dimensions</p>
                  <p className="font-semibold">{selectedPackage.length} × {selectedPackage.width} × {selectedPackage.height} cm</p>
                </div>
                {selectedPackage.trackingNumber && (
                  <div>
                    <p className="text-sm text-gray-600">Numéro de suivi</p>
                    <p className="font-mono">{selectedPackage.trackingNumber}</p>
                  </div>
                )}
              </div>

              {/* Shipping Address */}
              <div>
                <h3 className="font-semibold text-lg mb-2">Adresse de livraison</h3>
                <div className="bg-gray-50 p-4 rounded">
                  <p>{selectedPackage.shippingAddress.street}</p>
                  <p>{selectedPackage.shippingAddress.city}, {selectedPackage.shippingAddress.state} {selectedPackage.shippingAddress.zipCode}</p>
                  <p>{selectedPackage.shippingAddress.country}</p>
                </div>
              </div>

              {/* Notes */}
              {selectedPackage.notes && (
                <div>
                  <h3 className="font-semibold text-lg mb-2">Notes</h3>
                  <div className="bg-yellow-50 p-4 rounded">
                    <p>{selectedPackage.notes}</p>
                  </div>
                </div>
              )}

              {/* Label */}
              {selectedPackage.trackingNumber && (
                <div>
                  <button
                    onClick={() => handleDownloadLabel(selectedPackage.id)}
                    className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                  >
                    <FiDownload /> Télécharger l'étiquette PDF
                  </button>
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
