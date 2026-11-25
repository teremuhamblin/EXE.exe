# EXE.exe — Informations techniques

> Documentation du dépôt pour la version 1.5. Vérifiez la page [Releases](https://github.com/teremuhamblin/EXE.exe/releases) pour distinguer cette version de documentation d'une release effectivement publiée.

## Objectif

EXE.exe est une application Windows de surveillance réseau et d'interaction avec le Pare-feu Windows, inspirée du concept historique AntiNuke. Elle combine une interface Windows Forms, des écouteurs TCP/UDP sur les ports disponibles et la lecture de la table des connexions TCP via `GetExtendedTcpTable` dans `iphlpapi.dll`.

## Comportements implémentés

- Tente d'écouter certains ports TCP et UDP configurés dans `src/Program.cs`. Un port occupé ou inaccessible n'est pas pris par l'application.
- Parcourt périodiquement les connexions TCP IPv4 et associe les entrées à leur PID lorsque les données sont disponibles.
- Peut ajouter des règles de blocage via `netsh advfirewall` selon les options et événements observés.
- Propose une action de durcissement du pare-feu et une action de suppression de règles dont le nom correspond au préfixe de l'application.
- Surveille les processus et calcule le SHA-256 de certains exécutables localisés dans des dossiers ciblés.
- Affiche les alertes dans l'interface et la zone de notification Windows.

## Limites importantes

- La surveillance d'une connexion ou d'un port n'établit pas à elle seule qu'une activité est malveillante. Des faux positifs sont possibles.
- La liste de ports est une heuristique et n'est pas une signature de malware.
- Les règles de filtrage peuvent modifier la connectivité de l'ordinateur. Vérifiez chaque changement et conservez un moyen de récupération.
- Le comportement dépend de Windows, des permissions, des ports déjà utilisés, du profil réseau et des autres règles installées.
- Le manifeste `src/app.manifest` demande actuellement l'élévation administrateur au lancement.
- Cette application ne remplace pas un antivirus, un EDR ou les protections intégrées à Windows.

## Compiler

Depuis un terminal Windows, à la racine du dépôt :

```bat
build.bat
```

* *Le script cherche **csc.exe** dans les emplacements **.NET Framework** usuels, compile d'abord vers un fichier temporaire et ne remplace *EXE.exe* qu'après une compilation réussie*.

* *Consultez également le README, la politique de sécurité et le guide de contribution*.
