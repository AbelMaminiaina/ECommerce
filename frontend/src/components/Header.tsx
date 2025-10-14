import { Link } from 'react-router-dom'
import { FiShoppingCart, FiUser, FiLogOut, FiMenu } from 'react-icons/fi'
import { useState } from 'react'
import useAuthStore from '../stores/authStore'
import useCartStore from '../stores/cartStore'

export default function Header() {
  const { user, isAuthenticated, logout } = useAuthStore()
  const { cart } = useCartStore()
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false)

  const cartItemsCount = cart?.items.reduce((acc, item) => acc + item.quantity, 0) || 0

  return (
    <header className="bg-white shadow-md sticky top-0 z-50">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <Link to="/" className="text-2xl font-bold text-blue-600">
            E-Shop
          </Link>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-8">
            <Link to="/" className="text-gray-700 hover:text-blue-600 transition">
              Accueil
            </Link>
            <Link to="/products" className="text-gray-700 hover:text-blue-600 transition">
              Produits
            </Link>
          </nav>

          {/* Right Section */}
          <div className="hidden md:flex items-center gap-4">
            <Link to="/cart" className="relative text-gray-700 hover:text-blue-600 transition">
              <FiShoppingCart size={24} />
              {cartItemsCount > 0 && (
                <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                  {cartItemsCount}
                </span>
              )}
            </Link>

            {isAuthenticated ? (
              <>
                <Link
                  to="/support"
                  className="text-gray-700 hover:text-blue-600 transition flex items-center gap-1"
                  title="Support & Réclamations"
                >
                  <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z" />
                  </svg>
                </Link>
                <Link to="/profile" className="text-gray-700 hover:text-blue-600 transition">
                  <FiUser size={24} />
                </Link>
                {user?.role === 'Admin' && (
                  <Link
                    to="/admin"
                    className="px-4 py-2 bg-purple-600 text-white rounded-md hover:bg-purple-700 transition"
                  >
                    Admin
                  </Link>
                )}
                <button
                  onClick={logout}
                  className="text-gray-700 hover:text-red-600 transition"
                >
                  <FiLogOut size={24} />
                </button>
              </>
            ) : (
              <>
                <Link
                  to="/login"
                  className="px-4 py-2 text-blue-600 hover:text-blue-700 transition"
                >
                  Connexion
                </Link>
                <Link
                  to="/register"
                  className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition"
                >
                  Inscription
                </Link>
              </>
            )}
          </div>

          {/* Mobile Menu Button */}
          <button
            className="md:hidden text-gray-700"
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
          >
            <FiMenu size={24} />
          </button>
        </div>

        {/* Mobile Menu */}
        {mobileMenuOpen && (
          <div className="md:hidden py-4 border-t">
            <nav className="flex flex-col gap-4">
              <Link to="/" className="text-gray-700 hover:text-blue-600 transition">
                Accueil
              </Link>
              <Link to="/products" className="text-gray-700 hover:text-blue-600 transition">
                Produits
              </Link>
              <Link to="/cart" className="text-gray-700 hover:text-blue-600 transition">
                Panier ({cartItemsCount})
              </Link>
              {isAuthenticated ? (
                <>
                  <Link to="/profile" className="text-gray-700 hover:text-blue-600 transition">
                    Profil
                  </Link>
                  <Link to="/orders" className="text-gray-700 hover:text-blue-600 transition">
                    Commandes
                  </Link>
                  <Link to="/support" className="text-gray-700 hover:text-blue-600 transition">
                    Support & Réclamations
                  </Link>
                  {user?.role === 'Admin' && (
                    <Link to="/admin" className="text-purple-600 hover:text-purple-700 transition">
                      Admin
                    </Link>
                  )}
                  <button
                    onClick={logout}
                    className="text-left text-red-600 hover:text-red-700 transition"
                  >
                    Déconnexion
                  </button>
                </>
              ) : (
                <>
                  <Link to="/login" className="text-blue-600 hover:text-blue-700 transition">
                    Connexion
                  </Link>
                  <Link to="/register" className="text-blue-600 hover:text-blue-700 transition">
                    Inscription
                  </Link>
                </>
              )}
            </nav>
          </div>
        )}
      </div>
    </header>
  )
}
