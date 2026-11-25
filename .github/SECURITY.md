# 🔐 Politique de sécurité

## Versions concernées

Le projet est en cours de maintenance autour de la version **1.5**. Les versions antérieures peuvent ne plus recevoir de correctifs ; utilisez la dernière version publiée et consultez les notes de version. Une version indiquée dans le README n'est pas nécessairement une release publiée : vérifiez la page [Releases](https://github.com/teremuhamblin/EXE.exe/releases).

## Portée et limites

EXE.exe interagit avec des informations de connexions réseau, des processus locaux et le Pare-feu Windows. Le manifeste demande actuellement des privilèges administrateur au démarrage. N'exécutez l'application que si vous avez examiné et approuvé le code et le binaire utilisés.

EXE.exe est un outil complémentaire de surveillance. Il ne garantit pas la détection ou le blocage de toutes les menaces et ne remplace ni un antivirus, ni les protections Windows, ni les bonnes pratiques de sauvegarde. Les règles de pare-feu peuvent interrompre des communications légitimes. Vérifiez leur portée et leurs effets avant de les appliquer ou de les supprimer.

Les rapports de sécurité doivent notamment préciser, dans la mesure du possible :

- la version ou le commit concerné et la version de Windows ;
- les étapes de reproduction et le comportement observé ;
- l'impact potentiel et les journaux pertinents, après suppression de toute donnée sensible.

Ne joignez pas de secrets, de données personnelles, d'adresses IP publiques sensibles ou de fichiers malveillants non nécessaires au diagnostic.

## Signaler une vulnérabilité

**Ne publiez pas une vulnérabilité non corrigée dans une Issue publique.** Utilisez la fonction privée **Security → Advisories → Report a vulnerability** du dépôt GitHub. Si cette fonction n'est pas disponible, contactez le mainteneur via les coordonnées de [MAINTAINERS.md](MAINTAINERS.md).

Nous accuserons réception et coordonnerons la divulgation dès que possible. Aucun délai de réponse ou de correction n'est garanti ; les échéances dépendront de la gravité et des éléments disponibles.

## Utilisation responsable

- Téléchargez les sources et releases uniquement depuis le dépôt officiel.
- Examinez les changements et le script `build.bat` avant compilation ; vérifiez le SHA-256 publié lorsqu'un checksum est disponible.
- Testez les changements de pare-feu dans un environnement maîtrisé et conservez un moyen de restaurer la configuration.
- Accordez les privilèges élevés uniquement à un binaire de confiance.
