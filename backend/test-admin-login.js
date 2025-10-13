// ==============================================
// TESTER LA CONNEXION ADMIN
// ==============================================

const bcrypt = require('bcryptjs');
const { MongoClient } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';

async function testAdminLogin() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n📦 Test de connexion admin...\n');
    await client.connect();

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // Récupérer l'utilisateur admin
    const admin = await usersCollection.findOne({ email: 'admin@example.com' });

    if (!admin) {
      console.log('❌ Utilisateur admin non trouvé!\n');
      return;
    }

    console.log('✅ Utilisateur admin trouvé:');
    console.log('   📧 Email:', admin.email);
    console.log('   👤 Nom:', admin.firstName, admin.lastName);
    console.log('   🎭 Rôle:', admin.role);
    console.log('   🔓 Actif:', admin.isActive);
    console.log('   🔐 Hash:', admin.passwordHash.substring(0, 20) + '...\n');

    // Tester différents mots de passe
    console.log('🧪 Test des mots de passe:\n');

    const passwords = [
      'Admin123!',
      'admin123!',
      'Admin123',
      'User123!'
    ];

    for (const password of passwords) {
      const isValid = bcrypt.compareSync(password, admin.passwordHash);
      const icon = isValid ? '✅' : '❌';
      console.log(`   ${icon} "${password}": ${isValid ? 'VALIDE' : 'INVALIDE'}`);
    }

    console.log('\n');

    await client.close();
  } catch (error) {
    console.error('❌ Erreur:', error.message);
  }
}

testAdminLogin();
