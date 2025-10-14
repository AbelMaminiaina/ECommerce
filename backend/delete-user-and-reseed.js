// ==============================================
// SUPPRIMER L'UTILISATEUR ET LAISSER .NET LE RECRÉER
// ==============================================

const { MongoClient } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';

async function deleteAndReseed() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n📦 Connexion à MongoDB...\n');
    await client.connect();
    console.log('✅ Connecté à MongoDB!\n');

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // 1. Supprimer user@example.com
    console.log('🗑️  Suppression de user@example.com...\n');
    const deleteResult = await usersCollection.deleteOne({ email: 'user@example.com' });

    if (deleteResult.deletedCount > 0) {
      console.log('✅ Utilisateur supprimé avec succès!\n');
    } else {
      console.log('⚠️  Utilisateur non trouvé\n');
    }

    // 2. Afficher les utilisateurs restants
    console.log('📋 Utilisateurs restants:\n');
    const allUsers = await usersCollection.find({}).toArray();

    allUsers.forEach((user, index) => {
      const status = user.isActive ? '✅' : '❌';
      const roleIcon = user.role === 'Admin' ? '👑' : '👤';
      console.log(`   ${index + 1}. ${status} ${roleIcon} ${user.email}`);
      console.log(`      Nom: ${user.firstName} ${user.lastName}`);
      console.log('');
    });

    console.log('💡 Maintenant, redémarrez le backend .NET pour que le seeding recrée l\'utilisateur.\n');
    console.log('   Commande: cd backend/src/ECommerce.API && dotnet run\n');

  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
  } finally {
    await client.close();
    console.log('📪 Connexion fermée\n');
  }
}

deleteAndReseed();
