
namespace Hex
{
    /// <summary>
    /// A utility class to <see cref="Hex"/> for working with hexagonal grid coordinates in a standard 7-Block grid.
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
            if (index == 0) {
                return JMinus;
            } else if (index == 1) {
                return KMinus;
            } else if (index == 2) {
                return IMinus;
            } else if (index == 3) {
                return Origin;
            } else if (index == 4) {
                return IPlus;
            } else if (index == 5) {
                return KPlus;
            } else if (index == 6) {
                return JPlus;
            } else throw new System.IndexOutOfRangeException($"Index {index} out bounds for range 7");
        }
        /// <summary>
        /// Gets an array of all hexes in the standard 7-Block grid in the defined sequence.
        /// The standard 7-Block grid is defined as a sequence of hexes at the following line coordinates {I, J, K}:
        /// (-1, 0, -1), (-1, 1, 0), (0, -1, -1), (0, 0, 0), (0, 1, 1), (1, -1, 0), (1, 0, 1).
        /// This represents a grid of radius 2 centered at the origin (0, 0, 0).
        /// </summary>
        public static Hex[] SevenBlockArr => [JMinus, KMinus, IMinus, Origin, IPlus, KPlus, JPlus];
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
            if (sevenBlockHex.Equals(Origin)) {
                return Origin;
            } else if (sevenBlockHex.Equals(IMinus)) {
                return IPlus;
            } else if (sevenBlockHex.Equals(IPlus)) {
                return IMinus;
            } else if (sevenBlockHex.Equals(JMinus)) {
                return JPlus;
            } else if (sevenBlockHex.Equals(JPlus)) {
                return JMinus;
            } else if (sevenBlockHex.Equals(KMinus)) {
                return KPlus;
            } else if (sevenBlockHex.Equals(KPlus)) {
                return KMinus;
            } else throw new System.ArgumentOutOfRangeException($"Hex coordinate {sevenBlockHex} exceed 7-Block grid definition range");
        }
    }
}
