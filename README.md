# 📚 Bibliothèque — application multiplateforme .NET MAUI

Application de gestion de bibliothèque permettant à des lecteurs de consulter un catalogue de livres, de les mettre en favoris et de les évaluer, et à des administrateurs de gérer le catalogue et les comptes utilisateurs.

Projet réalisé en C# / .NET 9 avec **.NET MAUI**, organisé selon une architecture **MVVM en quatre projets** avec injection de dépendances et tests unitaires.

> **Statut** — projet scolaire (DEC en Techniques de l'informatique, profil Génie logiciel). Cible : Android, iOS, macOS et Windows à partir d'une base de code unique.

---

## ✨ Fonctionnalités

**Côté lecteur**

- Authentification par compte avec gestion des rôles (`RoleCompte`) et session utilisateur persistante
- Consultation du catalogue de livres avec page de détail
- Gestion d'une liste de favoris personnelle
- Évaluation des livres

**Côté administrateur**

- Tableau de bord dédié
- Ajout de livres au catalogue
- Suppression de livres
- Gestion des comptes utilisateurs

---

## 🏗️ Architecture

La solution est découpée en quatre projets pour isoler les responsabilités et rendre la logique métier testable indépendamment de l'interface.

```
View.sln
├── Bibliotheque.Model/       # Entités du domaine — aucune dépendance
│   ├── Livre.cs
│   ├── Compte.cs
│   ├── RoleCompte.cs
│   ├── SessionUtilisateur.cs
│   └── ResultatAuthentification.cs
│
├── Bibliotheque.Services/    # Logique métier, derrière des interfaces
│   ├── IAuthService.cs             / AuthService.cs
│   ├── IBibliothequeXmlService.cs  / BibliothequeXmlService.cs
│   └── IFavorisService.cs          / FavorisService.cs
│
├── Bibliotheque.ViewModel/   # ViewModels MVVM
│   ├── BaseViewModel.cs      # INotifyPropertyChanged mutualisé
│   ├── LoginViewModel.cs
│   ├── ListeLivresViewModel.cs
│   ├── DetailLivreViewModel.cs
│   ├── FavorisViewModel.cs
│   ├── EvaluationViewModel.cs
│   └── Admin*ViewModel.cs
│
├── View/                     # Interface .NET MAUI (XAML)
│   ├── AppShell.xaml         # Navigation
│   └── *Page.xaml            # Une page par écran
│
└── Bibliotheque.Tests/       # Tests unitaires
    ├── BibliothequeXmlServiceTests.cs
    └── LivresTests.cs
```

**Choix techniques**

- **MVVM strict** — les pages XAML ne contiennent pas de logique métier ; tout passe par les ViewModels, ce qui rend l'application testable sans lancer l'interface.
- **Programmation par interfaces** — chaque service est exposé via une interface (`IAuthService`, `IBibliothequeXmlService`, `IFavorisService`) et injecté depuis `MauiProgram.cs`. Remplacer la persistance XML par une base de données ne demanderait que d'écrire une nouvelle implémentation.
- **Persistance XML** — sérialisation du catalogue et des comptes dans des fichiers XML, sans dépendance externe ni serveur.
- **Tests unitaires** — la couche services est couverte par des tests, indépendamment de l'interface.

---

## 🛠️ Stack

| | |
|---|---|
| Langage | C# / .NET 9 |
| Interface | .NET MAUI (XAML) |
| Architecture | MVVM, injection de dépendances |
| Persistance | Sérialisation XML |
| Tests | Projet de tests unitaires dédié |
| Plateformes | Android 21+, iOS 15+, macOS (Mac Catalyst) 15+, Windows 10 17763+ |

---

## 🚀 Lancer le projet

**Prérequis** — [.NET 9 SDK](https://dotnet.microsoft.com/download) et la charge de travail MAUI :

```bash
dotnet workload install maui
```

**Cloner et compiler :**

```bash
git clone https://github.com/MinaSeliman1/View.git
cd View
dotnet restore
dotnet build
```

**Lancer sur Windows :**

```bash
dotnet run --project View --framework net9.0-windows10.0.19041.0
```

**Lancer sur Android** (émulateur démarré) :

```bash
dotnet build View --framework net9.0-android -t:Run
```

**Exécuter les tests :**

```bash
dotnet test
```

---

## 📸 Aperçu

<!-- TODO : ajouter 2 ou 3 captures d'écran de l'application.
     Crée un dossier docs/, dépose-y tes images, puis remplace le bloc ci-dessous par :

     | Connexion | Catalogue | Favoris |
     |---|---|---|
     | ![Connexion](docs/login.png) | ![Catalogue](docs/catalogue.png) | ![Favoris](docs/favoris.png) |
-->

_Captures d'écran à venir._

---

## 🎯 Ce que ce projet m'a appris

- Structurer une solution .NET en couches indépendantes plutôt qu'en un seul projet monolithique
- Appliquer MVVM pour séparer réellement l'interface de la logique métier
- Concevoir des services derrière des interfaces pour les rendre substituables et testables
- Écrire des tests unitaires sur une couche métier
- Gérer l'authentification et des permissions par rôle dans une application cliente

---

## 👤 Auteur

**Mina Seliman** — étudiant en Techniques de l'informatique, profil Génie logiciel

[GitHub](https://github.com/MinaSeliman1) · [LinkedIn](https://linkedin.com/in/mina-seliman)
