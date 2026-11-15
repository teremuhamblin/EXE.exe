# 🛠️ Guide de contribution — EXE.exe

Merci de votre intérêt pour **EXE.exe** ! Les contributions visant à renforcer la détection défensive, l'ergonomie ou la stabilité sont les bienvenues.

## 🧭 Règles strictes du projet

1. **Purement défensif :** Aucune fonctionnalité offensive (ex. riposte réseau, paquet malformé OOB, scan externe non sollicité) ne sera acceptée.
2. **Zéro dépendance externe :** Le code dans `src/Program.cs` doit rester compilable nativement avec `csc.exe` (C# 5 / .NET Framework 4.x intégré à Windows 10/11).
3. **Sécurité de l'utilisateur :** Toute règle de blocage automatique doit préserver le loopback (`127.0.0.1`) et respecter l'option d'exclusion du réseau local (LAN).

## 🔄 Processus de contribution

1. **Forkez** le dépôt sur GitHub.
2. Créez une branche dédiée :
   ```bash
   git checkout -b feat/nouvelle-detection
   ```
3. Compilez et testez localement avec `build.bat` en tant qu'Administrateur.
4. Vérifiez que le bouton **"Nettoyer les règles créées par EXE.exe"** supprime bien toute nouvelle règle ajoutée.
5. Ouvrez une **Pull Request** claire décrivant le comportement ajouté et les tests effectués.

## 🐛 Signaler un bug ou un faux positif

Ouvrez une *Issue* en précisant :
- Votre version de Windows (ex. Windows 11 24H2).
- Les journaux affichés dans la fenêtre de `EXE.exe` (sans exposer d'informations privées sensibles).
- Le nom du programme légitime éventuellement impacté en cas de faux positif.
