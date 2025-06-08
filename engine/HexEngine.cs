
namespace Hex
{
    public class CoordinateManager{
        private int moveLimit;
        private int spaceLimit;
        private Hex origin;
        private int moves;

        public delegate void HandleCoordinateReset(Hex oldOrigin);
        public event HandleCoordinateReset? OnReset;
        
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
                OnReset?.Invoke(offsetOrigin);
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
        public event HandleFetchBlock? OnFetch;

        public WindowManager(int windowSize){
            this.windowSize = windowSize;
            // Calculate array size
            // Recursive Formula Ak = A(k-1) + 6 * (k-1)
            // General Formula: Ak = 1 + 3 * (k-1)*(k)
            this.blockGrid = new Block[1 + 3*(windowSize)*(windowSize-1)];
            // Add into array to generate the grid
            int i = 0;
            for(int a = 1-windowSize; a < windowSize; a ++){
                for(int b = 1-windowSize; b < windowSize; b ++){
                    Block nb = new Block(new Hex());
                    nb.MoveI(b);
                    nb.MoveK(a);
                    if(nb.InRange(windowSize)){
                        blockGrid[i] = nb;
                        i ++;
                    }
                }
            }
            // Already sorted by first I then K

            // fetch request
            OnFetch?.Invoke(Array.ConvertAll(blockGrid, block => block.HexClone()));
        }

        public Block[] GetBlocks() {
            // This method is not safe. Please do not modify the returned array of Blocks.
            return this.blockGrid;
        }
    }

    public class HexEngine{
        public HexEngine(){

        }
    }

}
