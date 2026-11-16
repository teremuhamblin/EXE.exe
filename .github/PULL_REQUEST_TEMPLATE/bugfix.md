# 🐞 Pull Request — Correctif de Bug / Stabilité

## 🔗 Issue associée
- **Corrige :** Closes #

## 🔍 Diagnostic & Correctif
- **Module concerné :** *(Pièges TCP/UDP, `iphlpapi.dll`, `netsh advfirewall`, Durcissement, Moniteur de Processus, Tray/Toast, `build.bat`)*
- **Cause racine :**
- **Solution apportée :**

## 🧪 Tests effectués
- [ ] Compilation propre via `build.bat` (`csc.exe` uniquement, zéro SDK externe)
- [ ] Testé en environnement réel sous **Windows 10** et/ou **Windows 11** (mode Administrateur)
- [ ] Vérification d'absence de blocage involontaire sur le réseau local (**LAN préservé**)
- [ ] Vérification de l'absence de fuite mémoire ou de conflit sur les ports réservés (`135`, `137-139`, `445`, `3389`)

## 📋 Checklist avant fusion
- [ ] Aucune dépendance externe ajoutée
- [ ] Aucun secret, IP privée sensible ou chemin personnel laissé dans le code
- [ ] Code relu et conforme à `CONTRIBUTING.md`
