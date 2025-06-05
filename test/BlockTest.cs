using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hex;

namespace Hex.Tests
{
    [TestClass]
    public class BlockTest
    {
        [TestMethod]
        public void TestBasicConstructorWithColor()
        {
            Block block = new Block(2, 3, 1);
            Assert.AreEqual(2, block.I, "I coordinate should be 2");
            Assert.AreEqual(3, block.K, "K coordinate should be 3");
            Assert.AreEqual(1, block.Color(), "Color should be 1");
            Assert.IsFalse(block.State(), "State should be false (unoccupied)");
        }

        [TestMethod]
        public void TestBasicConstructorWithColorAndState()
        {
            Block block = new Block(1, 4, 3, true);
            Assert.AreEqual(1, block.I, "I coordinate should be 1");
            Assert.AreEqual(4, block.K, "K coordinate should be 4");
            Assert.AreEqual(3, block.Color(), "Color should be 3");
            Assert.IsTrue(block.State(), "State should be true (occupied)");
        }

        [TestMethod]
        public void TestHexConstructorWithColor()
        {
            Hex hex = new Hex(3, 2);
            Block block = new Block(hex, 4);
            Assert.AreEqual(3, block.I, "I coordinate should be 3");
            Assert.AreEqual(2, block.K, "K coordinate should be 2");
            Assert.AreEqual(4, block.Color(), "Color should be 4");
            Assert.IsFalse(block.State(), "State should be false (unoccupied)");
        }

        [TestMethod]
        public void TestHexConstructorWithColorAndState()
        {
            Hex hex = new Hex(0, 5);
            Block block = new Block(hex, 4, true);
            Assert.AreEqual(0, block.I, "I coordinate should be 0");
            Assert.AreEqual(5, block.K, "K coordinate should be 5");
            Assert.AreEqual(4, block.Color(), "Color should be 4");
            Assert.IsTrue(block.State(), "State should be true (occupied)");
        }

        [TestMethod]
        public void TestStaticLineBlockFactory()
        {
            Block block = Block.LineBlock(1, 2, 11);
            Assert.AreEqual(3, block.I, "I coordinate should be 3");
            Assert.AreEqual(0, block.K, "K coordinate should be 0");
            Assert.AreEqual(1, block.LineI, "Line I should be 1");
            Assert.AreEqual(2, block.LineK, "Line K should be 2");
            Assert.AreEqual(11, block.Color(), "Color should be 11");
            Assert.IsFalse(block.State(), "State should be false (unoccupied)");
        }

        [TestMethod]
        public void TestColorAndStateSetters()
        {
            Block block = new Block(0, 0, 5);
            block.SetColor(2);
            Assert.AreEqual(2, block.Color(), "Color should be 2 after SetColor");

            block.SetState(true);
            Assert.IsTrue(block.State(), "State should be true after SetState");

            block.ChangeState();
            Assert.IsFalse(block.State(), "State should be false after ChangeState");

            block.ChangeState();
            Assert.IsTrue(block.State(), "State should be true after second ChangeState");
        }

        [TestMethod]
        public void TestShiftMethods()
        {
            Block block = new Block(0, 0, 7);
            Block shiftedI = block.ShiftI(1);
            Assert.AreEqual(2, shiftedI.I, "Shifted I should be 2");
            Assert.AreEqual(-1, shiftedI.K, "Shifted K should be -1");
            Assert.AreEqual(7, shiftedI.Color(), "Color should remain 7");
            Assert.AreEqual(block.State(), shiftedI.State(), "State should remain unchanged");
            Assert.AreNotSame(block, shiftedI, "ShiftI should return new instance");

            Block shiftedJ = block.ShiftJ(1);
            Assert.AreEqual(3, shiftedJ.I, "Shifted I should be 3");
            Assert.AreEqual(0, shiftedJ.K, "Shifted K should be 0");
            Assert.AreNotSame(block, shiftedJ, "ShiftJ should return new instance");

            Block shiftedK = block.ShiftK(1);
            Assert.AreEqual(2, shiftedK.I, "Shifted I should be 2");
            Assert.AreEqual(2, shiftedK.K, "Shifted K should be 2");
            Assert.AreNotSame(block, shiftedK, "ShiftK should return new instance");
        }

        [TestMethod]
        public void TestAddAndSubtract()
        {
            Block block = Block.LineBlock(2, 3, 3);
            block.ChangeState();
            Hex other = new Hex(1, 1);

            Block added = block.Add(other);
            Assert.AreEqual(3, added.LineI, "Added I should be 3");
            Assert.AreEqual(4, added.LineK, "Added K should be 4");
            Assert.AreEqual(3, added.Color(), "Color should remain 3");
            Assert.IsTrue(added.State(), "State should remain true");

            // Note: There seems to be a bug in the Subtract method implementation
            // It incorrectly uses base.Add instead of base.Subtract
            Block subtracted = block.Subtract(other);
            Assert.AreEqual(1, subtracted.LineI, "Subtracted I should be 1");
            Assert.AreEqual(2, subtracted.LineK, "Subtracted K should be 2");
            Assert.AreEqual(3, subtracted.Color(), "Color should remain 3");
            Assert.IsTrue(subtracted.State(), "State should remain true");
        }

        [TestMethod]
        public void TestToString()
        {
            Block block = new Block(3, 6, 2, true);
            string expected = "Block[color = 2, coordinates = {5, -1, 4}, state = true]";
            Assert.AreEqual(expected, block.ToString(), "ToString output should match");
        }

        [TestMethod]
        public void TestClone()
        {
            Block block = new Block(2, -7, 5, true);
            Block cloned = block.Clone();
            Assert.AreEqual(block.I, cloned.I, "Cloned I should match");
            Assert.AreEqual(block.J, cloned.J, "Cloned J should match");
            Assert.AreEqual(block.K, cloned.K, "Cloned K should match");
            Assert.AreEqual(block.Color(), cloned.Color(), "Cloned color should match");
            Assert.AreEqual(block.State(), cloned.State(), "Cloned state should match");
            Assert.AreNotSame(block, cloned, "Cloned object should be different instance");
        }
    }
}
