# 🚨 Politique de sécurité (`SECURITY.md`)

## 📌 Versions prises en charge

| Version | Statut de support |
| :--- | :--- |
| `1.0.x` | ✅ Supportée activement |
| `< 1.0` | ❌ Non supportée |

## 🛡️ Périmètre & limites

`EXE.exe` s'exécute avec des **privilèges Administrateur** afin de lire la table réseau Windows et de configurer le Pare-feu Windows. Une attention particulière est donc portée à :
- L'absence d'injection de commandes via `netsh` ou `powershell`.
- La validation stricte des adresses IP et des chemins d'exécutables avant toute action.
- La non-exécution automatique de binaires tiers.

## 📩 Signaler une vulnérabilité

**Merci de ne pas publier de faille de sécurité dans une Issue publique.**

Pour signaler une vulnérabilité de manière responsable :
1. Utilisez l'onglet **Security → Advisories → Report a vulnerability** du dépôt GitHub (recommandé), **ou**
2. Contactez directement le mainteneur principal indiqué dans `MAINTAINERS.md`.

### Ce que nous attendons dans votre signalement
- Description détaillée de la faille et impact potentiel (ex. contournement du blocage, exécution de commande, plantage/DoS local).
- Étapes reproductibles pour vérifier le problème.
- Toute piste de correctif si vous en avez une.

Nous nous engageons à accuser réception sous **72 heures** et à publier un correctif dans les meilleurs délais.
