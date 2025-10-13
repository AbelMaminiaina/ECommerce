// ==============================================
// TEST DU SCRIPT - CRÉER DES UTILISATEURS
// Utilise Node.js + MongoDB Driver
// ==============================================

const { MongoClient, ObjectId } = require('mongodb');
const bcrypt = require('bcryptjs');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';

async function createUsers() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n📦 Connexion à MongoDB...\n');
    await client.connect();
    console.log('✅ Connecté à MongoDB!\n');

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // ==============================================
    // CRÉER DES UTILISATEURS
    // ==============================================

    // Hash du mot de passe "User123!"
    const passwordHash = bcrypt.hashSync('User123!', 11);

    const newUsers = [
      {
        _id: new ObjectId(),
        email: "marie.dupont@example.com",
        firstName: "Marie",
        lastName: "Dupont",
        passwordHash: passwordHash,
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
      },
      {
        _id: new ObjectId(),
        email: "pierre.martin@example.com",
        firstName: "Pierre",
        lastName: "Martin",
        passwordHash: passwordHash,
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
        _id: new ObjectId(),
        email: "sophie.bernard@example.com",
        firstName: "Sophie",
        lastName: "Bernard",
        passwordHash: passwordHash,
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
        _id: new ObjectId(),
        email: "lucas.dubois@example.com",
        firstName: "Lucas",
        lastName: "Dubois",
        passwordHash: passwordHash,
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
    console.log('➡️  Vérification des utilisateurs existants...\n');
    const usersToInsert = [];

    for (const user of newUsers) {
      const exists = await usersCollection.findOne({ email: user.email });
      if (!exists) {
        usersToInsert.push(user);
      } else {
        console.log(`⚠️  L'utilisateur ${user.email} existe déjà`);
      }
    }

    // Insérer les nouveaux utilisateurs
    if (usersToInsert.length > 0) {
      await usersCollection.insertMany(usersToInsert);
      console.log(`\n✅ ${usersToInsert.length} nouveaux utilisateurs créés!\n`);
      console.log('👥 Comptes créés:');
      usersToInsert.forEach(user => {
        console.log(`   📧 ${user.email} | ${user.firstName} ${user.lastName}`);
      });
      console.log('\n   🔑 Mot de passe pour tous: User123!');
    } else {
      console.log('\nℹ️  Aucun nouvel utilisateur à créer\n');
    }

    // ==============================================
    // AFFICHER TOUS LES UTILISATEURS
    // ==============================================

    console.log('\n➡️  Liste de tous les utilisateurs dans la base:\n');

    const allUsers = await usersCollection.find({}, {
      projection: {
        email: 1,
        firstName: 1,
        lastName: 1,
        role: 1,
        phoneNumber: 1,
        isActive: 1,
        _id: 0
      }
    }).toArray();

    allUsers.forEach(user => {
      const status = user.isActive ? '✅' : '❌';
      const roleIcon = user.role === 'Admin' ? '👑' : '👤';
      const email = (user.email || '').padEnd(35);
      console.log(`${status} ${roleIcon} ${email} | ${user.firstName} ${user.lastName} | ${user.role}`);
    });

    console.log('\n✅ Script terminé!\n');

  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
    console.error(error);
  } finally {
    await client.close();
    console.log('📪 Connexion fermée\n');
  }
}

// Exécuter le script
createUsers();
