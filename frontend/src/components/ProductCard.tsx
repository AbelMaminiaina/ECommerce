import { Link } from 'react-router-dom'
import { FiShoppingCart } from 'react-icons/fi'
import { Product } from '../types'
import useCartStore from '../stores/cartStore'
import { useState } from 'react'

interface ProductCardProps {
  product: Product
}

export default function ProductCard({ product }: ProductCardProps) {
  const { addToCart } = useCartStore()
  const [loading, setLoading] = useState(false)

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.preventDefault()
    setLoading(true)
    await addToCart(product.id, 1)
    setLoading(false)
  }

  return (
    <Link
      to={`/products/${product.id}`}
      className="group bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition-shadow duration-300"
    >
      <div className="aspect-square overflow-hidden bg-gray-100">
        <img
          src={product.images[0] || '/placeholder.jpg'}
          alt={product.name}
          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
        />
      </div>

      <div className="p-4">
        <h3 className="font-semibold text-lg mb-2 line-clamp-2">{product.name}</h3>
        <p className="text-gray-600 text-sm mb-4 line-clamp-2">{product.description}</p>

        <div className="flex items-center justify-between">
          <span className="text-2xl font-bold text-blue-600">${product.price.toFixed(2)}</span>

          <button
            onClick={handleAddToCart}
            disabled={loading || product.stock === 0}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition"
          >
            <FiShoppingCart />
            {loading ? 'Ajout...' : 'Ajouter'}
          </button>
        </div>

        {product.stock === 0 && (
          <p className="text-red-500 text-sm mt-2">Rupture de stock</p>
        )}
        {product.stock > 0 && product.stock < 10 && (
          <p className="text-orange-500 text-sm mt-2">Plus que {product.stock} en stock!</p>
        )}
      </div>
    </Link>
  )
}
