# Scripts MongoDB pour ECommerceDB

Ce dossier contient des scripts utilitaires pour gérer les utilisateurs MongoDB de votre application e-commerce.

## 📁 Fichiers

- **`create-mongodb-user.js`** - Script MongoDB pour créer des utilisateurs dans la collection `users`
- **`generate-password-hash.js`** - Script Node.js pour générer des hash BCrypt
- **`MONGODB-SCRIPTS-README.md`** - Ce fichier

---

## 🚀 Utilisation rapide

### 1. Créer des utilisateurs dans MongoDB

**Option A : Via mongosh directement**
```bash
cd backend
mongosh mongodb://localhost:27017/ECommerceDB < create-mongodb-user.js
```

**Option B : Dans mongosh interactif**
```bash
mongosh mongodb://localhost:27017/ECommerceDB
```
Puis dans mongosh :
```javascript
load('create-mongodb-user.js')
```

### 2. Générer un hash de mot de passe personnalisé

**Installation de la dépendance (première fois uniquement)**
```bash
cd backend
npm install bcryptjs
```

**Générer un hash**
```bash
node generate-password-hash.js "MonMotDePasse123!"
```

Exemple de sortie :
```
✅ Hash BCrypt généré avec succès!

🔑 Mot de passe: MonMotDePasse123!
🔐 Hash: $2a$11$xKj5L9Y8Z9Y9Y9Y9Y9Y9Y9eK...
```

---

## 📝 Utilisateurs créés par défaut

Le script `create-mongodb-user.js` crée les utilisateurs suivants :

| Email | Prénom | Nom | Rôle | Mot de passe |
|-------|--------|-----|------|--------------|
| `marie.dupont@example.com` | Marie | Dupont | Customer | `User123!` |
| `pierre.martin@example.com` | Pierre | Martin | Customer | `User123!` |
| `sophie.bernard@example.com` | Sophie | Bernard | Customer | `User123!` |
| `lucas.dubois@example.com` | Lucas | Dubois | Customer | `User123!` |

**Note :** Ces utilisateurs sont en plus des utilisateurs créés automatiquement au démarrage de l'application :
- `admin@example.com` (Admin) - mot de passe : `Admin123!`
- `user@example.com` (Customer) - mot de passe : `User123!`

---

## ✏️ Personnaliser le script

### Ajouter un nouvel utilisateur

Ouvrez `create-mongodb-user.js` et ajoutez un objet dans le tableau `newUsers` :

```javascript
{
  _id: ObjectId(),
  email: "nouveau@example.com",
  firstName: "Prénom",
  lastName: "Nom",
  passwordHash: "$2a$11$...", // Générer avec generate-password-hash.js
  phoneNumber: "+33600000000",
  addresses: [
    {
      street: "123 Rue Example",
      city: "Paris",
      state: "Île-de-France",
      zipCode: "75001",
      country: "France",
      isDefault: true
    }
  ],
  role: "Customer", // ou "Admin"
  isActive: true,
  refreshToken: null,
  refreshTokenExpiryTime: null,
  createdAt: new Date(),
  updatedAt: new Date()
}
```

### Créer un utilisateur Admin

Changez simplement le champ `role` :
```javascript
role: "Admin"
```

---

## 🔐 Sécurité

**IMPORTANT :**
- ⚠️ Les mots de passe dans ce script sont des exemples (`User123!`)
- 🔒 En production, utilisez toujours des mots de passe forts et uniques
- 🚫 Ne commitez JAMAIS ce fichier avec de vrais mots de passe en clair
- ✅ Utilisez des variables d'environnement pour les informations sensibles

---

## 📚 Commandes MongoDB utiles

### Lister tous les utilisateurs
```javascript
use ECommerceDB
db.users.find({}, { email: 1, firstName: 1, lastName: 1, role: 1 })
```

### Trouver un utilisateur par email
```javascript
db.users.findOne({ email: "user@example.com" })
```

### Modifier le rôle d'un utilisateur
```javascript
db.users.updateOne(
  { email: "user@example.com" },
  { $set: { role: "Admin" } }
)
```

### Désactiver un utilisateur
```javascript
db.users.updateOne(
  { email: "user@example.com" },
  { $set: { isActive: false } }
)
```

### Supprimer un utilisateur
```javascript
db.users.deleteOne({ email: "user@example.com" })
```

### Compter les utilisateurs
```javascript
db.users.countDocuments()
db.users.countDocuments({ role: "Customer" })
db.users.countDocuments({ role: "Admin" })
```

---

## 🧪 Test de connexion

Après avoir créé un utilisateur, vous pouvez tester la connexion via l'API :

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "marie.dupont@example.com",
    "password": "User123!"
  }'
```

---

## 🆘 Dépannage

### Erreur : "MongoServerError: command insert requires authentication"
Votre MongoDB nécessite une authentification. Ajoutez les identifiants :
```bash
mongosh mongodb://username:password@localhost:27017/ECommerceDB
```

### Erreur : "bcryptjs not found"
Installez la dépendance :
```bash
npm install bcryptjs
```

### Les utilisateurs existent déjà
Le script vérifie automatiquement les doublons. Aucun utilisateur ne sera recréé si l'email existe déjà.

---

## 📖 Documentation

- [MongoDB Manual](https://docs.mongodb.com/manual/)
- [BCrypt.js Documentation](https://github.com/dcodeIO/bcrypt.js)
- [mongosh Reference](https://docs.mongodb.com/mongodb-shell/)

---

✅ **Scripts créés et maintenus pour le projet E-Commerce**
