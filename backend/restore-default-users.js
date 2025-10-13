// ==============================================
// RESTAURER LES UTILISATEURS PAR DÉFAUT
// Recrée admin@example.com et user@example.com
// ==============================================

const { MongoClient, ObjectId } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';

// Hash BCrypt.Net valides
const ADMIN_HASH = '$2a$11$XcSw4ju5pAaI125wr1UDpe2mSRUN78nqhHqW5uJH8kAvK18Sa1BrS'; // Admin123!
const USER_HASH = '$2a$11$PgT4JoRKE/PRZESAJKMcPedczOR9v.2tza9zJBj/9/cO.ss0E0.J2';  // User123!

async function restoreDefaultUsers() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n📦 Connexion à MongoDB...\n');
    await client.connect();
    console.log('✅ Connecté à MongoDB!\n');

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // ==============================================
    // CRÉER L'UTILISATEUR ADMIN
    // ==============================================

    const adminExists = await usersCollection.findOne({ email: 'admin@example.com' });

    if (!adminExists) {
      const adminUser = {
        _id: new ObjectId(),
        email: "admin@example.com",
        firstName: "Admin",
        lastName: "User",
        passwordHash: ADMIN_HASH,
        role: "Admin",
        phoneNumber: "+1234567890",
        addresses: [],
        isActive: true,
        refreshToken: null,
        refreshTokenExpiryTime: null,
        createdAt: new Date(),
        updatedAt: new Date()
      };

      await usersCollection.insertOne(adminUser);
      console.log('✅ Utilisateur admin créé avec succès!');
      console.log('   📧 Email: admin@example.com');
      console.log('   🔑 Mot de passe: Admin123!\n');
    } else {
      console.log('⚠️  L\'utilisateur admin existe déjà\n');
    }

    // ==============================================
    // CRÉER L'UTILISATEUR TEST
    // ==============================================

    const userExists = await usersCollection.findOne({ email: 'user@example.com' });

    if (!userExists) {
      const testUser = {
        _id: new ObjectId(),
        email: "user@example.com",
        firstName: "John",
        lastName: "Doe",
        passwordHash: USER_HASH,
        role: "Customer",
        phoneNumber: "+1234567891",
        addresses: [
          {
            street: "123 Main St",
            city: "New York",
            state: "NY",
            zipCode: "10001",
            country: "USA",
            isDefault: true
          }
        ],
        isActive: true,
        refreshToken: null,
        refreshTokenExpiryTime: null,
        createdAt: new Date(),
        updatedAt: new Date()
      };

      await usersCollection.insertOne(testUser);
      console.log('✅ Utilisateur test créé avec succès!');
      console.log('   📧 Email: user@example.com');
      console.log('   🔑 Mot de passe: User123!\n');
    } else {
      console.log('⚠️  L\'utilisateur test existe déjà\n');
    }

    // ==============================================
    // AFFICHER TOUS LES UTILISATEURS
    // ==============================================

    console.log('➡️  Liste de tous les utilisateurs dans la base:\n');

    const allUsers = await usersCollection.find(
      { email: { $exists: true, $ne: null } },
      { projection: { email: 1, firstName: 1, lastName: 1, role: 1, isActive: 1, _id: 0 } }
    ).toArray();

    allUsers.forEach(user => {
      const status = user.isActive ? '✅' : '❌';
      const roleIcon = user.role === 'Admin' ? '👑' : '👤';
      const email = (user.email || '').padEnd(35);
      console.log(`${status} ${roleIcon} ${email} | ${user.firstName} ${user.lastName} | ${user.role}`);
    });

    console.log('\n✅ Restauration terminée!\n');

  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
  } finally {
    await client.close();
    console.log('📪 Connexion fermée\n');
  }
}

restoreDefaultUsers();
