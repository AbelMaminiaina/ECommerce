// ==============================================
// ACTIVER UN UTILISATEUR
// ==============================================

const { MongoClient } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';
const emailToActivate = 'user@example.com';

async function activateUser() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n📦 Connexion à MongoDB...\n');
    await client.connect();
    console.log('✅ Connecté à MongoDB!\n');

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // Trouver et activer l'utilisateur
    const result = await usersCollection.updateOne(
      { email: emailToActivate },
      { $set: { isActive: true, updatedAt: new Date() } }
    );

    if (result.matchedCount === 0) {
      console.log(`❌ Aucun utilisateur trouvé avec l'email: ${emailToActivate}\n`);
    } else if (result.modifiedCount > 0) {
      console.log(`✅ Utilisateur ${emailToActivate} activé avec succès!\n`);

      // Afficher l'utilisateur mis à jour
      const user = await usersCollection.findOne(
        { email: emailToActivate },
        { projection: { email: 1, firstName: 1, lastName: 1, role: 1, isActive: 1, _id: 0 } }
      );

      console.log('Détails de l\'utilisateur:');
      console.log(`   📧 Email: ${user.email}`);
      console.log(`   👤 Nom: ${user.firstName} ${user.lastName}`);
      console.log(`   🎭 Rôle: ${user.role}`);
      console.log(`   ✅ Actif: ${user.isActive}\n`);
    } else {
      console.log(`⚠️  L'utilisateur ${emailToActivate} était déjà actif\n`);
    }

  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
  } finally {
    await client.close();
    console.log('📪 Connexion fermée\n');
  }
}

activateUser();
