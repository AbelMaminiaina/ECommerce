// ==============================================
// ACTIVER L'UTILISATEUR VIA L'API
// ==============================================

const API_BASE_URL = 'http://localhost:5000/api';

let adminToken = null;

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

    console.log('✅ Admin connecté avec succès!\n');
    return true;
  } catch (error) {
    console.error('❌ Erreur de connexion:', error.message);
    return false;
  }
}

async function listUsers() {
  console.log('📋 Liste des utilisateurs:\n');

  try {
    const response = await fetch(`${API_BASE_URL}/users`, {
      headers: { 'Authorization': `Bearer ${adminToken}` }
    });

    if (!response.ok) {
      throw new Error('Impossible de récupérer les utilisateurs');
    }

    const allUsers = await response.json();

    console.log(`   Total: ${allUsers.length} utilisateur(s)\n`);

    allUsers.forEach((user, index) => {
      const status = user.isActive ? '✅ ACTIF' : '❌ INACTIF';
      const roleIcon = user.role === 'Admin' ? '👑' : '👤';
      console.log(`   ${index + 1}. ${status} ${roleIcon} ${user.email}`);
      console.log(`      ID: ${user.id}`);
      console.log(`      Nom: ${user.firstName} ${user.lastName}`);
      console.log(`      Rôle: ${user.role}`);
      console.log('');
    });

    return allUsers;
  } catch (error) {
    console.error('❌ Erreur:', error.message);
    return [];
  }
}

async function activateUser(userEmail) {
  console.log(`\n🔄 Activation de l'utilisateur: ${userEmail}\n`);

  try {
    // Récupérer tous les utilisateurs
    const response = await fetch(`${API_BASE_URL}/users`, {
      headers: { 'Authorization': `Bearer ${adminToken}` }
    });

    if (!response.ok) {
      throw new Error('Impossible de récupérer les utilisateurs');
    }

    const allUsers = await response.json();
    const user = allUsers.find(u => u.email === userEmail);

    if (!user) {
      console.log('❌ Utilisateur non trouvé\n');
      return;
    }

    console.log(`   Utilisateur trouvé: ${user.firstName} ${user.lastName}`);
    console.log(`   Statut actuel: ${user.isActive ? 'ACTIF' : 'INACTIF'}\n`);

    // Activer l'utilisateur
    const activateResponse = await fetch(`${API_BASE_URL}/users/${user.id}/status`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${adminToken}`
      },
      body: JSON.stringify({ isActive: true })
    });

    if (activateResponse.ok) {
      const data = await activateResponse.json();
      console.log('✅ Utilisateur activé avec succès!');
      console.log(`   ${data.email}: ${data.isActive ? 'ACTIF ✅' : 'INACTIF ❌'}\n`);
    } else {
      const error = await activateResponse.text();
      console.log('❌ Erreur:', error, '\n');
    }
  } catch (error) {
    console.error('❌ Erreur:', error.message, '\n');
  }
}

async function testLogin(email, password) {
  console.log(`\n🔐 Test de connexion pour: ${email}\n`);

  try {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ email, password })
    });

    console.log('   📡 Status:', response.status, response.statusText);

    if (response.ok) {
      const data = await response.json();
      console.log('   ✅ Connexion réussie!');
      console.log('   👤 Utilisateur:', data.user?.firstName, data.user?.lastName);
      console.log('   🎭 Rôle:', data.user?.role);
      console.log('   🔑 Token:', data.accessToken?.substring(0, 20) + '...\n');
    } else {
      const error = await response.text();
      console.log('   ❌ Échec de connexion');
      console.log('   💬 Erreur:', error, '\n');
    }
  } catch (error) {
    console.log('   ❌ Erreur de connexion:', error.message, '\n');
  }
}

async function main() {
  console.log('\n' + '='.repeat(50));
  console.log('    ACTIVATION DE L\'UTILISATEUR user@example.com');
  console.log('='.repeat(50) + '\n');

  // 1. Se connecter en tant qu'admin
  const connected = await loginAdmin();
  if (!connected) {
    console.log('⚠️  Impossible de continuer sans connexion admin\n');
    return;
  }

  // 2. Lister les utilisateurs AVANT
  console.log('📋 AVANT:');
  await listUsers();

  // 3. Activer user@example.com
  await activateUser('user@example.com');

  // 4. Lister les utilisateurs APRÈS
  console.log('📋 APRÈS:');
  await listUsers();

  // 5. Tester la connexion
  await testLogin('user@example.com', 'User123!');

  console.log('='.repeat(50) + '\n');
}

main();
