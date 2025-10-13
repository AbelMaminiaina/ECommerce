import { useEffect, useState } from 'react'
import api from '../../services/api'

enum WarrantyClaimStatus {
  Submitted = 0,
  UnderReview = 1,
  Approved = 2,
  Rejected = 3,
  Resolved = 4,
}

interface WarrantyClaim {
  id: string
  orderId: string
  productId: string
  productName: string
  userId: string
  purchaseDate: string
  warrantyExpirationDate: string
  issueDescription: string
  photos: string[]
  status: WarrantyClaimStatus
  resolution?: string
  adminNotes?: string
  createdAt: string
  resolvedAt?: string
  isUnderWarranty: boolean
}

export default function AdminWarranty() {
  const [claims, setClaims] = useState<WarrantyClaim[]>([])
  const [selectedClaim, setSelectedClaim] = useState<WarrantyClaim | null>(null)
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [resolution, setResolution] = useState('')
  const [adminNotes, setAdminNotes] = useState('')

  useEffect(() => {
    fetchClaims()
  }, [])

  const fetchClaims = async () => {
    try {
      const response = await api.get<WarrantyClaim[]>('/warranty/claims/admin/all')
      setClaims(response.data)
    } catch (error) {
      console.error('Failed to fetch warranty claims:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleUpdateClaim = async (status: WarrantyClaimStatus) => {
    if (!selectedClaim) return

    try {
      await api.put(`/warranty/claims/${selectedClaim.id}`, {
        status,
        resolution: resolution || undefined,
        adminNotes: adminNotes || undefined,
      })
      alert('Réclamation mise à jour avec succès')
      setShowModal(false)
      setResolution('')
      setAdminNotes('')
      fetchClaims()
    } catch (error) {
      alert('Erreur lors de la mise à jour de la réclamation')
    }
  }

  const getStatusColor = (status: WarrantyClaimStatus) => {
    const colors = {
      [WarrantyClaimStatus.Submitted]: 'bg-yellow-100 text-yellow-800',
      [WarrantyClaimStatus.UnderReview]: 'bg-blue-100 text-blue-800',
      [WarrantyClaimStatus.Approved]: 'bg-green-100 text-green-800',
      [WarrantyClaimStatus.Rejected]: 'bg-red-100 text-red-800',
      [WarrantyClaimStatus.Resolved]: 'bg-green-100 text-green-800',
    }
    return colors[status] || 'bg-gray-100 text-gray-800'
  }

  const getStatusText = (status: WarrantyClaimStatus) => {
    const statusNames = {
      [WarrantyClaimStatus.Submitted]: 'Soumise',
      [WarrantyClaimStatus.UnderReview]: 'En examen',
      [WarrantyClaimStatus.Approved]: 'Approuvée',
      [WarrantyClaimStatus.Rejected]: 'Rejetée',
      [WarrantyClaimStatus.Resolved]: 'Résolue',
    }
    return statusNames[status] || 'Inconnu'
  }

  const calculateRemainingMonths = (expirationDate: string) => {
    const expiration = new Date(expirationDate)
    const today = new Date()
    const diffTime = expiration.getTime() - today.getTime()
    const diffMonths = Math.ceil(diffTime / (1000 * 60 * 60 * 24 * 30))
    return diffMonths
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
      <h1 className="text-3xl font-bold mb-8">Garantie légale de conformité (2 ans)</h1>

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Produit</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date achat</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Expiration</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Statut garantie</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Statut réclamation</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {claims.length === 0 ? (
              <tr>
                <td colSpan={7} className="px-6 py-8 text-center text-gray-500">
                  Aucune réclamation de garantie
                </td>
              </tr>
            ) : (
              claims.map((claim) => {
                const remainingMonths = calculateRemainingMonths(claim.warrantyExpirationDate)
                return (
                  <tr key={claim.id}>
                    <td className="px-6 py-4 font-mono text-sm">#{claim.id.slice(-8)}</td>
                    <td className="px-6 py-4 text-sm max-w-xs truncate">{claim.productName}</td>
                    <td className="px-6 py-4 text-sm">
                      {new Date(claim.purchaseDate).toLocaleDateString('fr-FR')}
                    </td>
                    <td className="px-6 py-4 text-sm">
                      <div>{new Date(claim.warrantyExpirationDate).toLocaleDateString('fr-FR')}</div>
                      {claim.isUnderWarranty && remainingMonths > 0 && (
                        <div className="text-xs text-green-600">{remainingMonths} mois restants</div>
                      )}
                      {!claim.isUnderWarranty && (
                        <div className="text-xs text-red-600">Expirée</div>
                      )}
                    </td>
                    <td className="px-6 py-4">
                      {claim.isUnderWarranty ? (
                        <span className="px-2 py-1 bg-green-100 text-green-800 text-xs rounded-full font-semibold">
                          Valide
                        </span>
                      ) : (
                        <span className="px-2 py-1 bg-red-100 text-red-800 text-xs rounded-full font-semibold">
                          Expirée
                        </span>
                      )}
                    </td>
                    <td className="px-6 py-4">
                      <span className={`px-2 py-1 rounded-full text-xs font-semibold ${getStatusColor(claim.status)}`}>
                        {getStatusText(claim.status)}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right">
                      <button
                        onClick={() => {
                          setSelectedClaim(claim)
                          setResolution(claim.resolution || '')
                          setAdminNotes(claim.adminNotes || '')
                          setShowModal(true)
                        }}
                        className="text-sm px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700"
                      >
                        Traiter
                      </button>
                    </td>
                  </tr>
                )
              })
            )}
          </tbody>
        </table>
      </div>

      {/* Modal pour traiter la réclamation */}
      {showModal && selectedClaim && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full mx-4 max-h-[80vh] overflow-y-auto">
            <h2 className="text-2xl font-bold mb-4">Réclamation de garantie</h2>

            <div className="mb-4">
              <p className="text-sm text-gray-600 mb-2">
                <strong>Produit:</strong> {selectedClaim.productName}
              </p>
              <p className="text-sm text-gray-600 mb-2">
                <strong>Commande:</strong> #{selectedClaim.orderId.slice(-8)}
              </p>
              <p className="text-sm text-gray-600 mb-2">
                <strong>Date d'achat:</strong> {new Date(selectedClaim.purchaseDate).toLocaleDateString('fr-FR')}
              </p>
              <p className="text-sm text-gray-600 mb-2">
                <strong>Expiration garantie:</strong> {new Date(selectedClaim.warrantyExpirationDate).toLocaleDateString('fr-FR')}
              </p>
              <p className="text-sm text-gray-600 mb-2">
                <strong>Description du problème:</strong>
              </p>
              <p className="text-sm bg-gray-50 p-3 rounded mb-4">{selectedClaim.issueDescription}</p>

              {selectedClaim.photos && selectedClaim.photos.length > 0 && (
                <div className="mb-4">
                  <p className="text-sm font-semibold mb-2">Photos jointes:</p>
                  <div className="grid grid-cols-3 gap-2">
                    {selectedClaim.photos.map((photo, idx) => (
                      <img key={idx} src={photo} alt={`Photo ${idx + 1}`} className="w-full h-24 object-cover rounded" />
                    ))}
                  </div>
                </div>
              )}
            </div>

            <div className="mb-4">
              <label className="block text-sm font-medium mb-2">Résolution</label>
              <textarea
                value={resolution}
                onChange={(e) => setResolution(e.target.value)}
                className="w-full border border-gray-300 rounded p-2 text-sm"
                rows={3}
                placeholder="Réparation, remplacement, remboursement..."
              />
            </div>

            <div className="mb-4">
              <label className="block text-sm font-medium mb-2">Notes administrateur</label>
              <textarea
                value={adminNotes}
                onChange={(e) => setAdminNotes(e.target.value)}
                className="w-full border border-gray-300 rounded p-2 text-sm"
                rows={3}
                placeholder="Notes internes..."
              />
            </div>

            <div className="flex justify-between items-center">
              <div className="space-x-2">
                <button
                  onClick={() => handleUpdateClaim(WarrantyClaimStatus.UnderReview)}
                  className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                >
                  En examen
                </button>
                <button
                  onClick={() => handleUpdateClaim(WarrantyClaimStatus.Approved)}
                  className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
                >
                  Approuver
                </button>
                <button
                  onClick={() => handleUpdateClaim(WarrantyClaimStatus.Rejected)}
                  className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
                >
                  Rejeter
                </button>
                <button
                  onClick={() => handleUpdateClaim(WarrantyClaimStatus.Resolved)}
                  className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
                >
                  Résolu
                </button>
              </div>
              <button
                onClick={() => {
                  setShowModal(false)
                  setResolution('')
                  setAdminNotes('')
                }}
                className="px-4 py-2 border border-gray-300 rounded hover:bg-gray-50"
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
