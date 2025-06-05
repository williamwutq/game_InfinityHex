
namespace Hex
{
    /// <summary>
    /// From Java implementation of Block from the HappyHex project.
    /// <para>
    /// The <c>Block</c> class inherits from <see cref="Hex"/> and represents a colored block with an occupancy state
    /// within the hexagonal grid system.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>Block</c> inherits the coordinate system from <see cref="Hex"/>. Refer to <see cref="Hex"/> for details on the hexagonal coordinate
    /// system, including both raw and line-based coordinates.
    /// </para>
    /// <para>
    /// In addition to the coordinate functionality provided by <see cref="Hex"/>, each <c>Block</c> instance encapsulates:
    /// <list type="bullet">
    ///   <item>
    ///     <description>A color index indicating the block's color. -1 represents the typical empty color for the block,
    ///       -2 is the default filled color for the block, and 0-n represent the real colors generated.</description>
    ///   </item>
    ///   <item>
    ///     <description>A boolean state indicating whether the block is occupied (<c>true</c>) or unoccupied (<c>false</c>).</description>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// The class provides various constructors and static factory methods for creating blocks using either
    /// standard (i, k) coordinates or line indices. It also includes methods for moving, shifting, adding, and
    /// subtracting coordinates, as well as modifying and retrieving the block's state and color.
    /// </para>
    /// </remarks>
    /// <seealso cref="Hex"/>
    /// <since>0.1</since>
    /// <author>William Wu</author>
    /// <version>0.1</version>
    public class Block : Hex
    {
        private int color;
        private bool state;

        public Block(int i, int k) : base(i, k)
        {
            this.state = false;
            this.color = -1;
        }
        public Block(int i, int k, int color) : base(i, k)
        {
            this.state = false;
            this.color = color;
        }
        public Block(int i, int k, bool state) : base(i, k)
        {
            if (state)
            {
                this.color = -2;
            }
            else
            {
                this.color = -1;
            }
            this.state = state;
        }
        public Block(int i, int k, int color, bool state) : base(i, k)
        {
            this.state = state;
            this.color = color;
        }
        public Block(Hex hex, int color) : base(hex.I, hex.K)
        {
            this.state = false;
            this.color = color;
        }
        public Block(Hex hex, bool state) : base(hex.I, hex.K)
        {
            if (state)
            {
                this.color = -2;
            }
            else
            {
                this.color = -1;
            }
            this.state = state;
        }
        public Block(Hex hex, int color, bool state) : base(hex.I, hex.K)
        {
            this.state = state;
            this.color = color;
        }
        public static Block LineBlock(int i, int k, int color)
        {
            return new Block(Hex.LineHex(i, k), color);
        }
        public int Color()
        {
            return color;
        }
        public bool State()
        {
            return state;
        }
        public override string ToString()
        {
            return $"Block[color = {Color}, coordinates = {{{LineI}, {LineJ}, {LineK}}}, state = {State}]";
        }
        public string ToBasicString()
        {
            return $"{{{LineI}, {LineJ}, {LineK}, {State}}}";
        }
        public override Block Clone()
        {
            return new Block(this.I, this.K, this.color, this.state);
        }
        public void SetColor(int color)
        {
            this.color = color;
        }
        public void SetState(bool state)
        {
            this.state = state;
        }
        public void ChangeState()
        {
            this.state = !this.state;
        }
        public override Block ShiftI(int unit)
        {
            return new Block(base.ShiftI(unit), this.color, this.state);
        }
        public override Block ShiftJ(int unit)
        {
            return new Block(base.ShiftJ(unit), this.color, this.state);
        }
        public override Block ShiftK(int unit)
        {
            return new Block(base.ShiftK(unit), this.color, this.state);
        }
        public override Hex Add(Hex other)
        {
            return new Block(base.Add(other), this.color, this.state);
        }
        public override Hex Subtract(Hex other)
        {
            return new Block(base.Add(other), this.color, this.state);
        }
    }
}
