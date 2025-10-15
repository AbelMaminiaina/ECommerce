export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  phoneNumber: string
  addresses: Address[]
  role: string
  isActive?: boolean
  createdAt?: string
}

export interface Address {
  street: string
  city: string
  state: string
  zipCode: string
  country: string
  isDefault: boolean
}

export interface Product {
  id: string
  name: string
  description: string
  price: number
  stock: number
  categoryId: string
  images: string[]
  isFeatured: boolean
  specifications: Record<string, string>
}

export interface Category {
  id: string
  name: string
  description: string
  parentCategoryId?: string
  imageUrl: string
}

export interface CartItem {
  productId: string
  productName: string
  price: number
  quantity: number
  subtotal: number
}

export interface Cart {
  id: string
  userId: string
  items: CartItem[]
  totalAmount: number
}

export interface OrderItem {
  productId: string
  productName: string
  quantity: number
  price: number
}

export interface Order {
  id: string
  userId: string
  items: OrderItem[]
  totalAmount: number
  status: OrderStatus
  shippingAddress: Address
  paymentStatus: PaymentStatus
  createdAt: string
  deliveredAt?: string
  returnRequestedAt?: string
  returnReason?: string
  returnStatus: ReturnStatus
  returnDeadline?: string
  canReturn: boolean
  estimatedDeliveryDate?: string
  shippedAt?: string
  trackingNumber?: string
  carrierName?: string
  estimatedDeliveryDays: number
  isDeliveryDelayed: boolean
}

export enum OrderStatus {
  Pending = 0,
  Processing = 1,
  Shipped = 2,
  Delivered = 3,
  Cancelled = 4,
  ReturnRequested = 5,
  Returned = 6,
}

export enum PaymentStatus {
  Pending = 0,
  Completed = 1,
  Failed = 2,
  Refunded = 3,
}

export enum ReturnStatus {
  None = 0,
  Requested = 1,
  Approved = 2,
  InTransit = 3,
  Received = 4,
  Refunded = 5,
  Rejected = 6,
}

export interface AuthResponse {
  accessToken: string
  refreshToken: string
  user: User
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterData {
  email: string
  password: string
  firstName: string
  lastName: string
  phoneNumber: string
}

export interface CreateOrderData {
  items: { productId: string; quantity: number }[]
  shippingAddress: Address
}

export interface PaymentIntentResponse {
  clientSecret: string
  paymentIntentId: string
}

export interface TrackingInfo {
  trackingNumber?: string
  carrierName?: string
  shippedAt?: string
  estimatedDeliveryDate?: string
  deliveredAt?: string
  isDelayed: boolean
}

export interface ShippingMethod {
  id: string
  name: string
  description: string
  price: number
  minDeliveryDays: number
  maxDeliveryDays: number
  carrierName: string
  isActive: boolean
}

export interface Package {
  id: string
  orderId: string
  weight: number
  length: number
  width: number
  height: number
  status: PackageStatus
  carrier: CarrierType
  trackingNumber?: string
  labelUrl?: string
  preparedAt?: string
  shippedAt?: string
  shippingAddress: Address
  pickupPointName?: string
  trackingNotificationSent: boolean
  notes?: string
  createdAt: string
}

export enum PackageStatus {
  Pending = 0,
  Preparing = 1,
  ReadyToShip = 2,
  Shipped = 3,
  Delivered = 4,
  Exception = 5,
  Returned = 6,
}

export enum CarrierType {
  LaPoste = 0,
  Colissimo = 1,
  Chronopost = 2,
  MondialRelay = 3,
  DHL = 4,
  UPS = 5,
  FedEx = 6,
}

export interface CreatePackageData {
  orderId: string
  weight: number
  length: number
  width: number
  height: number
  carrier: CarrierType
  pickupPointId?: string
  notes?: string
}

export interface GenerateLabelResponse {
  trackingNumber: string
  labelUrl: string
  estimatedDeliveryDate: string
}
