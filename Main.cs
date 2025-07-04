/**
 * Main file of the game
 * Run main
 */

using System;
using System.Threading;
using Engine;
using Interactive;

namespace Main
{
  /*
  class MainGame
  {
    static void Main(string[] args)
    {
#if DEBUG
      Console.WriteLine("Program Test Start");
#endif
      HexEngine hexEngine = new HexEngine();
      KeyboardListener listener = new KeyboardListener(hexEngine.GetDirectionManager());
      listener.Start();
      Random random = new Random();
      while (true)
      {
        Thread.Sleep(200);
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        // Console.WriteLine(hexEngine.GetASCIIArt());
        Console.WriteLine(hexEngine.GetASCIIArt(0));
        hexEngine.Move();
      }
    }
  }
  //*/
}
