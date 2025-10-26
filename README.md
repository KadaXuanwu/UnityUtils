# About 

This package contains utility tools I use for game and tool development, designed to be reusable across multiple projects. I update it whenever I create something in my workflow that can be generalized and shared.

Feel free to submit pull requests on GitHub with your suggestions or join me on Discord to discuss improvements.

Discord: https://discord.com/invite/d5zmfkT5nr
Website: https://turtlecodelabs.com/

# Content

## Runtime
### Components
- **ButtonIgnoreTransparent** - Makes UI buttons ignore clicks on transparent pixels
- **DestroyAfterDelay** - Automatically destroys GameObject after specified delay
- **DestroyOnCollision** - Destroys GameObject when colliding with specific layers
- **DontRotateWithParent** - Cancels out Z-axis rotation inherited from parent
- **FollowTransform** - Makes GameObject follow target transform's position and rotation
- **FollowTransformCode** - Code-controlled following with position and rotation offsets

### DataStructures
- **BiDirectionalDictionary** - Bidirectional dictionary for efficient lookup by key or value
- **GapFillingList** - List that reuses gaps from removed items instead of always appending
- **MaxHeap** - Max heap with O(log n) insertion/extraction, O(1) peek
- **MinHeap** - Min heap with O(log n) insertion/extraction, O(1) peek
- **NormalizedFloat** - Float automatically clamped to [0, 1] range
- **Singleton** - Base class for implementing singleton pattern
- **TimedList** - List with automatic element add/remove after specified time

### EventBus
- **EventBinding** - Binding between events and callback handlers
- **EventBus** - Centralized messaging system for decoupled communication
- **EventBusUtil** - Utility methods for managing event buses
- **Events** - Marker interface for custom event types

### Extensions
- **ColorExtensions** - Extension methods for Unity Color (alpha manipulation)

### Helper
- **NavMeshUtility** - NavMesh operations (random points, pathfinding validation, bounds)
- **PredefinedAssemblyUtil** - Get all types implementing specific interfaces from assemblies
- **Tools** - General utilities (color conversion, GameObject manipulation, math, type checking)

### ObjectPooling
- **ObjectPool** - Singleton object pooling system to reduce instantiation overhead

### Systems
- **CoroutineRunner** - Start coroutines from non-MonoBehaviour classes
- **ModifierManager** - Generic manager for temporary/permanent modifiers (buffs, debuffs, status effects)
- **PooledTimerSystem** - High-performance timer system using object pooling and sorted linked list

## Editor
### Components
Enhanced inspector UI for runtime components with validation and helpful warnings:
- **ButtonIgnoreTransparentEditor** - Validates Image component presence
- **DestroyAfterDelayEditor** - Shows warning for negative delays
- **DestroyOnCollisionEditor** - Validates Collider and Rigidbody presence
- **DontRotateWithParentEditor** - Warns if no parent exists
- **FollowTransformCodeEditor** - Shows runtime target info and available methods
- **FollowTransformEditor** - Validates target transform and detects self-referencing

### EditorEnhancements
- **ShowGlobalRotation** - Custom Transform inspector showing global position, rotation, and scale
