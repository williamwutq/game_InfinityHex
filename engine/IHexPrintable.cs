using System.Linq;
using System.Text;
using Hex;

namespace Core
{
    /// <summary>
    /// Defines methods for obtaining printable representations of a hexagon grid,
    /// including ASCII art and GUI rendering data. Implementations provide access
    /// to the grid's visual layout, block ordering, color information, and radius,
    /// supporting both text-based and graphical interfaces.
    /// </summary>
    public interface IHexPrintable
    {
        /// <summary>
        /// Converts the color index of <see cref="Hex.Block"/> into an ASCII art representation.
        /// </summary>
        /// <param name="index">The integer index representing the color of the block</param>
        /// <returns>The ASCII art character represeting the color of the block</returns>
        private static char ConvertColor(int index) => index switch
        {
            -2 => 'X', // Snake head
            -1 => 'O', // Unoccupied
            < 10 => (char)('0' + index), // Occupied with color 0-9
            < 36 => (char)('A' + index - 10), // Occupied with color A-Z
            < 62 => (char)('a' + index - 36), // Occupied with color a-z
            62 => '+', // Occupied with color +
            63 => '-', // Occupied with color -
            _ => '?' // Unknown color
        };
        /// <summary>
        /// Gets the ASCII art representation of the hexagon grid.
        /// </summary>
        /// <returns>A string containing the ASCII art representation.</returns>
        /// <remarks>
        /// This method is virtual and provides a default ASCII art rendering of the hexagon grid.
        /// Implementations can override this method to customize the output.
        /// </remarks>
        virtual string GetASCIIArt()
        {
            int radius = GetRadius();
            Block[] blocks = GetBlocks();
            int index = 0;
            StringBuilder sb = new ();
            for (int i = 0; i < radius; i++)
            {
                sb.Append(new string(' ', radius - i));
                for (int b = 0; b < i + radius; b++)
                {
                    Block block = blocks[index++];
                    sb.Append(ConvertColor(block.Color()));
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            for (int i = radius - 2; i >= 0; i--)
            {
                sb.Append(new string(' ', radius - i));
                for (int b = 0; b < i + radius; b++)
                {
                    Block block = blocks[index++];
                    sb.Append(ConvertColor(block.Color()));
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the ordered block array of the hexagon grid used for rendering.
        /// This method returns an array of <see cref="Hex.Block"/> objects representing the blocks
        /// sorted first by the I-line index and then by the K-line index.
        /// </summary>
        /// <returns>An array of <see cref="Hex.Block"/> objects representing the ordered blocks of the hexagon grid.</returns>
        abstract Block[] GetBlocks();

        /// <summary>
        /// Gets the colors of the blocks in the hexagon grid.
        /// This method returns an array of integers representing the color indices of each block.
        /// The colors are derived from blocks sorted by their I-line and K-line indices.
        /// </summary>
        /// <returns>An array of integers representing the color indices of each block in the hexagon grid.</returns>
        int[] GetColors()
        {
            return [.. GetBlocks().Select(block => block.Color())];
        }

        /// <summary>
        /// Gets the radius of the hexagon grid. The radius must match the radius of the returned block array.
        /// </summary>
        /// <returns>The radius of the hexagon grid.</returns>
        abstract int GetRadius();
    }
}