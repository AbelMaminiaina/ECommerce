// ==============================================
// TEST FINAL APRÈS REDÉMARRAGE DU BACKEND
// ==============================================

const { MongoClient } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';
const API_BASE_URL = 'http://localhost:5000/api';

async function finalTest() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n' + '='.repeat(60));
    console.log('    TEST FINAL DE CONNEXION');
    console.log('='.repeat(60) + '\n');

    // 1. Vérifier MongoDB
    console.log('📦 Connexion à MongoDB...\n');
    await client.connect();
    console.log('✅ MongoDB connecté!\n');

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // 2. Vérifier que user@example.com existe
    console.log('🔍 Vérification de user@example.com dans MongoDB...\n');
    const user = await usersCollection.findOne({ email: 'user@example.com' });

    if (!user) {
      console.log('❌ L\'utilisateur user@example.com n\'existe pas encore!');
      console.log('💡 Le backend doit être redémarré pour créer l\'utilisateur.\n');
      console.log('   Commandes:');
      console.log('   1. Arrêter le backend (Ctrl+C dans le terminal du backend)');
      console.log('   2. cd backend/src/ECommerce.API');
      console.log('   3. dotnet run');
      console.log('   4. Attendre que le backend démarre');
      console.log('   5. Re-exécuter ce script: node final-test.js\n');
      await client.close();
      return;
    }

    console.log('✅ Utilisateur trouvé dans MongoDB!');
    console.log(`   📧 Email: ${user.email}`);
    console.log(`   👤 Nom: ${user.firstName} ${user.lastName}`);
    console.log(`   🎭 Rôle: ${user.role}`);
    console.log(`   ✅ Actif: ${user.isActive}`);
    console.log(`   🔑 Hash (créé par): ${user.passwordHash?.substring(0, 7) === '$2a$11$' ? 'BCrypt.Net (C#) ✅' : 'bcryptjs (JS)'}`);
    console.log('');

    await client.close();

    // 3. Tester le login via l'API
    console.log('🔐 Test de connexion via l\'API...\n');
    console.log('   Email: user@example.com');
    console.log('   Password: User123!\n');

    try {
      const response = await fetch(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          email: 'user@example.com',
          password: 'User123!'
        })
      });

      console.log('   📡 Status:', response.status, response.statusText);
      console.log('');

      if (response.ok) {
        const data = await response.json();
        console.log('🎉🎉🎉 CONNEXION RÉUSSIE! 🎉🎉🎉\n');
        console.log(`   👤 Utilisateur: ${data.user.firstName} ${data.user.lastName}`);
        console.log(`   📧 Email: ${data.user.email}`);
        console.log(`   🎭 Rôle: ${data.user.role}`);
        console.log(`   🔑 Token: ${data.accessToken.substring(0, 30)}...\n`);
        console.log('✅ Le problème est résolu!\n');
      } else {
        const error = await response.text();
        console.log('❌ Échec de connexion');
        console.log(`   💬 Erreur: ${error}\n`);

        if (error.includes('inactive')) {
          console.log('💡 L\'utilisateur existe mais est inactif.');
          console.log('   Solution: Exécutez node activate-user.js\n');
        } else {
          console.log('💡 L\'utilisateur a peut-être été créé avec un hash incompatible.');
          console.log('   Solution: Redémarrez le backend .NET pour le recréer.\n');
        }
      }
    } catch (err) {
      console.log('❌ Erreur de connexion à l\'API');
      console.log(`   💬 ${err.message}`);
      console.log('\n💡 Le backend n\'est peut-être pas démarré.');
      console.log('   Démarrez-le avec: cd backend/src/ECommerce.API && dotnet run\n');
    }

    console.log('='.repeat(60) + '\n');

  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
    await client.close();
  }
}

finalTest();
