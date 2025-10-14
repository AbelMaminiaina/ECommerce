import { useState, useEffect, useRef } from 'react'
import { useParams, Link, useNavigate } from 'react-router-dom'
import { supportService } from '../services/supportService'
import { Ticket, getCategoryLabel, getStatusLabel, getStatusColor, getPriorityLabel, getPriorityColor } from '../types/support'

export default function SupportTicketDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [ticket, setTicket] = useState<Ticket | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [message, setMessage] = useState('')
  const [sendingMessage, setSendingMessage] = useState(false)
  const messagesEndRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (id) {
      loadTicket()
    }
  }, [id])

  useEffect(() => {
    scrollToBottom()
  }, [ticket?.messages])

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }

  const loadTicket = async () => {
    if (!id) return

    try {
      setLoading(true)
      const data = await supportService.getTicket(id)
      setTicket(data)
      setError('')
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erreur lors du chargement du ticket')
      if (err.response?.status === 404 || err.response?.status === 403) {
        setTimeout(() => navigate('/support'), 3000)
      }
    } finally {
      setLoading(false)
    }
  }

  const handleSendMessage = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!message.trim() || !id) return

    try {
      setSendingMessage(true)
      const updatedTicket = await supportService.addMessage(id, {
        message: message.trim(),
        attachments: []
      })
      setTicket(updatedTicket)
      setMessage('')
    } catch (err: any) {
      alert(err.response?.data?.message || 'Erreur lors de l\'envoi du message')
    } finally {
      setSendingMessage(false)
    }
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

  if (error) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
          <svg className="mx-auto h-12 w-12 text-red-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
          <h3 className="text-lg font-medium text-red-900 mb-2">Erreur</h3>
          <p className="text-red-700">{error}</p>
          <Link
            to="/support"
            className="mt-4 inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-red-600 hover:bg-red-700"
          >
            Retour à mes tickets
          </Link>
        </div>
      </div>
    )
  }

  if (!ticket) {
    return null
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      {/* Breadcrumb */}
      <nav className="mb-6">
        <ol className="flex items-center space-x-2 text-sm text-gray-500">
          <li>
            <Link to="/support" className="hover:text-indigo-600">
              Mes tickets
            </Link>
          </li>
          <li>
            <svg className="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clipRule="evenodd" />
            </svg>
          </li>
          <li className="text-gray-900">{ticket.subject}</li>
        </ol>
      </nav>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Main Content - Messages */}
        <div className="lg:col-span-2">
          <div className="bg-white shadow rounded-lg overflow-hidden">
            {/* Header */}
            <div className="px-6 py-5 border-b border-gray-200">
              <h1 className="text-2xl font-bold text-gray-900">{ticket.subject}</h1>
              <p className="mt-2 text-gray-600">{ticket.description}</p>
            </div>

            {/* Messages */}
            <div className="px-6 py-6 space-y-6 max-h-[500px] overflow-y-auto">
              {ticket.messages.map((msg, index) => (
                <div key={index} className={`flex ${msg.isFromAdmin ? 'justify-start' : 'justify-end'}`}>
                  <div className={`max-w-[80%] ${msg.isFromAdmin ? 'order-2' : 'order-1'}`}>
                    <div className={`rounded-lg px-4 py-3 ${
                      msg.isFromAdmin
                        ? 'bg-gray-100 text-gray-900'
                        : 'bg-indigo-600 text-white'
                    }`}>
                      <div className="flex items-center space-x-2 mb-1">
                        {msg.isFromAdmin ? (
                          <>
                            <svg className="h-4 w-4 text-amber-500" fill="currentColor" viewBox="0 0 20 20">
                              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                            </svg>
                            <span className="text-xs font-semibold">{msg.senderName} (Support)</span>
                          </>
                        ) : (
                          <span className="text-xs font-semibold">{msg.senderName}</span>
                        )}
                      </div>
                      <p className="text-sm whitespace-pre-wrap">{msg.message}</p>
                      <p className={`text-xs mt-2 ${msg.isFromAdmin ? 'text-gray-500' : 'text-indigo-200'}`}>
                        {new Date(msg.createdAt).toLocaleString('fr-FR')}
                      </p>
                    </div>
                  </div>
                  <div className={`flex-shrink-0 ${msg.isFromAdmin ? 'order-1 mr-3' : 'order-2 ml-3'}`}>
                    <div className={`h-10 w-10 rounded-full flex items-center justify-center ${
                      msg.isFromAdmin ? 'bg-amber-100' : 'bg-indigo-100'
                    }`}>
                      {msg.isFromAdmin ? (
                        <svg className="h-6 w-6 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                        </svg>
                      ) : (
                        <svg className="h-6 w-6 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                        </svg>
                      )}
                    </div>
                  </div>
                </div>
              ))}
              <div ref={messagesEndRef} />
            </div>

            {/* Message Input */}
            <div className="px-6 py-4 bg-gray-50 border-t border-gray-200">
              <form onSubmit={handleSendMessage}>
                <div className="flex space-x-3">
                  <div className="flex-1">
                    <textarea
                      value={message}
                      onChange={(e) => setMessage(e.target.value)}
                      placeholder="Écrivez votre message..."
                      rows={3}
                      className="block w-full border border-gray-300 rounded-md shadow-sm py-2 px-3 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm resize-none"
                      disabled={sendingMessage}
                    />
                  </div>
                </div>
                <div className="mt-3 flex justify-end">
                  <button
                    type="submit"
                    disabled={sendingMessage || !message.trim()}
                    className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {sendingMessage ? (
                      <>
                        <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                        </svg>
                        Envoi...
                      </>
                    ) : (
                      <>
                        <svg className="mr-2 h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                        </svg>
                        Envoyer
                      </>
                    )}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>

        {/* Sidebar - Ticket Info */}
        <div className="lg:col-span-1">
          <div className="bg-white shadow rounded-lg overflow-hidden">
            <div className="px-6 py-5 border-b border-gray-200">
              <h3 className="text-lg font-medium text-gray-900">Informations du ticket</h3>
            </div>
            <div className="px-6 py-5 space-y-4">
              <div>
                <dt className="text-sm font-medium text-gray-500">Statut</dt>
                <dd className="mt-1">
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(ticket.status)}`}>
                    {getStatusLabel(ticket.status)}
                  </span>
                </dd>
              </div>

              <div>
                <dt className="text-sm font-medium text-gray-500">Priorité</dt>
                <dd className={`mt-1 text-sm font-semibold ${getPriorityColor(ticket.priority)}`}>
                  {getPriorityLabel(ticket.priority)}
                </dd>
              </div>

              <div>
                <dt className="text-sm font-medium text-gray-500">Catégorie</dt>
                <dd className="mt-1 text-sm text-gray-900">
                  {getCategoryLabel(ticket.category)}
                </dd>
              </div>

              {ticket.orderId && (
                <div>
                  <dt className="text-sm font-medium text-gray-500">Commande associée</dt>
                  <dd className="mt-1 text-sm text-gray-900">
                    <Link
                      to={`/orders`}
                      className="text-indigo-600 hover:text-indigo-900 hover:underline"
                    >
                      {ticket.orderId.substring(0, 8)}...
                    </Link>
                  </dd>
                </div>
              )}

              <div>
                <dt className="text-sm font-medium text-gray-500">Date de création</dt>
                <dd className="mt-1 text-sm text-gray-900">
                  {new Date(ticket.createdAt).toLocaleDateString('fr-FR', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                  })}
                </dd>
              </div>

              {ticket.closedAt && (
                <div>
                  <dt className="text-sm font-medium text-gray-500">Date de clôture</dt>
                  <dd className="mt-1 text-sm text-gray-900">
                    {new Date(ticket.closedAt).toLocaleDateString('fr-FR', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                      hour: '2-digit',
                      minute: '2-digit'
                    })}
                  </dd>
                </div>
              )}

              <div>
                <dt className="text-sm font-medium text-gray-500">Numéro de ticket</dt>
                <dd className="mt-1 text-xs text-gray-500 font-mono break-all">
                  {ticket.id}
                </dd>
              </div>
            </div>
          </div>

          {/* Help Box */}
          <div className="mt-6 bg-blue-50 border border-blue-200 rounded-lg p-4">
            <div className="flex">
              <svg className="h-5 w-5 text-blue-400 mr-3 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clipRule="evenodd" />
              </svg>
              <div className="text-sm text-blue-700">
                <p className="font-medium">Temps de réponse</p>
                <p className="mt-1">
                  Notre équipe répond généralement dans les 24 heures.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
