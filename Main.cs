/**
 * Main file of the game
 * Run main
 */

using System;
using Hex;

namespace Main
{
  class MainGame
  {
    static void Main(string[] args)
    {
#if DEBUG
      Console.WriteLine("Program Test Start");
#endif
      HexEngine hexEngine = new HexEngine();
      hexEngine.GetWindowManager().TestPopulate();
      Console.WriteLine(hexEngine.GetASCIIArt());
      hexEngine.Move(HexLib.KMinus);
      Console.WriteLine(hexEngine.GetASCIIArt());
    }
  }
}
