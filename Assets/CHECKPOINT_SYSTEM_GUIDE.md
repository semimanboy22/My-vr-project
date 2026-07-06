# Checkpoint System Guide - Updated with Axis Triggers

## Overview
The checkpoint system allows you to set multiple checkpoints at specific 3D positions (X, Y, Z coordinates) along your track. You can now choose which axes the player must match to trigger the checkpoint respawn.

## Visual Indicators in Scene View

When you enable checkpoints, you'll see the following in the Scene view:
- **Green Box**: The checkpoint detection zone (where the player triggers the checkpoint)
- **Green Box Outline**: Clear boundary of the checkpoint detection area
- **Red Sphere**: The respawn point for that checkpoint (where the player will spawn)
- **Red Line**: Connection line from checkpoint to its respawn point
- **Yellow Cube**: Checkpoint order indicator marker
- **Blue Sphere**: Default respawn point (if no checkpoints are hit)

## How to Use

### Adding Checkpoints in the Inspector

1. **Select your Car GameObject** in the scene
2. **In the Inspector**, find the "Checkpoint Settings" section
3. **Enable "Use Checkpoints"** checkbox if not already enabled
4. **Edit "Checkpoints List"** - you can add/remove checkpoints directly here
5. **Configure each checkpoint:**
   - **Checkpoint Position (X, Y, Z)**: The 3D coordinates where the checkpoint is located
   - **Respawn Point (X, Y, Z)**: The 3D coordinates where the player will spawn after hitting this checkpoint
   - **Order**: The checkpoint priority number (higher = higher priority)
   - **Checkpoint Name**: A name to identify this checkpoint (optional)
   - **Trigger Axes**:
     - **Trigger On X Axis**: ? Player must be within 1.5 units on X axis
     - **Trigger On Y Axis**: ? Player must be within 1.5 units on Y axis
     - **Trigger On Z Axis**: ? Player must be within 1.5 units on Z axis

### Axis Trigger System Explained

The "Trigger Axes" checkboxes control which dimensions must match for the checkpoint to activate:

**Example 1: All Axes Enabled (Default)**
- Player must match X, Y, AND Z within 1.5 units ? Checkpoint triggers
- This is the most selective (requires all 3 conditions met)

**Example 2: Only X and Z Enabled**
- Player must match X AND Z within 1.5 units (Y doesn't matter)
- Useful for checkpoints where Y (height) varies

**Example 3: Only Y Enabled**
- Player only needs to match Y within 1.5 units (X and Z don't matter)
- Useful for detecting when player reaches a certain height

**Example 4: No Axes Enabled**
- Checkpoint always triggers when near it (not recommended, but possible)

### Single Source of Truth

- **Only one checkpoint list** - checkpoints are managed in one place
- Edit checkpoints directly in the "Checkpoints List" 
- No duplication between Inspector and runtime system
- Changes automatically sync when you modify the list

### Example Setups

**Setup 1: Racing Track (All Axes)**
```
Checkpoint 1:
- Checkpoint Position: (5, 0, 0)
- Respawn Point: (10, 0, 0)
- Order: 1
- Trigger X: ?, Y: ?, Z: ?
```

**Setup 2: Vertical Challenge (Height-Based)**
```
Checkpoint 2:
- Checkpoint Position: (20, 10, 5)
- Respawn Point: (22, 12, 5)
- Order: 2
- Trigger X: ?, Y: ?, Z: ?
(Only cares about reaching height 10)
```

**Setup 3: Lane Detector (X-Z Only)**
```
Checkpoint 3:
- Checkpoint Position: (0, 5, 15)
- Respawn Point: (0, 8, 15)
- Order: 3
- Trigger X: ?, Y: ?, Z: ?
(Ignores player height, only checks horizontal position)
```

## How Checkpoints Work

1. **As the player moves** in the world, the system checks proximity to checkpoints
2. **Axis-based detection** - player position is compared to checkpoint position on enabled axes
3. **When all enabled axes match** (within 1.5 units), the checkpoint activates
4. **The player's respawn point updates** to that checkpoint's respawn point
5. **Higher order checkpoints take priority** - if you hit checkpoint 1 then checkpoint 2, you respawn at checkpoint 2's point
6. **No checkpoint reached** = respawn at default respawn point

## Checkpoint Priority System

The checkpoint with the **highest order number** that you've passed is always your respawn point.

Example:
- You pass Checkpoint 1 (order: 1) ? Respawn at Checkpoint 1
- You pass Checkpoint 2 (order: 2) ? Respawn at Checkpoint 2
- You move away and approach Checkpoint 1 again ? Still respawn at Checkpoint 2 (higher order)

## Gizmo Visualization Guide

In the Scene view, look for:

| Element | Color | Meaning |
|---------|-------|---------|
| Green Box with outline | Green | Checkpoint detection zone (1x1x1 unit box) |
| Red Sphere | Red | Respawn point for that checkpoint |
| Red Line | Red | Connection between checkpoint and respawn |
| Yellow Cube | Yellow | Checkpoint order marker |
| Blue Sphere | Blue | Default respawn point |

## Checkpoint Detection Range

- **Per-axis detection**: 1.5 units on each enabled axis
- Checkpoints detect the player when they match all **enabled axes**
- This allows for flexible trigger zones in 3D space

## Scriptable Methods (For Code)

If you want to add checkpoints programmatically:

```csharp
Car carScript = GetComponent<Car>();

// Add a checkpoint with all axes enabled
carScript.AddCheckpoint(
    checkpointPosition: new Vector3(10, 0, 0), 
    respawnPoint: new Vector3(10, 1, 0), 
    order: 1,
    triggerX: true,
    triggerY: true,
    triggerZ: true
);

// Add a checkpoint that only cares about Y (height)
carScript.AddCheckpoint(
    checkpointPosition: new Vector3(20, 10, 5), 
    respawnPoint: new Vector3(22, 12, 5), 
    order: 2,
    triggerX: false,
    triggerY: true,
    triggerZ: false
);

// Get current checkpoint order
int currentOrder = carScript.GetCurrentCheckpointOrder();

// Get current respawn point
Vector3 respawnPos = carScript.GetCurrentRespawnPoint();

// Remove a checkpoint by order
carScript.RemoveCheckpointByOrder(1);

// Clear all checkpoints
carScript.ClearCheckpoints();

// Enable/disable checkpoint system
carScript.SetUseCheckpoints(true);
```

## Tips

- **Use axis selection strategically** - only enable needed axes to allow more flexibility
- **Space checkpoints strategically** - place them at important track locations
- **Use higher order numbers** for checkpoints further along the race/course
- **Watch the green boxes** in Scene view to ensure checkpoints are positioned correctly
- **Check red spheres** to verify respawn points are accessible to the player
- **Test checkpoint detection** - move the player near checkpoints to verify they trigger
- **Consider your track design** - use Y-axis for height-based challenges, X-Z for horizontal positioning
