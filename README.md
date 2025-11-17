# 🐱 Infinite Cat Runner - Platformer Infini

## 📖 Concept du Jeu

Un platformer infini avec génération procédurale où le joueur contrôle un chat qui doit esquiver des obstacles. La particularité du jeu réside dans le système de **joueur shadow** :

- **Joueur Principal (MainPlayer)** : Chat en haut de l'écran
  - Map défile de **droite vers gauche** (comme le dinosaure Google)
  - Obstacles arrivent de la droite

- **Joueur Shadow (ShadowPlayer)** : Chat en bas de l'écran
  - Map défile de **gauche vers droite** (sens inverse)
  - Obstacles arrivent de la gauche

- **Mécanique de jeu** : Les deux joueurs effectuent **les mêmes actions** (mouvements synchronisés), mais doivent éviter des obstacles différents. Le joueur doit coordonner ses mouvements pour survivre aux deux niveaux simultanément.

---

## ✅ Étapes Validées et Implémentées

### 1. **Structure de Base de la Scène** ✅
- ✅ Création de la scène `mainScene`
- ✅ GameObject `MainPlayer` (joueur du haut) avec sprite de chat
- ✅ GameObject `ShadowPlayer` (joueur du bas) avec sprite de chat
- ✅ GameObject `TopMap` (map du haut) avec tiles de sol
- ✅ GameObject `BottomMap` (map du bas) avec tiles de sol
- ✅ GameObject `MapManager` (gestionnaire central)

### 2. **Système de Mouvement Synchronisé** ✅
- ✅ Script `PlayerMovement` (dans `PlayerMovementBottom.cs`)
  - Contrôle le `ShadowPlayer` (joueur du bas)
  - Mouvement horizontal avec touches A/D
  - Synchronisation automatique :
    - Le `MainPlayer` bouge en sens inverse (synchronisé)
    - Les maps sont maintenant gérées par les `MapGenerator` (plus de défilement manuel)
  - Références configurées : `topPlayer`, `topMap`, `bottomMap`

### 3. **Système de Génération Procédurale** ✅
- ✅ **MapGenerator.cs** : Générateur procédural de chunks
  - ✅ **Intégré aux deux maps** : `TopMap` et `BottomMap`
  - ✅ **Génération initiale** : Chunks générés sur tout l'axe X au démarrage (2 écrans de largeur de chaque côté)
  - ✅ **Défilement automatique** : Les chunks défilent automatiquement selon leur sens configuré
  - ✅ **Recyclage intelligent** : Chunks sortis de l'écran sont recyclés et repositionnés au bord d'entrée
  - ✅ **Génération continue** : Nouveaux chunks générés automatiquement pour maintenir le flux infini
  - ✅ **Détection des bords d'écran** : Utilise la caméra principale pour calculer les bords
  - Support pour défilement droite→gauche (TopMap) ou gauche→droite (BottomMap)
  - Gestion de la difficulté progressive
  - Paramètres configurables : `chunkWidth`, `chunksAhead`, `scrollSpeed`, `scrollRightToLeft`

- ✅ **MapChunk.cs** : Représente un segment de map
  - Gestion des obstacles dans un chunk
  - Méthodes `ClearObstacles()` et `AddObstacle()`
  - Visualisation Gizmo dans l'éditeur

- ✅ **ObstaclePattern.cs** : ScriptableObject pour définir des patterns
  - Classe `ObstaclePlacement` pour positionner les obstacles
  - Position relative dans le chunk (0-1)
  - Décalage vertical depuis le sol
  - Niveau de difficulté associé (0-10)
  - Menu de création : `Assets > Create > Game > Obstacle Pattern`

### 4. **Intégration Complète du Système de Génération** ✅
- ✅ **MapGenerator attaché à TopMap** :
  - `scrollRightToLeft = true` (défilement droite→gauche)
  - Chunks apparaissent depuis le bord droit de l'écran
  - Défilent vers la gauche et disparaissent à gauche
- ✅ **MapGenerator attaché à BottomMap** :
  - `scrollRightToLeft = false` (défilement gauche→droite)
  - Chunks apparaissent depuis le bord gauche de l'écran
  - Défilent vers la droite et disparaissent à droite
- ✅ **Prefabs de chunks configurés** : `topChunk` et `bottomChunk`
- ✅ **Génération initiale fonctionnelle** : Les deux maps ont des chunks sur tout l'axe X au démarrage

### 5. **Architecture Technique** ✅
- ✅ Utilisation de `UnityEngine.InputSystem` pour les contrôles
- ✅ Système de queue pour gérer les chunks actifs
- ✅ Support bidirectionnel du défilement (configurable)
- ✅ Détection automatique de la caméra principale
- ✅ Calcul dynamique des bords d'écran
- ✅ Structure modulaire et extensible

---

## 🚧 Prochaines Étapes à Implémenter

### Priorité Haute

1. **Système de Collision/Détection**
   - [ ] Ajouter des colliders aux obstacles
   - [ ] Détection de collision joueur/obstacle
   - [ ] Système de Game Over
   - [ ] Affichage du score/distance parcourue

2. **Mouvement Vertical (Saut)**
   - [ ] Ajouter la possibilité de sauter (touche Espace)
   - [ ] Synchroniser le saut entre les deux joueurs
   - [ ] Gestion de la gravité et du sol

### Priorité Moyenne

3. **Système d'Obstacles**
   - [ ] Créer des prefabs d'obstacles variés
   - [ ] Créer plusieurs `ObstaclePattern` ScriptableObjects
   - [ ] Différencier les patterns pour TopMap et BottomMap
   - [ ] Ajouter des obstacles qui nécessitent de sauter

4. **Progression de Difficulté**
   - [ ] Augmenter automatiquement la difficulté avec la distance
   - [ ] Ajuster la vitesse de défilement progressivement
   - [ ] Varier la fréquence des obstacles

5. **UI et Feedback**
   - [ ] Interface de score
   - [ ] Écran de Game Over
   - [ ] Indicateur de difficulté
   - [ ] Particules/effets visuels

### Priorité Basse

6. **Polish et Optimisation**
   - [ ] Animations du chat (course, saut)
   - [ ] Sons et musique
   - [ ] Effets de particules
   - [ ] Optimisation des performances

---

## 📁 Structure des Fichiers

```
Assets/
├── Scripts/
│   ├── MapGenerator.cs          # Générateur procédural de chunks (intégré)
│   ├── MapChunk.cs              # Segment de map réutilisable
│   ├── ObstaclePattern.cs      # ScriptableObject pour patterns
│   └── PlayerMovementBottom.cs # Contrôle du joueur (nom à corriger)
├── Prefabs/
│   ├── topChunk.prefab         # Prefab de chunk pour TopMap
│   └── bottomChunk.prefab      # Prefab de chunk pour BottomMap
├── Scenes/
│   └── mainScene.unity         # Scène principale
└── Sprites/
    └── [Assets visuels]
```

---

## 🎮 Contrôles Actuels

- **A** : Déplacer à gauche (les deux joueurs)
- **D** : Déplacer à droite (les deux joueurs)
- **Espace** : (À implémenter) Sauter

---

## 📝 Notes Techniques

- Le script `PlayerMovement` est actuellement attaché au `ShadowPlayer`
- Le nom du fichier `PlayerMovementBottom.cs` ne correspond pas au nom de la classe (`PlayerMovement`)
- ✅ **MapGenerator intégré** : Les deux maps utilisent maintenant le système de génération procédurale
- ✅ **Défilement automatique** : Les chunks défilent automatiquement, plus besoin de défilement manuel
- ✅ **Génération initiale** : Les chunks sont générés sur tout l'axe X (2 écrans de largeur) au démarrage
- ✅ **Recyclage fonctionnel** : Les chunks sortis de l'écran sont recyclés automatiquement
- Les prefabs de chunks doivent être configurés dans l'inspecteur de chaque MapGenerator

---

## 🔄 Dernière Mise à Jour

**Date** : 17 novembre 2025  
**Dernière modification** : Intégration complète du système de génération procédurale
- MapGenerator intégré aux deux maps (TopMap et BottomMap)
- Génération initiale de chunks sur tout l'axe X
- Défilement automatique fonctionnel
- Recyclage des chunks opérationnel

---

## 💡 Prochaines Actions Recommandées

1. **Créer des obstacles** : Créer quelques prefabs d'obstacles et patterns de test
2. **Ajouter le saut** : Implémenter le mouvement vertical pour éviter les obstacles
3. **Système de collision** : Détecter quand le joueur touche un obstacle
4. **Game Over** : Implémenter l'écran de fin de partie

---

*Ce README sera mis à jour au fur et à mesure de l'avancement du projet.*
