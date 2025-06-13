/**
 * Main file of the game
 * Run main
 */

using Hex;

namespace Main
{
  class MainGame
  {
    static void Main(string[] args)
    {
      HexEngine hexEngine = new HexEngine();
      hexEngine.GetWindowManager().TestPopulate();
      Console.WriteLine(hexEngine.GetASCIIArt());
      hexEngine.Move(HexLib.JMinus);
      Console.WriteLine(hexEngine.GetASCIIArt());
    }
  }
}
