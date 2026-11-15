###### docs/INFORMATION.md >> markdown
# EXE.exe
- 》》 *v1.4 Stable*

###### L'intégralité du projet prêt à compiler sous Windows 10 / Windows 11 pour obtenir votre fichier EXE.exe en quelques secondes, sans aucun logiciel externe à installer (*en utilisant le compilateur .NET déjà intégré à Windows*).
##### Ce que fait votre projet EXE.exe (concept AntiNuke modernisé). Sur Windows moderne, le port 139 (et 445) est déjà occupé par le système : un simple programme d’écoute classique échoue donc souvent à le surveiller. EXE.exe contourne ce problème et va beaucoup plus loin que l’ancien AntiNuke.exe :
- Pièges réseau multi-ports (Honeypot TCP & UDP) :
   
   - Écoute les tentatives de connexion sur les ports cibles de vers, scanners et vieux/nouveaux chevaux de Troie (21, 23, 135, 137, 138, 139, 445, 1433, 3389, 4444, 5555, 5900, 6667, 11223, 12345, 27374, 31337, etc.).
   
   - Inspection en direct de la pile réseau Windows (iphlpapi.dll) :
Surveille en temps réel toutes les connexions actives (même sur les ports 139 et 445 déjà gérés par Windows), identifie le processus (.exe) qui communique et l'IP distante.

- Blocage automatique dans le Pare-feu Windows :
   - Dès qu’une IP distante sonde un port sensible ou qu’un malware tente une connexion sur un port de contrôle (RAT / Backdoor), EXE.exe crée automatiquement une règle de blocage dans le Pare-feu Windows (avec option pour préserver le réseau local/LAN).
   
   - Bouton « Durcissement Anti-Vers / Anti-Ransomware » :
Bloque d'un clic les vecteurs d’attaques réseau les plus exploités par les virus (NetBIOS 137-139, SMB/EternalBlue 445, RPC 135, RDP 3389, Telnet 23).

- Détection comportementale de processus suspects :
   - Repère les exécutables lancés depuis des zones à risque (%TEMP%, Public, Recycle.Bin) ou les scripts suspects (powershell, mshta, wscript, cscript, rundll32) ouvrant des connexions externes, affiche leur hash SHA-256 et permet de bloquer leur accès réseau ou de couper le processus.

- Alertes temps réel & icône dans la barre des tâches :
   - Notifications Windows instantanées à chaque menace détectée/bloquée, comme le faisait AntiNuke.
