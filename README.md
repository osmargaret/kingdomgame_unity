# Kingdom Quest — Unity 3D Project (`kingdomgame_unity`)

This folder holds the **Unity 3D** project and C# scripts for **Destination 3: Bible Adventures**, designed to integrate with `kingdomgame_front` via `UnityBridge.ts`.

## 1. Open in Unity Hub
1. Install **Unity Hub** and **Unity 2022.3 LTS** (or **Unity 6 LTS**) with **Android Build Support (SDK/NDK/IL2CPP)** and **iOS Build Support** (macOS).
2. Create/open `kingdomgame_unity/` in Unity Hub so Unity generates `ProjectSettings/` and `Packages/`.
3. Add a bootstrap scene containing a GameObject named **`KingdomBridge`** with `Assets/Scripts/Bridge/KingdomBridge.cs` attached.
4. Create and add your 5 adventure scenes to **File → Build Settings**:
   - `Scene_TheExodus`
   - `Scene_TheGreatArk`
   - `Scene_JosephEgypt`
   - `Scene_ValleyOfElah`
   - `Scene_QueensMission`

## 2. Export Unity as a Library (UaaL)
- **Android:** Export to `kingdomgame_front/unity/builds/android/` (`unityLibrary`).
- **iOS:** Export to `kingdomgame_front/unity/builds/ios/` (`UnityFramework.framework`).

## 3. Connect to `kingdomgame_front`
1. In `kingdomgame_front/`:
   ```bash
   npm install @azesmway/react-native-unity
   ```
2. Uncomment `UnityMessageManager.Instance.SendMessageToRN(json);` in `KingdomBridge.cs` once the `@azesmway/react-native-unity` Unity package is imported.
3. Build a custom Dev Client (Unity does not run in Expo Go):
   ```bash
   npx expo prebuild
   npx expo run:android
   ```

