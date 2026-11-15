###### PUBLISH.md >> markdown
# EXE.exe

#### 🚀 Commandes pour publier sur GitHub

- **Une fois tous ces fichiers créés dans votre dossier EXE.exe** :

```bash
git init
git add .
git commit -m "feat: initial release v1.0.0 - EXE.exe bouclier reseau & gouvernance GitHub"
git branch -M main
git remote add origin https://github.com/teremuhamblin /EXE.exe.git
git push -u origin main
```

- Si vous souhaitez publier le binaire compilé EXE.exe proprement sur GitHub :
   - compilez-le avec build.bat, puis ajoutez-le dans l'onglet Releases → Draft a new release (avec le tag v1.0.0), ce qui activera automatiquement votre badge dynamique de release dans le README.md.
