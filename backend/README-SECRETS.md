# Configuration des Secrets - Backend .NET

## 🔐 Pourquoi User Secrets ?

Les credentials sensibles (mots de passe, clés API) ne doivent **JAMAIS** être commitées dans Git. .NET User Secrets permet de stocker ces informations de manière sécurisée en local.

## 📋 Configuration Initiale

### 1. Initialiser User Secrets

```bash
cd backend/src/ecommerce.api
dotnet user-secrets init
```

### 2. Ajouter vos Secrets

#### MongoDB (OBLIGATOIRE)

```bash
dotnet user-secrets set "MongoDbSettings:ConnectionString" "mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority"
```

**Remplacez** :
- `username` : votre nom d'utilisateur MongoDB Atlas
- `password` : votre mot de passe MongoDB Atlas
- `cluster` : votre nom de cluster MongoDB Atlas

**⚠️ Attention** : Si votre mot de passe contient des caractères spéciaux (`@`, `:`, `/`, etc.), encodez-les en URL :
- `@` → `%40`
- `:` → `%3A`
- `/` → `%2F`

Exemple : `MyP@ss` devient `MyP%40ss`

#### Stripe (OBLIGATOIRE)

```bash
dotnet user-secrets set "StripeSettings:SecretKey" "sk_test_your_stripe_secret_key"
dotnet user-secrets set "StripeSettings:PublishableKey" "pk_test_your_stripe_publishable_key"
```

Récupérez vos clés Stripe sur : https://dashboard.stripe.com/test/apikeys

### 3. Vérifier la Configuration

Listez tous vos secrets :

```bash
dotnet user-secrets list
```

Vous devriez voir :
```
MongoDbSettings:ConnectionString = mongodb+srv://...
StripeSettings:SecretKey = sk_test_...
StripeSettings:PublishableKey = pk_test_...
```

## 🚀 Lancement du Projet

Une fois les secrets configurés, lancez simplement :

```bash
dotnet run
```

.NET chargera automatiquement :
1. Les valeurs de `appsettings.json`
2. Les valeurs de `appsettings.Development.json`
3. Les valeurs de User Secrets (qui **écrasent** les précédentes)

## 📁 Où sont stockés les Secrets ?

Les secrets sont stockés **en dehors du projet** dans :

**Windows** : `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`

**Linux/Mac** : `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

Ils ne sont **jamais** commitées dans Git.

## 🔄 Configuration pour un Nouveau Développeur

Si vous clonez le projet pour la première fois :

1. Copiez le fichier exemple :
   ```bash
   cp appsettings.Development.json.example appsettings.Development.json
   ```

2. Configurez vos User Secrets (voir section "Configuration Initiale")

3. Lancez l'application :
   ```bash
   dotnet run
   ```

## 🛠️ Commandes Utiles

### Lister tous les secrets
```bash
dotnet user-secrets list
```

### Supprimer un secret
```bash
dotnet user-secrets remove "MongoDbSettings:ConnectionString"
```

### Supprimer tous les secrets
```bash
dotnet user-secrets clear
```

### Voir l'ID User Secrets
```bash
dotnet user-secrets list --id
```

## 🔒 Sécurité

✅ **À FAIRE** :
- Utiliser User Secrets pour le développement local
- Utiliser des variables d'environnement en production (Render, Azure, etc.)
- Ne jamais commiter `appsettings.Development.json` s'il contient des credentials

❌ **À NE PAS FAIRE** :
- Commiter des mots de passe dans Git
- Partager votre fichier `secrets.json`
- Utiliser User Secrets en production (utilisez des variables d'environnement)

## 📚 Documentation Officielle

- [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [MongoDB Atlas](https://www.mongodb.com/docs/atlas/)
- [Stripe API Keys](https://stripe.com/docs/keys)

## ❓ Dépannage

### Erreur : "Unable to authenticate"

Vérifiez que :
1. Votre mot de passe MongoDB est correctement encodé en URL
2. Vous avez bien initialisé User Secrets (`dotnet user-secrets init`)
3. Les secrets sont bien configurés (`dotnet user-secrets list`)

### L'application ne trouve pas les secrets

Assurez-vous que :
1. Vous êtes dans le bon répertoire (`backend/src/ecommerce.api`)
2. Le projet a bien un `UserSecretsId` dans le `.csproj`
3. Vous êtes en mode `Development` (par défaut avec `dotnet run`)

## 🎯 Résumé Rapide

```bash
# 1. Initialiser (une seule fois)
cd backend/src/ecommerce.api
dotnet user-secrets init

# 2. Configurer MongoDB
dotnet user-secrets set "MongoDbSettings:ConnectionString" "votre_connection_string"

# 3. Configurer Stripe
dotnet user-secrets set "StripeSettings:SecretKey" "votre_cle_secrete"
dotnet user-secrets set "StripeSettings:PublishableKey" "votre_cle_publique"

# 4. Vérifier
dotnet user-secrets list

# 5. Lancer
dotnet run
```

Voilà ! Vos credentials sont sécurisés. 🎉
