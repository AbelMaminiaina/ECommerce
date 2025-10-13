// ==============================================
// GESTION DES UTILISATEURS VIA L'API
// Script pour modifier les utilisateurs facilement
// ==============================================

const API_BASE_URL = 'http://localhost:5000/api';

// Variables globales
let adminToken = null;
let allUsers = [];

// ==========================================
// 1. SE CONNECTER EN TANT QU'ADMIN
// ==========================================
async function loginAdmin() {
  console.log('🔐 Connexion admin...\n');

  try {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email: 'admin@example.com',
        password: 'Admin123!'
      })
    });

    if (!response.ok) {
      throw new Error('Échec de connexion admin');
    }

    const data = await response.json();
    adminToken = data.accessToken;

    console.log('✅ Admin connecté avec succès!');
    console.log('   👤 Utilisateur:', data.user.email);
    console.log('   🎭 Rôle:', data.user.role);
    console.log('');

    return true;
  } catch (error) {
    console.error('❌ Erreur de connexion:', error.message);
    console.log('   💡 Assurez-vous que le backend est démarré et que l\'admin existe\n');
    return false;
  }
}

// ==========================================
// 2. LISTER TOUS LES UTILISATEURS
// ==========================================
async function listUsers() {
  console.log('📋 Liste des utilisateurs:\n');

  try {
    const response = await fetch(`${API_BASE_URL}/users`, {
      headers: { 'Authorization': `Bearer ${adminToken}` }
    });

    if (!response.ok) {
      throw new Error('Impossible de récupérer les utilisateurs');
    }

    allUsers = await response.json();

    console.log(`   Total: ${allUsers.length} utilisateur(s)\n`);

    allUsers.forEach((user, index) => {
      const status = user.isActive ? '✅' : '❌';
      const roleIcon = user.role === 'Admin' ? '👑' : '👤';
      console.log(`   ${index + 1}. ${status} ${roleIcon} ${user.email}`);
      console.log(`      ID: ${user.id}`);
      console.log(`      Nom: ${user.firstName} ${user.lastName}`);
      console.log(`      Rôle: ${user.role} | Actif: ${user.isActive}`);
      console.log('');
    });

    return allUsers;
  } catch (error) {
    console.error('❌ Erreur:', error.message);
    return [];
  }
}

// ==========================================
// 3. MODIFIER LE RÔLE D'UN UTILISATEUR
// ==========================================
async function changeRole(userEmail, newRole) {
  console.log(`\n🔄 Changement de rôle: ${userEmail} → ${newRole}\n`);

  const user = allUsers.find(u => u.email === userEmail);
  if (!user) {
    console.log('❌ Utilisateur non trouvé\n');
    return;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/users/${user.id}/role`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${adminToken}`
      },
      body: JSON.stringify({ role: newRole })
    });

    if (response.ok) {
      const data = await response.json();
      console.log('✅ Rôle modifié avec succès!');
      console.log(`   ${data.email}: ${data.role}\n`);
    } else {
      const error = await response.text();
      console.log('❌ Erreur:', error, '\n');
    }
  } catch (error) {
    console.error('❌ Erreur:', error.message, '\n');
  }
}

// ==========================================
// 4. ACTIVER/DÉSACTIVER UN UTILISATEUR
// ==========================================
async function toggleStatus(userEmail, isActive) {
  console.log(`\n🔄 Modification du statut: ${userEmail} → ${isActive ? 'ACTIF' : 'INACTIF'}\n`);

  const user = allUsers.find(u => u.email === userEmail);
  if (!user) {
    console.log('❌ Utilisateur non trouvé\n');
    return;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/users/${user.id}/status`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${adminToken}`
      },
      body: JSON.stringify({ isActive })
    });

    if (response.ok) {
      const data = await response.json();
      console.log('✅ Statut modifié avec succès!');
      console.log(`   ${data.email}: ${data.isActive ? 'ACTIF ✅' : 'INACTIF ❌'}\n`);
    } else {
      const error = await response.text();
      console.log('❌ Erreur:', error, '\n');
    }
  } catch (error) {
    console.error('❌ Erreur:', error.message, '\n');
  }
}

// ==========================================
// 5. MODIFIER LES INFORMATIONS
// ==========================================
async function updateInfo(userEmail, updates) {
  console.log(`\n🔄 Modification des infos: ${userEmail}\n`);

  const user = allUsers.find(u => u.email === userEmail);
  if (!user) {
    console.log('❌ Utilisateur non trouvé\n');
    return;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/users/${user.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${adminToken}`
      },
      body: JSON.stringify({
        ...updates,
        addresses: user.addresses // Garder les adresses existantes si non modifiées
      })
    });

    if (response.ok) {
      const data = await response.json();
      console.log('✅ Informations modifiées avec succès!');
      console.log(`   ${data.email}: ${data.firstName} ${data.lastName}\n`);
    } else {
      const error = await response.text();
      console.log('❌ Erreur:', error, '\n');
    }
  } catch (error) {
    console.error('❌ Erreur:', error.message, '\n');
  }
}

// ==========================================
// 6. SUPPRIMER UN UTILISATEUR
// ==========================================
async function deleteUser(userEmail) {
  console.log(`\n🗑️  Suppression: ${userEmail}\n`);

  const user = allUsers.find(u => u.email === userEmail);
  if (!user) {
    console.log('❌ Utilisateur non trouvé\n');
    return;
  }

  try {
    const response = await fetch(`${API_BASE_URL}/users/${user.id}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${adminToken}` }
    });

    if (response.ok) {
      console.log('✅ Utilisateur supprimé avec succès!\n');
    } else {
      const error = await response.text();
      console.log('❌ Erreur:', error, '\n');
    }
  } catch (error) {
    console.error('❌ Erreur:', error.message, '\n');
  }
}

// ==========================================
// MENU PRINCIPAL
// ==========================================
async function main() {
  console.log('\n' + '='.repeat(50));
  console.log('    GESTION DES UTILISATEURS');
  console.log('='.repeat(50) + '\n');

  // 1. Se connecter
  const connected = await loginAdmin();
  if (!connected) {
    console.log('⚠️  Impossible de continuer sans connexion admin\n');
    return;
  }

  // 2. Lister les utilisateurs
  await listUsers();

  // ==========================================
  // EXEMPLES D'UTILISATION
  // Décommentez les actions que vous voulez effectuer
  // ==========================================

  // Exemple 1: Changer le rôle d'un utilisateur
  // await changeRole('user@example.com', 'Admin');

  // Exemple 2: Activer un utilisateur
  // await toggleStatus('admin@example.com', true);

  // Exemple 3: Désactiver un utilisateur
  // await toggleStatus('user@example.com', false);

  // Exemple 4: Modifier les informations
  // await updateInfo('user@example.com', {
  //   firstName: 'Jean',
  //   lastName: 'Dupont',
  //   phoneNumber: '+33612345678'
  // });

  // Exemple 5: Supprimer un utilisateur
  // await deleteUser('marie.dupont@example.com');

  // Relister après modifications
  // await listUsers();

  console.log('💡 Décommentez les exemples ci-dessus pour effectuer des modifications\n');
  console.log('='.repeat(50) + '\n');
}

// Exécuter le script
main();
