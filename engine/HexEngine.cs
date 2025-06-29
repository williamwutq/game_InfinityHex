
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hex
{
    /// <summary>
    /// Manages hexagonal grid coordinates with movement tracking and origin reset functionality.
    /// Implements a coordinate system for hexagonal grids, with limits on moves and spatial range.
    /// </summary>
    public class CoordinateManager
    {
        private readonly int moveLimit;
        private readonly int spaceLimit;
        private Hex origin;
        private int moves;

        private HandleCoordinateReset? coordinateResetHandler;
        /// <summary>
        /// Delegate for handling origin reset events.
        /// </summary>
        /// <param name="oldOrigin">The origin before reset.</param>
        public delegate void HandleCoordinateReset(Hex oldOrigin);
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
        /// <summary>
        /// Moves the origin by the specified offset and handles reset conditions.
        /// </summary>
        /// <param name="offset">Hex offset to move the origin by.</param>
        /// <remarks>
        /// Increments move count and updates origin. If moveLimit or spaceLimit is exceeded,
        /// resets origin to (0,0,0), clears move count, and invokes reset handler with old origin.
        /// </remarks>
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

        /// <summary>
        /// Resets the origin to (0,0,0) and clears move count.
        /// </summary>
        public void Reset()
        {
            origin = new Hex();
            moves = 0;
        }

        /// <summary>
        /// Gets a copy of the current origin.
        /// </summary>
        /// <returns>A cloned Hex representing the current origin.</returns>
        public Hex GetOrigin()
        {
            return this.origin.HexClone();
        }

        /// <summary>
        /// Converts a relative coordinate to an absolute coordinate by adding the origin.
        /// </summary>
        /// <param name="relativeCoordinate">The relative coordinate to convert.</param>
        /// <returns>The absolute coordinate.</returns>
        /// <exception cref="ArgumentNullException">Thrown if relativeCoordinate is null.</exception>
        public Hex ToAbsolute(Hex relativeCoordinate)
        {
            ArgumentNullException.ThrowIfNull(relativeCoordinate);
            return relativeCoordinate.Add(this.origin);
        }
        /// <summary>
        /// Converts an absolute coordinate to a relative coordinate by subtracting the origin.
        /// </summary>
        /// <param name="absoluteCoordinate">The absolute coordinate to convert.</param>
        /// <returns>The relative coordinate.</returns>
        /// <exception cref="ArgumentNullException">Thrown if absoluteCoordinate is null.</exception>
        public Hex ToRelative(Hex absoluteCoordinate)
        {
            ArgumentNullException.ThrowIfNull(absoluteCoordinate);
            return absoluteCoordinate.Subtract(this.origin);
        }
        /// <summary>
        /// Converts a block in relative coordinate to a block in absolute coordinate by adding the origin.
        /// </summary>
        /// <param name="relativeBlock">The relative block to convert.</param>
        /// <returns>The absolute block.</returns>
        /// <exception cref="ArgumentNullException">Thrown if relativeBlock is null.</exception>
        public Block ToAbsolute(Block relativeBlock)
        {
            ArgumentNullException.ThrowIfNull(relativeBlock);
            return relativeBlock.Add(this.origin);
        }
        /// <summary>
        /// Converts a block in absolute coordinate to a block in relative coordinate by subtracting the origin.
        /// </summary>
        /// <param name="absoluteBlock">The absolute block to convert.</param>
        /// <returns>The relative block.</returns>
        /// <exception cref="ArgumentNullException">Thrown if absoluteBlock is null.</exception>
        public Block ToRelative(Block absoluteBlock)
        {
            ArgumentNullException.ThrowIfNull(absoluteBlock);
            return absoluteBlock.Subtract(this.origin);
        }
        /// <summary>
        /// Converts an array of relative coordinates to absolute coordinates.
        /// </summary>
        /// <param name="relativeCoordinates">Array of relative coordinates.</param>
        /// <returns>Array of absolute coordinates.</returns>
        public Hex[] ToAbsolute(Hex[] relativeCoordinates)
        {
            return Array.ConvertAll(relativeCoordinates, coo => ToAbsolute(coo));
        }
        /// <summary>
        /// Converts an array of absolute coordinates to relative coordinates.
        /// </summary>
        /// <param name="absoluteCoordinates">Array of absolute coordinates.</param>
        /// <returns>Array of relative coordinates.</returns>
        public Hex[] ToRelative(Hex[] absoluteCoordinates)
        {
            return Array.ConvertAll(absoluteCoordinates, coo => ToRelative(coo));
        }
        /// <summary>
        /// Converts an array of blocks in relative coordinates to absolute coordinates.
        /// </summary>
        /// <param name="relativeBlocks">Array of blocks in relative coordinates.</param>
        /// <returns>Array of blocks in absolute coordinates.</returns>
        public Block[] ToAbsolute(Block[] relativeBlocks)
        {
            return Array.ConvertAll(relativeBlocks, coo => ToAbsolute(coo));
        }
        /// <summary>
        /// Converts an array of blocks in absolute coordinates to relative coordinates.
        /// </summary>
        /// <param name="absoluteBlocks">Array of blocks in absolute coordinates.</param>
        /// <returns>Array of blocks in relative coordinates.</returns>
        public Block[] ToRelative(Block[] absoluteBlocks)
        {
            return Array.ConvertAll(absoluteBlocks, coo => ToRelative(coo));
        }
    }

    public class WindowManager
    {
        private readonly int windowSize;
        private Block[] blockGrid;

        public delegate Block[] HandleFetchBlock(Hex[] coordinates);
        private HandleFetchBlock? fetchBlockHandler;
        public void SetFetchBlockHandler(HandleFetchBlock fetchBlockHandler)
        {
            this.fetchBlockHandler = fetchBlockHandler;
        }

        public WindowManager(int windowSize)
        {
            this.windowSize = windowSize;
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
            Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(blockGrid, block => block.HexClone()));
            if (fetchedGrid != null)
            {
                for (int index = 0; index < blockGrid.Length; index++)
                {
                    blockGrid[index].SetColor(fetchedGrid[index].Color());
                    blockGrid[index].SetState(fetchedGrid[index].State());
                }
            }
        }

        public void Reset()
        {
            foreach (Block block in blockGrid)
            {
                block.SetColor(-1); block.SetState(false);
            }
            // fetch request
            Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(blockGrid, block => block.HexClone()));
            if (fetchedGrid != null)
            {
                for (int index = 0; index < blockGrid.Length; index++)
                {
                    blockGrid[index].SetColor(fetchedGrid[index].Color());
                    blockGrid[index].SetState(fetchedGrid[index].State());
                }
            }
        }

        private int Search(Hex coordinate, int start, int end)
        {
            if (start > end) { return -1; }
            int middleIndex = start + (end - start) / 2;
            Block middle = blockGrid[middleIndex];
            int i = coordinate.LineI;
            int k = coordinate.LineK;
            int mi = middle.LineI;
            int mk = middle.LineK;
            if (mi == i && mk == k)
            {
                return middleIndex;
            }
            if (mi < i || (mi == i && mk < k))
            {
                // second half
                return Search(coordinate, middleIndex + 1, end);
            }
            else
            {
                // first half
                return Search(coordinate, start, middleIndex - 1);
            }
        }

        public Block GetBlock(int index) => blockGrid[index];
        public Block GetBlock(Hex coordinate)
        {
            ArgumentNullException.ThrowIfNull(coordinate, "Input coordinate cannot be null");
            int index = Search(coordinate, 0, blockGrid.Length - 1);
            if (index == -1)
            {
                throw new ArgumentOutOfRangeException($"Target coordinate out of range of the displayed block grid of radius {windowSize}");
            }
            return blockGrid[index];
        }

        public String GetASCIIArt()
        {
            StringBuilder sb = new StringBuilder();
            int size = windowSize - 1;
            for (int lineJ = -size; lineJ < windowSize; lineJ++)
            {
                for (int j = -size * 2; j <= size * 2; j++)
                {
                    // Calculate hex coordinate
                    Hex coordinate = new Hex((j + 3 * lineJ) / 2, (j - 3 * lineJ) / 2);
                    // Filter for "straight" hexes
                    if (coordinate.LineJ != lineJ || coordinate.J != j)
                    {
                        sb.Append(' ');
                    }
                    else try
                        {
                            Block block = GetBlock(coordinate);
                            if (block.State())
                            {
                                if (block.Color() == -2)
                                {
                                    sb.Append('X'); // Snake head
                                }
                                else if (block.Color() == -1)
                                {
                                    sb.Append('O'); // Unoccupied
                                }
                                else if (block.Color() < 10)
                                {
                                    sb.Append(block.Color().ToString("X")); // Occupied with color
                                }
                                else if (block.Color() < 36)
                                {
                                    sb.Append((char)('A' + block.Color() - 10)); // Occupied with color A-Z
                                }
                                else if (block.Color() < 62)
                                {
                                    sb.Append((char)('a' + block.Color() - 36)); // Occupied with color a-z
                                }
                                else if (block.Color() == 62)
                                {
                                    sb.Append('+'); // Occupied with color +
                                }
                                else if (block.Color() == 63)
                                {
                                    sb.Append('-'); // Occupied with color -
                                }
                                else
                                {
                                    sb.Append('?');
                                }
                            }
                            else
                            {
                                sb.Append('O');
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            sb.Append(' ');
                        }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public void Move(Hex offset)
        {
            ArgumentNullException.ThrowIfNull(offset, "Input offset cannot be null");
            if (offset.Equals(HexLib.Origin))
            {
                throw new ArgumentOutOfRangeException("No move recieved");
            }
            else if (offset.Equals(HexLib.IMinus))
            {
                Block[] artifacts = new Block[windowSize * 2 - 1];
                int artifactIndex = 0;
                int index = 0;
                void ProcessRow(int rowLength)
                {
                    for (int b = 0; b < rowLength; b++)
                    {
                        if (b != 0)
                        {
                            Block block = blockGrid[index];
                            Block prev = blockGrid[index - 1];
                            prev.SetColor(block.Color());
                            prev.SetState(block.State());
                        }
                        else if (index != 0)
                        {
                            // Mark artifact
                            artifacts[artifactIndex] = blockGrid[index - 1];
                            artifactIndex++;
#if DEBUG
                            blockGrid[index - 1].SetColor(-1);
#endif
                        }
                        index++;
                    }
                }
                for (int i = 0; i < windowSize - 1; i++) ProcessRow(windowSize + i);
                for (int i = windowSize - 1; i >= 0; i--) ProcessRow(windowSize + i);
                artifacts[artifactIndex] = blockGrid[index - 1];
#if DEBUG
                blockGrid[index - 1].SetColor(-1);
#endif
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2 - 1)
                {
                    int i_index = -1;
                    int i_artifactIndex = 0;
                    for (int i = 0; i < windowSize - 1; i++)
                    {
                        if (i_index != -1)
                        {
                            blockGrid[i_index] = fetchedGrid[i_artifactIndex];
                            i_artifactIndex++;
                        }
                        i_index += (windowSize + i);
                    }
                    for (int i = windowSize - 1; i >= 0; i--)
                    {
                        blockGrid[i_index] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                        i_index += (windowSize + i);
                    }
                    blockGrid[i_index] = fetchedGrid[i_artifactIndex];
                }
            }
            else if (offset.Equals(HexLib.IPlus))
            {
                Block[] artifacts = new Block[windowSize * 2 - 1];
                artifacts[0] = blockGrid[0];
                int index = blockGrid.Length - 1;
                int artifactIndex = 0;
                void ProcessRow(int rowLength)
                {
                    for (int b = rowLength - 1; b >= 0; b--)
                    {
                        if (b != 0)
                        {
                            Block block = blockGrid[index];
                            Block prev = blockGrid[index - 1];
                            block.SetColor(prev.Color());
                            block.SetState(prev.State());
                        }
                        else
                        {
                            // Mark artifact
                            artifacts[artifactIndex] = blockGrid[index];
                            artifactIndex++;
#if DEBUG
                            blockGrid[index].SetColor(-1);
#endif
                        }
                        index--;
                    }
                }
                for (int i = 0; i < windowSize - 1; i++) ProcessRow(windowSize + i);
                for (int i = windowSize - 1; i >= 0; i--) ProcessRow(windowSize + i);
#if DEBUG
                blockGrid[0].SetColor(-1);
#endif
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2 - 1)
                {
                    blockGrid[0] = fetchedGrid[0];
                    int i_index = blockGrid.Length - 1;
                    int i_artifactIndex = 0;
                    for (int i = 0; i < windowSize - 1; i++)
                    {
                        i_index -= (windowSize + i);
                        blockGrid[i_index + 1] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                    }
                    for (int i = windowSize - 1; i >= 0; i--)
                    {
                        i_index -= (windowSize + i);
                        blockGrid[i_index + 1] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                    }
                }
            }
            else if (offset.Equals(HexLib.JMinus))
            {
                Block[] artifacts = new Block[windowSize * 2 - 1];
                int artifactIndex = 0;
                void Shift(int start, int end)
                {
                    Block prev = blockGrid[start];
                    Block block = blockGrid[end];
                    prev.SetColor(block.Color());
                    prev.SetState(block.State());
                }
                for (int r = 0; r < windowSize; r++)
                {
                    int index = r;
                    Boolean notLast = r != windowSize - 1;
                    for (int c = 0; c < windowSize - 1; c++)
                    {
                        Shift(index, index += windowSize + c + 1);
                    }
                    if (notLast)
                    {
                        Shift(index, index += windowSize * 2 - 1);
                    }
                    else
                    {
                        artifacts[artifactIndex] = blockGrid[index];
                        artifactIndex++;
#if DEBUG
                        blockGrid[index].SetColor(-1);
#endif
                        index += windowSize * 2 - 1;
                    }
                    for (int c = 0; c < windowSize - r - 2; c++)
                    {
                        Shift(index, index += 2 * windowSize - c - 2);
                    }
                    if (notLast)
                    {
                        artifacts[artifactIndex] = blockGrid[index];
                        artifactIndex++;
#if DEBUG
                        blockGrid[index].SetColor(-1);
#endif
                    }
                }
                for (int r = 1; r < windowSize; r++)
                {
                    int index = windowSize * r + r * (r - 1) / 2;
                    for (int c = 0; c < windowSize - r - 1; c++)
                    {
                        Shift(index, index += windowSize + c + r + 1);
                    }
                    Shift(index, index += 2 * windowSize - 1);
                    for (int c = 0; c < windowSize - 2; c++)
                    {
                        Shift(index, index += 2 * windowSize - c - 2);
                    }
                    artifacts[artifactIndex] = blockGrid[index];
                    artifactIndex++;
#if DEBUG
                    blockGrid[index].SetColor(-1);
#endif
                }
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2 - 1)
                {
                    int i_artifactIndex = 0;
                    int wm = windowSize - 1;
                    int ws = 3 * windowSize * wm;
                    for (int r = 0; r < windowSize - 1; r++)
                    {
                        blockGrid[ws - r * wm - r * (r + 1) / 2] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                    }
                    blockGrid[windowSize * windowSize - 1 + windowSize * wm / 2] = fetchedGrid[i_artifactIndex];
                    i_artifactIndex++;
                    for (int r = 1; r < windowSize; r++)
                    {
                        blockGrid[ws - r] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                    }
                }
            }
            else if (offset.Equals(HexLib.JPlus))
            {
                Block[] artifacts = new Block[windowSize * 2 - 1];
                int artifactIndex = 0;
                void Shift(int start, int end)
                {
                    Block next = blockGrid[end];
                    Block block = blockGrid[start];
                    block.SetColor(next.Color());
                    block.SetState(next.State());
                }
                for (int r = 0; r < windowSize; r++)
                {
                    Boolean notLast = r != windowSize - 1;
                    int wm = windowSize - 1;
                    int index = 3 * windowSize * wm - r * wm - r * (r + 1) / 2;
                    for (int c = windowSize - r - 3; c >= 0; c--)
                    {
                        Shift(index, index -= 2 * windowSize - c - 2);
                    }
                    if (notLast)
                    {
                        Shift(index, index -= windowSize * 2 - 1);
                    }
                    for (int c = windowSize - 2; c >= 0; c--)
                    {
                        Shift(index, index -= windowSize + c + 1);
                    }
                    artifacts[artifactIndex] = blockGrid[index];
                    artifactIndex++;
#if DEBUG
                    blockGrid[index].SetColor(-1);
#endif
                }
                for (int r = 1; r < windowSize; r++)
                {
                    int index = 3 * windowSize * (windowSize - 1) - r;
                    for (int c = windowSize - 2 - 1; c >= 0; c--)
                    {
                        Shift(index, index -= 2 * windowSize - c - 2);
                    }
                    Shift(index, index -= 2 * windowSize - 1);
                    for (int c = windowSize - r - 2; c >= 0; c--)
                    {
                        Shift(index, index -= windowSize + c + r + 1);
                    }
                    artifacts[artifactIndex] = blockGrid[index];
                    artifactIndex++;
#if DEBUG
                    blockGrid[index].SetColor(-1);
#endif
                }
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2 - 1)
                {
                    int i_artifactIndex = 0;
                    while (i_artifactIndex < windowSize)
                    {
                        blockGrid[i_artifactIndex] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                    }
                    for (int r = 1; r < windowSize; r++)
                    {
                        int i_index = windowSize * r + r * (r - 1) / 2;
                        blockGrid[i_index] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                    }
                }
            }
            else if (offset.Equals(HexLib.KMinus))
            {
                Block[] artifacts = new Block[windowSize * 2 - 1];
                int artifactIndex = 0;
                void Shift(int start, int end)
                {
                    Block prev = blockGrid[start];
                    Block block = blockGrid[end];
                    prev.SetColor(block.Color());
                    prev.SetState(block.State());
                }
                for (int r = 0; r < windowSize; r++)
                {
                    int index = r;
                    for (int c = 0; c < windowSize - 1; c++)
                    {
                        Shift(index, index += windowSize + c);
                    }
                    for (int c = 0; c < r; c++)
                    {
                        Shift(index, index += 2 * windowSize - c - 2);
                    }
                    artifacts[artifactIndex] = blockGrid[index];
                    artifactIndex++;
#if DEBUG
                    blockGrid[index].SetColor(-1);
#endif
                }
                for (int r = 1; r < windowSize; r++)
                {
                    int index = windowSize * (r + 1) + r * (r + 1) / 2 - 1;
                    for (int c = r; c < windowSize - 1; c++)
                    {
                        Shift(index, index += windowSize + c);
                    }
                    for (int c = windowSize - 2; c >= 0; c--)
                    {
                        Shift(index, index += windowSize + c);
                    }
                    artifacts[artifactIndex] = blockGrid[index];
                    artifactIndex++;
#if DEBUG
                    blockGrid[index].SetColor(-1);
#endif
                }
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2 - 1)
                {
                    int i_artifactIndex = 0;
                    int wm = windowSize - 1;
                    int i_index = wm * wm + wm * windowSize / 2;
                    for (int r = 1; r < windowSize; r++)
                    {
                        blockGrid[i_index] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                        i_index += 2 * windowSize - r;
                    }
                    for (int r = 0; r < windowSize; r++)
                    {
                        blockGrid[i_index] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                        i_index++;
                    }
                }
            }
            else if (offset.Equals(HexLib.KPlus))
            {
                Block[] artifacts = new Block[windowSize * 2 - 1];
                int artifactIndex = 0;
                int wm = windowSize - 1;
                int ws = 3 * windowSize * wm - wm;
                int wb = wm * wm + wm * windowSize / 2;
                void Shift(int start, int end)
                {
                    Block next = blockGrid[end];
                    Block block = blockGrid[start];
                    block.SetColor(next.Color());
                    block.SetState(next.State());
                }
                for (int r = 0; r < windowSize; r++)
                {
                    int index = wb + 2 * windowSize * r - (r + 1) * r / 2;
                    for (int c = r; c > 0; c--)
                    {
                        Shift(index, index -= 2 * windowSize - c - 1);
                    }
                    for (int c = windowSize - 2; c >= 0; c--)
                    {
                        Shift(index, index -= windowSize + c);
                    }
                    artifacts[artifactIndex] = blockGrid[index];
                    artifactIndex++;
#if DEBUG
                    blockGrid[index].SetColor(-1);
#endif
                }
                for (int r = 1; r < windowSize; r++)
                {
                    int index = ws + r;
                    for (int c = 0; c < windowSize - 1; c++)
                    {
                        Shift(index, index -= windowSize + c);
                    }
                    for (int c = windowSize - 2; c >= r; c--)
                    {
                        Shift(index, index -= windowSize + c);
                    }
                    artifacts[artifactIndex] = blockGrid[index];
                    artifactIndex++;
#if DEBUG
                    blockGrid[index].SetColor(-1);
#endif
                }
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2 - 1)
                {
                    int i_artifactIndex = 0;
                    while (i_artifactIndex < windowSize)
                    {
                        blockGrid[i_artifactIndex] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                    }
                    for (int r = 1; r < windowSize; r++)
                    {
                        int i_index = windowSize * (r + 1) + r * (r + 1) / 2 - 1;
                        blockGrid[i_index] = fetchedGrid[i_artifactIndex];
                        i_artifactIndex++;
                    }
                }
            }
            else throw new ArgumentOutOfRangeException("Move offset exceed 7-Block grid definition range");
        }

        public void TestPopulate()
        {
            // This method is for testing purpose only
            for (int i = 0; i < blockGrid.Length; i++)
            {
                blockGrid[i].SetState(true);
                blockGrid[i].SetColor(i % 64);
            }
        }

        public Block[] GetBlocks()
        {
            // This method is not safe. Please do not modify the returned array of Blocks.
            return this.blockGrid;
        }
    }

    public class HexEngine
    {
        private readonly List<Block> cache;
        private readonly CoordinateManager coordinateManager;
        private readonly WindowManager windowManager;
        private readonly BlockGenerator blockGenerator;
        public HexEngine()
        {
            cache = new List<Block>();
            blockGenerator = new BlockGenerator();
            coordinateManager = new CoordinateManager();
            coordinateManager.SetCoordinateResetHandler(OnCoordinateReset);
            windowManager = new WindowManager(7);
            windowManager.SetFetchBlockHandler(OnFetchRequested);
            windowManager.Reset();
        }
        public WindowManager GetWindowManager()
        {
            return windowManager;
        }
        public void Move(Hex offset)
        {
            if (offset.InRange(2))
            {
                // Save original origin
                Hex originalOrigin = coordinateManager.GetOrigin();
                // Request coordinate move to the opposite direction
                coordinateManager.Move(HexLib.Negate(offset));
                // Visual move
                windowManager.Move(offset);
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
        public Block[] OnFetchRequested(Hex[] coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);
            if (coordinates.Length == 0)
            {
                return [];
            }
            else if (coordinates.Length == 1)
            {
                // If only one coordinate, return it immediately
                return [coordinateManager.ToAbsolute(SafeGetBlock(coordinateManager.ToRelative(coordinates[0])))];
            }
            // Cast to relative
            coordinates = coordinateManager.ToRelative(coordinates);
            List<Hex> notInCache = new List<Hex>();
            foreach (Hex coordinate in coordinates)
            {
                if (!cache.Any(block => block.HexClone().Equals(coordinate)))
                {
                    notInCache.Add(coordinate);
                }
            }
            // If all blocks are in cache, return them, else generate blocks
            if (notInCache.Count == 0)
            {
                return Array.ConvertAll(coordinates, coo => coordinateManager.ToAbsolute(CacheSearch(coo)));
            }
            else
            {
                OnGenerationRequested(notInCache.ToArray());
            }
            // Return all blocks in cache
            return Array.ConvertAll(coordinates, coo => coordinateManager.ToAbsolute(CacheSearch(coo)));
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
            List<Block> validList = cache.Where(block => block.HexClone().Equals(coordinate)).ToList();
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
            cache.Clear();
            coordinateManager.Reset();
            windowManager.Reset();
        }
        private void OnGenerationRequested(Hex[] coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);
            if (coordinates.Length == 0)
            {
                return;
            } else if (coordinates.Length == 1) {
                cache.Add(blockGenerator.GenerateBlock(coordinates[0]));
            }
            else
            {
                cache.AddRange(blockGenerator.GenerateBlocks(coordinates));
            }
        }
        public String GetASCIIArt()
        {
            return windowManager.GetASCIIArt();
        }
    }

    /// <summary>
    /// Generates <see cref="Block"/> instances with configurable frequency and color range,
    /// supporting both single and batch block generation for a hexagonal grid.
    /// </summary>
    public class BlockGenerator
    {
        private readonly int frequency;
        private readonly int colorRange;
        private readonly Random colorGenerator;
        private readonly Random stateGenerator;
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
