import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { supportService } from '../services/supportService'
import { Ticket, TicketCategory, getCategoryLabel, getStatusLabel, getStatusColor } from '../types/support'

export default function Support() {
  const [tickets, setTickets] = useState<Ticket[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [showCreateModal, setShowCreateModal] = useState(false)

  useEffect(() => {
    loadTickets()
  }, [])

  const loadTickets = async () => {
    try {
      setLoading(true)
      const data = await supportService.getMyTickets()
      setTickets(data)
      setError('')
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erreur lors du chargement des tickets')
    } finally {
      setLoading(false)
    }
  }

  const handleTicketCreated = () => {
    setShowCreateModal(false)
    loadTickets()
  }

  if (loading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600"></div>
        </div>
      </div>
    )
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      {/* Header */}
      <div className="mb-8">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Support & Réclamations</h1>
            <p className="mt-2 text-gray-600">
              Gérez vos tickets de support et réclamations
            </p>
          </div>
          <button
            onClick={() => setShowCreateModal(true)}
            className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
          >
            <svg className="mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
            </svg>
            Nouvelle réclamation
          </button>
        </div>
      </div>

      {/* Error Message */}
      {error && (
        <div className="mb-6 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded relative">
          {error}
        </div>
      )}

      {/* Info Box */}
      <div className="mb-8 bg-blue-50 border border-blue-200 rounded-lg p-4">
        <div className="flex">
          <svg className="h-5 w-5 text-blue-400 mr-3 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
            <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
          </svg>
          <div className="text-sm text-blue-700">
            <p className="font-medium">Besoin d'aide ?</p>
            <p className="mt-1">
              Créez un ticket pour toute réclamation concernant vos colis, produits ou commandes.
              Notre équipe vous répondra dans les plus brefs délais.
            </p>
          </div>
        </div>
      </div>

      {/* Tickets List */}
      {tickets.length === 0 ? (
        <div className="text-center py-12 bg-gray-50 rounded-lg">
          <svg
            className="mx-auto h-12 w-12 text-gray-400"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z"
            />
          </svg>
          <h3 className="mt-2 text-sm font-medium text-gray-900">Aucun ticket</h3>
          <p className="mt-1 text-sm text-gray-500">
            Vous n'avez pas encore créé de ticket de support.
          </p>
          <div className="mt-6">
            <button
              onClick={() => setShowCreateModal(true)}
              className="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700"
            >
              Créer votre premier ticket
            </button>
          </div>
        </div>
      ) : (
        <div className="bg-white shadow overflow-hidden sm:rounded-lg">
          <ul className="divide-y divide-gray-200">
            {tickets.map((ticket) => (
              <li key={ticket.id}>
                <Link
                  to={`/support/${ticket.id}`}
                  className="block hover:bg-gray-50 transition duration-150"
                >
                  <div className="px-4 py-5 sm:px-6">
                    <div className="flex items-center justify-between">
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center space-x-3">
                          <h3 className="text-lg font-medium text-gray-900 truncate">
                            {ticket.subject}
                          </h3>
                          <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(ticket.status)}`}>
                            {getStatusLabel(ticket.status)}
                          </span>
                        </div>
                        <div className="mt-2 flex items-center text-sm text-gray-500 space-x-4">
                          <span className="flex items-center">
                            <svg className="flex-shrink-0 mr-1.5 h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
                              <path fillRule="evenodd" d="M17.707 9.293a1 1 0 010 1.414l-7 7a1 1 0 01-1.414 0l-7-7A.997.997 0 012 10V5a3 3 0 013-3h5c.256 0 .512.098.707.293l7 7zM5 6a1 1 0 100-2 1 1 0 000 2z" clipRule="evenodd" />
                            </svg>
                            {getCategoryLabel(ticket.category)}
                          </span>
                          <span className="flex items-center">
                            <svg className="flex-shrink-0 mr-1.5 h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
                              <path fillRule="evenodd" d="M18 10c0 3.866-3.582 7-8 7a8.841 8.841 0 01-4.083-.98L2 17l1.338-3.123C2.493 12.767 2 11.434 2 10c0-3.866 3.582-7 8-7s8 3.134 8 7zM7 9H5v2h2V9zm8 0h-2v2h2V9zM9 9h2v2H9V9z" clipRule="evenodd" />
                            </svg>
                            {ticket.messages.length} message{ticket.messages.length > 1 ? 's' : ''}
                          </span>
                          <span className="flex items-center">
                            <svg className="flex-shrink-0 mr-1.5 h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
                              <path fillRule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clipRule="evenodd" />
                            </svg>
                            {new Date(ticket.createdAt).toLocaleDateString('fr-FR')}
                          </span>
                        </div>
                        <p className="mt-2 text-sm text-gray-600 line-clamp-2">
                          {ticket.description}
                        </p>
                      </div>
                      <div className="ml-5 flex-shrink-0">
                        <svg className="h-5 w-5 text-gray-400" fill="currentColor" viewBox="0 0 20 20">
                          <path fillRule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clipRule="evenodd" />
                        </svg>
                      </div>
                    </div>
                  </div>
                </Link>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Create Ticket Modal */}
      {showCreateModal && (
        <CreateTicketModal
          onClose={() => setShowCreateModal(false)}
          onSuccess={handleTicketCreated}
        />
      )}
    </div>
  )
}

// Modal pour créer un nouveau ticket
function CreateTicketModal({ onClose, onSuccess }: { onClose: () => void; onSuccess: () => void }) {
  const [formData, setFormData] = useState({
    subject: '',
    description: '',
    category: TicketCategory.Delivery,
    orderId: ''
  })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!formData.subject || !formData.description) {
      setError('Veuillez remplir tous les champs obligatoires')
      return
    }

    try {
      setLoading(true)
      setError('')
      await supportService.createTicket({
        subject: formData.subject,
        description: formData.description,
        category: formData.category,
        orderId: formData.orderId || null
      })
      onSuccess()
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erreur lors de la création du ticket')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="fixed z-10 inset-0 overflow-y-auto">
      <div className="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" onClick={onClose}></div>

        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
          <form onSubmit={handleSubmit}>
            <div className="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
              <div className="mb-4">
                <h3 className="text-lg font-medium text-gray-900">Nouvelle réclamation</h3>
                <p className="mt-1 text-sm text-gray-500">
                  Décrivez votre problème et nous vous répondrons rapidement.
                </p>
              </div>

              {error && (
                <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
                  {error}
                </div>
              )}

              <div className="space-y-4">
                <div>
                  <label htmlFor="category" className="block text-sm font-medium text-gray-700">
                    Catégorie *
                  </label>
                  <select
                    id="category"
                    value={formData.category}
                    onChange={(e) => setFormData({ ...formData, category: Number(e.target.value) as TicketCategory })}
                    className="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md"
                    required
                  >
                    <option value={TicketCategory.Delivery}>📦 Livraison (colis perdu, retardé)</option>
                    <option value={TicketCategory.Order}>🛒 Commande</option>
                    <option value={TicketCategory.Product}>📱 Produit (défectueux)</option>
                    <option value={TicketCategory.Return}>↩️ Retour/Remboursement</option>
                    <option value={TicketCategory.Payment}>💳 Paiement</option>
                    <option value={TicketCategory.Technical}>⚙️ Technique</option>
                    <option value={TicketCategory.Other}>❓ Autre</option>
                  </select>
                </div>

                <div>
                  <label htmlFor="subject" className="block text-sm font-medium text-gray-700">
                    Sujet *
                  </label>
                  <input
                    type="text"
                    id="subject"
                    value={formData.subject}
                    onChange={(e) => setFormData({ ...formData, subject: e.target.value })}
                    placeholder="Ex: Colis non reçu"
                    className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    required
                  />
                </div>

                <div>
                  <label htmlFor="orderId" className="block text-sm font-medium text-gray-700">
                    Numéro de commande (optionnel)
                  </label>
                  <input
                    type="text"
                    id="orderId"
                    value={formData.orderId}
                    onChange={(e) => setFormData({ ...formData, orderId: e.target.value })}
                    placeholder="Ex: 12345..."
                    className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  />
                </div>

                <div>
                  <label htmlFor="description" className="block text-sm font-medium text-gray-700">
                    Description détaillée *
                  </label>
                  <textarea
                    id="description"
                    rows={4}
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    placeholder="Décrivez votre problème en détail. N'oubliez pas d'inclure le numéro de suivi si c'est un problème de livraison."
                    className="mt-1 block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    required
                  />
                </div>
              </div>
            </div>

            <div className="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
              <button
                type="submit"
                disabled={loading}
                className="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-indigo-600 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:ml-3 sm:w-auto sm:text-sm disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading ? 'Création...' : 'Créer le ticket'}
              </button>
              <button
                type="button"
                onClick={onClose}
                disabled={loading}
                className="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:mt-0 sm:w-auto sm:text-sm disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Annuler
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}
