import { useEffect, useState } from 'react'
import api from '../../services/api'
import { Product, Category } from '../../types'
import { FiEdit, FiTrash2, FiPlus, FiX } from 'react-icons/fi'

interface ProductFormData {
  name: string
  description: string
  price: number
  stock: number
  categoryId: string
  images: string[]
  isFeatured: boolean
  warrantyMonths: number
  warrantyType: string
  isNew: boolean
}

export default function AdminProducts() {
  const [products, setProducts] = useState<Product[]>([])
  const [categories, setCategories] = useState<Category[]>([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [editingProduct, setEditingProduct] = useState<Product | null>(null)

  const [formData, setFormData] = useState<ProductFormData>({
    name: '',
    description: '',
    price: 0,
    stock: 0,
    categoryId: '',
    images: [''],
    isFeatured: false,
    warrantyMonths: 24,
    warrantyType: 'Légale',
    isNew: true
  })

  useEffect(() => {
    fetchProducts()
    fetchCategories()
  }, [])

  const fetchProducts = async () => {
    try {
      const response = await api.get<Product[]>('/products')
      setProducts(response.data)
    } catch (error) {
      console.error('Failed to fetch products:', error)
    } finally {
      setLoading(false)
    }
  }

  const fetchCategories = async () => {
    try {
      const response = await api.get<Category[]>('/categories')
      setCategories(response.data)
    } catch (error) {
      console.error('Failed to fetch categories:', error)
    }
  }

  const handleDelete = async (id: string) => {
    if (!confirm('Êtes-vous sûr de vouloir supprimer ce produit?')) {
      return
    }

    try {
      await api.delete(`/products/${id}`)
      setProducts(products.filter(p => p.id !== id))
      alert('Produit supprimé avec succès')
    } catch (error) {
      alert('Erreur lors de la suppression du produit')
    }
  }

  const handleOpenCreateModal = () => {
    setEditingProduct(null)
    setFormData({
      name: '',
      description: '',
      price: 0,
      stock: 0,
      categoryId: categories[0]?.id || '',
      images: [''],
      isFeatured: false,
      warrantyMonths: 24,
      warrantyType: 'Légale',
      isNew: true
    })
    setShowModal(true)
  }

  const handleOpenEditModal = (product: Product) => {
    setEditingProduct(product)
    setFormData({
      name: product.name,
      description: product.description,
      price: product.price,
      stock: product.stock,
      categoryId: product.categoryId,
      images: product.images.length > 0 ? product.images : [''],
      isFeatured: product.isFeatured,
      warrantyMonths: 24, // Default, backend will handle
      warrantyType: 'Légale',
      isNew: true
    })
    setShowModal(true)
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()

    if (!formData.name || !formData.categoryId || formData.price <= 0) {
      alert('Veuillez remplir tous les champs obligatoires')
      return
    }

    try {
      const productData = {
        ...formData,
        images: formData.images.filter(img => img.trim() !== ''),
        specifications: {} // Can be extended later
      }

      if (editingProduct) {
        // Update existing product
        await api.put(`/products/${editingProduct.id}`, productData)
        alert('Produit modifié avec succès')
      } else {
        // Create new product
        await api.post('/products', productData)
        alert('Produit créé avec succès')
      }

      setShowModal(false)
      fetchProducts()
    } catch (error: any) {
      alert(error.response?.data?.message || 'Erreur lors de la sauvegarde du produit')
    }
  }

  const handleImageChange = (index: number, value: string) => {
    const newImages = [...formData.images]
    newImages[index] = value
    setFormData({ ...formData, images: newImages })
  }

  const handleAddImage = () => {
    setFormData({ ...formData, images: [...formData.images, ''] })
  }

  const handleRemoveImage = (index: number) => {
    const newImages = formData.images.filter((_, i) => i !== index)
    setFormData({ ...formData, images: newImages.length > 0 ? newImages : [''] })
  }

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="animate-pulse">
          <div className="h-8 bg-gray-200 rounded w-64 mb-4"></div>
          <div className="space-y-4">
            {[...Array(5)].map((_, i) => (
              <div key={i} className="h-20 bg-gray-200 rounded"></div>
            ))}
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-3xl font-bold">Gestion des produits</h1>
        <button
          onClick={handleOpenCreateModal}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
        >
          <FiPlus />
          Nouveau produit
        </button>
      </div>

      <div className="bg-white rounded-lg shadow overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Produit</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Prix</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Stock</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Statut</th>
              <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {products.map((product) => (
              <tr key={product.id}>
                <td className="px-6 py-4">
                  <div className="flex items-center gap-3">
                    <img
                      src={product.images[0] || '/placeholder.jpg'}
                      alt={product.name}
                      className="w-12 h-12 object-cover rounded"
                    />
                    <div>
                      <p className="font-medium">{product.name}</p>
                      <p className="text-sm text-gray-500">
                        {categories.find(c => c.id === product.categoryId)?.name || product.categoryId}
                      </p>
                    </div>
                  </div>
                </td>
                <td className="px-6 py-4">${product.price.toFixed(2)}</td>
                <td className="px-6 py-4">
                  <span className={product.stock > 0 ? 'text-green-600 font-semibold' : 'text-red-600 font-semibold'}>
                    {product.stock}
                  </span>
                </td>
                <td className="px-6 py-4">
                  {product.isFeatured && (
                    <span className="px-2 py-1 bg-yellow-100 text-yellow-800 text-xs rounded">
                      Vedette
                    </span>
                  )}
                </td>
                <td className="px-6 py-4 text-right">
                  <div className="flex items-center justify-end gap-2">
                    <button
                      onClick={() => handleOpenEditModal(product)}
                      className="p-2 text-blue-600 hover:bg-blue-50 rounded"
                    >
                      <FiEdit />
                    </button>
                    <button
                      onClick={() => handleDelete(product.id)}
                      className="p-2 text-red-600 hover:bg-red-50 rounded"
                    >
                      <FiTrash2 />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Modal Create/Edit Product */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg p-6 max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-2xl font-bold">
                {editingProduct ? 'Modifier le produit' : 'Nouveau produit'}
              </h2>
              <button onClick={() => setShowModal(false)} className="text-gray-500 hover:text-gray-700">
                <FiX size={24} />
              </button>
            </div>

            <form onSubmit={handleSubmit}>
              <div className="space-y-4">
                {/* Name */}
                <div>
                  <label className="block text-sm font-medium mb-2">Nom du produit *</label>
                  <input
                    type="text"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    className="w-full border border-gray-300 rounded p-2"
                    required
                  />
                </div>

                {/* Description */}
                <div>
                  <label className="block text-sm font-medium mb-2">Description *</label>
                  <textarea
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    className="w-full border border-gray-300 rounded p-2"
                    rows={3}
                    required
                  />
                </div>

                {/* Price & Stock */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium mb-2">Prix ($) *</label>
                    <input
                      type="number"
                      step="0.01"
                      min="0"
                      value={formData.price}
                      onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) })}
                      className="w-full border border-gray-300 rounded p-2"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-2">Stock *</label>
                    <input
                      type="number"
                      min="0"
                      value={formData.stock}
                      onChange={(e) => setFormData({ ...formData, stock: parseInt(e.target.value) })}
                      className="w-full border border-gray-300 rounded p-2"
                      required
                    />
                  </div>
                </div>

                {/* Category */}
                <div>
                  <label className="block text-sm font-medium mb-2">Catégorie *</label>
                  <select
                    value={formData.categoryId}
                    onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                    className="w-full border border-gray-300 rounded p-2"
                    required
                  >
                    <option value="">Sélectionner une catégorie</option>
                    {categories.map((cat) => (
                      <option key={cat.id} value={cat.id}>
                        {cat.name}
                      </option>
                    ))}
                  </select>
                </div>

                {/* Images */}
                <div>
                  <label className="block text-sm font-medium mb-2">Images (URLs)</label>
                  {formData.images.map((img, index) => (
                    <div key={index} className="flex gap-2 mb-2">
                      <input
                        type="url"
                        value={img}
                        onChange={(e) => handleImageChange(index, e.target.value)}
                        placeholder="https://example.com/image.jpg"
                        className="flex-1 border border-gray-300 rounded p-2"
                      />
                      {formData.images.length > 1 && (
                        <button
                          type="button"
                          onClick={() => handleRemoveImage(index)}
                          className="px-3 py-2 bg-red-500 text-white rounded hover:bg-red-600"
                        >
                          <FiX />
                        </button>
                      )}
                    </div>
                  ))}
                  <button
                    type="button"
                    onClick={handleAddImage}
                    className="text-sm text-blue-600 hover:text-blue-700"
                  >
                    + Ajouter une image
                  </button>
                </div>

                {/* Featured */}
                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    id="isFeatured"
                    checked={formData.isFeatured}
                    onChange={(e) => setFormData({ ...formData, isFeatured: e.target.checked })}
                    className="w-4 h-4"
                  />
                  <label htmlFor="isFeatured" className="text-sm font-medium">
                    Produit en vedette
                  </label>
                </div>

                {/* Warranty */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium mb-2">Garantie (mois)</label>
                    <input
                      type="number"
                      min="0"
                      value={formData.warrantyMonths}
                      onChange={(e) => setFormData({ ...formData, warrantyMonths: parseInt(e.target.value) })}
                      className="w-full border border-gray-300 rounded p-2"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-2">Type de garantie</label>
                    <input
                      type="text"
                      value={formData.warrantyType}
                      onChange={(e) => setFormData({ ...formData, warrantyType: e.target.value })}
                      className="w-full border border-gray-300 rounded p-2"
                    />
                  </div>
                </div>

                {/* Is New */}
                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    id="isNew"
                    checked={formData.isNew}
                    onChange={(e) => setFormData({ ...formData, isNew: e.target.checked })}
                    className="w-4 h-4"
                  />
                  <label htmlFor="isNew" className="text-sm font-medium">
                    Produit neuf (garantie légale 2 ans applicable)
                  </label>
                </div>
              </div>

              {/* Buttons */}
              <div className="flex justify-end gap-2 mt-6">
                <button
                  type="button"
                  onClick={() => setShowModal(false)}
                  className="px-4 py-2 border border-gray-300 rounded hover:bg-gray-50"
                >
                  Annuler
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                >
                  {editingProduct ? 'Modifier' : 'Créer'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
