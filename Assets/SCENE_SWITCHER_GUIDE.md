# XR Scene Switcher Script - Setup Guide

## Overview
The `switchscenescript.cs` works with **XR Simple Interactable** buttons to switch scenes and position the player at a specific XR Origin (XR Rig) in the target scene. **Automatically detects ALL scenes AND XR Rigs in your project!**

## Available Scenes in Your Project

The script automatically scans your Assets folder and displays ALL `.unity` scene files, including:

1. **level-1** - Level 1
2. **tutorial** - Tutorial scene
3. **Lock and Key Scene** - Lock and Key Asset Pack scene
4. **DemoScene** - XR Interaction Toolkit Demo Scene
5. **SampleScene** - Sample scene
6. **Sukkel** - Sukkel scene
7. **+ Any other scenes in Assets/**

## How to Use

### Step 1: Add the Script to Your Button
1. Create a button GameObject (or use an existing one with XR Simple Interactable)
2. Attach the `switchscenescript.cs` component to it
3. Make sure the GameObject also has an **XRSimpleInteractable** component

### Step 2: Configure the Script (Super Easy Dropdowns!)
In the Inspector, you'll see two dropdowns:

#### **Target Scene**
- Click the dropdown and select the scene you want to switch to
- The editor automatically scans all scenes in your Assets folder

#### **XR Rig in Target Scene**
- After you select a target scene, a second dropdown appears
- This dropdown automatically shows ALL XR Origins/XR Rigs found in that scene
- Just select the one where you want the player to spawn
- The XR Rig name is automatically filled in!

**That's it! No manual typing needed at all.**

### How the Auto-Detection Works
1. You select a scene from the first dropdown
2. The editor loads that scene in the background
3. It scans the scene for GameObjects named with "XR Origin", "XR Rig", etc.
4. Shows you all found XR Rigs in the second dropdown
5. You just click and select!

### Step 3: Test
Press your button in VR - it should:
1. Fade out the current scene
2. Load the selected target scene
3. Player will be positioned at the selected XR Origin location in the new scene

## Example Configuration

### Button in Level-1 ? Switch to Tutorial:
1. **Target Scene dropdown**: Select `tutorial`
2. **XR Rig in Target Scene dropdown**: See all XR Rigs in tutorial ? Select `XR Origin`
3. Done! Button is ready to use

### Button in Tutorial ? Switch to Level-1:
1. **Target Scene dropdown**: Select `level-1`
2. **XR Rig in Target Scene dropdown**: See all XR Rigs in level-1 ? Select `XR Origin`
3. Done! Button is ready to use

## Files Included

- **switchscenescript.cs** - Main scene switcher script
- **SwitchSceneScriptEditor.cs** - Editor script with automatic scene and XR Rig detection (in Assets/Editor/)
- **XRRigSpawner.cs** - Optional helper script for your XR Rigs

## What XR Origins/Rigs Are Detected

The editor automatically finds GameObjects named:
- "XR Origin"
- "XR Rig"
- "XROrigin"
- "XRRig"
- Or any variation containing these terms

Make sure your XR Rigs in each scene are named clearly with one of these terms!

## Troubleshooting

- **No XR Rigs showing in dropdown**: Make sure your XR Origins are named with "XR Origin" or "XR Rig" in the name
- **Scene not loading**: Click "Refresh Scene & XR Rig List" button to update the dropdown
- **No scenes showing**: Verify the `.unity` files are in the Assets folder
- **No response on button press**: Ensure the GameObject has an XRSimpleInteractable component
- **Compilation errors**: Make sure the XR Interaction Toolkit package is installed in your project
- **Scene not loading at runtime**: Scenes must be added to Build Settings (File > Build Settings) to load at runtime


