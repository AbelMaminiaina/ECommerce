import { useEffect, useState } from 'react'
import api from '../../services/api'

enum TicketCategory {
  Order = 0,
  Product = 1,
  Delivery = 2,
  Return = 3,
  Technical = 4,
  Payment = 5,
  Other = 6,
}

enum TicketStatus {
  Open = 0,
  InProgress = 1,
  WaitingCustomer = 2,
  Resolved = 3,
  Closed = 4,
}

enum TicketPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Urgent = 3,
}

interface TicketMessage {
  content: string
  isAdminReply: boolean
  createdAt: string
}

interface SupportTicket {
  id: string
  userId: string
  orderId?: string
  subject: string
  description: string
  category: TicketCategory
  status: TicketStatus
  priority: TicketPriority
  messages: TicketMessage[]
  assignedToAdminId?: string
  createdAt: string
  closedAt?: string
}

export default function AdminSupport() {
  const [tickets, setTickets] = useState<SupportTicket[]>([])
  const [selectedTicket, setSelectedTicket] = useState<SupportTicket | null>(null)
  const [loading, setLoading] = useState(true)
  const [replyMessage, setReplyMessage] = useState('')
  const [showModal, setShowModal] = useState(false)

  useEffect(() => {
    fetchTickets()
  }, [])

  const fetchTickets = async () => {
    try {
      const response = await api.get<SupportTicket[]>('/support/tickets/admin/all')
      setTickets(response.data)
    } catch (error) {
      console.error('Failed to fetch tickets:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleStatusChange = async (ticketId: string, newStatus: TicketStatus) => {
    try {
      await api.put(`/support/tickets/${ticketId}/status`, { status: newStatus })
      setTickets(tickets.map(t => t.id === ticketId ? { ...t, status: newStatus } : t))
      alert('Statut mis à jour avec succès')
    } catch (error) {
      alert('Erreur lors de la mise à jour du statut')
    }
  }

  const handleReply = async () => {
    if (!selectedTicket || !replyMessage.trim()) return

    try {
      await api.post(`/support/tickets/${selectedTicket.id}/messages`, { content: replyMessage })
      alert('Réponse envoyée avec succès')
      setReplyMessage('')
      setShowModal(false)
      fetchTickets()
    } catch (error) {
      alert('Erreur lors de l\'envoi de la réponse')
    }
  }

  const getStatusColor = (status: TicketStatus) => {
    const colors = {
      [TicketStatus.Open]: 'bg-red-100 text-red-800',
      [TicketStatus.InProgress]: 'bg-blue-100 text-blue-800',
      [TicketStatus.WaitingCustomer]: 'bg-yellow-100 text-yellow-800',
      [TicketStatus.Resolved]: 'bg-green-100 text-green-800',
      [TicketStatus.Closed]: 'bg-gray-100 text-gray-800',
    }
    return colors[status] || 'bg-gray-100 text-gray-800'
  }

  const getStatusText = (status: TicketStatus) => {
    const statusNames = {
      [TicketStatus.Open]: 'Ouvert',
      [TicketStatus.InProgress]: 'En cours',
      [TicketStatus.WaitingCustomer]: 'En attente client',
      [TicketStatus.Resolved]: 'Résolu',
      [TicketStatus.Closed]: 'Fermé',
    }
    return statusNames[status] || 'Inconnu'
  }

  const getPriorityColor = (priority: TicketPriority) => {
    const colors = {
      [TicketPriority.Low]: 'bg-gray-100 text-gray-800',
      [TicketPriority.Medium]: 'bg-blue-100 text-blue-800',
      [TicketPriority.High]: 'bg-orange-100 text-orange-800',
      [TicketPriority.Urgent]: 'bg-red-100 text-red-800',
    }
    return colors[priority] || 'bg-gray-100 text-gray-800'
  }

  const getPriorityText = (priority: TicketPriority) => {
    const priorityNames = {
      [TicketPriority.Low]: 'Basse',
      [TicketPriority.Medium]: 'Moyenne',
      [TicketPriority.High]: 'Haute',
      [TicketPriority.Urgent]: 'Urgente',
    }
    return priorityNames[priority] || 'Inconnu'
  }

  const getCategoryText = (category: TicketCategory) => {
    const categoryNames = {
      [TicketCategory.Order]: 'Commande',
      [TicketCategory.Product]: 'Produit',
      [TicketCategory.Delivery]: 'Livraison',
      [TicketCategory.Return]: 'Retour',
      [TicketCategory.Technical]: 'Technique',
      [TicketCategory.Payment]: 'Paiement',
      [TicketCategory.Other]: 'Autre',
    }
    return categoryNames[category] || 'Inconnu'
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
      <h1 className="text-3xl font-bold mb-8">Support client (SAV)</h1>

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">ID</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Sujet</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Catégorie</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Priorité</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Statut</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {tickets.length === 0 ? (
              <tr>
                <td colSpan={7} className="px-6 py-8 text-center text-gray-500">
                  Aucun ticket de support
                </td>
              </tr>
            ) : (
              tickets.map((ticket) => (
                <tr key={ticket.id}>
                  <td className="px-6 py-4 font-mono text-sm">#{ticket.id.slice(-8)}</td>
                  <td className="px-6 py-4 text-sm max-w-xs truncate font-medium">{ticket.subject}</td>
                  <td className="px-6 py-4 text-sm">
                    <span className="px-2 py-1 bg-gray-100 text-gray-800 text-xs rounded">
                      {getCategoryText(ticket.category)}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${getPriorityColor(ticket.priority)}`}>
                      {getPriorityText(ticket.priority)}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-semibold ${getStatusColor(ticket.status)}`}>
                      {getStatusText(ticket.status)}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm">
                    {new Date(ticket.createdAt).toLocaleDateString('fr-FR')}
                  </td>
                  <td className="px-6 py-4 text-right space-x-2">
                    <button
                      onClick={() => {
                        setSelectedTicket(ticket)
                        setShowModal(true)
                      }}
                      className="text-sm px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700"
                    >
                      Voir
                    </button>
                    <select
                      value={ticket.status}
                      onChange={(e) => handleStatusChange(ticket.id, parseInt(e.target.value) as TicketStatus)}
                      className="text-sm border border-gray-300 rounded px-2 py-1"
                    >
                      <option value={TicketStatus.Open}>Ouvert</option>
                      <option value={TicketStatus.InProgress}>En cours</option>
                      <option value={TicketStatus.WaitingCustomer}>En attente client</option>
                      <option value={TicketStatus.Resolved}>Résolu</option>
                      <option value={TicketStatus.Closed}>Fermé</option>
                    </select>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Modal pour voir et répondre au ticket */}
      {showModal && selectedTicket && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full mx-4 max-h-[80vh] overflow-y-auto">
            <h2 className="text-2xl font-bold mb-4">{selectedTicket.subject}</h2>

            <div className="mb-4">
              <p className="text-sm text-gray-600 mb-2">
                <strong>Catégorie:</strong> {getCategoryText(selectedTicket.category)}
              </p>
              <p className="text-sm text-gray-600 mb-2">
                <strong>Description:</strong>
              </p>
              <p className="text-sm bg-gray-50 p-3 rounded">{selectedTicket.description}</p>
            </div>

            {/* Messages */}
            <div className="mb-4">
              <h3 className="font-semibold mb-2">Messages</h3>
              <div className="space-y-2 max-h-64 overflow-y-auto">
                {selectedTicket.messages.map((msg, idx) => (
                  <div
                    key={idx}
                    className={`p-3 rounded ${msg.isAdminReply ? 'bg-blue-50 ml-8' : 'bg-gray-50 mr-8'}`}
                  >
                    <div className="flex items-center justify-between mb-1">
                      <span className="text-xs font-semibold">
                        {msg.isAdminReply ? 'Admin' : 'Client'}
                      </span>
                      <span className="text-xs text-gray-500">
                        {new Date(msg.createdAt).toLocaleString('fr-FR')}
                      </span>
                    </div>
                    <p className="text-sm">{msg.content}</p>
                  </div>
                ))}
              </div>
            </div>

            {/* Répondre */}
            <div className="mb-4">
              <label className="block text-sm font-medium mb-2">Répondre au client</label>
              <textarea
                value={replyMessage}
                onChange={(e) => setReplyMessage(e.target.value)}
                className="w-full border border-gray-300 rounded p-2 text-sm"
                rows={4}
                placeholder="Votre réponse..."
              />
            </div>

            <div className="flex justify-end gap-2">
              <button
                onClick={() => {
                  setShowModal(false)
                  setReplyMessage('')
                }}
                className="px-4 py-2 border border-gray-300 rounded hover:bg-gray-50"
              >
                Fermer
              </button>
              <button
                onClick={handleReply}
                className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
              >
                Envoyer la réponse
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
