// ==============================================
// CRÉER DES UTILISATEURS VIA L'API
// Utilise l'endpoint POST /api/auth/register
// ==============================================

const API_BASE_URL = 'http://localhost:5000/api';

async function registerUser(userData) {
  try {
    const response = await fetch(`${API_BASE_URL}/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(userData)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || `HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    throw error;
  }
}

async function createUsers() {
  console.log('\n📦 Création d\'utilisateurs via l\'API...\n');

  const users = [
    {
      email: "marie.dupont@example.com",
      password: "User123!",
      firstName: "Marie",
      lastName: "Dupont",
      phoneNumber: "+33612345678"
    },
    {
      email: "pierre.martin@example.com",
      password: "User123!",
      firstName: "Pierre",
      lastName: "Martin",
      phoneNumber: "+33623456789"
    },
    {
      email: "sophie.bernard@example.com",
      password: "User123!",
      firstName: "Sophie",
      lastName: "Bernard",
      phoneNumber: "+33634567890"
    },
    {
      email: "lucas.dubois@example.com",
      password: "User123!",
      firstName: "Lucas",
      lastName: "Dubois",
      phoneNumber: "+33645678901"
    }
  ];

  let successCount = 0;
  let errorCount = 0;

  for (const user of users) {
    try {
      await registerUser(user);
      console.log(`✅ ${user.email} créé avec succès!`);
      successCount++;
    } catch (error) {
      if (error.message.includes('already exists')) {
        console.log(`⚠️  ${user.email} existe déjà`);
      } else {
        console.error(`❌ Erreur pour ${user.email}:`, error.message);
        errorCount++;
      }
    }
  }

  console.log('\n' + '='.repeat(50));
  console.log(`✅ ${successCount} utilisateurs créés`);
  if (errorCount > 0) {
    console.log(`❌ ${errorCount} erreurs`);
  }
  console.log('🔑 Mot de passe pour tous: User123!');
  console.log('='.repeat(50) + '\n');
}

// Vérifier si l'API est accessible
async function checkAPI() {
  try {
    const response = await fetch(`${API_BASE_URL}/products`);
    return response.ok;
  } catch (error) {
    return false;
  }
}

// Exécuter le script
(async () => {
  const apiAvailable = await checkAPI();

  if (!apiAvailable) {
    console.error('\n❌ Erreur: L\'API n\'est pas accessible sur ' + API_BASE_URL);
    console.error('   Assurez-vous que le backend est démarré.');
    console.error('   Exécutez: cd backend/src/ECommerce.API && dotnet run\n');
    process.exit(1);
  }

  await createUsers();
})();
