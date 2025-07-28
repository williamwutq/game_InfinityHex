namespace Core
{
    /// <summary>
    /// Manages a directional state represented by a modular index in the range [0, 5], 
    /// allowing clockwise and counter-clockwise rotations. 
    /// Thread-safe operations are provided for safe concurrent use.
    /// 
    /// This class works in conjunction with <see cref="Hex.HexLib.CircularSixBlock"/> 
    /// to convert the directional index hexagonal coordinates as defined in the method.
    /// </summary>
    public class DirectionManager
    {
        private int direction;
        private readonly bool mirrored;
        private readonly System.Threading.Lock syncRoot = new();
        /// <summary>
        /// Initializes a new instance of <see cref="DirectionManager"/> with direction set to 0.
        /// </summary>
        public DirectionManager()
        {
            mirrored = false;
            direction = 0;
        }
        /// <summary>
        /// Initializes a new instance of <see cref="DirectionManager"/> with direction set to 0 and custom mirrored status.
        /// </summary>
        /// <param name="mirrored">Whether left / right input are to be mirrored.</param>
        public DirectionManager(bool mirrored)
        {
            this.mirrored = mirrored;
            direction = 0;
        }
        /// <summary>
        /// Initializes a new instance of <see cref="DirectionManager"/> with a given initial direction.
        /// The direction is wrapped into the [0, 5] range using modulo arithmetic.
        /// Negative values are supported and normalized.
        /// </summary>
        /// <param name="initialDirection">The initial direction index (can be negative).</param>
        public DirectionManager(int initialDirection)
        {
            mirrored = false;
            direction = ((initialDirection % 6) + 6) % 6; // Handles negative input safely
        }
        /// <summary>
        /// Initializes a new instance of <see cref="DirectionManager"/> with a given initial direction and mirrored status.
        /// The direction is wrapped into the [0, 5] range using modulo arithmetic.
        /// Negative values are supported and normalized.
        /// </summary>
        /// <param name="mirrored">Whether left / right input are to be mirrored.</param>
        /// <param name="initialDirection">The initial direction index (can be negative).</param>
        public DirectionManager(bool mirrored, int initialDirection)
        {
            this.mirrored = mirrored;
            direction = ((initialDirection % 6) + 6) % 6; // Handles negative input safely
        }
        /// <summary>
        /// Returns the current direction index converted to a hexagonal coordinate.
        /// The conversion is circular, with 0 to 5 mapping to hex directions.
        /// Mapping is done with <see cref="Hex.HexLib.CircularSixBlock"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Hex.Hex"/> representing the direction offset in a hex grid.
        /// </returns>
        public Hex.Hex GetOffset()
        {
            lock (syncRoot) return Hex.HexLib.CircularSixBlock(direction);
        }
        /// <summary>
        /// Returns the current direction index in the range [0, 5].
        /// </summary>
        /// <returns>An integer representing the current direction index.</returns>
        public int GetOffsetIndex()
        {
            lock (syncRoot) return direction;
        }
        /// <summary>
        /// Sets the direction index based on a hexagonal coordinate.
        /// The coordinate is converted to a direction index using <see cref="Hex.HexLib.CircularSixBlockIndex"/>.
        /// </summary>
        /// <param name="coordinate">The hexagonal coordinate to set the direction from.</param>
        /// <exception cref="ArgumentNullException">Thrown if the coordinate is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the coordinate is not a valid hexagonal coordinate.</exception>
        /// <see cref="DirectionManager.Set(int direction)"/> 
        public void Set(Hex.Hex coordinate)
        {
            int direction = Hex.HexLib.CircularSixBlockIndex(coordinate);
            lock (syncRoot)
            {
                this.direction = direction;
            }
        }
        /// <summary>
        /// Sets the direction index to a specific value, wrapping it into the [0, 5] range.
        /// Negative values are supported and normalized.
        /// </summary>
        /// <param name="direction">The new direction index to set.</param>
        public void Set(int direction)
        {
            lock (syncRoot) this.direction = ((direction % 6) + 6) % 6; // Handles negative input safely
        }
        /// <summary>
        /// Performs a no-op (no operation), but still counts as a synchronized access.
        /// Useful for logging, locking coordination, or sequencing without modifying state.
        /// </summary>
        public void Noop()
        {
            lock (syncRoot) _ = direction; // This does nothing
        }
        /// <summary>
        /// Rotates the direction by one step.
        /// Clockwise if <paramref name="direction"/> is true, counter-clockwise if false.
        /// </summary>
        /// <param name="direction">
        /// Direction to rotate: true for clockwise, false for counter-clockwise.
        /// </param>
        public void Move(bool direction)
        {
            if (direction != mirrored) Increment(); else Decrement();
        }
        /// <summary>
        /// Rotates the direction one step clockwise, or increment the direction index.
        /// </summary>
        public void Increment()
        {
            lock (syncRoot) direction = (direction + 1) % 6;
        }
        /// <summary>
        /// Rotates the direction one step counter-clockwise, or decrement the direction index.
        /// </summary>
        public void Decrement()
        {
            lock (syncRoot) direction = (direction + 5) % 6;
        }
        /// <summary>
        /// Returns a string representation of the current direction index.
        /// </summary>
        /// <returns>A string describing the DirectionManager state.</returns>
        public override string ToString()
        {
            lock (syncRoot) return $"DirectionManager(direction={direction})";
        }
    }
}