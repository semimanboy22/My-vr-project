# Complete Car Movement & Checkpoint System Guide

## Table of Contents
1. [Movement Settings](#movement-settings)
2. [Random Start Delay](#random-start-delay)
3. [Random Speed](#random-speed)
4. [Checkpoint System](#checkpoint-system)
5. [Respawn System](#respawn-system)

---

## Movement Settings

### Basic Movement Controls
- **Move Direction (X, Y, Z)**: Direction the car moves
  - Default: (1, 0, 0) - moves right
  - Normalized automatically
- **Move Speed**: Base speed of the car (units per second)
  - Default: 5
  - Overridden by random speed if enabled
- **Max Distance**: How far the car travels before resetting
  - Default: 10 units
- **Is Moving**: Whether the car is currently moving
- **Loop Movement**: When reaching max distance, reset to start and repeat
  - Enabled: Car loops forever
  - Disabled: Car stops after reaching max distance

---

## Random Start Delay

### How It Works
When the car starts moving OR starts a new loop cycle, it can wait a random amount of time before actually moving.

### Inspector Settings
- **Use Random Delay**: Enable/disable random delay feature
- **Min Random Delay**: Minimum wait time in seconds (default: 0)
- **Max Random Delay**: Maximum wait time in seconds (default: 1.5)

### Example
```
Min Delay: 0.5 seconds
Max Delay: 2.0 seconds

Result: Car waits 0.5-2.0 seconds before each movement
```

### Code Usage
```csharp
Car carScript = GetComponent<Car>();

// Enable random delay with custom range
carScript.SetRandomDelay(
    useRandom: true,
    minDelay: 0.5f,
    maxDelay: 2.0f
);

// Disable random delay
carScript.SetRandomDelay(useRandom: false);

// Get remaining delay time
float timeRemaining = carScript.GetRandomDelayRemaining();
```

---

## Random Speed

### How It Works
Each time the car starts moving (including loop resets), it picks a random speed within the specified range. The car then maintains that speed for the entire movement cycle.

### Inspector Settings
- **Use Random Speed**: Enable/disable random speed feature
- **Min Random Speed**: Minimum speed in units/second (default: 3)
- **Max Random Speed**: Maximum speed in units/second (default: 7)

### How Speeds Work
- **Random Speed OFF**: Car always moves at "Move Speed"
- **Random Speed ON**: Car picks random speed between min/max for each cycle
  - A new random speed is chosen every time:
    - The car starts moving initially
    - The car resets and loops
    - The random delay expires

### Example Scenario
```
Move Speed: 5 (ignored when Random Speed is ON)
Min Random Speed: 2
Max Random Speed: 8

Cycle 1: Car picks speed 4.2 units/sec for this movement
Cycle 2: Car waits (random delay), then picks speed 6.8 units/sec
Cycle 3: Car picks speed 3.5 units/sec
... and so on
```

### Code Usage
```csharp
Car carScript = GetComponent<Car>();

// Enable random speed with custom range
carScript.SetRandomSpeed(
    useRandom: true,
    minSpeed: 2f,
    maxSpeed: 8f
);

// Disable random speed (use Move Speed value)
carScript.SetRandomSpeed(useRandom: false);

// Set just the move speed (used when Random Speed is OFF)
carScript.SetMoveSpeed(5f);
```

### Combined Random Delay + Random Speed

You can use both together for maximum variation:

```
Scenario: Dynamic obstacle course

Cycle 1:
- Wait 0.8 seconds (random delay)
- Move at 3.2 units/sec (random speed)

Cycle 2:
- Wait 1.5 seconds (random delay)
- Move at 7.1 units/sec (random speed)

Cycle 3:
- Wait 0.3 seconds (random delay)
- Move at 4.6 units/sec (random speed)

Result: Each cycle is completely different!
```

---

## Checkpoint System

### Visual Indicators in Scene View

When you enable checkpoints, you'll see:
- **Green Box**: Checkpoint detection zone
- **Green Box Outline**: Clear boundary of checkpoint area
- **Red Sphere**: Respawn point for that checkpoint
- **Red Line**: Connection from checkpoint to respawn point
- **Yellow Cube**: Checkpoint order marker
- **Blue Sphere**: Default respawn point

### Adding Checkpoints

1. Select your Car GameObject
2. In Inspector, find "Checkpoint Settings"
3. Enable "Use Checkpoints"
4. Edit "Checkpoints List"
5. For each checkpoint, configure:
   - **Checkpoint Position (X, Y, Z)**: Where checkpoint is located
   - **Respawn Point (X, Y, Z)**: Where player spawns
   - **Order**: Priority number (higher = more important)
   - **Checkpoint Name**: Optional label
   - **Trigger Axes**:
     - **Trigger On X Axis**: ? Detect X position match
     - **Trigger On Y Axis**: ? Detect Y position match
     - **Trigger On Z Axis**: ? Detect Z position match

### Axis Trigger System

Each axis can be individually enabled/disabled:

- **All axes enabled**: Full 3D detection (most restrictive)
- **Only Y enabled**: Height-based detection
- **X and Z only**: Horizontal detection (height ignored)
- **No axes enabled**: Always triggers (not recommended)

### Checkpoint Detection Range
- Per-axis tolerance: ±1.5 units
- Checkpoint activates when all **enabled axes** match

### Priority System

The checkpoint with the **highest order number** you've reached is always your respawn point:

```
Hit Checkpoint 1 (order: 1) ? Respawn at CP1
Hit Checkpoint 2 (order: 2) ? Respawn at CP2
Move back to CP1 area ? Still respawn at CP2 (higher order)
```

### Code Usage
```csharp
Car carScript = GetComponent<Car>();

// Add checkpoint
carScript.AddCheckpoint(
    checkpointPosition: new Vector3(10, 0, 0),
    respawnPoint: new Vector3(10, 1, 0),
    order: 1,
    triggerX: true,
    triggerY: true,
    triggerZ: true
);

// Get current checkpoint
int currentOrder = carScript.GetCurrentCheckpointOrder();
Vector3 respawnPos = carScript.GetCurrentRespawnPoint();

// Remove checkpoint
carScript.RemoveCheckpointByOrder(1);

// Clear all
carScript.ClearCheckpoints();
```

---

## Respawn System

### How It Works
When the player collides with the car hitbox:
- If checkpoints are enabled: Respawn at highest-order checkpoint
- If no checkpoints hit: Respawn at default respawn point

### Inspector Settings
- **Use Respawn**: Enable/disable respawn feature
- **Default Respawn Point (X, Y, Z)**: Where player spawns if no checkpoints reached

### Hitbox System
- **Use Hitbox**: Enable/disable collision detection
- **Hitbox Size (X, Y, Z)**: Size of collision box
- **Hitbox Offset (X, Y, Z)**: Position offset from car

### Code Usage
```csharp
Car carScript = GetComponent<Car>();

// Set respawn settings
carScript.SetUseRespawn(true);
carScript.SetDefaultRespawnPoint(new Vector3(0, 1, 0));

// Adjust hitbox
carScript.SetHitboxSize(new Vector3(1, 1, 1));
carScript.SetHitboxOffset(new Vector3(0, 0.5f, 0));
carScript.EnableHitbox(true);

// Set player reference
carScript.SetPlayer(playerGameObject);
```

---

## Complete Workflow Example

```csharp
// Get car script
Car car = GetComponent<Car>();

// Setup movement
car.SetMoveDirection(Vector3.right);
car.SetMaxDistance(50f);
car.SetMoveSpeed(5f);  // Base speed
car.SetLoopMovement(true);

// Setup random delay (before each movement)
car.SetRandomDelay(
    useRandom: true,
    minDelay: 0.5f,
    maxDelay: 1.5f
);

// Setup random speed (different for each cycle)
car.SetRandomSpeed(
    useRandom: true,
    minSpeed: 3f,
    maxSpeed: 8f
);

// Setup respawn
car.SetDefaultRespawnPoint(new Vector3(0, 1, 0));
car.SetPlayer(player);
car.SetUseRespawn(true);

// Setup hitbox
car.SetHitboxSize(1f, 1f, 1f);
car.EnableHitbox(true);

// Setup checkpoints via code
car.AddCheckpoint(
    checkpointPosition: new Vector3(10, 0, 0),
    respawnPoint: new Vector3(10, 1, 0),
    order: 1,
    triggerX: true, triggerY: true, triggerZ: true
);

car.AddCheckpoint(
    checkpointPosition: new Vector3(25, 0, 0),
    respawnPoint: new Vector3(25, 1, 0),
    order: 2,
    triggerX: true, triggerY: true, triggerZ: true
);

// Start movement
car.StartMoving();
```

---

## Tips & Best Practices

### Random Speed Tips
- **Obstacle avoidance**: Use random speed to vary timing, making obstacles unpredictable
- **Difficulty scaling**: High range = more difficulty variation
- **Performance**: Each loop rerandomizes speed for freshness
- **Balance**: Keep min/max close for subtle variation, wide for dramatic changes

### Random Delay Tips
- **Setup time**: Allow delay for player preparation
- **Synchronization**: Use delay with checkpoints for synchronized events
- **Visual feedback**: Display remaining delay to player

### Checkpoint Tips
- **Strategic placement**: Place at key track locations
- **Axis selection**: Use Y-only for height challenges, X-Z for horizontal
- **Order matters**: Always use increasing order numbers
- **Visual verification**: Use gizmos to confirm positions in Scene view

### Combined Features
- Use random speed + delay for unpredictable movement patterns
- Use checkpoints + respawn to create safe zones
- Use axis triggers for complex detection scenarios
