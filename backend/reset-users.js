// ==============================================
// RÉINITIALISER LES UTILISATEURS
// Supprime tous les utilisateurs pour les recréer
// ==============================================

const { MongoClient } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';

async function resetUsers() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n🗑️  Réinitialisation des utilisateurs...\n');
    await client.connect();
    console.log('✅ Connecté à MongoDB!\n');

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // Compter les utilisateurs actuels
    const count = await usersCollection.countDocuments();
    console.log(`📊 Nombre d'utilisateurs actuels: ${count}\n`);

    if (count > 0) {
      // Supprimer tous les utilisateurs
      const result = await usersCollection.deleteMany({});
      console.log(`✅ ${result.deletedCount} utilisateur(s) supprimé(s)\n`);
    } else {
      console.log('ℹ️  Aucun utilisateur à supprimer\n');
    }

    console.log('📝 Instructions:');
    console.log('   1. Redémarrez le backend: cd backend/src/ECommerce.API && dotnet run');
    console.log('   2. Le SeedData va recréer automatiquement:');
    console.log('      - admin@example.com (mot de passe: Admin123!)');
    console.log('      - user@example.com (mot de passe: User123!)\n');

    await client.close();
    console.log('📪 Connexion fermée\n');
  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
  }
}

resetUsers();
