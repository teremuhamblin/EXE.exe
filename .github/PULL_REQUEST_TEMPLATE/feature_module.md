# 🛡️ Pull Request — Nouvelle Fonctionnalité / Port / Règle Défensive

## 🔗 Issue associée
- **Implémente :** Closes #

## ⚙️ Description technique
<!-- Expliquez l'architecture de l'ajout et son utilité défensive. -->

### 🦠 Ports / Vecteurs ajoutés ou modifiés (si applicable)
| Port(s) | Protocole | Vecteur / Menace associée | Impact LAN / Faux positifs |
| :--- | :--- | :--- | :--- |
| | | | |

## 🧱 Contraintes d'architecture EXE.exe
- [ ] 100 % compatible **C# (.NET Framework 4.x)** sans package NuGet ni DLL tierce
- [ ] Compilation validée en 1 clic via `build.bat` (`csc.exe`)
- [ ] Respect de la séparation des rôles : complète **Microsoft Defender** sans bloquer son fonctionnement
- [ ] Les règles `netsh advfirewall` créées préservent explicitement les plages **LAN** locales
- [ ] Si surveillance de processus : calcul **SHA-256** et notification Toast vérifiés

## 📋 Checklist avant fusion
- [ ] Documentation (`README.md` et matrice des ports) mise à jour si nécessaire
- [ ] Testé avec succès en mode Administrateur sous Windows 10 / 11
- [ ] Conforme à `CONTRIBUTING.md` et `CODE_OF_CONDUCT.md`
