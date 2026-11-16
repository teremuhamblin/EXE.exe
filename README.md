###### README.md >> markdown
---

[![CI — Build EXE.exe](https://github.com/teremuhamblin/EXE.exe/actions/workflows/ci.yml/badge.svg)](https://github.com/teremuhamblin/EXE.exe/actions/workflows/ci.yml)

---

##### Encrypted_X-Environnement
#### v1.4 Stable
<div align="center">

# 🦠🛡️ EXE.exe
  Bouclier Réseau & Anti-Intrusion Windows

**Inspiré du concept historique *AntiNuke.exe* (1997), modernisé pour contrer les vers, ransomwares, RAT et scans réseau sous Windows 10/11.**

[![GitHub Release](https://img.shields.io/github/v/release/teremuhamblin/EXE.exe?style=for-the-badge&logo=github&color=flat-square)](https://github.com/teremuhamblin/EXE.exe/releases)
[![Dernier Commit](https://img.shields.io/github/last-commit/teremuhamblin/EXE.exe?style=for-the-badge&logo=git&color=flat-square)](https://github.com/teremuhamblin/EXE.exe/commits)
[![GitHub Stars](https://img.shields.io/github/stars/teremuhamblin/EXE.exe?style=for-the-badge&color=flat-square)](https://github.com/teremuhamblin/EXE.exe/stargazers)
[![GitHub Issues](https://img.shields.io/github/issues/teremuhamblin/EXE.exe?style=for-the-badge&color=flat-square)](https://github.com/teremuhamblin/EXE.exe/issues)
[![Licence](https://img.shields.io/github/license/teremuhamblin/EXE.exe?style=for-the-badge&color=flat-square)](LICENSE)

![Plateforme](https://img.shields.io/badge/Plateforme-Windows%2010%20%2F%2011-512BD4?style=flat-square&logo=windows)
![Langage](https://img.shields.io/badge/Langage-C%23%20(.NET%20Framework%204.x)-512BD4?style=flat-square&logo=dotnet)
![Dépendances](https://img.shields.io/badge/D%C3%A9pendances-Z%C3%A9ro%20externe-critical?style=flat-square)
![Statut](https://img.shields.io/badge/Statut-Actif%20%26%20D%C3%A9fensif-critical?style=flat-square)

---

[🚀 Installation](#-compilation-en-1-clic) • [🦠 Menaces bloquées](#-ports--menaces-surveillés) • [🛡️ Fonctionnalités](#️-fonctionnalités-principales) • [🤝 Gouvernance](#-gouvernance--sécurité)

</div>

---

## 📖 Genèse du projet

En 1997, l'utilitaire néo-zélandais **`AntiNuke.exe`** protégeait les PC contre les attaques *WinNuke* sur le port **139 (NetBIOS)** en alertant l'utilisateur au moindre mouvement suspect.

**`EXE.exe`** reprend cette philosophie légère, transparente et réactive, tout en l'adaptant aux menaces modernes :
- Surveillance de la pile réseau Windows au niveau noyau (`iphlpapi.dll`), même sur les ports déjà réservés par le système (**139**, **445**).
- Pièges actifs (*Honeypots* TCP/UDP) sur les ports ciblés par les virus et chevaux de Troie.
- Réponse automatisée via le **Pare-feu Windows** (`netsh advfirewall`).

---

## 🛡️ Fonctionnalités principales

| Module | Rôle | Action automatique |
| :--- | :--- | :--- |
| 🕸️ **Pièges TCP / UDP** | Écoute directe sur les ports sensibles libres | Alerte immédiate + blocage IP |
| 🔍 **Inspection Noyau (`iphlpapi`)** | Lit toutes les connexions TCP actives + PID associé | Détecte les mouvements sur 139 / 445 et ports C2 |
| 🧱 **Auto-Blocage Pare-feu** | Ajoute des règles entrantes/sortantes par IP suspecte | Bloque l'attaquant en temps réel (LAN préservé) |
| 🔒 **Durcissement Anti-Vers** | Verrouille NetBIOS, SMB, RPC, RDP, Telnet en entrée | Protection 1 clic contre WannaCry / EternalBlue |
| 🧪 **Moniteur de Processus** | Repère les `.exe` lancés depuis `Temp`, `Public`, `$Recycle.Bin` | Calcule le SHA-256 + coupe l'accès réseau |
| 🔔 **Barre des tâches Windows** | Tourne discrètement en arrière-plan | Notifications toast en cas d'intrusion |

---

## 🦠 Ports & menaces surveillés

<details>
<summary><b>📋 Cliquez pour afficher la matrice des ports protégés par défaut</b></summary>

| Port(s) | Protocole | Vecteur / Menace associée |
| :--- | :--- | :--- |
| **137, 138, 139** | TCP / UDP | NetBIOS, *WinNuke*, propagation de vers sur réseau |
| **445** | TCP | SMB / *EternalBlue*, *WannaCry*, *NotPetya*, ransomwares |
| **135** | TCP | RPC / DCOM (*Blaster*, déploiement latéral) |
| **3389** | TCP / UDP | RDP (force brute et prise de contrôle par ransomwares) |
| **21, 23, 69, 1433, 5900** | TCP / UDP | FTP, Telnet, TFTP, SQL Server, VNC |
| **4444, 5555, 6666, 6667, 8081** | TCP | Shells distants (*Metasploit*), botnets IRC, RAT |
| **11223, 12345, 27374, 31337** | TCP / UDP | Chevaux de Troie classiques (*Progenic/AntiNuke*, *NetBus*, *Sub7*, *Back Orifice*) |

</details>

---

## 🚀 Compilation en 1 clic

Aucune installation de Visual Studio ou de SDK n'est requise : Windows intègre déjà le compilateur C# (`csc.exe`).

```bat
git clone https://github.com/teremuhamblin/EXE.exe.git
cd EXE.exe
build.bat
```

1. Le script génère **`EXE.exe`** à la racine du dossier.
2. Lancez **`EXE.exe`** (acceptez l'invite **Administrateur**, indispensable pour interagir avec le Pare-feu Windows et les sockets).

---

## ⚠️ Avertissement & limites

> **Important :** `EXE.exe` est un **pare-feu comportemental et un système de détection d'intrusion réseau (IDS/IPS)**.  
> Il **complète** efficacement **Microsoft Defender** (ou votre antivirus principal), mais **ne remplace pas** l'analyse heuristique et par signatures de fichiers d'un antivirus complet (notamment pour les pièces jointes, macros ou fichiers téléchargés).

---

## 🤝 Gouvernance & sécurité

>> 📜 Licence :
- [MIT](LICENSE)

>> 📝 Historique des versions :
- [VERSION.md](.github/VERSION.md)

>> 👥 Mainteneurs :
- [MAINTAINERS.md](.github/MAINTAINERS.md)

>> 🛠️ Contribuer :
- [CONTRIBUTING.md](.github/CONTRIBUTING.md)

>> 🚨 Signaler une faille :
- [SECURITY.md](.github/SECURITY.md)

>> 🤝 Charte de la communauté :
- [CODE_OF_CONDUCT.md](.github/CODE_OF_CONDUCT.md)

<div align="center">
<sub>Fait avec 🛡️ pour une défense réseau simple, transparente et open-source.</sub>
</div>
