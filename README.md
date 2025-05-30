# 🎮 Figma-Style Unity App with Coin Tap Game

A professional Unity mobile application featuring a complete UI flow and an engaging coin collection mini-game, built with enterprise-level architecture and mobile optimization.

## 📋 Table of Contents
- [Features](#-features)
- [Demo](#-demo)
- [Architecture](#-architecture)
- [Technical Specifications](#-technical-specifications)
- [Performance](#-performance)
- [Screenshots](#-screenshots)

---

## ✨ Features

### 🎯 **Core Gameplay**
- **30-Second Coin Tap Game** - Fast-paced coin collection with real-time scoring
- **Smart Coin Spawning** - Advanced algorithm prevents overlapping coins
- **Object Pooling System** - Memory-efficient coin management for smooth performance
- **Real-Time Score Tracking** - Live score updates with high score persistence
- **Visual Timer** - Countdown with progress bar visualization

### 🎨 **User Interface**
- **Professional UI Flow** - Entry → Login → Menu → Game navigation
- **Input Validation** - Real-time phone number (10-digit) and password validation
- **Smooth Animations** - DOTween-powered transitions and button feedback
- **Loading Screens** - Animated loading with progress bars and tips
- **Mobile-Optimized Layout** - Responsive design for various screen sizes

### 🔊 **Audio System**
- **Dynamic Background Music** - Scene-specific BGM with smooth crossfading
- **Interactive Sound Effects** - Coin collection, button clicks, and game events
- **Volume Controls** - Master, BGM, and SFX volume sliders
- **Audio Persistence** - Settings saved between sessions
- **Mobile Audio Handling** - Proper pause/resume on app focus changes

### 📱 **Mobile Features**
- **Touch-Optimized Input** - Responsive touch detection with proper hit areas
- **Android Back Button** - Native navigation handling
- **Exit Confirmation** - User-friendly app exit confirmation
- **Safe Area Support** - Compatible with notched devices
- **Performance Optimization** - 60 FPS target with memory management

---

## 🎥 Demo

### Game Flow
1. **Entry Screen** - Animated logo with login button
2. **Login Screen** - Phone number and password validation
3. **Loading Screen** - Smooth transition with progress indication
4. **Main Menu** - Play game, settings, and exit options
5. **Coin Tap Game** - 30-second coin collection gameplay
6. **Results Screen** - Final score with restart/menu options

---

## 🏗️ Architecture

### **Design Patterns Used**
- **State Machine Pattern** - Clean game state management (Playing, Paused, GameOver)
- **Observer Pattern** - Event-driven communication via GameEvents system
- **Object Pool Pattern** - Efficient memory management for coins
- **Singleton Pattern** - Manager classes with proper lifecycle
- **MVC Pattern** - Separation of UI, logic, and data

### **SOLID Principles**
- ✅ **Single Responsibility** - Each class has one clear purpose
- ✅ **Open/Closed** - Extensible through interfaces and events
- ✅ **Liskov Substitution** - Proper inheritance hierarchy
- ✅ **Interface Segregation** - Focused interfaces (IInteractable, IPoolable)
- ✅ **Dependency Inversion** - Dependencies through events and abstractions

### **Core Systems**
```
GameStateMachine → Manages game states and transitions
GameEvents → Centralized event communication
AudioManager → Professional audio management with fading
SceneLoader → Async scene loading with loading screens
CoinSpawner → Smart spawning with overlap prevention
ObjectPool<T> → Generic, reusable pooling system
```

---

## 🔧 Technical Specifications

### **Performance Targets**
- **Target FPS:** 60 (mobile optimized)
- **Memory Usage:** < 100MB RAM
- **Battery Efficiency:** Optimized for mobile devices
- **Load Times:** < 2 seconds between scenes

### **Supported Platforms**
- **Primary:** Android (API 21+)
- **Architecture:** ARM64
- **Screen Resolutions:** 720p to 1440p+
- **Aspect Ratios:** 16:9, 18:9, 19.5:9

### **Audio Specifications**
- **BGM Format:** OGG Vorbis, 44.1kHz, Stereo
- **SFX Format:** WAV, 44.1kHz, Mono
- **Compression:** Optimized for mobile
- **Latency:** < 50ms for interactive sounds

### **Code Quality**
- **Architecture:** Clean Architecture with SOLID principles
- **Testing:** Unit testable components
- **Documentation:** Comprehensive code comments
- **Performance:** Zero GC allocations during gameplay
- **Memory:** Efficient object pooling and caching

---

## 📊 Performance

### **Optimization Features**
- **Object Pooling** - Eliminates runtime allocations
- **Cached References** - Minimizes expensive lookups
- **Efficient Coroutines** - WaitForSeconds caching
- **Smart UI Updates** - Event-driven UI refresh
- **Audio Optimization** - Compressed audio with streaming

### **Memory Management**
- **Pool Sizes:** Configurable via ScriptableObjects
- **Garbage Collection:** Minimized allocations
- **Asset Management:** Efficient loading/unloading
- **UI Efficiency:** CanvasGroup-based optimizations

### **Mobile Optimizations**
- **Touch Input:** Layer-based collision detection
- **Battery Life:** Proper pause/resume handling  
- **Performance Scaling:** Adaptive quality settings
- **Network Efficiency:** Offline-first design

---

## 📸 Screenshots

### Main Flow
| Entry Screen | Login Screen | Main Menu | Coin Game |
|--------------|--------------|-----------|-----------|
| ![Entry](screenshots/entry.png) | ![Login](screenshots/login.png) | ![Menu](screenshots/menu.png) | ![Game](screenshots/game.png) |

### Features
| Audio Settings | Loading Screen | Game Over | High Score |
|----------------|----------------|-----------|------------|
| ![Audio](screenshots/audio.png) | ![Loading](screenshots/loading.png) | ![GameOver](screenshots/gameover.png) | ![Score](screenshots/score.png) |

---

## 🎮 Gameplay Features

### **Coin Collection Mechanics**
- **Spawn Algorithm:** Prevents overlapping with minimum distance checking
- **Visual Feedback:** Smooth scale animations on spawn/collect
- **Audio Feedback:** Satisfying collection sound effects
- **Score System:** Real-time updates with combo potential

### **Game Balance**
- **Duration:** 30 seconds for focused gameplay sessions
- **Difficulty:** Progressive coin spawn rate
- **Scoring:** Points per coin with potential multipliers
- **Challenge:** Random spawn positions keep gameplay fresh

### **Progression System**
- **High Score Tracking:** Persistent across sessions
- **Achievement Ready:** Extensible scoring system
- **Statistics:** Foundation for detailed analytics
- **Replay Value:** Quick restart functionality

---

## 🛡️ Quality Assurance

### **Testing Coverage**
- ✅ **Unit Tests:** Core game logic
- ✅ **Integration Tests:** Scene transitions
- ✅ **Performance Tests:** Memory and FPS profiling
- ✅ **Device Tests:** Multiple Android devices
- ✅ **User Experience:** Complete flow testing

### **Error Handling**
- **Graceful Degradation:** Handles missing assets
- **Input Validation:** Prevents invalid states
- **Audio Fallbacks:** Continues without audio if needed
- **Save System:** Robust data persistence

### **Accessibility**
- **Touch Targets:** Minimum 44px for comfortable interaction
- **Visual Feedback:** Clear state indicators
- **Audio Cues:** Sound effects for important actions
- **Simple Navigation:** Intuitive user flow

---

## 🙏 Acknowledgments

- **Unity Technologies** - Game engine and tools
- **DOTween** - Animation system
- **TextMeshPro** - Advanced text rendering
- **Community Assets** - Various free assets used

---

## 📞 Support

For support, email ajai.gamedev@gmail.com or create an issue in this repository.

---

*Built with ❤️ using Unity 2021.3 and modern game development practices*
