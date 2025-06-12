

namespace Hex
{
    public static class HexLib
    {
        public static Hex Origin => Hex.LineHex();
        public static Hex IPlus => Hex.LineHex(1, 0);
        public static Hex JPlus => Hex.LineHex(1, 1);
        public static Hex KPlus => Hex.LineHex(0, 1);
        public static Hex IMinus => Hex.LineHex(-1, 0);
        public static Hex JMinus => Hex.LineHex(-1, -1);
        public static Hex KMinus => Hex.LineHex(0, -1);
        public static Hex SevenBlock(int index)
        {
            if (index == 0) {
                return JMinus;
            } else if (index == 1) {
                return IMinus;
            } else if (index == 2) {
                return KMinus;
            } else if (index == 3) {
                return Origin;
            } else if (index == 4) {
                return KPlus;
            } else if (index == 5) {
                return IPlus;
            } else if (index == 6) {
                return JPlus;
            } else throw new IndexOutOfRangeException($"Index {index} out bounds for range 7");
        }
    }
}