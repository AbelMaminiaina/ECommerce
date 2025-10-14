// ==============================================
// VÉRIFIER DIRECTEMENT LA BASE DE DONNÉES
// ==============================================

const { MongoClient } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';

async function checkDatabase() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n📦 Connexion à MongoDB...\n');
    await client.connect();
    console.log('✅ Connecté à MongoDB!\n');

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // Compter tous les utilisateurs
    const totalUsers = await usersCollection.countDocuments({});
    console.log(`📊 Total d'utilisateurs dans la base: ${totalUsers}\n`);

    // Chercher spécifiquement user@example.com
    console.log('🔍 Recherche de user@example.com...\n');
    const users = await usersCollection.find({ email: 'user@example.com' }).toArray();

    console.log(`   Trouvé: ${users.length} utilisateur(s) avec cet email\n`);

    users.forEach((user, index) => {
      console.log(`   === UTILISATEUR ${index + 1} ===`);
      console.log(`   📧 Email: ${user.email}`);
      console.log(`   👤 Nom: ${user.firstName} ${user.lastName}`);
      console.log(`   🎭 Rôle: ${user.role}`);
      console.log(`   ✅ Actif: ${user.isActive}`);
      console.log(`   🔑 Hash (début): ${user.passwordHash?.substring(0, 30)}...`);
      console.log(`   🆔 ID: ${user._id}`);
      console.log(`   📅 Créé: ${user.createdAt}`);
      console.log(`   📅 Modifié: ${user.updatedAt}`);
      console.log('');
    });

    // Afficher TOUS les utilisateurs de la base
    console.log('\n📋 TOUS LES UTILISATEURS DANS LA BASE:\n');
    const allUsers = await usersCollection.find({}).toArray();

    allUsers.forEach((user, index) => {
      const status = user.isActive ? '✅' : '❌';
      const roleIcon = user.role === 'Admin' ? '👑' : '👤';
      console.log(`   ${index + 1}. ${status} ${roleIcon} ${user.email}`);
      console.log(`      ID: ${user._id}`);
      console.log(`      Nom: ${user.firstName} ${user.lastName}`);
      console.log(`      Rôle: ${user.role}`);
      console.log(`      Hash (début): ${user.passwordHash?.substring(0, 30)}...`);
      console.log('');
    });

  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
  } finally {
    await client.close();
    console.log('📪 Connexion fermée\n');
  }
}

checkDatabase();
