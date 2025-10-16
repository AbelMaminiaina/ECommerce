# E-Commerce Platform

Site e-commerce complet avec React, TypeScript, Tailwind CSS v4, .NET 8, MongoDB et OAuth 2.0.

## Fonctionnalités

### Frontend (React + TypeScript + Tailwind v4)
- ✅ Page d'accueil avec produits mis en avant
- ✅ Catalogue produits avec filtres et recherche
- ✅ Fiche produit détaillée
- ✅ Panier d'achat
- ✅ Paiement sécurisé avec Stripe
- ✅ Authentification (Login/Register)
- ✅ Espace client (profil, commandes, adresses)
- ✅ Tableau de bord administrateur

### Backend (.NET 8 + Clean Architecture)
- ✅ API REST complète
- ✅ Architecture Clean (Domain, Application, Infrastructure, API)
- ✅ MongoDB pour la base de données
- ✅ OAuth 2.0 / JWT pour l'authentification
- ✅ Gestion des utilisateurs et rôles
- ✅ Gestion des produits et catégories
- ✅ Gestion des commandes
- ✅ Intégration Stripe pour les paiements
- ✅ Swagger pour la documentation API

---

## 🎯 Conformité Légale E-Commerce

Cette plateforme implémente **toutes les obligations légales** pour un site e-commerce conforme au droit français et européen.

### 1️⃣ Droit de Rétractation (14 jours)

**Implémentation complète du délai légal de rétractation :**

#### Backend :
- ✅ Propriétés dans `Order` : `DeliveredAt`, `ReturnRequestedAt`, `ReturnReason`, `ReturnStatus`, `CanReturn`
- ✅ Enum `ReturnStatus` : None, Requested, Approved, InTransit, Received, Refunded, Rejected
- ✅ Calcul automatique de la date limite (14 jours après livraison)
- ✅ Vérification automatique de l'éligibilité au retour

#### Fonctionnalités :
- Compte à rebours automatique des 14 jours après livraison
- Interface de demande de retour avec motif obligatoire
- Suivi du statut de retour en temps réel
- Gestion des retours par les admins
- Remboursement automatique une fois le retour validé

#### Endpoints :
- `POST /api/returns/orders/{orderId}/request` - Demander un retour (Client)
- `GET /api/returns/orders/{orderId}` - Obtenir les infos de retour
- `PUT /api/returns/orders/{orderId}/status` - Mettre à jour le statut (Admin)
- `GET /api/returns` - Toutes les demandes de retour (Admin)

---

### 2️⃣ Livraison dans les Délais Annoncés

**Système complet de suivi de livraison avec alertes automatiques :**

#### Backend :
- ✅ Propriétés dans `Order` : `EstimatedDeliveryDate`, `ShippedAt`, `TrackingNumber`, `CarrierName`, `IsDeliveryDelayed`
- ✅ Entité `ShippingMethod` avec transporteurs et délais
- ✅ Calcul automatique de la date de livraison estimée
- ✅ Détection automatique des retards

#### Fonctionnalités :
- Numéro de suivi fourni automatiquement à l'expédition
- Date de livraison estimée calculée selon le transporteur
- Alerte automatique en cas de retard
- Historique complet de l'expédition
- Support multi-transporteurs (Colissimo, Chronopost, etc.)

#### Endpoints :
- `GET /api/shipping/tracking/{orderId}` - Suivi de livraison
- `PUT /api/shipping/orders/{orderId}/shipping` - Mettre à jour le tracking (Admin)
- `GET /api/shipping/methods` - Liste des méthodes de livraison
- `GET /api/shipping/delayed-orders` - Commandes en retard (Admin)

---

### 3️⃣ Service Après-Vente (SAV) Joignable

**Système de tickets de support complet :**

#### Backend :
- ✅ Entité `SupportTicket` avec système de messages bidirectionnel
- ✅ Catégories : Order, Product, Delivery, Return, Technical, Payment, Other
- ✅ Statuts : Open, InProgress, WaitingCustomer, Resolved, Closed
- ✅ Priorités : Low, Medium, High, Urgent

#### Fonctionnalités :
- Création de tickets par les clients
- Messagerie client-admin en temps réel
- Pièces jointes supportées
- Attribution automatique des tickets aux admins
- Historique complet des échanges
- Système de priorités pour traiter les urgences
- Notifications de réponse

#### Endpoints :
- `POST /api/support/tickets` - Créer un ticket
- `GET /api/support/tickets` - Mes tickets
- `GET /api/support/tickets/{id}` - Détails d'un ticket
- `POST /api/support/tickets/{id}/messages` - Ajouter un message
- `PUT /api/support/tickets/{id}/status` - Mettre à jour le statut (Admin)
- `GET /api/support/tickets/admin/open` - Tickets ouverts (Admin)
- `GET /api/support/tickets/admin/all` - Tous les tickets (Admin)

---

### 4️⃣ Garantie Légale de Conformité (2 ans)

**Gestion complète de la garantie légale de 2 ans pour les produits neufs :**

#### Backend :
- ✅ Propriétés dans `Product` : `WarrantyMonths` (24 par défaut), `WarrantyType`, `IsNew`
- ✅ Entité `WarrantyClaim` pour les réclamations de garantie
- ✅ Calcul automatique de la date d'expiration (date d'achat + 24 mois)
- ✅ Vérification automatique de l'éligibilité

#### Fonctionnalités :
- Garantie de 2 ans pour tous les produits neufs
- Formulaire de réclamation avec photos du défaut
- Vérification automatique de la validité de la garantie
- Suivi du traitement de la réclamation
- Résolutions possibles : Réparation, Remplacement, Remboursement
- Historique des garanties par utilisateur

#### Endpoints :
- `POST /api/warranty/claims` - Soumettre une réclamation
- `GET /api/warranty/claims` - Mes réclamations
- `GET /api/warranty/claims/{id}` - Détails d'une réclamation
- `PUT /api/warranty/claims/{id}` - Traiter une réclamation (Admin)
- `GET /api/warranty/claims/admin/all` - Toutes les réclamations (Admin)

---

### 📊 Tableau de Conformité

| Obligation Légale | Status | Automatisation |
|-------------------|--------|----------------|
| Droit de rétractation 14j | ✅ Implémenté | ✅ Calcul auto + alertes |
| Suivi de livraison | ✅ Implémenté | ✅ Tracking + alerte retard |
| SAV joignable | ✅ Implémenté | ✅ Système de tickets |
| Garantie légale 2 ans | ✅ Implémenté | ✅ Vérification auto |
| Conditions générales | ⚠️ À créer | Textes légaux à rédiger |
| Mentions légales | ⚠️ À créer | Textes légaux à rédiger |
| Politique de confidentialité | ⚠️ À créer | Conformité RGPD |

---

### 🗂️ Nouvelles Entités Créées

#### Domain/Entities :
- `SupportTicket` - Tickets de support avec messages
- `WarrantyClaim` - Réclamations de garantie
- `ShippingMethod` - Méthodes et transporteurs de livraison

#### Enums :
- `ReturnStatus` - Statuts des retours
- `TicketCategory` - Catégories de tickets
- `TicketStatus` - Statuts des tickets
- `TicketPriority` - Priorités des tickets
- `WarrantyClaimStatus` - Statuts des réclamations garantie

---

## Technologies utilisées

### Frontend
- React 19
- TypeScript
- Tailwind CSS v4
- Vite
- React Router DOM
- Zustand (state management)
- Axios
- Stripe (paiement)
- React Icons

### Backend
- .NET 8
- MongoDB Driver
- JWT Bearer Authentication
- Stripe.net
- BCrypt.Net
- FluentValidation
- Swashbuckle (Swagger)

## Installation

### Prérequis
- Node.js 18+
- .NET 8 SDK
- MongoDB

### Backend

1. Naviguez dans le dossier backend :
```bash
cd backend
```

2. Restaurez les packages :
```bash
dotnet restore
```

3. Configurez `appsettings.json` :
   - Modifiez la chaîne de connexion MongoDB
   - Configurez la clé secrète JWT
   - Ajoutez vos clés Stripe

4. Lancez l'API :
```bash
dotnet run --project src/ECommerce.API
```

L'API sera disponible sur `https://localhost:5000`

### Frontend

1. Naviguez dans le dossier frontend :
```bash
cd frontend
```

2. Installez les dépendances :
```bash
npm install --legacy-peer-deps
```

3. Configurez les variables d'environnement :
   - Mettez à jour la clé publique Stripe dans `src/pages/Checkout.tsx`
   - Vérifiez l'URL de l'API dans `src/services/api.ts`

4. Lancez le serveur de développement :
```bash
npm run dev
```

L'application sera disponible sur `http://localhost:3000`

## Configuration MongoDB

### Créer la base de données

```javascript
// Se connecter à MongoDB
use ECommerceDB

// Créer des collections de base
db.createCollection("products")
db.createCollection("categories")
db.createCollection("users")
db.createCollection("orders")
db.createCollection("carts")

// Créer des collections pour la conformité légale
db.createCollection("shippingMethods")
db.createCollection("supportTickets")
db.createCollection("warrantyClaims")

// Créer un utilisateur admin
db.users.insertOne({
  email: "admin@example.com",
  firstName: "Admin",
  lastName: "User",
  passwordHash: "$2a$11$...", // Utilisez BCrypt pour hasher le mot de passe
  role: "Admin",
  phoneNumber: "+1234567890",
  addresses: [],
  isActive: true,
  createdAt: new Date(),
  updatedAt: new Date()
})
```

### Seed Data Automatique

Le backend inclut un système de seed automatique qui crée :
- ✅ 2 utilisateurs de test (admin + client)
- ✅ 5 catégories de produits
- ✅ 10 produits avec spécifications et garantie
- ✅ 2 commandes de démonstration

Ces données sont créées automatiquement au premier démarrage de l'application.

## ⚙️ Configuration Complète

### 1. Configuration Stripe (Paiement)

#### Étape 1 : Obtenir les clés de test Stripe

1. Créez un compte sur [stripe.com](https://stripe.com)
2. Connectez-vous au Dashboard Stripe
3. Allez dans **Développeurs → Clés API**
4. **Activez le mode Test** (toggle en haut à droite)
5. Copiez :
   - **Clé secrète** (commence par `sk_test_...`)
   - **Clé publiable** (commence par `pk_test_...`)

#### Étape 2 : Configurer le Backend

Éditez `backend/src/ECommerce.API/appsettings.json` :
```json
{
  "StripeSettings": {
    "SecretKey": "sk_test_51Abc...XYZ",
    "PublishableKey": "pk_test_51Abc...XYZ"
  }
}
```

**ET** `backend/src/ECommerce.API/appsettings.Development.json` :
```json
{
  "StripeSettings": {
    "SecretKey": "sk_test_51Abc...XYZ",
    "PublishableKey": "pk_test_51Abc...XYZ"
  }
}
```

#### Étape 3 : Configurer le Frontend

Éditez `frontend/src/pages/Checkout.tsx` ligne 10 :
```typescript
const stripePromise = loadStripe('pk_test_51Abc...XYZ')
```

#### Étape 4 : Tester avec des cartes de test

| Carte | Numéro | Résultat |
|-------|--------|----------|
| Succès | `4242 4242 4242 4242` | Paiement réussi |
| Refus | `4000 0000 0000 0002` | Carte refusée |
| Fonds insuffisants | `4000 0000 0000 9995` | Fonds insuffisants |
| 3D Secure | `4000 0025 0000 3155` | Authentification requise |

**Pour tous les tests :**
- Date d'expiration : n'importe quelle date future (ex: `12/34`)
- CVC : n'importe quel code à 3 chiffres (ex: `123`)
- Code postal : n'importe lequel (ex: `75001`)

#### Étape 5 : Vérifier les paiements

- Dashboard Stripe : [dashboard.stripe.com/test/payments](https://dashboard.stripe.com/test/payments)
- Vous verrez tous vos paiements de test

---

### 2. Configuration MongoDB

#### Installation locale

**Windows :**
```bash
# Télécharger MongoDB Community Server depuis mongodb.com
# Installer et démarrer le service MongoDB
```

**macOS (Homebrew) :**
```bash
brew tap mongodb/brew
brew install mongodb-community
brew services start mongodb-community
```

**Linux :**
```bash
sudo apt-get install mongodb
sudo systemctl start mongodb
```

#### Configuration dans appsettings.json

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ECommerceDB"
  }
}
```

**Pour MongoDB Atlas (cloud) :**
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb+srv://username:password@cluster.mongodb.net",
    "DatabaseName": "ECommerceDB"
  }
}
```

#### Vérifier la connexion

```bash
# Ouvrir MongoDB Shell
mongosh

# Lister les bases de données
show dbs

# Utiliser votre base de données
use ECommerceDB

# Vérifier les collections
show collections
```

---

### 3. Configuration JWT (Authentification)

Éditez `backend/src/ECommerce.API/appsettings.json` :

```json
{
  "JwtSettings": {
    "Secret": "VotreCleSuperSecreteDeMinimum32Caracteres!",
    "Issuer": "ECommerceAPI",
    "Audience": "ECommerceClient",
    "AccessTokenExpirationMinutes": 480,
    "RefreshTokenExpirationDays": 7
  }
}
```

**Important :**
- La clé secrète (`Secret`) doit faire **au moins 32 caractères**
- **Changez cette valeur en production !**
- Ne commitez jamais vos vraies clés secrètes dans Git

---

### 4. Configuration Email (Optionnel)

Pour envoyer des emails de confirmation, notifications, etc.

#### Avec Gmail

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "votre.email@gmail.com",
    "SmtpPassword": "votre_mot_de_passe_application",
    "FromEmail": "votre.email@gmail.com",
    "FromName": "Votre E-Commerce"
  }
}
```

**Pour Gmail, vous devez :**
1. Activer la validation en 2 étapes sur votre compte Google
2. Générer un "Mot de passe d'application" :
   - Allez dans Paramètres Google → Sécurité → Validation en 2 étapes
   - "Mots de passe des applications" → Créer un mot de passe
   - Utilisez ce mot de passe dans `SmtpPassword`

#### Avec d'autres fournisseurs

**SendGrid :**
```json
{
  "Email": {
    "SmtpHost": "smtp.sendgrid.net",
    "SmtpPort": 587,
    "SmtpUsername": "apikey",
    "SmtpPassword": "votre_api_key_sendgrid",
    "FromEmail": "noreply@votresite.com",
    "FromName": "Votre E-Commerce"
  }
}
```

**Mailgun, Postmark, etc.** : Consultez leur documentation respective.

---

### 5. Configuration des Transporteurs (Optionnel)

Pour l'intégration avec les API de suivi de colis.

```json
{
  "Carriers": {
    "Colissimo": {
      "ApiUrl": "https://ws.colissimo.fr/sls-ws/SlsServiceWS",
      "ContractNumber": "votre_numero_contrat",
      "Password": "votre_mot_de_passe"
    },
    "Chronopost": {
      "ApiUrl": "https://ws.chronopost.fr/shipping-cxf/ShippingServiceWS",
      "AccountNumber": "votre_numero_compte",
      "Password": "votre_mot_de_passe"
    },
    "MondialRelay": {
      "ApiUrl": "https://api.mondialrelay.com/Web_Services.asmx",
      "BrandCode": "votre_code_enseigne",
      "ApiKey": "votre_cle_api"
    },
    "DHL": {
      "ApiUrl": "https://api-eu.dhl.com/parcel/de/shipping/v2",
      "ApiKey": "votre_cle_api",
      "AccountNumber": "votre_numero_compte"
    }
  }
}
```

**Note :** Ces intégrations nécessitent des contrats avec les transporteurs.

---

### 6. Variables d'Environnement (Production)

**Ne mettez JAMAIS vos vraies clés dans `appsettings.json` en production !**

Utilisez plutôt des variables d'environnement :

```bash
# Linux/macOS
export StripeSettings__SecretKey="sk_live_..."
export MongoDbSettings__ConnectionString="mongodb+srv://..."
export JwtSettings__Secret="VotreCleSecrete..."

# Windows (PowerShell)
$env:StripeSettings__SecretKey="sk_live_..."
$env:MongoDbSettings__ConnectionString="mongodb+srv://..."
$env:JwtSettings__Secret="VotreCleSecrete..."
```

Ou configurez-les dans votre plateforme de déploiement (Azure, AWS, Heroku, etc.).

---

### 7. Récapitulatif des Configurations

| Configuration | Requis | Fichier(s) | Notes |
|---------------|--------|-----------|-------|
| **MongoDB** | ✅ Oui | `appsettings.json` | Base de données principale |
| **JWT Secret** | ✅ Oui | `appsettings.json` | Min 32 caractères |
| **Stripe (Test)** | ✅ Oui | `appsettings.json` + `Checkout.tsx` | Pour tester les paiements |
| **Stripe (Prod)** | ⚠️ Production | Variables d'environnement | Clés `sk_live_...` et `pk_live_...` |
| **Email/SMTP** | ❌ Optionnel | `appsettings.json` | Pour notifications |
| **Transporteurs** | ❌ Optionnel | `appsettings.json` | Pour suivi avancé |

---

### 8. Démarrage Rapide

Une fois toutes les configurations en place :

```bash
# Terminal 1 : Backend
cd backend/src/ECommerce.API
dotnet run

# Terminal 2 : Frontend
cd frontend
npm run dev
```

**URLs :**
- Frontend : `http://localhost:3000`
- Backend API : `https://localhost:5000`
- Swagger : `https://localhost:5000/swagger`

---

## Structure du projet

```
e-commerce/
├── backend/
│   └── src/
│       ├── ECommerce.Domain/         # Entités et interfaces
│       ├── ECommerce.Application/    # DTOs, use cases, validations
│       ├── ECommerce.Infrastructure/ # Implémentations (MongoDB, Auth, Payment)
│       └── ECommerce.API/            # Controllers et configuration
└── frontend/
    └── src/
        ├── components/    # Composants réutilisables
        ├── pages/         # Pages de l'application
        ├── services/      # Services API
        ├── stores/        # State management (Zustand)
        └── types/         # Types TypeScript
```

## Endpoints API

### Authentification
- `POST /api/auth/register` - Inscription
- `POST /api/auth/login` - Connexion
- `POST /api/auth/refresh` - Rafraîchir le token

### Produits
- `GET /api/products` - Liste des produits
- `GET /api/products/featured` - Produits en vedette
- `GET /api/products/{id}` - Détails d'un produit
- `GET /api/products/category/{categoryId}` - Produits par catégorie
- `GET /api/products/search?term=...` - Recherche de produits
- `POST /api/products` - Créer un produit (Admin)
- `PUT /api/products/{id}` - Modifier un produit (Admin)
- `DELETE /api/products/{id}` - Supprimer un produit (Admin)

### Panier
- `GET /api/cart` - Obtenir le panier
- `POST /api/cart/items` - Ajouter au panier
- `PUT /api/cart/items` - Modifier la quantité
- `DELETE /api/cart/items/{productId}` - Retirer du panier
- `DELETE /api/cart` - Vider le panier

### Commandes
- `GET /api/orders` - Liste des commandes
- `GET /api/orders/{id}` - Détails d'une commande
- `POST /api/orders` - Créer une commande
- `POST /api/orders/{id}/payment` - Créer un paiement
- `PUT /api/orders/{id}/status` - Modifier le statut (Admin)

### Catégories
- `GET /api/categories` - Liste des catégories
- `GET /api/categories/{id}` - Détails d'une catégorie
- `GET /api/categories/{id}/subcategories` - Sous-catégories
- `POST /api/categories` - Créer une catégorie (Admin)
- `DELETE /api/categories/{id}` - Supprimer une catégorie (Admin)

### 🔄 Retours (Conformité Légale)
- `POST /api/returns/orders/{orderId}/request` - Demander un retour (Client)
- `GET /api/returns/orders/{orderId}` - Obtenir les infos de retour
- `PUT /api/returns/orders/{orderId}/status` - Mettre à jour le statut (Admin)
- `GET /api/returns` - Toutes les demandes de retour (Admin)

### 🚚 Livraison & Suivi
- `GET /api/shipping/tracking/{orderId}` - Suivi de livraison
- `PUT /api/shipping/orders/{orderId}/shipping` - Mettre à jour le tracking (Admin)
- `GET /api/shipping/methods` - Liste des méthodes de livraison
- `GET /api/shipping/delayed-orders` - Commandes en retard (Admin)

### 🎧 Support (SAV)
- `POST /api/support/tickets` - Créer un ticket
- `GET /api/support/tickets` - Mes tickets
- `GET /api/support/tickets/{id}` - Détails d'un ticket
- `POST /api/support/tickets/{id}/messages` - Ajouter un message
- `PUT /api/support/tickets/{id}/status` - Mettre à jour le statut (Admin)
- `GET /api/support/tickets/admin/open` - Tickets ouverts (Admin)
- `GET /api/support/tickets/admin/all` - Tous les tickets (Admin)

### 📜 Garantie Légale
- `POST /api/warranty/claims` - Soumettre une réclamation
- `GET /api/warranty/claims` - Mes réclamations
- `GET /api/warranty/claims/{id}` - Détails d'une réclamation
- `PUT /api/warranty/claims/{id}` - Traiter une réclamation (Admin)
- `GET /api/warranty/claims/admin/all` - Toutes les réclamations (Admin)

## Documentation API

Une fois l'API lancée, accédez à Swagger UI sur : `https://localhost:5000/swagger`

## Sécurité

- Authentification JWT avec refresh tokens
- Hachage des mots de passe avec BCrypt
- CORS configuré
- Validation des entrées avec FluentValidation
- Autorisation basée sur les rôles

## Prochaines améliorations

### Fonctionnalités additionnelles
- [ ] Upload d'images pour les produits
- [ ] Système d'évaluation et commentaires
- [ ] Wishlist (liste de souhaits)
- [ ] Système de coupons/promotions
- [ ] Analytics et rapports
- [ ] Support PayPal en plus de Stripe
- [ ] Export des données (factures, commandes)
- [ ] Système de fidélité/points

### Conformité & Légal
- [ ] Notifications par email automatiques (retour, livraison, support)
- [ ] Pages légales (CGV, mentions légales, politique de confidentialité)
- [ ] Conformité RGPD complète
- [ ] Export des données personnelles (droit à la portabilité)
- [ ] Suppression de compte et données (droit à l'oubli)

### Améliorations techniques
- [ ] Tests unitaires et d'intégration
- [ ] CI/CD Pipeline
- [ ] Containerisation (Docker)
- [ ] Monitoring et logging (Serilog, Application Insights)
- [ ] Cache avec Redis
- [ ] Rate limiting
- [ ] Optimisation des performances

## Licence

MIT
