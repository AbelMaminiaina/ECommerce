# Guide d'intégration des transporteurs

Ce guide explique comment intégrer les vraies API des transporteurs français pour générer des étiquettes d'expédition réelles.

## État actuel

Le système utilise actuellement des **données simulées** pour le développement. Pour la production, vous devez intégrer les vraies API.

## Transporteurs supportés

1. **La Poste / Colissimo** - Recommandé pour débuter
2. **Chronopost** - Livraison express
3. **Mondial Relay** - Points relais économiques
4. **DHL Express** - International

---

## 1. La Poste / Colissimo (Recommandé)

### Inscription

1. Créez un compte professionnel : https://www.laposte.fr/entreprise
2. Souscrivez à "Colissimo Accès Entreprise"
3. Accédez à l'espace développeur : https://developer.laposte.fr/
4. Demandez un accès aux Web Services

### Documentation API

- **Affranchissement** : https://developer.laposte.fr/products/colissimo/latest
- **Guide PDF** : https://www.colissimo.entreprise.laposte.fr/fr/systeme/files/imagescontent/docs/spec_ws_affranchissement.pdf
- **API Suivi** : https://developer.laposte.fr/products/suivi/latest

### Credentials

Après validation de votre compte, vous recevrez :
- `ContractNumber` : Numéro de contrat (6 chiffres)
- `Password` : Mot de passe API
- `SiteId` : Identifiant du site (optionnel)

### Configuration

Dans `appsettings.json` :

```json
"Carriers": {
  "Colissimo": {
    "ApiUrl": "https://ws.colissimo.fr/sls-ws/SlsServiceWS",
    "ContractNumber": "123456",
    "Password": "VotreMotDePasse"
  }
}
```

### Activation dans le code

1. Ouvrez `ColissimoCarrierService.cs`
2. **Décommentez** les lignes 38-89 (appel SOAP)
3. Ajoutez `using System.Text;` en haut du fichier
4. Commentez la simulation (lignes 91-102)

### Test

```bash
dotnet run
```

Créez un colis et générez une étiquette. Vous devriez recevoir une vraie étiquette PDF !

---

## 2. Chronopost (Express)

### Inscription

1. Compte : https://www.chronopost.fr/fr/espace-client
2. Contactez le service commercial : **0825 801 801**
3. Demandez l'accès aux Web Services

### Documentation

- Guide PDF : https://www.chronopost.fr/sites/default/files/documents/WS_Shipping_Service.pdf
- API Tracking : https://www.chronopost.fr/fr/page/integration-tracking-api

### Credentials

- `AccountNumber` : Numéro de compte Chronopost (8 chiffres)
- `Password` : Mot de passe Web Services
- `SubAccount` : Sous-compte (généralement "001")

### Configuration

```json
"Chronopost": {
  "ApiUrl": "https://ws.chronopost.fr/shipping-cxf/ShippingServiceWS",
  "AccountNumber": "12345678",
  "Password": "VotreMotDePasse",
  "SubAccount": "001"
}
```

### Particularités

- Livraison express (24-48h)
- Plus cher que Colissimo
- API SOAP similaire à Colissimo
- Excellente documentation

---

## 3. Mondial Relay (Points Relais)

### Inscription

1. Compte e-commerçant : https://www.mondialrelay.fr/creation-compte-e-commercant/
2. Validation sous 24-48h
3. Accès API dans votre espace client

### Documentation

- Guide complet : https://www.mondialrelay.fr/media/108958/solutions_marchand_suivi_webservice_v16.pdf
- API REST moderne (plus facile que SOAP)

### Credentials

- `BrandCode` : Code enseigne (ex: "BDTEST" pour test, "BD12345" pour production)
- `ApiKey` : Clé API (32 caractères hexadécimaux)

### Configuration

```json
"MondialRelay": {
  "ApiUrl": "https://api.mondialrelay.com/Web_Services.asmx",
  "BrandCode": "BD12345",
  "ApiKey": "votre_cle_api_32_caracteres"
}
```

### Particularités

- **Nécessite un point relais** pour la livraison
- Tarifs très compétitifs
- API de recherche de points relais disponible
- Idéal pour les petits colis

---

## 4. DHL Express (International)

### Inscription

1. Compte : https://mydhl.express.dhl/fr/fr/home.html
2. Activez l'API dans votre espace client
3. Générez vos credentials

### Documentation

- API Reference : https://developer.dhl.com/api-reference/shipment
- Guide : https://developer.dhl.com/api-reference/dhl-express-mydhl-api

### Credentials

- `ApiKey` : Clé API DHL
- `AccountNumber` : Numéro de compte

### Configuration

```json
"DHL": {
  "ApiUrl": "https://api-eu.dhl.com/parcel/de/shipping/v2",
  "ApiKey": "votre_cle_api",
  "AccountNumber": "123456789"
}
```

### Particularités

- API REST moderne (plus simple)
- Excellent pour l'international
- Plus cher
- Tracking en temps réel

---

## Ordre d'implémentation recommandé

### Phase 1 : Test (1-2 semaines)
1. **Colissimo** en mode sandbox
2. Tester la génération d'étiquettes
3. Vérifier l'envoi des notifications

### Phase 2 : Production Nationale (2-4 semaines)
1. Activer **Colissimo** en production
2. Ajouter **Mondial Relay** (points relais)
3. Optionnel : **Chronopost** (express)

### Phase 3 : International (si nécessaire)
1. Ajouter **DHL** ou **FedEx**
2. Gérer les douanes
3. Calculer les frais internationaux

---

## Configuration Email (Notifications)

Pour envoyer les notifications avec numéro de suivi :

### Gmail (simple pour débuter)

```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUsername": "votre.email@gmail.com",
  "SmtpPassword": "mot_de_passe_application",
  "FromEmail": "noreply@votresite.fr",
  "FromName": "Votre E-Commerce"
}
```

**Important** : Utilisez un "Mot de passe d'application" Gmail, pas votre mot de passe normal.

### SendGrid (recommandé en production)

1. Créez un compte : https://sendgrid.com/
2. Obtenez une API Key
3. Configurez :

```json
"Email": {
  "SmtpHost": "smtp.sendgrid.net",
  "SmtpPort": 587,
  "SmtpUsername": "apikey",
  "SmtpPassword": "VOTRE_SENDGRID_API_KEY",
  "FromEmail": "noreply@votresite.fr",
  "FromName": "Votre E-Commerce"
}
```

---

## Coûts estimés

### Abonnements mensuels
- **La Poste/Colissimo** : Gratuit + coût par envoi
- **Chronopost** : Selon contrat commercial
- **Mondial Relay** : Gratuit + coût par envoi
- **DHL** : Selon contrat commercial

### Coûts par colis (estimation France)
- **Colissimo** : 5-8€
- **Chronopost** : 10-15€
- **Mondial Relay** : 3-6€
- **DHL** : 15-25€

Les prix varient selon le poids, la destination et votre volume mensuel.

---

## Support et aide

### Contacts support
- **La Poste** : 3631 (entreprises)
- **Chronopost** : 0825 801 801
- **Mondial Relay** : 09 69 32 23 32
- **DHL** : 0825 10 00 80

### Groupes et forums
- Forum La Poste : https://www.laposte.fr/entreprise/forum
- Documentation Mondial Relay : Très complète en français
- Stack Overflow : Tags "colissimo", "chronopost"

---

## Checklist avant mise en production

- [ ] Credentials obtenus pour au moins 1 transporteur
- [ ] Configuration testée en sandbox
- [ ] Vraie étiquette PDF générée avec succès
- [ ] Notification email fonctionnelle
- [ ] Test de bout en bout : commande → colis → étiquette → expédition
- [ ] Adresse entrepôt configurée dans le code
- [ ] Conditions générales de vente mises à jour
- [ ] Assurance colis activée (recommandé)

---

## Dépannage

### Erreur : "Unauthorized" ou "403"
→ Vérifiez vos credentials dans appsettings.json

### Erreur : "Invalid address"
→ Vérifiez le format des adresses (certaines API sont strictes)

### PDF vide ou erreur
→ Vérifiez les logs du transporteur, souvent un champ manquant

### Pas de notification email
→ Vérifiez la configuration SMTP et les logs

---

## Prochaines étapes

1. Inscrivez-vous chez **Colissimo** (le plus simple)
2. Obtenez vos credentials de test
3. Remplissez `appsettings.json`
4. Décommentez le code dans `ColissimoCarrierService.cs`
5. Testez !

Bonne chance ! 🚀📦
