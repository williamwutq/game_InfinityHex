
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
            // Calculate array size
            // Recursive Formula Ak = A(k-1) + 6 * (k-1)
            // General Formula: Ak = 1 + 3 * (k-1)*(k)
            this.blockGrid = new Block[1 + 3 * (windowSize) * (windowSize - 1)];
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
        private List<Block> snake;
        private List<Block> cache;
        private CoordinateManager coordinateManager;
        private WindowManager windowManager;
        public HexEngine()
        {
            coordinateManager = new CoordinateManager();
            coordinateManager.SetCoordinateResetHandler(OnCoordinateReset);
            windowManager = new WindowManager(7);
            snake = new List<Block>(1);
            cache = new List<Block>();
            snake.Add(new Block(new Hex()));
        }
        public void Move(Hex offset)
        {
            if (offset.InRange(2))
            {
                // Request coordinate move to the opposite direction
                coordinateManager.Move(HexLib.Origin.Subtract(offset));
                // Check head

            }
            else throw new ArgumentOutOfRangeException("Move offset exceed 7-Block grid definition range");
        }
        public void OnCoordinateReset(Hex offset)
        {
            snake.ForEach(block => block.Subtract(offset));
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
        private void OnGenerationRequested(Hex[] coordinates)
        {

        }
    }

    public class BlockGenerator
    {
        private int frequency;
        private int colorRange;
        private Random colorGenerator;
        private Random stateGenerator;
        public BlockGenerator(int frequency = 9, int colorRange = 12)
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
        public Block[] GenerateBlocks(Hex[] coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);
            // Generate unoccupied blocks as default
            Block[] blocks = Array.ConvertAll(coordinates, coo => new Block(coo));
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
                blocks[index] = GenerateBlock(coordinates[index]);
            }
            // Return
            return blocks;
        }
    }

}
