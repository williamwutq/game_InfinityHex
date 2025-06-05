using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hex;

namespace Hex.Tests
{
    [TestClass]
    public class HexTest
    {
        [TestMethod]
        public void TestDefaultConstructor()
        {
            Hex hex = new Hex();
            Assert.AreEqual(0, hex.I, "I coordinate should be 0");
            Assert.AreEqual(0, hex.J, "J coordinate should be 0");
            Assert.AreEqual(0, hex.K, "K coordinate should be 0");
            Assert.AreEqual(0, hex.LineI, "Line I should be 0");
            Assert.AreEqual(0, hex.LineJ, "Line J should be 0");
            Assert.AreEqual(0, hex.LineK, "Line K should be 0");
        }

        [TestMethod]
        public void TestCoordinateConstructor()
        {
            Hex hex = new Hex(5, 2);
            Assert.AreEqual(5, hex.I, "I coordinate should be 5");
            Assert.AreEqual(7, hex.J, "J coordinate should be 7");
            Assert.AreEqual(2, hex.K, "K coordinate should be 2");
            Assert.AreEqual(3, hex.LineI, "Line I should be 3");
            Assert.AreEqual(1, hex.LineJ, "Line J should be 1");
            Assert.AreEqual(4, hex.LineK, "Line K should be 4");
        }

        [TestMethod]
        public void TestStaticHexFactory()
        {
            Hex hex = Hex.LineHex();
            Assert.AreEqual(0, hex.I, "I coordinate should be 0");
            Assert.AreEqual(0, hex.J, "J coordinate should be 0");
            Assert.AreEqual(0, hex.K, "K coordinate should be 0");
        }

        [TestMethod]
        public void TestHexLineFactory()
        {
            Hex hex = Hex.LineHex(1, 2);
            Assert.AreEqual(3, hex.I, "I coordinate should be 3");
            Assert.AreEqual(3, hex.J, "J coordinate should be 3");
            Assert.AreEqual(0, hex.K, "K coordinate should be 0");
            Assert.AreEqual(1, hex.LineI, "Line I should be 1");
            Assert.AreEqual(1, hex.LineJ, "Line J should be 1");
            Assert.AreEqual(2, hex.LineK, "Line K should be 2");
        }

        [TestMethod]
        public void TestLineCoordinates()
        {
            Hex hex = new Hex(3, 6);
            Assert.AreEqual(5, hex.LineI, "Line I should be 5");
            Assert.AreEqual(-1, hex.LineJ, "Line J should be -1");
            Assert.AreEqual(4, hex.LineK, "Line K should be 4");
            Assert.AreEqual("{I = 5, J = -1, K = 4}", hex.Lines(), "Line string should match");
        }

        [TestMethod]
        public void TestInLineChecks()
        {
            Hex hex1 = new Hex(3, 6);
            Hex hex2 = new Hex(0, 3);
            Assert.IsTrue(hex1.InLineI(5), "hex1 should be in I line 5");
            Assert.IsTrue(hex1.InLineJ(-1), "hex1 should be in J line -1");
            Assert.IsTrue(hex1.InLineK(4), "hex1 should be in K line 4");
            Assert.IsFalse(hex1.InLineI(hex2), "hex1 and hex2 not in same I line");
            Assert.IsTrue(hex1.InLineJ(hex2), "hex1 and hex2 in same J line");
            Assert.IsFalse(hex1.InLineK(hex2), "hex1 and hex2 not in same K line");
        }

        [TestMethod]
        public void TestAdjacency()
        {
            Hex hex1 = Hex.LineHex(0, 0);
            Hex hex2 = Hex.LineHex(0, 1); // Front I
            Hex hex3 = Hex.LineHex(1, 1); // Front J
            Hex hex4 = Hex.LineHex(1, 0); // Front K
            Assert.IsTrue(hex2.FrontI(hex1), "hex1 should be front I of hex2");
            Assert.IsTrue(hex3.FrontJ(hex1), "hex1 should be front J of hex3");
            Assert.IsTrue(hex4.FrontK(hex1), "hex1 should be front K of hex4");
            Assert.IsTrue(hex1.Adjacent(hex2), "hex1 should be adjacent to hex2");
            Assert.IsTrue(hex1.BackI(hex2), "hex2 should be back I of hex1");
        }

        [TestMethod]
        public void TestMovement()
        {
            Hex hex = new Hex(0, 0);
            hex.MoveI(1);
            Assert.AreEqual(2, hex.I, "I should be 2 after MoveI");
            Assert.AreEqual(-1, hex.K, "K should be -1 after MoveI");

            hex.MoveJ(1);
            Assert.AreEqual(3, hex.I, "I should be 3 after MoveJ");
            Assert.AreEqual(0, hex.K, "K should be 0 after MoveJ");

            hex.MoveK(1);
            Assert.AreEqual(2, hex.I, "I should be 2 after MoveK");
            Assert.AreEqual(2, hex.K, "K should be 2 after MoveK");
        }

        [TestMethod]
        public void TestShift()
        {
            Hex hex = new Hex(0, 0);
            Hex shiftedI = hex.ShiftI(1);
            Hex shiftedJ = hex.ShiftJ(1);
            Hex shiftedK = hex.ShiftK(1);

            Assert.AreEqual(2, shiftedI.I, "Shifted I should be 2");
            Assert.AreEqual(-1, shiftedI.K, "Shifted K should be -1");
            Assert.AreEqual(1, shiftedJ.I, "Shifted I should be 1");
            Assert.AreEqual(1, shiftedJ.K, "Shifted K should be 1");
            Assert.AreEqual(-1, shiftedK.I, "Shifted I should be -1");
            Assert.AreEqual(2, shiftedK.K, "Shifted K should be 2");
        }

        [TestMethod]
        public void TestAddSubtract()
        {
            Hex hex1 = new Hex(2, 3);
            Hex hex2 = new Hex(1, 1);
            Hex sum = hex1.Add(hex2);
            Hex diff = hex1.Subtract(hex2);

            Assert.AreEqual(3, sum.I, "Sum I should be 3");
            Assert.AreEqual(4, sum.K, "Sum K should be 4");
            Assert.AreEqual(1, diff.I, "Diff I should be 1");
            Assert.AreEqual(2, diff.K, "Diff K should be 2");
        }

        [TestMethod]
        public void TestRectangularConversion()
        {
            Hex hex = new Hex(3, 6);
            double halfSin60 = Math.Sqrt(3) / 4;
            Assert.AreEqual(halfSin60 * (3 + 6), hex.X, 0.0001, "X coordinate conversion incorrect");
            Assert.AreEqual((3 - 6) / 4.0, hex.Y, 0.0001, "Y coordinate conversion incorrect");
        }

        [TestMethod]
        public void TestInRange()
        {
            Hex hex = Hex.LineHex(1, 1);
            Assert.IsTrue(hex.InRange(2), "Hex should be in range 2");
            Assert.IsFalse(hex.InRange(1), "Hex should not be in range 1");
        }

        [TestMethod]
        public void TestEquals()
        {
            Hex hex1 = new Hex(2, 3);
            Hex hex2 = new Hex(2, 3);
            Hex hex3 = new Hex(3, 2);
            Assert.IsTrue(hex1.Equals(hex2), "Equal hexes should be equal");
            Assert.IsFalse(hex1.Equals(hex3), "Different hexes should not be equal");
        }

        [TestMethod]
        public void TestClone()
        {
            Hex hex = new Hex(2, 3);
            Hex cloned = (Hex) hex.Clone();
            Assert.AreEqual(hex.I, cloned.I, "Cloned I should match");
            Assert.AreEqual(hex.K, cloned.K, "Cloned K should match");
            Assert.AreNotSame(hex, cloned, "Cloned object should be different instance");
        }

        [TestMethod]
        public void TestToString()
        {
            Hex hex = new Hex(3, 6);
            string expected = $"Hex[raw = {{3, 9, 6}}, line = {{5, -1, 4}}, rect = {{ {Math.Sqrt(3) / 4 * 9}, {-3 / 4.0}}}]";
            Assert.AreEqual(expected, hex.ToString(), "ToString output should match");
        }
    }
}
