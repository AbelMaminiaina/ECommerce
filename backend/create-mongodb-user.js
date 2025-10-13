// ==============================================
// SCRIPT MONGODB - CRÉER DES UTILISATEURS
// Base de données: ECommerceDB
// ==============================================
//
// Exécution:
//   mongosh mongodb://localhost:27017/ECommerceDB < create-mongodb-user.js
//
// Ou dans mongosh:
//   use ECommerceDB
//   load('create-mongodb-user.js')
// ==============================================

use ECommerceDB

print("\n📦 Création d'utilisateurs dans ECommerceDB...\n");

// ==============================================
// MÉTHODE 1: Créer un seul utilisateur
// ==============================================

print("➡️  Méthode 1: Créer un utilisateur unique");

// Vérifier si l'utilisateur existe déjà
var existingUser = db.users.findOne({ email: "marie.dupont@example.com" });

if (existingUser) {
  print("⚠️  L'utilisateur marie.dupont@example.com existe déjà");
} else {
  db.users.insertOne({
    _id: ObjectId(),
    email: "marie.dupont@example.com",
    firstName: "Marie",
    lastName: "Dupont",
    // Mot de passe: "User123!" (hashé avec BCrypt.Net)
    passwordHash: "$2a$11$0HPVoPle1puN2I3qHzWrmeGXwT/RyPm67k5J4hPnAi/ZVH4e0OIAu",
    phoneNumber: "+33612345678",
    addresses: [
      {
        street: "45 Rue de la République",
        city: "Paris",
        state: "Île-de-France",
        zipCode: "75001",
        country: "France",
        isDefault: true
      }
    ],
    role: "Customer",
    isActive: true,
    refreshToken: null,
    refreshTokenExpiryTime: null,
    createdAt: new Date(),
    updatedAt: new Date()
  });

  print("✅ Utilisateur marie.dupont@example.com créé avec succès!");
  print("   🔑 Mot de passe: User123!");
}

print("");

// ==============================================
// MÉTHODE 2: Créer plusieurs utilisateurs
// ==============================================

print("➡️  Méthode 2: Créer plusieurs utilisateurs");

var newUsers = [
  {
    _id: ObjectId(),
    email: "pierre.martin@example.com",
    firstName: "Pierre",
    lastName: "Martin",
    passwordHash: "$2a$11$0HPVoPle1puN2I3qHzWrmeGXwT/RyPm67k5J4hPnAi/ZVH4e0OIAu",
    phoneNumber: "+33623456789",
    addresses: [
      {
        street: "12 Avenue des Champs-Élysées",
        city: "Paris",
        state: "Île-de-France",
        zipCode: "75008",
        country: "France",
        isDefault: true
      }
    ],
    role: "Customer",
    isActive: true,
    refreshToken: null,
    refreshTokenExpiryTime: null,
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    email: "sophie.bernard@example.com",
    firstName: "Sophie",
    lastName: "Bernard",
    passwordHash: "$2a$11$0HPVoPle1puN2I3qHzWrmeGXwT/RyPm67k5J4hPnAi/ZVH4e0OIAu",
    phoneNumber: "+33634567890",
    addresses: [
      {
        street: "8 Rue du Commerce",
        city: "Lyon",
        state: "Auvergne-Rhône-Alpes",
        zipCode: "69002",
        country: "France",
        isDefault: true
      },
      {
        street: "25 Boulevard de la Liberté",
        city: "Marseille",
        state: "Provence-Alpes-Côte d'Azur",
        zipCode: "13001",
        country: "France",
        isDefault: false
      }
    ],
    role: "Customer",
    isActive: true,
    refreshToken: null,
    refreshTokenExpiryTime: null,
    createdAt: new Date(),
    updatedAt: new Date()
  },
  {
    _id: ObjectId(),
    email: "lucas.dubois@example.com",
    firstName: "Lucas",
    lastName: "Dubois",
    passwordHash: "$2a$11$0HPVoPle1puN2I3qHzWrmeGXwT/RyPm67k5J4hPnAi/ZVH4e0OIAu",
    phoneNumber: "+33645678901",
    addresses: [
      {
        street: "67 Rue de la Paix",
        city: "Toulouse",
        state: "Occitanie",
        zipCode: "31000",
        country: "France",
        isDefault: true
      }
    ],
    role: "Customer",
    isActive: true,
    refreshToken: null,
    refreshTokenExpiryTime: null,
    createdAt: new Date(),
    updatedAt: new Date()
  }
];

// Filtrer les utilisateurs qui n'existent pas déjà
var usersToInsert = [];
newUsers.forEach(function(user) {
  var exists = db.users.findOne({ email: user.email });
  if (!exists) {
    usersToInsert.push(user);
  } else {
    print("⚠️  L'utilisateur " + user.email + " existe déjà");
  }
});

// Insérer les nouveaux utilisateurs
if (usersToInsert.length > 0) {
  db.users.insertMany(usersToInsert);
  print("✅ " + usersToInsert.length + " nouveaux utilisateurs créés!");
  print("");
  print("👥 Comptes créés:");
  usersToInsert.forEach(function(user) {
    print("   📧 " + user.email + " | " + user.firstName + " " + user.lastName);
  });
  print("");
  print("   🔑 Mot de passe pour tous: User123!");
} else {
  print("ℹ️  Aucun nouvel utilisateur à créer");
}

print("");

// ==============================================
// AFFICHER TOUS LES UTILISATEURS
// ==============================================

print("➡️  Liste de tous les utilisateurs dans la base:");
print("");

db.users.find({}, {
  email: 1,
  firstName: 1,
  lastName: 1,
  role: 1,
  phoneNumber: 1,
  isActive: 1,
  _id: 0
}).forEach(function(user) {
  var status = user.isActive ? "✅" : "❌";
  var roleIcon = user.role === "Admin" ? "👑" : "👤";
  print(status + " " + roleIcon + " " + user.email.padEnd(30) + " | " +
        user.firstName + " " + user.lastName + " | " + user.role);
});

print("");
print("✅ Script terminé!");
print("");
print("📝 Notes:");
print("   - Tous les utilisateurs créés ont le mot de passe: User123!");
print("   - Pour changer un mot de passe, il faut générer un nouveau hash BCrypt");
print("   - Les utilisateurs peuvent se connecter via l'API: POST /api/auth/login");
print("");
