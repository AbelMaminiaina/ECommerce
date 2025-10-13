// ==============================================
// MODIFIER UN UTILISATEUR
// Script pour modifier directement dans MongoDB
// ==============================================

const { MongoClient } = require('mongodb');

const connectionString = 'mongodb://localhost:27017';
const databaseName = 'ECommerceDB';

// ==========================================
// EXEMPLES D'UTILISATION
// ==========================================

// 1. Activer/Désactiver un utilisateur
async function toggleUserStatus(email, isActive) {
  const client = new MongoClient(connectionString);
  try {
    await client.connect();
    const db = client.db(databaseName);
    const result = await db.collection('users').updateOne(
      { email: email },
      {
        $set: {
          isActive: isActive,
          updatedAt: new Date()
        }
      }
    );
    console.log(`✅ Utilisateur ${email} ${isActive ? 'activé' : 'désactivé'}`);
    return result;
  } finally {
    await client.close();
  }
}

// 2. Changer le rôle d'un utilisateur
async function changeUserRole(email, role) {
  const client = new MongoClient(connectionString);
  try {
    await client.connect();
    const db = client.db(databaseName);
    const result = await db.collection('users').updateOne(
      { email: email },
      {
        $set: {
          role: role,
          updatedAt: new Date()
        }
      }
    );
    console.log(`✅ Rôle de ${email} changé en: ${role}`);
    return result;
  } finally {
    await client.close();
  }
}

// 3. Modifier les informations personnelles
async function updateUserInfo(email, updates) {
  const client = new MongoClient(connectionString);
  try {
    await client.connect();
    const db = client.db(databaseName);
    const result = await db.collection('users').updateOne(
      { email: email },
      {
        $set: {
          ...updates,
          updatedAt: new Date()
        }
      }
    );
    console.log(`✅ Informations de ${email} mises à jour`);
    return result;
  } finally {
    await client.close();
  }
}

// 4. Changer le mot de passe (nécessite un hash BCrypt)
async function changePassword(email, newPasswordHash) {
  const client = new MongoClient(connectionString);
  try {
    await client.connect();
    const db = client.db(databaseName);
    const result = await db.collection('users').updateOne(
      { email: email },
      {
        $set: {
          passwordHash: newPasswordHash,
          updatedAt: new Date()
        }
      }
    );
    console.log(`✅ Mot de passe de ${email} modifié`);
    return result;
  } finally {
    await client.close();
  }
}

// 5. Ajouter une adresse
async function addAddress(email, address) {
  const client = new MongoClient(connectionString);
  try {
    await client.connect();
    const db = client.db(databaseName);
    const result = await db.collection('users').updateOne(
      { email: email },
      {
        $push: { addresses: address },
        $set: { updatedAt: new Date() }
      }
    );
    console.log(`✅ Adresse ajoutée pour ${email}`);
    return result;
  } finally {
    await client.close();
  }
}

// ==========================================
// EXÉCUTION - Décommentez ce que vous voulez faire
// ==========================================

(async () => {
  console.log('\n🔧 Modification d\'utilisateur\n');

  try {
    // Exemple 1: Activer l'admin
    // await toggleUserStatus('admin@example.com', true);

    // Exemple 2: Changer un utilisateur en Admin
    // await changeUserRole('user@example.com', 'Admin');

    // Exemple 3: Modifier les informations
    // await updateUserInfo('user@example.com', {
    //   firstName: 'Jean',
    //   lastName: 'Dupont',
    //   phoneNumber: '+33612345678'
    // });

    // Exemple 4: Changer le mot de passe (hash pour "User123!")
    // await changePassword('user@example.com', '$2a$11$PgT4JoRKE/PRZESAJKMcPedczOR9v.2tza9zJBj/9/cO.ss0E0.J2');

    // Exemple 5: Ajouter une adresse
    // await addAddress('user@example.com', {
    //   street: '456 New Street',
    //   city: 'Paris',
    //   state: 'Île-de-France',
    //   zipCode: '75002',
    //   country: 'France',
    //   isDefault: false
    // });

    console.log('\n💡 Décommentez les exemples ci-dessus pour les utiliser\n');

  } catch (error) {
    console.error('❌ Erreur:', error.message);
  }
})();
