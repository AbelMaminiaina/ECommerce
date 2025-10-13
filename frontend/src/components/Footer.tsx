import { Link } from 'react-router-dom'
import { FiFacebook, FiTwitter, FiInstagram, FiMail } from 'react-icons/fi'

export default function Footer() {
  return (
    <footer className="bg-gray-900 text-white py-12 mt-12">
      <div className="container mx-auto px-4">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* About */}
          <div>
            <h3 className="text-xl font-bold mb-4">E-Shop</h3>
            <p className="text-gray-400">
              Votre destination pour des produits de qualité à des prix compétitifs.
            </p>
          </div>

          {/* Quick Links */}
          <div>
            <h3 className="text-lg font-semibold mb-4">Liens Rapides</h3>
            <ul className="space-y-2">
              <li>
                <Link to="/" className="text-gray-400 hover:text-white transition">
                  Accueil
                </Link>
              </li>
              <li>
                <Link to="/products" className="text-gray-400 hover:text-white transition">
                  Produits
                </Link>
              </li>
              <li>
                <Link to="/cart" className="text-gray-400 hover:text-white transition">
                  Panier
                </Link>
              </li>
            </ul>
          </div>

          {/* Customer Service */}
          <div>
            <h3 className="text-lg font-semibold mb-4">Service Client</h3>
            <ul className="space-y-2">
              <li>
                <a href="#" className="text-gray-400 hover:text-white transition">
                  Contact
                </a>
              </li>
              <li>
                <a href="#" className="text-gray-400 hover:text-white transition">
                  FAQ
                </a>
              </li>
              <li>
                <a href="#" className="text-gray-400 hover:text-white transition">
                  Politique de retour
                </a>
              </li>
            </ul>
          </div>

          {/* Social Media */}
          <div>
            <h3 className="text-lg font-semibold mb-4">Suivez-nous</h3>
            <div className="flex gap-4">
              <a href="#" className="text-gray-400 hover:text-white transition">
                <FiFacebook size={24} />
              </a>
              <a href="#" className="text-gray-400 hover:text-white transition">
                <FiTwitter size={24} />
              </a>
              <a href="#" className="text-gray-400 hover:text-white transition">
                <FiInstagram size={24} />
              </a>
              <a href="#" className="text-gray-400 hover:text-white transition">
                <FiMail size={24} />
              </a>
            </div>
          </div>
        </div>

        <div className="border-t border-gray-800 mt-8 pt-8 text-center text-gray-400">
          <p>&copy; 2025 E-Shop. Tous droits réservés.</p>
        </div>
      </div>
    </footer>
  )
}
