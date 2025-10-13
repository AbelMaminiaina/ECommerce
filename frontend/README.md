# E-Commerce Frontend

Application frontend React + TypeScript pour la plateforme e-commerce.

## Technologies

- **React 19** - Bibliothèque UI
- **TypeScript** - Typage statique
- **Vite** - Build tool et dev server
- **Tailwind CSS** - Framework CSS utilitaire
- **Zustand** - Gestion d'état
- **Axios** - Client HTTP
- **React Router** - Navigation
- **React Icons** - Bibliothèque d'icônes
- **Stripe** - Intégration des paiements

## Installation

```bash
npm install
```

## Développement

```bash
npm run dev
```

L'application sera accessible sur `http://localhost:5173`

## Build

```bash
npm run build
```

## Architecture

```
src/
├── components/     # Composants réutilisables
├── contexts/       # Contextes React
├── hooks/          # Hooks personnalisés
├── pages/          # Pages de l'application
├── services/       # Services API
├── stores/         # Stores Zustand
├── types/          # Types TypeScript
└── utils/          # Utilitaires
```

## Système d'Authentification

### Configuration JWT

Le système utilise des tokens JWT avec rafraîchissement automatique :

- **Access Token** : Durée de vie de 8 heures (480 minutes)
- **Refresh Token** : Durée de vie de 7 jours

### Rafraîchissement Automatique des Tokens

L'application implémente un système de rafraîchissement automatique des tokens pour maintenir la session utilisateur active sans interruption.

#### Comment ça fonctionne

1. **Connexion initiale**
   - L'utilisateur se connecte avec email/mot de passe
   - L'API retourne un `accessToken` et un `refreshToken`
   - Les tokens sont stockés dans le `localStorage` et le store Zustand

2. **Utilisation normale**
   - Toutes les requêtes API incluent automatiquement l'`accessToken` dans l'en-tête `Authorization: Bearer {token}`
   - L'utilisateur peut utiliser l'application normalement pendant 8 heures

3. **Expiration du token**
   - Quand l'`accessToken` expire, l'API retourne une erreur 401 Unauthorized
   - L'intercepteur Axios détecte automatiquement cette erreur

4. **Rafraîchissement automatique**
   - L'intercepteur appelle `/api/auth/refresh` avec le `refreshToken`
   - L'API génère un nouveau `accessToken` et `refreshToken`
   - Les nouveaux tokens sont sauvegardés automatiquement
   - La requête initiale est automatiquement réessayée avec le nouveau token
   - **L'utilisateur ne remarque rien, tout se passe en arrière-plan**

5. **Échec du rafraîchissement**
   - Si le `refreshToken` est invalide ou expiré
   - L'utilisateur est automatiquement déconnecté et redirigé vers `/login`

#### Implémentation technique

##### Intercepteur de requêtes (api.ts)

```typescript
// Ajoute automatiquement le token à chaque requête
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken')
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})
```

##### Intercepteur de réponses (api.ts)

```typescript
// Gère automatiquement le rafraîchissement du token
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401 && !originalRequest._retry) {
      // 1. Marquer la requête pour éviter les boucles infinies
      originalRequest._retry = true

      // 2. Récupérer le refresh token
      const refreshToken = localStorage.getItem('refreshToken')

      // 3. Appeler l'API de rafraîchissement
      const response = await axios.post('/api/auth/refresh', refreshToken)

      // 4. Sauvegarder les nouveaux tokens
      const { accessToken, refreshToken: newRefreshToken, user } = response.data
      localStorage.setItem('accessToken', accessToken)
      localStorage.setItem('refreshToken', newRefreshToken)

      // 5. Mettre à jour le store Zustand
      window.useAuthStore.getState().setAuth(user, accessToken, newRefreshToken)

      // 6. Réessayer la requête initiale avec le nouveau token
      originalRequest.headers.Authorization = `Bearer ${accessToken}`
      return api(originalRequest)
    }

    // Rediriger vers login si le rafraîchissement échoue
    if (error.response?.status === 401) {
      localStorage.clear()
      window.location.href = '/login'
    }

    return Promise.reject(error)
  }
)
```

##### Store d'authentification (authStore.ts)

```typescript
interface AuthState {
  user: User | null
  accessToken: string | null
  refreshToken: string | null
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<{ success: boolean }>
  logout: () => void
  checkAuth: () => Promise<void>
  setAuth: (user: User, accessToken: string, refreshToken: string) => void
}
```

La méthode `setAuth` permet à l'intercepteur de mettre à jour l'état d'authentification après un rafraîchissement.

#### Avantages

✅ **Transparence** : L'utilisateur ne perd jamais sa session
✅ **Sécurité** : Les tokens ont une durée de vie limitée
✅ **Automatique** : Aucune action requise de l'utilisateur
✅ **Résilience** : Gère automatiquement les erreurs de rafraîchissement
✅ **Performance** : Pas de polling, rafraîchissement uniquement si nécessaire

#### Points d'attention

⚠️ **Déconnexion automatique** : Si le refresh token expire (après 7 jours d'inactivité), l'utilisateur devra se reconnecter
⚠️ **Sécurité** : Les tokens sont stockés dans le localStorage (vulnérable au XSS)
⚠️ **Boucles infinies** : Le flag `_retry` empêche les requêtes de rafraîchissement récursives

## Endpoints API

### Authentification

- `POST /api/auth/register` - Créer un compte
- `POST /api/auth/login` - Se connecter
- `POST /api/auth/refresh` - Rafraîchir le token
- `GET /api/auth/profile` - Obtenir le profil utilisateur

### Produits

- `GET /api/products` - Liste des produits
- `GET /api/products/featured` - Produits en vedette
- `GET /api/products/:id` - Détails d'un produit
- `GET /api/products/category/:categoryId` - Produits par catégorie
- `GET /api/products/search?term=...` - Rechercher des produits

### Admin uniquement

- `POST /api/products` - Créer un produit
- `PUT /api/products/:id` - Modifier un produit
- `DELETE /api/products/:id` - Supprimer un produit
- `PUT /api/orders/:id/status` - Mettre à jour le statut d'une commande

## Variables d'environnement

Créer un fichier `.env` :

```env
VITE_API_URL=http://localhost:5000/api
VITE_STRIPE_PUBLISHABLE_KEY=your_stripe_publishable_key
```

## Comptes de test

### Administrateur
- Email: `admin@example.com`
- Mot de passe: `Admin123!`

### Client
- Email: `user@example.com`
- Mot de passe: `User123!`

## Licence

MIT
