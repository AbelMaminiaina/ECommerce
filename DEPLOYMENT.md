# Guide de Déploiement E-Commerce

Ce guide vous aidera à déployer votre application e-commerce sur **Render** (backend) et **Vercel** (frontend).

## 📋 Prérequis

- [x] Compte MongoDB Atlas configuré
- [x] Compte Render (gratuit)
- [x] Compte Vercel (gratuit)
- [x] Compte Stripe (mode test)
- [x] Repository GitHub

---

## 🚀 Partie 1: Déployer le Backend sur Render

### Étape 1: Préparer le Repository

Le fichier `render.yaml` est déjà configuré à la racine du projet.

### Étape 2: Créer le Service sur Render

1. Allez sur [render.com](https://render.com)
2. Cliquez sur **"New +"** → **"Web Service"**
3. Connectez votre repository GitHub
4. Render détectera automatiquement le fichier `render.yaml`

### Étape 3: Configurer les Variables d'Environnement

Dans le dashboard Render, allez dans **Environment** et ajoutez ces variables:

```bash
# MongoDB (OBLIGATOIRE)
MongoDbSettings__ConnectionString=mongodb+srv://mhm_db_user:VOTRE_MOT_DE_PASSE@cluster0.vrg1xjv.mongodb.net/?retryWrites=true&w=majority
MongoDbSettings__DatabaseName=ECommerceDB

# JWT (OBLIGATOIRE)
JwtSettings__Secret=YourSuperSecretKeyThatIsAtLeast32CharactersLong!
JwtSettings__Issuer=ECommerceAPI
JwtSettings__Audience=ECommerceClient
JwtSettings__AccessTokenExpirationMinutes=480
JwtSettings__RefreshTokenExpirationDays=7

# Stripe (OBLIGATOIRE)
StripeSettings__SecretKey=sk_test_VOTRE_CLE_SECRETE
StripeSettings__PublishableKey=pk_test_VOTRE_CLE_PUBLIQUE

# Frontend URL - À MODIFIER après déploiement Vercel
FRONTEND_URL=https://votre-app.vercel.app
```

### Étape 4: Déployer

1. Cliquez sur **"Create Web Service"**
2. Render va automatiquement:
   - Restaurer les packages NuGet
   - Compiler le projet .NET
   - Démarrer l'application

3. **Notez l'URL de votre API**: `https://votre-backend.onrender.com`

⚠️ **Important**: Le plan gratuit de Render met en veille l'application après 15 minutes d'inactivité. Le premier appel après une période d'inactivité peut prendre 30-60 secondes.

---

## 🎨 Partie 2: Déployer le Frontend sur Vercel

### Étape 1: Créer le Projet Vercel

1. Allez sur [vercel.com](https://vercel.com)
2. Cliquez sur **"Add New..."** → **"Project"**
3. Importez votre repository GitHub
4. Configurez le projet:
   - **Framework Preset**: Vite
   - **Root Directory**: `frontend`
   - **Build Command**: `npm run build`
   - **Output Directory**: `dist`

### Étape 2: Configurer les Variables d'Environnement

Dans **Settings** → **Environment Variables**, ajoutez:

```bash
# URL de votre API Render
VITE_API_URL=https://votre-backend.onrender.com/api

# Clé publique Stripe
VITE_STRIPE_PUBLIC_KEY=pk_test_VOTRE_CLE_PUBLIQUE
```

### Étape 3: Déployer

1. Cliquez sur **"Deploy"**
2. Vercel va automatiquement:
   - Installer les dépendances npm
   - Builder le projet Vite
   - Déployer sur le CDN

3. **Notez l'URL de votre frontend**: `https://votre-app.vercel.app`

---

## 🔄 Partie 3: Finaliser la Configuration

### Mettre à jour le Backend avec l'URL Frontend

1. Retournez sur Render
2. Allez dans **Environment**
3. Modifiez la variable `FRONTEND_URL`:
   ```bash
   FRONTEND_URL=https://votre-app.vercel.app
   ```
4. Sauvegardez (le service redémarrera automatiquement)

### Vérifier le CORS

Le CORS est déjà configuré dans `Program.cs` pour accepter l'URL du `FRONTEND_URL`.

---

## ✅ Vérification du Déploiement

### Backend (Render)

Testez votre API:

```bash
curl https://votre-backend.onrender.com/api/products
```

Vous devriez recevoir une réponse JSON.

### Frontend (Vercel)

1. Ouvrez `https://votre-app.vercel.app`
2. Testez l'inscription/connexion
3. Testez l'ajout de produits au panier
4. Vérifiez que les appels API fonctionnent (F12 → Network)

---

## 🔧 Configuration MongoDB Atlas

Assurez-vous que:

1. **Network Access** autorise toutes les IPs (`0.0.0.0/0`) ou les IPs de Render
2. **Database Access** a un utilisateur configuré avec les bonnes permissions
3. La chaîne de connexion contient le bon mot de passe

---

## 🎯 Déploiement Automatique (CI/CD)

### Configuration Automatique

Les deux plateformes sont configurées pour le déploiement automatique:

- **Render**: Redéploie automatiquement à chaque push sur `main`
- **Vercel**: Redéploie automatiquement à chaque push sur `main`

Pour désactiver le déploiement auto:
- **Render**: Settings → Build & Deploy → Auto-Deploy: OFF
- **Vercel**: Settings → Git → Auto-Deploy: OFF

---

## 📊 Monitoring

### Render

- **Logs**: Allez dans **Logs** pour voir les logs en temps réel
- **Metrics**: Consultez l'utilisation CPU/RAM

### Vercel

- **Analytics**: Activez Vercel Analytics pour voir les métriques
- **Logs**: Consultez les logs de build et runtime

---

## 🐛 Dépannage

### Le backend ne démarre pas

1. Vérifiez les logs Render
2. Vérifiez que toutes les variables d'environnement sont définies
3. Vérifiez la connexion MongoDB Atlas

### Erreur CORS

1. Vérifiez que `FRONTEND_URL` dans Render correspond à l'URL Vercel
2. Redémarrez le service Render après modification

### Le frontend ne se connecte pas au backend

1. Vérifiez `VITE_API_URL` dans Vercel
2. Ouvrez la console du navigateur (F12) pour voir les erreurs
3. Testez l'API directement avec curl

### Le premier appel est lent

C'est normal avec le plan gratuit de Render. Le service se met en veille après 15 minutes d'inactivité.

**Solutions**:
- Passez au plan payant ($7/mois)
- Utilisez un service de "ping" pour garder le service actif

---

## 💰 Coûts

### Plan Gratuit

- **Render**: Gratuit (avec limitations)
- **Vercel**: Gratuit (100 GB bande passante/mois)
- **MongoDB Atlas**: Gratuit (512 MB stockage)

**Total: 0€/mois**

### Plan Recommandé pour Production

- **Render**: Starter $7/mois
- **Vercel**: Pro $20/mois (optionnel)
- **MongoDB Atlas**: Shared M2 $9/mois

**Total: ~16€/mois minimum**

---

## 🔐 Sécurité

### Avant la Production

1. **Changez les secrets JWT**: Générez une nouvelle clé secrète
2. **Utilisez des clés Stripe en production**: Remplacez les clés de test
3. **Configurez HTTPS**: Déjà activé sur Render et Vercel
4. **Limitez les IPs MongoDB**: Au lieu de 0.0.0.0/0, ajoutez uniquement les IPs de Render
5. **Activez l'authentification 2FA**: Sur tous vos comptes (GitHub, Render, Vercel)

---

## 📚 Ressources

- [Documentation Render](https://render.com/docs)
- [Documentation Vercel](https://vercel.com/docs)
- [MongoDB Atlas Docs](https://docs.atlas.mongodb.com/)
- [Stripe Docs](https://stripe.com/docs)

---

## 🎉 Félicitations !

Votre application e-commerce est maintenant déployée et accessible publiquement !

**URLs à partager**:
- Frontend: `https://votre-app.vercel.app`
- API: `https://votre-backend.onrender.com/api`
- Swagger Docs: `https://votre-backend.onrender.com/swagger`
