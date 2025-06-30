namespace Hex
{
    /// <summary>
    /// A utility class to <see cref="Hex"/> for working with hexagonal grid coordinates in a standard 7-Block grid or a circular 6-Block grid.
    /// The grid uses a line coordinate system {I, J, K} with a radius of 2 centered at the origin (0, 0, 0).
    /// </summary>
    public static class HexLib
    {
        /// <summary>
        /// Gets the origin hex at coordinates (0, 0, 0), the zero coordinate and the center of the 7-Block grid.
        /// </summary>
        public static Hex Origin => Hex.LineHex();
        /// <summary>
        /// Gets the hex at coordinates (1, -1, 0) in the 7-Block grid.
        /// </summary>
        public static Hex KPlus => Hex.LineHex(1, 0);
        /// <summary>
        /// Gets the hex at coordinates (1, 0, 1) in the 7-Block grid.
        /// </summary>
        public static Hex JPlus => Hex.LineHex(1, 1);
        /// <summary>
        /// Gets the hex at coordinates (0, 1, 1) in the 7-Block grid.
        /// </summary>
        public static Hex IPlus => Hex.LineHex(0, 1);
        /// <summary>
        /// Gets the hex at coordinates (-1, 1, 0) in the 7-Block grid.
        /// </summary>
        public static Hex KMinus => Hex.LineHex(-1, 0);
        /// <summary>
        /// Gets the hex at coordinates (-1, 0, -1) in the 7-Block grid.
        /// </summary>
        public static Hex JMinus => Hex.LineHex(-1, -1);
        /// <summary>
        /// Gets the hex at coordinates (0, -1, -1) in the 7-Block grid.
        /// </summary>
        public static Hex IMinus => Hex.LineHex(0, -1);
        /// <summary>
        /// Retrieves a hex from the standard 7-Block grid by index.
        /// The standard 7-Block grid is defined as a sequence of hexes at the following line coordinates {I, J, K}:
        /// (-1, 0, -1), (-1, 1, 0), (0, -1, -1), (0, 0, 0), (0, 1, 1), (1, -1, 0), (1, 0, 1).
        /// This represents a grid of radius 2 centered at the origin (0, 0, 0).
        /// </summary>
        /// <param name="index">The index of the hex in the 7-Block grid (0 to 6).</param>
        /// <returns>The hex at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the index is outside the range [0, 6].</exception>
        public static Hex SevenBlock(int index)
        {
            return index switch
            {
                0 => JMinus,
                1 => KMinus,
                2 => IMinus,
                3 => Origin,
                4 => IPlus,
                5 => KPlus,
                6 => JPlus,
                _ => throw new System.IndexOutOfRangeException($"Index {index} out bounds for range 7")
            };
        }
        /// <summary>
        /// Retrieves index of a standard 7-Block grid hex coordinate.
        /// The standard 7-Block grid is defined as a sequence of hexes at the following line coordinates {I, J, K}:
        /// (-1, 0, -1), (-1, 1, 0), (0, -1, -1), (0, 0, 0), (0, 1, 1), (1, -1, 0), (1, 0, 1).
        /// This represents a grid of radius 2 centered at the origin (0, 0, 0).
        /// </summary>
        /// <param name="sevenBlockHex">The hex coordinate in the 7-Block grid.</param>
        /// <returns>The index representing the hex coordinate (0-6).</returns>
        /// <exception cref="ArgumentNullException">Thrown when the coordinate is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the coordinate is outside of the standard 7-Block grid.</exception>
        public static int SevenBlockIndex(Hex sevenBlockHex)
        {
            System.ArgumentNullException.ThrowIfNull(sevenBlockHex);
            return (sevenBlockHex) switch
            {
                var hex when hex.Equals(Origin) => 0,
                var hex when hex.Equals(IMinus) => 1,
                var hex when hex.Equals(IPlus)  => 2,
                var hex when hex.Equals(JMinus) => 3,
                var hex when hex.Equals(JPlus)  => 4,
                var hex when hex.Equals(KMinus) => 5,
                var hex when hex.Equals(KPlus)  => 6,
                _ => throw new System.ArgumentOutOfRangeException($"Hex coordinate {sevenBlockHex} exceed 7-Block grid definition range")
            };
        }
        /// <summary>
        /// Retrieves a hex from the circular 6-Block grid by index.
        /// The circular 6-Block grid is defined as a sequence of hexes at the following line coordinates {I, J, K}:
        /// (0, 1, 1), (1, 0, 1), (1, -1, 0), (0, -1, -1), (-1, 0, -1), (-1, 1, 0).
        /// This represents a grid of radius 2 centered at the origin (0, 0, 0), but without the origin.
        /// </summary>
        /// <param name="index">The index of the hex in the 6-Block grid (0 to 5).</param>
        /// <returns>The hex at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the index is outside the range [0, 5].</exception>
        public static Hex CircularSixBlock(int index)
        {
            return index switch
            {
                0 => IPlus,
                1 => JPlus,
                2 => KPlus,
                3 => IMinus,
                4 => JMinus,
                5 => KMinus,
                _ => throw new System.IndexOutOfRangeException($"Index {index} out bounds for range 6")
            };
        }
        /// <summary>
        /// Retrieves index of a circular 6-Block grid hex coordinate.
        /// The circular 6-Block grid is defined as a sequence of hexes at the following line coordinates {I, J, K}:
        /// (0, 1, 1), (1, 0, 1), (1, -1, 0), (0, -1, -1), (-1, 0, -1), (-1, 1, 0).
        /// This represents a grid of radius 2 centered at the origin (0, 0, 0), but without the origin.
        /// </summary>
        /// <param name="sevenBlockHex">The hex coordinate in the circular 6-Block grid.</param>
        /// <returns>The index representing the hex coordinate (0-5).</returns>
        /// <exception cref="ArgumentNullException">Thrown when the coordinate is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the coordinate is outside of the circular 6-Block grid.</exception>
        public static int CircularSixBlockIndex(Hex circularSixBlockHex)
        {
            System.ArgumentNullException.ThrowIfNull(circularSixBlockHex);
            return (circularSixBlockHex) switch
            {
                var hex when hex.Equals(IPlus)  => 0,
                var hex when hex.Equals(JPlus)  => 1,
                var hex when hex.Equals(KPlus)  => 2,
                var hex when hex.Equals(IMinus) => 3,
                var hex when hex.Equals(JMinus) => 4,
                var hex when hex.Equals(KMinus) => 5,
                _ => throw new System.ArgumentOutOfRangeException($"Hex coordinate {circularSixBlockHex} exceed circular 6-Block grid definition range")
            };
        }
        /// <summary>
        /// Gets an array of all hexes in the standard 7-Block grid in the defined sequence.
        /// The standard 7-Block grid is defined as a sequence of hexes at the following line coordinates {I, J, K}:
        /// (-1, 0, -1), (-1, 1, 0), (0, -1, -1), (0, 0, 0), (0, 1, 1), (1, -1, 0), (1, 0, 1).
        /// This represents a grid of radius 2 centered at the origin (0, 0, 0).
        /// </summary>
        public static Hex[] SevenBlockArr => [JMinus, KMinus, IMinus, Origin, IPlus, KPlus, JPlus];
        /// <summary>
        /// Gets an array of all hexes in the circular 6-Block grid in the defined sequence.
        /// The circular 6-Block grid is defined as a sequence of hexes at the following line coordinates {I, J, K}:
        /// (0, 1, 1), (1, 0, 1), (1, -1, 0), (0, -1, -1), (-1, 0, -1), (-1, 1, 0).
        /// This represents a grid of radius 2 centered at the origin (0, 0, 0), but without the origin.
        /// </summary>
        public static Hex[] CircularSixBlockArr => [IPlus, JPlus, KPlus, IMinus, JMinus, KMinus];
        /// <summary>
        /// Negate a Hex coordinate to its opposite peer. This method is perfered compare to new Hex().Subtract(hex).
        /// This method only works for coordinates in the standard 7-Block grid, otherwise an <see cref="ArgumentOutOfRangeException"/> 
        /// will be thrown.
        /// The standard 7-Block grid is defined as a sequence of hexes at the following line coordinates {I, J, K}:
        /// (-1, 0, -1), (-1, 1, 0), (0, -1, -1), (0, 0, 0), (0, 1, 1), (1, -1, 0), (1, 0, 1).
        /// This represents a grid of radius 2 centered at the origin (0, 0, 0).
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the coordinate is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the coordinate is outside of the standard 7-Block grid.</exception>
        public static Hex Negate(Hex sevenBlockHex)
        {
            System.ArgumentNullException.ThrowIfNull(sevenBlockHex);
            return sevenBlockHex switch
            {
                var hex when hex.Equals(Origin)   => Origin,
                var hex when hex.Equals(IMinus)   => IPlus,
                var hex when hex.Equals(IPlus)    => IMinus,
                var hex when hex.Equals(JMinus)   => JPlus,
                var hex when hex.Equals(JPlus)    => JMinus,
                var hex when hex.Equals(KMinus)   => KPlus,
                var hex when hex.Equals(KPlus)    => KMinus,
                _ => throw new System.ArgumentOutOfRangeException($"Hex coordinate {sevenBlockHex} exceed 7-Block grid definition range")
            };
        }
        /// <summary>
        /// Rotate a Hex coordinate to its adjacent peer. This method is perfered compare to manual implementations or double index casting.
        /// This method only works for coordinates in the circular 6-Block grid, otherwise an <see cref="ArgumentOutOfRangeException"/> 
        /// will be thrown.
        /// The circular 6-Block grid is defined as a sequence of hexes at the following line coordinates {I, J, K}:
        /// (0, 1, 1), (1, 0, 1), (1, -1, 0), (0, -1, -1), (-1, 0, -1), (-1, 1, 0).
        /// This represents a grid of radius 2 centered at the origin (0, 0, 0), but without the origin.
        /// </summary>
        /// <param name="circularSixBlockHex">The input hex coordinate to undergo direction change.</param>
        /// <param name="direction">The direction of the rotation, true for clockwise (index increment), false for counter-clockwise (index decrement).</param>
        /// <exception cref="ArgumentNullException">Thrown when the coordinate is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the coordinate is outside of the circular 6-Block grid.</exception>
        public static Hex Rotate(Hex circularSixBlockHex, bool direction)
        {
            System.ArgumentNullException.ThrowIfNull(circularSixBlockHex);
            return (circularSixBlockHex, direction) switch
            {
                (var hex, true) when hex.Equals(IPlus)    => JPlus,
                (var hex, false) when hex.Equals(IPlus)   => KMinus,
                (var hex, true) when hex.Equals(JPlus)    => KPlus,
                (var hex, false) when hex.Equals(JPlus)   => IPlus,
                (var hex, true) when hex.Equals(KPlus)    => IMinus,
                (var hex, false) when hex.Equals(KPlus)   => JPlus,
                (var hex, true) when hex.Equals(IMinus)   => JMinus,
                (var hex, false) when hex.Equals(IMinus)  => KPlus,
                (var hex, true) when hex.Equals(JMinus)   => KMinus,
                (var hex, false) when hex.Equals(JMinus)  => IMinus,
                (var hex, true) when hex.Equals(KMinus)   => IPlus,
                (var hex, false) when hex.Equals(KMinus)  => JMinus,
                _ => throw new System.ArgumentOutOfRangeException($"Hex coordinate {circularSixBlockHex} exceed circular 6-Block grid definition range")
            };
        }
    }
}
