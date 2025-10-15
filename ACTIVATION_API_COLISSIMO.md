# Activation de l'API Colissimo

L'API Colissimo est maintenant **ACTIVÉE** dans votre application ! 🎉

## État actuel

✅ **Code SOAP implémenté**
✅ **Parsing XML de la réponse**
✅ **Système de fallback intelligent**
✅ **Logs détaillés**

## Modes de fonctionnement

### Mode SIMULATION (par défaut)

**Quand :** Credentials non configurés dans `appsettings.json`

**Comportement :**
- Génère des numéros de suivi fictifs
- Retourne des URLs d'étiquettes simulées
- Parfait pour le développement
- Aucun appel API réel

**Log dans la console :**
```
⚠️ Colissimo: Credentials manquants, mode simulation activé
```

### Mode PRODUCTION

**Quand :** Credentials configurés dans `appsettings.json`

**Comportement :**
- Appelle la vraie API Colissimo
- Génère de vraies étiquettes PDF
- Retourne de vrais numéros de suivi
- Facturation selon votre contrat Colissimo

**Logs dans la console :**
```
📤 Colissimo: Envoi requête pour commande ABC123
📥 Colissimo: Réponse reçue (status: 200)
✅ Colissimo: Étiquette générée - 6A12345678901FR
```

---

## Configuration pour la production

### Étape 1 : Obtenir vos credentials

1. Créez un compte sur **https://www.laposte.fr/entreprise**
2. Souscrivez à l'offre **"Colissimo Accès Entreprise"**
3. Accédez à votre espace client
4. Dans la section "Web Services", obtenez :
   - **Numéro de contrat** (6 chiffres)
   - **Mot de passe API**

**Délai de validation :** 2-3 jours ouvrés

### Étape 2 : Configuration

Éditez `backend/src/ECommerce.API/appsettings.json` :

```json
"Carriers": {
  "Colissimo": {
    "ApiUrl": "https://ws.colissimo.fr/sls-ws/SlsServiceWS",
    "ContractNumber": "123456",          ← Remplacez par votre numéro
    "Password": "VotreMotDePasseAPI"     ← Remplacez par votre mot de passe
  }
}
```

**Important :** Ne committez JAMAIS ce fichier avec vos vrais credentials ! Utilisez des variables d'environnement en production.

### Étape 3 : Test

1. Redémarrez le backend :
```bash
cd backend/src/ECommerce.API
dotnet run
```

2. Dans l'interface admin :
   - Créez un colis
   - Cliquez sur "Générer l'étiquette"

3. Vérifiez les logs :
```
📤 Colissimo: Envoi requête pour commande [ID]
📥 Colissimo: Réponse reçue (status: 200)
✅ Colissimo: Étiquette générée - [TRACKING]
```

4. **Téléchargez et imprimez l'étiquette PDF** ! 🎉

---

## Requête SOAP envoyée

Voici ce que le système envoie à Colissimo :

```xml
<?xml version="1.0" encoding="UTF-8"?>
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                  xmlns:sls="http://sls.ws.coliposte.fr">
    <soapenv:Body>
        <sls:generateLabel>
            <contractNumber>VOTRE_NUMERO</contractNumber>
            <password>VOTRE_PASSWORD</password>
            <outputFormat>
                <outputPrintingType>PDF_A4_300dpi</outputPrintingType>
            </outputFormat>
            <letter>
                <service>
                    <productCode>DOM</productCode>
                    <depositDate>2025-01-15</depositDate>
                    <orderNumber>ABC123</orderNumber>
                </service>
                <parcel>
                    <weight>1000</weight> <!-- poids en grammes -->
                </parcel>
                <sender>
                    <address>
                        <companyName>Votre Entreprise</companyName>
                        <line2>123 Rue de l'Entrepôt</line2>
                        <city>Paris</city>
                        <zipCode>75001</zipCode>
                        <countryCode>FR</countryCode>
                    </address>
                </sender>
                <addressee>
                    <address>
                        <line2>456 Avenue du Client</line2>
                        <city>Lyon</city>
                        <zipCode>69001</zipCode>
                        <countryCode>FR</countryCode>
                    </address>
                </addressee>
            </letter>
        </sls:generateLabel>
    </soapenv:Body>
</soapenv:Envelope>
```

## Réponse attendue

Colissimo retourne un XML avec :
- **parcelNumber** : Numéro de suivi (ex: 6A12345678901FR)
- **label** : URL ou contenu Base64 du PDF
- **messages** : Éventuels messages d'erreur

Le code parse automatiquement cette réponse.

---

## Gestion des erreurs

### Fallback automatique

Si l'API Colissimo échoue (timeout, erreur serveur, credentials invalides), le système **bascule automatiquement en mode simulation** :

```
❌ Colissimo: Exception - The remote server returned an error: (401) Unauthorized
⚠️ Bascule en mode simulation
```

Cela évite de bloquer votre application si l'API Colissimo est temporairement indisponible.

### Erreurs courantes

| Erreur | Cause | Solution |
|--------|-------|----------|
| `401 Unauthorized` | Credentials invalides | Vérifiez votre numéro de contrat et mot de passe |
| `400 Bad Request` | Données invalides | Vérifiez le format des adresses |
| `Timeout` | API Colissimo lente | Augmentez le timeout du HttpClient |
| `Réponse Colissimo invalide` | Format XML inattendu | Vérifiez la documentation API |

---

## Environnements

### Développement (local)
- Mode simulation activé
- Pas de credentials requis
- Numéros de suivi fictifs

### Staging (pré-production)
- Utilisez les credentials de **test** Colissimo
- Permet de tester sans frais réels
- Demandez un accès sandbox à Colissimo

### Production
- Credentials réels dans variables d'environnement
- Vraies étiquettes générées
- Facturation selon votre contrat

---

## Configuration de l'adresse entrepôt

N'oubliez pas de mettre à jour l'adresse de votre entrepôt dans :

`backend/src/ECommerce.Application/Services/PackageService.cs` ligne 267 :

```csharp
private Address GetWarehouseAddress()
{
    return new Address
    {
        Street = "123 Rue de votre Entrepôt",  ← Modifiez
        City = "Votre Ville",                   ← Modifiez
        ZipCode = "75001",                      ← Modifiez
        Country = "France"
    };
}
```

---

## Prochaines étapes

1. **Testez en mode simulation** (c'est déjà prêt !)
2. **Inscrivez-vous chez Colissimo** si vous voulez de vraies étiquettes
3. **Configurez vos credentials** dans appsettings.json
4. **Testez en production** avec une vraie commande

Pour les autres transporteurs (Chronopost, Mondial Relay, DHL), suivez le même principe :
- Le code est prêt
- Configurez les credentials
- Le système s'adapte automatiquement

---

## Support

- **Documentation Colissimo** : https://www.colissimo.entreprise.laposte.fr/fr/systeme/files/imagescontent/docs/spec_ws_affranchissement.pdf
- **Support La Poste** : 3631
- **Guide complet** : Voir `GUIDE_INTEGRATION_TRANSPORTEURS.md`

Bonne chance ! 🚀📦
