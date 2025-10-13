const { MongoClient } = require('mongodb');

const client = new MongoClient('mongodb://localhost:27017');

client.connect().then(async () => {
  const db = client.db('ECommerceDB');
  const admins = await db.collection('users').find({email: 'admin@example.com'}).toArray();

  console.log('\n📊 Nombre d\'utilisateurs admin trouvés:', admins.length, '\n');

  admins.forEach((admin, i) => {
    console.log(`Admin #${i+1}:`);
    console.log('  🆔 ID:', admin._id);
    console.log('  📧 Email:', admin.email);
    console.log('  🔓 isActive:', admin.isActive);
    console.log('  🔐 Hash:', admin.passwordHash.substring(0, 20) + '...');
    console.log('  📅 Créé le:', admin.createdAt);
    console.log('');
  });

  await client.close();
});
