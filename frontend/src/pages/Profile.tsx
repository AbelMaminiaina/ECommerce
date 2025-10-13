import { Link } from 'react-router-dom'
import useAuthStore from '../stores/authStore'
import { FiUser, FiMail, FiPhone, FiMapPin } from 'react-icons/fi'

export default function Profile() {
  const { user } = useAuthStore()

  if (!user) {
    return null
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-8">Mon Profil</h1>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Personal Information */}
        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-xl font-bold mb-4">Informations personnelles</h2>

          <div className="space-y-4">
            <div className="flex items-center gap-3">
              <FiUser className="text-gray-400" />
              <div>
                <p className="text-sm text-gray-600">Nom complet</p>
                <p className="font-semibold">{user.firstName} {user.lastName}</p>
              </div>
            </div>

            <div className="flex items-center gap-3">
              <FiMail className="text-gray-400" />
              <div>
                <p className="text-sm text-gray-600">Email</p>
                <p className="font-semibold">{user.email}</p>
              </div>
            </div>

            <div className="flex items-center gap-3">
              <FiPhone className="text-gray-400" />
              <div>
                <p className="text-sm text-gray-600">Téléphone</p>
                <p className="font-semibold">{user.phoneNumber}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Addresses */}
        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-xl font-bold mb-4">Adresses</h2>

          {user.addresses.length > 0 ? (
            <div className="space-y-4">
              {user.addresses.map((address, index) => (
                <div key={index} className="border-l-4 border-blue-600 pl-4">
                  <div className="flex items-start gap-2">
                    <FiMapPin className="text-gray-400 mt-1" />
                    <div>
                      <p>{address.street}</p>
                      <p>{address.city}, {address.state} {address.zipCode}</p>
                      <p>{address.country}</p>
                      {address.isDefault && (
                        <span className="inline-block mt-1 px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded">
                          Par défaut
                        </span>
                      )}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-gray-600">Aucune adresse enregistrée</p>
          )}
        </div>
      </div>

      {/* Quick Links */}
      <div className="mt-8 grid grid-cols-1 md:grid-cols-3 gap-4">
        <Link
          to="/orders"
          className="bg-white p-6 rounded-lg shadow text-center hover:shadow-md transition"
        >
          <h3 className="font-semibold text-lg mb-2">Mes Commandes</h3>
          <p className="text-gray-600 text-sm">Voir l'historique de vos commandes</p>
        </Link>

        <Link
          to="/cart"
          className="bg-white p-6 rounded-lg shadow text-center hover:shadow-md transition"
        >
          <h3 className="font-semibold text-lg mb-2">Mon Panier</h3>
          <p className="text-gray-600 text-sm">Gérer votre panier d'achat</p>
        </Link>

        <Link
          to="/products"
          className="bg-white p-6 rounded-lg shadow text-center hover:shadow-md transition"
        >
          <h3 className="font-semibold text-lg mb-2">Continuer mes achats</h3>
          <p className="text-gray-600 text-sm">Découvrir plus de produits</p>
        </Link>
      </div>
    </div>
  )
}
