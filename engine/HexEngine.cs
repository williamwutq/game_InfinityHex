
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
        public WindowManager(int windowSize){
            this.windowSize = windowSize;
        }

    }

    public class HexEngine{
        public HexEngine(){

        }
    }

}
