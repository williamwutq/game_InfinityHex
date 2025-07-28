# InfinityHex

InfinityHex is a hexagonal grid-based game built with C# and Avalonia UI. The game features a snake-like gameplay on an infinite hexagonal grid where players navigate through hexagonal blocks.

This will be a fun game to play with a grid of unlimited size. Preformance optimizations will be added later.

Enjoy :)  

<b>Author</b>: William Wu  
<b>Written in</b>: C#  
<b>Last edited</b>: 28/07/2025  
<b>Latest release</b>: Game is still in beta  

> [!NOTE]
> The project may contain bugs:  
> - Report bugs by create an [issue](https://github.com/williamwutq/game_InfinityHex/issues).  

## License
Distributed under the MIT License

[License Link](https://github.com/williamwutq/game_InfinityHex/blob/main/LICENSE)

Copyright: William Wu © 2025

### Features
- Infinite hexagonal grid for gameplay  
- Customizable themes via JSON configuration  
- Interactive UI with hints system  
Cross-platform support via Avalonia UI framework  
- Keyboard navigation with multiple control schemes  

### Game Structure
The game is structured into several main components:

- Hex System: Core hexagonal grid implementation with coordinate systems
- Game Engine: Manages game state, movement, and collision detection
- UI Components: Launcher, game interface, and interactive elements
- [Keyboard controls](#controls): Controling the direction of snake through keys
- [Theme](#themes) System: Customizable appearance through JSON theme files

### Game Engine
The hexagonal game engine is the core of the hexagonal snake game, and it handles data processing, transformation, collision checking, and coordinate shifting efficiently using relative coordinate system, relative time system, and multiple delegates to runners for fetching blocks, generating blocks, and converting between absolute and relative time and coordinates.  

#### Coordinate System
InfinityHex reuses a specialized hexagonal coordinate system from [HappyHex](https://github.com/williamwutq/game_HappyHex) to represent positions on the grid. This system is designed to simplify operations on hexagonal grids, such as movement, adjacency checks, and transformations. The coordinate system includes both raw coordinates and line-based coordinates. These two systems are interdependent and interchangable, although not all raw coordinates map to a integer line coordinate.  

<b>Raw Coordinates</b>  

- The raw coordinate system uses three axes: I, J, and K.  
- These axes run diagonally through the hexagonal grid:  
  - I+ is 60° to J+.
  - J+ is 60° to K+.
  - K+ is 60° to I-.
- The relationship between the axes is defined as: `I + J + K = 0`  
- Each hexagon is represented by a triplet (i, j, k) where one of the values is redundant due to the above constraint.
- Visualization:  
```
Hex Coordinates (i, j, k)
    I
   / * (5, 4, -1)
  /     * (5, 7, 2)
 o - - J
  \ * (0, 3, 3)
   \
    K
```

<b>Line Coordinates</b>  

- Line coordinates are derived from the raw coordinates and represent distances perpendicular to the axes, in contrast to alone the axes.  
- The relationships for line coordinates are:  
  - LineI = (2I + K) / 3
  - LineJ = (I - K) / 3
  - LineK = (2I + K) / 3
- Line coordinates simplify operations like movement and adjacency checks.
- Visualization:  
```
Line Coordinates (I, J, K)
    I
   / * (1, 2, 3)
  /     * (3, 1, 4)
 o - - J
  \ * (2, -1, 1)
   \
    K
```

This coordinate system is implemented in the `Hex` class and extended by other components like `Block`, which added color property, and `HexLib`, which  is a library that accelerates the handling of directional coordinates that are within one unit distance to the origin. These ensure efficient and intuitive handling of hexagonal grid operations.

#### Coordinate Manager

To avoid shifting coordinates of all `Block` references during a snake move, the coordinates are managed by a `CoordinateManager`, which helps to convert between relative and absolute coordinates.

- <b>Origin</b>: The offset between the relative and absolute system, used to convert between these systems. To avoid too large bound, it is reset to zero periodically.  
- <b>Relative Coordinates</b>: These are coordinates relative to the current origin of the grid. They simplify operations like movement and rendering within a localized window.  
- <b>Absolute Coordinates</b>: These are global coordinates that represent the actual position of a hex in the infinite grid. They only change during an origin reset.  

For the snake, this means that the head of the snake is always at the origin, effectively moving the view grid with the snake head when the snake move, avoiding snake head wandering out of view and ensuring user experience.  

#### `TimedObject<T>` Class

The `TimedObject<T>` class is a thread-safe generic class that associates an object with a time value. It is used to track the age or duration of objects in the game. This is useful for cache management and definition of the snake, especially the tail.  

Key features include:
- <b>Thread-Safe Time Manipulation</b>: Methods like `Age()`, `SetTime()`, and `Renew()` ensure safe concurrent access.  
- <b>Equality and Comparison</b>: Implements equality and comparison based on both the object and its time value.  
- <b>Cloning</b>: Supports shallow cloning for creating copies of timed objects.  

This class serves as the foundation for engine cache management by providing time metadata for cache objects.  

#### Time Manager

Similar to the use of [Coordinate Manager](#coordinate-manager), the HexEngine also manages time using relative and absolute time to avoid repeated age calls.  

- <b>Relative Time</b>: Represents the time difference relative to the current time reference. It is used for operations like aging objects and checking expiration.
- <b>Absolute Time</b>: Represents the global time value, which is reset periodically.
- <b>Expliration</b>: Present as constant in both relative and absolute time. Objects with age greater than this value are considered to be expired.  

The `TimeReferenceManager` handles time conversions and ensures thread-safe operations. It provides methods to age objects, reset the time reference, and check for expiration.  

For the snake, time management means that every block of the snake has different time stemp: the head of the snake has time of 0, and the tail of the snake has time equals to the snake length.  

#### Cache Management

Based on time and coordinate managers, the cache is managed efficiently using a `LinkedList<TimedObject<Block>>`. This cache is used to store recently accessed or generated blocks, ensuring efficient retrieval and management of grid data. This is critical for lazy block creation and unused or old cache collection that supports the infinite grid nature of the game. In later versions, cache management will be progressively improved.

- The engine uses cache search and lazy block creation. The engine searches the cache for blocks using their coordinates. If a block is not found, it is generated with a generator and added to the cache.  
- The engine handles cache expiration by removing Blocks that exceed the expiration threshold (managed by `TimeReferenceManager`) from the cache.  
- When the grid is updated (e.g., due to movement or time aging), the cache is adjusted to reflect the new state.

The cache ensures that the engine can efficiently manage a large, infinite grid while maintaining performance and consistency and it will be continue to be updated with new techniques.

#### Graphics Interface

The engine implements the `IHexPrintable` interface, which provides methods for interacting with, updating, and rendering the hexagonal grid in both GUI and ASCII configurations. The GUI build only use the GUI configuration. This interface contains methods to get radius and block array from the engine, checking whether rendering is updated, and more. This interface ensures that the grid can be rendered consistently across different platforms and output formats.

### Graphics

InfinityHex uses Avalonia UI for its graphical interface, ensuring cross-platform compatibility and a responsive design. The application starts with a `MainWindow` that dynamically loads interactive pages.

To start the game, launch the application, and this will leads you to the Launch Page.  

#### Launch Page
The initial page displayed is the **Launch Page**, which includes:
- A randomly chosen game hint displayed at the top of the page.
- The current version number prominently shown alongside the hint.
- A **Start** button centered on the page to begin the game.
- Copyright information displayed at the bottom of the page.

#### Game Page
When the **Start** button is clicked, the application transitions to the **Game Page**, where players can control the snake using the keyboard. The game page features:
- Real-time gameplay on an infinite hexagonal grid.
- Keyboard controls for navigating the snake.
- A responsive UI that adapts to the player's actions.
- Game data fetched from the backend via the [IHexPrintable](#graphics-interface) interface.

#### Quit Game
Pressing the **Escape** key at any time during gameplay will return the user to the **Launch Page**, allowing them to restart or exit the game.

#### Quit Application
In any time, clicking on the quit button on top of the screen will quit the application.

### Themes
All configuration are stored in the [config](config) directory, which contains a [settings](config/Setting.json) JSON file that can be used to configure the default theme to use.

Custom themes can be created in the [themes](config/themes) directory using JSON format.

The default theme is stored as [Default.json](config/themes/Default.json).

You are encouraged to create your own themes by adding json files into the [themes](config/themes) directory.

### Controls
The game supports multiple control schemes:

Left/Decrement: ←, A, Q, U, J  
- These keys are used to direct the snake to turn left 60 degrees.  

Right/Increment: →, D, E, O, L
- These keys are used to direct the snake to turn right 60 degrees.

No Operation: ↑, W, I
- These keys result in no operation, and will not modify the direction of the snake. In the future, they might be used to change the speed of the game.

Escape: Quit game
- This quits the current game and returns to the main page.

