// ==============================================
// TEST COMPLET DU SYSTÈME DE SUPPORT
// ==============================================

const API_BASE_URL = 'http://localhost:5000/api';

let userToken = null;
let ticketId = null;

// ==========================================
// 1. CONNEXION UTILISATEUR
// ==========================================
async function loginUser() {
  console.log('\n🔐 Connexion utilisateur...\n');

  try {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email: 'user@example.com',
        password: 'User123!'
      })
    });

    if (!response.ok) {
      const error = await response.text();
      console.log('❌ Échec de connexion:', error);
      return false;
    }

    const data = await response.json();
    userToken = data.accessToken;

    console.log('✅ Connexion réussie!');
    console.log(`   👤 ${data.user.firstName} ${data.user.lastName}`);
    console.log(`   📧 ${data.user.email}\n`);
    return true;
  } catch (error) {
    console.error('❌ Erreur:', error.message);
    return false;
  }
}

// ==========================================
// 2. CRÉER UN TICKET DE RÉCLAMATION
// ==========================================
async function createDeliveryComplaint() {
  console.log('📋 Création d\'un ticket de réclamation pour un colis...\n');

  const ticketData = {
    orderId: null,  // Vous pouvez mettre un vrai ID de commande
    subject: 'Colis non reçu',
    description: 'Bonjour, je n\'ai toujours pas reçu ma commande COL123456789FR. Elle devait arriver il y a 5 jours. Pouvez-vous vérifier le statut de ma livraison ?',
    category: 2  // 2 = Delivery (livraison)
  };

  try {
    const response = await fetch(`${API_BASE_URL}/support/tickets`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${userToken}`
      },
      body: JSON.stringify(ticketData)
    });

    if (!response.ok) {
      const error = await response.text();
      console.log('❌ Erreur lors de la création du ticket:', error);
      return false;
    }

    const ticket = await response.json();
    ticketId = ticket.id;

    console.log('✅ Ticket créé avec succès!');
    console.log(`   🆔 ID: ${ticket.id}`);
    console.log(`   📌 Sujet: ${ticket.subject}`);
    console.log(`   📂 Catégorie: ${getCategoryName(ticket.category)}`);
    console.log(`   🔄 Statut: ${getStatusName(ticket.status)}`);
    console.log(`   ⚡ Priorité: ${getPriorityName(ticket.priority)}`);
    console.log(`   💬 Messages: ${ticket.messages.length}`);
    console.log(`   📅 Créé: ${new Date(ticket.createdAt).toLocaleString('fr-FR')}\n`);
    return true;
  } catch (error) {
    console.error('❌ Erreur:', error.message);
    return false;
  }
}

// ==========================================
// 3. OBTENIR TOUS MES TICKETS
// ==========================================
async function getMyTickets() {
  console.log('📋 Récupération de tous mes tickets...\n');

  try {
    const response = await fetch(`${API_BASE_URL}/support/tickets`, {
      headers: {
        'Authorization': `Bearer ${userToken}`
      }
    });

    if (!response.ok) {
      const error = await response.text();
      console.log('❌ Erreur:', error);
      return;
    }

    const tickets = await response.json();

    console.log(`✅ ${tickets.length} ticket(s) trouvé(s):\n`);

    tickets.forEach((ticket, index) => {
      const statusIcon = getStatusIcon(ticket.status);
      console.log(`   ${index + 1}. ${statusIcon} ${ticket.subject}`);
      console.log(`      🆔 ${ticket.id}`);
      console.log(`      📂 ${getCategoryName(ticket.category)}`);
      console.log(`      🔄 ${getStatusName(ticket.status)}`);
      console.log(`      💬 ${ticket.messages.length} message(s)`);
      console.log(`      📅 ${new Date(ticket.createdAt).toLocaleString('fr-FR')}`);
      console.log('');
    });
  } catch (error) {
    console.error('❌ Erreur:', error.message);
  }
}

// ==========================================
// 4. OBTENIR UN TICKET SPÉCIFIQUE
// ==========================================
async function getTicketDetails() {
  if (!ticketId) {
    console.log('⚠️  Pas de ticket créé pour obtenir les détails\n');
    return;
  }

  console.log(`📄 Détails du ticket ${ticketId}...\n`);

  try {
    const response = await fetch(`${API_BASE_URL}/support/tickets/${ticketId}`, {
      headers: {
        'Authorization': `Bearer ${userToken}`
      }
    });

    if (!response.ok) {
      const error = await response.text();
      console.log('❌ Erreur:', error);
      return;
    }

    const ticket = await response.json();

    console.log('✅ Détails du ticket:');
    console.log(`   🆔 ID: ${ticket.id}`);
    console.log(`   📌 Sujet: ${ticket.subject}`);
    console.log(`   📝 Description: ${ticket.description}`);
    console.log(`   📂 Catégorie: ${getCategoryName(ticket.category)}`);
    console.log(`   🔄 Statut: ${getStatusName(ticket.status)}`);
    console.log(`   ⚡ Priorité: ${getPriorityName(ticket.priority)}`);
    console.log(`   📅 Créé: ${new Date(ticket.createdAt).toLocaleString('fr-FR')}`);
    console.log(`\n   💬 Messages (${ticket.messages.length}):`);

    ticket.messages.forEach((msg, index) => {
      const fromIcon = msg.isFromAdmin ? '👑 ADMIN' : '👤 CLIENT';
      console.log(`\n      ${index + 1}. ${fromIcon} - ${msg.senderName}`);
      console.log(`         📅 ${new Date(msg.createdAt).toLocaleString('fr-FR')}`);
      console.log(`         💬 ${msg.message}`);
    });
    console.log('');
  } catch (error) {
    console.error('❌ Erreur:', error.message);
  }
}

// ==========================================
// 5. AJOUTER UN MESSAGE AU TICKET
// ==========================================
async function addMessageToTicket() {
  if (!ticketId) {
    console.log('⚠️  Pas de ticket créé pour ajouter un message\n');
    return;
  }

  console.log(`💬 Ajout d'un message au ticket ${ticketId}...\n`);

  const messageData = {
    message: 'Bonjour, avez-vous des nouvelles concernant ma réclamation ? C\'est urgent, j\'attends ce colis depuis longtemps.',
    attachments: []
  };

  try {
    const response = await fetch(`${API_BASE_URL}/support/tickets/${ticketId}/messages`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${userToken}`
      },
      body: JSON.stringify(messageData)
    });

    if (!response.ok) {
      const error = await response.text();
      console.log('❌ Erreur:', error);
      return;
    }

    const ticket = await response.json();

    console.log('✅ Message ajouté avec succès!');
    console.log(`   💬 Total de messages: ${ticket.messages.length}`);
    console.log(`   🔄 Nouveau statut: ${getStatusName(ticket.status)}\n`);
  } catch (error) {
    console.error('❌ Erreur:', error.message);
  }
}

// ==========================================
// 6. CRÉER DIFFÉRENTS TYPES DE RÉCLAMATIONS
// ==========================================
async function createVariousTickets() {
  console.log('📋 Création de différents types de tickets...\n');

  const tickets = [
    {
      subject: 'Produit défectueux',
      description: 'Les écouteurs que j\'ai reçus ne fonctionnent pas. L\'oreillette droite ne charge plus.',
      category: 1  // Product
    },
    {
      subject: 'Remboursement non reçu',
      description: 'J\'ai retourné ma commande il y a 2 semaines mais je n\'ai toujours pas été remboursé.',
      category: 3  // Return
    },
    {
      subject: 'Question sur ma commande',
      description: 'Ma commande est marquée comme livrée mais je ne l\'ai pas reçue. Pouvez-vous vérifier ?',
      category: 0  // Order
    }
  ];

  for (const ticketData of tickets) {
    try {
      const response = await fetch(`${API_BASE_URL}/support/tickets`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${userToken}`
        },
        body: JSON.stringify({
          orderId: null,
          ...ticketData
        })
      });

      if (response.ok) {
        const ticket = await response.json();
        console.log(`✅ ${ticket.subject}`);
        console.log(`   📂 ${getCategoryName(ticket.category)}`);
        console.log(`   🆔 ${ticket.id}\n`);
      } else {
        const error = await response.text();
        console.log(`❌ Erreur pour "${ticketData.subject}":`, error);
      }
    } catch (error) {
      console.error('❌ Erreur:', error.message);
    }
  }
}

// ==========================================
// FONCTIONS UTILITAIRES
// ==========================================
function getCategoryName(category) {
  const categories = {
    0: 'Commande',
    1: 'Produit',
    2: 'Livraison',
    3: 'Retour/Remboursement',
    4: 'Technique',
    5: 'Paiement',
    6: 'Autre'
  };
  return categories[category] || 'Inconnu';
}

function getStatusName(status) {
  const statuses = {
    0: 'Ouvert',
    1: 'En cours',
    2: 'En attente client',
    3: 'Résolu',
    4: 'Fermé'
  };
  return statuses[status] || 'Inconnu';
}

function getPriorityName(priority) {
  const priorities = {
    0: 'Basse',
    1: 'Moyenne',
    2: 'Haute',
    3: 'Urgente'
  };
  return priorities[priority] || 'Inconnu';
}

function getStatusIcon(status) {
  const icons = {
    0: '🔵',  // Open
    1: '🟡',  // InProgress
    2: '🟠',  // WaitingCustomer
    3: '🟢',  // Resolved
    4: '⚫'   // Closed
  };
  return icons[status] || '⚪';
}

// ==========================================
// MENU PRINCIPAL
// ==========================================
async function main() {
  console.log('\n' + '='.repeat(60));
  console.log('    TEST DU SYSTÈME DE SUPPORT - RÉCLAMATIONS');
  console.log('='.repeat(60));

  // 1. Se connecter
  const connected = await loginUser();
  if (!connected) {
    console.log('⚠️  Impossible de continuer sans connexion\n');
    console.log('💡 Assurez-vous que:');
    console.log('   - Le backend est démarré (dotnet run)');
    console.log('   - L\'utilisateur user@example.com existe');
    console.log('   - MongoDB est en cours d\'exécution\n');
    return;
  }

  // 2. Créer un ticket de réclamation pour livraison
  await createDeliveryComplaint();

  // 3. Obtenir tous les tickets
  await getMyTickets();

  // 4. Obtenir les détails du ticket créé
  await getTicketDetails();

  // 5. Ajouter un message au ticket
  await addMessageToTicket();

  // 6. Obtenir à nouveau les détails pour voir le nouveau message
  console.log('📄 Rafraîchissement des détails du ticket...\n');
  await getTicketDetails();

  // 7. Créer d'autres types de tickets
  await createVariousTickets();

  // 8. Lister tous les tickets à la fin
  console.log('📋 Liste finale de tous mes tickets:\n');
  await getMyTickets();

  console.log('='.repeat(60));
  console.log('✅ Test terminé!\n');
}

// Exécuter le test
main();
