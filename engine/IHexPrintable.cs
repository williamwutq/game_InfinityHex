using System.Linq;
using Hex;

namespace Core
{
    public interface IHexPrintable
    {
        /// <summary>
        /// Gets the ASCII art representation of the hexagon grid.
        /// </summary>
        /// <returns>A string containing the ASCII art representation.</returns>
        string GetASCIIArt();

        /// <summary>
        /// Gets the ordered block array of the hexagon grid used for rendering.
        /// This method returns an array of <see cref="Hex.Block"/> objects representing the blocks
        /// sorted first by the I-line index and then by the K-line index.
        /// </summary>
        /// <returns>An array of <see cref="Hex.Block"/> objects representing the ordered blocks of the hexagon grid.</returns>
        Block[] GetBlocks();

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
        int GetRadius();
    }
}