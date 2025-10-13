// ==============================================
// METTRE À JOUR LES HASH DE MOT DE PASSE
// Remplace les hash $2b$ (bcryptjs) par des hash $2a$ (BCrypt.Net)
// ==============================================
y
const { MongoClient } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';(())
const databaseName = 'ECommerceDB';

// Hash BCrypt.Net valide pour "User123!" généré avec BCrypt.Net
const VALID_HASH_USER123 = '$2a$11$0HPVoPle1puN2I3qHzWrmeGXwT/RyPm67k5J4hPnAi/ZVH4e0OIAu';

async function updatePasswordHashes() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n📦 Connexion à MongoDB...\n');
    await client.connect();
    console.log('✅ Connecté à MongoDB!\n');

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // Trouver tous les utilisateurs avec des hash $2b$ (bcryptjs)
    const usersWithBadHash = await usersCollection.find({
      passwordHash: { $regex: /^\$2b\$/ }
    }).toArray();

    console.log(`➡️  Trouvé ${usersWithBadHash.length} utilisateur(s) avec des hash incompatibles\n`);

    if (usersWithBadHash.length === 0) {
      console.log('✅ Tous les hash sont déjà compatibles!\n');
      return;
    }

    // Afficher les utilisateurs à corriger
    console.log('👥 Utilisateurs à corriger:');
    usersWithBadHash.forEach(user => {
      console.log(`   📧 ${user.email || 'Sans email'}`);
    });

    console.log('\n➡️  Mise à jour des hash avec le hash valide pour "User123!"...\n');

    // Mettre à jour chaque utilisateur
    let updatedCount = 0;
    for (const user of usersWithBadHash) {
      await usersCollection.updateOne(
        { _id: user._id },
        {
          $set: {
            passwordHash: VALID_HASH_USER123,
            updatedAt: new Date()
          }
        }
      );
      console.log(`✅ ${user.email || 'Sans email'} mis à jour`);
      updatedCount++;
    }

    console.log(`\n✅ ${updatedCount} utilisateur(s) mis à jour avec succès!`);
    console.log('🔑 Tous ces utilisateurs peuvent maintenant se connecter avec: User123!\n');

  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
  } finally {
    await client.close();
    console.log('📪 Connexion fermée\n');
  }
}

updatePasswordHashes();
