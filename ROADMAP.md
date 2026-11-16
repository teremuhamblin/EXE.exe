# 🗺️ Feuille de route — EXE.exe (*Encrypted_X-Environnement*)

## ✅ v1.4 Stable *(Actuelle)*
- [x] Pièges actifs TCP / UDP sur les ports ciblés par vers, RAT et chevaux de Troie
- [x] Inspection noyau via `iphlpapi.dll` (corrélation PID sur ports 135, 139, 445, 3389)
- [x] Auto-blocage temps réel via `netsh advfirewall` (LAN préservé)
- [x] Durcissement Anti-Vers en 1 clic
- [x] Moniteur de processus (`Temp`, `Public`, `$Recycle.Bin`) avec hash SHA-256
- [x] Compilation 100 % autonome sans SDK via `build.bat` (`csc.exe`) & CI GitHub Actions

## 🚧 v1.5 *(Prochaine version)*
- [ ] **Mode Observation (*Dry-Run*)** : alertes Toast sans blocage automatique pour phase de test
- [ ] **Configuration externe (`exe.config.json`)** : ports personnalisés & liste blanche SHA-256/IP
- [ ] **Nettoyage en 1 clic** : bouton dans le System Tray pour supprimer toutes les règles pare-feu créées par `EXE.exe`
- [ ] **Journaux exportables** : historique CSV des tentatives bloquées (`logs/`)
- [ ] **Mode CLI `--self-test`** : tests automatisés non destructifs intégrés au workflow `ci.yml`

## 🔮 v2.0 *(Vision long terme)*
- [ ] Signature numérique du binaire via **SignPath Foundation** *(suppression des alertes SmartScreen)*
- [ ] Tableau de bord statistique léger intégré (connexions bloquées, ports les plus visés)
- [ ] Support de règles communautaires d'indicateurs de compromission (IoC)
