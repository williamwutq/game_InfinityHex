namespace Core
{
    /// <summary>
    /// Manages hexagonal grid coordinates with movement tracking and origin reset functionality.
    /// Implements a coordinate system for hexagonal grids, with limits on moves and spatial range.
    /// This manager is thread-safe, and it can maintain global spacial reference consistency across threads.
    /// </summary>
    public class CoordinateManager
    {
        private readonly int moveLimit;
        private readonly int spaceLimit;
        private readonly System.Threading.Lock lockObject = new();
        private Hex.Hex origin;
        private int moves;

        private HandleCoordinateReset? coordinateResetHandler;
        /// <summary>
        /// Delegate for handling origin reset events.
        /// </summary>
        /// <param name="oldOrigin">The origin before reset.</param>
        public delegate void HandleCoordinateReset(Hex.Hex oldOrigin);
        /// <summary>
        /// Sets the handler for coordinate reset events.
        /// </summary>
        /// <param name="coordinateResetHandler">The handler to invoke on reset.</param>
        public void SetCoordinateResetHandler(HandleCoordinateReset coordinateResetHandler)
        {
            this.coordinateResetHandler = coordinateResetHandler;
        }

        /// <summary>
        /// Initializes a new CoordinateManager with specified limits.
        /// </summary>
        /// <param name="moveLimit">Maximum moves before origin reset (default: 40).</param>
        /// <param name="spaceLimit">Maximum distance from origin before reset (default: 16).</param>
        public CoordinateManager(int moveLimit, int spaceLimit)
        {
            this.moveLimit = moveLimit;
            this.spaceLimit = spaceLimit;
            this.origin = new Hex.Hex();
            this.moves = 0;
        }
        /// <summary>
        /// Moves the origin by the specified offset and handles reset conditions.
        /// </summary>
        /// <param name="offset">Hex offset to move the origin by.</param>
        /// <remarks>
        /// Increments move count and updates origin. If moveLimit or spaceLimit is exceeded,
        /// resets origin to (0,0,0), clears move count, and invokes reset handler with old origin.
        /// </remarks>
        public void Move(Hex.Hex offset)
        {
            lock (lockObject)
            {
                moves++;
                Hex.Hex offsetOrigin = origin.Add(offset);
                if (moves >= moveLimit || !offsetOrigin.InRange(spaceLimit))
                {
                    origin = new Hex.Hex();
                    moves = 0;
                    coordinateResetHandler?.Invoke(offsetOrigin);
                }
                else
                {
                    origin = offsetOrigin;
                }
            }
        }

        /// <summary>
        /// Resets the origin to (0,0,0) and clears move count.
        /// </summary>
        public void Reset()
        {
            lock (lockObject)
            {
                origin = new Hex.Hex();
                moves = 0;
            }
        }

        /// <summary>
        /// Gets a copy of the current origin.
        /// </summary>
        /// <returns>A cloned Hex representing the current origin.</returns>
        public Hex.Hex GetOrigin()
        {
            lock (lockObject)
            {
                return this.origin.HexClone();
            }
        }

        /// <summary>
        /// Converts a relative coordinate to an absolute coordinate by adding the origin.
        /// </summary>
        /// <param name="relativeCoordinate">The relative coordinate to convert.</param>
        /// <returns>The absolute coordinate.</returns>
        /// <exception cref="ArgumentNullException">Thrown if relativeCoordinate is null.</exception>
        public Hex.Hex ToAbsolute(Hex.Hex relativeCoordinate)
        {
            System.ArgumentNullException.ThrowIfNull(relativeCoordinate);
            lock (lockObject)
            {
                return relativeCoordinate.Add(this.origin);
            }
        }
        /// <summary>
        /// Converts an absolute coordinate to a relative coordinate by subtracting the origin.
        /// </summary>
        /// <param name="absoluteCoordinate">The absolute coordinate to convert.</param>
        /// <returns>The relative coordinate.</returns>
        /// <exception cref="ArgumentNullException">Thrown if absoluteCoordinate is null.</exception>
        public Hex.Hex ToRelative(Hex.Hex absoluteCoordinate)
        {
            System.ArgumentNullException.ThrowIfNull(absoluteCoordinate);
            lock (lockObject)
            {
                return absoluteCoordinate.Subtract(this.origin);
            }
        }
        /// <summary>
        /// Converts a block in relative coordinate to a block in absolute coordinate by adding the origin.
        /// </summary>
        /// <param name="relativeBlock">The relative block to convert.</param>
        /// <returns>The absolute block.</returns>
        /// <exception cref="ArgumentNullException">Thrown if relativeBlock is null.</exception>
        public Hex.Block ToAbsolute(Hex.Block relativeBlock)
        {
            System.ArgumentNullException.ThrowIfNull(relativeBlock);
            lock (lockObject)
            {
                return relativeBlock.Add(this.origin);
            }
        }
        /// <summary>
        /// Converts a block in absolute coordinate to a block in relative coordinate by subtracting the origin.
        /// </summary>
        /// <param name="absoluteBlock">The absolute block to convert.</param>
        /// <returns>The relative block.</returns>
        /// <exception cref="ArgumentNullException">Thrown if absoluteBlock is null.</exception>
        public Hex.Block ToRelative(Hex.Block absoluteBlock)
        {
            System.ArgumentNullException.ThrowIfNull(absoluteBlock);
            lock (lockObject)
            {
                return absoluteBlock.Subtract(this.origin);
            }
        }
        /// <summary>
        /// Converts an array of relative coordinates to absolute coordinates.
        /// </summary>
        /// <param name="relativeCoordinates">Array of relative coordinates.</param>
        /// <returns>Array of absolute coordinates.</returns>
        public Hex.Hex[] ToAbsolute(Hex.Hex[] relativeCoordinates)
        {
            return System.Array.ConvertAll(relativeCoordinates, coo => ToAbsolute(coo));
        }
        /// <summary>
        /// Converts an array of absolute coordinates to relative coordinates.
        /// </summary>
        /// <param name="absoluteCoordinates">Array of absolute coordinates.</param>
        /// <returns>Array of relative coordinates.</returns>
        public Hex.Hex[] ToRelative(Hex.Hex[] absoluteCoordinates)
        {
            return System.Array.ConvertAll(absoluteCoordinates, coo => ToRelative(coo));
        }
        /// <summary>
        /// Converts an array of blocks in relative coordinates to absolute coordinates.
        /// </summary>
        /// <param name="relativeBlocks">Array of blocks in relative coordinates.</param>
        /// <returns>Array of blocks in absolute coordinates.</returns>
        public Hex.Block[] ToAbsolute(Hex.Block[] relativeBlocks)
        {
            return System.Array.ConvertAll(relativeBlocks, coo => ToAbsolute(coo));
        }
        /// <summary>
        /// Converts an array of blocks in absolute coordinates to relative coordinates.
        /// </summary>
        /// <param name="absoluteBlocks">Array of blocks in absolute coordinates.</param>
        /// <returns>Array of blocks in relative coordinates.</returns>
        public Hex.Block[] ToRelative(Hex.Block[] absoluteBlocks)
        {
            return System.Array.ConvertAll(absoluteBlocks, coo => ToRelative(coo));
        }
        /// <summary>
        /// Returns a string representation of the CoordinateManager.
        /// </summary>
        /// <returns>A string describing the current origin and move count.</returns>
        public override string ToString()
        {
            lock (lockObject)
            {
                return $"CoordinateManager(origin={origin}/{spaceLimit}, moves={moves}/{moveLimit})";
            }
        }
    }
}