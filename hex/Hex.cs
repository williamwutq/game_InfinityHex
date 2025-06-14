namespace Hex
{
    /// <summary>
    /// From Java implementation of Hex from the HappyHex project.
    /// <para>
    /// Represents a 2D coordinate in a hexagonal grid system using a specialized integer coordinate model.
    /// Supports both raw coordinate access and derived line-based computations across three axes: I, J, and K.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <h2>Coordinate System</h2>
    /// Designed by William Wu.
    /// </para>
    /// <para>In this system:</para>
    /// <list type="bullet">
    ///   <item>The axes I, J, and K run diagonally through the hexagonal grid.</item>
    ///   <item>I+ is 60 degrees to J+, J+ is 60 degrees to K+, and K+ is 60 degrees to J-.</item>
    ///   <item>Coordinates (i, k) correspond to a basis for representing any hexagon.</item>
    ///   <item>Raw coordinate (or hex coordinate) refers to the distance of a point along one of the axes multiplied by 2.</item>
    ///   <item>For raw coordinates, the relationships between the axes are defined such that <c>i - j + k = 0</c>.</item>
    ///   <item>Line coordinate (or line-distance based coordinate) are based on the distance perpendicular to the axes.</item>
    ///   <item>For line coordinates, the relationships between the axes are defined such that <c>I + J - K = 0</c>.</item>
    ///   <item>All line coordinates correspond to some raw coordinate, but the inverse is not true. Concerning the complexities
    ///   with dealing with raw coordinates, it is preferable to use line coordinates.</item>
    /// </list>
    ///
    /// <para>
    /// <h2>Coordinate System Visualization</h2>
    /// Three example points with raw coordinates:
    /// <code>
    /// hex Coordinates (2i, 2j, 2k)
    ///    I
    ///   / * (5, 4, -1)
    ///  /     * (5, 7, 2)
    /// o - - J
    ///  \ * (0, 3, 3)
    ///   \
    ///    K
    /// </code>
    /// Three example points with line coordinates:
    /// <code>
    /// Line Coordinates (I, J, K)
    ///    I
    ///   / * (1, 2, 3)
    ///  /     * (3, 1, 4)
    /// o - - J
    ///  \ * (2, -1, 1)
    ///   \
    ///    K
    /// </code>
    /// </para>
    ///
    /// <para>
    /// <h2>Coordinate System Implementation</h2>
    /// </para>
    /// <list type="bullet">
    ///   <item><c>x</c> and <c>y</c> are the base values stored in each <see cref="Hex"/> instance.</item>
    ///   <item><c>I = x</c>, <c>K = y</c>, and <c>J = x + y</c>.</item>
    ///   <item>Line indices are derived as follows:
    ///     <list type="bullet">
    ///       <item><see cref="LineI"/> is <c>(2y + x) / 3</c></item>
    ///       <item><see cref="LineJ"/> is <c>(x - y) / 3</c></item>
    ///       <item><see cref="LineK"/> is <c>(2x + y) / 3</c></item>
    ///     </list>
    ///   </item>
    /// </list>
    ///
    /// <para>
    /// <h2>Functionality</h2>
    /// The class provides functionality to:
    /// </para>
    /// <list type="bullet">
    ///   <item>Access and compute raw coordinates: <see cref="I"/>, <see cref="J"/>, <see cref="K"/>.</item>
    ///   <item>Access and compute line-distance based coordinates: <see cref="LineI"/>, <see cref="LineJ"/>, <see cref="LineK"/>.</item>
    ///   <item>Create hex objects through constructors or factory methods: <see cref="Hex()"/>, <see cref="Hex(int, int)"/>, <see cref="LineHex()"/>, <see cref="LineHex(int, int)"/>.</item>
    ///   <item>Move hex object along I, J, or K axes (increment line coordinates): <see cref="MoveI(int)"/>, <see cref="MoveJ(int)"/>, <see cref="MoveK(int)"/>.</item>
    ///   <item>Addition and subtraction of coordinates: <see cref="Add(Hex)"/> and <see cref="Subtract(Hex)"/>.</item>
    ///   <item>Check for line alignment and adjacency between hexes: <see cref="InLineI(Hex)"/>, <see cref="Adjacent(Hex)"/>, etc.</item>
    ///   <item>Determine relative orientation in the grid: <see cref="Front(Hex)"/>, <see cref="Back(Hex)"/>, and axis-specific versions.</item>
    ///   <item>Cloning any instance of its subclasses using <see cref="Clone"/>.</item>
    /// </list>
    ///
    /// <para>
    /// <h2>Usage Notes</h2>
    /// It is recommended to use the factory method <see cref="Create(int, int)"/> instead of direct constructors,
    /// as it provides hexes correctly shifted in line coordinates according to hexagonal grid logic.
    /// </para>
    /// </remarks>
    /// <since>0.1</since>
    /// <author>William Wu</author>
    /// <version>0.1</version>
    public class Hex : System.ICloneable
    {
        private readonly double halfSinOf60 = System.Math.Sqrt(3) / 4;
        private int x;
        private int y;
        /// <summary>
        /// Initializes a new instance of the <see cref="Hex"/> class at the origin (0,0).
        /// </summary>
        public Hex()
        {
            this.x = 0;
            this.y = 0;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Hex"/> class at the specified (i, k) coordinates.
        /// </summary>
        /// <param name="i">The i-coordinate.</param>
        /// <param name="k">The k-coordinate.</param>
        public Hex(int i, int k)
        {
            this.x = i;
            this.y = k;
        }
        /// <summary>
        /// Creates a default hex coordinate at (0,0).
        /// </summary>
        /// <returns>A new <see cref="Hex"/> instance at the origin.</returns>
        public static Hex LineHex()
        {
            return new Hex();
        }
        /// <summary>
        /// Creates a hex coordinate using hexagonal line indices instead of direct coordinates.
        /// The hex is shifted accordingly in the coordinate system.
        /// </summary>
        /// <param name="i">The I-line index in the hexagonal coordinate system.</param>
        /// <param name="k">The K-line index in the hexagonal coordinate system.</param>
        /// <returns>A new <see cref="Hex"/> coordinate positioned according to the given line indices.</returns>
        public static Hex LineHex(int i, int k)
        {
            return new Hex(2 * k - i, 2 * i - k);
        }

        // Raw coordinates

        /// <summary>
        /// Gets the raw I-coordinate.
        /// </summary>
        public int I => x;
        /// <summary>
        /// Gets the raw J-coordinate.
        /// </summary>
        public int J => x + y;
        /// <summary>
        /// Gets the raw K-coordinate.
        /// </summary>
        public int K => y;

        // Lines
        /// <summary>
        /// Gets the line index along the I-axis in the hexagonal coordinate system.
        /// </summary>
        public int LineI => (2 * y + x) / 3;
        /// <summary>
        /// Gets the line index along the J-axis in the hexagonal coordinate system.
        /// </summary>
        public int LineJ => (x - y) / 3;
        /// <summary>
        /// Gets the line index along the K-axis in the hexagonal coordinate system.
        /// </summary>
        public int LineK => (2 * x + y) / 3;

        /// <summary>
        /// Gets a string representation of the line indices of the hex along all axes.
        /// Format: {I = i, J = j, K = k}
        /// </summary>
        public string Lines()
        {
            return $"{{I = {LineI}, J = {LineJ}, K = {LineK}}}";
        }

        // Line booleans
        /// <summary>
        /// Determines whether this hex is in the specified I-line.
        /// </summary>
        /// <param name="line">The I-line value to check.</param>
        /// <returns>True if the hex is in the specified I-line, otherwise false.</returns>
        public bool InLineI(int line)
        {
            return LineI == line;
        }
        /// <summary>
        /// Determines whether this hex is in the specified J-line.
        /// </summary>
        /// <param name="line">The J-line value to check.</param>
        /// <returns>True if the hex is in the specified J-line, otherwise false.</returns>
        public bool InLineJ(int line)
        {
            return LineJ == line;
        }
        /// <summary>
        /// Determines whether this hex is in the specified K-line.
        /// </summary>
        /// <param name="line">The K-line value to check.</param>
        /// <returns>True if the hex is in the specified K-line, otherwise false.</returns>
        public bool InLineK(int line)
        {
            return LineK == line;
        }
        /// <summary>
        /// Determines whether this hex coordinate is in the same I-line as the other hex coordinate.
        /// </summary>
        /// <param name="other">The other hex coordinate to compare with.</param>
        /// <returns>True if this hex coordinate is in the same I-line as the other hex coordinate.</returns>
        public bool InLineI(Hex other)
        {
            return LineI == other.LineI;
        }
        /// <summary>
        /// Determines whether this hex coordinate is in the same J-line as the other hex coordinate.
        /// </summary>
        /// <param name="other">The other hex coordinate to compare with.</param>
        /// <returns>True if this hex coordinate is in the same J-line as the other hex coordinate.</returns>
        public bool InLineJ(Hex other)
        {
            return LineJ == other.LineJ;
        }
        /// <summary>
        /// Determines whether this hex coordinate is in the same K-line as the other hex coordinate.
        /// </summary>
        /// <param name="other">The other hex coordinate to compare with.</param>
        /// <returns>True if this hex coordinate is in the same K-line as the other hex coordinate.</returns>
        public bool InLineK(Hex other)
        {
            return LineK == other.LineK;
        }
        /// <summary>
        /// Determines whether this hex coordinate is adjacent to another hex.
        /// Two hex coordinates are considered adjacent if they share an edge in the hexagonal grid.
        /// </summary>
        /// <param name="other">The other hex to compare with.</param>
        /// <returns>True if the hex coordinates are adjacent, otherwise false.</returns>
        public bool Adjacent(Hex other)
        {
            return Front(other) || Back(other);
        }

        /// <summary>
        /// Determines if this hex coordinate is in front of another hex coordinate.
        /// A hex coordinate is considered "in the front" if it is positioned one step higher in any of the three coordinate axes.
        /// </summary>
        /// <param name="other">The other hex to compare with.</param>
        /// <returns>True if this hex coordinate is one unit higher in I, J, or K, otherwise false.</returns>
        /// <see cref="Adjacent(Hex)"/>
        /// <see cref="FrontI(Hex)"/>
        /// <see cref="FrontJ(Hex)"/>
        /// <see cref="FrontK(Hex)"/>
        public bool Front(Hex other)
        {
            return FrontI(other) || FrontJ(other) || FrontK(other);
        }
        /// <summary>
        /// Determines if this hex coordinate is behind another hex coordinate.
        /// A hex coordinate is considered "behind" if it is positioned one step lower in any of the three coordinate axes.
        /// </summary>
        /// <param name="other">The other hex to compare with.</param>
        /// <returns>True if this hex coordinate is one unit lower in I, J, or K, otherwise false.</returns>
        /// <see cref="Adjacent(Hex)"/>
        /// <see cref="BackI(Hex)"/>
        /// <see cref="BackJ(Hex)"/>
        /// <see cref="BackK(Hex)"/>
        public bool Back(Hex other)
        {
            return BackI(other) || BackJ(other) || BackK(other);
        }
        /// <summary>
        /// Determines if this hex coordinate is in front of another hex coordinate on the I-axis.
        /// </summary>
        /// <param name="other">The other hex to compare with.</param>
        /// <returns>True if this hex coordinate is one unit higher on the I-axis.</returns>
        /// <see cref="Front(Hex)"/>
        public bool FrontI(Hex other)
        {
            return x == other.x + 2 && y == other.y - 1;
        }
        /// <summary>
        /// Determines if this hex coordinate is in front of another hex coordinate on the J-axis.
        /// </summary>
        /// <param name="other">The other hex to compare with.</param>
        /// <returns>True if this hex coordinate is one unit higher on the J-axis.</returns>
        /// <see cref="Front(Hex)"/>
        public bool FrontJ(Hex other)
        {
            return x == other.x + 1 && y == other.y + 1;
        }
        /// <summary>
        /// Determines if this hex coordinate is in front of another hex coordinate on the K-axis.
        /// </summary>
        /// <param name="other">The other hex to compare with.</param>
        /// <returns>True if this hex coordinate is one unit higher on the K-axis.</returns>
        /// <see cref="Front(Hex)"/>
        public bool FrontK(Hex other)
        {
            return x == other.x - 1 && y == other.y + 2;
        }
        /// <summary>
        /// Determines if this hex coordinate is behind another hex coordinate on the I-axis.
        /// </summary>
        /// <param name="other">The other hex to compare with.</param>
        /// <returns>True if this hex coordinate is one unit lower on the I-axis.</returns>
        /// <see cref="Back(Hex)"/>
        public bool BackI(Hex other)
        {
            return x == other.x - 2 && y == other.y + 1;
        }
        /// <summary>
        /// Determines if this hex coordinate is behind another hex coordinate on the J-axis.
        /// </summary>
        /// <param name="other">The other hex to compare with.</param>
        /// <returns>True if this hex coordinate is one unit lower on the J-axis.</returns>
        /// <see cref="Back(Hex)"/>
        public bool BackJ(Hex other)
        {
            return x == other.x - 1 && y == other.y - 1;
        }
        /// <summary>
        /// Determines if this hex coordinate is behind another hex coordinate on the K-axis.
        /// </summary>
        /// <param name="other">The other hex to compare with.</param>
        /// <returns>True if this hex coordinate is one unit lower on the K-axis.</returns>
        /// <see cref="Back(Hex)"/>
        public bool BackK(Hex other)
        {
            return x == other.x + 1 && y == other.y - 2;
        }

        /// <summary>
        /// Checks if this hex is equal to another hex.
        /// Two hex coordinates are considered equal if they have the same coordinates in all axes.
        /// </summary>
        /// <param name="other">The other hex to compare.</param>
        /// <returns>True if both hex coordinates have the same coordinates, otherwise false.</returns>
        public bool Equals(Hex other)
        {
            if (other == null) return false;
            return x == other.x && y == other.y;
        }
        /// <summary>
        /// Checks if this hex coordinate is within a given radius from the centered origin, as specified in the grid.
        /// The radius is determined using the hexagonal distance metric.
        /// </summary>
        /// <param name="radius">The radius to check.</param>
        /// <returns>True if the hex is within the radius, otherwise false.</returns>
        public bool InRange(int radius)
        {
            return -radius < LineI && LineI < radius &&
                   -radius < LineJ && LineJ < radius &&
                   -radius < LineK && LineK < radius;
        }

        // convert to rectangular
        /// <summary>
        /// Converts the hexagonal coordinates to a rectangular X coordinate.
        /// This transformation is based on the hexagonal grid layout, where the X-coordinate
        /// is computed using the sine of 60 degrees to account for the hexagonal tiling pattern.
        /// </summary>
        /// <returns>
        /// The X-coordinate in rectangular space.
        /// </returns>
        public double X => halfSinOf60 * (x + y);
        /// Converts the hexagonal coordinates to a rectangular Y coordinate.
        /// This transformation is based on the hexagonal grid layout, where the T-coordinate
        /// is computed using the sine of 30 degrees to account for the hexagonal tiling pattern.
        /// </summary>
        /// <returns>
        /// The Y-coordinate in rectangular space.
        /// </returns>
        public double Y => (x - y) / 4.0;
        /// <summary>
        /// Gets a string representation of the line indices of the hex along all axes.
        /// Format: {I = i, J = j, K = k}
        /// </summary>
        public override string ToString()
        {
            return $"Hex[raw = {{{I}, {J}, {K}}}, line = {{{LineI}, {LineJ}, {LineK}}}, rect = {{{X}, {Y}}}]";
        }

        /// <summary>
        /// Moves the hex coordinate along the I-axis.
        /// </summary>
        /// <param name="unit">The number of units to move.</param>
        public void MoveI(int unit)
        {
            x += 2 * unit;
            y -= unit;
        }
        /// <summary>
        /// Moves the hex coordinate along the J-axis.
        /// </summary>
        /// <param name="unit">The number of units to move.</param>
        public void MoveJ(int unit)
        {
            x += unit;
            y += unit;
        }
        /// <summary>
        /// Moves the hex coordinate along the K-axis.
        /// </summary>
        /// <param name="unit">The number of units to move.</param>
        public void MoveK(int unit)
        {
            x -= unit;
            y += 2 * unit;
        }
        /// <summary>
        /// Creates a new hex coordinate shifted along the I-axis.
        /// </summary>
        /// <param name="unit">The number of units to shift.</param>
        /// <returns>A new hex coordinate shifted along the I-axis.</returns>
        public virtual Hex ShiftI(int unit)
        {
            return new Hex(x + 2 * unit, y - unit);
        }
        /// <summary>
        /// Creates a new hex coordinate shifted along the J-axis.
        /// </summary>
        /// <param name="unit">The number of units to shift.</param>
        /// <returns>A new hex coordinate shifted along the J-axis.</returns>
        public virtual Hex ShiftJ(int unit)
        {
            return new Hex(x + unit, y + unit);
        }
        /// <summary>
        /// Creates a new hex coordinate shifted along the K-axis.
        /// </summary>
        /// <param name="unit">The number of units to shift.</param>
        /// <returns>A new hex coordinate shifted along the K-axis.</returns>
        public virtual Hex ShiftK(int unit)
        {
            return new Hex(x - unit, y + 2 * unit);
        }
        /// <summary>
        /// Adds another hex to this hex coordinate and returns a new hex coordinate.
        /// </summary>
        /// <param name="other">The hex coordinate to add.</param>
        /// <returns>A new hex coordinate with the summed coordinates.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public virtual Hex Add(Hex other)
        {
            System.ArgumentNullException.ThrowIfNull(other);
            return new Hex(x + other.x, y + other.y);
        }
        /// <summary>
        /// Subtracts another hex coordinate from this hex coordinate and returns a new hex coordinate.
        /// </summary>
        /// <param name="other">The hex coordinate to subtract.</param>
        /// <returns>A new hex coordinate with the subtracted coordinates.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public virtual Hex Subtract(Hex other)
        {
            System.ArgumentNullException.ThrowIfNull(other);
            return new Hex(x - other.x, y - other.y);
        }
        /// <summary>
        /// Sets this hex coordinate to match another hex coordinate.
        /// </summary>
        /// <param name="other">The target hex coordinate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="other"/> is null.</exception>
        public void Set(Hex other)
        {
            System.ArgumentNullException.ThrowIfNull(other);
            x = other.x;
            y = other.y;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Hex"/> object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj != null && obj is Hex other)
            {
                return Equals(other);
            }
            return false;
        }
        /// <summary>
        /// Returns a hash code for this <see cref="Hex"/> object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return System.HashCode.Combine(x, y);
        }
        /// <summary>
        /// Create a clone of this <see cref="Hex"/> object, with its hexagonal coordinates copied.
        /// If instance is a subclass of <see cref="Hex"/>, should use the clone method of that class.
        /// </summary>
        /// <returns>A clone of the hex object.</returns>
        public virtual object Clone()
        {
            return HexClone();
        }
        /// <summary>
        /// This hex coordinate. Strictly a Hex object.
        /// </summary>
        /// <returns>A clone of this hex object.</returns>
        public Hex HexClone()
        {
            return new Hex(x, y);
        }
    }
}
