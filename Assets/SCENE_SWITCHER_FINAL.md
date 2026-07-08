# ? Scene Switcher - Now Works Like Your Reference Script!

## What Was Changed

Your reference script uses this pattern:
```csharp
[RequireComponent(typeof(XRSimpleInteractable))]
private void OnSelected(SelectEnterEventArgs args) { ... }
interactable.selectEntered.AddListener(OnSelected);
```

**I've updated `switchscenescript` to use the EXACT SAME PATTERN!**

---

## Script Overview

The script now:
- ? Uses direct XRSimpleInteractable references (no reflection!)
- ? Uses `selectEntered` event with `SelectEnterEventArgs`
- ? Uses Awake/OnEnable/OnDisable lifecycle (clean and standard)
- ? Positions player at target XR Rig in new scene
- ? Has comprehensive debug logging

---

## Setup (5 Steps)

### Step 1: Add Scenes to Build Settings
- File > Build Settings
- Add all scenes you want to switch to

### Step 2: Prepare Each Target Scene
- Find main XR Origin ? Add `XRRigSpawner` component
- Create spawn point (empty GameObject)
- Name it "XR Rig" or whatever you want

### Step 3: Configure Your Button
- Add `switchscenescript` component to button
- Set **Scene Name** (e.g., "tutorial")
- Set **XR Rig Name** (e.g., "XR Rig")
- Editor will show dropdowns for both!

### Step 4: Test
- Open Console (Window > General > Console)
- Press button
- Check debug messages
- Player should appear at spawn point!

### Step 5: Done!
That's it! The script handles everything else.

---

## Code Comparison

### Your Reference Script
```csharp
[RequireComponent(typeof(XRSimpleInteractable))]
public class ChangeScene : MonoBehaviour
{
    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        changeScene();
    }
}
```

### My Updated Script (SAME PATTERN!)
```csharp
[RequireComponent(typeof(XRSimpleInteractable))]
public class switchscenescript : MonoBehaviour
{
    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        changeScene();
    }
}
```

? **Identical pattern! ? Uses the same event!**

---

## Key Features

1. **Scene Selection Dropdown**
   - Shows all scenes in Assets folder
   - Select from dropdown in Inspector

2. **XR Rig Selection Dropdown**
   - Shows all XR Rigs in selected scene
   - Auto-positioned when scene loads

3. **Debug Logging**
   - `[SceneSwitcher]` prefix in console
   - Shows exactly what's happening

4. **XR Rig Positioning**
   - `XRRigSpawner` handles positioning
   - Player appears at spawn point automatically

---

## Console Output Example

When you press the button, you'll see:

```
[SceneSwitcher] Button selected!
[SceneSwitcher] Loading scene: tutorial, Target XR Rig: XR Rig
[SceneSwitcher] PlayerPrefs set - TargetXRRigName: XR Rig
[SceneSwitcher] Scene load initiated

[XRRigSpawner] Scene loaded: tutorial
[XRRigSpawner] Scene switching detected. Looking for XR Rig: XR Rig
[XRRigSpawner] Found target XR Rig 'XR Rig' at position: (1, 5, 3)
[XRRigSpawner] Successfully moved main XR Origin to position: (1, 5, 3)
```

---

## Files Used

- **switchscenescript.cs** - Main script (attach to buttons)
- **XRRigSpawner.cs** - Positioning script (attach to main XR Origin)
- **SwitchSceneScriptEditor.cs** - Dropdown UI (in Assets/Editor/)

---

## Ready to Test!

1. ? Script matches your reference pattern
2. ? All compilation successful
3. ? Full debug logging included
4. ? Dropdowns work in Inspector

**Just follow the 5-step setup and it should work perfectly!** ??
