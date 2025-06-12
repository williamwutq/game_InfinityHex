
namespace Hex
{
    public class CoordinateManager
    {
        private int moveLimit;
        private int spaceLimit;
        private Hex origin;
        private int moves;

        private HandleCoordinateReset? coordinateResetHandler;
        public delegate void HandleCoordinateReset(Hex oldOrigin);
        public void SetCoordinateResetHandler(HandleCoordinateReset coordinateResetHandler)
        {
            this.coordinateResetHandler = coordinateResetHandler;
        }

        public CoordinateManager(
            int moveLimit = 40,
            int spaceLimit = 16
        )
        {
            this.moveLimit = moveLimit;
            this.spaceLimit = spaceLimit;
            this.origin = new Hex();
            this.moves = 0;
        }

        public void Move(Hex offset)
        {
            moves++;
            Hex offsetOrigin = origin.Add(offset);
            if (moves >= moveLimit || !offsetOrigin.InRange(spaceLimit))
            {
                origin = new Hex();
                moves = 0;
                coordinateResetHandler?.Invoke(offsetOrigin);
            }
            else
            {
                origin = offsetOrigin;
            }
        }

        public void Reset()
        {
            origin = new Hex();
            moves = 0;
        }

        public Hex GetOrigin()
        {
            return this.origin.HexClone();
        }

        public Hex ToAbsolute(Hex relativeCoordinate)
        {
            ArgumentNullException.ThrowIfNull(relativeCoordinate);
            return relativeCoordinate.Add(this.origin);
        }
        public Hex ToRelative(Hex absoluteCoordinate)
        {
            ArgumentNullException.ThrowIfNull(absoluteCoordinate);
            return absoluteCoordinate.Subtract(this.origin);
        }
        public Hex[] ToAbsolute(Hex[] relativeCoordinates)
        {
            return Array.ConvertAll(relativeCoordinates, coo => ToAbsolute(coo));
        }
        public Hex[] ToRelative(Hex[] absoluteCoordinates)
        {
            return Array.ConvertAll(absoluteCoordinates, coo => ToRelative(coo));
        }
    }

    public class WindowManager
    {
        private readonly int windowSize;
        private Block[] blockGrid;

        public delegate void HandleFetchBlock(Hex[] coordinates);
        private HandleFetchBlock? fetchBlockHandler;
        public void SetFetchBlockHandler(HandleFetchBlock fetchBlockHandler)
        {
            this.fetchBlockHandler = fetchBlockHandler;
        }

        public WindowManager(int windowSize)
        {
            this.windowSize = windowSize;
            blockGrid = new Block[1 + 3 * (windowSize) * (windowSize - 1)];
            Reset();
        }

        public void Reset()
        {
            blockGrid = new Block[1 + 3 * (windowSize) * (windowSize - 1)];
            // Add into array to generate the grid
            int i = 0;
            for (int a = 1 - windowSize; a < windowSize; a++)
            {
                for (int b = 1 - windowSize; b < windowSize; b++)
                {
                    Block nb = new Block(new Hex());
                    nb.MoveI(b);
                    nb.MoveK(a);
                    if (nb.InRange(windowSize))
                    {
                        blockGrid[i] = nb;
                        i++;
                    }
                }
            }
            // Already sorted by first I then K

            // fetch request
            fetchBlockHandler?.Invoke(Array.ConvertAll(blockGrid, block => block.HexClone()));
        }

        public Block[] GetBlocks()
        {
            // This method is not safe. Please do not modify the returned array of Blocks.
            return this.blockGrid;
        }
    }

    public class HexEngine
    {
        private List<Block> cache;
        private CoordinateManager coordinateManager;
        private WindowManager windowManager;
        public HexEngine()
        {
            coordinateManager = new CoordinateManager();
            coordinateManager.SetCoordinateResetHandler(OnCoordinateReset);
            windowManager = new WindowManager(7);
            cache = new List<Block>();
        }
        public void Move(Hex offset)
        {
            if (offset.InRange(2))
            {
                // Save original origin
                Hex originalOrigin = coordinateManager.GetOrigin();
                // Request coordinate move to the opposite direction
                coordinateManager.Move(HexLib.Origin.Subtract(offset));
                // Check head
                Block head = SafeGetBlock(originalOrigin);
                if (head.State())
                {
                    // If head is occupied, check the type of occupation
                    if (head.Color() == -2)
                    {
                        // If occupied by snake, cause snake death
                        // Currently, only reset everything
                        resetEngine();
                    }
                    else
                    {
                        // Eat food, increment snake length
                        head.SetState(true);
                        head.SetColor(-2); // -2 (default occupied color) refering to snake
                    }
                }
                else
                {
                    // If not, this block is now snake head, and do not increment snake length
                    head.SetState(true);
                    head.SetColor(-2); // -2 (default occupied color) refering to snake
                }
            }
            else throw new ArgumentOutOfRangeException("Move offset exceed 7-Block grid definition range");
        }
        public void OnCoordinateReset(Hex offset)
        {
            cache.ForEach(block => block.Subtract(offset));
        }
        public void OnFetchRequested(Hex[] coordinates)
        {
            // Cast to relative
            coordinates = coordinateManager.ToRelative(coordinates);

        }

        public Block SafeGetBlock(Hex coordinate)
        {
            ArgumentNullException.ThrowIfNull(coordinate);
            try
            {
                return CacheSearch(coordinate);
            }
            catch (InvalidOperationException)
            {
                // If block not found in cache, generate a new block immediately
                OnGenerationRequested([coordinate]);
                // Now return searched block
                return CacheSearch(coordinate);
            }
        }
        private Block CacheSearch(Hex coordinate)
        {
            List<Block> validList = (List<Block>)cache.Where(block => block.HexClone().Equals(coordinate));
            int count = validList.Count;
            if (count == 0)
            {
                throw new InvalidOperationException("Internal Error as cache search have no results");
            }
            else if (count == 1)
            {
                return validList[0];
            }
            else
            {
                Block first = validList[0];
                foreach (var block in validList.Skip(1))
                {
                    cache.Remove(block);
                }
                return first;
            }
        }
        private void resetEngine()
        {
            cache = new List<Block>();
            coordinateManager.Reset();
            windowManager.Reset();
        }
        private void OnGenerationRequested(Hex[] coordinates)
        {

        }
    }

    /// <summary>
    /// Generates <see cref="Block"/> instances with configurable frequency and color range,
    /// supporting both single and batch block generation for a hexagonal grid.
    /// </summary>
    public class BlockGenerator
    {
        private int frequency;
        private int colorRange;
        private Random colorGenerator;
        private Random stateGenerator;
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
        public BlockGenerator(
            int frequency = 9,
            int colorRange = 12
        )
        {
            if (frequency < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(frequency), "Frequency must be at least 1");
            }
            else if (frequency == 1)
            {
                throw new ArgumentOutOfRangeException(nameof(frequency), "Frequency of 1 is not allowed, as it will always generate occupied blocks");
            }
            if (colorRange < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(colorRange), "Color range must be at least 1");
            }
            this.frequency = frequency;
            this.colorRange = colorRange;
            colorGenerator = new Random();
            stateGenerator = new Random();
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
            ArgumentNullException.ThrowIfNull(coordinate);
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
            ArgumentNullException.ThrowIfNull(coordinates);
            // Generate unoccupied blocks as default
            Block[] blocks = Array.ConvertAll(coordinates, coo => coo == null ? throw new ArgumentException("Array contains null coordinates") : new Block(coo));
            // Randomly decide which blocks to be occupied
            int length = coordinates.Length;
            if (length < frequency)
            {
                // Simplify operation: do not occupy any blocks
                return blocks;
            }
            int expectedCount = length / frequency;
            // Generate indexes of blocks to be occupied
            HashSet<int> occupiedIndexes = new HashSet<int>();
            while (occupiedIndexes.Count < expectedCount)
            {
                int index = stateGenerator.Next(0, length);
                occupiedIndexes.Add(index);
            }
            // Set occupied blocks
            foreach (int index in occupiedIndexes)
            {
                // Generate an occupied block with random color
                int color = colorGenerator.Next(0, colorRange);
                blocks[index] = new Block(coordinates[index], color, true);
            }
            // Return
            return blocks;
        }
    }

}
