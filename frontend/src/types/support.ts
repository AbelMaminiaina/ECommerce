// Types pour le système de support

export enum TicketCategory {
  Order = 0,
  Product = 1,
  Delivery = 2,
  Return = 3,
  Technical = 4,
  Payment = 5,
  Other = 6
}

export enum TicketStatus {
  Open = 0,
  InProgress = 1,
  WaitingCustomer = 2,
  Resolved = 3,
  Closed = 4
}

export enum TicketPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Urgent = 3
}

export interface TicketMessage {
  senderId: string
  senderName: string
  isFromAdmin: boolean
  message: string
  attachments: string[]
  createdAt: string
}

export interface Ticket {
  id: string
  userId: string
  orderId?: string | null
  subject: string
  description: string
  category: TicketCategory
  status: TicketStatus
  priority: TicketPriority
  messages: TicketMessage[]
  assignedToAdminId?: string | null
  createdAt: string
  closedAt?: string | null
}

export interface CreateTicketDto {
  orderId?: string | null
  subject: string
  description: string
  category: TicketCategory
}

export interface AddMessageDto {
  message: string
  attachments?: string[]
}

// Helper functions
export const getCategoryLabel = (category: TicketCategory): string => {
  switch (category) {
    case TicketCategory.Order: return 'Commande'
    case TicketCategory.Product: return 'Produit'
    case TicketCategory.Delivery: return 'Livraison'
    case TicketCategory.Return: return 'Retour/Remboursement'
    case TicketCategory.Technical: return 'Technique'
    case TicketCategory.Payment: return 'Paiement'
    case TicketCategory.Other: return 'Autre'
    default: return 'Inconnu'
  }
}

export const getStatusLabel = (status: TicketStatus): string => {
  switch (status) {
    case TicketStatus.Open: return 'Ouvert'
    case TicketStatus.InProgress: return 'En cours'
    case TicketStatus.WaitingCustomer: return 'En attente client'
    case TicketStatus.Resolved: return 'Résolu'
    case TicketStatus.Closed: return 'Fermé'
    default: return 'Inconnu'
  }
}

export const getPriorityLabel = (priority: TicketPriority): string => {
  switch (priority) {
    case TicketPriority.Low: return 'Basse'
    case TicketPriority.Medium: return 'Moyenne'
    case TicketPriority.High: return 'Haute'
    case TicketPriority.Urgent: return 'Urgente'
    default: return 'Inconnu'
  }
}

export const getStatusColor = (status: TicketStatus): string => {
  switch (status) {
    case TicketStatus.Open: return 'bg-blue-100 text-blue-800'
    case TicketStatus.InProgress: return 'bg-yellow-100 text-yellow-800'
    case TicketStatus.WaitingCustomer: return 'bg-orange-100 text-orange-800'
    case TicketStatus.Resolved: return 'bg-green-100 text-green-800'
    case TicketStatus.Closed: return 'bg-gray-100 text-gray-800'
    default: return 'bg-gray-100 text-gray-800'
  }
}

export const getPriorityColor = (priority: TicketPriority): string => {
  switch (priority) {
    case TicketPriority.Low: return 'text-gray-600'
    case TicketPriority.Medium: return 'text-blue-600'
    case TicketPriority.High: return 'text-orange-600'
    case TicketPriority.Urgent: return 'text-red-600'
    default: return 'text-gray-600'
  }
}
