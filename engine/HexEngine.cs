
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hex;
using Core;

namespace Engine
{
    public class WindowManager
    {
        private readonly int windowSize;
        private Block[] blockGrid;

        public delegate Block[] HandleFetchBlock(Hex.Hex[] coordinates);
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
                    Block nb = new Block(new Hex.Hex());
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

        public int GetWindowSize() => windowSize;

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

        private int Search(Hex.Hex coordinate, int start, int end)
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
        public Block GetBlock(Hex.Hex coordinate)
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
                    Hex.Hex coordinate = new Hex.Hex((j + 3 * lineJ) / 2, (j - 3 * lineJ) / 2);
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

        public void Move(Hex.Hex offset)
        {
            ArgumentNullException.ThrowIfNull(offset, "Input offset cannot be null");
            if (offset.Equals(HexLib.Origin))
            {
                throw new ArgumentOutOfRangeException("No move recieved");
            }
            else if (offset.Equals(HexLib.IMinus))
            {
                Block[] artifacts = new Block[windowSize * 2];
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
                blockGrid[blockGrid.Length / 2].SetColor(64);
#endif
                artifacts[artifactIndex + 1] = blockGrid[blockGrid.Length / 2];
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2)
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
                    blockGrid[blockGrid.Length / 2] = fetchedGrid[i_artifactIndex + 1];
                }
            }
            else if (offset.Equals(HexLib.IPlus))
            {
                Block[] artifacts = new Block[windowSize * 2];
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
                blockGrid[blockGrid.Length / 2].SetColor(64);
#endif
                artifacts[artifactIndex] = blockGrid[blockGrid.Length / 2];
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2)
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
                    blockGrid[blockGrid.Length / 2] = fetchedGrid[i_artifactIndex];
                }
            }
            else if (offset.Equals(HexLib.JMinus))
            {
                Block[] artifacts = new Block[windowSize * 2];
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
                artifacts[artifactIndex] = blockGrid[blockGrid.Length / 2];
#if DEBUG
                blockGrid[blockGrid.Length / 2].SetColor(64);
#endif
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2)
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
                    blockGrid[blockGrid.Length / 2] = fetchedGrid[i_artifactIndex];
                }
            }
            else if (offset.Equals(HexLib.JPlus))
            {
                Block[] artifacts = new Block[windowSize * 2];
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
                artifacts[artifactIndex] = blockGrid[blockGrid.Length / 2];
#if DEBUG
                blockGrid[blockGrid.Length / 2].SetColor(64);
#endif
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2)
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
                    blockGrid[blockGrid.Length / 2] = fetchedGrid[i_artifactIndex];
                }
            }
            else if (offset.Equals(HexLib.KMinus))
            {
                Block[] artifacts = new Block[windowSize * 2];
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
                artifacts[artifactIndex] = blockGrid[blockGrid.Length / 2];
#if DEBUG
                blockGrid[blockGrid.Length / 2].SetColor(64);
#endif
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2)
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
                    blockGrid[blockGrid.Length / 2] = fetchedGrid[i_artifactIndex];
                }
            }
            else if (offset.Equals(HexLib.KPlus))
            {
                Block[] artifacts = new Block[windowSize * 2];
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
                artifacts[artifactIndex] = blockGrid[blockGrid.Length / 2];
#if DEBUG
                blockGrid[blockGrid.Length / 2].SetColor(64);
#endif
                // Fetch request
                Block[]? fetchedGrid = fetchBlockHandler?.Invoke(Array.ConvertAll(artifacts, block => block.HexClone()));
                if (fetchedGrid != null && fetchedGrid.Length == windowSize * 2)
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
                    blockGrid[blockGrid.Length / 2] = fetchedGrid[i_artifactIndex];
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

    public class HexEngine : IHexPrintable
    {
        private Object cacheLock = new();
        private volatile bool updated = false;
        private readonly LinkedList<TimedObject<Block>> cache;
        private readonly CoordinateManager coordinateManager;
        private readonly TimeReferenceManager timeReferenceManager;
        private int snakeLength;
        private readonly WindowManager windowManager;
        private readonly BlockGenerator blockGenerator;
        private DirectionManager directionManager;
        public event IHexPrintable.HexRenderDelegate? OnHexRender;
        public HexEngine()
        {
            lock (cacheLock)
            {
                cache = new();
                snakeLength = 1;
                timeReferenceManager = new TimeReferenceManager(256, 65536);
                timeReferenceManager.SetTimeResetHandler(OnTimeReset);
                cache.AddFirst(timeReferenceManager.ConstructAbsoluteTimedObject<Block>(new Block(new Hex.Hex(), -2, true)));
            }
            directionManager = new DirectionManager(true);
            blockGenerator = new BlockGenerator(12, 16);
            coordinateManager = new CoordinateManager(4096, 256);
            coordinateManager.SetCoordinateResetHandler(OnCoordinateReset);
            windowManager = new WindowManager(11);
            windowManager.SetFetchBlockHandler(OnFetchRequested);
            windowManager.Reset();
        }
        public void SetDirectionManager(DirectionManager directionManager)
        {
            ArgumentNullException.ThrowIfNull(directionManager);
            directionManager.Set(this.directionManager.GetOffsetIndex());
            this.directionManager = directionManager;
        }
        public WindowManager GetWindowManager()
        {
            return windowManager;
        }
        public DirectionManager GetDirectionManager()
        {
            return directionManager;
        }
        public void Move()
        {
            // Move with directionManager
            Move(directionManager.GetOffset());
        }
        public void Move(Hex.Hex offset)
        {
            if (offset.InRange(2))
            {
                // Increment time
                timeReferenceManager.Age();
                // Request coordinate move to the same direction as snake
                coordinateManager.Move(offset);
                // Visual background move opposite to snake
                windowManager.Move(HexLib.Negate(offset));
                // Check head
                TimedObject<Block> timedHead;
                try
                {
                    timedHead = CacheSearch(coordinateManager.GetOrigin());
                }
                catch (InvalidOperationException)
                {
                    OnGenerationRequested([coordinateManager.GetOrigin()]);
                    timedHead = CacheSearch(coordinateManager.GetOrigin());
                }
                Block head = timedHead.GetObject();
                if (head.State())
                {
                    // If head is occupied, check the type of occupation
                    if (head.Color() == -2)
                    {
                        // If occupied by snake, cause snake death
                        // Currently, only reset everything
                        ResetEngine();
                    }
                    else if (snakeLength + 1 >= timeReferenceManager.GetRelativeExpire())
                    {
                        // If snake reach maximum length, end game
                        // Currently, only reset everything
                        ResetEngine();
                    }
                    else
                    {
                        // Eat food, increment snake length
                        snakeLength++;
                        timeReferenceManager.Renew(timedHead);
                        head.SetState(true);
                        head.SetColor(-2); // -2 (default occupied color) refering to snake
                    }
                }
                else
                {
                    // Remove the tail to not increment snake length
                    TimedObject<Block>? timedTail;
                    try
                    {
                        timedTail = cache.First(obj => timeReferenceManager.ToRelative(obj.GetTime()) == snakeLength && obj.GetObject().State() && obj.GetObject().Color() == -2);
                        Block tail = timedTail.GetObject();
                        timeReferenceManager.Renew(timedTail);
                        tail.SetState(false);
                        tail.SetColor(-1); // -1 (default unoccupied color) refering to empty space
                    }
                    catch (InvalidOperationException){}
                    // If not, this block is now snake head
                    timeReferenceManager.Renew(timedHead);
                    head.SetState(true);
                    head.SetColor(-2); // -2 (default occupied color) refering to snake
                }
                // Remove outdated cache if exist using expire in timeReferenceManager
                while (cache.Count > 0 && cache.Last != null && timeReferenceManager.IsExpired(cache.Last.Value))
                {
                    cache.RemoveLast();
                }
                // Mark the grid as updated
                updated = true;
                OnHexRender?.Invoke(this);
            }
            else throw new ArgumentOutOfRangeException("Move offset exceed 7-Block grid definition range");
        }
        public void OnCoordinateReset(Hex.Hex offset)
        {
            offset = new Hex.Hex().Subtract(offset);
            lock (cacheLock)
            {
                foreach (TimedObject<Block> block in cache)
                {
                    block.GetObject().Move(offset);
                }
            }
        }
        public void OnTimeReset(int time)
        {
            lock (cacheLock)
            {
                foreach (TimedObject<Block> timedObject in cache)
                {
                    timedObject.Age(time);
                }
            }
        }
        public Block[] OnFetchRequested(Hex.Hex[] coordinates)
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
            List<Hex.Hex> notInCache = new List<Hex.Hex>();
            foreach (Hex.Hex coordinate in coordinates)
            {
                lock (cacheLock)
                {
                    if (!cache.Any(block => block.GetObject().HexClone().Equals(coordinate)))
                    {
                        notInCache.Add(coordinate);
                    }
                }
            }
            // If all blocks are in cache, return them, else generate blocks
            if (notInCache.Count == 0)
            {
                // Console.WriteLine("All in cache");
            }
            else
            {
                // Console.WriteLine("Not all in cache");
                OnGenerationRequested([.. notInCache]);
            }
            // Return all blocks in cache
            return Array.ConvertAll(coordinates, coo => coordinateManager.ToAbsolute(SafeGetBlock(coo)));
        }

        public Block SafeGetBlock(Hex.Hex coordinate)
        {
            ArgumentNullException.ThrowIfNull(coordinate);
            try
            {
                return CacheSearch(coordinate).GetObject();
            }
            catch (InvalidOperationException)
            {
                // If block not found in cache, generate a new block immediately
                OnGenerationRequested([coordinate]);
                // Now return searched block
                return CacheSearch(coordinate).GetObject();
            }
        }
        private TimedObject<Block> CacheSearch(Hex.Hex coordinate)
        {
            lock (cacheLock)
            {
                List<TimedObject<Block>> validList = cache.Where(block => block.GetObject().HexClone().Equals(coordinate)).ToList();
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
                    TimedObject<Block> first = validList[0];
                    foreach (var block in validList.Skip(1))
                    {
                        cache.Remove(block);
                    }
                    return first;
                }
            }
        }
        public void ResetEngine()
        {
            lock (cacheLock)
            {
                cache.Clear();
                timeReferenceManager.Reset();
                cache.AddFirst(timeReferenceManager.ConstructAbsoluteTimedObject<Block>(new Block(new Hex.Hex(), -2, true)));
            }
            snakeLength = 1;
            coordinateManager.Reset();
            windowManager.Reset();
        }
        private void OnGenerationRequested(Hex.Hex[] coordinates)
        {
            ArgumentNullException.ThrowIfNull(coordinates);
            if (coordinates.Length == 1)
            {
                lock (cacheLock)
                {
                    cache.AddFirst(timeReferenceManager.ConstructAbsoluteTimedObject<Block>(blockGenerator.GenerateBlock(coordinates[0])));
                }
            }
            else
            {
                lock (cacheLock)
                {
                    foreach (Hex.Hex coo in coordinates)
                    {
                        cache.AddFirst(timeReferenceManager.ConstructAbsoluteTimedObject<Block>(blockGenerator.GenerateBlock(coo)));
                    }
                }
            }
        }
        public Block[] GetBlocks()
        {
            int windowSize = windowManager.GetWindowSize();
            int size = windowSize - 1;
            Block[] blocks = new Block[1 + 3 * windowSize * (windowSize - 1)];
            int index = 0;
            for (int lineJ = -size; lineJ < windowSize; lineJ++)
            {
                for (int j = -size * 2; j <= size * 2; j++)
                {
                    // Calculate hex coordinate
                    Hex.Hex coordinate = new Hex.Hex((j + 3 * lineJ) / 2, (j - 3 * lineJ) / 2);
                    // Filter for "straight" hexes
                    if (coordinate.LineJ == lineJ && coordinate.J == j && coordinate.InRange(windowSize))
                    {
                        blocks[index] = SafeGetBlock(coordinateManager.ToAbsolute(coordinate));
                        index++;
                    }
                }
            }
            return blocks;
        }
        public int GetRadius()
        {
            return windowManager.GetWindowSize();
        }
        public bool IsGridUpdated()
        {
            bool isUpdated = updated;
            updated = false; // Reset the update status after checking
            return isUpdated;
        }
    }
}
