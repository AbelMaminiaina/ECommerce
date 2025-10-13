// ==============================================
// GÉNÉRATEUR DE HASH BCRYPT POUR MONGODB
// ==============================================
//
// Ce script Node.js génère un hash BCrypt pour un mot de passe
// Usage: node generate-password-hash.js "VotreMotDePasse"
//
// Prérequis:
//   npm install bcryptjs
// ==============================================

const bcrypt = require('bcryptjs');

// Récupérer le mot de passe depuis les arguments
const password = process.argv[2];

if (!password) {
  console.log('\n❌ Erreur: Veuillez fournir un mot de passe\n');
  console.log('Usage: node generate-password-hash.js "VotreMotDePasse"\n');
  console.log('Exemple: node generate-password-hash.js "User123!"\n');
  process.exit(1);
}

// Générer le hash avec 11 rounds (comme dans l'application C#)
const saltRounds = 11;
const hash = bcrypt.hashSync(password, saltRounds);

console.log('\n✅ Hash BCrypt généré avec succès!\n');
console.log('🔑 Mot de passe:', password);
console.log('🔐 Hash:', hash);
console.log('\n📋 Utilisez ce hash dans votre script MongoDB:');
console.log(`   passwordHash: "${hash}",`);
console.log('');
