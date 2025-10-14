// ==============================================
// NETTOYER LA BASE ET TESTER
// ==============================================

const { MongoClient, ObjectId } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';

async function cleanupAndTest() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n📦 Connexion à MongoDB...\n');
    await client.connect();
    console.log('✅ Connecté à MongoDB!\n');

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // 1. Supprimer l'utilisateur corrompu (celui sans email valide)
    console.log('🗑️  Suppression des utilisateurs corrompus...\n');
    const deleteResult = await usersCollection.deleteMany({
      $or: [
        { email: { $exists: false } },
        { email: null },
        { email: undefined },
        { email: '' },
        { firstName: { $exists: false } },
        { role: { $exists: false } }
      ]
    });

    console.log(`   Supprimé: ${deleteResult.deletedCount} utilisateur(s) corrompu(s)\n`);

    // 2. Afficher tous les utilisateurs restants
    console.log('📋 Utilisateurs dans la base:\n');
    const allUsers = await usersCollection.find({}).toArray();

    allUsers.forEach((user, index) => {
      const status = user.isActive ? '✅ ACTIF' : '❌ INACTIF';
      const roleIcon = user.role === 'Admin' ? '👑' : '👤';
      console.log(`   ${index + 1}. ${status} ${roleIcon} ${user.email}`);
      console.log(`      Nom: ${user.firstName} ${user.lastName}`);
      console.log(`      Rôle: ${user.role}`);
      console.log('');
    });

    // 3. Tester le login avec l'API
    console.log('🔐 Test de connexion via l\'API...\n');

    const loginResponse = await fetch('http://localhost:5000/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: 'user@example.com',
        password: 'User123!'
      })
    });

    console.log('   📡 Status:', loginResponse.status, loginResponse.statusText);

    if (loginResponse.ok) {
      const data = await loginResponse.json();
      console.log('\n   ✅✅✅ CONNEXION RÉUSSIE! ✅✅✅');
      console.log(`   👤 Utilisateur: ${data.user.firstName} ${data.user.lastName}`);
      console.log(`   📧 Email: ${data.user.email}`);
      console.log(`   🎭 Rôle: ${data.user.role}`);
      console.log(`   🔑 Token: ${data.accessToken.substring(0, 30)}...\n`);
    } else {
      const error = await loginResponse.text();
      console.log('\n   ❌ Échec de connexion');
      console.log(`   💬 Erreur: ${error}\n`);
    }

  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
  } finally {
    await client.close();
    console.log('📪 Connexion fermée\n');
  }
}

cleanupAndTest();
