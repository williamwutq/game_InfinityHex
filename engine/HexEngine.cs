
namespace Hex
{
    public class CoordinateManager{
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
        ){
            this.moveLimit = moveLimit;
            this.spaceLimit = spaceLimit;
            this.origin = new Hex();
            this.moves = 0;
        }

        public void Move(Hex offset){
            moves ++;
            Hex offsetOrigin = origin.Add(offset);
            if (moves >= moveLimit || !offsetOrigin.InRange(spaceLimit)){
                origin = new Hex();
                moves = 0;
                coordinateResetHandler?.Invoke(offsetOrigin);
            } else {
                origin = offsetOrigin;
            }
        }
        
        public Hex GetOrigin(){
            return this.origin.HexClone();
        }
    }

    public class WindowManager{
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

        public Block[] GetBlocks() {
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
                coordinateManager.Move(offset);
            }
            else throw new ArgumentOutOfRangeException("Move offset exceed 7-Block grid definition range");
        }
        public void OnCoordinateReset(Hex offset)
        {

        }
        public void OnFetchRequested(Hex[] coordinates)
        {
            
        }
    }

}
