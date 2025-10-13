// ==============================================
// TESTER LA CONNEXION VIA L'API
// ==============================================

async function testLogin(email, password) {
  console.log(`\n🔐 Test de connexion pour: ${email}`);

  try {
    const response = await fetch('http://localhost:5000/api/auth/login', {
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
      console.log('   🔑 Token:', data.accessToken?.substring(0, 20) + '...');
    } else {
      const error = await response.text();
      console.log('   ❌ Échec de connexion');
      console.log('   💬 Erreur:', error);
    }
  } catch (error) {
    console.log('   ❌ Erreur de connexion:', error.message);
  }
}

async function runTests() {
  console.log('\n🧪 Test de connexion via l\'API\n');
  console.log('='.repeat(50));

  // Tester admin
  await testLogin('admin@example.com', 'Admin123!');

  // Tester user
  await testLogin('user@example.com', 'User123!');

  // Tester avec mauvais mot de passe
  await testLogin('admin@example.com', 'WrongPassword');

  console.log('\n' + '='.repeat(50) + '\n');
}

runTests();
