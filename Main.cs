/**
 * Main file of the game
 * Run main
 */

using System;
using System.Threading;
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
      Random random = new Random();
      while (true)
      {
        Thread.Sleep(200);
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine(hexEngine.GetASCIIArt());
        hexEngine.Move(HexLib.CircularSixBlock(random.Next(6)));
      }
    }
  }
}
