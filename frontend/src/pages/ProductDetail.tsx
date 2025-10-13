import { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import api from '../services/api'
import { Product } from '../types'
import useCartStore from '../stores/cartStore'
import { FiShoppingCart, FiMinus, FiPlus } from 'react-icons/fi'

export default function ProductDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { addToCart } = useCartStore()

  const [product, setProduct] = useState<Product | null>(null)
  const [loading, setLoading] = useState(true)
  const [selectedImage, setSelectedImage] = useState(0)
  const [quantity, setQuantity] = useState(1)
  const [addingToCart, setAddingToCart] = useState(false)

  useEffect(() => {
    if (id) {
      fetchProduct()
    }
  }, [id])

  const fetchProduct = async () => {
    try {
      const response = await api.get<Product>(`/products/${id}`)
      setProduct(response.data)
    } catch (error) {
      console.error('Failed to fetch product:', error)
      navigate('/products')
    } finally {
      setLoading(false)
    }
  }

  const handleAddToCart = async () => {
    if (!product) return

    setAddingToCart(true)
    const result = await addToCart(product.id, quantity)
    setAddingToCart(false)

    if (result.success) {
      alert('Produit ajouté au panier!')
    } else {
      alert(result.message || 'Erreur lors de l\'ajout au panier')
    }
  }

  const incrementQuantity = () => {
    if (product && quantity < product.stock) {
      setQuantity(quantity + 1)
    }
  }

  const decrementQuantity = () => {
    if (quantity > 1) {
      setQuantity(quantity - 1)
    }
  }

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="animate-pulse">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            <div className="bg-gray-200 h-96 rounded-lg"></div>
            <div>
              <div className="h-8 bg-gray-200 rounded mb-4"></div>
              <div className="h-4 bg-gray-200 rounded mb-2"></div>
              <div className="h-4 bg-gray-200 rounded mb-2"></div>
              <div className="h-4 bg-gray-200 rounded"></div>
            </div>
          </div>
        </div>
      </div>
    )
  }

  if (!product) {
    return null
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {/* Images Section */}
        <div>
          <div className="aspect-square rounded-lg overflow-hidden bg-gray-100 mb-4">
            <img
              src={product.images[selectedImage] || '/placeholder.jpg'}
              alt={product.name}
              className="w-full h-full object-cover"
            />
          </div>

          {product.images.length > 1 && (
            <div className="grid grid-cols-4 gap-4">
              {product.images.map((image, index) => (
                <button
                  key={index}
                  onClick={() => setSelectedImage(index)}
                  className={`aspect-square rounded-lg overflow-hidden border-2 ${
                    selectedImage === index ? 'border-blue-600' : 'border-gray-200'
                  }`}
                >
                  <img src={image} alt={`${product.name} ${index + 1}`} className="w-full h-full object-cover" />
                </button>
              ))}
            </div>
          )}
        </div>

        {/* Details Section */}
        <div>
          <h1 className="text-3xl font-bold mb-4">{product.name}</h1>

          <div className="flex items-center gap-4 mb-6">
            <span className="text-4xl font-bold text-blue-600">${product.price.toFixed(2)}</span>
            {product.stock > 0 ? (
              <span className="px-3 py-1 bg-green-100 text-green-800 rounded-full text-sm">
                En stock ({product.stock} disponibles)
              </span>
            ) : (
              <span className="px-3 py-1 bg-red-100 text-red-800 rounded-full text-sm">
                Rupture de stock
              </span>
            )}
          </div>

          <p className="text-gray-700 mb-6 leading-relaxed">{product.description}</p>

          {/* Specifications */}
          {Object.keys(product.specifications).length > 0 && (
            <div className="mb-6">
              <h3 className="font-semibold text-lg mb-2">Spécifications</h3>
              <dl className="space-y-2">
                {Object.entries(product.specifications).map(([key, value]) => (
                  <div key={key} className="flex gap-2">
                    <dt className="font-medium text-gray-600">{key}:</dt>
                    <dd className="text-gray-800">{value}</dd>
                  </div>
                ))}
              </dl>
            </div>
          )}

          {/* Quantity Selector */}
          {product.stock > 0 && (
            <div className="mb-6">
              <label className="block font-semibold mb-2">Quantité</label>
              <div className="flex items-center gap-4">
                <button
                  onClick={decrementQuantity}
                  className="p-2 border border-gray-300 rounded-md hover:bg-gray-100"
                >
                  <FiMinus />
                </button>
                <span className="text-xl font-semibold w-12 text-center">{quantity}</span>
                <button
                  onClick={incrementQuantity}
                  disabled={quantity >= product.stock}
                  className="p-2 border border-gray-300 rounded-md hover:bg-gray-100 disabled:opacity-50"
                >
                  <FiPlus />
                </button>
              </div>
            </div>
          )}

          {/* Add to Cart Button */}
          <button
            onClick={handleAddToCart}
            disabled={product.stock === 0 || addingToCart}
            className="w-full flex items-center justify-center gap-2 px-6 py-4 bg-blue-600 text-white rounded-md font-semibold hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed transition text-lg"
          >
            <FiShoppingCart size={24} />
            {addingToCart ? 'Ajout en cours...' : 'Ajouter au panier'}
          </button>
        </div>
      </div>
    </div>
  )
}
