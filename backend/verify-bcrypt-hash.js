// ==============================================
// VÉRIFIER LE HASH BCRYPT
// ==============================================

const { MongoClient } = require('mongodb');
const bcrypt = require('bcryptjs');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';

// Hash attendu pour User123!
const EXPECTED_HASH = '$2a$11$PgT4JoRKE/PRZESAJKMcPedczOR9v.2tza9zJBj/9/cO.ss0E0.J2';
const PASSWORD = 'User123!';

async function verifyHash() {
  const client = new MongoClient(connectionString);

  try {
    console.log('\n📦 Connexion à MongoDB...\n');
    await client.connect();

    const db = client.db(databaseName);
    const usersCollection = db.collection('users');

    // Récupérer user@example.com
    const user = await usersCollection.findOne({ email: 'user@example.com' });

    if (!user) {
      console.log('❌ Utilisateur user@example.com introuvable!\n');
      return;
    }

    console.log('✅ Utilisateur trouvé!\n');
    console.log(`   📧 Email: ${user.email}`);
    console.log(`   👤 Nom: ${user.firstName} ${user.lastName}`);
    console.log(`   ✅ Actif: ${user.isActive}`);
    console.log('');

    console.log('🔑 VÉRIFICATION DU HASH:\n');
    console.log(`   Hash en base:`);
    console.log(`   ${user.passwordHash}`);
    console.log('');
    console.log(`   Hash attendu:`);
    console.log(`   ${EXPECTED_HASH}`);
    console.log('');
    console.log(`   Identiques: ${user.passwordHash === EXPECTED_HASH ? '✅ OUI' : '❌ NON'}`);
    console.log('');

    // Test BCrypt avec le hash de la base
    console.log('🔐 TEST BCRYPT:\n');
    console.log(`   Mot de passe testé: "${PASSWORD}"`);
    console.log('');

    try {
      const isValid = await bcrypt.compare(PASSWORD, user.passwordHash);
      console.log(`   Résultat BCrypt.compare(): ${isValid ? '✅ VALIDE' : '❌ INVALIDE'}`);
      console.log('');

      if (!isValid) {
        console.log('⚠️  Le mot de passe ne correspond PAS au hash en base!\n');
        console.log('💡 Essayons de créer un nouveau hash pour User123!...\n');

        const newHash = await bcrypt.hash(PASSWORD, 11);
        console.log(`   Nouveau hash généré:`);
        console.log(`   ${newHash}`);
        console.log('');

        // Mettre à jour le hash
        console.log('🔄 Mise à jour du hash en base...\n');
        await usersCollection.updateOne(
          { email: 'user@example.com' },
          { $set: { passwordHash: newHash, updatedAt: new Date() } }
        );

        console.log('✅ Hash mis à jour!\n');

        // Tester à nouveau
        const isValidNow = await bcrypt.compare(PASSWORD, newHash);
        console.log(`   Nouveau test: ${isValidNow ? '✅ VALIDE' : '❌ INVALIDE'}`);
        console.log('');
      } else {
        console.log('✅ Le hash est correct! Le problème doit être ailleurs.\n');
      }
    } catch (err) {
      console.error('❌ Erreur BCrypt:', err.message);
    }

  } catch (error) {
    console.error('\n❌ Erreur:', error.message);
  } finally {
    await client.close();
    console.log('📪 Connexion fermée\n');
  }
}

verifyHash();
