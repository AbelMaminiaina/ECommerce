import { useEffect } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import useCartStore from '../stores/cartStore'
import useAuthStore from '../stores/authStore'
import { FiTrash2, FiMinus, FiPlus, FiShoppingBag } from 'react-icons/fi'

export default function Cart() {
  const navigate = useNavigate()
  const { isAuthenticated } = useAuthStore()
  const { cart, loading, fetchCart, updateCartItem, removeFromCart } = useCartStore()

  useEffect(() => {
    if (isAuthenticated) {
      fetchCart()
    }
  }, [isAuthenticated])

  const handleUpdateQuantity = async (productId: string, newQuantity: number) => {
    if (newQuantity < 1) {
      await removeFromCart(productId)
    } else {
      await updateCartItem(productId, newQuantity)
    }
  }

  const handleRemove = async (productId: string) => {
    if (confirm('Êtes-vous sûr de vouloir retirer ce produit?')) {
      await removeFromCart(productId)
    }
  }

  const handleCheckout = () => {
    if (!isAuthenticated) {
      navigate('/login?redirect=/checkout')
    } else {
      navigate('/checkout')
    }
  }

  if (!isAuthenticated) {
    return (
      <div className="container mx-auto px-4 py-16 text-center">
        <h2 className="text-2xl font-bold mb-4">Connectez-vous pour voir votre panier</h2>
        <Link to="/login" className="inline-block px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700">
          Se connecter
        </Link>
      </div>
    )
  }

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="animate-pulse">
          <div className="h-8 bg-gray-200 rounded w-48 mb-8"></div>
          <div className="space-y-4">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="h-32 bg-gray-200 rounded"></div>
            ))}
          </div>
        </div>
      </div>
    )
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div className="container mx-auto px-4 py-16 text-center">
        <FiShoppingBag size={80} className="mx-auto text-gray-300 mb-4" />
        <h2 className="text-2xl font-bold mb-4">Votre panier est vide</h2>
        <p className="text-gray-600 mb-8">Ajoutez des produits pour commencer vos achats</p>
        <Link
          to="/products"
          className="inline-block px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700"
        >
          Voir les produits
        </Link>
      </div>
    )
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-8">Mon Panier</h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Cart Items */}
        <div className="lg:col-span-2 space-y-4">
          {cart.items.map((item) => (
            <div key={item.productId} className="bg-white p-4 rounded-lg shadow flex gap-4">
              <div className="w-24 h-24 bg-gray-100 rounded flex-shrink-0">
                <img
                  src="/placeholder.jpg"
                  alt={item.productName}
                  className="w-full h-full object-cover rounded"
                />
              </div>

              <div className="flex-grow">
                <h3 className="font-semibold text-lg mb-2">{item.productName}</h3>
                <p className="text-blue-600 font-bold">${item.price.toFixed(2)}</p>
              </div>

              <div className="flex flex-col items-end gap-2">
                {/* Quantity Controls */}
                <div className="flex items-center gap-2 border border-gray-300 rounded">
                  <button
                    onClick={() => handleUpdateQuantity(item.productId, item.quantity - 1)}
                    className="p-2 hover:bg-gray-100"
                  >
                    <FiMinus size={16} />
                  </button>
                  <span className="w-12 text-center font-semibold">{item.quantity}</span>
                  <button
                    onClick={() => handleUpdateQuantity(item.productId, item.quantity + 1)}
                    className="p-2 hover:bg-gray-100"
                  >
                    <FiPlus size={16} />
                  </button>
                </div>

                {/* Subtotal */}
                <p className="font-semibold">${item.subtotal.toFixed(2)}</p>

                {/* Remove Button */}
                <button
                  onClick={() => handleRemove(item.productId)}
                  className="text-red-500 hover:text-red-700"
                >
                  <FiTrash2 size={20} />
                </button>
              </div>
            </div>
          ))}
        </div>

        {/* Order Summary */}
        <div className="lg:col-span-1">
          <div className="bg-white p-6 rounded-lg shadow sticky top-4">
            <h2 className="text-xl font-bold mb-4">Résumé de la commande</h2>

            <div className="space-y-3 mb-6">
              <div className="flex justify-between">
                <span className="text-gray-600">Sous-total</span>
                <span className="font-semibold">${cart.totalAmount.toFixed(2)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Livraison</span>
                <span className="font-semibold">Calculée à la caisse</span>
              </div>
              <div className="border-t pt-3 flex justify-between text-lg">
                <span className="font-bold">Total</span>
                <span className="font-bold text-blue-600">${cart.totalAmount.toFixed(2)}</span>
              </div>
            </div>

            <button
              onClick={handleCheckout}
              className="w-full px-6 py-3 bg-blue-600 text-white rounded-md font-semibold hover:bg-blue-700 transition"
            >
              Passer la commande
            </button>

            <Link
              to="/products"
              className="block text-center mt-4 text-blue-600 hover:text-blue-700"
            >
              Continuer mes achats
            </Link>
          </div>
        </div>
      </div>
    </div>
  )
}
