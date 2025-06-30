namespace Hex
{
    /// <summary>
    /// Generates <see cref="Block"/> instances with configurable frequency and color range,
    /// supporting both single and batch block generation for a hexagonal grid.
    /// </summary>
    public class BlockGenerator
    {
        private readonly int frequency;
        private readonly int colorRange;
        private readonly System.Random colorGenerator;
        private readonly System.Random stateGenerator;
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockGenerator"/> class.
        /// </summary>
        /// <param name="frequency">
        /// The frequency at which occupied blocks are generated. Must be greater than 1.
        /// A lower frequency increases the chance of generating occupied blocks.
        /// </param>
        /// <param name="colorRange">
        /// The number of possible colors for occupied blocks. Must be at least 1.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="frequency"/> is less than 1 or equal to 1,
        /// or if <paramref name="colorRange"/> is less than 1.
        /// </exception>
        public BlockGenerator(int frequency, int colorRange)
        {
            if (frequency < 1)
            {
                throw new System.ArgumentOutOfRangeException(nameof(frequency), "Frequency must be at least 1");
            }
            else if (frequency == 1)
            {
                throw new System.ArgumentOutOfRangeException(nameof(frequency), "Frequency of 1 is not allowed, as it will always generate occupied blocks");
            }
            if (colorRange < 1)
            {
                throw new System.ArgumentOutOfRangeException(nameof(colorRange), "Color range must be at least 1");
            }
            this.frequency = frequency;
            this.colorRange = colorRange;
            System.Random superGenerator = new System.Random();
            colorGenerator = new System.Random(superGenerator.Next());
            stateGenerator = new System.Random(superGenerator.Next());
        }
        /// <summary>
        /// Generates a single <see cref="Block"/> at the specified <see cref="Hex"/> coordinate.
        /// The block may be occupied or unoccupied, determined randomly based on the frequency.
        /// If occupied, a random color within the specified color range is assigned.
        /// </summary>
        /// <param name="coordinate">The <see cref="Hex"/> coordinate for the block.</param>
        /// <returns>
        /// A new <see cref="Block"/> instance at the given coordinate, either occupied with a random color or unoccupied.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="coordinate"/> is <c>null</c>.
        /// </exception>
        public Block GenerateBlock(Hex coordinate)
        {
            System.ArgumentNullException.ThrowIfNull(coordinate);
            if (stateGenerator.Next(0, frequency) == 0)
            {
                // Generate an occupied block with random color
                int color = colorGenerator.Next(0, colorRange);
                return new Block(coordinate, color, true);
            }
            else
            {
                return new Block(coordinate);
            }
        }
        /// <summary>
        /// Generates an array of <see cref="Block"/> instances for the specified <see cref="Hex"/> coordinates.
        /// A subset of the blocks will be randomly set as occupied, each with a random color,
        /// based on the configured frequency. The rest will be unoccupied.
        /// </summary>
        /// <param name="coordinates">An array of <see cref="Hex"/> coordinates to generate blocks for.</param>
        /// <returns>
        /// An array of <see cref="Block"/> instances corresponding to the input coordinates,
        /// with some blocks randomly set as occupied.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="coordinates"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if any element in <paramref name="coordinates"/> is <c>null</c>.
        /// </exception>
        public Block[] GenerateBlocks(Hex[] coordinates)
        {
            System.ArgumentNullException.ThrowIfNull(coordinates);
            // Generate unoccupied blocks as default
            Block[] blocks = System.Array.ConvertAll(coordinates, coo => coo == null ? throw new System.ArgumentException("Array contains null coordinates") : new Block(coo));
            // Randomly decide which blocks to be occupied
            int length = coordinates.Length;
            if (length < frequency)
            {
                // Simplify operation: do not occupy any blocks
                return blocks;
            }
            int expectedCount = length / frequency;
            // Create an array of indices
            int[] indices = new int[length];
            for (int i = 0; i < length; i++)
            {
                indices[i] = i;
            }
            // Perform partial Fisher-Yates shuffle to select unique indices
            for (int i = 0; i < expectedCount; i++)
            {
                int j = stateGenerator.Next(i, length); // pick from [i..length-1]
                (indices[i], indices[j]) = (indices[j], indices[i]); // swap
            }
            // Use the first `expectedCount` indices as occupied blocks
            for (int i = 0; i < expectedCount; i++)
            {
                int index = indices[i];
                int color = colorGenerator.Next(0, colorRange);
                blocks[index] = new Block(coordinates[index], color, true);
            }
            // Return
            return blocks;
        }
    }
}