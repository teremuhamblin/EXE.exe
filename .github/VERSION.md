# 📝 Historique des versions

Le projet utilise [Semantic Versioning (SemVer)](https://semver.org/lang/fr/). Consultez la page [Releases](https://github.com/teremuhamblin/EXE.exe/releases) pour connaître les versions réellement publiées.

## 1.5 — Documentation et fiabilisation

Cette entrée décrit l'état de la documentation et des scripts du dépôt ; elle ne prétend pas qu'une release binaire 1.5 a été publiée.

- README réorganisé avec les prérequis, les limites et les avertissements de sécurité.
- Script de compilation rendu indépendant du répertoire courant et compilation temporaire avant remplacement du binaire existant.
- Documentation technique et politique de signalement de vulnérabilités clarifiées.
- Code applicatif et comportement réseau à auditer et tester séparément avant toute annonce de nouvelles capacités.

## 1.0.0 — Version initiale

- Application Windows Forms inspirée du concept AntiNuke.
- Tentatives d'écoute TCP/UDP sur une liste de ports configurée.
- Lecture de la table TCP Windows via `iphlpapi.dll`.
- Actions de filtrage par Pare-feu Windows et surveillance de processus.
- Interface sombre et icône dans la zone de notification.

> Les fonctionnalités décrites dépendent de la configuration de Windows et ne garantissent pas la détection ni le blocage de toutes les menaces.
