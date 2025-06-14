
namespace Hex
{
    /// <summary>
    /// From Java implementation of Block from the HappyHex project.
    /// <para>
    /// The <c>Block</c> class inherits from <see cref="Hex"/> and represents a colored block with an occupancy state
    /// within the hexagonal grid system.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>Block</c> inherits the coordinate system from <see cref="Hex"/>. Refer to <see cref="Hex"/> for details on the hexagonal coordinate
    /// system, including both raw and line-based coordinates.
    /// </para>
    /// <para>
    /// In addition to the coordinate functionality provided by <see cref="Hex"/>, each <c>Block</c> instance encapsulates:
    /// <list type="bullet">
    ///   <item>
    ///     <description>A color index indicating the block's color. -1 represents the typical empty color for the block,
    ///       -2 is the default filled color for the block, and 0-n represent the real colors generated.</description>
    ///   </item>
    ///   <item>
    ///     <description>A boolean state indicating whether the block is occupied (<c>true</c>) or unoccupied (<c>false</c>).</description>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// The class provides various constructors and static factory methods for creating blocks using either
    /// standard (i, k) coordinates or line indices. It also includes methods for moving, shifting, adding, and
    /// subtracting coordinates, as well as modifying and retrieving the block's state and color.
    /// </para>
    /// </remarks>
    /// <seealso cref="Hex"/>
    /// <since>0.1</since>
    /// <author>William Wu</author>
    /// <version>0.1</version>
    public class Block : Hex
    {
        private int color;
        private bool state;
        
        /// <summary>
        /// Constructs a block at the specified (i, k) coordinates with an unoccupied state.
        /// </summary>
        /// <param name="i">The i-coordinate.</param>
        /// <param name="k">The k-coordinate.</param>
        public Block(int i, int k) : base(i, k)
        {
            this.state = false;
            this.color = -1;
        }
        /// <summary>
        /// Constructs a block at the specified (i, k) coordinates with a specified color and unoccupied state.
        /// </summary>
        /// <param name="i">The i-coordinate.</param>
        /// <param name="k">The k-coordinate.</param>
        /// <param name="color">The color of the block.</param>
        public Block(int i, int k, int color) : base(i, k)
        {
            this.state = false;
            this.color = color;
        }
        /// <summary>
        /// Constructs a block at the specified (i, k) coordinates with a specified state.
        /// </summary>
        /// <param name="i">The i-coordinate.</param>
        /// <param name="k">The k-coordinate.</param>
        /// <param name="state">The state of the block.</param>
        public Block(int i, int k, bool state) : base(i, k)
        {
            if (state)
            {
                this.color = -2;
            }
            else
            {
                this.color = -1;
            }
            this.state = state;
        }
        /// <summary>
        /// Constructs a block at the specified (i, k) coordinates with a specified color and state.
        /// </summary>
        /// <param name="i">The i-coordinate.</param>
        /// <param name="k">The k-coordinate.</param>
        /// <param name="color">The color of the block.</param>
        /// <param name="state">The state of the block.</param>
        public Block(int i, int k, int color, bool state) : base(i, k)
        {
            this.state = state;
            this.color = color;
        }
        /// <summary>
        /// Constructs a block at the specified hex coordinates with unoccupied color and state.
        /// </summary>
        /// <param name="hex">The hex coordinate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hex"/> passed in is null.</exception>
        public Block(Hex hex) : base(hex.I, hex.K)
        {
            System.ArgumentNullException.ThrowIfNull(hex);
            this.state = false;
            this.color = -1;
        }
        /// <summary>
        /// Constructs a block at the specified hex coordinates with a specified color and unoccupied state.
        /// </summary>
        /// <param name="hex">The hex coordinate.</param>
        /// <param name="color">The color of the block.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hex"/> passed in is null.</exception>
        public Block(Hex hex, int color) : base(hex.I, hex.K)
        {
            System.ArgumentNullException.ThrowIfNull(hex);
            this.state = false;
            this.color = color;
        }
        /// <summary>
        /// Constructs a block at the specified hex coordinates with a specified state.
        /// </summary>
        /// <param name="hex">The hex coordinate.</param>
        /// <param name="state">The state of the block.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hex"/> passed in is null.</exception>
        public Block(Hex hex, bool state) : base(hex.I, hex.K)
        {
            System.ArgumentNullException.ThrowIfNull(hex);
            if (state)
            {
                this.color = -2;
            }
            else
            {
                this.color = -1;
            }
            this.state = state;
        }
        /// <summary>
        /// Constructs a block at the specified hex coordinates with a specified color and state.
        /// </summary>
        /// <param name="hex">The hex coordinate.</param>
        /// <param name="color">The color of the block.</param>
        /// <param name="state">The state of the block.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hex"/> passed in is null.</exception>
        public Block(Hex hex, int color, bool state) : base(hex.I, hex.K)
        {
            System.ArgumentNullException.ThrowIfNull(hex);
            this.state = state;
            this.color = color;
        }
        /// <summary>
        /// Creates a block using hexagonal line indices and assigns it a specific color.
        /// The block is shifted accordingly in the coordinate system.
        /// </summary>
        /// <param name="i">The I-line index in the hexagonal coordinate system.</param>
        /// <param name="k">The K-line index in the hexagonal coordinate system.</param>
        /// <param name="color">The color of the block.</param>
        /// <returns>A new block positioned according to the given line indices with the specified color.</returns>
        public static Block LineBlock(int i, int k, int color)
        {
            return new Block(Hex.LineHex(i, k), color);
        }
        /// <summary>
        /// Creates a block using hexagonal line indices and assigns it a specific color and state.
        /// The block is shifted accordingly in the coordinate system.
        /// </summary>
        /// <param name="i">The I-line index in the hexagonal coordinate system.</param>
        /// <param name="k">The K-line index in the hexagonal coordinate system.</param>
        /// <param name="color">The color of the block.</param>
        /// <param name="state">The state of the block.</param>
        /// <returns>A new block positioned according to the given line indices with the specified color and state.</returns>
        public static Block LineBlock(int i, int k, int color, bool state)
        {
            return new Block(Hex.LineHex(i, k), color, state);
        }
        /// <summary>
        /// Color of the block
        /// </summary>
        /// <returns>The color of the block.</returns>
        public int Color()
        {
            return color;
        }
        /// <summary>
        /// The state of the block, namely whether it is occupied
        /// </summary>
        /// <returns>The state of the block (occupied = true).</returns>
        public bool State()
        {
            return state;
        }
        /// <summary>
        /// String representation of the block used for debugging. This use line coordinates.
        /// Format: <code>Block[color = c, coordinates = {i, j, k}, State = state]</code>
        /// </summary>
        /// <returns>A string representation of the block, including color, coordinates, and state.</returns>
        public override string ToString()
        {
            return $"Block[color = {color}, coordinates = {{{LineI}, {LineJ}, {LineK}}}, state = {state}]";
        }
        /// <summary>
        /// String representation of the block used for debugging with less information. This use line coordinates.
        /// Format: <code>{i, j, k, state}</code>
        /// </summary>
        /// <returns>A string representation of the block, including only coordinates and state.</returns>
        public string ToBasicString()
        {
            return $"{{{LineI}, {LineJ}, {LineK}, {state}}}";
        }
        /// <summary>
        /// <inheritdoc/>
        /// In addition, it also copies the state and color of this <see cref="Block"/>.
        /// </summary>
        /// <returns>A clone of this block.</returns>
        public override Block Clone()
        {
            return new Block(this.I, this.K, this.color, this.state);
        }
        /// <summary>
        /// Sets the color index of the block.
        /// </summary>
        /// <param name="color">The new color index of the block.</param>
        public void SetColor(int color)
        {
            this.color = color;
        }
        /// <summary>
        /// Sets the state of the block.
        /// </summary>
        /// <param name="state">The new state of the block (true = occupied).</param>
        public void SetState(bool state)
        {
            this.state = state;
        }
        /// <summary>
        /// Toggles the state of the block.
        /// </summary>
        public void ChangeState()
        {
            this.state = !this.state;
        }
        /// <summary>
        /// Creates a new block with its coordinate shifted along the I-axis.
        /// </summary>
        /// <param name="unit"><inheritdoc/></param>
        /// <returns>A new Block with its <see cref="Hex"/> coordinate shifted along the I-axis.</returns>
        public override Block ShiftI(int unit)
        {
            return new Block(base.ShiftI(unit), this.color, this.state);
        }
        /// <summary>
        /// Creates a new block with its coordinate shifted along the J-axis.
        /// </summary>
        /// <param name="unit"><inheritdoc/></param>
        /// <returns>A new Block with its <see cref="Hex"/> coordinate shifted along the J-axis.</returns>
        public override Block ShiftJ(int unit)
        {
            return new Block(base.ShiftJ(unit), this.color, this.state);
        }
        /// <summary>
        /// Creates a new block with its coordinate shifted along the K-axis.
        /// </summary>
        /// <param name="unit"><inheritdoc/></param>
        /// <returns>A new Block with its <see cref="Hex"/> coordinate shifted along the K-axis.</returns>
        public override Block ShiftK(int unit)
        {
            return new Block(base.ShiftK(unit), this.color, this.state);
        }
        /// <summary>
        /// Adds another hex to this block's coordinate and returns a new block with the new coordinate.
        /// </summary>
        /// <param name="other"><inheritdoc/></param>
        /// <returns>A new Block with the summed coordinates.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public override Block Add(Hex other)
        {
            System.ArgumentNullException.ThrowIfNull(other);
            return new Block(base.Add(other), this.color, this.state);
        }
        /// <summary>
        /// Subtracts another hex from this block's coordinate and returns a new block with the new coordinate.
        /// </summary>
        /// <param name="other"><inheritdoc/></param>
        /// <returns>A new Block with the subtracted coordinates.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public override Block Subtract(Hex other)
        {
            System.ArgumentNullException.ThrowIfNull(other);
            return new Block(base.Add(other), this.color, this.state);
        }
    }
}
