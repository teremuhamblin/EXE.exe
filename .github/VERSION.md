# 📝 Historique des versions (`VERSION.md`)

Ce projet suit le standard [Semantic Versioning (SemVer)](https://semver.org/lang/fr/).

## [1.0.0] — 2026-11-15

### ✨ Ajouts initiaux
- Création du bouclier **`EXE.exe`** inspiré du concept original **`AntiNuke.exe`**.
- Écoute directe (pièges TCP/UDP) sur les ports NetBIOS (`137-139`), SMB (`445`), RPC (`135`), RDP (`3389`), Telnet (`23`), FTP (`21`), SQL (`1433`), VNC (`5900`) et ports connus de malwares/RAT (`4444`, `5555`, `6666`, `6667`, `8081`, `11223`, `12345`, `27374`, `31337`).
- Surveillance temps réel de la table TCP Windows via `iphlpapi.dll` (`GetExtendedTcpTable`) avec résolution du PID et du processus associé.
- Blocage automatique en entrée/sortie des IPs suspectes via le Pare-feu Windows (`netsh advfirewall`) avec protection configurable du LAN.
- Bouton de **durcissement anti-vers / anti-ransomware** en 1 clic et bouton de nettoyage complet des règles créées.
- Surveillance comportementale des processus lancés depuis des répertoires sensibles (`Temp`, `Users\Public`, `$Recycle.Bin`) avec calcul du hash **SHA-256** et coupure réseau.
- Interface sombre temps réel + icône de notification dans la barre des tâches Windows.
