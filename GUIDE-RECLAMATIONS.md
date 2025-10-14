# Guide du système de réclamations

## ✅ Système opérationnel !

Le système de support et réclamations est maintenant **100% fonctionnel** !

## 🎯 Fonctionnalités disponibles

### Pour les utilisateurs

1. **Créer une réclamation** pour :
   - 📦 Problème de livraison (colis perdu, retardé, endommagé)
   - 📱 Produit défectueux
   - 🛒 Problème de commande
   - ↩️ Demande de retour/remboursement
   - 💳 Problème de paiement
   - ⚙️ Problème technique
   - ❓ Autre

2. **Suivre ses tickets** en temps réel
3. **Échanger avec le support** via messagerie intégrée
4. **Voir l'historique** de toutes ses réclamations

### Pour les admins

1. Voir tous les tickets ouverts
2. Répondre aux réclamations
3. Changer le statut et la priorité
4. Assigner les tickets à des admins

---

## 🚀 Comment utiliser le système

### Interface utilisateur (Frontend)

1. **Se connecter** à l'application
2. **Cliquer sur l'icône de chat** 💬 dans le header ou aller sur `/support`
3. **Créer une nouvelle réclamation** :
   - Cliquer sur "Nouvelle réclamation"
   - Choisir la catégorie (Livraison pour les colis)
   - Remplir le sujet et la description
   - Ajouter le numéro de commande (optionnel)
   - Soumettre

4. **Suivre une réclamation** :
   - Cliquer sur un ticket dans la liste
   - Voir tous les messages échangés
   - Ajouter des messages pour plus d'informations

### API REST (Backend)

**Base URL:** `http://localhost:5000/api`

#### Créer une réclamation

```http
POST /support/tickets
Authorization: Bearer {token}
Content-Type: application/json

{
  "orderId": "68...",  // Optionnel
  "subject": "Colis non reçu",
  "description": "Mon colis COL123456789FR n'est toujours pas arrivé après 10 jours",
  "category": 2  // 2 = Delivery
}
```

#### Obtenir tous mes tickets

```http
GET /support/tickets
Authorization: Bearer {token}
```

#### Obtenir un ticket spécifique

```http
GET /support/tickets/{ticketId}
Authorization: Bearer {token}
```

#### Ajouter un message

```http
POST /support/tickets/{ticketId}/messages
Authorization: Bearer {token}
Content-Type: application/json

{
  "message": "Avez-vous des nouvelles ?",
  "attachments": []
}
```

---

## 📂 Catégories de tickets

| Code | Nom                   | Usage                              |
|------|-----------------------|------------------------------------|
| 0    | Order                 | Problème de commande               |
| 1    | Product               | Produit défectueux                 |
| 2    | **Delivery**          | **Problème de livraison (colis)**  |
| 3    | Return                | Retour/Remboursement               |
| 4    | Technical             | Problème technique                 |
| 5    | Payment               | Problème de paiement               |
| 6    | Other                 | Autre                              |

---

## 🔄 Statuts des tickets

| Statut            | Description                        | Couleur |
|-------------------|------------------------------------|---------|
| Open              | Nouveau ticket ouvert              | 🔵 Bleu |
| InProgress        | En cours de traitement             | 🟡 Jaune |
| WaitingCustomer   | En attente de réponse du client    | 🟠 Orange |
| Resolved          | Problème résolu                    | 🟢 Vert |
| Closed            | Ticket fermé                       | ⚫ Gris |

---

## 🧪 Test du système

### Test rapide avec script

```bash
cd backend
node test-support-system.js
```

Ce script va :
- ✅ Se connecter avec user@example.com
- ✅ Créer un ticket de réclamation pour un colis
- ✅ Lister tous les tickets
- ✅ Ajouter un message au ticket
- ✅ Créer d'autres types de tickets

### Test manuel via Frontend

1. **Démarrer le backend** :
```bash
cd backend/src/ECommerce.API
dotnet run
```

2. **Démarrer le frontend** :
```bash
cd frontend
npm run dev
```

3. **Se connecter** :
   - Email: `user@example.com`
   - Mot de passe: `User123!`

4. **Aller sur** `http://localhost:5173/support`

5. **Créer une réclamation** et tester !

---

## 📁 Fichiers créés

### Frontend

```
frontend/src/
├── types/
│   └── support.ts                    # Types TypeScript pour les tickets
├── services/
│   └── supportService.ts             # Service API pour les tickets
├── pages/
│   ├── Support.tsx                   # Page liste des tickets + modal de création
│   └── SupportTicketDetail.tsx       # Page détails d'un ticket avec messagerie
└── components/
    └── Header.tsx                    # Mise à jour avec lien support
```

### Backend

```
backend/
├── test-support-system.js            # Script de test complet
├── src/ECommerce.API/
│   └── Controllers/
│       └── SupportController.cs      # API déjà présente ✅
├── src/ECommerce.Domain/
│   └── Entities/
│       └── SupportTicket.cs          # Entité déjà présente ✅
└── src/ECommerce.Infrastructure/
    └── Persistence/
        └── SupportTicketRepository.cs # Repository déjà présent ✅
```

---

## 💡 Bonnes pratiques

### Pour les utilisateurs

1. **Soyez précis** dans la description du problème
2. **Mentionnez le numéro de suivi** pour les problèmes de livraison
3. **Liez la commande** concernée si possible
4. **Répondez rapidement** aux questions du support
5. **Fournissez des photos** si nécessaire (attachments)

### Pour les admins

1. **Répondez dans les 24h** pour maintenir la satisfaction client
2. **Changez le statut** au fur et à mesure du traitement
3. **Assignez les tickets** aux bons membres de l'équipe
4. **Utilisez les priorités** pour gérer l'urgence
5. **Fermez les tickets** une fois résolus

---

## 🎨 Aperçu de l'interface

### Page liste des tickets

- 📋 Vue d'ensemble de tous les tickets
- 🎯 Filtrage par statut (à venir)
- ➕ Bouton de création rapide
- 🔍 Recherche de tickets (à venir)

### Page détails du ticket

- 💬 Messagerie en temps réel
- 📊 Sidebar avec infos du ticket
- 🔄 Statut et priorité visibles
- ⏰ Horodatage de chaque message
- 👤 Distinction admin/client

### Modal de création

- 📝 Formulaire simple et clair
- 🏷️ Catégories avec icônes
- 🔗 Lien optionnel à une commande
- ✅ Validation des champs

---

## 🔧 Configuration requise

### Backend
- ✅ .NET 8.0
- ✅ MongoDB (localhost:27017)
- ✅ Backend démarré sur port 5000

### Frontend
- ✅ React 19
- ✅ TypeScript
- ✅ Tailwind CSS v4
- ✅ Frontend démarré sur port 5173 (ou 3000)

---

## 🎉 Prêt à l'emploi !

Le système est **100% opérationnel** et testé. Les utilisateurs peuvent maintenant :

1. ✅ Créer des réclamations pour leurs colis
2. ✅ Suivre l'état de leurs tickets
3. ✅ Échanger avec le support
4. ✅ Voir l'historique complet

**Le backend était déjà prêt**, j'ai simplement ajouté l'interface utilisateur frontend pour faciliter l'utilisation ! 🚀
