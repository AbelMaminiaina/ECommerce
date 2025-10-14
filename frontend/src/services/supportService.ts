import api from './api'
import { Ticket, CreateTicketDto, AddMessageDto } from '../types/support'

export const supportService = {
  // Obtenir tous mes tickets
  getMyTickets: async (): Promise<Ticket[]> => {
    const response = await api.get('/support/tickets')
    return response.data
  },

  // Obtenir un ticket spécifique
  getTicket: async (ticketId: string): Promise<Ticket> => {
    const response = await api.get(`/support/tickets/${ticketId}`)
    return response.data
  },

  // Créer un nouveau ticket
  createTicket: async (data: CreateTicketDto): Promise<Ticket> => {
    const response = await api.post('/support/tickets', data)
    return response.data
  },

  // Ajouter un message à un ticket
  addMessage: async (ticketId: string, data: AddMessageDto): Promise<Ticket> => {
    const response = await api.post(`/support/tickets/${ticketId}/messages`, data)
    return response.data
  }
}
